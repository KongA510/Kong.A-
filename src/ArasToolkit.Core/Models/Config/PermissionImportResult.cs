namespace ArasToolkit.Core.Models;

/// <summary>
/// 权限配置导入结果 POCO 模型（双 Sheet 统计）
/// </summary>
public class PermissionImportResult
{
    /// <summary>Sheet1 权限配置成功导入数量</summary>
    public int Sheet1Count { get; set; }

    /// <summary>Sheet1 权限配置总行数</summary>
    public int Sheet1Total { get; set; }

    /// <summary>Sheet1 失败数量（计算值）</summary>
    public int Sheet1Failed => Sheet1Total - Sheet1Count;

    /// <summary>Sheet2 详细权限成功导入数量</summary>
    public int Sheet2Count { get; set; }

    /// <summary>Sheet2 详细权限总行数</summary>
    public int Sheet2Total { get; set; }

    /// <summary>Sheet2 失败数量（计算值）</summary>
    public int Sheet2Failed => Sheet2Total - Sheet2Count;

    /// <summary>失败详细列表</summary>
    public List<string> FailedDetails { get; set; } = new();

    /// <summary>日志文件绝对路径</summary>
    public string LogFilePath { get; set; } = string.Empty;

    /// <summary>总成功数</summary>
    public int TotalCount => Sheet1Count + Sheet2Count;

    /// <summary>是否成功</summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>错误信息（失败时填充）</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>是否有失败项</summary>
    public bool HasFailures => FailedDetails.Count > 0;
}
