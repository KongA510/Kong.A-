using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 应用用户服务实现 — 应用本地登录 + 管理员用户管理
/// </summary>
public class AppUserService : IAppUserService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public AppUserService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _contextFactory = contextFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    /// <summary>
    /// 应用登录：按用户名查找并校验密码（仅允许激活用户）
    /// </summary>
    public async Task<AppUser?> LoginAsync(string username, string password)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Set<AppUser>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            if (!user.IsActive)
                return null;

            string hashedInput = IsMd5Format(password) ? password : password.ToMd5();

            if (user.Password != hashedInput)
                return null;

            return user;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户登录", ex.Message, "P0-致命", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    public async Task<bool> UserExistsAsync(string username)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<AppUser>().AnyAsync(u => u.Username == username);
    }

    /// <summary>
    /// 同步数据库表结构 + 插入默认管理员
    /// </summary>
    public async Task EnsureSchemaAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='app_user')
                BEGIN
                    CREATE TABLE app_user (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        username NVARCHAR(100) NOT NULL,
                        password NVARCHAR(100) NOT NULL,
                        display_name NVARCHAR(100) NULL,
                        role NVARCHAR(50) NOT NULL DEFAULT 'User',
                        is_active BIT NOT NULL DEFAULT 1,
                        is_admin BIT NOT NULL DEFAULT 0,
                        avatar NVARCHAR(500) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='app_user' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE app_user ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='app_user' AND COLUMN_NAME='role')
                BEGIN
                    ALTER TABLE app_user ADD role NVARCHAR(50) NOT NULL DEFAULT 'User';
                END

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='app_user' AND COLUMN_NAME='is_active')
                BEGIN
                    ALTER TABLE app_user ADD is_active BIT NOT NULL DEFAULT 1;
                END

                -- 同步 is_admin 与 role 的一致性
                UPDATE app_user SET role = 'Admin' WHERE is_admin = 1 AND (role IS NULL OR role = 'User');
                UPDATE app_user SET is_admin = 1 WHERE role = 'Admin' AND is_admin = 0;
            ";
            await context.Database.ExecuteSqlRawAsync(sql);

            // 仅在部署方显式提供一次性引导密码时创建管理员，避免内置已知口令。
            var adminExists = await context.Set<AppUser>().AnyAsync(u => u.Username == "admin");
            var bootstrapPassword = Environment.GetEnvironmentVariable(
                "ARAS_TOOLKIT_BOOTSTRAP_ADMIN_PASSWORD");
            if (!adminExists && !string.IsNullOrWhiteSpace(bootstrapPassword))
            {
                context.Set<AppUser>().Add(new AppUser
                {
                    Username = "admin",
                    Password = bootstrapPassword.ToMd5(),
                    DisplayName = "系统管理员",
                    Role = "Admin",
                    IsAdmin = true,
                    IsActive = true,
                    CreatorOn = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AppUserService] Schema同步失败: {ex.Message}");
        }
    }

    // ===== 管理员用户管理 =====

    /// <summary>
    /// 获取所有用户列表（按创建时间倒序）
    /// </summary>
    public async Task<List<AppUser>> GetAllUsersAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<AppUser>()
                .OrderByDescending(u => u.CreatorOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-获取列表", ex.Message, "P0-致命", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 管理员创建用户
    /// </summary>
    public async Task<AppUser> CreateUserAsync(string username, string password, string? displayName, string role)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var exists = await context.Set<AppUser>().AnyAsync(u => u.Username == username);
            if (exists)
                throw new InvalidOperationException($"用户名 '{username}' 已存在");

            var user = new AppUser
            {
                Username = username,
                Password = password.ToMd5(),
                DisplayName = displayName ?? username,
                Role = role,
                IsAdmin = role == "Admin",
                IsActive = true,
                CreatorOn = DateTime.Now
            };

            context.Set<AppUser>().Add(user);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Create", "AppUser", user.Id,
                $"管理员创建用户: {username} (角色: {role})");

            return user;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-创建用户", ex.Message, "P1-普通", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 更新用户信息（显示名、角色）
    /// </summary>
    public async Task UpdateUserAsync(string userId, string? displayName, string role)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Set<AppUser>().FindAsync(userId)
                ?? throw new InvalidOperationException("用户不存在");

            user.DisplayName = displayName;
            user.Role = role;
            user.IsAdmin = role == "Admin";

            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Update", "AppUser", userId,
                $"更新用户信息: {user.Username} (角色: {role})");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-更新用户", ex.Message, "P1-普通", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    public async Task ResetPasswordAsync(string userId, string newPassword)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Set<AppUser>().FindAsync(userId)
                ?? throw new InvalidOperationException("用户不存在");

            user.Password = newPassword.ToMd5();
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Update", "AppUser", userId,
                $"重置用户密码: {user.Username}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-重置密码", ex.Message, "P1-普通", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 设置用户启用/禁用状态
    /// </summary>
    public async Task SetUserActiveAsync(string userId, bool isActive)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Set<AppUser>().FindAsync(userId)
                ?? throw new InvalidOperationException("用户不存在");

            user.IsActive = isActive;
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Update", "AppUser", userId,
                $"{(isActive ? "启用" : "禁用")}用户: {user.Username}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-设置状态", ex.Message, "P1-普通", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task DeleteUserAsync(string userId)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Set<AppUser>().FindAsync(userId)
                ?? throw new InvalidOperationException("用户不存在");

            context.Set<AppUser>().Remove(user);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Delete", "AppUser", userId,
                $"删除用户: {user.Username}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("用户管理-删除用户", ex.Message, "P1-普通", ex.StackTrace);
            throw;
        }
    }

    /// <summary>
    /// 判断字符串是否已经是 32 位小写十六进制（MD5 格式）
    /// </summary>
    private static bool IsMd5Format(string s)
    {
        if (s == null || s.Length != 32) return false;
        foreach (char c in s)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                return false;
        }
        return true;
    }
}
