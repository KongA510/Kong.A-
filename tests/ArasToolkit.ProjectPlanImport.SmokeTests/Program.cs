using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services;
using ArasToolkit.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.ProjectPlanImport.SmokeTests;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Contains("--write-changelog", StringComparer.OrdinalIgnoreCase))
        {
            await WriteChangelogAsync();
            return;
        }
        var exportIndex = Array.FindIndex(args,
            argument => string.Equals(argument, "--export-sample", StringComparison.OrdinalIgnoreCase));
        if (exportIndex >= 0)
        {
            if (exportIndex + 1 >= args.Length) throw new ArgumentException("--export-sample 后必须提供 .xlsx 路径。");
            await ExportSampleAsync(args[exportIndex + 1]);
            return;
        }

        var runLive = args.Contains("--live", StringComparer.OrdinalIgnoreCase);
        var verifyLatest = args.Contains("--verify-latest", StringComparer.OrdinalIgnoreCase);
        await RunLocalRoundTripAsync();
        if (runLive || verifyLatest) await RunLiveImportAsync(verifyLatest);
    }

    private static async Task RunLocalRoundTripAsync()
    {
        var innovator = new FakeInnovator();
        var connection = new FakeConnectionService(innovator);
        var operationLog = new FakeOperationLogService();
        var errorLog = new FakeErrorLogService();
        var service = new ProjectPlanImportService(connection, operationLog, errorLog);
        var tempFile = Path.Combine(Path.GetTempPath(), $"project-plan-template-{Guid.NewGuid():N}.xlsx");

        try
        {
            await File.WriteAllBytesAsync(tempFile, service.GenerateTemplate());
            var definition = await service.ParseTemplateAsync(tempFile);
            Assert(definition.TemplateVersion == "1.0", "模板版本应为 1.0。");
            Assert(definition.PhaseCount == 2, "示例模板应包含 2 个阶段。");
            Assert(definition.ActivityCount == 2, "示例模板应包含 2 个任务。");
            Assert(definition.MilestoneCount == 2, "示例模板应包含 2 个里程碑。");
            Assert(definition.Warnings.Count == 0, "标准示例模板不应产生警告。");

            definition.TemplateName = $"项目计划回归测试_{DateTime.Now:yyyyMMddHHmmss}";
            var prepared = await service.PrepareImportAsync(definition);
            var aml = XDocument.Parse(prepared.Aml);
            Assert(CountItems(aml, "Project Template") == 1, "AML 应包含一个 Project Template。");
            Assert(CountItems(aml, "WBS Element") == 3, "AML 应包含顶层 WBS 与两个阶段。");
            Assert(CountItems(aml, "Sub WBS") == 2, "AML 应包含两个 Sub WBS 关系。");
            Assert(CountItems(aml, "Activity2") == 4, "AML 应包含四个 Activity2。");
            Assert(CountItems(aml, "WBS Activity2") == 4, "AML 应包含四个 WBS Activity2 关系。");
            Assert(CountItems(aml, "Predecessor") == 3, "AML 应包含三个 Predecessor 关系。");
            Assert(CountItems(aml, "Activity2 Assignment") == 0, "角色留空时不应创建 Assignment。");
            Assert(aml.Descendants("ProjectTemplate").Count() == 0, "不得生成不存在的猜测节点名。");
            AssertPrevItemChain(aml, prepared.RootWbsId, definition.Nodes.Count);

            var multiPredecessorNode = definition.Nodes.Single(node => node.Code == "M2");
            multiPredecessorNode.PredecessorCodes = "T1,T2";
            service.NormalizeAndValidate(definition);
            Assert(multiPredecessorNode.PredecessorCodes == "T1,T2",
                "多前置节点应规范化为英文逗号分隔。");
            var multiPrepared = await service.PrepareImportAsync(definition);
            var multiAml = XDocument.Parse(multiPrepared.Aml);
            var m2Id = (string?)multiAml.Root!.Elements("Item")
                .Single(item => (string?)item.Attribute("type") == "Activity2" &&
                                item.Element("name")?.Value == "设计评审")
                .Attribute("id") ?? string.Empty;
            var m2Predecessors = multiAml.Root.Elements("Item")
                .Where(item => (string?)item.Attribute("type") == "Predecessor" &&
                               item.Element("source_id")?.Value == m2Id)
                .ToList();
            Assert(CountItems(multiAml, "Predecessor") == 4,
                "T1,T2 应在原三条依赖基础上生成第四条 Predecessor。");
            Assert(m2Predecessors.Count == 2 &&
                   m2Predecessors.Select(item => item.Element("related_id")?.Value).Distinct().Count() == 2,
                "同一 Activity2 的英文逗号多前置必须拆成两条独立关系。");
            multiPredecessorNode.PredecessorCodes = "T2";
            service.NormalizeAndValidate(definition);

            var originalOrders = definition.Nodes.ToDictionary(node => node.Code, node => node.SortOrder);
            definition.Nodes.Single(node => node.Code == "P2").SortOrder = 2;
            definition.Nodes.Single(node => node.Code == "T1").SortOrder = 3;
            definition.Nodes.Single(node => node.Code == "M1").SortOrder = 4;
            var brokenTreeRejected = false;
            try
            {
                service.NormalizeAndValidate(definition);
            }
            catch (InvalidDataException ex) when (ex.Message.Contains("prev_item", StringComparison.Ordinal))
            {
                brokenTreeRejected = true;
            }
            finally
            {
                foreach (var node in definition.Nodes) node.SortOrder = originalOrders[node.Code];
                service.NormalizeAndValidate(definition);
            }
            Assert(brokenTreeRejected, "打断子树连续性的顺序必须在上传时被拒绝。");

            var result = await service.ImportAsync(prepared, definition.TemplateName);
            Assert(result.IsSuccess && innovator.ImportCalls == prepared.Steps.Count,
                "必须按 ArasLabs 官方顺序逐项执行全部写入步骤。");
            Assert(operationLog.Entries.Count == 1 && operationLog.Entries[0].EntityType == "ProjectTemplate",
                "成功汇入后应记录 ProjectTemplate Import 操作日志。");
            Assert(errorLog.Entries.Count == 0, "成功回归过程不应产生错误日志。");

            Console.WriteLine(
                $"LOCAL_OK template=1 rootWbs=1 phases=2 activities=4 predecessors=3 " +
                $"multiPredecessors=2 steps={prepared.Steps.Count}");
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    private static async Task RunLiveImportAsync(bool verifyLatest)
    {
        Console.Write("Aras URL: ");
        var url = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Database: ");
        var database = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Password: ");
        var password = ReadPassword();
        Console.WriteLine();

        var connection = new ArasConnectionService();
        var login = new LoginService(connection);
        var operationLog = new FakeOperationLogService();
        var errorLog = new FakeErrorLogService();
        var service = new ProjectPlanImportService(connection, operationLog, errorLog);
        var tempFile = Path.Combine(Path.GetTempPath(), $"project-plan-live-{Guid.NewGuid():N}.xlsx");

        try
        {
            await login.LoginAsync(new LoginInfo
            {
                Url = url,
                Database = database,
                Username = username,
                Password = password,
                IsPasswordHashed = false
            });
            password = string.Empty;

            if (verifyLatest)
            {
                var latest = FindLatestValidationTemplate(connection.InnovatorInstance!);
                var latestVerification = VerifyImportedStructure(connection.InnovatorInstance!, latest.Result);
                Assert(latestVerification.PhaseCount == 2, "真实实例应写入两个顶层阶段。");
                Assert(latestVerification.ActivityCount == 4, "真实实例应写入四个活动/里程碑。");
                Assert(latestVerification.PredecessorCount == 4, "多前置真实实例应写入四个前置关系。");
                Assert(latestVerification.MaxPredecessorsPerActivity == 2,
                    "至少一个 Activity2 应持有两条独立 Predecessor 关系。");
                Assert(latestVerification.PrevItemCount == 6, "真实实例的 prev_item 链应覆盖六个计划节点。");
                Console.WriteLine(
                    $"LIVE_VERIFY_OK name={latest.Name} templateId={latest.Result.TemplateId} " +
                    $"rootWbsId={latest.Result.RootWbsId} phases={latestVerification.PhaseCount} " +
                    $"activities={latestVerification.ActivityCount} predecessors={latestVerification.PredecessorCount} " +
                    $"maxPredecessors={latestVerification.MaxPredecessorsPerActivity} " +
                    $"prevChain={latestVerification.PrevItemCount}");
                return;
            }

            await File.WriteAllBytesAsync(tempFile, service.GenerateTemplate());
            var definition = await service.ParseTemplateAsync(tempFile);
            definition.TemplateName = $"Codex_项目计划模板验证_{DateTime.Now:yyyyMMdd_HHmmss}";
            definition.Description = "由个人工具箱项目计划模板汇入功能生成的真实连接验证记录";
            definition.Nodes.Single(node => node.Code == "M2").PredecessorCodes = "T1,T2";

            var prepared = await service.PrepareImportAsync(definition);
            var imported = await service.ImportAsync(prepared, definition.TemplateName);
            var verification = VerifyImportedStructure(connection.InnovatorInstance!, imported);

            Assert(verification.PhaseCount == 2, "真实实例应写入两个顶层阶段。");
            Assert(verification.ActivityCount == 4, "真实实例应写入四个活动/里程碑。");
            Assert(verification.PredecessorCount == 4, "多前置真实实例应写入四个前置关系。");
            Assert(verification.MaxPredecessorsPerActivity == 2,
                "至少一个 Activity2 应持有两条独立 Predecessor 关系。");
            Assert(verification.PrevItemCount == 6, "真实实例的 prev_item 链应覆盖六个计划节点。");
            Assert(operationLog.Entries.Count == 1, "真实汇入代码路径应调用一次操作日志服务。");
            Assert(errorLog.Entries.Count == 0, "真实汇入不应产生错误日志。");

            Console.WriteLine(
                $"LIVE_OK name={definition.TemplateName} templateId={imported.TemplateId} rootWbsId={imported.RootWbsId} " +
                $"phases={verification.PhaseCount} activities={verification.ActivityCount} " +
                $"predecessors={verification.PredecessorCount} " +
                $"maxPredecessors={verification.MaxPredecessorsPerActivity} prevChain={verification.PrevItemCount}");
        }
        finally
        {
            password = string.Empty;
            login.Logout();
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    private static VerificationResult VerifyImportedStructure(object innovatorObject, ProjectPlanImportResult imported)
    {
        dynamic innovator = innovatorObject;
        dynamic templateQuery = innovator.newItem("Project Template", "get");
        templateQuery.setID(imported.TemplateId);
        templateQuery.setAttribute("select", "id,name,wbs_id");
        dynamic templateResult = templateQuery.apply();
        AssertArasSuccess(templateResult, "回查 Project Template");
        Assert((int)templateResult.getItemCount() == 1, "无法按 ID 回查刚汇入的 Project Template。");
        var actualWbsId = (string)templateResult.getItemByIndex(0).getProperty("wbs_id", "");
        Assert(actualWbsId == imported.RootWbsId, "Project Template.wbs_id 与预生成根 WBS ID 不一致。");

        var phaseIds = GetRelatedIds(innovator, "Sub WBS", imported.RootWbsId);
        var activityIds = new List<string>();
        foreach (var phaseId in phaseIds)
            activityIds.AddRange(GetRelatedIds(innovator, "WBS Activity2", phaseId));

        var predecessorCounts = new List<int>();
        foreach (var activityId in activityIds)
            predecessorCounts.Add(GetRelatedIds(innovator, "Predecessor", activityId).Count);
        var predecessorCount = predecessorCounts.Sum();
        var maxPredecessorsPerActivity = predecessorCounts.Count == 0 ? 0 : predecessorCounts.Max();

        var prevItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var phaseId in phaseIds) prevItems[phaseId] = GetPrevItem(innovator, "WBS Element", phaseId);
        foreach (var activityId in activityIds) prevItems[activityId] = GetPrevItem(innovator, "Activity2", activityId);
        var prevItemCount = AssertLivePrevItemChain(imported.RootWbsId, prevItems);

        return new VerificationResult(
            phaseIds.Count, activityIds.Count, predecessorCount, maxPredecessorsPerActivity, prevItemCount);
    }

    private static string GetPrevItem(dynamic innovator, string itemType, string id)
    {
        dynamic query = innovator.newItem(itemType, "get");
        query.setID(id);
        query.setAttribute("select", "id,prev_item");
        dynamic result = query.apply();
        AssertArasSuccess(result, $"回查 {itemType}.prev_item");
        Assert((int)result.getItemCount() == 1, $"无法唯一回查 {itemType} {id}。");
        return (string)result.getItemByIndex(0).getProperty("prev_item", "");
    }

    private static int AssertLivePrevItemChain(string rootWbsId, IReadOnlyDictionary<string, string> prevItems)
    {
        var allowedPredecessors = new HashSet<string>(prevItems.Keys, StringComparer.OrdinalIgnoreCase)
        {
            rootWbsId
        };
        foreach (var pair in prevItems)
            Assert(allowedPredecessors.Contains(pair.Value), $"prev_item 指向了树外 ID：{pair.Key} → {pair.Value}。");

        var nextByPrevious = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in prevItems)
            Assert(nextByPrevious.TryAdd(pair.Value, pair.Key), $"prev_item 链出现分叉：{pair.Value} 被多个节点引用。");

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var current = rootWbsId;
        while (nextByPrevious.TryGetValue(current, out var next))
        {
            Assert(visited.Add(next), $"prev_item 链存在环：{next}。");
            current = next;
        }
        Assert(visited.Count == prevItems.Count, $"prev_item 链断裂：仅覆盖 {visited.Count}/{prevItems.Count} 个节点。");
        return visited.Count;
    }

    private static (string Name, ProjectPlanImportResult Result) FindLatestValidationTemplate(object innovatorObject)
    {
        dynamic innovator = innovatorObject;
        dynamic query = innovator.newItem("Project Template", "get");
        query.setAttribute("select", "id,name,wbs_id,created_on");
        query.setAttribute("orderBy", "created_on DESC");
        query.setAttribute("maxRecords", "1");
        query.setProperty("name", "Codex_项目计划模板验证_%");
        query.setPropertyAttribute("name", "condition", "like");
        dynamic result = query.apply();
        AssertArasSuccess(result, "读取最新项目计划验证模板");
        Assert((int)result.getItemCount() == 1, "未找到刚刚汇入的项目计划验证模板。");
        dynamic item = result.getItemByIndex(0);
        return (
            (string)item.getProperty("name", ""),
            new ProjectPlanImportResult
            {
                IsSuccess = true,
                TemplateId = (string)item.getID(),
                RootWbsId = (string)item.getProperty("wbs_id", "")
            });
    }

    private static List<string> GetRelatedIds(dynamic innovator, string relationshipType, string sourceId)
    {
        dynamic query = innovator.newItem(relationshipType, "get");
        query.setAttribute("select", "id,source_id,related_id");
        query.setProperty("source_id", sourceId);
        dynamic result = query.apply();
        if ((bool)result.isError())
        {
            string code;
            try { code = (string)result.getErrorCode(); }
            catch { code = string.Empty; }
            if (code == "0") return [];
            AssertArasSuccess(result, $"回查 {relationshipType}");
        }
        var ids = new List<string>();
        var count = (int)result.getItemCount();
        for (var index = 0; index < count; index++)
        {
            var relatedId = (string)result.getItemByIndex(index).getProperty("related_id", "");
            if (!string.IsNullOrWhiteSpace(relatedId)) ids.Add(relatedId);
        }
        return ids;
    }

    private static void AssertArasSuccess(dynamic? result, string operation)
    {
        if (result == null)
            throw new InvalidOperationException($"{operation}失败：无返回");
        if ((bool)result.isError())
            throw new InvalidOperationException($"{operation}失败：{(string)result.getErrorString()}");
    }

    private static string ReadPassword()
    {
        var value = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) return value.ToString();
            if (key.Key == ConsoleKey.Backspace)
            {
                if (value.Length > 0) value.Length--;
                continue;
            }
            if (!char.IsControl(key.KeyChar)) value.Append(key.KeyChar);
        }
    }

    private static int CountItems(XContainer aml, string type) =>
        aml.Descendants("Item").Count(item => (string?)item.Attribute("type") == type);

    private static void AssertPrevItemChain(XDocument aml, string rootWbsId, int expectedNodeCount)
    {
        var linearItems = aml.Root!.Elements("Item")
            .Where(item => (string?)item.Attribute("type") is "WBS Element" or "Activity2")
            .ToList();
        Assert(linearItems.Count == expectedNodeCount + 1, "prev_item 链应包含根 WBS 和所有计划节点。");
        Assert((string?)linearItems[0].Attribute("id") == rootWbsId, "prev_item 链的头节点必须是顶层 WBS。");
        Assert(linearItems[0].Element("prev_item") == null, "顶层 WBS 不应设置 prev_item。");

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { rootWbsId };
        for (var index = 1; index < linearItems.Count; index++)
        {
            var id = (string?)linearItems[index].Attribute("id") ?? string.Empty;
            var expectedPreviousId = (string?)linearItems[index - 1].Attribute("id") ?? string.Empty;
            var actualPreviousId = linearItems[index].Element("prev_item")?.Value ?? string.Empty;
            Assert(actualPreviousId == expectedPreviousId,
                $"prev_item 断链：节点 {id} 应指向 {expectedPreviousId}，实际为 {actualPreviousId}。");
            Assert(visited.Add(id), $"prev_item 链中存在重复 ID 或环：{id}。");
        }
        Assert(visited.Count == expectedNodeCount + 1, "prev_item 链未覆盖全部计划节点。");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    /// <summary>只有显式传入 --write-changelog 才会通过真实服务写入数据库。</summary>
    private static async Task WriteChangelogAsync()
    {
        const string description = "新增项目计划模板汇入：支持标准Excel导出、Aras结构预检、WBS/Activity2/prev_item/前置关系汇入与失败回滚";
        var services = new ServiceCollection();
        services.AddArasToolkitServices();
        await using var provider = services.BuildServiceProvider();
        var changelogService = provider.GetRequiredService<IChangelogService>();
        var existing = await changelogService.GetAllEntriesAsync();
        if (existing.Any(entry => entry.Description == description))
        {
            Console.WriteLine("更新日志已存在，未重复写入。");
            return;
        }

        var currentVersion = changelogService.GetCurrentVersion();
        var parts = currentVersion.Split('.');
        var newVersion = parts.Length == 3 && int.TryParse(parts[2], out var patch)
            ? $"{parts[0]}.{parts[1]}.{patch + 1}"
            : "1.0.1";
        var now = DateTime.Now;
        await changelogService.AddEntryAsync(new Changelog
        {
            Version = newVersion,
            ReleaseDate = now,
            Type = "新增",
            Description = description,
            Author = "开发团队",
            CreatorOn = now
        });
        Console.WriteLine($"已通过 IChangelogService 写入更新日志 v{newVersion}。");
    }

    private static async Task ExportSampleAsync(string path)
    {
        var service = new ProjectPlanImportService(
            new FakeConnectionService(new FakeInnovator()),
            new FakeOperationLogService(),
            new FakeErrorLogService());
        await File.WriteAllBytesAsync(path, service.GenerateTemplate());
        Console.WriteLine($"SAMPLE_EXPORTED {path}");
    }

    private sealed record VerificationResult(
        int PhaseCount,
        int ActivityCount,
        int PredecessorCount,
        int MaxPredecessorsPerActivity,
        int PrevItemCount);
}

