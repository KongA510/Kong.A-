using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 数据库导出配置管理服务接口 — 多记录 CRUD + 互斥启用（参照 IAiModelConfigService）
/// </summary>
public interface IDatabaseExportConfigService
{
    Task<List<DatabaseExportConfig>> GetAllAsync(string? userId = null);
    Task<DatabaseExportConfig?> GetEnabledAsync(string? userId = null);
    Task SaveAsync(DatabaseExportConfig config);
    Task EnableAsync(string id, string userId);
    Task DeleteAsync(string id);
}
