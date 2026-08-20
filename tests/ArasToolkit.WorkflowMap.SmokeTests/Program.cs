using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services;
using ArasToolkit.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.WorkflowMap.SmokeTests;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Contains("--write-changelog", StringComparer.OrdinalIgnoreCase))
        {
            await WriteChangelogAsync();
            return;
        }

        var innovator = new FakeInnovator();
        var connection = new FakeConnectionService(innovator);
        var operationLog = new FakeOperationLogService();
        var errorLog = new FakeErrorLogService();
        var service = new WorkflowMapService(connection, operationLog, errorLog);
        var tempFile = Path.Combine(Path.GetTempPath(), $"workflow-map-{Guid.NewGuid():N}.xlsx");

        try
        {
            // 验证模板生成与“一文件一流程”解析结果。
            await File.WriteAllBytesAsync(tempFile, service.GenerateTemplate());
            var definition = await service.ParseTemplateAsync(tempFile);
            Assert(definition.Nodes.Count == 6, "模板应解析为 6 个节点。");
            Assert(definition.Paths.Count == 6, "模板应解析为 6 条路径。");
            Assert(definition.Nodes.Single(node => node.Code == "START").IsAutomatic, "开始节点应自动化。");
            Assert(definition.Nodes.Single(node => node.Code == "END").IsAutomatic, "结束节点应自动化。");
            Assert(definition.Nodes.Single(node => node.Code == "NOTICE").IsAutomatic, "通知节点应自动化。");
            Assert(definition.Nodes.Where(node => !node.IsStart && !node.IsEnd && !node.IsAutomatic)
                    .All(node => string.IsNullOrWhiteSpace(node.Assignee)),
                "人工节点执行角色留空时应继承流程所有者 Creator。");
            Assert(definition.Warnings.Count(warning => warning.Contains("继承流程所有者 Creator", StringComparison.Ordinal)) == 2,
                "模板中的两个人工节点应提示继承流程所有者 Creator。");
            Assert(definition.Nodes.Single(node => node.Code == "START").X
                   < definition.Nodes.Single(node => node.Code == "CREATE").X
                   && definition.Nodes.Single(node => node.Code == "CREATE").X
                   < definition.Nodes.Single(node => node.Code == "REVIEW").X,
                "标准模板的主流程必须从左到右排列。");

            var returnPath = definition.Paths.Single(path => path.Name == "退回到建立者");
            Assert(returnPath.Segments == "560,90|340,90", "模板退回线应保留两个原生 segments 转折点。");
            var roundTrip = service.SerializeSegments(service.ParseSegments(returnPath.Segments));
            Assert(roundTrip == returnPath.Segments, "segments 序列化必须可无损往返。");

            // 验证退回线未填折点时会自动移到主线上方。
            returnPath.Segments = string.Empty;
            returnPath.LabelOffsetX = null;
            returnPath.LabelOffsetY = null;
            service.NormalizeAndValidate(definition);
            var generatedBends = service.ParseSegments(returnPath.Segments);
            Assert(generatedBends.Count == 2, "退回线未填 segments 时应自动生成两个折点。");
            Assert(generatedBends.All(point => point.Y < 210), "自动退回折点应位于主线上方。");
            Assert(generatedBends[0].Y == generatedBends[1].Y
                   && generatedBends[0].X != generatedBends[1].X,
                "退回线应形成可辨识的上方水平正交段。");

            // 验证纵向/重叠 Visio 坐标会自动转换为左到右拓扑布局，分支线自动直角化。
            var verticalDefinition = await service.ParseTemplateAsync(tempFile);
            for (var index = 0; index < verticalDefinition.Nodes.Count; index++)
            {
                verticalDefinition.Nodes[index].X = 140;
                verticalDefinition.Nodes[index].Y = 90 + index * 110;
            }
            foreach (var path in verticalDefinition.Paths)
            {
                path.Segments = string.Empty;
                path.LabelOffsetX = null;
                path.LabelOffsetY = null;
            }
            service.NormalizeAndValidate(verticalDefinition);
            var horizontalMainCodes = new[] { "START", "CREATE", "REVIEW", "NOTICE", "END" };
            var horizontalMainNodes = horizontalMainCodes
                .Select(code => verticalDefinition.Nodes.Single(node => node.Code == code))
                .ToList();
            Assert(horizontalMainNodes.Zip(horizontalMainNodes.Skip(1), (source, target) => target.X > source.X).All(value => value),
                "纵向坐标应自动转换为从左到右的主流程。");
            var verticalCancel = verticalDefinition.Nodes.Single(node => node.Code == "CANCEL");
            var verticalReview = verticalDefinition.Nodes.Single(node => node.Code == "REVIEW");
            Assert(verticalCancel.X == verticalReview.X && verticalCancel.Y > verticalReview.Y,
                "同层取消分支应位于主流程下方，不应挤入主线。");
            var cancelPath = verticalDefinition.Paths.Single(path => path.Name == "取消流程");
            var cancelBends = service.ParseSegments(cancelPath.Segments);
            Assert(cancelBends.Count == 2 && cancelBends[0].X == cancelBends[1].X,
                "斜向分支未填 segments 时应自动生成垂直中段的正交折线。");
            Assert(verticalDefinition.Warnings.Any(warning => warning.Contains("从左到右布局", StringComparison.Ordinal)),
                "自动横向转换应给出清晰提示。");

            // 验证新增 AML 的完整嵌套对象图。
            var prepared = await service.PrepareAmlAsync(
                definition, "工作流程设定回归测试", "自动化结构校验", "新增");
            var aml = XDocument.Parse(prepared.Aml);
            Assert(aml.Root?.Name == "AML", "完整 AML 根节点应为 AML。");
            Assert(CountItems(aml, "Workflow Map") == 1, "一份文件必须只生成一个 Workflow Map。");
            Assert(CountItems(aml, "Workflow Map Activity") == 6, "AML 应包含 6 个 Workflow Map Activity。");
            Assert(CountItems(aml, "Activity Template") == 6, "AML 应包含 6 个 Activity Template。");
            Assert(CountItems(aml, "Workflow Map Path") == 6, "AML 应包含 6 个 Workflow Map Path。");
            Assert(CountItems(aml, "Activity Template Assignment") == 0,
                "人工节点角色留空时不应生成 Assignment，应由 Aras 继承流程所有者。");
            Assert(aml.Descendants("process_owner").Descendants("name").Any(value => value.Value == "Creator"),
                "Creator 固定 ID 未配置时应生成按名称查询的 process_owner。");
            Assert(aml.Descendants("segments").Any(value => value.Value == returnPath.Segments),
                "退回路径 segments 必须进入最终 AML。");

            // 显式填写节点角色时仍应生成单独 Assignment，并覆盖流程所有者回退规则。
            var reviewNode = definition.Nodes.Single(node => node.Code == "REVIEW");
            reviewNode.Assignee = "Creator";
            var explicitAssigneePrepared = await service.PrepareAmlAsync(
                definition, "工作流程设定显式角色回归测试", "显式角色结构校验", "新增");
            var explicitAssigneeAml = XDocument.Parse(explicitAssigneePrepared.Aml);
            Assert(CountItems(explicitAssigneeAml, "Activity Template Assignment") == 1,
                "显式填写节点角色时应生成一个 Activity Template Assignment。");
            reviewNode.Assignee = string.Empty;

            // Prepare 只执行同名查询；Import 只应额外执行一次完整 AML。
            Assert(innovator.QueryCalls == 2 && innovator.ImportCalls == 0,
                "流程所有者回退与显式角色两次生成应各查询一次同名模板。");
            var result = await service.ImportAsync(prepared, "工作流程设定回归测试");
            Assert(result.IsSuccess, "伪 Aras 成功结果应返回导入成功。");
            Assert(innovator.ImportCalls == 1, "导入阶段必须只执行一次完整 AML。");
            Assert(operationLog.Entries.Count == 1 && operationLog.Entries[0].EntityType == "WorkflowMap",
                "导入成功后应记录 WorkflowMap Import 操作日志。");

            // 验证覆盖模式保留 Map ID，并在一份 AML 内删除旧节点关系后重建。
            innovator.ExistingMapId = "E0000000000000000000000000000001";
            innovator.ExistingMapActivityIds.AddRange([
                "D0000000000000000000000000000001",
                "D0000000000000000000000000000002"
            ]);
            var overwrite = await service.PrepareAmlAsync(
                definition, "工作流程设定回归测试", "覆盖结构校验", "覆盖");
            var overwriteAml = XDocument.Parse(overwrite.Aml);
            var overwriteMap = overwriteAml.Descendants("Item")
                .Single(item => (string?)item.Attribute("type") == "Workflow Map");
            var mapActivities = overwriteAml.Descendants("Item")
                .Where(item => (string?)item.Attribute("type") == "Workflow Map Activity").ToList();
            Assert(overwrite.IsOverwrite && overwrite.MapId == innovator.ExistingMapId,
                "覆盖应保留查询到的 Workflow Map ID。");
            Assert((string?)overwriteMap.Attribute("action") == "edit", "覆盖根项应使用 action=edit。");
            Assert(mapActivities.Count(item => (string?)item.Attribute("action") == "delete") == 2,
                "覆盖 AML 应包含全部旧 Workflow Map Activity 删除项。");
            Assert(mapActivities.Count(item => (string?)item.Attribute("action") == "add") == 6,
                "覆盖 AML 应在同一根项中重建 6 个节点关系。");
            await service.ImportAsync(overwrite, "工作流程设定回归测试");
            Assert(innovator.QueryCalls == 3 && innovator.ImportCalls == 2,
                "三次生成应各查询一次；新增与覆盖应各导入一次。");
            Assert(operationLog.Entries.Count == 2, "新增和覆盖应各记录一条 Import 操作日志。");
            Assert(errorLog.Entries.Count == 0, "成功回归过程不应产生错误日志。");

            Console.WriteLine("工作流程设定回归测试通过：横向布局 / 正交折线 / 流程所有者回退 / 通知自动节点 / 新增与覆盖。");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static int CountItems(XContainer aml, string type)
        => aml.Descendants("Item").Count(item => (string?)item.Attribute("type") == type);

    /// <summary>
    /// 仅在明确传入 --write-changelog 时调用真实 IChangelogService，
    /// 普通回归测试绝不会修改数据库。
    /// </summary>
    private static async Task WriteChangelogAsync()
    {
        const string description = "修复工作流程汇入：横向布局、清晰正交折线及扩大节点和路径参数编辑区";
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
            Type = "修复",
            Description = description,
            Author = "开发团队",
            CreatorOn = now
        });
        Console.WriteLine($"已通过 IChangelogService 写入更新日志 v{newVersion}。");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}

