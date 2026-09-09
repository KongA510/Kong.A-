using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 属性配置导入服务接口 — 模板下载、模板预检、对象类选择、AML 组装、逐条提交及历史查询。
/// </summary>
public interface IPropertyImportService
{
    /// <summary>
    /// 生成 Excel 模板文件。模板包含“属性配置”和“数据字典”两个 Sheet，
    /// 数据类型、必填和唯一均使用 Office 原生下拉校验。
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    byte[] GenerateTemplate();

    /// <summary>从当前 Aras 连接读取可选择的现有对象类。</summary>
    Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>仅读取并校验模板，不访问 Aras。</summary>
    Task<PropertyImportPreview> PreviewAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 针对选中的对象类解析 Item/List/Foreign 等引用，并生成每一行的最终 AML 预览。
    /// </summary>
    Task<PropertyImportPreview> PrepareAsync(
        string filePath,
        string itemTypeId,
        string itemTypeName,
        string importMode = "覆盖",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行导入 — 读取 Excel，逐行提交属性后对目标 ItemType 执行一次不修改字段的 edit。
    /// 部分成功或取消时仍保存已提交的属性；对象类保存失败时结果不会标记成功。
    /// </summary>
    /// <param name="filePath">上传的 Excel 文件完整路径</param>
    /// <param name="itemTypeId">用户在系统现有对象类列表中选中的 ItemType GUID</param>
    /// <param name="itemTypeName">选中的 ItemType 名称，仅用于日志和连接一致性校验</param>
    /// <param name="importMode">“新增”仅 add；“覆盖”按 source_id + name 唯一查询，命中后 edit，否则 add</param>
    /// <param name="progress">进度回调 — 报告结构化的进度信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果汇总</returns>
    Task<PropertyImportResult> ImportAsync(
        string filePath,
        string itemTypeId,
        string itemTypeName,
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
