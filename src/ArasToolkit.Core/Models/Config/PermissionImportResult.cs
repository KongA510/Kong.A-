namespace ArasToolkit.Core.Models;

/// <summary>
/// 权限配置导入结果 POCO 模型
/// </summary>
public class PermissionImportResult
{
    /// <summary>权限配置成功导入条数</summary>
    public int Sheet1Count { get; set; }

    /// <summary>权限配置总行数</summary>
    public int Sheet1Total { get; set; }

    /// <summary>失败行数（计算值）</summary>
    public int Sheet1Failed => Sheet1Total - Sheet1Count;

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
