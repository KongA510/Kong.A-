namespace ArasToolkit.Core.Models;

/// <summary>数据库表结构检查状态。</summary>
public enum DatabaseSchemaTableStatus
{
    Ready,
    Created,
    Updated,
    Failed
}

/// <summary>单张 EF 受管表的检查结果。</summary>
public sealed class DatabaseSchemaTableResult
{
    public string TableName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public DatabaseSchemaTableStatus Status { get; init; }
    public string Details { get; init; } = string.Empty;

    public string StatusText => Status switch
    {
        DatabaseSchemaTableStatus.Ready => "完整",
        DatabaseSchemaTableStatus.Created => "已新建",
        DatabaseSchemaTableStatus.Updated => "已补全",
        DatabaseSchemaTableStatus.Failed => "失败",
        _ => Status.ToString()
    };
}

/// <summary>一次数据库检查和同步的汇总。</summary>
public sealed class DatabaseSchemaCheckResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<DatabaseSchemaTableResult> Tables { get; init; } = [];

    public int ReadyCount => Tables.Count(item => item.Status == DatabaseSchemaTableStatus.Ready);
    public int ChangedCount => Tables.Count(item => item.Status is
        DatabaseSchemaTableStatus.Created or DatabaseSchemaTableStatus.Updated);
    public int FailedCount => Tables.Count(item => item.Status == DatabaseSchemaTableStatus.Failed);
}
