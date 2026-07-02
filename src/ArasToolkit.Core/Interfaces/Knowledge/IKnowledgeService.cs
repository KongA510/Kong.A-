using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 个人资料库服务接口 — 管理知识笔记的增删改查
/// </summary>
public interface IKnowledgeService
{
    /// <summary>
    /// 分页获取笔记列表（按置顶+创建时间倒序）
    /// </summary>
    Task<(List<KnowledgeEntry> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchKeyword = null);

    /// <summary>
    /// 按 ID 获取笔记详情（含 Content）
    /// </summary>
    Task<KnowledgeEntry?> GetByIdAsync(string id);

    /// <summary>
    /// 新增笔记
    /// </summary>
    Task<KnowledgeEntry> AddAsync(KnowledgeEntry entry);

    /// <summary>
    /// 更新笔记
    /// </summary>
    Task<KnowledgeEntry> UpdateAsync(KnowledgeEntry entry);

    /// <summary>
    /// 删除笔记
    /// </summary>
    Task DeleteAsync(string id);

    /// <summary>
    /// 同步数据库表结构
    /// </summary>
    Task SyncSchemaAsync();
}
