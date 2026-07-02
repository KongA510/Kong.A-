using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 个人资料库笔记实体 — EF Core 映射 knowledge_entry 表
/// </summary>
[Table("knowledge_entry")]
public class KnowledgeEntry
{
    /// <summary>唯一标识（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>笔记标题</summary>
    [Column("title")]
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>FlowDocument XAML 序列化内容（含base64内嵌图片）</summary>
    [Column("content")]
    public string? Content { get; set; }

    /// <summary>纯文本预览（列表展示用，取前200字）</summary>
    [Column("plain_text_preview")]
    [MaxLength(500)]
    public string? PlainTextPreview { get; set; }

    /// <summary>标签（逗号分隔）</summary>
    [Column("tags")]
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>分类</summary>
    [Column("category")]
    [MaxLength(200)]
    public string? Category { get; set; }

    /// <summary>是否置顶</summary>
    [Column("pinned")]
    public bool Pinned { get; set; }

    /// <summary>创建日期</summary>
    [Column("created_date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>最后修改日期</summary>
    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>记录创建时间（系统级，不可修改）</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    /// <summary>所属用户ID（数据隔离）</summary>
    [Column("user_id")]
    [MaxLength(100)]
    public string? UserId { get; set; }

    // ===== UI / 计算属性（不映射到数据库） =====

    /// <summary>格式化修改日期</summary>
    [NotMapped]
    public string DisplayModifiedDate => ModifiedDate?.ToString("yyyy-MM-dd HH:mm") ?? "";

    /// <summary>格式化创建日期</summary>
    [NotMapped]
    public string DisplayCreatedDate => CreatedDate.ToString("yyyy-MM-dd HH:mm");

    /// <summary>标签转为 #tag1 #tag2 展示</summary>
    [NotMapped]
    public string DisplayTags => string.IsNullOrEmpty(Tags)
        ? ""
        : string.Join(" ", Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => $"#{t.Trim()}"));

    /// <summary>是否有标签</summary>
    [NotMapped]
    public bool HasTags => !string.IsNullOrEmpty(Tags);

    /// <summary>是否为当前选中条目（UI状态）</summary>
    [NotMapped]
    public bool IsSelected { get; set; }
}
