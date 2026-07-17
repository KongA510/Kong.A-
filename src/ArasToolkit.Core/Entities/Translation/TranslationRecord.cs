using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

[Table("translation_record")]
public class TranslationRecord
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("task_id")]
    [MaxLength(12)]
    [Required]
    public string TaskId { get; set; } = string.Empty;

    [Column("field_id")]
    [MaxLength(50)]
    public string? FieldId { get; set; }

    [Column("field_name")]
    [MaxLength(200)]
    public string? FieldName { get; set; }

    [Column("original_label")]
    [MaxLength(500)]
    public string? OriginalLabel { get; set; }

    [Column("translated_label")]
    [MaxLength(500)]
    public string? TranslatedLabel { get; set; }

    [Column("target_language")]
    [MaxLength(50)]
    public string? TargetLanguage { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
