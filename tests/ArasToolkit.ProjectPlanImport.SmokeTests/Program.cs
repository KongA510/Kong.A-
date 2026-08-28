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

        var validateFileIndex = Array.FindIndex(args,
            argument => string.Equals(argument, "--validate-file", StringComparison.OrdinalIgnoreCase));
        var liveFileIndex = Array.FindIndex(args,
            argument => string.Equals(argument, "--live-file", StringComparison.OrdinalIgnoreCase));
        var fileIndex = liveFileIndex >= 0 ? liveFileIndex : validateFileIndex;
        if (fileIndex >= 0)
        {
            if (fileIndex + 1 >= args.Length) throw new ArgumentException("文件验证参数后必须提供 .xlsx 路径。");
            var path = Path.GetFullPath(args[fileIndex + 1]);
            await RunFileValidationAsync(path);
            if (liveFileIndex >= 0) await RunLiveFileImportAsync(path);
            return;
        }

        var runLive = args.Contains("--live", StringComparer.OrdinalIgnoreCase);
        var verifyLatest = args.Contains("--verify-latest", StringComparer.OrdinalIgnoreCase);
        await RunLocalRoundTripAsync();
        if (runLive || verifyLatest) await RunLiveImportAsync(verifyLatest);
    }

    private static async Task RunLocalRoundTripAsync()
    {
        const string projectRoleLabel = "项目经理";
        const string projectRoleValue = "PROJECT_MANAGER";
        var innovator = new FakeInnovator(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [projectRoleLabel] = projectRoleValue
        });
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
            var defaultDurationTask = definition.Nodes.Single(node => node.Code == "T1");
            defaultDurationTask.ProjectRole = projectRoleLabel;
            defaultDurationTask.ExpectedDuration = 0;
            defaultDurationTask.IsExpectedDurationSpecified = false;
            defaultDurationTask.WorkEstimate = 99;
            var explicitDurationTask = definition.Nodes.Single(node => node.Code == "T2");
            explicitDurationTask.ExpectedDuration = 2.5m;
            explicitDurationTask.IsExpectedDurationSpecified = true;
            explicitDurationTask.WorkEstimate = 99;
            definition.Nodes.Single(node => node.Code == "M1").ProjectRole = projectRoleLabel;
            var prepared = await service.PrepareImportAsync(definition);
            var aml = XDocument.Parse(prepared.Aml);
            Assert(defaultDurationTask.ExpectedDuration == 1 && defaultDurationTask.WorkEstimate == 8,
                "任务未填写计划工期时应默认 1 天、8 小时。");
            Assert(explicitDurationTask.ExpectedDuration == 2.5m && explicitDurationTask.WorkEstimate == 20,
                "任务计划工时应统一按计划工期 × 8 计算。");
            Assert(CountItems(aml, "Project Template") == 1, "AML 应包含一个 Project Template。");
            Assert(CountItems(aml, "WBS Element") == 3, "AML 应包含顶层 WBS 与两个阶段。");
            Assert(CountItems(aml, "Sub WBS") == 2, "AML 应包含两个 Sub WBS 关系。");
            Assert(CountItems(aml, "Activity2") == 4, "AML 应包含四个 Activity2。");
            Assert(CountItems(aml, "WBS Activity2") == 4, "AML 应包含四个 WBS Activity2 关系。");
            Assert(CountItems(aml, "Predecessor") == 3, "AML 应包含三个 Predecessor 关系。");
            Assert(CountItems(aml, "Activity2 Assignment") == 0, "项目计划模板不得创建 Activity2 Assignment。");
            AssertHierarchySortOrders(aml, definition.Nodes.Select(node => node.SortOrder));
            var defaultDurationActivity = aml.Root!.Elements("Item")
                .Single(item => (string?)item.Attribute("type") == "Activity2" &&
                                item.Element("name")?.Value == defaultDurationTask.Name);
            Assert(defaultDurationActivity.Element("expected_duration")?.Value == "1" &&
                   defaultDurationActivity.Element("work_est")?.Value == "8",
                "默认工期和计划工时必须写入 Activity2 AML。");
            var leadRoles = aml.Root!.Elements("Item")
                .Where(item => (string?)item.Attribute("type") == "Activity2")
                .Select(item => item.Element("lead_role")?.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();
            Assert(prepared.LeadRoleCount == 2 && leadRoles.Count == 2 &&
                   leadRoles.All(value => value == projectRoleValue),
                "任务和里程碑应将 Project Role label 对应的 value 写入 Activity2.lead_role。");
            Assert(innovator.ProjectRoleQueryCalls == 1,
                "同一次预检只能查询一次 Project Role 列表，不能按节点重复查询。");
            Assert(aml.Descendants("ProjectTemplate").Count() == 0, "不得生成不存在的猜测节点名。");
            AssertPerParentPrevItemChains(aml, prepared.RootWbsId, definition.Nodes.Count);

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
            catch (InvalidDataException ex) when (ex.Message.Contains("前序显示顺序", StringComparison.Ordinal))
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
                Assert(latestVerification.NumberedActivityCount == latestVerification.ActivityCount,
                    "真实实例的 N 列必须按显示顺序连续覆盖全部活动。");
                Console.WriteLine(
                    $"LIVE_VERIFY_OK name={latest.Name} templateId={latest.Result.TemplateId} " +
                    $"rootWbsId={latest.Result.RootWbsId} phases={latestVerification.PhaseCount} " +
                    $"activities={latestVerification.ActivityCount} predecessors={latestVerification.PredecessorCount} " +
                    $"maxPredecessors={latestVerification.MaxPredecessorsPerActivity} " +
                    $"prevChain={latestVerification.PrevItemCount} numbered={latestVerification.NumberedActivityCount}");
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
            Assert(verification.NumberedActivityCount == verification.ActivityCount,
                "真实实例的 N 列必须按显示顺序连续覆盖全部活动。");
            Assert(operationLog.Entries.Count == 1, "真实汇入代码路径应调用一次操作日志服务。");
            Assert(errorLog.Entries.Count == 0, "真实汇入不应产生错误日志。");

            Console.WriteLine(
                $"LIVE_OK name={definition.TemplateName} templateId={imported.TemplateId} rootWbsId={imported.RootWbsId} " +
                $"phases={verification.PhaseCount} activities={verification.ActivityCount} " +
                $"predecessors={verification.PredecessorCount} " +
                $"maxPredecessors={verification.MaxPredecessorsPerActivity} prevChain={verification.PrevItemCount} " +
                $"numbered={verification.NumberedActivityCount}");
        }
        finally
        {
            password = string.Empty;
            login.Logout();
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    private static async Task RunFileValidationAsync(string path)
    {
        Assert(File.Exists(path), $"待验证文件不存在：{path}");
        var parseService = new ProjectPlanImportService(
            new FakeConnectionService(new FakeInnovator()),
            new FakeOperationLogService(),
            new FakeErrorLogService());
        var definition = await parseService.ParseTemplateAsync(path);
        var roleValues = definition.Nodes
            .Select(node => node.ProjectRole)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select((label, index) => (label, value: $"ROLE_{index + 1:D3}"))
            .ToDictionary(pair => pair.label, pair => pair.value, StringComparer.OrdinalIgnoreCase);
        var innovator = new FakeInnovator(roleValues);
        var service = new ProjectPlanImportService(
            new FakeConnectionService(innovator),
            new FakeOperationLogService(),
            new FakeErrorLogService());
        var prepared = await service.PrepareImportAsync(definition);
        var aml = XDocument.Parse(prepared.Aml);
        var expectedActivityCount = definition.ActivityCount + definition.MilestoneCount;
        var expectedPredecessorCount = definition.Nodes.Sum(node => SplitPredecessorCodes(node.PredecessorCodes).Count);
        var expectedLeadRoleCount = definition.Nodes.Count(node => !string.IsNullOrWhiteSpace(node.ProjectRole));
        var expectedDefaultedTaskCount = definition.Nodes.Count(node =>
            node.NodeType == ProjectPlanNodeType.Activity && !node.IsExpectedDurationSpecified);
        var maxPredecessors = definition.Nodes.Max(node => SplitPredecessorCodes(node.PredecessorCodes).Count);

        Assert(CountItems(aml, "Project Template") == 1, "文件预检应生成一个 Project Template。");
        Assert(CountItems(aml, "WBS Element") == definition.PhaseCount + 1,
            "文件预检生成的 WBS Element 数量不正确。");
        Assert(CountItems(aml, "Sub WBS") == definition.PhaseCount, "文件预检生成的 Sub WBS 数量不正确。");
        Assert(CountItems(aml, "Activity2") == expectedActivityCount, "文件预检生成的 Activity2 数量不正确。");
        Assert(CountItems(aml, "WBS Activity2") == expectedActivityCount,
            "文件预检生成的 WBS Activity2 数量不正确。");
        Assert(CountItems(aml, "Predecessor") == expectedPredecessorCount,
            "文件预检生成的 Predecessor 数量不正确。");
        Assert(CountItems(aml, "Activity2 Assignment") == 0,
            "文件预检不得生成 Activity2 Assignment。");
        Assert(definition.Nodes
                .Where(node => node.NodeType == ProjectPlanNodeType.Activity && !node.IsExpectedDurationSpecified)
                .All(node => node.ExpectedDuration == 1 && node.WorkEstimate == 8),
            "文件中的空白任务工期必须默认成 1 天、8 小时。");
        Assert(definition.Nodes
                .Where(node => node.NodeType == ProjectPlanNodeType.Activity)
                .All(node => node.WorkEstimate == node.ExpectedDuration * 8),
            "所有任务计划工时必须等于计划工期 × 8。");
        Assert(definition.Nodes
                .Where(node => node.NodeType == ProjectPlanNodeType.Milestone)
                .All(node => node.ExpectedDuration == 0 && node.WorkEstimate == 0),
            "里程碑计划工期和计划工时必须为 0。");
        AssertHierarchySortOrders(aml, definition.Nodes.Select(node => node.SortOrder));
        var writtenLeadRoles = aml.Root!.Elements("Item")
            .Where(item => (string?)item.Attribute("type") == "Activity2")
            .Select(item => item.Element("lead_role")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
        Assert(prepared.LeadRoleCount == expectedLeadRoleCount &&
               writtenLeadRoles.Count == expectedLeadRoleCount &&
               writtenLeadRoles.All(value => roleValues.Values.Contains(value!, StringComparer.Ordinal)),
            "文件预检生成的 Activity2.lead_role 数量或 value 不正确。");
        Assert(innovator.ProjectRoleQueryCalls == (expectedLeadRoleCount == 0 ? 0 : 1),
            "同一次文件预检只能查询一次 Project Role 列表。");
        Assert(definition.Nodes.All(node =>
                !node.PredecessorCodes.Contains('，') &&
                !node.PredecessorCodes.Contains('；') &&
                !node.PredecessorCodes.Contains('、')),
            "文件预检后仍存在非英文逗号的多前置分隔符。");
        AssertPerParentPrevItemChains(aml, prepared.RootWbsId, definition.Nodes.Count);

        Console.WriteLine(
            $"FILE_OK name={definition.TemplateName} phases={definition.PhaseCount} " +
            $"activities={expectedActivityCount} milestones={definition.MilestoneCount} " +
            $"predecessors={expectedPredecessorCount} maxPredecessors={maxPredecessors} " +
            $"prevChain={definition.Nodes.Count} defaultDurations={expectedDefaultedTaskCount} " +
            $"leadRoles={prepared.LeadRoleCount} roleQueries={innovator.ProjectRoleQueryCalls}");
    }

    private static async Task RunLiveFileImportAsync(string path)
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

            var definition = await service.ParseTemplateAsync(path);
            var prepared = await service.PrepareImportAsync(definition);
            var expectedActivityCount = definition.ActivityCount + definition.MilestoneCount;
            var expectedPredecessorCount = definition.Nodes.Sum(node => SplitPredecessorCodes(node.PredecessorCodes).Count);
            var expectedMaxPredecessors = definition.Nodes.Max(node => SplitPredecessorCodes(node.PredecessorCodes).Count);
            var imported = await service.ImportAsync(prepared, definition.TemplateName);
            var verification = VerifyImportedStructure(connection.InnovatorInstance!, imported);

            Assert(verification.PhaseCount == definition.PhaseCount,
                $"真实实例阶段数不正确：{verification.PhaseCount}/{definition.PhaseCount}。");
            Assert(verification.ActivityCount == expectedActivityCount,
                $"真实实例活动数不正确：{verification.ActivityCount}/{expectedActivityCount}。");
            Assert(verification.PredecessorCount == expectedPredecessorCount,
                $"真实实例前置关系数不正确：{verification.PredecessorCount}/{expectedPredecessorCount}。");
            Assert(verification.MaxPredecessorsPerActivity == expectedMaxPredecessors,
                $"真实实例单活动最大前置数不正确：{verification.MaxPredecessorsPerActivity}/{expectedMaxPredecessors}。");
            Assert(verification.PrevItemCount == definition.Nodes.Count,
                $"真实实例 prev_item 链覆盖不正确：{verification.PrevItemCount}/{definition.Nodes.Count}。");
            Assert(verification.NumberedActivityCount == expectedActivityCount,
                $"真实实例 N 列覆盖不正确：{verification.NumberedActivityCount}/{expectedActivityCount}。");
            Assert(operationLog.Entries.Count == 1, "真实文件汇入应记录一次操作日志。");
            Assert(errorLog.Entries.Count == 0, "真实文件汇入不应产生错误日志。");

            Console.WriteLine(
                $"LIVE_FILE_OK name={definition.TemplateName} templateId={imported.TemplateId} " +
                $"rootWbsId={imported.RootWbsId} phases={verification.PhaseCount} " +
                $"activities={verification.ActivityCount} predecessors={verification.PredecessorCount} " +
                $"maxPredecessors={verification.MaxPredecessorsPerActivity} " +
                $"prevChain={verification.PrevItemCount} numbered={verification.NumberedActivityCount}");
        }
        finally
        {
            password = string.Empty;
            login.Logout();
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

        var phaseIds = new List<string>();
        var activityIds = new List<string>();
        CollectWbsTree(innovator, imported.RootWbsId, phaseIds, activityIds,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        var activityIdSet = activityIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var predecessorCounts = new List<int>();
        foreach (var activityId in activityIds)
        {
            List<string> predecessorIds = GetRelatedIds((object)innovator, "Predecessor", activityId);
            Assert(predecessorIds.All(activityIdSet.Contains),
                $"Activity2 {activityId} 的 Predecessor 指向了项目计划树外节点。");
            predecessorCounts.Add(predecessorIds.Count);
        }
        var predecessorCount = predecessorCounts.Sum();
        var maxPredecessorsPerActivity = predecessorCounts.Count == 0 ? 0 : predecessorCounts.Max();

        var displayActivityIds = new List<string>();
        var prevItemCount = AssertLivePerParentPrevItemChains(
            innovator,
            imported.RootWbsId,
            displayActivityIds,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        Assert(displayActivityIds.Count == activityIds.Count,
            $"按显示顺序收集的 Activity2 数量不正确：{displayActivityIds.Count}/{activityIds.Count}。");
        var numberedActivityCount = AssertLiveActivityNumbers(innovator, imported.RootWbsId, displayActivityIds);

        return new VerificationResult(
            phaseIds.Count, activityIds.Count, predecessorCount, maxPredecessorsPerActivity,
            prevItemCount, numberedActivityCount);
    }

    private static void CollectWbsTree(
        dynamic innovator,
        string wbsId,
        ICollection<string> phaseIds,
        ICollection<string> activityIds,
        ISet<string> visitedWbsIds)
    {
        Assert(visitedWbsIds.Add(wbsId), $"Sub WBS 关系存在环或重复父级：{wbsId}。");
        foreach (var activityId in GetRelatedIds(innovator, "WBS Activity2", wbsId))
            activityIds.Add(activityId);
        foreach (var phaseId in GetRelatedIds(innovator, "Sub WBS", wbsId))
        {
            phaseIds.Add(phaseId);
            CollectWbsTree(innovator, phaseId, phaseIds, activityIds, visitedWbsIds);
        }
    }

    private static string GetPrevItem(dynamic innovator, string itemType, string id)
        => GetItemProperty(innovator, itemType, id, "prev_item");

    private static string GetItemProperty(
        dynamic innovator,
        string itemType,
        string id,
        string propertyName)
    {
        dynamic query = innovator.newItem(itemType, "get");
        query.setID(id);
        query.setAttribute("select", $"id,{propertyName}");
        dynamic result = query.apply();
        AssertArasSuccess(result, $"回查 {itemType}.{propertyName}");
        Assert((int)result.getItemCount() == 1, $"无法唯一回查 {itemType} {id}。");
        return (string)result.getItemByIndex(0).getProperty(propertyName, "");
    }

    private static int AssertLivePerParentPrevItemChains(
        dynamic innovator,
        string parentWbsId,
        ICollection<string> displayActivityIds,
        ISet<string> visitedWbsIds)
    {
        Assert(visitedWbsIds.Add(parentWbsId), $"Sub WBS 关系存在环或重复父级：{parentWbsId}。");
        var children = GetOrderedHierarchyChildren(innovator, parentWbsId);
        var coveredCount = 0;
        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            var expectedPreviousId = index == 0 ? string.Empty : children[index - 1].RelatedId;
            var actualPreviousId = GetPrevItem(innovator, child.ItemType, child.RelatedId);
            Assert(actualPreviousId == expectedPreviousId,
                $"{child.ItemType} {child.RelatedId} 的 prev_item 错误：应为“{expectedPreviousId}”，实际为“{actualPreviousId}”。");
            coveredCount++;

            if (child.ItemType == "Activity2")
            {
                displayActivityIds.Add(child.RelatedId);
            }
            else
            {
                coveredCount += AssertLivePerParentPrevItemChains(
                    innovator, child.RelatedId, displayActivityIds, visitedWbsIds);
            }
        }
        return coveredCount;
    }

    private static List<HierarchyChild> GetOrderedHierarchyChildren(dynamic innovator, string sourceId)
    {
        var children = new List<HierarchyChild>();
        AddChildren("Sub WBS", "WBS Element");
        AddChildren("WBS Activity2", "Activity2");
        return children
            .OrderBy(child => child.SortOrder)
            .ThenBy(child => child.RelatedId, StringComparer.Ordinal)
            .ToList();

        void AddChildren(string relationshipType, string itemType)
        {
            dynamic query = innovator.newItem(relationshipType, "get");
            query.setAttribute("select", "id,related_id,sort_order");
            query.setAttribute("orderBy", "sort_order");
            query.setProperty("source_id", sourceId);
            dynamic result = query.apply();
            if ((bool)result.isError())
            {
                string code;
                try { code = (string)result.getErrorCode(); }
                catch { code = string.Empty; }
                if (code == "0") return;
                AssertArasSuccess(result, $"回查 {relationshipType}");
            }

            for (var index = 0; index < (int)result.getItemCount(); index++)
            {
                dynamic relationship = result.getItemByIndex(index);
                var relatedId = (string)relationship.getProperty("related_id", "");
                _ = int.TryParse((string)relationship.getProperty("sort_order", "0"), out var sortOrder);
                children.Add(new HierarchyChild(relatedId, itemType, sortOrder));
            }
        }
    }

    private static int AssertLiveActivityNumbers(
        dynamic innovator,
        string rootWbsId,
        IReadOnlyList<string> displayActivityIds)
    {
        dynamic result = innovator.applyMethod("GetActivitiesNumbers", $"<rootWBS>{rootWbsId}</rootWBS>");
        AssertArasSuccess(result, "调用 GetActivitiesNumbers");
        string xml;
        try { xml = (string)result.getResult(); }
        catch { xml = result.ToString(); }
        if (string.IsNullOrWhiteSpace(xml)) xml = result.ToString();

        XDocument document;
        try { document = XDocument.Parse(xml); }
        catch { document = XDocument.Parse($"<root>{xml}</root>"); }
        var numberById = document.Descendants()
            .Where(element => element.Name.LocalName == "a" &&
                              element.Attribute("id") != null &&
                              element.Attribute("number") != null)
            .ToDictionary(
                element => element.Attribute("id")!.Value,
                element => int.Parse(element.Attribute("number")!.Value),
                StringComparer.OrdinalIgnoreCase);

        Assert(numberById.Count == displayActivityIds.Count,
            $"GetActivitiesNumbers 返回数量不正确：{numberById.Count}/{displayActivityIds.Count}。");
        for (var index = 0; index < displayActivityIds.Count; index++)
        {
            var activityId = displayActivityIds[index];
            Assert(numberById.TryGetValue(activityId, out var actualNumber),
                $"GetActivitiesNumbers 未返回 Activity2 {activityId}。");
            Assert(actualNumber == index + 1,
                $"Activity2 {activityId} 的 N 列错误：应为 {index + 1}，实际为 {actualNumber}。");
        }
        return numberById.Count;
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

    private static void AssertHierarchySortOrders(XDocument aml, IEnumerable<int> nodeSequences)
    {
        var actualSortOrders = aml.Root!.Elements("Item")
            .Where(item => (string?)item.Attribute("type") is "Sub WBS" or "WBS Activity2")
            .Select(item => int.Parse(item.Element("sort_order")?.Value ?? "0"))
            .OrderBy(value => value)
            .ToList();
        var expectedSortOrders = nodeSequences
            .Select(sequence => checked(sequence * 128))
            .OrderBy(value => value)
            .ToList();
        Assert(actualSortOrders.SequenceEqual(expectedSortOrders),
            "Sub WBS 与 WBS Activity2 的 sort_order 必须按 Excel 顺序 × 128 写入。");
    }

    private static IReadOnlyList<string> SplitPredecessorCodes(string value) =>
        value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static void AssertPerParentPrevItemChains(
        XDocument aml,
        string rootWbsId,
        int expectedNodeCount)
    {
        var root = aml.Root!.Elements("Item")
            .Single(item => (string?)item.Attribute("type") == "WBS Element" &&
                            (string?)item.Attribute("id") == rootWbsId);
        Assert(root.Element("prev_item") == null, "顶层 WBS 不应设置 prev_item。");

        var hierarchyItems = aml.Root.Elements("Item")
            .Where(item => (string?)item.Attribute("type") is "WBS Element" or "Activity2")
            .Where(item => (string?)item.Attribute("id") != rootWbsId)
            .ToDictionary(item => (string)item.Attribute("id")!, StringComparer.OrdinalIgnoreCase);
        var relationships = aml.Root.Elements("Item")
            .Where(item => (string?)item.Attribute("type") is "Sub WBS" or "WBS Activity2")
            .Select(item => new
            {
                SourceId = item.Element("source_id")?.Value ?? string.Empty,
                RelatedId = item.Element("related_id")?.Value ?? string.Empty,
                SortOrder = int.Parse(item.Element("sort_order")?.Value ?? "0")
            })
            .ToList();
        Assert(hierarchyItems.Count == expectedNodeCount,
            $"AML 计划节点数量不正确：{hierarchyItems.Count}/{expectedNodeCount}。");
        Assert(relationships.Count == expectedNodeCount,
            $"AML 层级关系数量不正确：{relationships.Count}/{expectedNodeCount}。");

        var covered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var siblings in relationships
                     .GroupBy(item => item.SourceId, StringComparer.OrdinalIgnoreCase)
                     .Select(group => group.OrderBy(item => item.SortOrder).ToList()))
        {
            for (var index = 0; index < siblings.Count; index++)
            {
                var sibling = siblings[index];
                var expectedPreviousId = index == 0 ? string.Empty : siblings[index - 1].RelatedId;
                var actualPreviousId = hierarchyItems[sibling.RelatedId].Element("prev_item")?.Value ?? string.Empty;
                Assert(actualPreviousId == expectedPreviousId,
                    $"同级 prev_item 错误：节点 {sibling.RelatedId} 应指向“{expectedPreviousId}”，实际为“{actualPreviousId}”。");
                Assert(covered.Add(sibling.RelatedId),
                    $"节点 {sibling.RelatedId} 被重复挂载到多个父 WBS。");
            }
        }
        Assert(covered.Count == expectedNodeCount,
            $"同级 prev_item 链未覆盖全部计划节点：{covered.Count}/{expectedNodeCount}。");
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
        int PrevItemCount,
        int NumberedActivityCount);

    private sealed record HierarchyChild(string RelatedId, string ItemType, int SortOrder);
}

public sealed class FakeInnovator
{
    private int _id;
    private readonly IReadOnlyDictionary<string, string> _projectRoleValues;
    public int ImportCalls { get; private set; }
    public int ProjectRoleQueryCalls { get; private set; }

    public FakeInnovator(IReadOnlyDictionary<string, string>? projectRoleValues = null)
    {
        _projectRoleValues = projectRoleValues ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public string getNewID() => (++_id).ToString("X32");

    public FakeQuery newItem(string type, string action) => new(type, action);

    public FakeArasResult applyAML(string aml)
    {
        var document = XDocument.Parse(aml);
        var item = document.Root?.Element("Item");
        if (item != null &&
            string.Equals((string?)item.Attribute("type"), "List", StringComparison.Ordinal) &&
            string.Equals((string?)item.Attribute("action"), "get", StringComparison.Ordinal) &&
            string.Equals(item.Element("keyed_name")?.Value, "Project Role", StringComparison.Ordinal))
        {
            ProjectRoleQueryCalls++;
            var roles = _projectRoleValues
                .Select(pair => new FakeArasItem(string.Empty, new Dictionary<string, string>
                {
                    ["label"] = pair.Key,
                    ["value"] = pair.Value
                }))
                .ToList();
            return new FakeArasResult(
                [new FakeArasItem("F0000000000000000000000000000002")],
                new FakeArasResult(roles));
        }

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
    private readonly IReadOnlyList<FakeArasItem> _items;
    private readonly FakeArasResult? _relationships;

    public FakeArasResult(int count)
        : this(Enumerable.Range(0, count)
            .Select(_ => new FakeArasItem("F0000000000000000000000000000001"))
            .ToList()) { }

    public FakeArasResult(IReadOnlyList<FakeArasItem> items, FakeArasResult? relationships = null)
    {
        _items = items;
        _relationships = relationships;
    }

    public bool isError() => false;
    public string getErrorString() => string.Empty;
    public int getItemCount() => _items.Count;
    public FakeArasItem getItemByIndex(int index) => _items[index];
    public FakeArasResult getRelationships() => _relationships ?? new FakeArasResult(0);
}

public sealed class FakeArasItem
{
    private readonly string _id;
    private readonly IReadOnlyDictionary<string, string> _properties;

    public FakeArasItem(string id, IReadOnlyDictionary<string, string>? properties = null)
    {
        _id = id;
        _properties = properties ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public string getID() => _id;
    public string getProperty(string name, string defaultValue) =>
        _properties.TryGetValue(name, out var value) ? value : defaultValue;
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
