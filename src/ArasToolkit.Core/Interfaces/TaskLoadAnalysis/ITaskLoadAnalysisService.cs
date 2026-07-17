using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 任务负载分析服务接口
/// </summary>
public interface ITaskLoadAnalysisService
{
    /// <summary>
    /// 执行流式 AI 分析，逐块回调输出
    /// </summary>
    Task AnalyzeStreamAsync(
        DateTime startDate,
        DateTime endDate,
        Action<string> onChunk,
        System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取历史分析记录（分页）
    /// </summary>
    Task<(List<TaskLoadAnalysisRecord> Items, int TotalCount)> GetPagedRecordsAsync(
        int page, int pageSize);

    /// <summary>
    /// 删除分析记录
    /// </summary>
    Task DeleteRecordAsync(string id);

    /// <summary>
    /// 获取单条分析记录
    /// </summary>
    Task<TaskLoadAnalysisRecord?> GetRecordByIdAsync(string id);
}
