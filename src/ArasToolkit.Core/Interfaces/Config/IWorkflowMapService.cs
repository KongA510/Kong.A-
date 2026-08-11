using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 工作流程设定服务：生成模板、解析并校验预览、组装完整 AML、一次性导入 Aras。
/// </summary>
public interface IWorkflowMapService
{
    /// <summary>生成包含“流程节点”“流程路径”“填写说明”的标准 Excel 模板。</summary>
    byte[] GenerateTemplate();

    /// <summary>读取一份 Excel 文件并解析为唯一的一份工作流程定义。</summary>
    Task<WorkflowMapDefinition> ParseTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 规范化并校验预览数据。退回路径未填写转折点时，会自动生成上方正交折线。
    /// </summary>
    void NormalizeAndValidate(WorkflowMapDefinition definition);

    /// <summary>解析 Aras 原生 segments 字符串。</summary>
    IReadOnlyList<WorkflowMapPoint> ParseSegments(string segments);

    /// <summary>把转折点序列化为 Aras 原生 segments 字符串。</summary>
    string SerializeSegments(IEnumerable<WorkflowMapPoint> points);

    /// <summary>
    /// 根据当前系统中是否已有同名模板，生成最终会执行的完整 AML。
    /// 覆盖模式会保留既有 Workflow Map ID，并在同一个 AML 中删除旧节点关系后添加新结构。
    /// </summary>
    Task<WorkflowMapPreparedAml> PrepareAmlAsync(
        WorkflowMapDefinition definition,
        string workflowName,
        string description,
        string importMode,
        CancellationToken cancellationToken = default);

    /// <summary>通过 Innovator.applyAML 一次性执行准备好的完整 AML。</summary>
    Task<WorkflowMapImportResult> ImportAsync(
        WorkflowMapPreparedAml preparedAml,
        string workflowName,
        CancellationToken cancellationToken = default);
}
