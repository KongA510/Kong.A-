using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public class ItemTypeItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class PropertyItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
}

public interface IPropertyTranslationService
{
    Task<List<ItemTypeItem>> GetItemTypeListAsync();
    Task<List<PropertyItem>> GetPropertiesByItemTypeIdAsync(string itemTypeId);
    Task<List<PropertyItem>> QueryPropertiesByAmlAsync(string aml);
    Task<List<PropertyItem>> QueryPropertiesBySqlAsync(string sql);
    Task<TranslationTask> CreateTaskAsync(string taskName, string queryMode, string queryCondition, string sourceLanguage, string targetLanguages, int totalFields);
    Task TranslateAsync(TranslationTask task, List<PropertyItem> properties, string sourceLanguage, string targetLanguages, IProgress<TranslationProgressInfo>? progress = null, CancellationToken cancellationToken = default);
    Task<string> ExportToExcelAsync(TranslationTask task, List<PropertyItem> properties);
    Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);
}
