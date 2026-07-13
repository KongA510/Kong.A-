using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

public class SqlTemplateService : ISqlTemplateService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public SqlTemplateService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<SqlTemplate>> GetAllAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.SqlTemplates.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(t => t.UserId == userId);
        return await query
            .OrderByDescending(x => x.CreatorOn)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<SqlTemplate?> GetByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.SqlTemplates.FindAsync(id).ConfigureAwait(false);
    }

    public async Task SaveAsync(SqlTemplate template)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);

            // 判断是否为新建：Id 为空 或 数据库中没有此 Id 的记录
            var isNew = string.IsNullOrWhiteSpace(template.Id);
            if (!isNew)
            {
                var existing = await db.SqlTemplates.FindAsync(template.Id).ConfigureAwait(false);
                isNew = existing == null;
            }

            if (isNew)
            {
                if (string.IsNullOrWhiteSpace(template.Id))
                    template.Id = Guid.NewGuid().ToString("N")[..12];
                template.CreatorOn = DateTime.Now;
                template.UserId = Core.Models.CurrentUserContext.CurrentUserId ?? "system";
                db.SqlTemplates.Add(template);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await _operationLogService.LogAsync("Create", "SqlTemplate", template.Id,
                    $"创建SQL模板: {template.TemplateName}").ConfigureAwait(false);
            }
            else
            {
                var existing = await db.SqlTemplates.FindAsync(template.Id).ConfigureAwait(false);
                if (existing != null)
                {
                    existing.TemplateName = template.TemplateName;
                    existing.SqlContent = template.SqlContent;
                    existing.Description = template.Description;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                    await _operationLogService.LogAsync("Update", "SqlTemplate", template.Id,
                        $"更新SQL模板: {template.TemplateName}").ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("SqlTemplate-保存", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var template = await db.SqlTemplates.FindAsync(id).ConfigureAwait(false);
            if (template != null)
            {
                var name = template.TemplateName;
                db.SqlTemplates.Remove(template);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await _operationLogService.LogAsync("Delete", "SqlTemplate", id,
                    $"删除SQL模板: {name}").ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("SqlTemplate-删除", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }
}
