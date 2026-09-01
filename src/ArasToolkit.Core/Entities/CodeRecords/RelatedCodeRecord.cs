using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>相关代码主题记录，映射 related_code_record 表。</summary>
[Table("related_code_record")]
public class RelatedCodeRecord
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("title")]
    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(2000)]
    public string? Description { get; set; }

    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    public ICollection<RelatedCodeSegment> Segments { get; set; } = [];

    [NotMapped]
    public string DisplayCreatorOn => CreatorOn.ToString("yyyy-MM-dd HH:mm");
}
