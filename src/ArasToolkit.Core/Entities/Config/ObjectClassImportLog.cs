using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 对象类配置汇入记录 — EF Core 映射 object_class_import_log 表
/// 敏感操作：记录每次对象类/关系类批量导入的详细信息
/// </summary>
[Table("object_class_import_log")]
public class ObjectClassImportLog
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

    /// <summary>汇入时间</summary>
    [Column("import_time")]
    [Required]
    public DateTime ImportTime { get; set; } = DateTime.Now;

    /// <summary>汇入文件名（相对路径，如 Config/ObjectClassImports/xxx.xlsx）</summary>
    [Column("import_file")]
    [MaxLength(500)]
    [Required]
    public string ImportFile { get; set; } = string.Empty;

    /// <summary>汇入状态：Success / Failed</summary>
    [Column("status")]
    [MaxLength(20)]
    [Required]
    public string Status { get; set; } = "Success";

    /// <summary>失败日志（错误堆栈信息）</summary>
    [Column("error_log")]
    public string? ErrorLog { get; set; }

    /// <summary>Sheet1（对象类新增）导入条数</summary>
    [Column("sheet1_count")]
    public int Sheet1Count { get; set; }

    /// <summary>Sheet2（关系类新增）导入条数</summary>
    [Column("sheet2_count")]
    public int Sheet2Count { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== 常量 =====
    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";

    // ===== UI 属性（不映射） =====

    /// <summary>格式化汇入时间</summary>
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
    public string Summary => $"对象类 {Sheet1Count} 条 / 关系类 {Sheet2Count} 条";
}
