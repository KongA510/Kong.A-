using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 更新日志实体 — EF Core 映射 changelog 表
/// </summary>
[Table("changelog")]
public class Changelog
{
    /// <summary>自增主键</summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>版本号</summary>
    [Column("version")]
    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    /// <summary>发布日期</summary>
    [Column("release_date")]
    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    /// <summary>更新类型: 新增/修复/优化/移除</summary>
    [Column("type")]
    [MaxLength(20)]
    public string Type { get; set; } = "新增";

    /// <summary>更新内容描述</summary>
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>更新人</summary>
    [Column("author")]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    /// <summary>记录创建时间（系统级）</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== UI / 计算属性（不映射到数据库） =====

    /// <summary>格式化日期显示（中文格式）</summary>
    [NotMapped]
    public string DisplayDate => ReleaseDate.ToString("yyyy年M月d日");
}
