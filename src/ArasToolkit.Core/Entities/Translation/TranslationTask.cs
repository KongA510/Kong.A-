using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

[Table("translation_task")]
public class TranslationTask
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("task_name")]
    [MaxLength(200)]
    [Required]
    public string TaskName { get; set; } = string.Empty;

    [Column("task_type")]
    [MaxLength(50)]
    public string TaskType { get; set; } = string.Empty;

    [Column("query_mode")]
    [MaxLength(50)]
    public string QueryMode { get; set; } = string.Empty;

    [Column("query_condition")]
    public string? QueryCondition { get; set; }

    [Column("source_language")]
    [MaxLength(50)]
    public string? SourceLanguage { get; set; }

    [Column("target_languages")]
    [MaxLength(500)]
    public string? TargetLanguages { get; set; }

    [Column("total_count")]
    public int TotalFields { get; set; }

    [Column("completed_count")]
    public int TranslatedFields { get; set; }

    [NotMapped]
    public string? ProgressText { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [Column("output_file_path")]
    [MaxLength(1000)]
    public string? OutputFilePath { get; set; }

    [Column("ai_model_id")]
    [MaxLength(12)]
    public string? AiModelId { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [Column("completed_on")]
    public DateTime? CompletedOn { get; set; }

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");

    [NotMapped]
    public string CompletionText => $"{TranslatedFields}/{TotalFields}";

    [NotMapped]
    public string? UserId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(QueryCondition) ||
                !QueryCondition.StartsWith("user:", StringComparison.Ordinal))
                return null;
            var separator = QueryCondition.IndexOf("|scope:", StringComparison.Ordinal);
            return separator > 5 ? QueryCondition[5..separator] : null;
        }
    }

    [NotMapped]
    public string ScopeId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(QueryCondition)) return string.Empty;
            var marker = QueryCondition.IndexOf("|scope:", StringComparison.Ordinal);
            return marker >= 0 ? QueryCondition[(marker + 7)..] : QueryCondition;
        }
    }

    [NotMapped]
    public string StatusText => Status switch
    {
        "Pending" => "等待中",
        "Translating" => "翻译中",
        "Completed" => "已完成",
        "Cancelled" => "已取消",
        "Failed" => "失败",
        _ => Status
    };
}
