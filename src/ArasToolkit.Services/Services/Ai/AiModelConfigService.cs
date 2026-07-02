using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// AI 模型配置管理服务 — 独立于任何业务功能，纯 CRUD + 互斥启用。
/// 从 TextTranslationService 中独立出来，遵循单一职责原则。
/// </summary>
public class AiModelConfigService : IAiModelConfigService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;

    public AiModelConfigService(IDbContextFactory<ArasToolkitDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<AiModelConfig>> GetAllAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.AiModelConfigs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(m => m.UserId == userId);
        return await query.OrderByDescending(m => m.CreatorOn).ToListAsync();
    }

    public async Task<AiModelConfig?> GetEnabledAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AiModelConfigs
            .Where(m => m.IsEnabled)
            .OrderByDescending(m => m.CreatorOn)
            .FirstOrDefaultAsync();
    }

    public async Task SaveAsync(AiModelConfig config)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.AiModelConfigs.FindAsync(config.Id);
        if (existing != null)
        {
            existing.ModelName = config.ModelName;
            existing.ApiKey = config.ApiKey;
            existing.ApiBaseUrl = config.ApiBaseUrl;
            existing.ModelIdentifier = config.ModelIdentifier;
            existing.ExtraParams = config.ExtraParams;
            existing.IsEnabled = config.IsEnabled;
        }
        else
        {
            db.AiModelConfigs.Add(config);
        }
        await db.SaveChangesAsync();
    }

    public async Task EnableAsync(string id, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        // 禁用该用户所有模型
        var all = await db.AiModelConfigs.Where(m => m.UserId == userId).ToListAsync();
        foreach (var m in all)
            m.IsEnabled = (m.Id == id);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var model = await db.AiModelConfigs.FindAsync(id);
        if (model != null)
        {
            db.AiModelConfigs.Remove(model);
            await db.SaveChangesAsync();
        }
    }
}
