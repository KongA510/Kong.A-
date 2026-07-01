using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Aras 登录配置服务接口 — 数据库持久化管理
/// </summary>
public interface IArasLoginConfigService
{
    /// <summary>获取当前用户的所有登录配置</summary>
    Task<List<ArasLoginConfig>> GetAllAsync(string? userId);

    /// <summary>按ID获取单条配置</summary>
    Task<ArasLoginConfig?> GetByIdAsync(string id);

    /// <summary>保存配置（新增或更新）</summary>
    Task<ArasLoginConfig> SaveAsync(ArasLoginConfig config);

    /// <summary>删除指定配置</summary>
    Task DeleteAsync(string id);

    /// <summary>获取当前启用的配置</summary>
    Task<ArasLoginConfig?> GetEnabledAsync(string? userId);

    /// <summary>启用指定配置（禁用该用户所有其他配置）</summary>
    Task EnableAsync(string id, string userId);

    /// <summary>一次性从JSON迁移到数据库（无DB记录时自动执行）</summary>
    Task<bool> MigrateFromJsonIfNeededAsync(string userId);
}
