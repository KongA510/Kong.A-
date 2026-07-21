using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 权限配置导入服务接口 — Excel模板生成(2个Sheet) + Aras权限导入 + 历史记录查询
/// </summary>
public interface IPermissionImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（2个Sheet）
    /// Sheet1「权限配置」: 权限配置名称 | 权限名称
    /// Sheet2「详细权限」: 权限名称 | 所属角色 | 允许创建 | 允许读取 | 允许更新 | 允许删除
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 → 读取 Excel（先Sheet1再Sheet2），导入权限到 Aras
    /// </summary>
    /// <param name="filePath">上传的 Excel 文件绝对路径</param>
    /// <param name="importMode">导入模式："新增" 使用 add；"覆盖" 使用 merge；默认"覆盖"</param>
    /// <param name="progress">进度回调 → 结构化进度信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果</returns>
    Task<PermissionImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入历史记录（分页）
    /// </summary>
    Task<(List<PermissionImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    Task<PermissionImportLog?> GetLogByIdAsync(string id);
}
