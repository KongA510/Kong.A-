using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 应用用户服务实现 — 应用本地登录/注册
/// </summary>
public class AppUserService : IAppUserService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;

    public AppUserService(IDbContextFactory<ArasToolkitDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// 应用登录：按用户名查找并校验密码
    /// </summary>
    public async Task<AppUser?> LoginAsync(string username, string password)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Set<AppUser>()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || user.Password != password)
            return null;

        return user;
    }

    /// <summary>
    /// 应用注册：创建新用户（默认 is_admin=0）
    /// </summary>
    public async Task<AppUser> RegisterAsync(string username, string password, string? displayName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = new AppUser
        {
            Username = username,
            Password = password,
            DisplayName = displayName ?? username,
            IsAdmin = false,
            CreatorOn = DateTime.Now
        };

        context.Set<AppUser>().Add(user);
        await context.SaveChangesAsync();

        return user;
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

            // 创建 app_user 表
            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='app_user')
                BEGIN
                    CREATE TABLE app_user (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        username NVARCHAR(100) NOT NULL,
                        password NVARCHAR(100) NOT NULL,
                        display_name NVARCHAR(100) NULL,
                        is_admin BIT NOT NULL DEFAULT 0,
                        avatar NVARCHAR(500) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='app_user' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE app_user ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END
            ";
            await context.Database.ExecuteSqlRawAsync(sql);

            // 插入默认管理员（如果不存在）
            var adminExists = await context.Set<AppUser>().AnyAsync(u => u.Username == "admin");
            if (!adminExists)
            {
                context.Set<AppUser>().Add(new AppUser
                {
                    Username = "admin",
                    Password = "admin",
                    DisplayName = "系统管理员",
                    IsAdmin = true,
                    CreatorOn = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 插入管理员 xinke.wang（如果不存在）
            var xinkeExists = await context.Set<AppUser>().AnyAsync(u => u.Username == "xinke.wang");
            if (!xinkeExists)
            {
                context.Set<AppUser>().Add(new AppUser
                {
                    Username = "xinke.wang",
                    Password = "xwxy51020",
                    DisplayName = "王新轲",
                    IsAdmin = true,
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
}
