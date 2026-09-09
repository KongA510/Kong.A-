using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public interface IFormConfigurationEditService
{
    Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FormEditorSummary>> GetFormsAsync(string itemTypeId, CancellationToken cancellationToken = default);
    Task<FormEditorDefinition> GetDefinitionAsync(string itemTypeId, string formId, CancellationToken cancellationToken = default);
    Task<FormEditorMetadata> GetEditorMetadataAsync(CancellationToken cancellationToken = default);
    FormEditorChangeSet BuildChangeSet(FormEditorDefinition document, FormEditorMetadata metadata);
    Task<FormEditorDefinition> SaveAsync(FormEditorDefinition document, FormEditorMetadata metadata, CancellationToken cancellationToken = default);
    Task<FormEditorResource?> GetPreviewResourceAsync(string relativeOrAbsoluteUrl, string connectionKey, CancellationToken cancellationToken = default);
}
