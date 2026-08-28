namespace ArasToolkit.Core.Models;

/// <summary>可作为类结构覆盖目标的 Aras ItemType。</summary>
public sealed class ClassStructureItemType
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LabelEn { get; set; } = string.Empty;
    public string LabelZc { get; set; } = string.Empty;
    public string LabelZt { get; set; } = string.Empty;

    public string DisplayName => string.IsNullOrWhiteSpace(LabelZc)
        ? string.IsNullOrWhiteSpace(LabelEn) ? Name : $"{LabelEn} ({Name})"
        : $"{LabelZc} ({Name})";
}

/// <summary>Excel 中类结构路径的解析预览。</summary>
public sealed class ClassStructurePreview
{
    public int PathCount { get; set; }
    public int NodeCount { get; set; }
    public int MaxDepth { get; set; }
    public int DuplicatePathCount { get; set; }
    public string TreeText { get; set; } = string.Empty;

    public string Summary =>
        $"{PathCount} 条有效路径 · {NodeCount} 个唯一节点 · 最深 {MaxDepth} 级" +
        (DuplicatePathCount > 0 ? $" · 忽略 {DuplicatePathCount} 条重复路径" : string.Empty);
}

/// <summary>类结构全量覆盖结果。</summary>
public sealed class ClassStructureImportResult
{
    public string ItemTypeId { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
    public int PathCount { get; set; }
    public int NodeCount { get; set; }
    public int MaxDepth { get; set; }
    public string ClassStructureXml { get; set; } = string.Empty;
}