public sealed class FakeInnovator
{
    private int _id;
    public int ImportCalls { get; private set; }

    public string getNewID() => (++_id).ToString("X32");

    public FakeQuery newItem(string type, string action) => new(type, action);

    public FakeArasResult applyAML(string aml)
    {
        XDocument.Parse(aml);
        ImportCalls++;
        return new FakeArasResult(0);
    }
}

public sealed class FakeQuery
{
    private readonly string _type;
    private readonly string _action;
    public FakeQuery(string type, string action) { _type = type; _action = action; }
    public void setAttribute(string name, string value) { }
    public void setProperty(string name, string value) { }
    public FakeArasResult apply() => new(_type == "ItemType" && _action == "get" ? 1 : 0);
}

public sealed class FakeArasResult
{
    private readonly int _count;
    public FakeArasResult(int count) => _count = count;
    public bool isError() => false;
    public string getErrorString() => string.Empty;
    public int getItemCount() => _count;
    public FakeArasResult getItemByIndex(int index) => this;
    public string getID() => "F0000000000000000000000000000001";
    public string getProperty(string name, string defaultValue) => defaultValue;
}

public sealed class FakeConnectionService : IArasConnectionService
{
    public FakeConnectionService(object innovator) => InnovatorInstance = innovator;
    public event Action? ConnectionChanged { add { } remove { } }
    public bool IsConnected => true;
    public ArasConnectionInfo? CurrentConnection => new();
    public object? InnovatorInstance { get; }
    public object? HttpConnection => null;
    public void SetConnection(ArasConnectionInfo connectionInfo, object innovator, object httpConnection)
        => throw new NotSupportedException();
    public void Disconnect() { }
}

