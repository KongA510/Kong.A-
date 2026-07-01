using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Aras 登录配置服务 — 数据库持久化（替代旧版 JSON 文件存储）
/// </summary>
public class ArasLoginConfigService : IArasLoginConfigService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IConfigService _configService;
    private readonly IOperationLogService? _operationLogService;

    public ArasLoginConfigService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IConfigService configService,
        IOperationLogService? operationLogService = null)
    {
        _dbFactory = dbFactory;
        _configService = configService;
        _operationLogService = operationLogService;
    }

    public async Task<List<ArasLoginConfig>> GetAllAsync(string? userId)
    {
        // 首次访问时自动触发 JSON → DB 迁移
        if (!string.IsNullOrEmpty(userId))
            await MigrateFromJsonIfNeededAsync(userId);

        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.ArasLoginConfigs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(c => c.UserId == userId);
        return await query.OrderByDescending(c => c.CreatorOn).ToListAsync();
    }

    public async Task<ArasLoginConfig?> GetByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ArasLoginConfigs.FindAsync(id);
    }

    public async Task<ArasLoginConfig> SaveAsync(ArasLoginConfig config)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.ArasLoginConfigs.FindAsync(config.Id);
        if (existing != null)
        {
            // 更新已有记录
            existing.Url = config.Url;
            existing.DatabaseName = config.DatabaseName;
            existing.Username = config.Username;
            if (!string.IsNullOrEmpty(config.Md5Password))
                existing.Md5Password = config.Md5Password;
            existing.IsEnabled = config.IsEnabled;
        }
        else
        {
            db.ArasLoginConfigs.Add(config);
        }
        await db.SaveChangesAsync();

        // 记录操作日志
        if (_operationLogService != null)
        {
            try
            {
                var action = existing != null ? "Update" : "Create";
                await _operationLogService.LogAsync(action, "ArasLoginConfig", config.Id,
                    $"{(existing != null ? "更新" : "新增")}Aras登录配置: {config.Username}@{config.DatabaseName}");
            }
            catch { /* 日志写入失败不影响主流程 */ }
        }

        return config;
    }

    public async Task DeleteAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var config = await db.ArasLoginConfigs.FindAsync(id);
        if (config != null)
        {
            db.ArasLoginConfigs.Remove(config);
            await db.SaveChangesAsync();

            if (_operationLogService != null)
            {
                try
                {
                    await _operationLogService.LogAsync("Delete", "ArasLoginConfig", id,
                        $"删除Aras登录配置: {config.Username}@{config.DatabaseName}");
                }
                catch { }
            }
        }
    }

    public async Task<ArasLoginConfig?> GetEnabledAsync(string? userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ArasLoginConfigs
            .Where(c => c.IsEnabled)
            .OrderByDescending(c => c.CreatorOn)
            .FirstOrDefaultAsync();
    }

    public async Task EnableAsync(string id, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        // 禁用该用户所有配置，仅启用目标
        var all = await db.ArasLoginConfigs.Where(c => c.UserId == userId).ToListAsync();
        foreach (var c in all)
            c.IsEnabled = (c.Id == id);
        await db.SaveChangesAsync();
    }

    public async Task<bool> MigrateFromJsonIfNeededAsync(string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        // 如果 DB 中已有该用户的记录，跳过迁移
        var hasRecords = await db.ArasLoginConfigs.AnyAsync(c => c.UserId == userId);
        if (hasRecords) return false;

        // 从 JSON 文件加载旧配置
        var jsonLogins = await _configService.LoadAllLoginInfosAsync();
        if (jsonLogins.Count == 0) return false;

        foreach (var login in jsonLogins)
        {
            var config = new ArasLoginConfig
            {
                Id = Guid.NewGuid().ToString("N")[..12],
                Url = login.Url,
                DatabaseName = login.Database,
                Username = login.Username,
                Md5Password = login.Password.ToMd5(),  // JSON中密码为明文，转MD5
                IsEnabled = false,
                CreatorOn = DateTime.Now,
                UserId = userId
            };
            db.ArasLoginConfigs.Add(config);
        }

        await db.SaveChangesAsync();

        // 迁移完成后删除旧的 JSON 文件
        foreach (var login in jsonLogins)
        {
            try { await _configService.DeleteLoginInfoAsync(login.ConfigName ?? ""); }
            catch { /* 删除失败静默处理 */ }
        }

        return true;
    }
}
