using ArasToolkit.Core.Extensions;

namespace ArasToolkit.Core.Models;

/// <summary>可用于生成窗体的 Aras 对象类。</summary>
public sealed class ArasItemTypeInfo
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DisplayName => string.IsNullOrWhiteSpace(Label) || Label == Name
        ? Name
        : $"{Label} ({Name})";
}

/// <summary>对象类属性及其窗体选择状态。</summary>
public sealed class ArasFormProperty : ObservableObject
{
    private bool _isSelected = true;

    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty;
    public string DataSourceId { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsHiddenInSearch { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string DisplayLabel => string.IsNullOrWhiteSpace(Label) ? Name : Label;
}

/// <summary>经典 Aras 窗体中一个字段的布局结果。</summary>
public sealed class ArasFormFieldLayout
{
    public string PropertyId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty;
    public string FieldType { get; init; } = string.Empty;
    public int X { get; init; }
    public int Y { get; init; }
    public int DisplayLength { get; init; }
    public int Sequence { get; init; }
}

/// <summary>创建或覆盖 Aras 窗体的请求。</summary>
public sealed class ArasFormConfigurationRequest
{
    public string ItemTypeId { get; init; } = string.Empty;
    public string ItemTypeName { get; init; } = string.Empty;
    public string FormName { get; init; } = string.Empty;
    public string FormLabel { get; init; } = string.Empty;
    public bool ReplaceExisting { get; init; }
    public bool SetAsDefaultView { get; init; } = true;
    public IReadOnlyList<ArasFormFieldLayout> Fields { get; init; } = [];
}

/// <summary>窗体配置写入结果。</summary>
public sealed class ArasFormConfigurationResult
{
    public string FormId { get; init; } = string.Empty;
    public string FormName { get; init; } = string.Empty;
    public int FieldCount { get; init; }
    public bool WasUpdated { get; init; }
    public bool ViewAssigned { get; init; }
}
