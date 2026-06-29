namespace ArasToolkit.Core.Models;

/// <summary>
/// 对象类配置汇入结果 POCO 模型
/// </summary>
public class ObjectClassImportResult
{
    /// <summary>Sheet1 对象类新增条数</summary>
    public int Sheet1Count { get; set; }

    /// <summary>Sheet2 关系类新增条数</summary>
    public int Sheet2Count { get; set; }

    /// <summary>日志文件完整路径</summary>
    public string LogFilePath { get; set; } = string.Empty;

    /// <summary>汇总信息</summary>
    public int TotalCount => Sheet1Count + Sheet2Count;

    /// <summary>是否成功</summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>错误信息（失败时填充）</summary>
    public string? ErrorMessage { get; set; }
}
