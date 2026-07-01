using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 生命周期配置导入服务接口 — Excel模板下载 + Aras批量汇入 + 历史记录查询
/// </summary>
public interface ILifecycleImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（1个Sheet：生命周期配置）
    /// 列: 状态名称 | 状态标签 | 状态标签繁体 | 状态标签英文 | 所属对象类 | 是否为初始状态 | 允许流转目标 | 转移条件 | 备注说明
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 — 读取 Excel，导入生命周期状态到 Aras
    /// </summary>
    /// <param name="filePath">上传的 Excel 文件完整路径</param>
    /// <param name="importMode">导入模式："新增" 使用 add动作，"覆盖" 使用 merge动作。默认"覆盖"</param>
    /// <param name="progress">进度回调 — 报告结构化的进度信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果汇总</returns>
    Task<LifecycleImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入历史记录（分页）
    /// </summary>
    Task<(List<LifecycleImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    Task<LifecycleImportLog?> GetLogByIdAsync(string id);
}
