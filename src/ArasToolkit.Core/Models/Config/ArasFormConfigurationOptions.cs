using System.Collections.Frozen;

namespace ArasToolkit.Core.Models;

/// <summary>窗体配置下拉选项。</summary>
public sealed record ArasFormOption(string Label, string Value)
{
    public string DisplayText => $"{Label} · {Value}";
}

/// <summary>
/// Aras Field 的受支持选项与默认映射。
/// 使用进程内只读集合，避免页面打开时重复读取和解析配置文件。
/// </summary>
public static class ArasFormConfigurationOptions
{
    public const string DefaultFieldType = "text";
    public const string TextAreaFieldType = "textarea";
    public const string DefaultFontColor = "#333333";

    public static IReadOnlyList<ArasFormOption> FieldTypes { get; } =
    [
        new("Text", "text"),
        new("Password", "password"),
        new("Text Area", "textarea"),
        new("Checkbox", "checkbox"),
        new("FormattedText", "formatted text"),
        new("Label", "label"),
        new("Button", "button"),
        new("Radio Button List", "radio button list"),
        new("Dropdown", "dropdown"),
        new("Listbox Single Select", "listbox single select"),
        new("Listbox Multi Select", "listbox multi select"),
        new("Checkbox List", "checkbox list"),
        new("Date", "date"),
        new("Color", "color"),
        new("Color List", "color list"),
        new("HTML", "html"),
        new("Item", "item"),
        new("Nested Form", "nested form"),
        new("File Item", "file item"),
        new("Class Structure", "class structure"),
        new("Image", "image"),
        new("Multilingual String", "ml_string"),
        new("Group Box", "groupbox")
    ];

    public static IReadOnlyList<ArasFormOption> FontColors { get; } =
    [
        new("黑色", "#333333"),
        new("蓝色", "#0000ff"),
        new("红色", "#DC2626"),
        new("绿色", "#16A34A"),
        new("橙色", "#F97316"),
        new("紫色", "#7C3AED"),
        new("靛青", "#4F46E5"),
        new("灰色", "#6B7280"),
        new("白色", "#FFFFFF")
    ];

    public static IReadOnlyList<string> FieldTypeDisplayOptions { get; } = FieldTypes
        .Select(option => option.DisplayText)
        .ToArray();

    public static IReadOnlyList<string> FontColorDisplayOptions { get; } = FontColors
        .Select(option => option.DisplayText)
        .ToArray();

    private static readonly FrozenDictionary<string, string> DataTypeFieldTypeMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["string"] = "text",
            ["text"] = "textarea",
            ["item"] = "item",
            ["list"] = "dropdown",
            ["mv_list"] = "dropdown",
            ["filter list"] = "dropdown",
            ["boolean"] = "checkbox",
            ["date"] = "date",
            ["color"] = "color",
            ["color list"] = "color list",
            ["formatted text"] = "formatted text",
            ["image"] = "image",
            ["file item"] = "file item",
            ["ml_string"] = "ml_string"
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> CanonicalFieldTypes = FieldTypes
        .ToFrozenDictionary(option => option.Value, option => option.Value, StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> CanonicalFontColors = FontColors
        .ToFrozenDictionary(option => option.Value, option => option.Value, StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> FieldTypeDisplayToValue = FieldTypes
        .ToFrozenDictionary(option => option.DisplayText, option => option.Value, StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> FieldTypeValueToDisplay = FieldTypes
        .ToFrozenDictionary(option => option.Value, option => option.DisplayText, StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> FontColorDisplayToValue = FontColors
        .ToFrozenDictionary(option => option.DisplayText, option => option.Value, StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> FontColorValueToDisplay = FontColors
        .ToFrozenDictionary(option => option.Value, option => option.DisplayText, StringComparer.OrdinalIgnoreCase);

    public static string GetDefaultFieldType(string? dataType)
    {
        var key = dataType?.Trim() ?? string.Empty;
        return DataTypeFieldTypeMap.TryGetValue(key, out var fieldType)
            ? fieldType
            : DefaultFieldType;
    }

    public static string NormalizeFieldType(string? fieldType)
    {
        var key = fieldType?.Trim() ?? string.Empty;
        return CanonicalFieldTypes.TryGetValue(key, out var canonical)
            ? canonical
            : DefaultFieldType;
    }

    public static string NormalizeFontColor(string? fontColor)
    {
        var key = fontColor?.Trim() ?? string.Empty;
        return CanonicalFontColors.TryGetValue(key, out var canonical)
            ? canonical
            : DefaultFontColor;
    }

    public static string GetFieldTypeDisplayText(string? fieldType)
    {
        var normalized = NormalizeFieldType(fieldType);
        return FieldTypeValueToDisplay[normalized];
    }

    public static string GetFieldTypeFromDisplayText(string? displayText)
    {
        var key = displayText?.Trim() ?? string.Empty;
        return FieldTypeDisplayToValue.TryGetValue(key, out var value)
            ? value
            : DefaultFieldType;
    }

    public static string GetFontColorDisplayText(string? fontColor)
    {
        var normalized = NormalizeFontColor(fontColor);
        return FontColorValueToDisplay[normalized];
    }

    public static string GetFontColorFromDisplayText(string? displayText)
    {
        var key = displayText?.Trim() ?? string.Empty;
        return FontColorDisplayToValue.TryGetValue(key, out var value)
            ? value
            : DefaultFontColor;
    }

    public static bool IsKnownFieldType(string? fieldType) =>
        !string.IsNullOrWhiteSpace(fieldType) && CanonicalFieldTypes.ContainsKey(fieldType.Trim());

    public static bool IsKnownFontColor(string? fontColor) =>
        !string.IsNullOrWhiteSpace(fontColor) && CanonicalFontColors.ContainsKey(fontColor.Trim());
}
