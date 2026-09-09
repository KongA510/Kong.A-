using System.Collections.Concurrent;
using System.Reflection;
using System.Xml;
using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Services;
using OfficeOpenXml;

// Exercise the real import loop and R37 response parser with an in-memory transport.
// No login credentials, live Aras writes or toolkit database connection are used.
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
var directory = Path.Combine(Path.GetTempPath(), "data-import-regression-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
var file = Path.Combine(directory, "input.xlsx");
using (var package = new ExcelPackage())
{
    var sheet = package.Workbook.Worksheets.Add("Data");
    sheet.Cells[1, 1].Value = "name";
    sheet.Cells[2, 1].Value = "first";
    sheet.Cells[4, 1].Value = "last";
    package.SaveAs(new FileInfo(file));
}
const string template = "<AML><Item type='Part' action='add'><name>@A</name></Item></AML>";
int calls = 0;
bool fail = false, auditFails = false;
bool failLast = false;
var submittedNames = new ConcurrentBag<string>();
var messages = new ConcurrentBag<string>();
var audits = new ConcurrentBag<string>();
var transport = DispatchProxy.Create<IServerConnection, TestProxy>();
((TestProxy)(object)transport).Handler = (method, args) =>
{
    if (method.Name != "CallAction") throw new Exception("Unexpected transport call: " + method.Name);
    Interlocked.Increment(ref calls);
    var name = ((XmlDocument)args[1]!).SelectSingleNode("//name")!.InnerText;
    submittedNames.Add(name);
    ((XmlDocument)args[2]!).LoadXml("<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'><SOAP-ENV:Body>"
        + (fail || (failLast && name == "last") ? "<SOAP-ENV:Fault><faultcode>1</faultcode><faultstring>Server rejected row&#10;details&#9;column</faultstring></SOAP-ENV:Fault>"
            : "<Result><Item type='Part' id='11111111111111111111111111111111'/></Result>")
        + "</SOAP-ENV:Body></SOAP-ENV:Envelope>");
    return null;
};
var errors = DispatchProxy.Create<IErrorLogService, TestProxy>();
((TestProxy)(object)errors).Handler = (_, args) => { messages.Add((string)args[1]!); return Task.CompletedTask; };
var operations = DispatchProxy.Create<IOperationLogService, TestProxy>();
((TestProxy)(object)operations).Handler = (_, args) =>
{
    if (auditFails) throw new IOException("Audit unavailable");
    audits.Add((string)args[0]!); return Task.CompletedTask;
};
var connection = new ArasConnectionService();
connection.SetConnection(new ArasConnectionInfo { Database = "Regression" }, new Innovator(transport), null!);
var service = new DataImportService(null!, errors, operations, connection, new ArasConnectionPool(connection));
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
}
Task<ImportResult> Run(Func<int, int, Task>? callback = null, CancellationToken token = default) =>
    service.ExecuteImportAsync(file, "Data", 2, -1, 1, -1, template, 1, token, callback);

AmlRenderingTests.Run(service, Check);

var success = await Run();
Check(success.IsCompleted && success.TotalRows == 3 && success.ProcessedRows == 3
    && success.SuccessCount == 2 && success.SkippedCount == 1 && calls == 2 && audits.Count == 2,
    "real loop reuses Innovator, skips blank rows and logs successful writes");
var interrupted = await Run((_, _) => throw new System.Runtime.InteropServices.COMException("", unchecked((int)0x8001010E)));
Check(!interrupted.IsCompleted && interrupted.ProcessedRows == 1 && interrupted.SuccessCount == 1
    && interrupted.FailureCount == 0 && interrupted.ErrorMessage.Contains("8001010E"),
    "empty-message callback exception preserves committed count and reports HRESULT");
using var cancellation = new CancellationTokenSource();
var cancelled = await Run((_, _) => { cancellation.Cancel(); return Task.CompletedTask; }, cancellation.Token);
Check(cancelled.IsCancelled && !cancelled.IsCompleted && cancelled.ProcessedRows == 1 && cancelled.SuccessCount == 1,
    "cancellation preserves partial counters");
