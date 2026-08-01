using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>字段、表单、窗体翻译共用日志服务。</summary>
public sealed class ArasTranslationLogService : IArasTranslationLogService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;
    private readonly IAiDispatcherService _aiDispatcherService;

    public ArasTranslationLogService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService,
        IAiDispatcherService aiDispatcherService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        _aiDispatcherService = aiDispatcherService;
    }

    public async Task<TranslationTask> CreateTaskAsync(
        string taskType,
        string taskName,
        string scopeId,
        string sourceLanguage,
        string targetLanguages,
        int totalItems)
    {
        try
        {
            var task = new TranslationTask
            {
                TaskName = taskName,
                TaskType = taskType,
                QueryMode = "Aras连接",
                QueryCondition = EncodeScope(CurrentUserContext.CurrentUserId, scopeId),
                SourceLanguage = sourceLanguage,
                TargetLanguages = targetLanguages,
                TotalFields = totalItems,
                TranslatedFields = 0,
                ProgressText = $"0/{totalItems}",
                Status = "Pending",
                AiModelId = (await _aiDispatcherService.GetCurrentModelAsync())?.Id,
                CreatorOn = DateTime.Now
            };

            await using var db = await _dbFactory.CreateDbContextAsync();
            db.TranslationTasks.Add(task);
            await db.SaveChangesAsync();

            await SafeOperationLogAsync("Create", task.Id, $"创建{taskType}任务: {taskName}");
            return task;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Aras翻译日志-创建任务", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task SaveOutcomeAsync(
        TranslationTask task,
        IReadOnlyCollection<TranslationRecord> records,
        string status,
        int translatedItems,
        string progressText)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var storedTask = await db.TranslationTasks.FindAsync(task.Id)
                ?? throw new InvalidOperationException($"翻译任务不存在: {task.Id}");

            storedTask.Status = status;
            storedTask.TranslatedFields = translatedItems;
            storedTask.ProgressText = progressText;
            storedTask.CompletedOn = status is "Completed" or "Cancelled" or "Failed"
                ? DateTime.Now
                : null;

            if (records.Count > 0)
                db.TranslationRecords.AddRange(records);

            await db.SaveChangesAsync();

            task.Status = status;
            task.TranslatedFields = translatedItems;
            task.ProgressText = progressText;
            task.CompletedOn = storedTask.CompletedOn;
            await SafeOperationLogAsync("Update", task.Id,
                $"{task.TaskType}任务{status}: {task.TaskName}，完成 {translatedItems}/{task.TotalFields}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Aras翻译日志-保存结果", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task SetOutputFileAsync(string taskId, string outputFilePath)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var task = await db.TranslationTasks.FindAsync(taskId)
                ?? throw new InvalidOperationException($"翻译任务不存在: {taskId}");
            task.OutputFilePath = outputFilePath;
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Aras翻译日志-更新输出文件", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<(List<TranslationTask> Items, int TotalCount)> GetTasksAsync(
        string? userId,
        string? taskType,
        string? status,
        string? searchText,
        int page,
        int pageSize)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var query = db.TranslationTasks.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userPrefix = $"user:{userId}|scope:";
                query = query.Where(task => task.QueryCondition != null &&
                                            task.QueryCondition.StartsWith(userPrefix));
            }
            if (!string.IsNullOrWhiteSpace(taskType) && taskType != "全部类型")
                query = query.Where(task => task.TaskType == taskType);
            if (!string.IsNullOrWhiteSpace(status) && status != "全部状态")
                query = query.Where(task => task.Status == status);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var keyword = searchText.Trim();
                query = query.Where(task => task.TaskName.Contains(keyword) ||
                                            (task.QueryCondition != null && task.QueryCondition.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(task => task.CreatorOn)
                .Skip((Math.Max(page, 1) - 1) * Math.Max(pageSize, 1))
                .Take(Math.Max(pageSize, 1))
                .ToListAsync();
            return (items, totalCount);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Aras翻译日志-查询任务", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<TranslationRecord>> GetRecordsAsync(string taskId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.TranslationRecords
                .AsNoTracking()
                .Where(record => record.TaskId == taskId)
                .OrderBy(record => record.CreatorOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Aras翻译日志-查询明细", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    private async Task SafeOperationLogAsync(string operationType, string entityId, string description)
    {
        try
        {
            await _operationLogService.LogAsync(operationType, "TranslationTask", entityId, description);
        }
        catch
        {
            // 操作日志失败不能影响翻译主流程。
        }
    }

    private static string EncodeScope(string userId, string scopeId)
        => $"user:{userId}|scope:{scopeId}";
}
