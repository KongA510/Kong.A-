using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>字段、表单、窗体翻译共用的任务及多语言明细日志。</summary>
public interface IArasTranslationLogService
{
    Task<TranslationTask> CreateTaskAsync(
        string taskType,
        string taskName,
        string scopeId,
        string sourceLanguage,
        string targetLanguages,
        int totalItems);

    Task SaveOutcomeAsync(
        TranslationTask task,
        IReadOnlyCollection<TranslationRecord> records,
        string status,
        int translatedItems,
        string progressText);

    Task SetOutputFileAsync(string taskId, string outputFilePath);

    Task<(List<TranslationTask> Items, int TotalCount)> GetTasksAsync(
        string? userId,
        string? taskType,
        string? status,
        string? searchText,
        int page,
        int pageSize);

    Task<List<TranslationRecord>> GetRecordsAsync(string taskId);
}
