using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// Aras 登录配置实体 — 数据库持久化存储
/// </summary>
[Table("aras_login_config")]
public class ArasLoginConfig
{
    /// <summary>12位短GUID主键</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>Aras 服务器 URL</summary>
    [Column("url")]
    [MaxLength(500)]
    [Required]
    public string Url { get; set; } = string.Empty;

    /// <summary>数据库名称</summary>
    [Column("database_name")]
    [MaxLength(200)]
    [Required]
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>登录用户名</summary>
    [Column("username")]
    [MaxLength(100)]
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>MD5 加密密码（32位小写十六进制）</summary>
    [Column("md5_password")]
    [MaxLength(32)]
    public string Md5Password { get; set; } = string.Empty;

    /// <summary>是否启用（启用后应用启动时自动连接）</summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    /// <summary>所属用户ID</summary>
    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>UI 显示用启用状态文本</summary>
    [NotMapped]
    public string StatusText => IsEnabled ? "已启用" : "未启用";
}
