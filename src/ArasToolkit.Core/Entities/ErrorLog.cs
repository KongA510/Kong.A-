using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 错误日志实体 — EF Core 映射 error_log 表
/// </summary>
[Table("error_log")]
public class ErrorLog
{
    // ===== 错误级别常量 =====
    /// <summary>P0 — 致命错误：闪退、数据库/服务连接失败、无法恢复的崩溃</summary>
    public const string LevelP0 = "P0-致命";
    /// <summary>P1 — 普通错误：操作失败、导出/导入失败、业务逻辑异常</summary>
    public const string LevelP1 = "P1-普通";

    /// <summary>日志ID（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>调用功能模块名称</summary>
    [Column("function_name")]
    [MaxLength(200)]
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>错误信息</summary>
    [Column("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>记录日期</summary>
    [Column("record_date")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>错误级别: P0-致命 / P1-普通</summary>
    [Column("level")]
    [MaxLength(20)]
    public string Level { get; set; } = LevelP1;

    /// <summary>堆栈跟踪</summary>
    [Column("stack_trace")]
    public string? StackTrace { get; set; }

    /// <summary>记录创建时间（系统级）</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== UI / 计算属性（不映射到数据库） =====

    /// <summary>格式化日期显示</summary>
    [NotMapped]
    public string DisplayDate => RecordDate.ToString("yyyy-MM-dd HH:mm:ss");
}
