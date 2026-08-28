namespace ArasToolkit.Core.Models;

/// <summary>文本比对行的差异类型。</summary>
public enum DataDiffKind
{
    Equal,
    Removed,
    Modified,
    Added
}

/// <summary>格式化后按行对齐的单条比对结果。</summary>
public sealed class DataDiffLine
{
    public int? LeftLineNumber { get; init; }
    public int? RightLineNumber { get; init; }
    public string LeftText { get; init; } = string.Empty;
    public string RightText { get; init; } = string.Empty;
    public DataDiffKind Kind { get; init; }

    /// <summary>左侧单元格颜色语义；新增行在左侧保持无色。</summary>
    public DataDiffKind LeftKind => Kind == DataDiffKind.Added ? DataDiffKind.Equal : Kind;

    /// <summary>右侧单元格颜色语义；删除行在右侧保持无色。</summary>
    public DataDiffKind RightKind => Kind == DataDiffKind.Removed ? DataDiffKind.Equal : Kind;

    public string KindLabel => Kind switch
    {
        DataDiffKind.Removed => "B 中缺少",
        DataDiffKind.Modified => "内容差异",
        DataDiffKind.Added => "A 中缺少",
        _ => string.Empty
    };
}

/// <summary>结构化数据比对结果。</summary>
public sealed class DataComparisonResult
{
    public string LeftFormatted { get; init; } = string.Empty;
    public string RightFormatted { get; init; } = string.Empty;
    public IReadOnlyList<DataDiffLine> Lines { get; init; } = [];
    public int RemovedCount { get; init; }
    public int ModifiedCount { get; init; }
    public int AddedCount { get; init; }
    public int DifferenceCount => RemovedCount + ModifiedCount + AddedCount;
}
