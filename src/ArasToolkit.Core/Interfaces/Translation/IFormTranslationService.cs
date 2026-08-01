using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>Aras Form 标签在线翻译。</summary>
public interface IFormTranslationService
{
    Task<List<ItemTypeItem>> GetItemTypeListAsync();
    Task<List<ArasFormItem>> GetFormsByItemTypeIdAsync(string itemTypeId);
    Task<TranslationTask> CreateTaskAsync(
        string taskName,
        string itemTypeId,
        string sourceLanguage,
        string targetLanguages,
        int totalForms);
    Task TranslateAsync(
        TranslationTask task,
        List<ArasFormItem> forms,
        string sourceLanguage,
        string targetLanguages,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);
    Task<string> ExportToExcelAsync(TranslationTask task, List<ArasFormItem> forms);
}
