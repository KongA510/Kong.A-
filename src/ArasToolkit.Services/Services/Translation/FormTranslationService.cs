using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>按对象类拉取关联 Form，并翻译 Form 多语言标签。</summary>
public sealed class FormTranslationService : IFormTranslationService
{
    private const string TaskType = "表单翻译";
    private const string OutputDir = "Config/FormTranslations";

    private readonly ArasConnectionService _connectionService;
    private readonly IAiDispatcherService _aiDispatcher;
    private readonly IArasTranslationLogService _translationLogService;
    private readonly IErrorLogService _errorLogService;

    public FormTranslationService(
        ArasConnectionService connectionService,
        IAiDispatcherService aiDispatcher,
        IArasTranslationLogService translationLogService,
        IErrorLogService errorLogService)
    {
        _connectionService = connectionService;
        _aiDispatcher = aiDispatcher;
        _translationLogService = translationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<ItemTypeItem>> GetItemTypeListAsync()
    {
        try
        {
            return ArasTranslationSupport.GetItemTypes(
                ArasTranslationSupport.GetConnectedInnovator(_connectionService));
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("表单翻译-获取对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<ArasFormItem>> GetFormsByItemTypeIdAsync(string itemTypeId)
    {
        try
        {
            return ArasTranslationSupport.GetFormsByItemType(
                ArasTranslationSupport.GetConnectedInnovator(_connectionService),
                itemTypeId);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("表单翻译-获取表单", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public Task<TranslationTask> CreateTaskAsync(
        string taskName,
        string itemTypeId,
        string sourceLanguage,
        string targetLanguages,
        int totalForms)
        => _translationLogService.CreateTaskAsync(
            TaskType, taskName, itemTypeId, sourceLanguage, targetLanguages, totalForms);

    public async Task TranslateAsync(
        TranslationTask task,
        List<ArasFormItem> forms,
        string sourceLanguage,
        string targetLanguages,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var selected = forms.Where(form => form.IsSelected).ToList();
        var targets = ArasTranslationSupport.ParseTargets(targetLanguages);
        var records = new List<TranslationRecord>();
        var completedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var totalOperations = selected.Count * targets.Count;
        var completedOperations = 0;

        if (selected.Count == 0)
            throw new InvalidOperationException("至少需要选择一个表单进行翻译。");

        try
        {
            await _translationLogService.SaveOutcomeAsync(
                task, [], "Translating", 0, $"0/{totalOperations}");
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);

            foreach (var target in targets)
            {
                foreach (var form in selected)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var original = string.IsNullOrWhiteSpace(form.Label) ? form.Name : form.Label;
                    var translated = await ArasTranslationSupport.TranslateTextAsync(
                        _aiDispatcher,
                        original,
                        sourceLanguage,
                        target,
                        $"Aras 对象类 {form.ItemTypeName} 的表单标签",
                        cancellationToken);

                    ArasTranslationSupport.ApplyLocalizedValues(
                        innovator,
                        "Form",
                        form.Id,
                        target,
                        new Dictionary<string, string> { ["label"] = translated });

                    records.Add(ArasTranslationSupport.CreateRecord(
                        task, form.Id, form.Name, original, translated, target));
                    form.TranslationPreview = AppendPreview(
                        form.TranslationPreview, target.Name, translated);
                    completedIds.Add(form.Id);
                    completedOperations++;
                    progress?.Report(new TranslationProgressInfo
                    {
                        Phase = $"正在翻译为{target.Name}",
                        Current = completedOperations,
                        PhaseTotal = totalOperations,
                        OverallCurrent = completedOperations,
                        OverallTotal = totalOperations,
                        ItemName = form.Name
                    });
                }
            }

            await _translationLogService.SaveOutcomeAsync(
                task,
                records,
                "Completed",
                completedIds.Count,
                $"{completedOperations}/{totalOperations}");
        }
        catch (OperationCanceledException)
        {
            await _translationLogService.SaveOutcomeAsync(
                task,
                records,
                "Cancelled",
                completedIds.Count,
                $"{completedOperations}/{totalOperations}");
            throw;
        }
        catch (Exception ex)
        {
            try
            {
                await _translationLogService.SaveOutcomeAsync(
                    task,
                    records,
                    "Failed",
                    completedIds.Count,
                    $"{completedOperations}/{totalOperations}");
            }
            catch
            {
                // 原始异常优先返回。
            }

            await _errorLogService.LogErrorAsync("表单翻译-执行", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<string> ExportToExcelAsync(TranslationTask task, List<ArasFormItem> forms)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var directory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                OutputDir,
                DateTime.Now.ToString("yyyy_M_d"));
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, $"{task.TaskName}_{DateTime.Now:HHmmss}.xlsx");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("表单翻译");
            worksheet.Cells[1, 1].Value = "表单名称";
            worksheet.Cells[1, 2].Value = "原标签";
            worksheet.Cells[1, 3].Value = "翻译结果";
            worksheet.Cells[1, 4].Value = "对象类";
            for (var index = 0; index < forms.Count; index++)
            {
                worksheet.Cells[index + 2, 1].Value = forms[index].Name;
                worksheet.Cells[index + 2, 2].Value = forms[index].Label;
                worksheet.Cells[index + 2, 3].Value = forms[index].TranslationPreview;
                worksheet.Cells[index + 2, 4].Value = forms[index].ItemTypeName;
            }

            worksheet.Cells.AutoFitColumns();
            await package.SaveAsAsync(new FileInfo(filePath));
            await _translationLogService.SetOutputFileAsync(task.Id, filePath);
            await _translationLogService.SaveOutcomeAsync(
                task, [], "Completed", task.TotalFields, "已导出");
            task.OutputFilePath = filePath;
            return filePath;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("表单翻译-导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    private static string AppendPreview(string current, string language, string translated)
        => string.IsNullOrWhiteSpace(current)
            ? $"{language}: {translated}"
            : $"{current}；{language}: {translated}";
}
