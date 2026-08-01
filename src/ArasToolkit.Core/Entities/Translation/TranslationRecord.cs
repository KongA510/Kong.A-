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

    [Column("item_type")]
    [MaxLength(100)]
    public string? ItemType { get; set; }

    [Column("item_id")]
    [MaxLength(50)]
    public string? FieldId { get; set; }

    [Column("item_name")]
    [MaxLength(200)]
    public string? ItemName { get; set; }

    [Column("field_name")]
    [MaxLength(200)]
    public string? FieldName { get; set; }

    [Column("original_text")]
    public string? OriginalLabel { get; set; }

    [Column("translated_text")]
    public string? TranslatedLabel { get; set; }

    [Column("language")]
    [MaxLength(50)]
    public string? TargetLanguage { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Completed";

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");

    [NotMapped]
    public string TranslationDisplay => $"{OriginalLabel} → {TranslatedLabel}";
}
