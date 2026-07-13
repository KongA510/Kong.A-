using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 数据库导出配置实体 — 映射 database_export_config 表
/// 支持多个数据库连接配置，可启用/切换（参照 AiModelConfig 模式）
/// </summary>
[Table("database_export_config")]
public class DatabaseExportConfig
{
    [Key] [Column("id")] [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("user_id")] [MaxLength(100)] [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>配置名称（如 "生产环境"、"测试库"）</summary>
    [Column("config_name")] [MaxLength(200)] [Required]
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>数据库连接字符串</summary>
    [Column("connection_string")] [MaxLength(1000)] [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>备注说明</summary>
    [Column("remark")] [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>是否启用（同一用户只能有一个启用）</summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped] public string StatusText => IsEnabled ? "已启用" : "未启用";
}
