using System.Text;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ArasToolkit.Services.Services;

/// <summary>解析最多十级的类结构路径，并覆盖 ItemType.class_structure。</summary>
public sealed class ClassStructureImportService : IClassStructureImportService
{
    private const string DataSheetName = "类结构";
    private const int MaximumDepth = 10;
    private const string LabelLanguages = "en,zc,zt";

    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public ClassStructureImportService(
        ArasConnectionService connectionService,
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
            var dataSheet = package.Workbook.Worksheets.Add(DataSheetName);
            var headers = new List<string> { "路径序号" };
            headers.AddRange(Enumerable.Range(1, MaximumDepth).Select(level => $"第{level}级"));
            headers.Add("备注");

            for (var column = 0; column < headers.Count; column++)
                dataSheet.Cells[1, column + 1].Value = headers[column];

            var examples = new object?[][]
            {
                [1, "模组", "TR1", "产品需求分析说明书", null, null, null, null, null, null, null, "每行表示从一级分类到末级节点的一条完整路径"],
                [2, "模组", "TR2", "软件概要设计", null, null, null, null, null, null, null, "共享的父级名称会自动合并"],
                [3, "模组", "TR2", "硬件概要设计", null, null, null, null, null, null, null, null],
                [4, "芯片", "01.MRD", "产品需求评审单", null, null, null, null, null, null, null, null],
                [5, "芯片", "01.MRD", "市场分析报告", null, null, null, null, null, null, null, null],
                [6, "芯片", "02.PRD", "PRD_Checklist", null, null, null, null, null, null, null, null],
                [7, "芯片", "02.PRD", "PRD_成本评估表", null, null, null, null, null, null, null, null]
            };
            for (var row = 0; row < examples.Length; row++)
            {
                for (var column = 0; column < examples[row].Length; column++)
                    dataSheet.Cells[row + 2, column + 1].Value = examples[row][column];
            }

            var headerRange = dataSheet.Cells[1, 1, 1, headers.Count];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            dataSheet.Row(1).Height = 24;
            dataSheet.View.FreezePanes(2, 2);
            dataSheet.View.ShowGridLines = false;
            dataSheet.Cells[1, 1, examples.Length + 1, headers.Count].AutoFilter = true;
            dataSheet.Column(1).Width = 12;
            for (var column = 2; column <= MaximumDepth + 1; column++)
                dataSheet.Column(column).Width = 21;
            dataSheet.Column(headers.Count).Width = 46;
            dataSheet.Cells[2, 1, examples.Length + 1, headers.Count].Style.VerticalAlignment =
                ExcelVerticalAlignment.Center;

            var instructionSheet = package.Workbook.Worksheets.Add("使用说明");
            instructionSheet.View.ShowGridLines = false;
            instructionSheet.Cells[1, 1, 1, 6].Merge = true;
            instructionSheet.Cells[1, 1].Value = "Aras 类结构汇入模板";
            instructionSheet.Cells[1, 1].Style.Font.Bold = true;
            instructionSheet.Cells[1, 1].Style.Font.Size = 18;
            instructionSheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(17, 24, 39));
            instructionSheet.Row(1).Height = 32;

