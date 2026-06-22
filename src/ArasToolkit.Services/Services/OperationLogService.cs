using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Entities;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 操作日志服务实现 — 记录所有业务操作的审计轨迹
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IArasConnectionService _connectionService;
    private readonly IErrorLogService _errorLogService;

    public OperationLogService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IArasConnectionService connectionService,
        IErrorLogService errorLogService)
    {
        _contextFactory = contextFactory;
        _connectionService = connectionService;
        _errorLogService = errorLogService;
    }

    /// <summary>
    /// 获取当前操作用户名 — 优先用 Aras 登录用户名，回退到 Windows 用户名
    /// </summary>
    public string GetCurrentUserName()
    {
        return _connectionService.CurrentConnection?.Username
            ?? Environment.UserName
            ?? "Unknown";
    }

    /// <summary>
    /// 记录一条操作日志
    /// </summary>
    public async Task LogAsync(string operationType, string entityType, string entityId,
        string? description = null, string? userName = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.OperationLogs.Add(new OperationLog
            {
                OperationType = operationType,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                OperateTime = DateTime.Now,
                UserName = userName ?? GetCurrentUserName()
            });
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // 日志记录失败不应阻止主业务流程，但写入错误日志
            await _errorLogService.LogErrorAsync("操作日志写入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    /// <summary>
    /// 分页查询操作日志
    /// </summary>
    public async Task<(List<OperationLog> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? operationTypeFilter = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.OperationLogs.AsQueryable();

        if (!string.IsNullOrEmpty(operationTypeFilter))
            query = query.Where(l => l.OperationType == operationTypeFilter);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CreatorOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
