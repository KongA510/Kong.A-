using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// AML模板配置存储 — 数据库映射实体
/// </summary>
[Table("data_import_config")]
public class DataImportConfig
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("config_name")]
    [MaxLength(200)]
    [Required]
    public string ConfigName { get; set; } = string.Empty;

    [Column("aml_content")]
    [Required]
    public string AmlContent { get; set; } = string.Empty;

    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
