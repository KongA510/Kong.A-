using System.Data;

namespace ArasToolkit.Core.Models;

/// <summary>
/// 数据库修改模式的 Excel 范围、SQL 模板与执行选项。
/// </summary>
public sealed class DatabaseModificationRequest
{
    public string FilePath { get; set; } = string.Empty;
    public string SheetName { get; set; } = string.Empty;
    public int StartRow { get; set; } = 2;
    public int EndRow { get; set; } = -1;
    public int StartColumn { get; set; } = 1;
    public int EndColumn { get; set; } = -1;
    public string SqlTemplate { get; set; } = string.Empty;
    public int MaxConcurrency { get; set; } = 4;
    public string ExecutionMode { get; set; } = DatabaseModificationExecutionModes.Orm;
}

public static class DatabaseModificationExecutionModes
{
    public const string Aras = "Aras方式";
    public const string Orm = "ORM模式";
}

/// <summary>
/// Excel 数据与预组装 SQL 的双预览结果。
/// </summary>
public sealed class DatabaseModificationPreview
{
    public DataTable ExcelData { get; set; } = new();
    public List<ColumnMapping> ColumnMappings { get; set; } = [];
    public List<string> SqlStatements { get; set; } = [];
    public int TotalDataRows { get; set; }
    public int ResolvedEndRow { get; set; }
    public int ResolvedEndColumn { get; set; }
}

/// <summary>
/// 数据库修改执行进度。
/// </summary>
public sealed class DatabaseModificationProgress
{
    public int ProcessedRows { get; set; }
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 数据库修改批处理结果。
/// </summary>
public sealed class DatabaseModificationResult
{
    public int TotalRows { get; set; }
    public int ProcessedRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int SkippedCount { get; set; }
    public bool IsCancelled { get; set; }
    public string LogFilePath { get; set; } = string.Empty;
    public List<string> FailedDetails { get; set; } = [];
}
