using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>管理员可查询全部常用 SQL/AML/XML 片段；写操作仍限创建者。</summary>
public sealed class CommonQuerySnippetService : ICommonQuerySnippetService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public CommonQuerySnippetService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<CommonQuerySnippet>> GetAllAsync(string? contentType = null, string? keyword = null)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var isAdmin = CurrentUserContext.IsAdmin;
            var query = db.CommonQuerySnippets
                .AsNoTracking()
                .Where(item => isAdmin || item.UserId == userId);

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                var normalizedType = CommonQuerySnippetTypes.Normalize(contentType);
                query = query.Where(item => item.ContentType == normalizedType);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim();
                query = query.Where(item =>
                    item.Title.Contains(normalizedKeyword) ||
                    (item.Description != null && item.Description.Contains(normalizedKeyword)) ||
                    item.Content.Contains(normalizedKeyword));
            }

            return await query
                .OrderByDescending(item => item.CreatorOn)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("常用SQL/AML-加载", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task SaveAsync(CommonQuerySnippet snippet)
    {
        try
        {
            Validate(snippet);
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var existing = string.IsNullOrWhiteSpace(snippet.Id)
                ? null
                : await db.CommonQuerySnippets
                    .FirstOrDefaultAsync(item => item.Id == snippet.Id)
                    .ConfigureAwait(false);

            // 共享记录仅供查看复用，不能把无权编辑的记录误判为新建并生成副本。
            if (existing != null && existing.UserId != userId)
                throw new InvalidOperationException("仅创建者可修改此片段，当前账号可查看和复制复用。");

            if (existing == null)
            {
                snippet.Id = Guid.NewGuid().ToString("N")[..12];
                snippet.Title = snippet.Title.Trim();
                snippet.ContentType = CommonQuerySnippetTypes.Normalize(snippet.ContentType);
                snippet.Description = NormalizeOptional(snippet.Description);
                snippet.UserId = userId;
                snippet.CreatorOn = DateTime.Now;
                db.CommonQuerySnippets.Add(snippet);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await LogOperationSafelyAsync("Create", snippet.Id,
                    $"创建{snippet.ContentType}片段: {snippet.Title}").ConfigureAwait(false);
                return;
            }

            existing.Title = snippet.Title.Trim();
            existing.ContentType = CommonQuerySnippetTypes.Normalize(snippet.ContentType);
            existing.Content = snippet.Content;
            existing.Description = NormalizeOptional(snippet.Description);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Update", existing.Id,
                $"更新{existing.ContentType}片段: {existing.Title}").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("常用SQL/AML-保存", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var existing = await db.CommonQuerySnippets
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId)
                .ConfigureAwait(false);
            if (existing == null)
                return;

            db.CommonQuerySnippets.Remove(existing);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Delete", existing.Id,
                $"删除{existing.ContentType}片段: {existing.Title}").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("常用SQL/AML-删除", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private static void Validate(CommonQuerySnippet snippet)
    {
        if (string.IsNullOrWhiteSpace(snippet.Title))
            throw new InvalidOperationException("请输入片段标题。");
        if (snippet.Title.Trim().Length > 200)
            throw new InvalidOperationException("片段标题不能超过 200 个字符。");
        if (string.IsNullOrWhiteSpace(snippet.Content))
            throw new InvalidOperationException("请输入 SQL、AML 或 XML 内容。");
        if (snippet.Description?.Trim().Length > 500)
            throw new InvalidOperationException("说明不能超过 500 个字符。");
    }

    private async Task LogOperationSafelyAsync(string operationType, string id, string description)
    {
        try
        {
            await _operationLogService.LogAsync(operationType, nameof(CommonQuerySnippet), id, description)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[常用SQL/AML] 操作日志写入失败: {ex.Message}");
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
