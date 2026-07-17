using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public class ArasFormItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class FieldItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Legend { get; set; } = string.Empty;
    public string FormName { get; set; } = string.Empty;
}

public interface IFieldTranslationService
{
    Task<List<ArasFormItem>> GetFormListAsync();
    Task<List<FieldItem>> GetFieldsByFormIdAsync(string formId);
    Task<List<FieldItem>> QueryFieldsByAmlAsync(string aml);
    Task<List<FieldItem>> QueryFieldsBySqlAsync(string sql);
    Task<TranslationTask> CreateTaskAsync(string taskName, string queryMode, string queryCondition, string sourceLanguage, string targetLanguages, int totalFields);
    Task TranslateAsync(TranslationTask task, List<FieldItem> fields, string sourceLanguage, string targetLanguages, IProgress<TranslationProgressInfo>? progress = null, CancellationToken cancellationToken = default);
    Task<string> ExportToExcelAsync(TranslationTask task, List<FieldItem> fields);
    Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);
}
