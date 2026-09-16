using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Services;

var passed = 0;
void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
void Pass(string name) { Console.WriteLine("PASS " + name); passed++; }
async Task Fails(Func<Task> action, string message)
{ try { await action(); } catch { return; } throw new Exception(message); }

var server = new FakeAras();
var service = server.Service;
var metadata = await service.GetEditorMetadataAsync();
Check(metadata.FieldTypes.Count() == 10 && metadata.ContainerProperty == "container", "Options must come from metadata");
Pass("控件类型、容器配置来自服务器元数据");
var itemTypes = await service.GetItemTypesAsync();
var forms = await service.GetFormsAsync(FakeAras.ItemTypeId);
Check(itemTypes.Count == 1 && forms.Count == 1 && forms[0].Views.Count == 2, "Views must be deduplicated without losing uses");
Pass("对象类 → View → Form 去重并保留用途");
var original = await service.GetDefinitionAsync(FakeAras.ItemTypeId, FakeAras.FormId);
Check(original.Fields.Count() == 3 && original.Fields.Any(field => field.Get("is_visible") == "0"), "All fields must load");
Check(original.SharedViews.Count == 2 && original.Properties.Count == 1, "Shared views/properties missing");
Check(service.BuildChangeSet(original, metadata).Changes.Count == 0 && server.Writes.Count == 0, "Read/preview must not write");
Pass("读取隐藏控件、HTML、共享引用，零修改不写入");

var session = new FormEditorSession(original.Clone(), metadata);
var fields = session.Fields.ToArray();
session.Select([fields[1].Id, fields[0].Id]);
var oldX = fields[0].Get("x"); var anchorY = fields[1].Get("y");
session.AlignY();
Check(session.Selected.Last().Get("y") == anchorY && session.Selected.Last().Get("x") == oldX, "Align must only change Y");
session.Undo(); Check(!session.IsDirty, "Undo must restore clean baseline");
session.Redo(); Check(session.IsDirty, "Redo must restore alignment");
Pass("首选控件为基准只对齐 Y，撤销/重做恢复脏状态");
session.DiscardChanges();
session.Select([fields[0].Id]); session.BeginEdit();
session.Move(new Dictionary<string,(int,int)> { [fields[0].Id] = (310, 160) });
session.Move(new Dictionary<string,(int,int)> { [fields[0].Id] = (320, 170) }); session.EndEdit();
session.Undo(); Check(!session.IsDirty, "A whole gesture is one undo"); session.Redo();
var delta = service.BuildChangeSet(session.Document, metadata);
var update = delta.Changes.Single();
Check(update.Properties.Keys.Order().SequenceEqual(new[] {"x","y"}), "Drag must only write x/y");
Check(!delta.Aml.Contains("delete") && !delta.Aml.Contains("custom_property") && !delta.Aml.Contains("html_code"), "Must not reconstruct bodies or overwrite unrelated data");
Pass("拖动为一次撤销，增量 AML 仅包含 X/Y");
var saved = await service.SaveAsync(session.Document, metadata);
Check(saved.Fields.First().Get("x") == "320", "Coordinates not saved");
Check(server.Form.ToString().Contains("keep_event") && server.Form.ToString().Contains("keep_unknown") && server.Form.ToString().Contains("保留翻译") && server.Form.Descendants("html_code").Single().Value.Contains("<div"), "Preserved values lost");
Check(server.Locks == 1 && server.Unlocks == 1 && server.Logs.Count == 1, "Own lock and audit missing");
Pass("保存保留 ID、事件、多语言、未知属性与 HTML，并释放本次锁");

