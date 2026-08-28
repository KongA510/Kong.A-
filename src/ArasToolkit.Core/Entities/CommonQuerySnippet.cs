using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 常用 SQL、AML 与 XML 片段，映射 common_query_snippet 表。
/// </summary>
[Table("common_query_snippet")]
public class CommonQuerySnippet
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("title")]
    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Column("content_type")]
    [MaxLength(10)]
    [Required]
    public string ContentType { get; set; } = "SQL";

    [Column("content")]
    [Required]
    public string Content { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatorOn => CreatorOn.ToString("yyyy-MM-dd HH:mm");
}
