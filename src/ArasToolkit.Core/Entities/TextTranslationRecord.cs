using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 文本翻译记录实体 — EF Core 映射 text_translation_record 表
/// </summary>
[Table("text_translation_record")]
public class TextTranslationRecord
{
    /// <summary>唯一标识（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>源文件名（不含路径）</summary>
    [Column("source_file_name")]
    [MaxLength(500)]
    [Required]
    public string SourceFileName { get; set; } = string.Empty;

    /// <summary>输出文件名</summary>
    [Column("output_file_name")]
    [MaxLength(500)]
    public string OutputFileName { get; set; } = string.Empty;

    /// <summary>输出文件完整路径</summary>
    [Column("output_file_path")]
    [MaxLength(1000)]
    public string OutputFilePath { get; set; } = string.Empty;

    /// <summary>翻译模式: "Aras翻译" | "自定义翻译"</summary>
    [Column("template_type")]
    [MaxLength(50)]
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>源语言</summary>
    [Column("source_language")]
    [MaxLength(50)]
    public string? SourceLanguage { get; set; }

    /// <summary>源数据行数</summary>
    [Column("source_row_count")]
    public int SourceRowCount { get; set; }

    /// <summary>翻译批次数量</summary>
    [Column("batch_count")]
    public int BatchCount { get; set; }

    /// <summary>所属用户ID（用于按用户筛选）</summary>
    [Column("user_id")]
    [MaxLength(100)]
    public string? UserId { get; set; }

    /// <summary>使用的 AI 模型配置 ID</summary>
    [Column("ai_model_id")]
    [MaxLength(12)]
    public string? AiModelId { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== UI 属性（不映射） =====

    /// <summary>格式化创建时间</summary>
    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