session = new FormEditorSession(await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId),metadata);
var field = session.Fields.First(); field.Set("font_color", "#123abc");
Check(service.BuildChangeSet(session.Document,metadata).Changes.Single().Properties["font_color"]=="#123abc","Custom color normalized");
field.Set("font_color",field.Original["font_color"]); Check(!session.IsDirty,"Restoring custom color should clean draft");
field.Set("label",null);
Check(service.BuildChangeSet(session.Document,metadata).Aml.Contains("is_null=\"1\""),"Null must be explicit");
Pass("自定义颜色原样保留，清空属性使用 is_null");
field.Changes.Clear(); field.Set("injected_property","bad");
await Fails(()=>Task.FromResult(service.BuildChangeSet(session.Document,metadata)),"Unknown properties must be blocked");
field.Changes.Clear();field.Set("name","renamed");
await Fails(()=>Task.FromResult(service.BuildChangeSet(session.Document,metadata)),"Existing names must be immutable");
Pass("阻止未验证配置与已有名称修改");

session.DiscardChanges(); field=session.Fields.First(); field.Set("positioning","static");
session.Select([field.Id]); Check(!session.CanMoveSelection,"Static fields must not drag");
session.DiscardChanges();
var group=session.Add("groupbox",null,20,20);
session.Fields.First(item=>item.Id!=group.Id).Set("container",group.Name);
session.Select([group.Id]);
await Fails(()=>Task.FromResult(session.DeleteSelection(false)),"Orphaned group deletion must fail");
var deleted=session.DeleteSelection(true); Check(deleted.Count==2,"Group deletion must include descendants");
session.Undo();Check(session.Fields.Any(item=>item.Id==group.Id),"Undo deletion lost group");
session.Select([group.Id,session.Fields.First(item=>item.Get("container")==group.Name).Id]);
Check(!session.CanMoveSelection,"Different parent spaces must not move together");
Pass("静态控件与跨容器移动限制，整组删除和撤销");

server=new FakeAras(); service=server.Service;metadata=await service.GetEditorMetadataAsync();
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);
original.Fields.First().Set("x","99");server.Form.Descendants("custom_property").Single().Value="changed_by_other";
await Fails(()=>service.SaveAsync(original,metadata),"Concurrent change must block");
Check(server.Writes.Count==0,"Conflict must not acquire a lock or edit");Pass("完整配置冲突检查在写入前阻止覆盖");
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);
original.Fields.First().Set("x","99");server.Form.SetElementValue("locked_by_id",new string('E',32));
await Fails(()=>service.SaveAsync(original,metadata),"Other lock must block");Check(server.Writes.Count==0,"Must not steal locks");
Pass("他人锁定不抢锁");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
server.Form.SetElementValue("locked_by_id",FakeAras.UserId);
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);original.Fields.First().Set("x","99");
await service.SaveAsync(original,metadata);Check(server.Locks==0&&server.Unlocks==0&&(string?)server.Form.Element("locked_by_id")==FakeAras.UserId,"Preexisting user lock must remain");
Check((string?)XElement.Parse(service.BuildChangeSet(original,metadata).Aml).Element("Item")?.Attribute("action")=="update","Root Form must not use auto-unlocking edit action");
Pass("保留用户原有锁");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
session=new(await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId),metadata);
var added=session.Add("label",null,150,250); server.LoseResponse=true;
saved=await service.SaveAsync(session.Document,metadata);
Check(saved.Fields.Count(field=>field.Id==added.Id)==1&&server.Mutations==1,"Lost response must not duplicate additions");
await service.SaveAsync(session.Document,metadata);
Check(server.Mutations==1,"Retry should reconcile fixed IDs before writing");
Check(server.Logs.Count == 1, "Recovered write must be audited exactly once");
Pass("响应丢失后核对固定新增 ID，重复保存不重复新增");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
session=new(await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId),metadata);
session.Fields.First().Set("x","88");session.Add("button",null,20,300);server.FailBatch=true;
var before=server.Form.ToString();
await Fails(()=>service.SaveAsync(session.Document,metadata),"Failed batch must fail");
Check(server.Form.ToString()==before&&server.Unlocks==1&&session.IsDirty,"Failed batch must preserve draft/rollback/release own lock");
Pass("单条操作失败时整批回滚，保留草稿并释放本次锁（模拟事务）");

