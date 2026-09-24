using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Services;
using OfficeOpenXml;

// 真实 R37 IOM，替换服务端传输；不会连接 Aras 或工具箱数据库。
var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    assertions++;
}
var errors = new List<string>();
var operations = new List<string>();
var errorLog = DispatchProxy.Create<IErrorLogService, Stub>();
((Stub)(object)errorLog).Handler = (_, args) => { errors.Add((string)args[1]!); return Task.CompletedTask; };
var operationLog = DispatchProxy.Create<IOperationLogService, Stub>();
((Stub)(object)operationLog).Handler = (_, args) => { operations.Add((string)args[3]!); return Task.CompletedTask; };
var writes = 0;
var reads = 0;
var saved = "";
Action<XElement>? alterReadback = null;
Action? afterWrite = null;
const string targetId = "11111111111111111111111111111111";
var transport = DispatchProxy.Create<IServerConnection, Stub>();
((Stub)(object)transport).Handler = (method, args) =>
{
    if (method.Name != "CallAction") throw new InvalidOperationException(method.Name);
    var request = XDocument.Parse(((XmlDocument)args[1]!).OuterXml);
    var item = request.Descendants("Item").First();
    Check((string?)item.Attribute("type") == "ItemType" && (string?)item.Attribute("id") == targetId, "只访问指定 ItemType");
    if ((string?)item.Attribute("action") == "edit")
    {
        writes++;
        Check(item.Elements().Count() == 1 && item.Element("class_structure") != null, "汇入只写 class_structure");
        var property = item.Element("class_structure")!;
        Check(!property.Elements().Any(), "IOM 把类结构作为 Text 字符串提交，不能嵌套 AML 节点");
        saved = property.Value;
        afterWrite?.Invoke();
    }
    else reads++;
    var xml = XElement.Parse(saved);
    if ((string?)item.Attribute("action") != "edit") alterReadback?.Invoke(xml);
    var response = new XElement("Result", new XElement("Item", new XAttribute("type", "ItemType"),
        new XAttribute("id", targetId), new XElement("class_structure", xml.ToString(SaveOptions.DisableFormatting))));
    ((XmlDocument)args[2]!).LoadXml("<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'><SOAP-ENV:Body>" + response + "</SOAP-ENV:Body></SOAP-ENV:Envelope>");
    return null;
};
var connection = new ArasConnectionService();
connection.SetConnection(new ArasConnectionInfo(), new Innovator(transport), null!);
var service = new ClassStructureImportService(connection, operationLog, errorLog);
var target = new ClassStructureItemType { Id = targetId, Name = "Test" };
var directory = Path.Combine(Path.GetTempPath(), "class-structure-regression-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
string Template(string name, params string[][] paths)
{
    var file = Path.Combine(directory, name + ".xlsx");
    using var package = new ExcelPackage(new MemoryStream(service.GenerateTemplate()));
    var sheet = package.Workbook.Worksheets["类结构"];
    sheet.Cells[2, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column].Clear();
    for (var row = 0; row < paths.Length; row++)
    for (var level = 0; level < paths[row].Length; level++)
        sheet.Cells[row + 2, level + 2].Value = paths[row][level];
    package.SaveAs(new FileInfo(file));
    return file;
}
async Task Reject(Func<Task> action, string expected)
{
    var errorCount = errors.Count;
    try { await action(); throw new Exception("Expected rejection: " + expected); }
    catch (InvalidOperationException ex) { Check(ex.Message.Contains(expected), ex.Message); }
    Check(errors.Count > errorCount, "拒绝原因写入错误日志");
}
try
{
    var file = Template("slash", ["芯片/单管", "R&D <设计> \"样件\"", "叶节点"],
        ["芯片/单管", "R&D <设计> \"样件\"", "叶节点"], ["模块", "FRD"]);
    var preview = await service.AnalyzeTemplateAsync(file);
    Check(preview.PathCount == 2 && preview.NodeCount == 5 && preview.MaxDepth == 3 && preview.DuplicatePathCount == 1, "层级、共享前缀、重复路径统计");
    Check(preview.NormalizedCellCount == 2 && preview.TreeText.Contains("芯片／单管") && !preview.TreeText.Contains("芯片/单管"), "预览展示真实汇入名称");
    Check(preview.NormalizationMessage.Contains("芯片/单管 → 芯片／单管"), "明确展示名称变更");
    var result = await service.ImportAsync(file, target);
    var root = XElement.Parse(result.ClassStructureXml);
    Check((string?)root.Attribute("id") == targetId && root.Attribute("name") == null, "无名称根节点必须使用 ItemType ID");
    Check(root.Descendants("class").Any(n => (string?)n.Attribute("name") == "R&D <设计> \"样件\""), "保留非分隔符特殊字符并正确转义");
    Check(root.Descendants("class").All(n => !n.Attribute("name")!.Value.Contains('/')), "实际写入无非法半角斜杠");
    Check(root.DescendantsAndSelf().Select(n => n.Attribute("id")!.Value).Distinct().Count() == 6, "所有节点 GUID 唯一");
    Check(reads == 1 && operations.Count == 1 && result.NormalizedCellCount == preview.NormalizedCellCount, "回读、操作日志和结果一致");
    var firstIds = root.Descendants("class").Select(n => n.Attribute("id")!.Value).ToHashSet();
    var repeated = XElement.Parse((await service.ImportAsync(file, target)).ClassStructureXml);
    Check((string?)repeated.Attribute("id") == targetId && !repeated.Descendants("class").Any(n => firstIds.Contains(n.Attribute("id")!.Value)), "覆盖保留目标根 ID，子节点生成新 ID，支持修复旧异常结构");

    var before = writes;
    var conflict = Template("collision", ["芯片/单管"], ["芯片／单管"]);
    await Reject(async () => { await service.AnalyzeTemplateAsync(conflict); }, "第 3 行第1级");
    await Reject(async () => { await service.ImportAsync(conflict, target); }, "避免合并不同节点");
    var gap = Template("gap", ["一级", "", "三级"]);
    await Reject(async () => { await service.ImportAsync(gap, target); }, "层级中间存在空白");
    Check(writes == before, "非法模板在发送写请求之前拒绝");
    var branches = Template("different-parents", ["甲", "A/B"], ["乙", "A／B"]);
    Check((await service.AnalyzeTemplateAsync(branches)).NodeCount == 4, "不同父级相同名称合法");
    var tenLevels = Template("ten-levels", Enumerable.Range(1, 10).Select(i => "层" + i).ToArray());
    Check((await service.AnalyzeTemplateAsync(tenLevels)).MaxDepth == 10, "支持完整十级模板");

    alterReadback = xml => xml.SetAttributeValue("id", "22222222222222222222222222222222");
    await Reject(async () => { await service.ImportAsync(file, target); }, "根节点");
    alterReadback = xml => xml.Elements().First().SetAttributeValue("name", "芯片/单管");
    await Reject(async () => { await service.ImportAsync(file, target); }, "半角 /");
    alterReadback = xml => xml.Elements().First().SetAttributeValue("id", targetId);
    await Reject(async () => { await service.ImportAsync(file, target); }, "重复");
    alterReadback = xml => xml.Elements().First().SetAttributeValue("name", "服务器不同值");
    await Reject(async () => { await service.ImportAsync(file, target); }, "不一致");
    alterReadback = null;
    using var cancelled = new CancellationTokenSource();
    cancelled.Cancel();
    before = writes;
    try { await service.ImportAsync(file, target, cancelled.Token); throw new Exception("Expected cancellation"); }
    catch (OperationCanceledException) { Check(writes == before, "写入前取消不修改数据"); }
    using var afterSave = new CancellationTokenSource();
    afterWrite = () => afterSave.Cancel();
    var previousReads = reads;
    var previousLogs = operations.Count;
    await service.ImportAsync(file, target, afterSave.Token);
    Check(reads == previousReads + 1 && operations.Count == previousLogs + 1, "写入后取消仍完成回读和审计");
    Console.WriteLine($"PASS: {assertions} assertions; template -> preview -> real R37 IOM -> readback, failure and cancellation coverage.");
}
finally { Directory.Delete(directory, true); }

public class Stub : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
}