fail = true;
var rejected = await Run();
Check(rejected.IsCompleted && rejected.FailureCount == 2 && rejected.SuccessCount == 0
    && rejected.SkippedCount == 1 && messages.Any(x => x.Contains("Server rejected row")),
    "server faults count as row failures and are recorded");
var failureLines = File.ReadAllLines(rejected.LogFilePath).Where(line => line.StartsWith("[失败]\t")).ToArray();
Check(rejected.FailedRowNumbers.SequenceEqual(new[] { 2, 4 }) && failureLines.Length == 2
    && failureLines.All(line => line.Split('\t').Length == 4)
    && failureLines.Select(line => int.Parse(line.Split('\t')[^1])).SequenceEqual(new[] { 2, 4 })
    && failureLines.All(line => line.Contains(@"\ndetails\tcolumn")),
    "failure log final column holds original Excel row numbers even with multiline errors");
fail = false;
failLast = true;
var mixed = await Run();
Check(mixed.SuccessCount == 1 && mixed.FailureCount == 1 && mixed.FailedRowNumbers.SequenceEqual(new[] { 4 })
    && File.ReadAllLines(mixed.LogFilePath).Where(line => line.StartsWith("[成功]\t") || line.StartsWith("[跳过]\t"))
        .All(line => line.Split('\t').Length == 4 && line.Split('\t')[^1] == ""),
    "success and skipped rows leave the final failure column empty");
var offset = await service.ExecuteImportAsync(file, "Data", 4, 4, 1, -1, template);
Check(offset.FailedRowNumbers.SequenceEqual(new[] { 4 })
    && File.ReadAllLines(offset.LogFilePath).Single(line => line.StartsWith("[失败]\t")).EndsWith("\t4\t4"),
    "non-default starting rows retain the source worksheet row number");
failLast = false;

var specialFile = Path.Combine(directory, "special.xlsx");
const string specialValue = "  R&D <零件> 'single' \"double\" &#x20; @A ]]>\r\nline\t😀 \r\n";
using (var package = new ExcelPackage())
{
    var sheet = package.Workbook.Worksheets.Add("Data");
    sheet.Cells[1, 1].Value = "name";
    sheet.Cells[2, 1].Value = specialValue;
    sheet.Cells[3, 1].Value = "bad\u0001value";
    package.SaveAs(new FileInfo(specialFile));
}
var priorCalls = calls;
var specialPreview = await service.ReadSheetRangeAsync(specialFile, "Data", 2, 2, 1, -1);
Check(specialPreview.Data.Rows[0]["name"].ToString() == specialValue, "Excel preview preserves leading and trailing whitespace");
var special = await service.ExecuteImportAsync(specialFile, "Data", 2, -1, 1, -1, template);
Check(special.IsCompleted && special.SuccessCount == 1 && special.FailureCount == 1
    && calls == priorCalls + 1 && submittedNames.Contains(specialValue) && special.FailedRowNumbers.SequenceEqual(new[] { 3 })
    && File.ReadAllLines(special.LogFilePath).Single(line => line.StartsWith("[失败]\t")).EndsWith("\t3\t3"),
    "actual Excel-to-IOM roundtrip preserves special text and logs invalid XML rows without submitting them");

auditFails = true;
var audited = await Run();
Check(audited.IsCompleted && audited.SuccessCount == 2 && audited.FailureCount == 0,
    "audit logging failure does not turn committed rows into failed rows");
var invalid = await service.ExecuteImportAsync(file, "Data", 0, -1, 1, -1, template);
var emptyAml = await service.ExecuteImportAsync(file, "Data", 2, -1, 1, -1, " ");
Check(!invalid.IsCompleted && invalid.ErrorMessage.Length > 0 && !emptyAml.IsCompleted && emptyAml.ErrorMessage.Length > 0,
    "invalid input never reports completion");
connection.Disconnect();
var disconnected = await Run();
Check(!disconnected.IsCompleted && disconnected.ErrorMessage.Contains("登录"), "missing connection is explicit");
Console.WriteLine("All data import service regressions passed.");

public class TestProxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
}
