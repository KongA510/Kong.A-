using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// List导入日志实体 — 映射 list_import_log 表
/// 记录每次 List 配置导入的执行结果
/// </summary>
[Table("list_import_log")]
public class ListImportLog
{
    /// <summary>操作成功</summary>
    public const string StatusSuccess = "Success";

    /// <summary>操作失败</summary>
    public const string StatusFailed = "Failed";

    /// <summary>主键（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>操作用户ID</summary>
    [Column("user_id")]
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>导入执行时间</summary>
    [Column("import_time")]
    [Required]
    public DateTime ImportTime { get; set; } = DateTime.Now;

    /// <summary>导入的 Excel 文件相对路径</summary>
    [Column("import_file")]
    [Required]
    [MaxLength(500)]
    public string ImportFile { get; set; } = string.Empty;

    /// <summary>执行状态: Success / Failed</summary>
    [Column("status")]
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = StatusSuccess;

    /// <summary>错误详情（失败时记录堆栈）</summary>
    [Column("error_log")]
    public string? ErrorLog { get; set; }

    /// <summary>Sheet1（List主档）成功导入条数</summary>
    [Column("sheet1_count")]
    public int Sheet1Count { get; set; }

    /// <summary>Sheet2（菜单内容）成功导入条数</summary>
    [Column("sheet2_count")]
    public int Sheet2Count { get; set; }

    /// <summary>Sheet3（菜单内容过滤）成功导入条数</summary>
    [Column("sheet3_count")]
    public int Sheet3Count { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== UI 展示属性（不映射到数据库） =====

    /// <summary>格式化的导入时间（yyyy-MM-dd HH:mm）</summary>
    [NotMapped]
    public string DisplayImportTime => ImportTime == DateTime.MinValue
        ? ""
        : ImportTime.ToString("yyyy-MM-dd HH:mm");

    /// <summary>格式化的创建时间</summary>
    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm");

    /// <summary>状态中文文本</summary>
    [NotMapped]
    public string StatusText => Status switch
    {
        StatusSuccess => "成功",
        StatusFailed => "失败",
        _ => Status
    };

    /// <summary>汇总文本（List主档X条/菜单内容X条/菜单过滤X条）</summary>
    [NotMapped]
    public string Summary =>
        $"[{StatusText}] List主档{Sheet1Count}条 / 菜单{Sheet2Count}条 / 菜单过滤{Sheet3Count}条";
}
