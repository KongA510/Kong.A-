using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// XML 格式化与比对工具共用的已保存 XML，映射 saved_xml 表。
/// </summary>
[Table("saved_xml")]
public class SavedXml
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("name")]
    [MaxLength(200)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Column("xml_content")]
    [Required]
    public string XmlContent { get; set; } = string.Empty;

    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatorOn => CreatorOn.ToString("yyyy-MM-dd HH:mm");
}
