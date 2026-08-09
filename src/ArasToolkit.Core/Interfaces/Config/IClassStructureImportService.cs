using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>通过 Excel 路径模板全量覆盖 Aras ItemType.class_structure。</summary>
public interface IClassStructureImportService
{
    byte[] GenerateTemplate();

    Task<List<ClassStructureItemType>> QueryItemTypesAsync(
        string? keyword = null,
        CancellationToken cancellationToken = default);

    Task<ClassStructurePreview> AnalyzeTemplateAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    Task<ClassStructureImportResult> ImportAsync(
        string filePath,
        ClassStructureItemType itemType,
        CancellationToken cancellationToken = default);
}
