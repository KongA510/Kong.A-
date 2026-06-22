using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 操作日志服务接口 — 记录所有业务操作的审计轨迹
/// </summary>
public interface IOperationLogService
{
    /// <summary>
    /// 记录一条操作日志
    /// </summary>
    /// <param name="operationType">操作类型：Create / Update / Delete / Import</param>
    /// <param name="entityType">实体类型，如 TodoItem</param>
    /// <param name="entityId">被操作记录的主键值</param>
    /// <param name="description">操作描述（可选）</param>
    /// <param name="userName">操作用户名（可选，默认取当前 Aras 登录用户）</param>
    Task LogAsync(string operationType, string entityType, string entityId,
        string? description = null, string? userName = null);

    /// <summary>
    /// 分页查询操作日志
    /// </summary>
    Task<(List<OperationLog> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? operationTypeFilter = null);

    /// <summary>
    /// 获取当前操作用户名（Aras 登录用户名，若未登录则用 Windows 用户名）
    /// </summary>
    string GetCurrentUserName();
}
