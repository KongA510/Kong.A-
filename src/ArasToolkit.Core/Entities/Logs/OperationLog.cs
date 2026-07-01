using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 操作日志实体 — EF Core 映射 operation_log 表
/// </summary>
[Table("operation_log")]
public class OperationLog
{
    /// <summary>自增主键</summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>操作类型：Create / Update / Delete / Import</summary>
    [Column("operation_type")]
    [MaxLength(50)]
    public string OperationType { get; set; } = string.Empty;

    /// <summary>实体类型，如 PersonalTask</summary>
    [Column("entity_type")]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>被操作记录的主键值</summary>
    [Column("entity_id")]
    [MaxLength(50)]
    public string EntityId { get; set; } = string.Empty;

    /// <summary>操作描述</summary>
    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>操作时间</summary>
    [Column("operate_time")]
    public DateTime OperateTime { get; set; } = DateTime.Now;

    /// <summary>操作用户名</summary>
    [Column("user_name")]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>记录创建时间（系统级）</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