public sealed class FakeOperationLogService : IOperationLogService
{
    public List<(string OperationType, string EntityType, string EntityId)> Entries { get; } = [];
    public Task LogAsync(string operationType, string entityType, string entityId,
        string? description = null, string? userName = null)
    {
        Entries.Add((operationType, entityType, entityId));
        return Task.CompletedTask;
    }
    public Task<(List<OperationLog> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? operationTypeFilter = null)
        => Task.FromResult((new List<OperationLog>(), 0));
    public string GetCurrentUserName() => "ProjectPlanImportSmokeTest";
}

public sealed class FakeErrorLogService : IErrorLogService
{
    public List<(string FunctionName, string ErrorMessage)> Entries { get; } = [];
    public Task LogErrorAsync(string functionName, string errorMessage,
        string? level = null, string? stackTrace = null)
    {
        Entries.Add((functionName, errorMessage));
        return Task.CompletedTask;
    }
    public Task<(List<ErrorLog> Items, int TotalCount)> GetPagedEntriesAsync(
        int page, int pageSize, string? levelFilter = null, DateTime? fromDate = null, DateTime? toDate = null)
        => Task.FromResult((new List<ErrorLog>(), 0));
    public Task<List<ErrorLog>> GetAllEntriesAsync() => Task.FromResult(new List<ErrorLog>());
    public Task ClearAllAsync() => Task.CompletedTask;
}
