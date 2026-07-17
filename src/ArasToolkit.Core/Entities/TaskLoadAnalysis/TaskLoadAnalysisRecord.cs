using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 任务负载分析记录 - 映射 task_load_analysis_record 表
/// </summary>
[Table("task_load_analysis_record")]
public class TaskLoadAnalysisRecord
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>分析开始日期</summary>
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    /// <summary>分析结束日期</summary>
    [Column("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>分析范围内的任务数量</summary>
    [Column("task_count")]
    public int TaskCount { get; set; }

    /// <summary>AI 分析完整结果</summary>
    [Column("analysis_result")]
    public string AnalysisResult { get; set; } = string.Empty;

    /// <summary>使用的 AI 模型名称</summary>
    [Column("model_name")]
    [MaxLength(200)]
    public string? ModelName { get; set; }

    /// <summary>用户 ID</summary>
    [Column("user_id")]
    [MaxLength(100)]
    public string? UserId { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== 非映射属性 =====

    [NotMapped]
    public string DisplayDateRange => $"{StartDate:yyyy-MM-dd} ~ {EndDate:yyyy-MM-dd}";

    [NotMapped]
    public string DisplayCreatedOn => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");

    [NotMapped]
    public string Preview => AnalysisResult.Length > 100
        ? AnalysisResult[..100] + "..."
        : AnalysisResult;
}
