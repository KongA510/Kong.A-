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
public sealed class ArasFormFieldLayout : ObservableObject
{
    private int _x;
    private int _y;
    private int _sequence;
    private int _displayLength = 150;
    private int _textAreaRows = 100;
    private int _textAreaColumns = 340;
    private string _fieldType = ArasFormConfigurationOptions.DefaultFieldType;
    private bool _isDisabled;
    private string _fontColor = ArasFormConfigurationOptions.DefaultFontColor;

    public string PropertyId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty;
    public string FieldType
    {
        get => _fieldType;
        set
        {
            var normalized = ArasFormConfigurationOptions.NormalizeFieldType(value);
            if (SetProperty(ref _fieldType, normalized))
            {
                OnPropertyChanged(nameof(IsTextAreaField));
                OnPropertyChanged(nameof(FieldTypeLabel));
            }
        }
    }

    public int X
    {
        get => _x;
        set => SetProperty(ref _x, Math.Max(0, value));
    }

    public int Y
    {
        get => _y;
        set => SetProperty(ref _y, Math.Max(0, value));
    }

    public int DisplayLength
    {
        get => _displayLength;
        set => SetProperty(ref _displayLength, Math.Max(1, value));
    }

    public int Sequence
    {
        get => _sequence;
        set => SetProperty(ref _sequence, value);
    }

    /// <summary>textarea 控件的 textarea_rows；其他控件不会写入 AML。</summary>
    public int TextAreaRows
    {
        get => _textAreaRows;
        set => SetProperty(ref _textAreaRows, Math.Max(1, value));
    }

    /// <summary>textarea 控件的 textarea_cols；其他控件不会写入 AML。</summary>
    public int TextAreaColumns
    {
        get => _textAreaColumns;
        set => SetProperty(ref _textAreaColumns, Math.Max(1, value));
    }

    public bool IsDisabled
    {
        get => _isDisabled;
        set => SetProperty(ref _isDisabled, value);
    }

    public string FontColor
    {
        get => _fontColor;
        set
        {
            if (SetProperty(ref _fontColor, ArasFormConfigurationOptions.NormalizeFontColor(value)))
                OnPropertyChanged(nameof(FontColorDisplayText));
        }
    }

    /// <summary>当前选择的控件类型是否为多行文本。</summary>
    public bool IsTextAreaField => FieldType.Equals(
        ArasFormConfigurationOptions.TextAreaFieldType,
        StringComparison.OrdinalIgnoreCase);

    public string FieldTypeLabel
    {
        get => ArasFormConfigurationOptions.GetFieldTypeLabel(FieldType);
        set => FieldType = ArasFormConfigurationOptions.GetFieldTypeFromLabel(value);
    }

    public string FontColorDisplayText
    {
        get => ArasFormConfigurationOptions.GetFontColorDisplayText(FontColor);
        set => FontColor = ArasFormConfigurationOptions.GetFontColorFromDisplayText(value);
    }

    public IReadOnlyList<string> AvailableFieldTypes => ArasFormConfigurationOptions.FieldTypeLabels;
    public IReadOnlyList<string> AvailableFontColors => ArasFormConfigurationOptions.FontColorDisplayOptions;
}

/// <summary>创建或覆盖 Aras 窗体的请求。</summary>
public sealed class ArasFormConfigurationRequest
{
    /// <summary>显式窗体高度；省略时保留历史调用的自动计算行为。</summary>
    public int? FormHeight { get; init; }
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