server=new FakeAras();server.Form.Element("Relationships")!.Elements("Item").Where(item=>(string?)item.Attribute("type")=="Body").Remove();
service=server.Service;metadata=await service.GetEditorMetadataAsync();
session=new(await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId),metadata);
session.Add("label",null,0,0);saved=await service.SaveAsync(session.Document,metadata);
Check(saved.Bodies.Count()==1&&saved.Fields.Count()==1,"Missing Body must be created with field");
Pass("空窗体一次保存新增 Body 和 Field");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);
var secondBody=original.Form.Children.Single(item=>item.Type=="Body").Clone();secondBody.Id=new string('F',32);
foreach(var item in secondBody.Children)item.Id=Guid.NewGuid().ToString("N").ToUpperInvariant();
original.Form.Children.Add(secondBody);session=new(original,metadata);
session.BodyId=secondBody.Id;session.Select([secondBody.Children[0].Id]);session.AlignY();
Check(session.Fields.First().Id==secondBody.Children[0].Id&&session.Document.Bodies.First().Children.First().Id!=session.Fields.First().Id,"Bodies mixed");
Pass("多 Body 独立选择与坐标空间");
session.Document.Form.Set("classification","Responsive");
await Fails(()=>Task.FromResult(service.BuildChangeSet(session.Document,metadata)),"Responsive form must not save");
Pass("响应式窗体禁止写入");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);original.Fields.First().Set("x","101");
var transport=server.Connection.InnovatorInstance!;
server.Connection.SetConnection(new ArasConnectionInfo{Url="https://example.invalid/Innovator",Database="Regression",Username="tester"},transport,null!);
saved=await service.SaveAsync(original,metadata);
Check(saved.Fields.First().Get("x")=="101"&&saved.ConnectionKey!=original.ConnectionKey,"Same-target reconnect must revalidate and save");
Pass("相同系统与账号重连后重新验证元数据和原始快照");
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);original.Fields.First().Set("x","102");
server.Connection.SetConnection(new ArasConnectionInfo{Url="https://example.invalid/Innovator",Database="OtherDatabase",Username="tester"},transport,null!);
var previousWrites=server.Writes.Count;
await Fails(()=>service.SaveAsync(original,metadata),"Different target must block");
Check(server.Writes.Count==previousWrites,"Different target must never receive old draft");
Pass("切换数据库不会把旧草稿写入新目标");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);original.Form.Children.Clear();session=new(original,metadata);
session.Add("label",null,0,0);session.DeleteSelection(false);
Check(!session.IsDirty&&!session.Document.Bodies.Any(),"Add-delete must not leave an empty new Body");
Pass("未保存的新增再删除恢复零变更，不残留空 Body");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);original.Fields.First().Set("x","99");
server.Connection.Disconnect();
await Fails(()=>service.SaveAsync(original,metadata),"Disconnected session must not write");
Check(server.Writes.Count==0,"Connection mismatch must not write");Pass("连接失效保留草稿并阻止写入");

var build=typeof(FormConfigurationService).GetMethod("BuildAddFormAml",BindingFlags.Static|BindingFlags.NonPublic)!;
XElement Generated(int? height)=> (XElement)build.Invoke(null,[new ArasFormConfigurationRequest
{ FormHeight=height,FormName="Test",FormLabel="Test",Fields=[new ArasFormFieldLayout{PropertyId=FakeAras.PropertyId,Name="name",Y=50}]}])!;
Check(Generated(350).Descendants("height").Single().Value=="350","Height350 missing");
Check(Generated(600).Descendants("height").Single().Value=="600","Custom height missing");
Check(Generated(null).Descendants("height").Single().Value=="150","Historical auto height changed");
Check(Generated(350).Descendants("html_code").Single().Value==Generated(600).Descendants("html_code").Single().Value,"Border height must be independent");
Pass("原功能350/自定义/历史自动高度与HTML边框独立");

