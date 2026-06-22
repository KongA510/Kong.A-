using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 错误日志服务实现 — EF Core 数据库持久化
/// </summary>
public class ErrorLogService : IErrorLogService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public ErrorLogService(IDbContextFactory<ArasToolkitDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task LogErrorAsync(string functionName, string errorMessage,
        string? level = null, string? stackTrace = null)
    {
        try
        {
            await _lock.WaitAsync();
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                context.ErrorLogs.Add(new ErrorLog
                {
                    FunctionName = functionName,
                    ErrorMessage = errorMessage,
                    StackTrace = stackTrace,
                    Level = level ?? ErrorLog.LevelP1,
                    RecordDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
            finally { _lock.Release(); }
        }
        catch
        {
            // 错误日志写入失败不能阻止主流程，静默处理
            System.Diagnostics.Debug.WriteLine($"[ErrorLog] 写入失败: {functionName} - {errorMessage}");
        }
    }

    public async Task<(List<ErrorLog> Items, int TotalCount)> GetPagedEntriesAsync(
        int page, int pageSize, string? levelFilter = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.ErrorLogs.AsQueryable();

            if (!string.IsNullOrEmpty(levelFilter) && levelFilter != "全部")
                query = query.Where(e => e.Level == levelFilter);

            if (fromDate.HasValue)
                query = query.Where(e => e.RecordDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.RecordDate <= toDate.Value.AddDays(1));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatorOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception)
        {
            return ([], 0);
        }
    }

    public async Task<List<ErrorLog>> GetAllEntriesAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.ErrorLogs
                .OrderByDescending(e => e.CreatorOn)
                .ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public async Task ClearAllAsync()
    {
        try
        {
            await _lock.WaitAsync();
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var all = await context.ErrorLogs.ToListAsync();
                context.ErrorLogs.RemoveRange(all);
                await context.SaveChangesAsync();
            }
            finally { _lock.Release(); }
        }
        catch
        {
            // 静默处理
        }
    }
}
