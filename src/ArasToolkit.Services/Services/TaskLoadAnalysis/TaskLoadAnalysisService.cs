using System.Text;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Task load analysis service - reads PersonalTask data, calls AI streaming analysis, saves results
/// </summary>
public class TaskLoadAnalysisService : ITaskLoadAnalysisService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IAiDispatcherService _aiDispatcher;
    private readonly IErrorLogService _errorLogService;
    private readonly IOperationLogService _operationLogService;

    public TaskLoadAnalysisService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IAiDispatcherService aiDispatcher,
        IErrorLogService errorLogService,
        IOperationLogService operationLogService)
    {
        _dbFactory = dbFactory;
        _aiDispatcher = aiDispatcher;
        _errorLogService = errorLogService;
        _operationLogService = operationLogService;
    }

    public async Task AnalyzeStreamAsync(
        DateTime startDate,
        DateTime endDate,
        Action<string> onChunk,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tasks = await GetTasksInRangeAsync(startDate, endDate);
            var prompt = BuildAnalysisPrompt(tasks, startDate, endDate);
            var model = await _aiDispatcher.GetCurrentModelAsync();
            var modelName = model?.ModelName ?? "unknown";

            var fullResult = new StringBuilder();
            var systemMessage = "You are a professional project management analyst. Analyze the provided task data and output a structured report in Chinese. The report must include these sections with headers: " +
                "## 一、任务排布分析 " +
                "## 二、负载分析（每天标准工时8小时，超过6小时需预警，超过8小时标记超载） " +
                "## 三、压力分析 " +
                "## 四、风险分析 " +
                "## 五、综合建议 " +
                "Analyze step by step with data support. Output in Chinese.";

            await _aiDispatcher.ChatStreamAsync(
                prompt,
                chunk =>
                {
                    fullResult.Append(chunk);
                    onChunk(chunk);
                },
                new ChatOptions { SystemMessage = systemMessage, Temperature = 0.7 },
                cancellationToken);

            var record = new TaskLoadAnalysisRecord
            {
                StartDate = startDate,
                EndDate = endDate,
                TaskCount = tasks.Count,
                AnalysisResult = fullResult.ToString(),
                ModelName = modelName,
                UserId = CurrentUserContext.Current?.Username,
                CreatorOn = DateTime.Now
            };
            await SaveRecordAsync(record);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("TaskLoadAnalysis-Analyze", ex.Message, ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<(List<TaskLoadAnalysisRecord> Items, int TotalCount)> GetPagedRecordsAsync(int page, int pageSize)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var query = db.Set<TaskLoadAnalysisRecord>().OrderByDescending(x => x.CreatorOn);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("TaskLoadAnalysis-QueryRecords", ex.Message, ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task DeleteRecordAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var record = await db.Set<TaskLoadAnalysisRecord>().FindAsync(id);
            if (record != null)
            {
                db.Set<TaskLoadAnalysisRecord>().Remove(record);
                await db.SaveChangesAsync();
                await _operationLogService.LogAsync("Delete", "TaskLoadAnalysisRecord", id,
                    "Delete analysis record: " + record.DisplayDateRange);
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("TaskLoadAnalysis-DeleteRecord", ex.Message, ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<TaskLoadAnalysisRecord?> GetRecordByIdAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Set<TaskLoadAnalysisRecord>().FindAsync(id);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("TaskLoadAnalysis-GetRecord", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            return null;
        }
    }

    private async Task<List<PersonalTask>> GetTasksInRangeAsync(DateTime start, DateTime end)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var rangeStart = start.Date;
        var rangeEnd = end.Date.AddDays(1);
        return await db.PersonalTasks
            .Where(t =>
                (t.StartDate.HasValue && t.StartDate.Value >= rangeStart && t.StartDate.Value < rangeEnd) ||
                (t.ExpectedDate.HasValue && t.ExpectedDate.Value >= rangeStart && t.ExpectedDate.Value < rangeEnd) ||
                (t.CreatedDate >= rangeStart && t.CreatedDate < rangeEnd))
            .OrderBy(t => t.ExpectedDate ?? t.CreatedDate)
            .ToListAsync();
    }

    private static string BuildAnalysisPrompt(List<PersonalTask> tasks, DateTime start, DateTime end)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Please analyze the following personal task load for the period: " + start.ToString("yyyy-MM-dd") + " to " + end.ToString("yyyy-MM-dd"));
        sb.AppendLine("Total tasks: " + tasks.Count);
        sb.AppendLine();
        sb.AppendLine("Task list (sorted by expected completion date):");
        sb.AppendLine("| No | TaskName | Project | Status | StartDate | ExpectedDate | Progress | Remarks |");
        sb.AppendLine("|---|---|---|---|---|---|---|---|");
        for (int i = 0; i < tasks.Count; i++)
        {
            var t = tasks[i];
            var sd = t.StartDate?.ToString("yyyy-MM-dd") ?? "NotSet";
            var ed = t.ExpectedDate?.ToString("yyyy-MM-dd") ?? "NotSet";
            var rm = t.Remarks ?? "None";
            sb.AppendLine("| " + (i + 1) + " | " + t.TaskName + " | " + t.ProjectName + " | " + t.Status + " | " + sd + " | " + ed + " | " + t.CompletionPercent + "% | " + rm + " |");
        }
        sb.AppendLine();
        sb.AppendLine("Analysis requirements:");
        sb.AppendLine("1. Task arrangement: analyze distribution on timeline, identify clustering or gaps, dependencies and parallelism.");
        sb.AppendLine("2. Load analysis: based on 8h/day standard, estimate per-task hours, calculate daily load. Mark >6h as warning, >8h as overload. Give daily load overview.");
        sb.AppendLine("3. Stress analysis: evaluate overall stress level (Low/Medium/High/Extreme) based on task count, deadline urgency, completion rate. Identify peak stress periods.");
        sb.AppendLine("4. Risk analysis: identify potential delays, resource conflicts, bottlenecks.");
        sb.AppendLine("5. Recommendations: specific optimization suggestions including task rescheduling, priority adjustment, resource allocation.");
        sb.AppendLine();
        sb.AppendLine("Please analyze step by step with data support for each section. Output in Chinese.");
        return sb.ToString();
    }

    private async Task SaveRecordAsync(TaskLoadAnalysisRecord record)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Set<TaskLoadAnalysisRecord>().Add(record);
        await db.SaveChangesAsync();
        await _operationLogService.LogAsync("Create", "TaskLoadAnalysisRecord", record.Id,
            "Create analysis record: " + record.DisplayDateRange + ", Tasks: " + record.TaskCount);
    }
}
