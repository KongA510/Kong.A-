using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 生命周期配置导入日志实体 — 映射 lifecycle_import_log 表
/// 敏感操作：记录每次生命周期配置批量导入的详细信息
/// </summary>
[Table("lifecycle_import_log")]
public class LifecycleImportLog
{
    /// <summary>唯一标识（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>操作用户ID</summary>
    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>导入时间</summary>
    [Column("import_time")]
    [Required]
    public DateTime ImportTime { get; set; } = DateTime.Now;

    /// <summary>导入文件名（相对路径）</summary>
    [Column("import_file")]
    [MaxLength(500)]
    [Required]
    public string ImportFile { get; set; } = string.Empty;

    /// <summary>导入状态：Success / Failed</summary>
    [Column("status")]
    [MaxLength(20)]
    [Required]
    public string Status { get; set; } = "Success";

    /// <summary>失败日志（错误堆栈信息）</summary>
    [Column("error_log")]
    public string? ErrorLog { get; set; }

    /// <summary>生命周期配置成功导入条数</summary>
    [Column("sheet1_count")]
    public int Sheet1Count { get; set; }

    /// <summary>生命周期状态成功导入条数</summary>
    [Column("sheet2_count")]
    public int Sheet2Count { get; set; }

    /// <summary>转变成功导入条数</summary>
    [Column("sheet3_count")]
    public int Sheet3Count { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== 常量 =====
    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";

    // ===== UI 属性（不映射） =====

    /// <summary>格式化导入时间</summary>
    [NotMapped]
    public string DisplayImportTime => ImportTime.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>格式化创建时间</summary>
    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>状态标签文本</summary>
    [NotMapped]
    public string StatusText => Status == StatusSuccess ? "成功" : "失败";

    /// <summary>统计摘要</summary>
    [NotMapped]
    public string Summary => $"生命周期 {Sheet1Count} 状态 {Sheet2Count} 转变 {Sheet3Count}";
}
