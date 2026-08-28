using System.Collections.ObjectModel;

namespace ArasToolkit.Core.Models;

/// <summary>从 Excel 读取的 Aras 项目计划模板定义。</summary>
public class ProjectPlanTemplateDefinition
{
    public string SourceFilePath { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string ManagedByIdentity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TemplateVersion { get; set; } = string.Empty;
    public ObservableCollection<ProjectPlanNode> Nodes { get; } = [];
    public ObservableCollection<string> Warnings { get; } = [];

    public int PhaseCount => Nodes.Count(node => node.NodeType == ProjectPlanNodeType.Phase);
    public int ActivityCount => Nodes.Count(node => node.NodeType == ProjectPlanNodeType.Activity);
    public int MilestoneCount => Nodes.Count(node => node.NodeType == ProjectPlanNodeType.Milestone);
}

/// <summary>项目计划中的一个 WBS 阶段、任务或里程碑。</summary>
public class ProjectPlanNode
{
    public int SourceRow { get; set; }
    public int SortOrder { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ParentCode { get; set; } = string.Empty;
    public string NodeType { get; set; } = ProjectPlanNodeType.Activity;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ExpectedDuration { get; set; }
    public bool IsExpectedDurationSpecified { get; set; } = true;
    public decimal WorkEstimate { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public string PredecessorCodes { get; set; } = string.Empty;
    public string PrecedenceType { get; set; } = "Finish to Start";
    public decimal LeadLag { get; set; }

    public string DisplayType => NodeType switch
    {
        ProjectPlanNodeType.Phase => "阶段",
        ProjectPlanNodeType.Milestone => "里程碑",
        _ => "任务"
    };
}

/// <summary>项目计划节点类型常量。</summary>
public static class ProjectPlanNodeType
{
    public const string Phase = "Phase";
    public const string Activity = "Activity";
    public const string Milestone = "Milestone";
}

/// <summary>已经完成当前 Aras 实例预检、可直接提交的 AML。</summary>
public class ProjectPlanPreparedImport
{
    public string Aml { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string RootWbsId { get; set; } = string.Empty;
    public int NodeCount { get; set; }
    public int LeadRoleCount { get; set; }
    public int PredecessorCount { get; set; }
    public List<ProjectPlanImportStep> Steps { get; } = [];
}

/// <summary>按照 ArasLabs 官方导入顺序执行的单个可回滚写入步骤。</summary>
public class ProjectPlanImportStep
{
    public string ItemType { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Aml { get; set; } = string.Empty;
}

/// <summary>项目计划模板汇入结果。</summary>
public class ProjectPlanImportResult
{
    public bool IsSuccess { get; set; }
    public string TemplateId { get; set; } = string.Empty;
    public string RootWbsId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