server=new FakeAras();service=server.Service;metadata=await service.GetEditorMetadataAsync();
metadata.Field.Remove("form");metadata.Field.Remove("image");
original=await service.GetDefinitionAsync(FakeAras.ItemTypeId,FakeAras.FormId);
var nativeItemProperty=new FormEditorProperty{Id=new string('9',32),Name="owner",DataType="item",DataSource=FakeAras.ItemTypeId};
original.Properties.Add(nativeItemProperty);session=new(original,metadata);
added=session.Add("nested form",nativeItemProperty,0,0);
Check(service.BuildChangeSet(session.Document,metadata).Aml.Contains(nativeItemProperty.Id)&&added.Get("form")=="","Native nested form must use Item binding without invented form property");
Pass("BL 标准嵌套窗体使用 Item 属性绑定，不生成虚构来源字段");
session.DiscardChanges();session.Add("nested form",null,0,0);
await Fails(()=>Task.FromResult(service.BuildChangeSet(session.Document,metadata)),"Native nested form requires Item binding");
Pass("标准系统阻止无绑定的嵌套窗体");
session.DiscardChanges();metadata.Field["name"].StoredLength=12;
var longProperty=new FormEditorProperty{Id=new string('8',32),Name="very_long_property_name",DataType="string"};original.Properties.Add(longProperty);
var n1=session.Add("text",longProperty,0,0);var n2=session.Add("text",longProperty,10,10);
Check(n1.Name.Length<=12&&n2.Name.Length<=12&&n1.Name!=n2.Name,"Names must respect stored_length and uniqueness");
Pass("新增名称按目标长度截取并保持唯一");
var richLayout = new ArasFormFieldLayout { PropertyId = FakeAras.PropertyId, Name = "rich", FieldType = "formatted text", TextAreaRows = 180, TextAreaColumns = 460, IsDisabled = true };
var dimensionNotifications = new List<string?>();
richLayout.PropertyChanged += (_, e) => dimensionNotifications.Add(e.PropertyName);
richLayout.FieldType = "text";
Check(!richLayout.SupportsTextAreaDimensions && dimensionNotifications.Contains(nameof(ArasFormFieldLayout.SupportsTextAreaDimensions)), "Type changes must refresh dimension editors");
richLayout.FieldType = "formatted text";
var richAml = (XElement)build.Invoke(null, [new ArasFormConfigurationRequest { FormName = "RichTest", Fields = [richLayout] }])!;
var richField = richAml.Descendants("Item").Single(item => item.Element("name")?.Value == "rich");
Check(richField.Element("textarea_rows")?.Value == "180" && richField.Element("textarea_cols")?.Value == "460" && richField.Element("is_disabled")?.Value == "1", "Rich dimensions / disabled state must be serialized");
richLayout.FieldType = "text"; richLayout.IsDisabled = false;
var textAml = (XElement)build.Invoke(null, [new ArasFormConfigurationRequest { FormName = "TextTest", Fields = [richLayout] }])!;
var textField = textAml.Descendants("Item").Single(item => item.Element("name")?.Value == "rich");
Check(textField.Element("textarea_rows") == null && textField.Element("textarea_cols") == null && textField.Element("is_disabled")?.Value == "0", "Plain text must omit rich dimensions and retain editable state");
Check(ArasFormConfigurationOptions.SupportsTextAreaDimensions("textarea"), "Text Area support must remain");
Pass("富文本行列写入、控件切换通知与不可编辑双向状态");
Console.WriteLine($"All {passed} form editor regression cases passed.");

