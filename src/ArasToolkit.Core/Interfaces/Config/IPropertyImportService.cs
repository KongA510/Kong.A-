using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 属性配置导入服务接口 — Excel模板下载 + Aras批量汇入 + 历史记录查询
/// </summary>
public interface IPropertyImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（1个Sheet：属性配置）
    /// 列: 源对象 | 属性名称 | 属性标签 | 属性标签繁体 | 属性标签英文 | 数据类型 | 数据源 | 长度 | 必填 | 显示顺序 | 显示宽度 | 设置为键名 | 默认值
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 — 读取 Excel，导入属性到 Aras
    /// </summary>
    /// <param name="filePath">上传的 Excel 文件完整路径</param>
    /// <param name="importMode">导入模式："新增" 使用 add动作，"覆盖" 使用 merge动作。默认"覆盖"</param>
    /// <param name="progress">进度回调 — 报告结构化的进度信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果汇总</returns>
    Task<PropertyImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入历史记录（分页）
    /// </summary>
    Task<(List<PropertyImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    Task<PropertyImportLog?> GetLogByIdAsync(string id);
}
