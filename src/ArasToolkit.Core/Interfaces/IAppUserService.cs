using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 应用用户服务接口 — 应用本地登录 + 管理员用户管理
/// </summary>
public interface IAppUserService
{
    /// <summary>
    /// 应用登录：校验账号密码（仅允许激活用户）
    /// </summary>
    Task<AppUser?> LoginAsync(string username, string password);

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> UserExistsAsync(string username);

    /// <summary>
    /// 同步数据库表结构 + 插入默认管理员
    /// </summary>
    Task EnsureSchemaAsync();

    // ===== 管理员用户管理 =====

    /// <summary>
    /// 获取所有用户列表（按创建时间倒序）
    /// </summary>
    Task<List<AppUser>> GetAllUsersAsync();

    /// <summary>
    /// 管理员创建用户
    /// </summary>
    Task<AppUser> CreateUserAsync(string username, string password, string? displayName, string role);

    /// <summary>
    /// 更新用户信息（显示名、角色）
    /// </summary>
    Task UpdateUserAsync(string userId, string? displayName, string role);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    Task ResetPasswordAsync(string userId, string newPassword);

    /// <summary>
    /// 设置用户启用/禁用状态
    /// </summary>
    Task SetUserActiveAsync(string userId, bool isActive);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task DeleteUserAsync(string userId);
}