public sealed class FakeInnovator
{
    public int QueryCalls { get; private set; }
    public int ImportCalls { get; private set; }
    public string LastImportedAml { get; private set; } = string.Empty;
    public string? ExistingMapId { get; set; }
    public List<string> ExistingMapActivityIds { get; } = [];

    public FakeArasResult applyAML(string aml)
    {
        var document = XDocument.Parse(aml);
        var map = document.Descendants("Item")
            .FirstOrDefault(item => (string?)item.Attribute("type") == "Workflow Map");
        if (string.Equals((string?)map?.Attribute("action"), "get", StringComparison.OrdinalIgnoreCase))
        {
            QueryCalls++;
            return string.IsNullOrWhiteSpace(ExistingMapId)
                ? FakeArasResult.NoItemsFound()
                : new FakeArasResult(new FakeMapItem(ExistingMapId, ExistingMapActivityIds));
        }

        ImportCalls++;
        LastImportedAml = aml;
        return new FakeArasResult();
    }
}

public sealed class FakeArasResult
{
    private readonly List<FakeMapItem> _items;
    private readonly bool _isError;
    private readonly string _errorCode;
    private readonly string _errorString;

    public FakeArasResult(params FakeMapItem[] items)
        : this(false, string.Empty, string.Empty, items)
    {
    }

