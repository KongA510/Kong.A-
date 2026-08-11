using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 工作流程设定服务。
///
/// Aras 数据结构：
/// Workflow Map
///   └─ Workflow Map Activity（关系）
///        └─ Activity Template（实际节点，dependent Item）
///             ├─ Activity Template Assignment（执行角色）
///             └─ Workflow Map Path（出站路径）
///
/// Workflow Map Path.source_id 指向来源 Activity Template，related_id 指向目标 Activity Template。
/// segments 保存绝对坐标折点，格式为 x,y|x,y；x/y 保存路径名称相对来源节点的偏移。
/// </summary>
public class WorkflowMapService : IWorkflowMapService
{
    private const string SheetNodes = "流程节点";
    private const string SheetPaths = "流程路径";
    private const string SheetInstructions = "填写说明";

    /// <summary>
    /// Creator Identity 固定 ID 预留位。
    /// 用户提供 R37 环境中的 ID 后只需在此填写 32 位 GUID；留空时 AML 会按名称 Creator 查询，
    /// 因而当前功能仍可正常验证，不会把环境相关 ID 写入 Excel 模板。
    /// </summary>
    private const string CreatorIdentityId = "";

    private readonly IArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public WorkflowMapService(
        IArasConnectionService connectionService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _connectionService = connectionService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <inheritdoc />
    public byte[] GenerateTemplate()
    {
        try
        {
            using var package = new ExcelPackage();
            var nodes = package.Workbook.Worksheets.Add(SheetNodes);
            var paths = package.Workbook.Worksheets.Add(SheetPaths);
            var instructions = package.Workbook.Worksheets.Add(SheetInstructions);

            BuildNodeSheet(nodes);
            BuildPathSheet(paths);
            BuildInstructionSheet(instructions);

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _ = _errorLogService.LogErrorAsync("工作流程设定-生成模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<WorkflowMapDefinition> ParseTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("工作流程 Excel 文件不存在。", filePath);

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var package = new ExcelPackage(new FileInfo(filePath));
                var nodeSheet = package.Workbook.Worksheets[SheetNodes]
                    ?? throw new InvalidDataException($"缺少工作表“{SheetNodes}”。");
                var pathSheet = package.Workbook.Worksheets[SheetPaths]
                    ?? throw new InvalidDataException($"缺少工作表“{SheetPaths}”。");

                var definition = new WorkflowMapDefinition { SourceFilePath = filePath };
                ReadNodes(nodeSheet, definition, cancellationToken);
                ReadPaths(pathSheet, definition, cancellationToken);
                NormalizeAndValidate(definition);
                return definition;
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("工作流程设定-解析模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    /// <inheritdoc />
    public void NormalizeAndValidate(WorkflowMapDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        definition.Warnings.Clear();

        if (definition.Nodes.Count == 0)
            throw new InvalidDataException("流程节点不能为空。");
        if (definition.Paths.Count == 0)
            throw new InvalidDataException("流程路径不能为空。");

        var codeLookup = new Dictionary<string, WorkflowMapNode>(StringComparer.OrdinalIgnoreCase);
        var nameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < definition.Nodes.Count; index++)
        {
            var node = definition.Nodes[index];
            node.Code = node.Code.Trim();
            node.Name = node.Name.Trim();
            node.ActivityType = NormalizeNodeType(node.ActivityType);
            node.SortOrder = (index + 1) * 128;

            if (string.IsNullOrWhiteSpace(node.Code))
                throw new InvalidDataException($"“{SheetNodes}”第 {index + 2} 行缺少节点编码。");
            if (string.IsNullOrWhiteSpace(node.Name))
                throw new InvalidDataException($"节点 {node.Code} 缺少节点名称。");
            if (!codeLookup.TryAdd(node.Code, node))
                throw new InvalidDataException($"节点编码重复：{node.Code}。");
            if (!nameSet.Add(node.Name))
                throw new InvalidDataException($"节点名称重复：{node.Name}。为便于 Aras 维护，每个节点名称必须唯一。");

            // 开始和结束节点均由 Aras 自动推进，不应等待人工投票。
            if (node.IsStart || node.IsEnd)
                node.IsAutomatic = true;

            if (!node.IsStart && !node.IsEnd && !node.IsAutomatic && string.IsNullOrWhiteSpace(node.Assignee))
                throw new InvalidDataException($"人工节点“{node.Name}”必须填写执行角色（Identity 名称或 ID）。");

            EnsureNodeIds(node);
        }

        var startNodes = definition.Nodes.Where(node => node.IsStart).ToList();
        if (startNodes.Count != 1)
            throw new InvalidDataException($"一份工作流程必须且只能有一个开始节点，当前为 {startNodes.Count} 个。");
        if (!definition.Nodes.Any(node => node.IsEnd))
            throw new InvalidDataException("一份工作流程至少需要一个结束节点。");

        var duplicatePathSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var returnPathIndex = 0;
        for (var index = 0; index < definition.Paths.Count; index++)
        {
            var path = definition.Paths[index];
            path.SourceCode = path.SourceCode.Trim();
            path.TargetCode = path.TargetCode.Trim();
            path.Name = path.Name.Trim();
            path.Authentication = NormalizeAuthentication(path.Authentication);
            path.SortOrder = (index + 1) * 128;

            if (!codeLookup.TryGetValue(path.SourceCode, out var source))
                throw new InvalidDataException($"路径“{path.Name}”的来源节点不存在：{path.SourceCode}。");
            if (!codeLookup.TryGetValue(path.TargetCode, out var target))
                throw new InvalidDataException($"路径“{path.Name}”的目标节点不存在：{path.TargetCode}。");
            if (source.IsEnd)
                throw new InvalidDataException($"结束节点“{source.Name}”不能再配置出站路径。");
            if (string.IsNullOrWhiteSpace(path.Name))
                throw new InvalidDataException($"从 {source.Name} 到 {target.Name} 的路径缺少名称。");

            var duplicateKey = $"{path.SourceCode}|{path.TargetCode}|{path.Name}";
            if (!duplicatePathSet.Add(duplicateKey))
                throw new InvalidDataException($"路径重复：{source.Name} → {target.Name}（{path.Name}）。");

            // 先验证用户填写的 segments，再为没有折点的退回路径自动生成上方正交折线。
            var points = ParseSegments(path.Segments);
            path.Segments = SerializeSegments(points);
            if (target.SortOrder <= source.SortOrder && points.Count == 0)
            {
                var routeY = Math.Min(source.Y, target.Y) - 90 - returnPathIndex * 48;
                path.Segments = SerializeSegments([
                    new WorkflowMapPoint(source.X, routeY),
                    new WorkflowMapPoint(target.X, routeY)
                ]);
                returnPathIndex++;
                definition.Warnings.Add($"退回路径“{path.Name}”未填写转折点，已自动生成：{path.Segments}。");
            }

            EnsurePathId(path);
            EnsureLabelOffset(path, source, target);
        }

        // Workflow Map Path 是 Activity Template 的子关系，sort_order 应在每个来源节点内独立排序。
        foreach (var sourceGroup in definition.Paths.GroupBy(path => path.SourceCode, StringComparer.OrdinalIgnoreCase))
        {
            var localIndex = 0;
            foreach (var path in sourceGroup)
                path.SortOrder = ++localIndex * 128;
        }

        ValidateAutomaticActivities(definition, codeLookup);
        ValidateConnectivity(definition, startNodes[0], codeLookup);
        EnsureDefinitionIds(definition);
    }

    /// <inheritdoc />
    public IReadOnlyList<WorkflowMapPoint> ParseSegments(string segments)
    {
        if (string.IsNullOrWhiteSpace(segments))
            return Array.Empty<WorkflowMapPoint>();

        var points = new List<WorkflowMapPoint>();
        var tokens = segments.Trim().Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var token in tokens)
        {
            var coordinates = token.Split(',', StringSplitOptions.TrimEntries);
            if (coordinates.Length != 2 ||
                !int.TryParse(coordinates[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var x) ||
                !int.TryParse(coordinates[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
            {
                throw new InvalidDataException(
                    $"转折点格式无效：{segments}。正确格式为 x,y|x,y，例如 390,70|220,70。");
            }

            points.Add(new WorkflowMapPoint(x, y));
        }

        return points;
    }

    /// <inheritdoc />
    public string SerializeSegments(IEnumerable<WorkflowMapPoint> points)
        => string.Join('|', points.Select(point =>
            $"{point.X.ToString(CultureInfo.InvariantCulture)},{point.Y.ToString(CultureInfo.InvariantCulture)}"));

    /// <inheritdoc />
    public async Task<WorkflowMapPreparedAml> PrepareAmlAsync(
        WorkflowMapDefinition definition,
        string workflowName,
        string description,
        string importMode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            workflowName = workflowName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(workflowName))
                throw new InvalidDataException("请手动输入工作流程模板名称。");

            var isAddMode = string.Equals(importMode, "新增", StringComparison.OrdinalIgnoreCase);
            var isOverwriteMode = string.Equals(importMode, "覆盖", StringComparison.OrdinalIgnoreCase);
            if (!isAddMode && !isOverwriteMode)
                throw new InvalidDataException($"不支持的导入模式：{importMode}。请选择“新增”或“覆盖”。");

            NormalizeAndValidate(definition);
            var innovator = GetInnovatorOrThrow();
            // IOM applyAML 是同步网络调用，放入后台线程避免阻塞 WinUI。
            // 校验与 ObservableCollection 修正已在 await 之前完成，不会从后台线程修改已绑定集合。
            ExistingWorkflowMap? existing = await Task.Run(
                () => FindExistingMap(innovator, workflowName), cancellationToken).ConfigureAwait(false);
            if (!isOverwriteMode && existing != null)
                throw new InvalidOperationException($"系统中已存在同名 Workflow Map：{workflowName}。请选择“覆盖”模式或更换名称。");

            cancellationToken.ThrowIfCancellationRequested();
            if (existing != null)
                definition.MapId = existing.MapId;
            else
                EnsureDefinitionIds(definition);

            var aml = BuildAml(definition, workflowName, description?.Trim() ?? string.Empty,
                existing != null, existing?.MapActivityIds ?? []);

            return new WorkflowMapPreparedAml
            {
                Aml = aml,
                MapId = definition.MapId,
                IsOverwrite = existing != null,
                RemovedActivityCount = existing?.MapActivityIds.Count ?? 0
            };
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("工作流程设定-生成AML", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<WorkflowMapImportResult> ImportAsync(
        WorkflowMapPreparedAml preparedAml,
        string workflowName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(preparedAml);
            if (string.IsNullOrWhiteSpace(preparedAml.Aml))
                throw new InvalidDataException("待执行 AML 为空，请先生成预览 AML。");

            cancellationToken.ThrowIfCancellationRequested();
            dynamic innovator = GetInnovatorOrThrow();
            dynamic result = innovator.applyAML(preparedAml.Aml);
            cancellationToken.ThrowIfCancellationRequested();

            if (result == null)
                throw new InvalidOperationException("Aras 未返回导入结果。");
            if (result.isError())
                throw new InvalidOperationException((string)result.getErrorString());

            await _operationLogService.LogAsync(
                "Import",
                "WorkflowMap",
                preparedAml.MapId,
                $"{(preparedAml.IsOverwrite ? "覆盖" : "新增")}工作流程模板：{workflowName}，AML 一次性执行完成")
                .ConfigureAwait(false);

            return new WorkflowMapImportResult
            {
                IsSuccess = true,
                MapId = preparedAml.MapId,
                ExecutedAml = preparedAml.Aml,
                Message = preparedAml.IsOverwrite
                    ? $"已覆盖工作流程模板“{workflowName}”，保留 Workflow Map ID：{preparedAml.MapId}。"
                    : $"已新增工作流程模板“{workflowName}”，Workflow Map ID：{preparedAml.MapId}。"
            };
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("工作流程设定-导入Aras", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private static void BuildNodeSheet(ExcelWorksheet sheet)
    {
        var headers = new[]
        {
            "顺序", "节点编码", "节点名称", "节点类型", "执行角色", "是否自动", "X坐标", "Y坐标", "提示信息"
        };
        WriteHeaders(sheet, headers);

        object?[][] rows =
        [
            [1, "START", "开始", "开始", "", "是", 60, 170, "工作流程开始"],
            [2, "CREATE", "填写送审单", "普通", "Creator", "否", 220, 170, "请填写并提交送审资料"],
            [3, "REVIEW", "对应角色审核", "普通", "Creator", "否", 390, 170, "请审核送审资料"],
            [4, "NOTICE", "通知相关人员", "普通", "Creator", "否", 560, 170, "请确认处理结果"],
            [5, "END", "结束", "结束", "", "否", 730, 170, "工作流程结束"],
            [6, "CANCEL", "取消", "结束", "", "否", 300, 300, "工作流程取消"]
        ];
        WriteRows(sheet, rows);

        var typeValidation = sheet.DataValidations.AddListValidation("D2:D500");
        typeValidation.Formula.Values.Add("开始");
        typeValidation.Formula.Values.Add("普通");
        typeValidation.Formula.Values.Add("结束");
        var boolValidation = sheet.DataValidations.AddListValidation("F2:F500");
        boolValidation.Formula.Values.Add("是");
        boolValidation.Formula.Values.Add("否");

        FinalizeSheet(sheet, headers.Length, 500);
        sheet.Column(2).Width = 18;
        sheet.Column(3).Width = 24;
        sheet.Column(5).Width = 22;
        sheet.Column(9).Width = 34;
    }

    private static void BuildPathSheet(ExcelWorksheet sheet)
    {
        var headers = new[]
        {
            "顺序", "来源节点编码", "目标节点编码", "路径名称", "是否默认", "是否覆盖", "认证方式",
            "转折点segments", "名称偏移X", "名称偏移Y"
        };
        WriteHeaders(sheet, headers);

        object?[][] rows =
        [
            [1, "START", "CREATE", "开始", "是", "否", "none", "", null, null],
            [2, "CREATE", "REVIEW", "送相关人员审核", "否", "否", "none", "", null, null],
            [3, "REVIEW", "NOTICE", "通过", "否", "否", "none", "", null, null],
            [4, "NOTICE", "END", "结束", "否", "否", "none", "", null, null],
            // 退回线使用两个绝对坐标折点，形成与主线分离的上方正交路径。
            [5, "REVIEW", "CREATE", "退回到建立者", "否", "否", "none", "390,75|220,75", -85, -105],
            [6, "CREATE", "CANCEL", "取消流程", "否", "否", "none", "260,235", 45, 70]
        ];
        WriteRows(sheet, rows);

        foreach (var column in new[] { "E", "F" })
        {
            var validation = sheet.DataValidations.AddListValidation($"{column}2:{column}500");
            validation.Formula.Values.Add("是");
            validation.Formula.Values.Add("否");
        }
        var authValidation = sheet.DataValidations.AddListValidation("G2:G500");
        authValidation.Formula.Values.Add("none");
        authValidation.Formula.Values.Add("password");
        authValidation.Formula.Values.Add("esignature");

        FinalizeSheet(sheet, headers.Length, 500);
        sheet.Column(2).Width = 20;
        sheet.Column(3).Width = 20;
        sheet.Column(4).Width = 24;
        sheet.Column(7).Width = 16;
        sheet.Column(8).Width = 28;
    }

    private static void BuildInstructionSheet(ExcelWorksheet sheet)
    {
        sheet.Cells["A1"].Value = "工作流程设定模板填写说明";
        sheet.Cells["A1"].Style.Font.Bold = true;
        sheet.Cells["A1"].Style.Font.Size = 16;
        sheet.Cells["A3"].Value = "一份文件只生成一份 Workflow Map；流程名称在导入页面手动输入，不写入模板。";
        sheet.Cells["A4"].Value = "节点编码必须唯一；节点类型只能是开始、普通、结束；必须且只能有一个开始节点。";
        sheet.Cells["A5"].Value = "人工普通节点必须填写执行角色，可填 Identity 名称或 32 位 ID；开始/结束/自动节点可留空。";
        sheet.Cells["A6"].Value = "转折点使用 Aras 原生 segments 格式：单点 x,y；多点 x,y|x,y，例如 390,75|220,75。";
        sheet.Cells["A7"].Value = "退回路径若未填写转折点，预览时会自动在主线之上生成两个折点，避免与正向路径重合。";
        sheet.Cells["A8"].Value = "名称偏移X/Y对应 Workflow Map Path.x/y，是相对来源节点的路径名称位置；留空会自动计算。";
        sheet.Cells["A9"].Value = "覆盖模式保留原 Workflow Map ID，在一个完整 AML 中删除旧 Workflow Map Activity 后重建节点和路径。";
        sheet.Cells["A10"].Value = "导入前必须先生成并检查可编辑预览；最终通过 Innovator.applyAML 一次性执行完整 AML。";
        sheet.Column(1).Width = 120;
        sheet.Cells["A1:A10"].Style.WrapText = true;
        sheet.View.ShowGridLines = false;
    }

    private static void ReadNodes(
        ExcelWorksheet sheet,
        WorkflowMapDefinition definition,
        CancellationToken cancellationToken)
    {
        if (sheet.Dimension == null)
            return;

        var automaticX = 60;
        for (var row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var code = ReadText(sheet, row, 2);
            var name = ReadText(sheet, row, 3);
            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(name))
                continue;

            var sequence = ReadOptionalInt(sheet, row, 1) ?? definition.Nodes.Count + 1;
            var x = ReadOptionalInt(sheet, row, 7) ?? automaticX;
            var y = ReadOptionalInt(sheet, row, 8) ?? 170;
            automaticX = Math.Max(automaticX + 160, x + 160);

            definition.Nodes.Add(new WorkflowMapNode
            {
                SortOrder = sequence * 128,
                Code = code,
                Name = name,
                ActivityType = ReadText(sheet, row, 4),
                Assignee = ReadText(sheet, row, 5),
                IsAutomatic = ReadBoolean(sheet, row, 6),
                X = x,
                Y = y,
                Message = ReadText(sheet, row, 9)
            });
        }

        // 按“顺序”列稳定排序，保证退回路径判断与用户表格一致。
        var ordered = definition.Nodes.OrderBy(node => node.SortOrder).ToList();
        definition.Nodes.Clear();
        foreach (var node in ordered)
            definition.Nodes.Add(node);
    }

    private static void ReadPaths(
        ExcelWorksheet sheet,
        WorkflowMapDefinition definition,
        CancellationToken cancellationToken)
    {
        if (sheet.Dimension == null)
            return;

        for (var row = 2; row <= sheet.Dimension.End.Row; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var source = ReadText(sheet, row, 2);
            var target = ReadText(sheet, row, 3);
            var name = ReadText(sheet, row, 4);
            if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target) && string.IsNullOrWhiteSpace(name))
                continue;

            var sequence = ReadOptionalInt(sheet, row, 1) ?? definition.Paths.Count + 1;
            definition.Paths.Add(new WorkflowMapPath
            {
                SortOrder = sequence * 128,
                SourceCode = source,
                TargetCode = target,
                Name = name,
                IsDefault = ReadBoolean(sheet, row, 5),
                IsOverride = ReadBoolean(sheet, row, 6),
                Authentication = ReadText(sheet, row, 7),
                Segments = ReadText(sheet, row, 8),
                LabelOffsetX = ReadOptionalInt(sheet, row, 9),
                LabelOffsetY = ReadOptionalInt(sheet, row, 10)
            });
        }

        var ordered = definition.Paths.OrderBy(path => path.SortOrder).ToList();
        definition.Paths.Clear();
        foreach (var path in ordered)
            definition.Paths.Add(path);
    }

    private string BuildAml(
        WorkflowMapDefinition definition,
        string workflowName,
        string description,
        bool isEdit,
        IReadOnlyList<string> existingMapActivityIds)
    {
        var mapItem = new XElement("Item",
            new XAttribute("type", "Workflow Map"),
            new XAttribute("action", isEdit ? "edit" : "add"),
            new XAttribute("id", definition.MapId),
            new XElement("name", workflowName),
            new XElement("description", description),
            BuildProcessOwner(),
            new XElement("transition_line_color"),
            new XElement("transition_name_color"),
            new XElement("transition_name_font"));

        var relationships = new XElement("Relationships");

        // 覆盖时仅删除既有 Workflow Map Activity 关系。
        // Activity Template 是 dependent Item，会随关系删除；Workflow Map 本身 ID 保持不变，
        // 从而不会破坏 Allowed Workflow 等对该模板的引用。
        foreach (var relationshipId in existingMapActivityIds)
        {
            relationships.Add(new XElement("Item",
                new XAttribute("type", "Workflow Map Activity"),
                new XAttribute("action", "delete"),
                new XAttribute("id", relationshipId)));
        }

        foreach (var node in definition.Nodes.OrderBy(node => node.SortOrder))
            relationships.Add(BuildMapActivity(definition, node));

        mapItem.Add(relationships);
        return new XElement("AML", mapItem).ToString(SaveOptions.None);
    }

    private XElement BuildMapActivity(WorkflowMapDefinition definition, WorkflowMapNode node)
    {
        var activity = new XElement("Item",
            new XAttribute("type", "Activity Template"),
            new XAttribute("action", "add"),
            new XAttribute("id", node.ActivityId),
            new XElement("can_delegate", node.IsStart || node.IsEnd ? "0" : "1"),
            new XElement("can_refuse", node.IsStart || node.IsEnd ? "0" : "1"),
            new XElement("consolidate_ondelegate", "0"),
            new XElement("expected_duration", "0"),
            new XElement("icon", GetActivityIcon(node)),
            new XElement("is_auto", node.IsAutomatic ? "1" : "0"),
            new XElement("is_end", node.IsEnd ? "1" : "0"),
            new XElement("is_start", node.IsStart ? "1" : "0"),
            new XElement("message", string.IsNullOrWhiteSpace(node.Message) ? node.Name : node.Message),
            new XElement("name", node.Name),
            new XElement("priority", "1"),
            new XElement("reminder_count", "0"),
            new XElement("reminder_interval", "0"),
            new XElement("timeout_duration", "0"),
            new XElement("wait_for_all_inputs", "0"),
            new XElement("wait_for_all_votes", "0"),
            new XElement("x", node.X.ToString(CultureInfo.InvariantCulture)),
            new XElement("y", node.Y.ToString(CultureInfo.InvariantCulture)));

        var activityRelationships = new XElement("Relationships");
        if (!string.IsNullOrWhiteSpace(node.Assignee) && !node.IsStart && !node.IsEnd && !node.IsAutomatic)
            activityRelationships.Add(BuildAssignment(node));

        foreach (var path in definition.Paths
                     .Where(path => string.Equals(path.SourceCode, node.Code, StringComparison.OrdinalIgnoreCase))
                     .OrderBy(path => path.SortOrder))
        {
            var target = definition.Nodes.First(targetNode =>
                string.Equals(targetNode.Code, path.TargetCode, StringComparison.OrdinalIgnoreCase));
            activityRelationships.Add(BuildPath(node, target, path));
        }

        activity.Add(activityRelationships);

        return new XElement("Item",
            new XAttribute("type", "Workflow Map Activity"),
            new XAttribute("action", "add"),
            new XAttribute("id", node.MapActivityId),
            new XElement("related_id", activity),
            new XElement("sort_order", node.SortOrder.ToString(CultureInfo.InvariantCulture)),
            new XElement("source_id", definition.MapId));
    }

    private static XElement BuildAssignment(WorkflowMapNode node)
    {
        return new XElement("Item",
            new XAttribute("type", "Activity Template Assignment"),
            new XAttribute("action", "add"),
            new XAttribute("id", node.AssignmentId),
            new XElement("for_all_members", "0"),
            new XElement("is_required", "1"),
            BuildIdentityReference("related_id", node.Assignee),
            new XElement("sort_order", "128"),
            new XElement("source_id", node.ActivityId),
            new XElement("voting_weight", "100"));
    }

    private static XElement BuildPath(
        WorkflowMapNode source,
        WorkflowMapNode target,
        WorkflowMapPath path)
    {
        EnsureLabelOffset(path, source, target);
        return new XElement("Item",
            new XAttribute("type", "Workflow Map Path"),
            new XAttribute("action", "add"),
            new XAttribute("id", path.PathId),
            new XElement("authentication", path.Authentication),
            new XElement("is_default", path.IsDefault ? "1" : "0"),
            new XElement("is_override", path.IsOverride ? "1" : "0"),
            new XElement("name", path.Name),
            new XElement("related_id", target.ActivityId),
            new XElement("segments", path.Segments),
            new XElement("sort_order", path.SortOrder.ToString(CultureInfo.InvariantCulture)),
            new XElement("source_id", source.ActivityId),
            new XElement("x", (path.LabelOffsetX ?? 0).ToString(CultureInfo.InvariantCulture)),
            new XElement("y", (path.LabelOffsetY ?? 0).ToString(CultureInfo.InvariantCulture)));
    }

    private static XElement BuildProcessOwner()
    {
        if (IsArasId(CreatorIdentityId))
            return new XElement("process_owner", CreatorIdentityId);

        return new XElement("process_owner",
            new XElement("Item",
                new XAttribute("type", "Identity"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("name", "Creator")));
    }

    private static XElement BuildIdentityReference(string propertyName, string identity)
    {
        if (IsArasId(identity))
            return new XElement(propertyName, identity);

        return new XElement(propertyName,
            new XElement("Item",
                new XAttribute("type", "Identity"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("name", identity)));
    }

    private ExistingWorkflowMap? FindExistingMap(dynamic innovator, string workflowName)
    {
        var query = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "Workflow Map"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("name", workflowName),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "Workflow Map Activity"),
                        new XAttribute("action", "get"),
                        new XAttribute("select", "id"))))).ToString(SaveOptions.DisableFormatting);

        dynamic result = innovator.applyAML(query);
        if (result == null)
            throw new InvalidOperationException("查询同名 Workflow Map 时 Aras 未返回结果。");
        if (result.isError())
            throw new InvalidOperationException((string)result.getErrorString());

        var count = (int)result.getItemCount();
        if (count == 0)
            return null;
        if (count > 1)
            throw new InvalidOperationException($"系统中存在 {count} 个同名 Workflow Map，无法安全覆盖：{workflowName}。");

        dynamic map = result.getItemByIndex(0);
        var existing = new ExistingWorkflowMap { MapId = (string)map.getID() };
        dynamic relationships = map.getRelationships("Workflow Map Activity");
        var relationshipCount = (int)relationships.getItemCount();
        for (var index = 0; index < relationshipCount; index++)
        {
            var id = (string)relationships.getItemByIndex(index).getID();
            if (!string.IsNullOrWhiteSpace(id))
                existing.MapActivityIds.Add(id);
        }

        return existing;
    }

    private dynamic GetInnovatorOrThrow()
    {
        if (!_connectionService.IsConnected || _connectionService.InnovatorInstance == null)
            throw new InvalidOperationException("尚未连接 Aras Innovator，请先在“Aras连接”中完成登录。");
        return _connectionService.InnovatorInstance;
    }

    private static void ValidateAutomaticActivities(
        WorkflowMapDefinition definition,
        IReadOnlyDictionary<string, WorkflowMapNode> codeLookup)
    {
        foreach (var node in definition.Nodes.Where(node => node.IsAutomatic && !node.IsEnd))
        {
            var outgoing = definition.Paths
                .Where(path => string.Equals(path.SourceCode, node.Code, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (outgoing.Count == 0)
                throw new InvalidDataException($"自动节点“{node.Name}”至少需要一条出站路径。");

            var defaultCount = outgoing.Count(path => path.IsDefault);
            if (defaultCount == 0)
            {
                outgoing[0].IsDefault = true;
                definition.Warnings.Add($"自动节点“{node.Name}”没有默认路径，已将“{outgoing[0].Name}”设为默认。");
            }
            else if (defaultCount > 1)
            {
                throw new InvalidDataException($"自动节点“{node.Name}”只能有一条默认路径。");
            }
        }
    }

    private static void ValidateConnectivity(
        WorkflowMapDefinition definition,
        WorkflowMapNode start,
        IReadOnlyDictionary<string, WorkflowMapNode> codeLookup)
    {
        foreach (var node in definition.Nodes.Where(node => !node.IsStart))
        {
            if (!definition.Paths.Any(path =>
                    string.Equals(path.TargetCode, node.Code, StringComparison.OrdinalIgnoreCase)))
            {
                definition.Warnings.Add($"节点“{node.Name}”没有入站路径。");
            }
        }

        var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { start.Code };
        var queue = new Queue<string>();
        queue.Enqueue(start.Code);
        while (queue.Count > 0)
        {
            var sourceCode = queue.Dequeue();
            foreach (var path in definition.Paths.Where(path =>
                         string.Equals(path.SourceCode, sourceCode, StringComparison.OrdinalIgnoreCase)))
            {
                if (codeLookup.ContainsKey(path.TargetCode) && reachable.Add(path.TargetCode))
                    queue.Enqueue(path.TargetCode);
            }
        }

        foreach (var node in definition.Nodes.Where(node => !reachable.Contains(node.Code)))
            definition.Warnings.Add($"节点“{node.Name}”无法从开始节点到达。");
    }

    private static void EnsureLabelOffset(
        WorkflowMapPath path,
        WorkflowMapNode source,
        WorkflowMapNode target)
    {
        if (path.LabelOffsetX.HasValue && path.LabelOffsetY.HasValue)
            return;

        var points = new List<WorkflowMapPoint> { new(source.X, source.Y) };
        if (!string.IsNullOrWhiteSpace(path.Segments))
        {
            foreach (var token in path.Segments.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var values = token.Split(',');
                if (values.Length == 2 && int.TryParse(values[0], out var x) && int.TryParse(values[1], out var y))
                    points.Add(new WorkflowMapPoint(x, y));
            }
        }
        points.Add(new WorkflowMapPoint(target.X, target.Y));

        var middle = FindPolylineMiddle(points);
        path.LabelOffsetX ??= middle.X - source.X;
        path.LabelOffsetY ??= middle.Y - source.Y - 18;
    }

    private static WorkflowMapPoint FindPolylineMiddle(IReadOnlyList<WorkflowMapPoint> points)
    {
        if (points.Count < 2)
            return points.Count == 1 ? points[0] : new WorkflowMapPoint(0, 0);

        var lengths = new double[points.Count - 1];
        var total = 0d;
        for (var index = 0; index < points.Count - 1; index++)
        {
            var dx = points[index + 1].X - points[index].X;
            var dy = points[index + 1].Y - points[index].Y;
            lengths[index] = Math.Sqrt(dx * dx + dy * dy);
            total += lengths[index];
        }

        var targetLength = total / 2d;
        var travelled = 0d;
        for (var index = 0; index < lengths.Length; index++)
        {
            if (travelled + lengths[index] >= targetLength)
            {
                var ratio = lengths[index] == 0 ? 0 : (targetLength - travelled) / lengths[index];
                return new WorkflowMapPoint(
                    (int)Math.Round(points[index].X + (points[index + 1].X - points[index].X) * ratio),
                    (int)Math.Round(points[index].Y + (points[index + 1].Y - points[index].Y) * ratio));
            }
            travelled += lengths[index];
        }

        return points[^1];
    }

    private static void EnsureDefinitionIds(WorkflowMapDefinition definition)
    {
        if (!IsArasId(definition.MapId))
            definition.MapId = NewArasId();
        foreach (var node in definition.Nodes)
            EnsureNodeIds(node);
        foreach (var path in definition.Paths)
            EnsurePathId(path);
    }

    private static void EnsureNodeIds(WorkflowMapNode node)
    {
        if (!IsArasId(node.ActivityId)) node.ActivityId = NewArasId();
        if (!IsArasId(node.MapActivityId)) node.MapActivityId = NewArasId();
        if (!string.IsNullOrWhiteSpace(node.Assignee) && !IsArasId(node.AssignmentId))
            node.AssignmentId = NewArasId();
    }

    private static void EnsurePathId(WorkflowMapPath path)
    {
        if (!IsArasId(path.PathId)) path.PathId = NewArasId();
    }

    private static string NewArasId() => Guid.NewGuid().ToString("N").ToUpperInvariant();

    private static bool IsArasId(string? value)
        => value?.Length == 32 && value.All(Uri.IsHexDigit);

    private static string NormalizeNodeType(string value)
    {
        value = value?.Trim() ?? string.Empty;
        return value switch
        {
            "开始" or "Start" or "start" => WorkflowMapNode.NodeTypeStart,
            "结束" or "End" or "end" => WorkflowMapNode.NodeTypeEnd,
            "普通" or "Normal" or "normal" or "" => WorkflowMapNode.NodeTypeNormal,
            _ => throw new InvalidDataException($"未知节点类型：{value}。允许值为开始、普通、结束。")
        };
    }

    private static string NormalizeAuthentication(string value)
    {
        value = string.IsNullOrWhiteSpace(value) ? "none" : value.Trim().ToLowerInvariant();
        return value switch
        {
            "none" or "password" or "esignature" => value,
            _ => throw new InvalidDataException($"未知认证方式：{value}。允许值为 none、password、esignature。")
        };
    }

    private static string GetActivityIcon(WorkflowMapNode node)
    {
        // 使用新版 Innovator/ArasLabs 汇出包中的 SVG 图标路径；取消结束节点显示红色结束图标。
        if (node.IsStart) return "../images/WorkflowStart.svg";
        if (node.IsEnd && (node.Name.Contains("取消", StringComparison.OrdinalIgnoreCase) ||
                           node.Name.Contains("cancel", StringComparison.OrdinalIgnoreCase)))
            return "../images/Delete.svg";
        if (node.IsEnd) return "../images/Checkmark.svg";
        return "../images/WorkflowNode.svg";
    }

    private static string ReadText(ExcelWorksheet sheet, int row, int column)
        => sheet.Cells[row, column].Text?.Trim() ?? string.Empty;

    private static int? ReadOptionalInt(ExcelWorksheet sheet, int row, int column)
    {
        var text = ReadText(sheet, row, column);
        if (string.IsNullOrWhiteSpace(text)) return null;
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
        throw new InvalidDataException($"工作表“{sheet.Name}”第 {row} 行第 {column} 列不是有效整数：{text}。");
    }

    private static bool ReadBoolean(ExcelWorksheet sheet, int row, int column)
    {
        var text = ReadText(sheet, row, column);
        return text.Equals("是", StringComparison.OrdinalIgnoreCase) ||
               text.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               text == "1" ||
               text.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }

    private static void WriteHeaders(ExcelWorksheet sheet, IReadOnlyList<string> headers)
    {
        for (var index = 0; index < headers.Count; index++)
        {
            var cell = sheet.Cells[1, index + 1];
            cell.Value = headers[index];
            cell.Style.Font.Bold = true;
            cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    private static void WriteRows(ExcelWorksheet sheet, IReadOnlyList<object?[]> rows)
    {
        for (var row = 0; row < rows.Count; row++)
        for (var column = 0; column < rows[row].Length; column++)
            sheet.Cells[row + 2, column + 1].Value = rows[row][column];
    }

    private static void FinalizeSheet(ExcelWorksheet sheet, int columnCount, int validationRows)
    {
        sheet.View.FreezePanes(2, 1);
        sheet.View.ShowGridLines = false;
        sheet.Cells[1, 1, Math.Max(2, sheet.Dimension?.End.Row ?? 2), columnCount].AutoFitColumns(10, 30);
        sheet.Cells[1, 1, validationRows, columnCount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        sheet.Row(1).Height = 24;
        sheet.Cells[1, 1, 1, columnCount].AutoFilter = true;
    }

    private sealed class ExistingWorkflowMap
    {
        public string MapId { get; set; } = string.Empty;
        public List<string> MapActivityIds { get; } = [];
    }
}
