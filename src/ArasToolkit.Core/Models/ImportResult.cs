namespace ArasToolkit.Core.Models;

/// <summary>
/// 数据导入结果模型
/// </summary>
public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
   public int FailureCount { get; set; }
   public int SkippedCount { get; set; }
   /// <summary>实际已处理行数 — 暂停/取消时小于 TotalRows</summary>
   public int ProcessedRows { get; set; }
   public DateTime ImportTime { get; set; }
    public string LogFilePath { get; set; } = string.Empty;
}

/// <summary>
/// 列映射模型 — Excel列字母到真实列头的映射
/// </summary>
public class ColumnMapping
{
    public string Letter { get; set; } = string.Empty;   // "A", "B", "AA"
    public string Header { get; set; } = string.Empty;   // 真实列头名
    public int Index { get; set; }                       // 0-based
}
