using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using ArasToolkit.Services.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

// 真实 R37 IOM + 替换传输层，覆盖公开入口；不连接 Aras 或工具箱数据库。
XNamespace i18n = "http://www.aras.com/I18N";
var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    assertions++;
}
object? Invoke(Type type, string method, params object[] args)
{
    try { return type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args); }
    catch (TargetInvocationException ex) { throw ex.InnerException!; }
}
var errors = new List<string>();
var operations = new List<string>();
var failOperationLog = false;
var errorLog = DispatchProxy.Create<IErrorLogService, Proxy>();
((Proxy)(object)errorLog).Handler = (_, args) => { errors.Add((string)args[1]!); return Task.CompletedTask; };
var operationLog = DispatchProxy.Create<IOperationLogService, Proxy>();
((Proxy)(object)operationLog).Handler = (_, args) => {
    if (failOperationLog) throw new InvalidOperationException("Audit unavailable");
    operations.Add((string)args[0]!); return Task.CompletedTask;
};
ArasConnectionService Connection(Func<XElement, string> response)
{
    var transport = DispatchProxy.Create<IServerConnection, Proxy>();
    ((Proxy)(object)transport).Handler = (method, args) =>
    {
        if (method.Name != "CallAction") throw new InvalidOperationException("Unexpected IOM call " + method.Name);
        var request = XDocument.Parse(((XmlDocument)args[1]!).OuterXml);
        var aml = request.Descendants("AML").SingleOrDefault()
            ?? new XElement("AML", request.Descendants("Item").Where(e => !e.Ancestors("Item").Any()));
        var body = response(aml);
        ((XmlDocument)args[2]!).LoadXml("<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'><SOAP-ENV:Body>" + body + "</SOAP-ENV:Body></SOAP-ENV:Envelope>");
        return null;
    };
    var connection = new ArasConnectionService();
    connection.SetConnection(new ArasConnectionInfo(), new Innovator(transport), null!);
    return connection;
}
string Result(XElement item) => new XElement("Result", item).ToString(SaveOptions.DisableFormatting);
string Fault(string code) => $"<SOAP-ENV:Fault><faultcode>{code}</faultcode><faultstring>Test failure</faultstring></SOAP-ENV:Fault>";
const string objectId = "11111111111111111111111111111111";
const string mapId = "22222222222222222222222222222222";
string Id(int n) => n.ToString("X32");

var objectRow = new Dictionary<int, string> {
    [1] = "Test_'quoted'", [2] = "研发 & <零件>", [3] = "研發 & <零件>", [4] = "R&D <Part> 'quoted'",
    [5] = "零件列表", [6] = "零件清單", [7] = "Parts & Components", [8] = "1"
};
var relationRow = new Dictionary<int, string> {
    [1] = objectRow[1], [2] = "Test_Rel", [3] = "10", [4] = "关联 & 资料", [5] = "關聯 & 資料",
    [6] = "Related & Data", [7] = "3", [8] = "1", [9] = "Document"
};
foreach (var mode in new[] { "新增", "覆盖" })
{
    var aml = (string)Invoke(typeof(ObjectClassImportService), "BuildObjectClassAml", objectRow, mode)!;
    var item = XElement.Parse(aml).Element("Item")!;
    Check(item.Element("name")!.Value == objectRow[1], "对象类名称正确转义");
    Check(item.Element("label") == null && item.Elements(i18n + "label").Count() == 3 &&
        item.Elements(i18n + "label_plural").Count() == 3, "六个标签必须使用正确 i18n 命名空间");
    foreach (var (property, firstColumn) in new[] { ("label", 2), ("label_plural", 5) })
    foreach (var (language, offset) in new[] { ("zc", 0), ("zt", 1), ("en", 2) })
        Check(item.Elements(i18n + property).Single(e => (string?)e.Attribute(XNamespace.Xml + "lang") == language).Value
            == objectRow[firstColumn + offset], $"{mode} {property}[{language}] 保留原文及特殊字符");
    if (mode == "覆盖") Check(item.Attribute("where")!.Value == "ItemType.name='Test_''quoted'''", "覆盖名称转义 SQL 单引号");
    var rel = XElement.Parse((string)Invoke(typeof(ObjectClassImportService), "BuildRelationshipTypeAml", relationRow, mode)!).Element("Item")!;
    Check(rel.Elements(i18n + "label").Count() == 3 && rel.Element("source_id")!.Element("Item")!.Element("name")!.Value == objectRow[1], "关系类三语及来源名称");
}
var sparseRow = new Dictionary<int, string> { [1] = "Test", [2] = "仅简体" };
var sparse = XElement.Parse((string)Invoke(typeof(ObjectClassImportService), "BuildObjectClassAml", sparseRow, "覆盖")!);
Check(sparse.Descendants(i18n + "label").Count() == 1 && !sparse.Descendants(i18n + "label_plural").Any(), "空白译文不能擦除其他语言");

