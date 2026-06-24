using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 閿欒鏃ュ織鏈嶅姟瀹炵幇 鈥?EF Core 鏁版嵁搴撴寔涔呭寲
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
                    UserName = CurrentUserContext.CurrentUserName,
                   Level = level ?? ErrorLog.LevelP1,
                   RecordDate = DateTime.Now
               });
                await context.SaveChangesAsync();
            }
            finally { _lock.Release(); }
        }
        catch
        {
            // 閿欒鏃ュ織鍐欏叆澶辫触涓嶈兘闃绘涓绘祦绋嬶紝闈欓粯澶勭悊
            System.Diagnostics.Debug.WriteLine($"[ErrorLog] 鍐欏叆澶辫触: {functionName} - {errorMessage}");
        }
    }

    public async Task<(List<ErrorLog> Items, int TotalCount)> GetPagedEntriesAsync(
        int page, int pageSize, string? levelFilter = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.ErrorLogs.AsQueryable();

            if (!string.IsNullOrEmpty(levelFilter) && levelFilter != "鍏ㄩ儴")
                query = query.Where(e => e.Level == levelFilter);

            if (fromDate.HasValue)
                query = query.Where(e => e.RecordDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.RecordDate <= toDate.Value.AddDays(1));

       var total = await query.CountAsync();
 
             // 闈炵鐞嗗憳浠呮煡鐪嬭嚜宸辩殑鏃ュ織
             if (!CurrentUserContext.IsAdmin)
             {
                 query = query.Where(e => e.UserName == CurrentUserContext.CurrentUserName);
             }
 
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
            var query = context.ErrorLogs.AsQueryable();

            if (!CurrentUserContext.IsAdmin)
            {
                query = query.Where(e => e.UserName == CurrentUserContext.CurrentUserName);
            }

            return await query
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
            // 闈欓粯澶勭悊
        }
    }
}
