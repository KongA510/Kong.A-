using System.Text;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 任务负载分析服务 - 读取 PersonalTask 数据，调用 AI 流式分析，保存结果
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
            var systemMessage = "你是一位专业的项目管理分析师。请分析提供的任务数据，输出结构化的中文报告。报告必须包含以下章节：" +
                "## 一、任务排布分析 " +
                "## 二、负载分析（每天标准工时8小时，超过6小时需预警，超过8小时标记超载） " +
                "## 三、压力分析 " +
                "## 四、风险分析 " +
                "## 五、综合建议 " +
                "请逐步分析，用数据支撑结论。输出中文。";

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
                    "删除分析记录: " + record.DisplayDateRange);
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
        sb.AppendLine("请分析以下个人任务负载，时间段: " + start.ToString("yyyy-MM-dd") + " 至 " + end.ToString("yyyy-MM-dd"));
        sb.AppendLine("任务总数: " + tasks.Count);
        sb.AppendLine();
        sb.AppendLine("任务列表（按预期完成日期排序）:");
        sb.AppendLine("|序号|任务名称|项目|状态|开始日期|预期日期|进度|备注|");
        sb.AppendLine("|---|---|---|---|---|---|---|---|");
        for (int i = 0; i < tasks.Count; i++)
        {
            var t = tasks[i];
            var sd = t.StartDate?.ToString("yyyy-MM-dd") ?? "未设置";
            var ed = t.ExpectedDate?.ToString("yyyy-MM-dd") ?? "未设置";
            var rm = t.Remarks ?? "无";
            sb.AppendLine("| " + (i + 1) + " | " + t.TaskName + " | " + t.ProjectName + " | " + t.Status + " | " + sd + " | " + ed + " | " + t.CompletionPercent + "% | " + rm + " |");
        }
        sb.AppendLine();
        sb.AppendLine("分析要求:");
        sb.AppendLine("1. 任务排布分析: 分析任务在时间线上的分布情况，识别任务聚集或空闲期，分析任务依赖和并行性。");
        sb.AppendLine("2. 负载分析: 基于每天8小时标准工时，估算每个任务所需工时，计算每日负载。超过6小时标记预警，超过8小时标记超载。给出每日负载概览。");
        sb.AppendLine("3. 压力分析: 基于任务数量、截止日期紧迫度、完成率等，评估整体压力等级（低/中/高/极高）。识别压力峰值期。");
        sb.AppendLine("4. 风险分析: 识别可能延期的任务、资源冲突、瓶颈点。");
        sb.AppendLine("5. 综合建议: 给出具体的优化建议，包括任务重新排布、优先级调整、资源分配等。");
        sb.AppendLine();
        sb.AppendLine("请逐步分析，每个章节用数据支撑。输出中文。");
        return sb.ToString();
    }

    private async Task SaveRecordAsync(TaskLoadAnalysisRecord record)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Set<TaskLoadAnalysisRecord>().Add(record);
        await db.SaveChangesAsync();
        await _operationLogService.LogAsync("Create", "TaskLoadAnalysisRecord", record.Id,
            "创建分析记录: " + record.DisplayDateRange + ", 任务数: " + record.TaskCount);
    }
}
