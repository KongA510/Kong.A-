using System;
using System.Collections.ObjectModel;
using ArasToolkit.Core.Extensions;

namespace ArasToolkit.Core.Models;

/// <summary>
/// 工作流程模板定义。
/// 一份 Excel 文件只能解析为一个定义，流程名称由用户在导入页面中另行输入。
/// </summary>
public class WorkflowMapDefinition
{
    public string SourceFilePath { get; set; } = string.Empty;
    public string MapId { get; set; } = string.Empty;
    public ObservableCollection<WorkflowMapNode> Nodes { get; } = [];
    public ObservableCollection<WorkflowMapPath> Paths { get; } = [];
    public ObservableCollection<string> Warnings { get; } = [];
}

/// <summary>
/// 工作流程节点，对应 Aras 的 Activity Template，
/// 并通过 Workflow Map Activity 挂载到 Workflow Map。
/// </summary>
public class WorkflowMapNode : ObservableObject
{
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _activityType = NodeTypeNormal;
    private string _assignee = string.Empty;
    private bool _isAutomatic;
    private int _x;
    private int _y;
    private string _message = string.Empty;

    public const string NodeTypeStart = "开始";
    public const string NodeTypeNormal = "普通";
    public const string NodeTypeEnd = "结束";

    public string ActivityId { get; set; } = string.Empty;
    public string MapActivityId { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public string Code { get => _code; set => SetProperty(ref _code, value?.Trim() ?? string.Empty); }
    public string Name { get => _name; set => SetProperty(ref _name, value?.Trim() ?? string.Empty); }

    public string ActivityType
    {
        get => _activityType;
        set
        {
            if (SetProperty(ref _activityType, value?.Trim() ?? NodeTypeNormal))
            {
                OnPropertyChanged(nameof(IsStart));
                OnPropertyChanged(nameof(IsEnd));
            }
        }
    }

    /// <summary>
    /// 执行角色。既支持 Identity 名称，也支持 32 位 Identity ID。
    /// 人工节点留空时继承 Workflow Map 的流程所有者 Creator；开始、结束和自动节点也应留空。
    /// </summary>
    public string Assignee { get => _assignee; set => SetProperty(ref _assignee, value?.Trim() ?? string.Empty); }

    public bool IsAutomatic { get => _isAutomatic; set => SetProperty(ref _isAutomatic, value); }
    public int X { get => _x; set => SetProperty(ref _x, value); }
    public int Y { get => _y; set => SetProperty(ref _y, value); }
    public string Message { get => _message; set => SetProperty(ref _message, value?.Trim() ?? string.Empty); }

    public bool IsStart => string.Equals(ActivityType, NodeTypeStart, StringComparison.OrdinalIgnoreCase);
    public bool IsEnd => string.Equals(ActivityType, NodeTypeEnd, StringComparison.OrdinalIgnoreCase);

    public string[] ActivityTypeOptions { get; } = [NodeTypeStart, NodeTypeNormal, NodeTypeEnd];
}

/// <summary>
/// 工作流程路径，对应 Aras 的 Workflow Map Path。
/// </summary>
public class WorkflowMapPath : ObservableObject
{
    private string _sourceCode = string.Empty;
    private string _targetCode = string.Empty;
    private string _name = string.Empty;
    private bool _isDefault;
    private bool _isOverride;
    private string _authentication = "none";
    private string _segments = string.Empty;
    private int? _labelOffsetX;
    private int? _labelOffsetY;

    public string PathId { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public string SourceCode { get => _sourceCode; set => SetProperty(ref _sourceCode, value?.Trim() ?? string.Empty); }
    public string TargetCode { get => _targetCode; set => SetProperty(ref _targetCode, value?.Trim() ?? string.Empty); }
    public string Name { get => _name; set => SetProperty(ref _name, value?.Trim() ?? string.Empty); }
    public bool IsDefault { get => _isDefault; set => SetProperty(ref _isDefault, value); }
    public bool IsOverride { get => _isOverride; set => SetProperty(ref _isOverride, value); }
    public string Authentication { get => _authentication; set => SetProperty(ref _authentication, value?.Trim() ?? "none"); }

    /// <summary>
    /// Aras 原生转折点格式：单点为 x,y，多点使用竖线分隔，例如 390,70|220,70。
    /// 坐标是 Workflow Map 画布中的绝对坐标。
    /// </summary>
    public string Segments { get => _segments; set => SetProperty(ref _segments, value?.Trim() ?? string.Empty); }

    /// <summary>路径名称相对来源节点的 X 偏移；留空时由服务按折线中点自动计算。</summary>
    public int? LabelOffsetX { get => _labelOffsetX; set => SetProperty(ref _labelOffsetX, value); }

    /// <summary>路径名称相对来源节点的 Y 偏移；留空时由服务按折线中点自动计算。</summary>
    public int? LabelOffsetY { get => _labelOffsetY; set => SetProperty(ref _labelOffsetY, value); }

    public string[] AuthenticationOptions { get; } = ["none", "password", "esignature"];
}

/// <summary>工作流程画布坐标点。</summary>
public readonly record struct WorkflowMapPoint(int X, int Y);

/// <summary>准备执行的完整 AML，以及覆盖模式下解析出的系统现状。</summary>
public class WorkflowMapPreparedAml
{
    public string Aml { get; set; } = string.Empty;
    public string MapId { get; set; } = string.Empty;
    public bool IsOverwrite { get; set; }
    public int RemovedActivityCount { get; set; }
}

/// <summary>工作流程模板导入结果。</summary>
public class WorkflowMapImportResult
{
    public bool IsSuccess { get; set; }
    public string MapId { get; set; } = string.Empty;
    public string ExecutedAml { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
