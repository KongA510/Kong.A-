using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 个人任务记录服务接口 — 管理待办任务的增删改查、分页、Excel导入导出
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// 分页获取待办项（支持按状态和项目名称筛选）
    /// 排序规则：今日到期优先 → 按预计日期升序 → 无日期排最后
    /// </summary>
    Task<(List<PersonalTask> Items, int TotalCount)> GetPagedItemsAsync(
        int page, int pageSize, string? statusFilter = null, string? projectNameFilter = null, string? searchKeyword = null, DateTime? completionDate = null, DateTime? dueDate = null);

    /// <summary>
    /// 获取所有不重复的项目名称列表（供筛选下拉使用）
    /// </summary>
    Task<List<string>> GetDistinctProjectNamesAsync();

    /// <summary>
    /// 按 ID 获取单个待办项
    /// </summary>
    Task<PersonalTask?> GetItemByIdAsync(string id);

    /// <summary>
    /// 新增待办项
    /// </summary>
    Task AddItemAsync(PersonalTask item);

    /// <summary>
    /// 更新待办项
    /// </summary>
    Task UpdateItemAsync(PersonalTask item);

    /// <summary>
    /// 删除待办项
    /// </summary>
    Task DeleteItemAsync(string id);

    /// <summary>
    /// 批量删除待办项
    /// </summary>
    /// <returns>实际删除的数量</returns>
    Task<int> BatchDeleteAsync(List<string> ids);

    /// <summary>
    /// 从 Excel 文件批量导入待办项
    /// </summary>
    Task<List<PersonalTask>> ImportFromExcelAsync(string filePath);

    /// <summary>
    /// 导出 Excel 模板（含表头+示例行）
    /// </summary>
    Task ExportTemplateAsync(string filePath);

    /// <summary>
    /// 获取今日到期任务数量
    /// </summary>
    Task<int> GetDueTodayCountAsync();

    /// <summary>
    /// 同步数据库表结构（仅在用户明确要求时调用）
    /// </summary>
        /// <summary>
    /// 获取即将到期任务数量（今日之后2天内到期）
    /// </summary>
    Task<int> GetUpcomingDueCountAsync();

    /// <summary>
    /// 同步数据库表结构（仅在用户明确要求时调用）
    /// </summary>
    Task SyncSchemaAsync();
}
