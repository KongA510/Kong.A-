using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 应用用户服务接口 — 应用本地登录/注册
/// </summary>
public interface IAppUserService
{
    /// <summary>
    /// 应用登录：校验账号密码
    /// </summary>
    Task<AppUser?> LoginAsync(string username, string password);

    /// <summary>
    /// 应用注册：创建新用户
    /// </summary>
    Task<AppUser> RegisterAsync(string username, string password, string? displayName);

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> UserExistsAsync(string username);

    /// <summary>
    /// 同步数据库表结构 + 插入默认管理员
    /// </summary>
    Task EnsureSchemaAsync();
}
