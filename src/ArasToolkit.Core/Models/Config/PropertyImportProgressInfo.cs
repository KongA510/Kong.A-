namespace ArasToolkit.Core.Models;

/// <summary>属性汇入的逐行状态和对象类保存阶段。</summary>
public sealed class PropertyImportProgressInfo : ImportProgressInfo
{
    public int ExcelRowNumber { get; set; }
    public string SubmitStatus { get; set; } = string.Empty;
    public bool IsFinalizing { get; set; }
}
