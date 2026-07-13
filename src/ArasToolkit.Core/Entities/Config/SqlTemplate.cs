using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// SQL模板实体 — 映射 sql_template 表
/// 存储用户常用的SQL查询模板，方便导出时复用
/// </summary>
[Table("sql_template")]
public class SqlTemplate
{
    [Key] [Column("id")] [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("template_name")] [MaxLength(200)] [Required]
    public string TemplateName { get; set; } = string.Empty;

    [Column("sql_content")] [Required]
    public string SqlContent { get; set; } = string.Empty;

    [Column("description")] [MaxLength(500)]
    public string? Description { get; set; }

    [Column("user_id")] [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped] public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
