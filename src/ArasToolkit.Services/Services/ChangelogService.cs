using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 更新日志服务实现 — EF Core 数据库持久化
/// </summary>
public class ChangelogService : IChangelogService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public ChangelogService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _contextFactory = contextFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;

        // 首次运行时种子默认数据
        _ = SeedDefaultEntriesAsync();
    }

    public async Task<List<Changelog>> GetAllEntriesAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Changelogs
                .OrderByDescending(e => e.CreatorOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Changelog-查询", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            return [];
        }
    }

    public async Task<List<Changelog>> GetEntriesByVersionAsync(string version)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Changelogs
                .Where(e => e.Version == version)
                .OrderByDescending(e => e.CreatorOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Changelog-按版本查询", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return [];
        }
    }

    public async Task AddEntryAsync(Changelog entry)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Changelogs.Add(entry);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Create", "Changelog", entry.Id.ToString(),
                $"写入更新日志 v{entry.Version}: {entry.Description}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Changelog-新增", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public string GetCurrentVersion()
    {
        try
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Changelogs
                .OrderByDescending(e => e.CreatorOn)
                .Select(e => e.Version)
                .FirstOrDefault() ?? "1.0.0";
        }
        catch
        {
            return "1.0.0";
        }
    }

    /// <summary>
    /// 种子默认更新日志数据（仅在表为空时执行）
    /// </summary>
    private async Task SeedDefaultEntriesAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            if (await context.Changelogs.AnyAsync())
                return;

            var defaults = GetDefaultEntries();
            context.Changelogs.AddRange(defaults);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // 种子失败不影响启动，静默处理（可能表尚未创建）
            System.Diagnostics.Debug.WriteLine($"[Changelog] 种子数据失败: {ex.Message}");
        }
    }

    private static List<Changelog> GetDefaultEntries()
    {
        return new List<Changelog>
        {
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "Aras开发工具箱初始版本发布",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "仪表盘功能：系统概览、连接状态显示、快速操作入口",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "登录功能：Aras Innovator 登录验证，支持记住密码（JSON持久化）",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "Excel导入：支持读取.xlsx/.xls文件，可按Sheet切换显示数据",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "侧边导航栏：左侧菜单点击切换功能页面，支持二级菜单展开",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "待办项目管理：增删改查、分页筛选、Excel导入导出、到期提醒",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "新增",
                Description = "占位功能规划：字段翻译、表单翻译、窗体翻译、窗体配置、对象类配置、属性配置、List配置、权限配置",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2026, 6, 14),
                Type = "优化",
                Description = "采用三层架构（Core/Services/App）+ MVVM模式，高内聚低耦合设计",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.1",
                ReleaseDate = new DateTime(2026, 6, 16),
                Type = "新增",
                Description = "系统日志分类功能：整合更新日志、错误日志、敏感操作日志为统一入口，迁移至数据库存储",
                Author = "开发团队"
            },
            new()
            {
                Version = "1.0.1",
                ReleaseDate = new DateTime(2026, 6, 16),
                Type = "修复",
                Description = "修复待办项目加载失败（description列NULL导致EF Core报Data is Null）",
                Author = "开发团队"
            },
        };
    }
}
