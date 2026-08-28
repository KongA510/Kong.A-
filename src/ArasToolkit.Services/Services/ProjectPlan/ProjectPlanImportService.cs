using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 项目计划模板汇入服务。
///
/// 这里使用的 ItemType 与关系来自 Aras 官方社区及 ArasLabs ms-project-importer：
/// Project Template.wbs_id 指向顶层 WBS Element；阶段通过 Sub WBS 组织；
/// 任务/里程碑为 Activity2，并通过 WBS Activity2 挂到阶段；依赖与角色分别使用
/// Predecessor 和 Activity2 Assignment。汇入前会在目标实例逐项确认这些类型真实存在。
/// </summary>
public class ProjectPlanImportService : IProjectPlanImportService
{
    private const string SheetInformation = "模板信息";
    private const string SheetPlan = "项目计划";
    private const string SheetInstructions = "填写说明";
    private const string SupportedTemplateVersion = "1.0";

    private static readonly string[] RequiredHeaders =
    [
        "顺序", "节点编码", "父节点编码", "节点类型", "节点名称", "说明",
        "计划工期(天)", "预计工时(小时)", "项目角色", "分配工时(小时)",
        "前置节点编码", "依赖类型", "提前/滞后(天)"
    ];

    private readonly IArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public ProjectPlanImportService(
        IArasConnectionService connectionService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _connectionService = connectionService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public byte[] GenerateTemplate()
    {
        try
        {
            using var package = new ExcelPackage();
            BuildInformationSheet(package.Workbook.Worksheets.Add(SheetInformation));
            BuildPlanSheet(package.Workbook.Worksheets.Add(SheetPlan));
            BuildInstructionSheet(package.Workbook.Worksheets.Add(SheetInstructions));
            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _ = _errorLogService.LogErrorAsync("项目计划模板-生成模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<ProjectPlanTemplateDefinition> ParseTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("项目计划模板文件不存在。", filePath);

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var package = new ExcelPackage(new FileInfo(filePath));
                var informationSheet = package.Workbook.Worksheets[SheetInformation]
                    ?? throw new InvalidDataException($"缺少工作表“{SheetInformation}”。请使用本功能导出的模板。");
                var planSheet = package.Workbook.Worksheets[SheetPlan]
                    ?? throw new InvalidDataException($"缺少工作表“{SheetPlan}”。请使用本功能导出的模板。");

                var definition = new ProjectPlanTemplateDefinition { SourceFilePath = filePath };
                ReadInformation(informationSheet, definition);
                ReadPlan(planSheet, definition, cancellationToken);
                NormalizeAndValidate(definition);
                return definition;
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("项目计划模板-解析模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public void NormalizeAndValidate(ProjectPlanTemplateDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        definition.Warnings.Clear();
        definition.TemplateName = definition.TemplateName.Trim();
        definition.ManagedByIdentity = definition.ManagedByIdentity.Trim();
        definition.Description = definition.Description.Trim();
        definition.TemplateVersion = definition.TemplateVersion.Trim();

        if (string.IsNullOrWhiteSpace(definition.TemplateName))
            throw new InvalidDataException($"“{SheetInformation}”中必须填写模板名称。");
        if (!string.Equals(definition.TemplateVersion, SupportedTemplateVersion, StringComparison.Ordinal))
            throw new InvalidDataException(
                $"模板版本必须为 {SupportedTemplateVersion}，当前为“{definition.TemplateVersion}”。请重新下载模板。");
        if (definition.Nodes.Count == 0)
            throw new InvalidDataException($"“{SheetPlan}”中至少需要一个阶段和一个任务或里程碑。");

        var codeLookup = new Dictionary<string, ProjectPlanNode>(StringComparer.OrdinalIgnoreCase);
        var orderSet = new HashSet<int>();

        foreach (var node in definition.Nodes)
        {
            node.Code = node.Code.Trim();
            node.ParentCode = node.ParentCode.Trim();
            node.Name = node.Name.Trim();
            node.Description = node.Description.Trim();
            node.ProjectRole = node.ProjectRole.Trim();
            node.PredecessorCodes = NormalizeCodeList(node.PredecessorCodes);
            node.NodeType = NormalizeNodeType(node.NodeType, node.SourceRow);
            node.PrecedenceType = NormalizePrecedenceType(node.PrecedenceType, node.SourceRow);

            if (node.SortOrder <= 0)
                throw new InvalidDataException($"“{SheetPlan}”第 {node.SourceRow} 行的顺序必须为大于 0 的整数。");
            if (!orderSet.Add(node.SortOrder))
                throw new InvalidDataException($"“{SheetPlan}”中的顺序重复：{node.SortOrder}。");
            if (string.IsNullOrWhiteSpace(node.Code))
                throw new InvalidDataException($"“{SheetPlan}”第 {node.SourceRow} 行缺少节点编码。");
            if (node.Code.IndexOfAny([',', ';', '，', '；', '、']) >= 0)
                throw new InvalidDataException($"节点编码“{node.Code}”不能包含逗号、分号或顿号。");
            if (!codeLookup.TryAdd(node.Code, node))
                throw new InvalidDataException($"节点编码重复：{node.Code}。");
            if (string.IsNullOrWhiteSpace(node.Name))
                throw new InvalidDataException($"节点“{node.Code}”缺少节点名称。");
            if (node.ExpectedDuration < 0 || node.WorkEstimate < 0 || node.AssignmentWorkEstimate < 0)
                throw new InvalidDataException($"节点“{node.Code}”的工期和工时不能为负数。");

            if (node.NodeType == ProjectPlanNodeType.Phase)
            {
                if (node.ExpectedDuration != 0 || node.WorkEstimate != 0 ||
                    !string.IsNullOrWhiteSpace(node.ProjectRole) ||
                    !string.IsNullOrWhiteSpace(node.PredecessorCodes))
                {
                    throw new InvalidDataException(
                        $"阶段“{node.Code}”只能用于 WBS 分组，不能填写工期、工时、角色或前置节点。");
                }
                if (!string.IsNullOrWhiteSpace(node.Description))
                    definition.Warnings.Add($"阶段“{node.Name}”的说明仅用于 Excel 预览，不写入 WBS Element。请将执行说明填写在任务中。");
            }
            else if (node.NodeType == ProjectPlanNodeType.Milestone)
            {
                if (node.ExpectedDuration != 0 || node.WorkEstimate != 0 || node.AssignmentWorkEstimate != 0)
                {
                    node.ExpectedDuration = 0;
                    node.WorkEstimate = 0;
                    node.AssignmentWorkEstimate = 0;
                    definition.Warnings.Add($"里程碑“{node.Name}”的工期与工时已按 Aras 规则归零。");
                }
            }

            if (string.IsNullOrWhiteSpace(node.ProjectRole))
            {
                if (node.AssignmentWorkEstimate != 0)
                    throw new InvalidDataException($"节点“{node.Code}”填写分配工时前必须先填写项目角色。");
            }
        }

        var orderedNodes = definition.Nodes.OrderBy(node => node.SortOrder).ToList();
        if (orderedNodes[0].NodeType != ProjectPlanNodeType.Phase)
            throw new InvalidDataException("项目计划的第一个节点必须是阶段（WBS Element）。");

        foreach (var node in orderedNodes)
        {
            if (string.IsNullOrWhiteSpace(node.ParentCode))
            {
                if (node.NodeType != ProjectPlanNodeType.Phase)
                    throw new InvalidDataException(
                        $"任务或里程碑“{node.Code}”必须填写一个阶段作为父节点。Aras 不允许活动直接挂在模板根节点下。");
            }
            else
            {
                if (!codeLookup.TryGetValue(node.ParentCode, out var parent))
                    throw new InvalidDataException($"节点“{node.Code}”的父节点不存在：{node.ParentCode}。");
                if (parent.NodeType != ProjectPlanNodeType.Phase)
                    throw new InvalidDataException($"节点“{node.Code}”的父节点“{parent.Code}”不是阶段。");
                if (parent.SortOrder >= node.SortOrder)
                    throw new InvalidDataException($"父节点“{parent.Code}”必须排在子节点“{node.Code}”之前。");
            }

            var predecessors = SplitCodes(node.PredecessorCodes);
            if (predecessors.Count == 0 && node.LeadLag != 0)
                throw new InvalidDataException($"节点“{node.Code}”填写提前/滞后天数前必须先填写前置节点。");

            foreach (var predecessorCode in predecessors)
            {
                if (!codeLookup.TryGetValue(predecessorCode, out var predecessor))
                    throw new InvalidDataException($"节点“{node.Code}”的前置节点不存在：{predecessorCode}。");
                if (predecessor.NodeType == ProjectPlanNodeType.Phase)
                    throw new InvalidDataException($"前置节点“{predecessorCode}”必须是任务或里程碑，不能是阶段。");
                if (node.NodeType == ProjectPlanNodeType.Phase)
                    throw new InvalidDataException($"阶段“{node.Code}”不能建立 Predecessor 关系。");
                if (string.Equals(node.Code, predecessorCode, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException($"节点“{node.Code}”不能依赖自身。");
            }
        }

        if (!orderedNodes.Any(node => node.NodeType == ProjectPlanNodeType.Phase))
            throw new InvalidDataException("项目计划至少需要一个阶段。");
        if (!orderedNodes.Any(node => node.NodeType != ProjectPlanNodeType.Phase))
            throw new InvalidDataException("项目计划至少需要一个任务或里程碑。");

        ValidateFlattenedTreeOrder(orderedNodes);
        ValidatePredecessorCycles(orderedNodes, codeLookup);
    }

    public async Task<ProjectPlanPreparedImport> PrepareImportAsync(
        ProjectPlanTemplateDefinition definition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            NormalizeAndValidate(definition);
            cancellationToken.ThrowIfCancellationRequested();
            dynamic innovator = GetInnovatorOrThrow();

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                EnsureRequiredItemTypes(innovator, definition);
                EnsureTemplateNameAvailable(innovator, definition.TemplateName);
                EnsureProjectRolesExist(innovator, definition);
                var managedById = ResolveManagedById(innovator, definition);
                return BuildPreparedImport(innovator, definition, managedById);
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var level = ex is InvalidDataException ? ErrorLog.LevelP1 : ErrorLog.LevelP0;
            await _errorLogService.LogErrorAsync("项目计划模板-Aras预检", ex.Message,
                level, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<ProjectPlanImportResult> ImportAsync(
        ProjectPlanPreparedImport preparedImport,
        string templateName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(preparedImport);
            if (string.IsNullOrWhiteSpace(preparedImport.Aml))
                throw new InvalidDataException("待执行 AML 为空，请重新上传并校验模板。");

            cancellationToken.ThrowIfCancellationRequested();
            dynamic innovator = GetInnovatorOrThrow();
            if (preparedImport.Steps.Count == 0)
                throw new InvalidDataException("项目计划汇入步骤为空，请重新上传并校验模板。");

            var appliedSteps = new List<ProjectPlanImportStep>();
            try
            {
                foreach (var step in preparedImport.Steps)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    dynamic result = await Task.Run(() => innovator.applyAML(step.Aml), cancellationToken)
                        .ConfigureAwait(false);
                    ThrowIfArasError(result, step.Description);
                    appliedSteps.Add(step);
                }
            }
            catch
            {
                await RollbackAppliedStepsAsync(innovator, appliedSteps).ConfigureAwait(false);
                throw;
            }

            await TryLogOperationAsync(
                preparedImport.TemplateId,
                $"汇入项目计划模板：{templateName}；阶段/任务/里程碑共 {preparedImport.NodeCount} 个，" +
                $"角色分配 {preparedImport.AssignmentCount} 条，前置关系 {preparedImport.PredecessorCount} 条")
                .ConfigureAwait(false);

            return new ProjectPlanImportResult
            {
                IsSuccess = true,
                TemplateId = preparedImport.TemplateId,
                RootWbsId = preparedImport.RootWbsId,
                Message = $"项目计划模板“{templateName}”已正确汇入 Aras。"
            };
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("项目计划模板-汇入Aras", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private static void ReadInformation(
        ExcelWorksheet sheet,
        ProjectPlanTemplateDefinition definition)
    {
        if (sheet.Dimension == null)
            throw new InvalidDataException($"工作表“{SheetInformation}”为空。");

        for (var row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            var field = sheet.Cells[row, 1].Text.Trim();
            var value = sheet.Cells[row, 2].Text.Trim();
            switch (field)
            {
                case "模板名称": definition.TemplateName = value; break;
                case "管理身份": definition.ManagedByIdentity = value; break;
                case "模板说明": definition.Description = value; break;
                case "模板版本": definition.TemplateVersion = value; break;
            }
        }
    }

    private static void ReadPlan(
        ExcelWorksheet sheet,
        ProjectPlanTemplateDefinition definition,
        CancellationToken cancellationToken)
    {
        if (sheet.Dimension == null)
            throw new InvalidDataException($"工作表“{SheetPlan}”为空。");

        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var column = 1; column <= sheet.Dimension.End.Column; column++)
        {
            var header = sheet.Cells[1, column].Text.Trim();
            if (!string.IsNullOrWhiteSpace(header))
                headers[header] = column;
        }

        foreach (var requiredHeader in RequiredHeaders)
        {
            if (!headers.ContainsKey(requiredHeader))
                throw new InvalidDataException($"工作表“{SheetPlan}”缺少列“{requiredHeader}”。请重新下载模板。");
        }

        for (var row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (RequiredHeaders.All(header => string.IsNullOrWhiteSpace(CellText(sheet, row, headers[header]))))
                continue;

            definition.Nodes.Add(new ProjectPlanNode
            {
                SourceRow = row,
                SortOrder = ParseRequiredInt(CellText(sheet, row, headers["顺序"]), row, "顺序"),
                Code = CellText(sheet, row, headers["节点编码"]),
                ParentCode = CellText(sheet, row, headers["父节点编码"]),
                NodeType = CellText(sheet, row, headers["节点类型"]),
                Name = CellText(sheet, row, headers["节点名称"]),
                Description = CellText(sheet, row, headers["说明"]),
                ExpectedDuration = ParseOptionalDecimal(CellText(sheet, row, headers["计划工期(天)"]), row, "计划工期(天)"),
                WorkEstimate = ParseOptionalDecimal(CellText(sheet, row, headers["预计工时(小时)"]), row, "预计工时(小时)"),
                ProjectRole = CellText(sheet, row, headers["项目角色"]),
                AssignmentWorkEstimate = ParseOptionalDecimal(CellText(sheet, row, headers["分配工时(小时)"]), row, "分配工时(小时)"),
                PredecessorCodes = CellText(sheet, row, headers["前置节点编码"]),
                PrecedenceType = CellText(sheet, row, headers["依赖类型"]),
                LeadLag = ParseOptionalDecimal(CellText(sheet, row, headers["提前/滞后(天)"]), row, "提前/滞后(天)")
            });
        }
    }

    private static void EnsureRequiredItemTypes(dynamic innovator, ProjectPlanTemplateDefinition definition)
    {
        var itemTypes = new List<string>
        {
            "Project Template", "WBS Element", "Activity2", "Sub WBS", "WBS Activity2", "Predecessor"
        };
        if (definition.Nodes.Any(node => !string.IsNullOrWhiteSpace(node.ProjectRole)))
            itemTypes.Add("Activity2 Assignment");

        foreach (var itemTypeName in itemTypes)
        {
            dynamic query = innovator.newItem("ItemType", "get");
            query.setAttribute("select", "id,name");
            query.setProperty("name", itemTypeName);
            dynamic result = query.apply();
            var count = GetQueryItemCount(result, $"检查 ItemType {itemTypeName}");
            if (count != 1)
            {
                throw new InvalidOperationException(
                    $"当前 Aras 实例未找到标准 ItemType“{itemTypeName}”。请确认已安装并启用 Project Management 模块；不会使用猜测关系继续汇入。");
            }
        }
    }

    private static void EnsureTemplateNameAvailable(dynamic innovator, string templateName)
    {
        dynamic query = innovator.newItem("Project Template", "get");
        query.setAttribute("select", "id,name");
        query.setProperty("name", templateName);
        dynamic result = query.apply();
        var count = GetQueryItemCount(result, "检查同名 Project Template");
        if (count > 0)
        {
            throw new InvalidOperationException(
                $"Aras 中已存在同名 Project Template“{templateName}”。为避免破坏既有项目引用，本功能不自动覆盖，请修改模板名称后重新上传。");
        }
    }

    private static void EnsureProjectRolesExist(dynamic innovator, ProjectPlanTemplateDefinition definition)
    {
        var requiredRoles = definition.Nodes
            .Select(node => node.ProjectRole)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (requiredRoles.Count == 0) return;

        var query = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "List"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id,name"),
                new XElement("keyed_name", "Project Role"),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "Value"),
                        new XAttribute("action", "get"),
                        new XAttribute("select", "value"),
                        new XAttribute("orderBy", "value")))))
            .ToString(SaveOptions.DisableFormatting);

        dynamic result = innovator.applyAML(query);
        if (GetQueryItemCount(result, "读取 Project Role 列表") != 1)
            throw new InvalidOperationException("当前 Aras 实例未找到唯一的“Project Role”列表，无法验证任务角色。");

        var existingRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        dynamic relationships = result.getRelationships();
        var relationshipCount = (int)relationships.getItemCount();
        for (var index = 0; index < relationshipCount; index++)
        {
            var value = (string)relationships.getItemByIndex(index).getProperty("value", "");
            if (!string.IsNullOrWhiteSpace(value)) existingRoles.Add(value);
        }

        var missingRoles = requiredRoles.Where(role => !existingRoles.Contains(role)).OrderBy(role => role).ToList();
        if (missingRoles.Count > 0)
        {
            throw new InvalidOperationException(
                $"下列项目角色不在当前 Aras 的 Project Role 列表中：{string.Join("、", missingRoles)}。请先在 Aras 中建立角色或修正 Excel。");
        }
    }

    private static string ResolveManagedById(dynamic innovator, ProjectPlanTemplateDefinition definition)
    {
        dynamic itemTypeQuery = innovator.newItem("ItemType", "get");
        itemTypeQuery.setAttribute("select", "id");
        itemTypeQuery.setProperty("name", "Project Template");
        dynamic itemTypeResult = itemTypeQuery.apply();
        if (GetQueryItemCount(itemTypeResult, "读取 Project Template ItemType") != 1)
            throw new InvalidOperationException("无法唯一确定 Project Template ItemType。");
        var itemTypeId = (string)itemTypeResult.getItemByIndex(0).getID();

        dynamic propertyQuery = innovator.newItem("Property", "get");
        propertyQuery.setAttribute("select", "id,name,is_required");
        propertyQuery.setProperty("source_id", itemTypeId);
        propertyQuery.setProperty("name", "managed_by_id");
        dynamic propertyResult = propertyQuery.apply();
        var propertyCount = GetQueryItemCount(propertyResult, "检查 Project Template.managed_by_id");
        if (propertyCount > 1)
            throw new InvalidOperationException("当前 Aras 中 Project Template.managed_by_id 属性不唯一。");
        if (propertyCount == 0)
        {
            if (!string.IsNullOrWhiteSpace(definition.ManagedByIdentity))
                throw new InvalidOperationException("当前 Aras 未定义 Project Template.managed_by_id，不能汇入 Excel 中的管理身份。");
            return string.Empty;
        }
        var isRequired = propertyCount == 1 &&
                         string.Equals((string)propertyResult.getItemByIndex(0).getProperty("is_required", "0"),
                             "1", StringComparison.Ordinal);

        if (!string.IsNullOrWhiteSpace(definition.ManagedByIdentity))
            return ResolveIdentityId(innovator, definition.ManagedByIdentity, "管理身份");

        if (!isRequired) return string.Empty;

        dynamic aliasQuery = innovator.newItem("Alias", "get");
        aliasQuery.setAttribute("select", "related_id");
        aliasQuery.setProperty("source_id", (string)innovator.getUserID());
        dynamic aliasResult = aliasQuery.apply();
        if (GetQueryItemCount(aliasResult, "读取当前用户 Alias") != 1)
        {
            throw new InvalidOperationException(
                "当前 Aras 的 Project Template.managed_by_id 为必填，但无法唯一确定当前用户的 Alias Identity。" +
                "请在 Excel“模板信息”中填写“管理身份”的完整 Identity 名称。");
        }

        dynamic alias = aliasResult.getItemByIndex(0);
        var identityId = (string)alias.getProperty("related_id", "");
        var identityName = (string)alias.getPropertyAttribute("related_id", "keyed_name", "");
        if (string.IsNullOrWhiteSpace(identityId))
            throw new InvalidOperationException("当前用户 Alias 未关联 Identity，无法设置必填的 managed_by_id。");

        definition.ManagedByIdentity = string.IsNullOrWhiteSpace(identityName) ? identityId : identityName;
        definition.Warnings.Add(
            $"当前实例要求 Project Template.managed_by_id 必填；未在 Excel 指定管理身份，" +
            $"已使用当前登录用户的 Alias Identity“{definition.ManagedByIdentity}”。");
        return identityId;
    }

    private static string ResolveIdentityId(dynamic innovator, string keyedName, string fieldName)
    {
        dynamic query = innovator.newItem("Identity", "get");
        query.setAttribute("select", "id,keyed_name");
        query.setProperty("keyed_name", keyedName);
        dynamic result = query.apply();
        var count = GetQueryItemCount(result, $"验证{fieldName}");
        if (count != 1)
            throw new InvalidOperationException($"{fieldName}“{keyedName}”在当前 Aras 中不存在或不唯一。");
        return (string)result.getItemByIndex(0).getID();
    }

    private static ProjectPlanPreparedImport BuildPreparedImport(
        dynamic innovator,
        ProjectPlanTemplateDefinition definition,
        string managedById)
    {
        var orderedNodes = definition.Nodes.OrderBy(node => node.SortOrder).ToList();
        var rootWbsId = NewArasId(innovator);
        var templateId = NewArasId(innovator);
        var nodeIds = orderedNodes.ToDictionary(
            node => node.Code,
            _ => NewArasId(innovator),
            StringComparer.OrdinalIgnoreCase);

        var items = new List<XElement>();
        var steps = new List<ProjectPlanImportStep>();

        AddStep(items, steps, new XElement("Item",
            new XAttribute("type", "WBS Element"),
            new XAttribute("action", "add"),
            new XAttribute("id", rootWbsId),
            new XElement("name", definition.TemplateName),
            new XElement("is_top", "1")), "建立顶层 WBS");

        var projectTemplate = new XElement("Item",
            new XAttribute("type", "Project Template"),
            new XAttribute("action", "add"),
            new XAttribute("id", templateId),
            new XElement("name", definition.TemplateName),
            new XElement("wbs_id", rootWbsId));
        if (!string.IsNullOrWhiteSpace(managedById))
            projectTemplate.Add(new XElement("managed_by_id", managedById));
        AddStep(items, steps, projectTemplate, "建立 Project Template");

        var previousItemId = rootWbsId;
        foreach (var node in orderedNodes)
        {
            var nodeId = nodeIds[node.Code];
            var parentId = string.IsNullOrWhiteSpace(node.ParentCode)
                ? rootWbsId
                : nodeIds[node.ParentCode];

            if (node.NodeType == ProjectPlanNodeType.Phase)
            {
                AddStep(items, steps, new XElement("Item",
                    new XAttribute("type", "WBS Element"),
                    new XAttribute("action", "add"),
                    new XAttribute("id", nodeId),
                    new XElement("name", node.Name),
                    new XElement("prev_item", previousItemId)), $"建立阶段 {node.Code} - {node.Name}");
                AddStep(items, steps,
                    BuildRelationship("Sub WBS", NewArasId(innovator), parentId, nodeId),
                    $"建立阶段层级关系 {node.Code}");
            }
            else
            {
                var activity = new XElement("Item",
                    new XAttribute("type", "Activity2"),
                    new XAttribute("action", "add"),
                    new XAttribute("id", nodeId),
                    new XElement("name", node.Name),
                    new XElement("prev_item", previousItemId),
                    new XElement("is_milestone", node.NodeType == ProjectPlanNodeType.Milestone ? "1" : "0"),
                    new XElement("work_est", FormatDecimal(node.WorkEstimate)),
                    new XElement("expected_duration", FormatDecimal(node.ExpectedDuration)));
                if (!string.IsNullOrWhiteSpace(node.Description))
                    activity.Add(new XElement("description", node.Description));
                AddStep(items, steps, activity, $"建立{node.DisplayType} {node.Code} - {node.Name}");
                AddStep(items, steps,
                    BuildRelationship("WBS Activity2", NewArasId(innovator), parentId, nodeId),
                    $"建立{node.DisplayType}层级关系 {node.Code}");
            }

            previousItemId = nodeId;
        }

        var assignmentCount = 0;
        foreach (var node in orderedNodes.Where(node => !string.IsNullOrWhiteSpace(node.ProjectRole)))
        {
            AddStep(items, steps, new XElement("Item",
                new XAttribute("type", "Activity2 Assignment"),
                new XAttribute("action", "add"),
                new XAttribute("id", NewArasId(innovator)),
                new XElement("source_id", nodeIds[node.Code]),
                new XElement("role", node.ProjectRole),
                new XElement("work_est", FormatDecimal(node.AssignmentWorkEstimate))),
                $"建立角色分配 {node.Code} - {node.ProjectRole}");
            assignmentCount++;
        }

        var predecessorCount = 0;
        foreach (var node in orderedNodes)
        {
            foreach (var predecessorCode in SplitCodes(node.PredecessorCodes))
            {
                AddStep(items, steps, new XElement("Item",
                    new XAttribute("type", "Predecessor"),
                    new XAttribute("action", "add"),
                    new XAttribute("id", NewArasId(innovator)),
                    new XElement("precedence_type", node.PrecedenceType),
                    new XElement("lead_lag", FormatDecimal(node.LeadLag)),
                    new XElement("source_id", nodeIds[node.Code]),
                    new XElement("related_id", nodeIds[predecessorCode])),
                    $"建立前置关系 {predecessorCode} → {node.Code}");
                predecessorCount++;
            }
        }

        var prepared = new ProjectPlanPreparedImport
        {
            Aml = new XElement("AML", items.Select(item => new XElement(item)))
                .ToString(SaveOptions.DisableFormatting),
            TemplateId = templateId,
            RootWbsId = rootWbsId,
            NodeCount = orderedNodes.Count,
            AssignmentCount = assignmentCount,
            PredecessorCount = predecessorCount
        };
        prepared.Steps.AddRange(steps);
        return prepared;
    }

    private static XElement BuildRelationship(string type, string id, string sourceId, string relatedId) =>
        new("Item",
            new XAttribute("type", type),
            new XAttribute("action", "add"),
            new XAttribute("id", id),
            new XElement("source_id", sourceId),
            new XElement("related_id", relatedId));

    private static void AddStep(
        ICollection<XElement> items,
        ICollection<ProjectPlanImportStep> steps,
        XElement item,
        string description)
    {
        items.Add(item);
        steps.Add(new ProjectPlanImportStep
        {
            ItemType = (string?)item.Attribute("type") ?? string.Empty,
            ItemId = (string?)item.Attribute("id") ?? string.Empty,
            Description = description,
            Aml = new XElement("AML", new XElement(item)).ToString(SaveOptions.DisableFormatting)
        });
    }

    private async Task RollbackAppliedStepsAsync(
        dynamic innovator,
        IReadOnlyList<ProjectPlanImportStep> appliedSteps)
    {
        for (var index = appliedSteps.Count - 1; index >= 0; index--)
        {
            var step = appliedSteps[index];
            try
            {
                var deleteAml = new XElement("AML",
                    new XElement("Item",
                        new XAttribute("type", step.ItemType),
                        new XAttribute("action", "delete"),
                        new XAttribute("id", step.ItemId)))
                    .ToString(SaveOptions.DisableFormatting);
                dynamic result = await Task.Run(() => innovator.applyAML(deleteAml)).ConfigureAwait(false);
                if (result == null) continue;
#pragma warning disable CS8602 // dynamic IOM/FakeItem 已在上方做运行时空值检查。
                if ((bool)result.isError())
                {
                    string code;
                    try { code = (string)result.getErrorCode(); }
                    catch { code = string.Empty; }
                    if (!string.Equals(code, "0", StringComparison.Ordinal))
                    {
                        var message = $"回滚 {step.Description} 失败：{GetArasErrorDescription(result)}";
                        Debug.WriteLine($"[项目计划模板] {message}");
                        await _errorLogService.LogErrorAsync("项目计划模板-回滚", message,
                            ErrorLog.LevelP1).ConfigureAwait(false);
                    }
                }
#pragma warning restore CS8602
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[项目计划模板] 回滚 {step.Description} 异常：{ex.Message}");
                await _errorLogService.LogErrorAsync("项目计划模板-回滚", ex.Message,
                    ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            }
        }
    }

    private static string NewArasId(dynamic innovator)
    {
        var id = (string)innovator.getNewID();
        if (string.IsNullOrWhiteSpace(id) || id.Length != 32)
            throw new InvalidOperationException("Aras IOM 未能生成有效的 32 位 Item ID。");
        return id;
    }

    private dynamic GetInnovatorOrThrow()
    {
        if (!_connectionService.IsConnected || _connectionService.InnovatorInstance == null)
            throw new InvalidOperationException("尚未连接 Aras Innovator，请先在“Aras连接”中完成登录。");
        return _connectionService.InnovatorInstance;
    }

    private static void ThrowIfArasError(dynamic? result, string operation)
    {
        if (result == null)
            throw new InvalidOperationException($"{operation}时 Aras 未返回结果。");
        if ((bool)result.isError())
            throw new InvalidOperationException($"{operation}失败：{GetArasErrorDescription(result)}");
    }

    /// <summary>
    /// Aras IOM 将 get 查询的零行结果表示为 fault code 0（No items found）。
    /// 仅查询可把该结果视为 0 行；导入写入仍由 ThrowIfArasError 严格处理。
    /// </summary>
    private static int GetQueryItemCount(dynamic? result, string operation)
    {
        if (result == null)
            throw new InvalidOperationException($"{operation}时 Aras 未返回结果。");
        if (!(bool)result.isError())
            return (int)result.getItemCount();

        string errorCode;
        try { errorCode = (string)result.getErrorCode(); }
        catch { errorCode = string.Empty; }
        if (string.Equals(errorCode, "0", StringComparison.Ordinal))
            return 0;

        throw new InvalidOperationException($"{operation}失败：{GetArasErrorDescription(result)}");
    }

    private static string GetArasErrorDescription(dynamic result)
    {
        var messages = new List<string>();
        try
        {
            var message = ((string)result.getErrorString())?.Trim();
            if (!string.IsNullOrWhiteSpace(message)) messages.Add(message);
        }
        catch { /* 继续读取 detail。 */ }
        try
        {
            var detail = ((string)result.getErrorDetail())?.Trim();
            if (!string.IsNullOrWhiteSpace(detail) && !messages.Contains(detail)) messages.Add(detail);
        }
        catch { /* 某些 IOM 结果不提供 detail。 */ }
        try
        {
            var raw = ((string?)result.ToString())?.Trim();
            if (!string.IsNullOrWhiteSpace(raw) && !messages.Contains(raw))
                messages.Add(raw.Length > 2000 ? raw[..2000] : raw);
        }
        catch { /* 保留通用错误。 */ }
        if (messages.Count == 1 && messages[0].Contains("faultcode", StringComparison.OrdinalIgnoreCase) &&
            (messages[0].Contains("<faultstring></faultstring>", StringComparison.OrdinalIgnoreCase) ||
             messages[0].Contains("<faultstring />", StringComparison.OrdinalIgnoreCase)))
        {
            return "Aras 返回 faultcode 1 但未提供错误文本。请检查目标 ItemType 上的 onBeforeAdd Server Event，" +
                   "尤其是后加的自定义方法；本次已创建数据将被回滚。";
        }
        return messages.Count == 0 ? "Aras 返回未知错误。" : string.Join(" | ", messages);
    }

    private async Task TryLogOperationAsync(string entityId, string description)
    {
        try
        {
            await _operationLogService.LogAsync(
                "Import", "ProjectTemplate", entityId, description).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[项目计划模板] 操作日志写入失败：{ex.Message}");
            await _errorLogService.LogErrorAsync("项目计划模板-操作日志", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
    }

    private static void ValidatePredecessorCycles(
        IReadOnlyCollection<ProjectPlanNode> nodes,
        IReadOnlyDictionary<string, ProjectPlanNode> codeLookup)
    {
        var state = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        void Visit(ProjectPlanNode node)
        {
            if (state.TryGetValue(node.Code, out var currentState))
            {
                if (currentState == 1)
                    throw new InvalidDataException($"项目计划的前置关系存在循环依赖，涉及节点“{node.Code}”。");
                if (currentState == 2) return;
            }

            state[node.Code] = 1;
            foreach (var predecessorCode in SplitCodes(node.PredecessorCodes))
                Visit(codeLookup[predecessorCode]);
            state[node.Code] = 2;
        }

        foreach (var node in nodes.Where(node => node.NodeType != ProjectPlanNodeType.Phase))
            Visit(node);
    }

    private static string NormalizeNodeType(string rawValue, int row)
    {
        var value = rawValue.Trim();
        if (value.Equals("阶段", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("WBS", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("Phase", StringComparison.OrdinalIgnoreCase))
            return ProjectPlanNodeType.Phase;
        if (value.Equals("任务", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("活动", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("Activity", StringComparison.OrdinalIgnoreCase))
            return ProjectPlanNodeType.Activity;
        if (value.Equals("里程碑", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("Milestone", StringComparison.OrdinalIgnoreCase))
            return ProjectPlanNodeType.Milestone;
        throw new InvalidDataException($"“{SheetPlan}”第 {row} 行的节点类型无效：{rawValue}。只能填写阶段、任务或里程碑。");
    }

    private static string NormalizePrecedenceType(string rawValue, int row)
    {
        var value = string.IsNullOrWhiteSpace(rawValue) ? "FS" : rawValue.Trim();
        return value.ToUpperInvariant() switch
        {
            "FS" or "FINISH TO START" or "完成到开始" => "Finish to Start",
            "FF" or "FINISH TO FINISH" or "完成到完成" => "Finish to Finish",
            "SS" or "START TO START" or "开始到开始" => "Start to Start",
            "SF" or "START TO FINISH" or "开始到完成" => "Start to Finish",
            _ => throw new InvalidDataException(
                $"“{SheetPlan}”第 {row} 行的依赖类型无效：{rawValue}。只能填写 FS、FF、SS 或 SF。")
        };
    }

    private static string NormalizeCodeList(string value) =>
        string.Join(",", SplitCodes(value).Distinct(StringComparer.OrdinalIgnoreCase));

    private static List<string> SplitCodes(string? value) =>
        (value ?? string.Empty)
        .Split([',', ';', '，', '；', '、'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(code => !string.IsNullOrWhiteSpace(code))
        .ToList();

    private static string CellText(ExcelWorksheet sheet, int row, int column) =>
        sheet.Cells[row, column].Text?.Trim() ?? string.Empty;

    private static int ParseRequiredInt(string value, int row, string columnName)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;
        throw new InvalidDataException($"“{SheetPlan}”第 {row} 行“{columnName}”必须为整数，当前为“{value}”。");
    }

    private static decimal ParseOptionalDecimal(string value, int row, string columnName)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var invariant))
            return invariant;
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var current))
            return current;
        throw new InvalidDataException($"“{SheetPlan}”第 {row} 行“{columnName}”必须为数字，当前为“{value}”。");
    }

    private static string FormatDecimal(decimal value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);

    /// <summary>
    /// prev_item 在 Project Management 中是整棵 WBS 树前序展开后的单链表，
    /// 不是每个父节点下各自独立的同级链。因此一旦开始下一个兄弟子树，
    /// 就不能再返回前一个子树添加后代。
    /// </summary>
    private static void ValidateFlattenedTreeOrder(IReadOnlyList<ProjectPlanNode> orderedNodes)
    {
        const string rootKey = "\0";
        var children = orderedNodes
            .GroupBy(node => string.IsNullOrWhiteSpace(node.ParentCode) ? rootKey : node.ParentCode,
                StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.OrderBy(node => node.SortOrder).ToList(),
                StringComparer.OrdinalIgnoreCase);

        var expected = new List<ProjectPlanNode>(orderedNodes.Count);
        void AppendSubtree(ProjectPlanNode node)
        {
            expected.Add(node);
            if (!children.TryGetValue(node.Code, out var descendants)) return;
            foreach (var child in descendants) AppendSubtree(child);
        }

        if (children.TryGetValue(rootKey, out var roots))
        {
            foreach (var root in roots) AppendSubtree(root);
        }

        if (expected.Count != orderedNodes.Count)
            throw new InvalidDataException("项目计划层级无法形成完整的 WBS 树。");

        for (var index = 0; index < orderedNodes.Count; index++)
        {
            if (ReferenceEquals(orderedNodes[index], expected[index])) continue;
            throw new InvalidDataException(
                $"顺序 {orderedNodes[index].SortOrder} 的节点“{orderedNodes[index].Code}”打断了 WBS 树的 prev_item 展开链。" +
                $"此位置应先排“{expected[index].Code}”；请让每个阶段的全部后代连续排列后，再开始下一阶段。");
        }
    }

    private static void BuildInformationSheet(ExcelWorksheet sheet)
    {
        sheet.Cells[1, 1].Value = "字段";
        sheet.Cells[1, 2].Value = "内容";
        sheet.Cells[2, 1].Value = "模板名称";
        sheet.Cells[2, 2].Value = "示例项目计划模板";
        sheet.Cells[3, 1].Value = "管理身份";
        sheet.Cells[4, 1].Value = "模板说明";
        sheet.Cells[4, 2].Value = "用于从标准 WBS 阶段、任务和里程碑创建 Aras Project Template";
        sheet.Cells[5, 1].Value = "模板版本";
        sheet.Cells[5, 2].Value = 1.0;
        sheet.Cells[5, 2].Style.Numberformat.Format = "0.0";
        StyleHeader(sheet.Cells[1, 1, 1, 2]);
        sheet.Cells[2, 2, 4, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[2, 2, 4, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 251, 235));
        sheet.Column(1).Width = 18;
        sheet.Column(2).Width = 72;
        sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        sheet.Cells.Style.WrapText = true;
        sheet.View.FreezePanes(2, 1);
    }

    private static void BuildPlanSheet(ExcelWorksheet sheet)
    {
        for (var column = 0; column < RequiredHeaders.Length; column++)
            sheet.Cells[1, column + 1].Value = RequiredHeaders[column];
        StyleHeader(sheet.Cells[1, 1, 1, RequiredHeaders.Length]);

        object?[][] rows =
        [
            [1, "P1", null, "阶段", "需求与规划", null, null, null, null, null, null, "FS", 0],
            [2, "T1", "P1", "任务", "收集项目需求", "整理范围、目标与约束", 3, 24, null, null, null, "FS", 0],
            [3, "M1", "P1", "里程碑", "需求确认", "需求基线确认完成", 0, 0, null, null, "T1", "FS", 0],
            [4, "P2", null, "阶段", "设计与开发", null, null, null, null, null, null, "FS", 0],
            [5, "T2", "P2", "任务", "方案设计", "完成方案与评审材料", 5, 40, null, null, "M1", "FS", 0],
            [6, "M2", "P2", "里程碑", "设计评审", "设计评审通过", 0, 0, null, null, "T2", "FS", 0]
        ];
        for (var row = 0; row < rows.Length; row++)
        for (var column = 0; column < rows[row].Length; column++)
            sheet.Cells[row + 2, column + 1].Value = rows[row][column];

        var typeValidation = sheet.DataValidations.AddListValidation("D2:D500");
        typeValidation.Formula.Values.Add("阶段");
        typeValidation.Formula.Values.Add("任务");
        typeValidation.Formula.Values.Add("里程碑");
        var dependencyValidation = sheet.DataValidations.AddListValidation("L2:L500");
        dependencyValidation.Formula.Values.Add("FS");
        dependencyValidation.Formula.Values.Add("FF");
        dependencyValidation.Formula.Values.Add("SS");
        dependencyValidation.Formula.Values.Add("SF");

        sheet.Cells[1, 1, 500, RequiredHeaders.Length].AutoFilter = true;
        sheet.View.FreezePanes(2, 1);
        sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        sheet.Cells.Style.WrapText = true;
        sheet.Column(1).Width = 9;
        sheet.Column(2).Width = 15;
        sheet.Column(3).Width = 15;
        sheet.Column(4).Width = 12;
        sheet.Column(5).Width = 26;
        sheet.Column(6).Width = 38;
        for (var column = 7; column <= 10; column++) sheet.Column(column).Width = 18;
        sheet.Column(11).Width = 22;
        sheet.Column(12).Width = 13;
        sheet.Column(13).Width = 18;
    }

    private static void BuildInstructionSheet(ExcelWorksheet sheet)
    {
        sheet.Cells[1, 1].Value = "汇入流程";
        sheet.Cells[1, 2].Value = "说明";
        StyleHeader(sheet.Cells[1, 1, 1, 2]);
        object?[][] rows =
        [
            ["1. 下载模板", "通过工具箱“导出项目计划模板”获取当前标准文件，不要修改工作表名称或表头。"],
            ["2. 填写信息", "在“模板信息”填写唯一模板名称；管理身份可填 Aras Identity 全名，仅当目标属性必填且留空时才使用当前用户 Alias；模板说明仅用于上传预览。"],
            ["3. 上传预检", "工具会先校验层级、循环依赖，再连接 Aras 核对真实 ItemType、同名模板和 Project Role。"],
            ["4. 确认汇入", "预检通过后按 ArasLabs 顺序建立 WBS、Project Template、Activity2 和依赖；任一步失败会逆序回滚本次已建数据。"],
            ["父子规则", "阶段可位于根节点或另一阶段下；任务/里程碑必须挂在阶段下，父节点必须排在子节点之前。"],
            ["prev_item 链", "顺序列必须是 WBS 树的前序展开：首节点指向顶层 WBS，后续节点指向展开序列中的前一节点；每个阶段的后代必须连续。"],
            ["前置节点", "多个任务/里程碑编码优先用英文逗号分隔（例：T1,M1）；同时兼容分号、中文逗号和顿号；系统会逐个建立 Predecessor 并拒绝循环依赖。"],
            ["项目角色", "可留空；填写时必须与当前 Aras 的 Project Role 列表值完全一致，工具不会自动创建角色；分配工时仅在填写角色时使用，无需填写负载率。"],
            ["真实 Aras 模型", "Project Template.wbs_id → WBS Element；Sub WBS 连接阶段；WBS Activity2 连接 Activity2；Activity2 Assignment 仅写角色和分配工时；Predecessor 保存依赖。"],
            ["官方 R37 文档", "https://docs.aras.com/aras-innovator-release-37/creating-projects-37"],
            ["ArasLabs 参考", "https://github.com/ArasLabs/ms-project-importer"]
        ];
        for (var row = 0; row < rows.Length; row++)
        {
            sheet.Cells[row + 2, 1].Value = rows[row][0];
            sheet.Cells[row + 2, 2].Value = rows[row][1];
        }
        sheet.Column(1).Width = 22;
        sheet.Column(2).Width = 110;
        sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        sheet.Cells.Style.WrapText = true;
        sheet.View.FreezePanes(2, 1);
    }

    private static void StyleHeader(ExcelRange range)
    {
        range.Style.Font.Bold = true;
        range.Style.Font.Color.SetColor(Color.White);
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 70, 229));
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    }
}
