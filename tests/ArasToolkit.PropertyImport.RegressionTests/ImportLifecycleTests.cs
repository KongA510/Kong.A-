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

internal static class ImportLifecycleTests
{
    private const string TargetId = "11111111111111111111111111111111";
    private const string TargetName = "RegressionItemType";

    public static async Task<int> RunAsync(string template)
    {
        Scenario[] scenarios =
        [
            new("全部新增"),
            new("全部覆盖", Existing: true),
            new("部分行失败", FailedRow: 2),
            new("全部行失败", FailedRow: -1),
            new("提交前取消", CancelBeforePrepare: true),
            new("提交一行后取消", CancelAfterRow: 1),
            new("对象类保存失败", SaveFails: true),
            new("取消且对象类保存失败", CancelAfterRow: 1, SaveFails: true),
            new("保存阶段收到取消", CancelDuringSave: true),
            new("预检失败", PrecheckFails: true)
        ];
        foreach (var scenario in scenarios)
            await RunScenarioAsync(template, scenario);
        return scenarios.Length;
    }

    private static async Task RunScenarioAsync(string template, Scenario scenario)
    {
        using var cancellation = new CancellationTokenSource();
        var writes = new List<XElement>();
        var errors = new List<string>();
        var audits = new List<string>();
        var progress = new List<ImportProgressInfo>();
        var factory = new RecordingFactory();
        var attempts = 0;
        var saved = false;
        var transport = DispatchProxy.Create<IServerConnection, TestProxy>();
        ((TestProxy)(object)transport).Handler = (method, args) =>
        {
            Check(method.Name == "CallAction", "只能调用模拟 IOM 传输");
            var request = XDocument.Parse(((XmlDocument)args[1]!).OuterXml).Descendants("Item").Single();
            var type = (string?)request.Attribute("type");
            var action = (string?)request.Attribute("action");
            string body;
            if (action == "get")
            {
                body = type == "ItemType" ? ItemResponse("ItemType", TargetId, TargetName)
                    : scenario.PrecheckFails ? Fault("SOAP-ENV:Server", "precheck denied")
                    : scenario.Existing ? ItemResponse("Property", "22222222222222222222222222222222", request.Element("name")!.Value)
                    : Fault("0", "No items of type Property found.");
            }
            else
            {
                writes.Add(new XElement(request));
                if (type == "Property")
                {
                    attempts++;
                    Check(action == (scenario.Existing ? "edit" : "add"), "属性操作类型错误");
                    Check((string?)request.Element("source_id") == TargetId, "属性写入错误对象类");
                    if (scenario.CancelAfterRow == attempts)
                        cancellation.Cancel();
                    body = scenario.FailedRow == -1 || scenario.FailedRow == attempts
                        ? Fault("SOAP-ENV:Server", "row rejected")
                        : ItemResponse("Property", "22222222222222222222222222222222", request.Element("name")!.Value);
                }
                else
                {
                    Check(type == "ItemType" && action == "edit", "收尾必须为目标 ItemType 的 edit");
                    Check((string?)request.Attribute("id") == TargetId && !request.HasElements,
                        "收尾 edit 必须只指定目标 ID，不能携带任何字段或关系");
                    Check(progress.OfType<PropertyImportProgressInfo>().Last().IsFinalizing,
                        "edit 前必须报告保存阶段以停止取消交互");
                    Check(progress.All(value => value.Percentage < 100), "保存完成前不能报告 100%");
                    if (scenario.CancelDuringSave)
                        cancellation.Cancel();
                    saved = !scenario.SaveFails;
                    body = scenario.SaveFails ? Fault("SOAP-ENV:Server", "itemtype save rejected")
                        : ItemResponse("ItemType", TargetId, TargetName);
                }
            }
            ((XmlDocument)args[2]!).LoadXml($"""
                <SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
                  <SOAP-ENV:Body>{body}</SOAP-ENV:Body>
                </SOAP-ENV:Envelope>
                """);
            return null;
        };
        var connection = new ArasConnectionService();
        connection.SetConnection(new ArasConnectionInfo(), new Innovator(transport), null!);
        var errorService = DispatchProxy.Create<IErrorLogService, TestProxy>();
        ((TestProxy)(object)errorService).Handler = (_, args) =>
        {
            errors.Add((string)args[1]!);
            return Task.CompletedTask;
        };
        var operationService = DispatchProxy.Create<IOperationLogService, TestProxy>();
        ((TestProxy)(object)operationService).Handler = (method, args) =>
        {
            Check(method.Name == nameof(IOperationLogService.LogAsync), "必须记录操作日志");
            audits.Add((string)args[3]!);
            return Task.CompletedTask;
        };
        var service = new PropertyImportService(factory, connection, operationService, errorService);
        var reporter = new ImmediateProgress(value =>
        {
            progress.Add(value);
            if (scenario.CancelBeforePrepare && value.Phase == "预检与组装")
                cancellation.Cancel();
            if (value.Phase == "完成" && writes.Any(item => (string?)item.Attribute("type") == "ItemType"))
                Check(saved, "对象类保存失败不能报告完成");
        });
        var result = await service.ImportAsync(template, TargetId, TargetName, "覆盖", reporter, cancellation.Token);
        var shouldCancel = scenario.CancelBeforePrepare || scenario.CancelAfterRow > 0;
        var expectedAttempts = scenario.CancelBeforePrepare || scenario.PrecheckFails ? 0 : scenario.CancelAfterRow > 0 ? 1 : 3;
        var expectedSuccesses = scenario.FailedRow == -1 ? 0 : expectedAttempts - (scenario.FailedRow > 0 ? 1 : 0);
        var shouldSave = expectedSuccesses > 0;
        Check(attempts == expectedAttempts && result.Sheet1Count == expectedSuccesses, "实际提交数量错误");
        Check(result.ItemTypeSaveAttempted == shouldSave && result.ItemTypeSaved == (shouldSave && !scenario.SaveFails), "对象类保存结果错误");
        Check(writes.Count(item => (string?)item.Attribute("type") == "ItemType") == (shouldSave ? 1 : 0), "收尾 edit 次数错误");
        if (shouldSave)
            Check((string?)writes.Last().Attribute("type") == "ItemType", "对象类 edit 必须在属性提交之后");
        Check(result.IsCanceled == shouldCancel, "取消状态错误");
        Check(result.IsSuccess == (!shouldCancel && !scenario.SaveFails && !scenario.PrecheckFails), "最终成功状态错误");
        if (scenario.CancelAfterRow > 0)
            Check(result.RowStatuses.Values.Count(status => status == "未提交") == 2 && result.UnsubmittedCount == 2,
                "取消后的未提交行不能显示成功");
        if (scenario.SaveFails)
            Check(result.ErrorMessage!.Contains("对象类保存未完成") && errors.Any(error => error.Contains("itemtype save rejected")),
                "保存失败必须保留明确错误和错误日志");
        Check(factory.Logs.Count == 1 && audits.Count == 1, "每次导入必须记录一次历史和操作日志");
        var expectedLogStatus = result.IsSuccess && !result.HasFailures ? PropertyImportLog.StatusSuccess : PropertyImportLog.StatusFailed;
        Check(factory.Logs[0].Status == expectedLogStatus, "历史日志不能误报成功");
        Check(progress.OfType<PropertyImportProgressInfo>().Count(value => value.SubmitStatus == "正在提交") == expectedAttempts,
            "必须逐行报告开始状态");
        Check(progress.OfType<PropertyImportProgressInfo>().Count(value => value.SubmitStatus is "已新增" or "已覆盖" or "提交失败") == expectedAttempts,
            "必须逐行报告结束状态");
        Console.WriteLine($"PASS 导入收尾: {scenario.Name}");
    }

