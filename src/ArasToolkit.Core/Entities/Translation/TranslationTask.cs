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

    [Column("query_mode")]
    [MaxLength(50)]
    public string QueryMode { get; set; } = string.Empty;

    [Column("query_condition")]
    [MaxLength(1000)]
    public string? QueryCondition { get; set; }

    [Column("source_language")]
    [MaxLength(50)]
    public string? SourceLanguage { get; set; }

    [Column("target_languages")]
    [MaxLength(200)]
    public string? TargetLanguages { get; set; }

    [Column("total_fields")]
    public int TotalFields { get; set; }

    [Column("translated_fields")]
    public int TranslatedFields { get; set; }

    [Column("progress_text")]
    [MaxLength(100)]
    public string? ProgressText { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [Column("output_file_path")]
    [MaxLength(1000)]
    public string? OutputFilePath { get; set; }

    [Column("user_id")]
    [MaxLength(100)]
    public string? UserId { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
