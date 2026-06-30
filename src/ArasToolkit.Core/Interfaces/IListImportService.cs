using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// List配置导入服务接口 — Excel模板下载 + Aras批量汇入 + 历史记录查询
///
/// 导入顺序（严格按序执行）:
/// 1. Sheet1「List主档」→ 创建/更新 List
/// 2. Sheet2「菜单内容」→ 创建/更新 菜单项
/// 3. Sheet3「菜单内容(过滤)」→ 创建/更新 菜单过滤规则
///
/// 注意: AML 模板逻辑由调用方自行维护，Service 提供注入入口
/// </summary>
public interface IListImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（3个Sheet：List主档 + 菜单内容 + 菜单内容过滤）
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 — 按序处理3个Sheet，通过 AML 汇入 Aras
    /// </summary>
    /// <param name="filePath">Excel 文件完整路径</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <param name="progress">结构化进度回调（百分比 + 阶段 + 当前条目）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果汇总</returns>
    Task<ListImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入历史记录（分页，按创建时间倒序）
    /// </summary>
    Task<(List<ListImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    Task<ListImportLog?> GetLogByIdAsync(string? id);
}