    private static string ItemResponse(string type, string id, string name) =>
        $"<Result><Item type=\"{type}\" id=\"{id}\"><name>{name}</name></Item></Result>";
    private static string Fault(string code, string message) =>
        $"<SOAP-ENV:Fault><faultcode>{code}</faultcode><faultstring>{message}</faultstring></SOAP-ENV:Fault>";
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed record Scenario(string Name, bool Existing = false, int FailedRow = 0,
        bool CancelBeforePrepare = false, int CancelAfterRow = 0, bool SaveFails = false,
        bool CancelDuringSave = false, bool PrecheckFails = false);

    private sealed class ImmediateProgress(Action<ImportProgressInfo> report) : IProgress<ImportProgressInfo>
    {
        public void Report(ImportProgressInfo value) => report(value);
    }

    // 使用真实 EF 映射追踪日志实体，只替换 SaveChanges，绝不连接数据库或修改表结构。
    private sealed class RecordingFactory : IDbContextFactory<ArasToolkitDbContext>
    {
        public List<PropertyImportLog> Logs { get; } = [];
        public ArasToolkitDbContext CreateDbContext() => new RecordingContext(Logs);
        public Task<ArasToolkitDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }

    private sealed class RecordingContext(List<PropertyImportLog> logs) : ArasToolkitDbContext(
        new DbContextOptionsBuilder<ArasToolkitDbContext>().UseSqlServer("Server=(local);Database=UnusedRegressionDatabase;Integrated Security=True").Options)
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            logs.AddRange(ChangeTracker.Entries<PropertyImportLog>().Select(entry => entry.Entity));
            return Task.FromResult(1);
        }
    }
}