// 通过 GenerateTemplate -> Excel -> ImportAsync -> R37 回读覆盖真实列映射及失败统计。
var logs = new List<ObjectClassImportLog>();
var factory = new LogFactory(logs);
var savedItems = new Dictionary<string, XElement>();
var mismatch = false;
var importConnection = Connection(aml => {
    var item = aml.Element("Item")!;
    var key = item.Element("name")!.Value;
    if ((string?)item.Attribute("action") != "get")
    {
        savedItems[key] = new XElement(item);
        return Result(new XElement("Item", new XAttribute("type", item.Attribute("type")!.Value), new XAttribute("id", objectId)));
    }
    Check((string?)item.Attribute("language") == "en,zc,zt", "回读明确请求所有语言");
    var saved = new XElement(savedItems[key]); saved.SetAttributeValue("id", objectId);
    if (mismatch) {
        saved.Elements(i18n + "label_plural").Where(e => (string?)e.Attribute(XNamespace.Xml + "lang") == "zt").Remove();
        // 会话回退值即使相同，也不能误判为繁体已写入。
        saved.Add(new XElement("label_plural", new XAttribute(XNamespace.Xml + "lang", "zt"), objectRow[6]));
    }
    return Result(saved);
});
var import = new ObjectClassImportService(factory, importConnection, operationLog, errorLog);
var testDirectory = Path.Combine(Path.GetTempPath(), "objectclass-regression-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(testDirectory);
try
{
    var file = Path.Combine(testDirectory, "multilingual.xlsx");
    using (var package = new ExcelPackage(new MemoryStream(import.GenerateTemplate())))
    {
        foreach (var (sheet, row) in new[] { ("对象类新增", objectRow), ("关系类新增", relationRow) })
        foreach (var cell in row) package.Workbook.Worksheets[sheet].Cells[2, cell.Key].Value = cell.Value;
        package.SaveAs(new FileInfo(file));
    }
    var success = await import.ImportAsync(file);
    Check(success.IsSuccess && success.Sheet1Count == 1 && success.Sheet2Count == 1, "完整模板汇入并逐语言核验成功");
    mismatch = true;
    var failed = await import.ImportAsync(file);
    Check(!failed.IsSuccess && failed.Sheet1Count == 0 && failed.Sheet2Count == 1 && failed.FailedDetails.Single().Contains("label_plural[zt]"), "漏写繁体标签必须标记该行失败");
    Check(logs.Last().Status == ObjectClassImportLog.StatusFailed && logs.Last().ErrorLog!.Contains("label_plural[zt]") && errors.Count > 0, "失败状态、明细与错误日志同步");
    mismatch = false;
    failOperationLog = true;
    var auditFailed = await import.ImportAsync(file);
    Check(auditFailed.IsSuccess && errors.Contains("Audit unavailable"), "敏感操作日志失败不能阻止汇入结果，且必须记录异常");
    failOperationLog = false;
}
finally { Directory.Delete(testDirectory, true); }

// 公开快速设置入口：新建与既有生命周期都要处理多语系和退回路径。
var stateNames = new[] { "Preliminary", "In Review", "Released", "Cancel" };
var stateIds = stateNames.Select((name, index) => (name, id: Id(index + 10))).ToDictionary(p => p.name, p => p.id);
async Task<XElement?> Configure(bool existing, string segments = "", bool failQuery = false)
{
    XElement? write = null;
    var connection = Connection(aml => {
        if (aml.Elements("Item").Any(i => (string?)i.Attribute("action") != "get"))
        {
            write = new XElement(aml);
            return "<Result>OK</Result>";
        }
        var item = aml.Element("Item")!;
        var type = item.Attribute("type")!.Value;
        var name = (string?)item.Element("name") ?? "";
        if (type == "Identity") return Result(new XElement("Item", new XAttribute("type", type), new XAttribute("id", Id(3)), new XElement("name", name)));
        if (type == "Life Cycle Map" && existing) return Result(new XElement("Item", new XAttribute("type", type), new XAttribute("id", mapId)));
        if (type == "Life Cycle State") return Result(new XElement("Item", new XAttribute("type", type), new XAttribute("id", stateIds[name]),
            new XElement("x", name == "Preliminary" ? "160" : "380"), new XElement("y", "200")));
        if (type == "Life Cycle Transition")
        {
            Check((string?)item.Element("source_id") == mapId && (string?)item.Element("from_state") == stateIds["In Review"] &&
                (string?)item.Element("to_state") == stateIds["Preliminary"], "退回路径查询限定当前生命周期及方向");
            return failQuery ? Fault("1") : Result(new XElement("Item", new XAttribute("type", type), new XAttribute("id", Id(30)), new XElement("segments", segments)));
        }
        return "<Result/>";
    });
    var service = new ObjectClassConfigurationService(connection, operationLog, errorLog);
    var configured = await service.ConfigureAsync([new() { Id = objectId, Name = "RegressionType" }],
        new() { ConfigureLifecycle = true, ConfigureCanAdd = false, ConfigureDefaultPermission = false }, new());
    Check(configured.SuccessCount == (failQuery ? 0 : 1), "快速设置成功/失败统计");
    return write;
}
var created = (await Configure(false))!;
var states = created.Elements("Item").Where(i => (string?)i.Attribute("type") == "Life Cycle State").ToList();
Check(states.Count == 4 && states.All(s => s.Elements(i18n + "label").Count() == 3 && s.Element("label") == null), "四状态均写入简繁英标签");
foreach (var state in states)
    Check(state.Elements(i18n + "label").Single(e => (string?)e.Attribute(XNamespace.Xml + "lang") == "en").Value == state.Element("name")!.Value, "英文状态名称固定写入 en");
var createdIds = states.ToDictionary(s => s.Element("name")!.Value, s => s.Attribute("id")!.Value);
var transitions = created.Elements("Item").Where(i => (string?)i.Attribute("type") == "Life Cycle Transition").ToList();
Check(transitions.Count == 5, "标准生命周期仍有五条转换");
var backward = transitions.Single(i => (string?)i.Element("from_state") == createdIds["In Review"] && (string?)i.Element("to_state") == createdIds["Preliminary"]);
Check((string?)backward.Element("segments") == "320,40|100,40" && transitions.Count(i => i.Element("segments") != null) == 1, "仅退回线增加上方两个转折点");
var repaired = (await Configure(true))!;
Check(repaired.Elements("Item").Where(i => (string?)i.Attribute("type") == "Life Cycle State").All(s => s.Elements(i18n + "label").Count() == 3 && s.Element("x") == null), "再次设置补齐翻译并保留已有节点布局");
Check(repaired.Elements("Item").Single(i => (string?)i.Attribute("type") == "Life Cycle Transition").Element("segments")!.Value == "380,120|160,120", "历史退回线按实际节点坐标修复");
var custom = (await Configure(true, "300,20|100,20"))!;
Check(!custom.Elements("Item").Any(i => (string?)i.Attribute("type") == "Life Cycle Transition"), "保留用户已设置的转折点且不重复创建转换");
Check(await Configure(true, failQuery: true) == null, "查询路径失败不能提交不完整配置");
Check((string)Invoke(typeof(ObjectClassConfigurationService), "BuildReturnSegments", 100, 300, 100, 120)! == "20,300|20,120", "纵向状态退回从侧面绕行");
Check((string)Invoke(typeof(ObjectClassConfigurationService), "BuildReturnSegments", 300, 20, 100, 20)! == "300,100|100,100", "节点靠近画布顶部时折线保持可见");
Check(operations.Count > 0, "写操作记录敏感操作日志");
Console.WriteLine($"PASS: {assertions} object class multilingual/lifecycle regression assertions.");

public class Proxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args!);
}
sealed class LogFactory(List<ObjectClassImportLog> logs) : IDbContextFactory<ArasToolkitDbContext>
{
    public ArasToolkitDbContext CreateDbContext() => new LogContext(logs);
    public Task<ArasToolkitDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) => Task.FromResult(CreateDbContext());
}
sealed class LogContext(List<ObjectClassImportLog> logs) : ArasToolkitDbContext(
    new DbContextOptionsBuilder<ArasToolkitDbContext>().UseSqlServer("Server=(local);Database=Unused;Integrated Security=True").Options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        logs.AddRange(ChangeTracker.Entries<ObjectClassImportLog>().Select(e => e.Entity));
        return Task.FromResult(1);
    }
}