            var instructions = new[]
            {
                "1. 只编辑“类结构”工作表；每行填写一条从第1级到末级节点的完整路径。",
                "2. 最多支持10级；同一行中间不可留空。末级之后的单元格请保持空白。",
                "3. 相同父级下的同名节点会合并，行顺序决定同级节点在 Aras 中的显示顺序。",
                "4. 不要填写 ItemType 名称。以 Document 为例，第1级应直接填写“模组”或“芯片”。",
                "5. 路径序号和备注仅供阅读，不参与 class_structure 组装；实际 GUID 全部由 Aras Innovator.getNewID() 生成。",
                "6. 每次汇入都会完整替换选中 ItemType 的 class_structure，不会保留或合并旧结构。",
                "7. 正式汇入前请先在页面查看解析预览，并确认目标对象类。"
            };
            for (var index = 0; index < instructions.Length; index++)
            {
                instructionSheet.Cells[index + 3, 1, index + 3, 6].Merge = true;
                instructionSheet.Cells[index + 3, 1].Value = instructions[index];
                instructionSheet.Cells[index + 3, 1].Style.WrapText = true;
                instructionSheet.Row(index + 3).Height = 27;
            }
            instructionSheet.Cells[11, 1, 11, 6].Merge = true;
            instructionSheet.Cells[11, 1].Value = "生成结果示意：<class id=\"根GUID\"><class id=\"节点GUID\" name=\"模组\">...</class></class>";
            instructionSheet.Cells[11, 1].Style.Font.Name = "Consolas";
            instructionSheet.Cells[11, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            instructionSheet.Cells[11, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 250, 251));
            instructionSheet.Column(1).Width = 24;
            for (var column = 2; column <= 6; column++)
                instructionSheet.Column(column).Width = 18;

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            LogErrorBestEffortAsync("类结构汇入-生成模板", ex, ErrorLog.LevelP1)
                .GetAwaiter().GetResult();
            throw;
        }
    }

    public async Task<List<ClassStructureItemType>> QueryItemTypesAsync(
        string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "ItemType"),
                    new XAttribute("action", "get"),
                    new XAttribute("language", LabelLanguages),
                    new XAttribute("select", "id,name,label")));
            var result = Apply(innovator, aml, "读取对象类失败");
            var terms = (keyword ?? string.Empty).Split(
                [' ', '\t', '\r', '\n'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var itemTypes = new List<ClassStructureItemType>();

            for (var index = 0; index < result.getItemCount(); index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var item = result.getItemByIndex(index);
                var target = new ClassStructureItemType
                {
                    Id = item.getID(),
                    Name = item.getProperty("name", string.Empty),
                    LabelEn = item.getProperty("label", string.Empty, "en"),
                    LabelZc = item.getProperty("label", string.Empty, "zc"),
                    LabelZt = item.getProperty("label", string.Empty, "zt")
                };
                if (string.IsNullOrWhiteSpace(target.Id) || string.IsNullOrWhiteSpace(target.Name))
                    continue;
                if (terms.Length > 0 && !terms.All(term => Matches(target, term)))
                    continue;
                itemTypes.Add(target);
            }

            return itemTypes
                .OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await LogErrorBestEffortAsync("类结构汇入-查询对象类", ex, ErrorLog.LevelP1);
            throw;
        }
    }

    public async Task<ClassStructurePreview> AnalyzeTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.Run(() =>
            {
                var hierarchy = ParseTemplate(filePath, cancellationToken);
                return CreatePreview(hierarchy);
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await LogErrorBestEffortAsync("类结构汇入-解析模板", ex, ErrorLog.LevelP1);
            throw;
        }
    }

    public async Task<ClassStructureImportResult> ImportAsync(
        string filePath,
        ClassStructureItemType itemType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(itemType.Id) || string.IsNullOrWhiteSpace(itemType.Name))
            throw new InvalidOperationException("目标对象类无效，请重新查询并选择。");

        try
        {
            var hierarchy = await Task.Run(
                () => ParseTemplate(filePath, cancellationToken), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();
            var classStructure = BuildClassStructureXml(innovator, hierarchy.Root);

            var update = innovator.newItem("ItemType", "edit");
            update.setID(itemType.Id);
            update.setProperty("class_structure", classStructure);
            var applyResult = update.apply();
            if (applyResult.isError())
                throw new InvalidOperationException($"覆盖 class_structure 失败: {applyResult.getErrorString()}");

            cancellationToken.ThrowIfCancellationRequested();
            VerifySavedStructure(innovator, itemType.Id, classStructure);
            var preview = CreatePreview(hierarchy);
            await TryLogOperationAsync(itemType, preview);

            return new ClassStructureImportResult
            {
                ItemTypeId = itemType.Id,
                ItemTypeName = itemType.Name,
                PathCount = preview.PathCount,
                NodeCount = preview.NodeCount,
                MaxDepth = preview.MaxDepth,
                ClassStructureXml = classStructure
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await LogErrorBestEffortAsync($"类结构汇入-{itemType.Name}", ex, ErrorLog.LevelP1);
            throw;
        }
    }

    private static ParsedHierarchy ParseTemplate(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("未找到类结构汇入文件。", filePath);
        if (!string.Equals(Path.GetExtension(filePath), ".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("类结构汇入仅支持 .xlsx 文件，请使用下载的模板。");

        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets[DataSheetName]
            ?? package.Workbook.Worksheets.FirstOrDefault()
            ?? throw new InvalidOperationException("Excel 中没有可读取的工作表。");
        if (worksheet.Dimension == null)
            throw new InvalidOperationException("“类结构”工作表为空。");

        var headerColumns = FindLevelColumns(worksheet);
        var root = new ClassNode(string.Empty);
        var pathKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pathCount = 0;
        var duplicateCount = 0;
        var maxDepth = 0;

        for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var levels = headerColumns
                .Select(column => worksheet.Cells[row, column].Text.Trim())
                .ToArray();
            if (levels.All(string.IsNullOrWhiteSpace))
                continue;

            var firstBlank = Array.FindIndex(levels, string.IsNullOrWhiteSpace);
            var depth = firstBlank < 0 ? levels.Length : firstBlank;
            if (depth == 0)
                throw new InvalidOperationException($"第 {row} 行缺少第1级节点。");
            if (levels.Skip(depth).Any(value => !string.IsNullOrWhiteSpace(value)))
                throw new InvalidOperationException($"第 {row} 行的层级中间存在空白，请连续填写第1级至末级。 ");

            var path = levels.Take(depth).ToArray();
            if (path.Any(name => name.Length > 128))
                throw new InvalidOperationException($"第 {row} 行存在超过 128 个字符的节点名称。");
            var key = string.Join('\u001F', path);
            if (!pathKeys.Add(key))
            {
                duplicateCount++;
                continue;
            }

            var parent = root;
            foreach (var name in path)
                parent = parent.GetOrAddChild(name);
            pathCount++;
            maxDepth = Math.Max(maxDepth, depth);
        }

        if (pathCount == 0)
            throw new InvalidOperationException("模板中没有有效路径，请至少填写一行第1级节点。");

        return new ParsedHierarchy(root, pathCount, duplicateCount, maxDepth);
    }

    private static int[] FindLevelColumns(ExcelWorksheet worksheet)
    {
        var columns = new int[MaximumDepth];
        for (var column = 1; column <= worksheet.Dimension!.End.Column; column++)
        {
            var header = worksheet.Cells[1, column].Text.Trim();
            for (var level = 1; level <= MaximumDepth; level++)
            {
                if (string.Equals(header, $"第{level}级", StringComparison.OrdinalIgnoreCase))
                    columns[level - 1] = column;
            }
        }

        var missingLevel = Array.FindIndex(columns, column => column == 0);
        if (missingLevel >= 0)
            throw new InvalidOperationException($"模板缺少“第{missingLevel + 1}级”列，请重新下载标准模板。");
        return columns;
    }

    private static ClassStructurePreview CreatePreview(ParsedHierarchy hierarchy)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < hierarchy.Root.Children.Count; index++)
            AppendTreeText(builder, hierarchy.Root.Children[index], string.Empty,
                index == hierarchy.Root.Children.Count - 1);

        return new ClassStructurePreview
        {
            PathCount = hierarchy.PathCount,
            NodeCount = CountNodes(hierarchy.Root),
            MaxDepth = hierarchy.MaxDepth,
            DuplicatePathCount = hierarchy.DuplicatePathCount,
            TreeText = builder.ToString().TrimEnd()
        };
    }

    private static string BuildClassStructureXml(Innovator innovator, ClassNode rootNode)
    {
        var root = new XElement("class", new XAttribute("id", innovator.getNewID()));
        foreach (var child in rootNode.Children)
            root.Add(BuildClassElement(innovator, child));
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement BuildClassElement(Innovator innovator, ClassNode node)
    {
        var element = new XElement("class",
            new XAttribute("id", innovator.getNewID()),
            new XAttribute("name", node.Name));
        foreach (var child in node.Children)
            element.Add(BuildClassElement(innovator, child));
        return element;
    }

    private static void VerifySavedStructure(Innovator innovator, string itemTypeId, string expectedXml)
    {
        var savedItemType = innovator.getItemById("ItemType", itemTypeId);
        if (savedItemType.isError())
            throw new InvalidOperationException($"读取覆盖结果失败: {savedItemType.getErrorString()}");
        var actualXml = savedItemType.getProperty("class_structure", string.Empty);
        if (string.IsNullOrWhiteSpace(actualXml))
            throw new InvalidOperationException("Aras 返回的 class_structure 为空，覆盖结果未通过校验。");

        try
        {
            if (!XNode.DeepEquals(XElement.Parse(expectedXml), XElement.Parse(actualXml)))
                throw new InvalidOperationException("Aras 中保存的 class_structure 与本次组装结果不一致。");
        }
        catch (System.Xml.XmlException ex)
        {
            throw new InvalidOperationException($"Aras 返回的 class_structure 不是有效 XML: {ex.Message}", ex);
        }
    }

    private static void AppendTreeText(
        StringBuilder builder,
        ClassNode node,
        string prefix,
        bool isLast)
    {
        builder.Append(prefix).Append(isLast ? "└─ " : "├─ ").AppendLine(node.Name);
        var childPrefix = prefix + (isLast ? "   " : "│  ");
        for (var index = 0; index < node.Children.Count; index++)
            AppendTreeText(builder, node.Children[index], childPrefix,
                index == node.Children.Count - 1);
    }

    private static int CountNodes(ClassNode node)
        => node.Children.Sum(child => 1 + CountNodes(child));

    private static bool Matches(ClassStructureItemType itemType, string term)
        => Contains(itemType.Name, term) ||
           Contains(itemType.LabelEn, term) ||
           Contains(itemType.LabelZc, term) ||
           Contains(itemType.LabelZt, term);

    private static bool Contains(string value, string term)
        => value.Contains(term, StringComparison.CurrentCultureIgnoreCase);

    private static Item Apply(Innovator innovator, XElement aml, string context)
    {
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        if (result.isError())
            throw new InvalidOperationException($"{context}: {result.getErrorString()}");
        return result;
    }

    private Innovator GetInnovator()
        => _connectionService.TypedInnovator
           ?? throw new InvalidOperationException("尚未连接 Aras，请先在“Aras连接”页面登录。");

    private async Task TryLogOperationAsync(
        ClassStructureItemType itemType,
        ClassStructurePreview preview)
    {
        try
        {
            await _operationLogService.LogAsync(
                "Update",
                "ItemType",
                itemType.Id,
                $"类结构汇入全量覆盖 {itemType.Name}.class_structure：{preview.PathCount} 条路径，{preview.NodeCount} 个节点，最深 {preview.MaxDepth} 级");
        }
        catch (Exception ex)
        {
            await LogErrorBestEffortAsync("类结构汇入-操作日志", ex, ErrorLog.LevelP1);
        }
    }

    private async Task LogErrorBestEffortAsync(string operation, Exception ex, string level)
    {
        try
        {
            await _errorLogService.LogErrorAsync(operation, ex.Message, level, ex.StackTrace);
        }
        catch
        {
            // 错误日志写入失败不能覆盖原始异常。
        }
    }

    private sealed class ClassNode
    {
        private readonly Dictionary<string, ClassNode> _childrenByName =
            new(StringComparer.OrdinalIgnoreCase);

        public ClassNode(string name) => Name = name;

        public string Name { get; }
        public List<ClassNode> Children { get; } = [];

        public ClassNode GetOrAddChild(string name)
        {
            if (_childrenByName.TryGetValue(name, out var existing))
                return existing;
            var child = new ClassNode(name);
            _childrenByName.Add(name, child);
            Children.Add(child);
            return child;
        }
    }

    private sealed record ParsedHierarchy(
        ClassNode Root,
        int PathCount,
        int DuplicatePathCount,
        int MaxDepth);
}
