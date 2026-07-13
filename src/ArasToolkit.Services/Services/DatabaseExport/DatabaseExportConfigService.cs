using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 数据库导出配置管理服务 — 多记录 CRUD + 互斥启用（参照 AiModelConfigService）
/// </summary>
public class DatabaseExportConfigService : IDatabaseExportConfigService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;

    public DatabaseExportConfigService(IDbContextFactory<ArasToolkitDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<DatabaseExportConfig>> GetAllAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.DatabaseExportConfigs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(c => c.UserId == userId);
        return await query.OrderByDescending(c => c.CreatorOn).ToListAsync();
    }

    public async Task<DatabaseExportConfig?> GetEnabledAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.DatabaseExportConfigs.Where(c => c.IsEnabled);
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(c => c.UserId == userId);
        return await query.OrderByDescending(c => c.CreatorOn).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(DatabaseExportConfig config)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.DatabaseExportConfigs.FindAsync(config.Id);
        if (existing != null)
        {
            existing.ConfigName = config.ConfigName;
            existing.ConnectionString = config.ConnectionString;
            existing.Remark = config.Remark;
            existing.IsEnabled = config.IsEnabled;
        }
        else
        {
            db.DatabaseExportConfigs.Add(config);
        }
        await db.SaveChangesAsync();
    }

    public async Task EnableAsync(string id, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var all = await db.DatabaseExportConfigs.Where(c => c.UserId == userId).ToListAsync();
        foreach (var c in all)
            c.IsEnabled = (c.Id == id);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var config = await db.DatabaseExportConfigs.FindAsync(id);
        if (config != null)
        {
            db.DatabaseExportConfigs.Remove(config);
            await db.SaveChangesAsync();
        }
    }
}
