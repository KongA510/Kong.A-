using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 个人任务实体 — EF Core 映射 personal_task 表
/// </summary>
[Table("personal_task")]
public class PersonalTask
{
    /// <summary>唯一标识（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>任务名称</summary>
    [Column("task_name")]
    [Required]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>任务说明</summary>
    [Column("description")]
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>预计完成时间</summary>
    [Column("estinmated_time")]
    public DateTime? ExpectedDate { get; set; }

    /// <summary>开始时间</summary>
    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    /// <summary>完成时间</summary>
    [Column("completing_time")]
    public DateTime? CompletionDate { get; set; }

    /// <summary>项目名称</summary>
    [Column("project_name")]
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>备注</summary>
    [Column("remarks")]
    [MaxLength(1000)]
    public string? Remarks { get; set; }

    /// <summary>完成度 0-100</summary>
    [Column("completion_percent")]
    public int CompletionPercent { get; set; }

    /// <summary>完成状态: 未开始 | 处理中 | 延期 | 已完成</summary>
    [Column("task_state")]
    public string Status { get; set; } = "未开始";

    /// <summary>创建日期</summary>
    [Column("created_date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>最后修改日期</summary>
    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>记录创建时间（系统级，不可修改）</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
 
     /// <summary>创建人</summary>
     [Column("created_by")]
     [MaxLength(100)]
     public string? CreatedBy { get; set; }
 
     /// <summary>最后修改人</summary>
     [Column("modified_by")]
     [MaxLength(100)]
     public string? ModifiedBy { get; set; }

     /// <summary>所属用户ID（用于按用户筛选）</summary>
     [Column("user_id")]
     [MaxLength(100)]
     public string? UserId { get; set; }

    // ===== UI / 计算属性（不映射到数据库） =====

    /// <summary>UI 多选状态</summary>
    [NotMapped]
    public bool IsSelected { get; set; }

    /// <summary>预计完成时间是否就是今天</summary>
    [NotMapped]
    public bool IsDueToday => ExpectedDate?.Date == DateTime.Today;

    /// <summary>格式化日期显示</summary>
    [NotMapped]
    public string DisplayDate => ExpectedDate?.ToString("yyyy-MM-dd") ?? "";

    /// <summary>格式化开始时间</summary>
    [NotMapped]
    public string DisplayStartDate => StartDate?.ToString("yyyy-MM-dd") ?? "";

    /// <summary>格式化完成时间</summary>
    [NotMapped]
    public string DisplayCompletionDate => CompletionDate?.ToString("yyyy-MM-dd") ?? "";

    /// <summary>是否有完成时间</summary>
    [NotMapped]
    public bool HasCompletionDate => CompletionDate.HasValue;

    /// <summary>格式化创建日期</summary>
    [NotMapped]
    public string DisplayCreatedDate => CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>格式化修改日期</summary>
    [NotMapped]
    public string DisplayModifiedDate => ModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

    /// <summary>格式化记录创建时间</summary>
    [NotMapped]
    public string DisplayCreatorOn => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