    private FakeArasResult(bool isError, string errorCode, string errorString, params FakeMapItem[] items)
    {
        _isError = isError;
        _errorCode = errorCode;
        _errorString = errorString;
        _items = [.. items];
    }

    public static FakeArasResult NoItemsFound()
        => new(true, "0", "No items of type Workflow Map found.");

    public bool isError() => _isError;
    public string getErrorCode() => _errorCode;
    public string getErrorString() => _errorString;
    public int getItemCount() => _items.Count;
    public FakeMapItem getItemByIndex(int index) => _items[index];
}

public sealed class FakeMapItem
{
    private readonly string _id;
    private readonly FakeRelationshipCollection _relationships;

    public FakeMapItem(string id, IEnumerable<string> relationshipIds)
    {
        _id = id;
        _relationships = new FakeRelationshipCollection(relationshipIds);
    }

    public string getID() => _id;
    public FakeRelationshipCollection getRelationships(string relationshipType) => _relationships;
}

public sealed class FakeRelationshipCollection
{
    private readonly List<FakeIdItem> _items;

    public FakeRelationshipCollection(IEnumerable<string> ids)
        => _items = ids.Select(id => new FakeIdItem(id)).ToList();

    public int getItemCount() => _items.Count;
    public FakeIdItem getItemByIndex(int index) => _items[index];
}

public sealed class FakeIdItem
{
    private readonly string _id;
    public FakeIdItem(string id) => _id = id;
    public string getID() => _id;
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

    public string GetCurrentUserName() => "WorkflowMapSmokeTest";
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
