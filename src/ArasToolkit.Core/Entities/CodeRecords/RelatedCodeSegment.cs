using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>相关代码主题下的有序代码段，映射 related_code_segment 表。</summary>
[Table("related_code_segment")]
public class RelatedCodeSegment
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("record_id")]
    [MaxLength(12)]
    [Required]
    public string RecordId { get; set; } = string.Empty;

    [Column("segment_name")]
    [MaxLength(200)]
    [Required]
    public string SegmentName { get; set; } = string.Empty;

    [Column("code_type")]
    [MaxLength(20)]
    [Required]
    public string CodeType { get; set; } = "C#";

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("code_content")]
    [Required]
    public string CodeContent { get; set; } = string.Empty;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    public RelatedCodeRecord? Record { get; set; }

    [NotMapped]
    public string DisplayCreatorOn => CreatorOn.ToString("yyyy-MM-dd HH:mm");

    [NotMapped]
    public string DisplayDescription => string.IsNullOrWhiteSpace(Description)
        ? "未填写用途说明"
        : Description;
}
