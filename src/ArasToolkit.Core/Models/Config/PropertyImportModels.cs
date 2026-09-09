using ArasToolkit.Core.Extensions;

namespace ArasToolkit.Core.Models;

/// <summary>属性模板中的数据类型标签及写入 Aras Property.data_type 的真实值。</summary>
public sealed record PropertyDataTypeOption(
    string Label,
    string Value,
    int? DefaultStoredLength = null,
    int? DefaultPrecision = null,
    int? DefaultScale = null,
    string DataSourceHint = "");

/// <summary>属性配置支持的数据类型及默认规则。</summary>
public static class PropertyDataTypeOptions
{
    public static IReadOnlyList<PropertyDataTypeOption> All { get; } =
    [
        new("String", "string", 256),
        new("Text", "text"),
        new("Integer", "integer"),
        new("Float", "float"),
        new("Decimal", "decimal", null, 10, 2),
        new("Boolean", "boolean"),
        new("Date", "date"),
        new("Image", "image"),
        new("MD5", "md5"),
        new("Sequence", "sequence", null, null, null, "填写 Aras Sequence 名称"),
        new("Item", "item", null, null, null, "填写 Aras ItemType 名称"),
        new("List", "list", 64, null, null, "填写 Aras List 名称"),
        new("Filter List", "filter list", 64, null, null, "填写 Aras List 名称"),
        new("Color List", "color list", 64, null, null, "填写 Aras List 名称"),
        new("Color", "color"),
        new("Formatted Text", "formatted text"),
        new("Foreign", "foreign", null, null, null, "填写当前对象类中已有的 Item 属性名称"),
        new("Multilingual String", "ml_string", 256),
        new("Multi Value List", "mv_list", 64, null, null, "填写 Aras List 名称"),
        new("Global Version", "global_version"),
        new("Unsigned BigInt", "ubigint")
    ];

    public static bool TryResolve(string? input, out PropertyDataTypeOption option)
    {
        option = All.FirstOrDefault(item =>
            item.Label.Equals(input?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            item.Value.Equals(input?.Trim(), StringComparison.OrdinalIgnoreCase))!;
        return option != null;
    }
}

/// <summary>Excel 文件本地读取、默认值归一化及 Aras AML 预组装后的单行状态。</summary>
public sealed class PropertyImportPreviewRow : ObservableObject
{
    private string _plannedAction = "待组装";
    private string _validationMessage = string.Empty;
    private string _amlPreview = string.Empty;
    private string _submitStatus = "待提交";

    public int ExcelRowNumber { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LabelZhCn { get; init; } = string.Empty;
    public string LabelZhTw { get; init; } = string.Empty;
    public string LabelEn { get; init; } = string.Empty;
    public string DataTypeLabel { get; init; } = string.Empty;
    public string DataTypeValue { get; init; } = string.Empty;
    public string DataSource { get; init; } = string.Empty;
    public string ForeignProperty { get; init; } = string.Empty;
    public string ClassPath { get; init; } = string.Empty;
    public string ColumnAlignment { get; init; } = string.Empty;
    public string ColumnWidth { get; init; } = string.Empty;
    public string DefaultSearch { get; init; } = string.Empty;
    public string DefaultValueZhCn { get; init; } = string.Empty;
    public string DefaultValueZhTw { get; init; } = string.Empty;
    public string DefaultValueEn { get; init; } = string.Empty;
    public string HelpText { get; init; } = string.Empty;
    public string HelpTooltipZhCn { get; init; } = string.Empty;
    public string HelpTooltipZhTw { get; init; } = string.Empty;
    public string HelpTooltipEn { get; init; } = string.Empty;
    public string IsCopy { get; init; } = string.Empty;
    public string IsFederated { get; init; } = string.Empty;
    public string IsHidden { get; init; } = string.Empty;
    public string IsHidden2 { get; init; } = string.Empty;
    public string IsKeyed { get; init; } = string.Empty;
    public string IsRequired { get; init; } = string.Empty;
    public string ItemBehavior { get; init; } = string.Empty;
    public string KeyedNameOrder { get; init; } = string.Empty;
    public string OrderBy { get; init; } = string.Empty;
    public string Pattern { get; init; } = string.Empty;
    public string Precision { get; init; } = string.Empty;
    public string Scale { get; init; } = string.Empty;
    public string SortOrder { get; init; } = string.Empty;
    public string StoredLength { get; init; } = string.Empty;
    public string TrackHistory { get; init; } = string.Empty;

    /// <summary>解析后的 data_source GUID，仅用于 AML 组装。</summary>
    public string ResolvedDataSourceId { get; set; } = string.Empty;

    /// <summary>解析后的 foreign_property GUID，仅用于 AML 组装。</summary>
    public string ResolvedForeignPropertyId { get; set; } = string.Empty;

    /// <summary>覆盖模式命中的现有 Property GUID；为空表示将新增。</summary>
    public string ExistingPropertyId { get; set; } = string.Empty;

    public string PlannedAction
    {
        get => _plannedAction;
        set => SetProperty(ref _plannedAction, value);
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        set
        {
            if (SetProperty(ref _validationMessage, value))
                OnPropertyChanged(nameof(IsValid));
        }
    }

    public string AmlPreview
    {
        get => _amlPreview;
        set
        {
            if (SetProperty(ref _amlPreview, value))
                OnPropertyChanged(nameof(HasAmlPreview));
        }
    }

    public string SubmitStatus
    {
        get => _submitStatus;
        set
        {
            if (SetProperty(ref _submitStatus, value))
                OnPropertyChanged(nameof(IsSubmitting));
        }
    }

    public bool IsSubmitting => SubmitStatus == "正在提交";

    public bool IsValid => string.IsNullOrWhiteSpace(ValidationMessage);
    public bool HasAmlPreview => !string.IsNullOrWhiteSpace(AmlPreview);
    public string TypeDisplay => $"{DataTypeLabel} → {DataTypeValue}";
    public string LabelSummary => $"简 {LabelZhCn} · 繁 {LabelZhTw} · EN {LabelEn}";
    public string RuleSummary
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(StoredLength)) parts.Add($"长度 {StoredLength}");
            if (!string.IsNullOrWhiteSpace(Precision)) parts.Add($"精度 {Precision}");
            if (!string.IsNullOrWhiteSpace(Scale)) parts.Add($"小数 {Scale}");
            if (!string.IsNullOrWhiteSpace(SortOrder)) parts.Add($"顺序 {SortOrder}");
            return parts.Count == 0 ? "使用 Aras 默认值" : string.Join(" · ", parts);
        }
    }
}

/// <summary>模板预检及 AML 预组装结果。</summary>
public sealed class PropertyImportPreview
{
    public List<PropertyImportPreviewRow> Rows { get; init; } = [];
    public bool IsPrepared { get; set; }
    public string ItemTypeId { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
    public string ImportMode { get; set; } = string.Empty;
    public int ValidCount => Rows.Count(row => row.IsValid);
    public int InvalidCount => Rows.Count - ValidCount;
    public bool CanImport => Rows.Count > 0 && InvalidCount == 0 && IsPrepared;
    public string Summary => Rows.Count == 0
        ? "模板中没有可导入的数据"
        : $"共 {Rows.Count} 行 · 可提交 {ValidCount} 行 · 需修正 {InvalidCount} 行";
}
