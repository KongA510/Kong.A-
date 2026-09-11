using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>管理员可查询全部代码主题与代码段，其他用户查询及所有写操作仍按创建者隔离。</summary>
public interface IRelatedCodeRecordService
{
    Task<List<RelatedCodeRecord>> GetAllAsync(string? keyword = null);
    Task<RelatedCodeRecord?> GetByIdAsync(string id);
    Task<RelatedCodeRecord> SaveRecordAsync(RelatedCodeRecord record);
    Task<RelatedCodeSegment> SaveSegmentAsync(string recordId, RelatedCodeSegment segment);
    Task DeleteRecordAsync(string id);
    Task DeleteSegmentAsync(string recordId, string segmentId);
    Task ReorderRecordsAsync(IReadOnlyList<string> orderedRecordIds);
    Task ReorderSegmentsAsync(string recordId, IReadOnlyList<string> orderedSegmentIds);
}
