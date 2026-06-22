using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 错误日志服务接口
/// </summary>
public interface IErrorLogService
{
    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="functionName">功能模块名称</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="level">错误级别（默认 P1-普通），P0 用于致命错误</param>
    /// <param name="stackTrace">堆栈跟踪（可选）</param>
    Task LogErrorAsync(string functionName, string errorMessage,
        string? level = null, string? stackTrace = null);

    /// <summary>
    /// 分页获取错误日志
    /// </summary>
    Task<(List<ErrorLog> Items, int TotalCount)> GetPagedEntriesAsync(
        int page, int pageSize, string? levelFilter = null, DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// 获取所有错误日志
    /// </summary>
    Task<List<ErrorLog>> GetAllEntriesAsync();

    /// <summary>
    /// 清空所有错误日志
    /// </summary>
    Task ClearAllAsync();
}
