using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>对象类默认权限、可创建者和生命周期的一键基础设定服务。</summary>
public interface IObjectClassConfigurationService
{
    Task<List<ObjectClassConfigurationItem>> QueryItemTypesAsync(
        string? keyword = null,
        CancellationToken cancellationToken = default);

    Task<ObjectClassConfigurationSettings> LoadSettingsAsync();

    Task SaveSettingsAsync(ObjectClassConfigurationSettings settings);

    Task<ObjectClassConfigurationBatchResult> ConfigureAsync(
        IReadOnlyList<ObjectClassConfigurationItem> itemTypes,
        ObjectClassConfigurationOptions options,
        ObjectClassConfigurationSettings settings,
        IProgress<ObjectClassConfigurationProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
