using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 图表数据服务 — 查询数据库生成统计图表数据
/// </summary>
public class ChartService : IChartService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IErrorLogService _errorLogService;

    public ChartService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _errorLogService = errorLogService;
    }

    /// <summary>
    /// 按任务状态统计（柱状图数据）
    /// </summary>
    public async Task<ChartData> GetTaskStatusChartDataAsync()
    {
        try
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var groups = await context.PersonalTasks
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var statusOrder = new[] { "未开始", "处理中", "延期", "已完成" };
            var statusColors = new Dictionary<string, string>
            {
                ["未开始"] = "#9CA3AF",
                ["处理中"] = "#6366F1",
                ["延期"] = "#EF4444",
                ["已完成"] = "#10B981"
            };

            var dataPoints = statusOrder
                .Where(s => groups.Any(g => g.Status == s))
                .Select(s => new ChartDataPoint
                {
                    Label = s,
                    Value = groups.FirstOrDefault(g => g.Status == s)?.Count ?? 0,
                    Color = statusColors.GetValueOrDefault(s)
                })
                .ToList();

            return new ChartData
            {
                Title = "待办任务状态分布",
                XAxisTitle = "任务状态",
                YAxisTitle = "任务数量",
                DataPoints = dataPoints
            };
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Chart-状态统计", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            return new ChartData { Title = "数据加载失败" };
        }
    }

    /// <summary>
    /// 按项目统计（柱状图数据）
    /// </summary>
    public async Task<ChartData> GetTaskByProjectChartDataAsync()
    {
        try
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var groups = await context.PersonalTasks
                .GroupBy(t => t.ProjectName)
                .Select(g => new { ProjectName = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            var colors = new[] { "#6366F1", "#8B5CF6", "#EC4899", "#F59E0B", "#10B981",
                                 "#3B82F6", "#EF4444", "#06B6D4", "#84CC16", "#F97316" };

            var dataPoints = groups
                .Select((g, i) => new ChartDataPoint
                {
                    Label = string.IsNullOrWhiteSpace(g.ProjectName) ? "未分类" : g.ProjectName,
                    Value = g.Count,
                    Color = colors[i % colors.Length]
                })
                .ToList();

            return new ChartData
            {
                Title = "待办任务项目分布 (Top 10)",
                XAxisTitle = "项目名称",
                YAxisTitle = "任务数量",
                DataPoints = dataPoints
            };
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Chart-项目统计", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            return new ChartData { Title = "数据加载失败" };
        }
    }
}
