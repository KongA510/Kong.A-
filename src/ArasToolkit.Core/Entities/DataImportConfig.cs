using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 数据导入配置 — AML模板配置存储
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

    [Column("sheet_name")]
    [MaxLength(200)]
    public string? SheetName { get; set; }

    [Column("start_row")]
    public int StartRow { get; set; } = 2;

    [Column("end_row")]
    public int EndRow { get; set; } = -1;

    [Column("start_col")]
    public int StartCol { get; set; } = 1;

    [Column("end_col")]
    public int EndCol { get; set; } = -1;

    [Column("user_name")]
    [MaxLength(100)]
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
