using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 数据库导出日志实体 — 映射 database_export_log 表
/// 敏感操作：记录每次数据库查询导出的详细信息
/// </summary>
[Table("database_export_log")]
public class DatabaseExportLog
{
    [Key] [Column("id")] [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("connection_string")] [MaxLength(1000)]
    public string? ConnectionString { get; set; }

    [Column("sql_query")] [Required]
    public string SqlQuery { get; set; } = string.Empty;

    [Column("export_mode")] [MaxLength(20)] [Required]
    public string ExportMode { get; set; } = "一次导出";

    [Column("batch_size")]
    public int BatchSize { get; set; } = 500;

    [Column("total_rows")]
    public int TotalRows { get; set; }

    [Column("export_time")] [Required]
    public DateTime ExportTime { get; set; } = DateTime.Now;

    [Column("file_path")] [MaxLength(1000)]
    public string? FilePath { get; set; }

    [Column("file_index")]
    public int FileIndex { get; set; } = 1;

    [Column("file_count")]
    public int FileCount { get; set; } = 1;

    [Column("status")] [MaxLength(20)] [Required]
    public string Status { get; set; } = StatusSuccess;

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("user_id")] [MaxLength(100)] [Required]
    public string UserId { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    public const string StatusSuccess = "Success";
    public const string StatusFailed = "Failed";
    public const string ModeSingle = "一次导出";
    public const string ModeBatch = "多次导出";

    [NotMapped] public string StatusText => Status == StatusSuccess ? "成功" : "失败";
    [NotMapped] public string DisplayExportTime => ExportTime.ToString("yyyy-MM-dd HH:mm:ss");
    [NotMapped] public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
    [NotMapped] public string Summary => $"导出 {TotalRows} 行 ({ExportMode})";

    /// <summary>文件名（不含路径）</summary>
    [NotMapped]
    public string FileName => string.IsNullOrEmpty(FilePath)
        ? ""
        : System.IO.Path.GetFileName(FilePath);

    /// <summary>文件所在目录</summary>
    [NotMapped]
    public string FileDirectory => string.IsNullOrEmpty(FilePath)
        ? ""
        : System.IO.Path.GetDirectoryName(FilePath) ?? "";
}
