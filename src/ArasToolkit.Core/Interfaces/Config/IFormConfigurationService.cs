using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>Aras 经典窗体配置服务。</summary>
public interface IFormConfigurationService
{
    Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ArasFormProperty>> GetVisiblePropertiesAsync(
        string itemTypeId,
        CancellationToken cancellationToken = default);

    IReadOnlyList<ArasFormFieldLayout> BuildDefaultLayout(
        IEnumerable<ArasFormProperty> properties);

    void NormalizeLayoutOrder(IList<ArasFormFieldLayout> fields);

    Task<ArasFormConfigurationResult> ApplyAsync(
        ArasFormConfigurationRequest request,
        CancellationToken cancellationToken = default);
}
