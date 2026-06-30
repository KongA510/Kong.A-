namespace ArasToolkit.Core.Models;

/// <summary>
/// List配置导入结果 POCO 模型 — 汇总3个Sheet的导入情况
/// </summary>
public class ListImportResult
{
    /// <summary>Sheet1（List主档）成功导入条数</summary>
    public int Sheet1Count { get; set; }

    /// <summary>Sheet2（菜单内容）成功导入条数</summary>
    public int Sheet2Count { get; set; }

    /// <summary>Sheet3（菜单内容过滤）成功导入条数</summary>
    public int Sheet3Count { get; set; }

    /// <summary>Sheet1 总行数</summary>
    public int Sheet1Total { get; set; }

    /// <summary>Sheet2 总行数</summary>
    public int Sheet2Total { get; set; }

    /// <summary>Sheet3 总行数</summary>
    public int Sheet3Total { get; set; }

    /// <summary>Sheet1 失败行数（计算值）</summary>
    public int Sheet1Failed => Sheet1Total - Sheet1Count;

    /// <summary>Sheet2 失败行数（计算值）</summary>
    public int Sheet2Failed => Sheet2Total - Sheet2Count;

    /// <summary>Sheet3 失败行数（计算值）</summary>
    public int Sheet3Failed => Sheet3Total - Sheet3Count;

    /// <summary>失败明细列表</summary>
    public List<string> FailedDetails { get; set; } = new();

    /// <summary>日志文件完整路径</summary>
    public string LogFilePath { get; set; } = string.Empty;

    /// <summary>总成功数</summary>
    public int TotalCount => Sheet1Count + Sheet2Count + Sheet3Count;

    /// <summary>是否全部成功</summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>错误信息（异常时填充）</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>是否有失败行</summary>
    public bool HasFailures => FailedDetails.Count > 0;
}
