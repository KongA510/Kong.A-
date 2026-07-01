using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 对象类配置导入服务接口 — Excel模板下载 + Aras批量汇入 + 历史记录查询
/// </summary>
public interface IObjectClassImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（2个Sheet：对象类新增 + 关系类新增）
    /// </summary>
    /// <returns>Excel 文件字节数组，可直接写入磁盘</returns>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 — 读取 Excel，先建对象类再建关系类（单线程串行执行，保证数据一致性）
    /// </summary>
    /// <param name="filePath">上传的 Excel 文件完整路径</param>
    /// <param name="importMode">导入模式："新增" 使用 add动作，"覆盖" 使用 merge where动作。默认"覆盖"</param>
    /// <param name="progress">进度回调 — 报告结构化的进度信息（阶段/当前/总数/百分比），用于UI实时展示</param>
    /// <param name="cancellationToken">取消令牌 — 用户可取消长时间运行的导入操作</param>
    /// <returns>导入结果汇总（成功数/失败数/日志路径/错误明细）</returns>
    Task<ObjectClassImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入历史记录（分页）
    /// </summary>
    /// <param name="userId">按用户ID筛选，null=全部</param>
    /// <param name="page">页码（1-based）</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>分页记录列表 + 总条数</returns>
    Task<(List<ObjectClassImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    /// <param name="id">导入记录主键</param>
    /// <returns>导入记录实体，不存在时返回 null</returns>
    Task<ObjectClassImportLog?> GetLogByIdAsync(string id);
}