public class Proxy : DispatchProxy
{
    public Func<MethodInfo,object?[],object?> Handler {get;set;}=null!;
    protected override object? Invoke(MethodInfo? method,object?[]? args)=>Handler(method!,args!);
}
public sealed class FakeAras
{
    public const string ItemTypeId="11111111111111111111111111111111",FormId="22222222222222222222222222222222",BodyId="33333333333333333333333333333333",PropertyId="44444444444444444444444444444444",UserId="99999999999999999999999999999999";
    public XElement Form;
    public ArasConnectionService Connection {get;}=new();
    public FormConfigurationEditService Service {get;}
    public List<string> Writes {get;}=[];
    public List<string> Logs {get;}=[];
    public int Locks,Unlocks,Mutations;
    public bool LoseResponse,FailBatch;
    public FakeAras()
    {
        Form=XElement.Parse($"""
        <Item type="Form" id="{FormId}" xmlns:i18n="http://www.aras.com/I18N"><name>Regression Form</name><classification>Classic</classification><width>850</width><height>410</height><modified_on>2026-09-01</modified_on><Relationships>
        <Item type="Body" id="{BodyId}"><css>/* preserved body CSS */</css><Relationships>
        <Item type="Field" id="55555555555555555555555555555555"><name>name</name><label>Name</label><i18n:label xml:lang="zh">保留翻译</i18n:label><field_type>text</field_type><propertytype_id>{PropertyId}</propertytype_id><x>50</x><y>50</y><font_color>#234abc</font_color><custom_property>keep_unknown</custom_property><Relationships><Item type="Field Event" id="AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"><name>keep_event</name></Item></Relationships></Item>
        <Item type="Field" id="66666666666666666666666666666666"><name>hidden</name><label>隐藏</label><field_type>label</field_type><is_visible>0</is_visible><x>250</x><y>120</y></Item>
        <Item type="Field" id="77777777777777777777777777777777"><name>width_HTML</name><field_type>html</field_type><x>10</x><y>20</y><html_code>&lt;div style="height:200px;width:830px;border:1px solid"&gt; &lt;/div&gt;</html_code></Item>
        </Relationships></Item><Item type="Form Event" id="BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"><name>keep_form_event</name></Item></Relationships></Item>
        """);
        var transport=DispatchProxy.Create<IServerConnection,Proxy>();
        ((Proxy)(object)transport).Handler=(method,args)=>
        {
            if(method.Name!="CallAction")return method.ReturnType==typeof(string)?UserId:method.ReturnType==typeof(bool)?false:null;
            var request=XDocument.Parse(((XmlDocument)args[1]!).OuterXml);
            var items=request.Root!.Name.LocalName=="AML"?request.Root.Elements("Item").ToList():request.Descendants("AML").Elements("Item").ToList();
            if(items.Count==0) items=request.Descendants("Item").Where(item=>!item.Ancestors("Item").Any()).ToList();
            string response;
            try
            {
                if(items.All(item=>(string?)item.Attribute("action")=="get"))response="<Result>"+string.Concat(items.Select(Query))+"</Result>";
                else
                {
                    var root=items.Single();var action=(string?)root.Attribute("action");Writes.Add(root.ToString());
                    if(action=="lock"){Locks++;Form.SetElementValue("locked_by_id",UserId);}
                    else if(action=="unlock"){Unlocks++;Form.Element("locked_by_id")?.Remove();}
                    else
                    {
                        var next=new XElement(Form);Apply(next,root);if((string?)root.Attribute("action")=="edit")next.Element("locked_by_id")?.Remove();if(FailBatch)throw new Exception("Deliberate transaction failure");
                        Form=next;Mutations++;if(LoseResponse){LoseResponse=false;throw new IOException("Response lost after commit");}
                    }
                    response="<Result>"+Form+"</Result>";
                }
            }
            catch(Exception ex){response=$"<SOAP-ENV:Fault><faultcode>1</faultcode><faultstring>{System.Security.SecurityElement.Escape(ex.Message)}</faultstring></SOAP-ENV:Fault>";}
            ((XmlDocument)args[2]!).LoadXml($"<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body>{response}</SOAP-ENV:Body></SOAP-ENV:Envelope>");
            return null;
        };
        Connection.SetConnection(new ArasConnectionInfo{Url="https://example.invalid/Innovator",Database="Regression",Username="tester"},new Innovator(transport),null!);
        var errors=DispatchProxy.Create<IErrorLogService,Proxy>();((Proxy)(object)errors).Handler=(_,_)=>Task.CompletedTask;
        var logs=DispatchProxy.Create<IOperationLogService,Proxy>();((Proxy)(object)logs).Handler=(_,args)=>{Logs.Add(string.Join("|",args));return Task.CompletedTask;};
        Service=new(Connection,errors,logs);
    }
    private string Query(XElement query)
    {
        if (Environment.GetEnvironmentVariable("FORM_EDITOR_TRACE") == "1") Console.WriteLine(query.ToString(SaveOptions.DisableFormatting));
        var type=(string?)query.Attribute("type");
        if(type=="Form")return Form.ToString();
        if(type=="ItemType")
        {
            var name=(string?)query.Element("name");
            if(name!=null)
            {
                var props=name=="Field"?FormEditorRules.CommonFieldProperties.Concat(new[]{"container","form","image"}):name=="Form"?new[]{"label","width","height"}:new[]{"css"};
                return new XElement("Item",new XAttribute("type","ItemType"),new XAttribute("id",ItemTypeId),new XElement("name",name),new XElement("Relationships",props.Select(property=>new XElement("Item",new XAttribute("type","Property"),new XElement("name",property),new XElement("data_type",property=="field_type"?"list":new[]{"x","y","width","height","display_length","textarea_rows","textarea_cols"}.Contains(property)?"integer":property is "form" or "propertytype_id"?"item":"string"),new XElement("data_source",property=="field_type"?new string('C',32):""))))).ToString();
            }
            return $"<Item type=\"ItemType\" id=\"{ItemTypeId}\"><name>RegressionType</name><Relationships>{View("88888888888888888888888888888888","default")}{View("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD","edit")}</Relationships></Item>";
        }
        if(type=="View")return View("88888888888888888888888888888888","default")+View("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD","edit");
        if(type=="Property")return $"<Item type=\"Property\" id=\"{PropertyId}\"><name>name</name><label>Name</label><data_type>string</data_type></Item>";
        if(type=="Value")return string.Concat(new[]{"text","label","button","html","image","groupbox","nested form","textarea","checkbox","item"}.Select(value=>$"<Item type=\"Value\" id=\"{Guid.NewGuid():N}\"><value>{value}</value><label>{value}</label></Item>"));
        throw new Exception("Unexpected query "+type);
    }
    private static string View(string id,string usage)=>$"<Item type=\"View\" id=\"{id}\"><source_id keyed_name=\"RegressionType\">{ItemTypeId}</source_id><related_id><Item type=\"Form\" id=\"{FormId}\"><name>Regression Form</name><classification>Classic</classification></Item></related_id><type>{usage}</type></Item>";
    private static void Apply(XElement target,XElement delta)
    {
        foreach(var property in delta.Elements().Where(property=>property.Name.LocalName!="Relationships"))
        {target.Element(property.Name)?.Remove();target.Add(new XElement(property));}
        foreach(var change in delta.Element("Relationships")?.Elements("Item")??[])
        {
            var relationships=target.Element("Relationships");if(relationships==null){relationships=new XElement("Relationships");target.Add(relationships);}
            var existing=relationships.Elements("Item").FirstOrDefault(item=>(string?)item.Attribute("id")== (string?)change.Attribute("id"));
            var action=(string?)change.Attribute("action");
            if(action=="delete")existing?.Remove();
            else if(action=="add")
            {var added=new XElement("Item",new XAttribute("type",(string)change.Attribute("type")!),new XAttribute("id",(string)change.Attribute("id")!));relationships.Add(added);Apply(added,change);}
            else if(existing!=null)Apply(existing,change);else throw new Exception("Missing edit target");
        }
    }
}
