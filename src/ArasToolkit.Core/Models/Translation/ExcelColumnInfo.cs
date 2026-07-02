namespace ArasToolkit.Core.Models.Translation;

/// <summary>
/// Excel 列信息 — 用于「源文本自定义翻译」模式的列选择下拉
/// </summary>
public class ExcelColumnInfo
{
    /// <summary>1-based 列序号</summary>
    public int Index { get; set; }

    /// <summary>显示文本，形如 "A - 属性名称" 或 "A"（无表头时）</summary>
    public string Label { get; set; } = "";
}
