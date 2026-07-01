using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 更新日志服务接口
/// </summary>
public interface IChangelogService
{
    /// <summary>
    /// 获取所有更新日志
    /// </summary>
    Task<List<Changelog>> GetAllEntriesAsync();

    /// <summary>
    /// 按版本获取日志
    /// </summary>
    Task<List<Changelog>> GetEntriesByVersionAsync(string version);

    /// <summary>
    /// 添加新日志条目
    /// </summary>
    Task AddEntryAsync(Changelog entry);

    /// <summary>
    /// 获取当前系统版本
    /// </summary>
    string GetCurrentVersion();
}
