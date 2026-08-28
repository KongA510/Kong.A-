using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Aras 项目计划模板汇入服务。
/// 数据模型遵循 Project Template.wbs_id → WBS Element → Sub WBS / WBS Activity2。
/// </summary>
public interface IProjectPlanImportService
{
    byte[] GenerateTemplate();

    Task<ProjectPlanTemplateDefinition> ParseTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    void NormalizeAndValidate(ProjectPlanTemplateDefinition definition);

    Task<ProjectPlanPreparedImport> PrepareImportAsync(
        ProjectPlanTemplateDefinition definition,
        CancellationToken cancellationToken = default);

    Task<ProjectPlanImportResult> ImportAsync(
        ProjectPlanPreparedImport preparedImport,
        string templateName,
        CancellationToken cancellationToken = default);
}
