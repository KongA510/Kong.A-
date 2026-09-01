using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>按当前应用用户隔离的相关代码主题与代码段服务。</summary>
public interface IRelatedCodeRecordService
{
    Task<List<RelatedCodeRecord>> GetAllAsync(string? keyword = null);
    Task<RelatedCodeRecord?> GetByIdAsync(string id);
    Task<RelatedCodeRecord> SaveRecordAsync(RelatedCodeRecord record);
    Task<RelatedCodeSegment> SaveSegmentAsync(string recordId, RelatedCodeSegment segment);
    Task DeleteRecordAsync(string id);
    Task DeleteSegmentAsync(string recordId, string segmentId);
    Task ReorderSegmentsAsync(string recordId, IReadOnlyList<string> orderedSegmentIds);
}
