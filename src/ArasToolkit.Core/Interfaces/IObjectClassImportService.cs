using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 对象类配置导入服务接口 — Excel模板下载 + Aras批量汇入
/// </summary>
public interface IObjectClassImportService
{
    /// <summary>
    /// 生成 Excel 模板文件（2个Sheet：对象类新增 + 关系类新增）
    /// </summary>
    byte[] GenerateTemplate();

    /// <summary>
    /// 执行导入 — 读取Excel，先建对象类再建关系类（单线程串行）
    /// </summary>
    /// <param name="filePath">上传的Excel文件完整路径</param>
    /// <param name="progress">进度回调</param>
    /// <returns>导入结果汇总</returns>
    Task<ObjectClassImportResult> ImportAsync(string filePath, IProgress<string>? progress = null);

    /// <summary>获取导入历史（分页）</summary>
    Task<(List<ObjectClassImportLog> Items, int TotalCount)> GetHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    Task<ObjectClassImportLog?> GetLogByIdAsync(string id);
}
