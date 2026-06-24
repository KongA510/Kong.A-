using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// 应用用户表 — 存储应用本地登录账号
/// </summary>
[Table("app_user")]
public class AppUser
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("username")]
    [MaxLength(100)]
    [Required]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    [MaxLength(100)]
    [Required]
    public string Password { get; set; } = string.Empty;

    [Column("display_name")]
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    [Column("is_admin")]
    public bool IsAdmin { get; set; }

    [Column("avatar")]
    [MaxLength(500)]
    public string? Avatar { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
