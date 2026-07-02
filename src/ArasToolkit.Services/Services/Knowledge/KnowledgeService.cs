using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 个人资料库服务实现 — EF Core 数据库存储、操作日志
/// </summary>
public class KnowledgeService : IKnowledgeService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public KnowledgeService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _contextFactory = contextFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<(List<KnowledgeEntry> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchKeyword = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.KnowledgeEntries.AsQueryable();
            query = query.Where(e => e.UserId == CurrentUserContext.CurrentUserId);

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(e =>
                    e.Title.Contains(searchKeyword) ||
                    (e.Tags != null && e.Tags.Contains(searchKeyword)) ||
                    (e.PlainTextPreview != null && e.PlainTextPreview.Contains(searchKeyword)));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.Pinned)
                .ThenByDescending(e => e.CreatorOn)
                .Select(e => new KnowledgeEntry
                {
                    Id = e.Id,
                    Title = e.Title,
                    PlainTextPreview = e.PlainTextPreview,
                    Tags = e.Tags,
                    Category = e.Category,
                    Pinned = e.Pinned,
                    CreatedDate = e.CreatedDate,
                    ModifiedDate = e.ModifiedDate,
                    CreatorOn = e.CreatorOn,
                    UserId = e.UserId
                    // Content 不在列表查询中返回（减少内存占用）
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-查询列表", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<KnowledgeEntry?> GetByIdAsync(string id)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.KnowledgeEntries.FindAsync(id);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-获取详情", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return null;
        }
    }

    public async Task<KnowledgeEntry> AddAsync(KnowledgeEntry entry)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            entry.Id = string.IsNullOrEmpty(entry.Id)
                ? Guid.NewGuid().ToString("N")[..12]
                : entry.Id;
            entry.CreatorOn = DateTime.Now;
            entry.CreatedDate = DateTime.Now;
            entry.UserId = CurrentUserContext.CurrentUserId;

            // 提取纯文本预览
            entry.PlainTextPreview = ExtractPlainTextPreview(entry.Content);

            context.KnowledgeEntries.Add(entry);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Create", "KnowledgeEntry", entry.Id,
                $"创建笔记: {entry.Title}");

            return entry;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-新增笔记", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<KnowledgeEntry> UpdateAsync(KnowledgeEntry entry)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.KnowledgeEntries.FindAsync(entry.Id)
                ?? throw new InvalidOperationException($"笔记不存在: {entry.Id}");

            existing.Title = entry.Title;
            existing.Content = entry.Content;
            existing.PlainTextPreview = ExtractPlainTextPreview(entry.Content);
            existing.Tags = entry.Tags;
            existing.Category = entry.Category;
            existing.Pinned = entry.Pinned;
            existing.ModifiedDate = DateTime.Now;

            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Update", "KnowledgeEntry", entry.Id,
                $"更新笔记: {entry.Title}");

            return existing;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-更新笔记", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entry = await context.KnowledgeEntries.FindAsync(id)
                ?? throw new InvalidOperationException($"笔记不存在: {id}");

            context.KnowledgeEntries.Remove(entry);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Delete", "KnowledgeEntry", id,
                $"删除笔记: {entry.Title}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-删除笔记", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.KnowledgeEntries
                .Where(e => e.UserId == CurrentUserContext.CurrentUserId
                    && e.Category != null && e.Category != "")
                .Select(e => e.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-获取分类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<KnowledgeEntry>> GetByCategoryAsync(string? category)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.KnowledgeEntries
                .Where(e => e.UserId == CurrentUserContext.CurrentUserId);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
            }

            return await query
                .OrderByDescending(e => e.Pinned)
                .ThenByDescending(e => e.CreatorOn)
                .Select(e => new KnowledgeEntry
                {
                    Id = e.Id,
                    Title = e.Title,
                    PlainTextPreview = e.PlainTextPreview,
                    Tags = e.Tags,
                    Category = e.Category,
                    Pinned = e.Pinned,
                    CreatedDate = e.CreatedDate,
                    ModifiedDate = e.ModifiedDate,
                    CreatorOn = e.CreatorOn,
                    UserId = e.UserId
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-按分类查询", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task SyncSchemaAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.EnsureSchemaAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-同步表结构", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    /// <summary>
    /// 从 FlowDocument XAML 中提取纯文本预览（最多200字）
    /// </summary>
    private static string ExtractPlainTextPreview(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "";

        // FlowDocument XAML 中包含 <Run Text="..."> 等文本节点
        // 简单做法：剥离所有 XML 标签，取纯文本前200字
        var plain = System.Text.RegularExpressions.Regex.Replace(content, @"<[^>]+>", " ");
        plain = System.Text.RegularExpressions.Regex.Replace(plain, @"\s+", " ").Trim();

        return plain.Length <= 200 ? plain : plain[..200] + "…";
    }
}
