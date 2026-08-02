using System.Text.Json.Serialization;
using ArasToolkit.Core.Extensions;

namespace ArasToolkit.Core.Models;

/// <summary>对象类基础设定列表项。</summary>
public sealed class ObjectClassConfigurationItem : ObservableObject
{
    private bool _isSelected;
    private string _operationSummary = string.Empty;
    private string _defaultPermissionName = string.Empty;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LabelEn { get; set; } = string.Empty;
    public string LabelZc { get; set; } = string.Empty;
    public string LabelZt { get; set; } = string.Empty;
    public string DefaultPermissionName
    {
        get => _defaultPermissionName;
        set
        {
            if (!SetProperty(ref _defaultPermissionName, value))
                return;
            OnPropertyChanged(nameof(HasDefaultPermission));
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string OperationSummary
    {
        get => _operationSummary;
        set => SetProperty(ref _operationSummary, value);
    }

    [JsonIgnore]
    public bool HasDefaultPermission => !string.IsNullOrWhiteSpace(DefaultPermissionName);
}

/// <summary>可持久化的对象类基础设定角色模板。</summary>
public sealed class ObjectClassConfigurationSettings
{
    public string CreatorFullControlRoles { get; set; } = "Creator";
    public string ReadOnlyRoles { get; set; } = "World,Owner,Manager";
    public string AdministratorFullControlRoles { get; set; } = "Aras PLM,Administrator,Innovator Admin";
    public string CanAddRoles { get; set; } = "World";
    public string TransitionRole { get; set; } = "Aras PLM";

    public static ObjectClassConfigurationSettings CreateDefault() => new();
}

/// <summary>本次一键设定需要执行的项目。</summary>
public sealed class ObjectClassConfigurationOptions
{
    public bool ConfigureDefaultPermission { get; set; } = true;
    public bool ConfigureCanAdd { get; set; } = true;
    public bool ConfigureLifecycle { get; set; } = true;

    [JsonIgnore]
    public bool HasAnySelection =>
        ConfigureDefaultPermission || ConfigureCanAdd || ConfigureLifecycle;
}

/// <summary>对象类基础设定批次结果。</summary>
public sealed class ObjectClassConfigurationBatchResult
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> FailedDetails { get; set; } = [];
}

/// <summary>对象类基础设定执行进度。</summary>
public sealed class ObjectClassConfigurationProgress
{
    public int Current { get; set; }
    public int Total { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;

    [JsonIgnore]
    public double Percentage => Total <= 0 ? 0 : Current * 100d / Total;

    [JsonIgnore]
    public string StatusText => Total <= 0
        ? Phase
        : $"{Phase} · {Current}/{Total} · {ItemTypeName}";
}
