namespace ArasToolkit.Core.Models;

/// <summary>
/// 属性配置导入结果 POCO 模型
/// </summary>
public class PropertyImportResult
{
    /// <summary>属性配置成功导入条数</summary>
    public int Sheet1Count { get; set; }

    /// <summary>属性配置总行数</summary>
    public int Sheet1Total { get; set; }

    /// <summary>失败行数（计算值）</summary>
    public int Sheet1Failed => Sheet1Total - Sheet1Count;

    /// <summary>实际新增的属性数量。</summary>
    public int AddedCount { get; set; }

    /// <summary>实际覆盖的属性数量。</summary>
    public int UpdatedCount { get; set; }

    /// <summary>按 Excel 行号记录实际提交状态，未执行行不会被误报成功。</summary>
    public Dictionary<int, string> RowStatuses { get; } = new();

    public int FailedRowCount => RowStatuses.Values.Count(status => status == "提交失败");
    public int UnsubmittedCount => Math.Max(0, Sheet1Total - Sheet1Count - FailedRowCount);
    public bool IsCanceled { get; set; }

    /// <summary>属性提交后，对目标 ItemType 执行一次空 edit 的结果。</summary>
    public bool ItemTypeSaveAttempted { get; set; }
    public bool ItemTypeSaved { get; set; }
    public string? ItemTypeSaveError { get; set; }

    /// <summary>失败明细列表</summary>
    public List<string> FailedDetails { get; set; } = new();

    /// <summary>日志文件完整路径</summary>
    public string LogFilePath { get; set; } = string.Empty;

    /// <summary>汇总信息</summary>
    public int TotalCount => Sheet1Count;

    /// <summary>是否成功</summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>错误信息（失败时填充）</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>是否有失败行</summary>
    public bool HasFailures => FailedDetails.Count > 0;
}
