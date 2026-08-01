using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>按对象类选择 Form，并翻译 Form 下 Field 的标签及图例。</summary>
public sealed class FieldTranslationService : IFieldTranslationService
{
    private const string TaskType = "窗体翻译";
    private const string OutputDir = "Config/FieldTranslations";

    private readonly ArasConnectionService _connectionService;
    private readonly IAiDispatcherService _aiDispatcher;
    private readonly IArasTranslationLogService _translationLogService;
    private readonly IErrorLogService _errorLogService;

    public FieldTranslationService(
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
            await _errorLogService.LogErrorAsync("窗体翻译-获取对象类", ex.Message,
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
            await _errorLogService.LogErrorAsync("窗体翻译-获取窗体", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<ArasFormItem>> GetFormListAsync()
    {
        try
        {
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "Form"),
                    new XAttribute("action", "get"),
                    new XAttribute("select", "id,name,label")));
            var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
            ArasTranslationSupport.ThrowIfError(result, "读取窗体列表失败");

            var forms = new List<ArasFormItem>();
            for (var index = 0; index < result.getItemCount(); index++)
            {
                var form = result.getItemByIndex(index);
                forms.Add(new ArasFormItem
                {
                    Id = form.getID(),
                    Name = form.getProperty("name", string.Empty),
                    Label = form.getProperty("label", string.Empty),
                    IsSelected = true
                });
            }

            return forms.OrderBy(form => form.DisplayName, StringComparer.CurrentCultureIgnoreCase).ToList();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("窗体翻译-获取全部窗体", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<FieldItem>> GetFieldsByFormIdAsync(string formId)
    {
        try
        {
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "Body"),
                    new XAttribute("action", "get"),
                    new XAttribute("select", "id,source_id(name)"),
                    new XElement("source_id", formId),
                    new XElement("Relationships",
                        new XElement("Item",
                            new XAttribute("type", "Field"),
                            new XAttribute("action", "get"),
                            new XAttribute("select", "id,name,label,legend,sort_order")))));
            var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
            ArasTranslationSupport.ThrowIfError(result, "读取窗体字段失败");

            var fields = new List<FieldItem>();
            for (var bodyIndex = 0; bodyIndex < result.getItemCount(); bodyIndex++)
            {
                var body = result.getItemByIndex(bodyIndex);
                var formName = body.getProperty("source_id", "name", string.Empty);
                var relationships = body.getRelationships();
                for (var fieldIndex = 0; fieldIndex < relationships.getItemCount(); fieldIndex++)
                {
                    var field = relationships.getItemByIndex(fieldIndex);
                    fields.Add(new FieldItem
                    {
                        Id = field.getID(),
                        Name = field.getProperty("name", string.Empty),
                        Label = field.getProperty("label", string.Empty),
                        Legend = field.getProperty("legend", string.Empty),
                        FormName = formName,
                        IsSelected = true
                    });
                }
            }

            return fields
                .Where(field => !string.IsNullOrWhiteSpace(field.Id))
                .OrderBy(field => field.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("窗体翻译-获取字段", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public Task<List<FieldItem>> QueryFieldsByAmlAsync(string aml)
        => Task.FromResult(new List<FieldItem>());

    public Task<List<FieldItem>> QueryFieldsBySqlAsync(string sql)
        => Task.FromResult(new List<FieldItem>());

    public Task<TranslationTask> CreateTaskAsync(
        string taskName,
        string queryMode,
        string queryCondition,
        string sourceLanguage,
        string targetLanguages,
        int totalFields)
        => _translationLogService.CreateTaskAsync(
            TaskType, taskName, queryCondition, sourceLanguage, targetLanguages, totalFields);

    public async Task TranslateAsync(
        TranslationTask task,
        List<FieldItem> fields,
        string sourceLanguage,
        string targetLanguages,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var selected = fields.Where(field => field.IsSelected).ToList();
        var targets = ArasTranslationSupport.ParseTargets(targetLanguages);
        var records = new List<TranslationRecord>();
        var completedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var totalOperations = selected.Count * targets.Count;
        var completedOperations = 0;

        if (selected.Count == 0)
            throw new InvalidOperationException("至少需要选择一个窗体字段进行翻译。");

        try
        {
            await _translationLogService.SaveOutcomeAsync(
                task, [], "Translating", 0, $"0/{totalOperations}");
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);

            foreach (var target in targets)
            {
                foreach (var field in selected)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var originalLabel = string.IsNullOrWhiteSpace(field.Label) ? field.Name : field.Label;
                    var translatedLabel = await ArasTranslationSupport.TranslateTextAsync(
                        _aiDispatcher,
                        originalLabel,
                        sourceLanguage,
                        target,
                        $"Aras 窗体 {field.FormName} 的控件标签",
                        cancellationToken);

                    var values = new Dictionary<string, string> { ["label"] = translatedLabel };
                    records.Add(ArasTranslationSupport.CreateRecord(
                        task, field.Id, $"{field.Name}.label", originalLabel, translatedLabel, target));

                    if (!string.IsNullOrWhiteSpace(field.Legend))
                    {
                        var translatedLegend = await ArasTranslationSupport.TranslateTextAsync(
                            _aiDispatcher,
                            field.Legend,
                            sourceLanguage,
                            target,
                            $"Aras 窗体 {field.FormName} 的控件说明",
                            cancellationToken);
                        values["legend"] = translatedLegend;
                        records.Add(ArasTranslationSupport.CreateRecord(
                            task,
                            field.Id,
                            $"{field.Name}.legend",
                            field.Legend,
                            translatedLegend,
                            target));
                    }

                    ArasTranslationSupport.ApplyLocalizedValues(
                        innovator, "Field", field.Id, target, values);
                    field.TranslationPreview = AppendPreview(
                        field.TranslationPreview, target.Name, translatedLabel);
                    completedIds.Add(field.Id);
                    completedOperations++;
                    progress?.Report(CreateProgress(
                        completedOperations, totalOperations, field.Name, target.Name));
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

            await _errorLogService.LogErrorAsync("窗体翻译-执行", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<string> ExportToExcelAsync(TranslationTask task, List<FieldItem> fields)
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
            var worksheet = package.Workbook.Worksheets.Add("窗体翻译");
            worksheet.Cells[1, 1].Value = "字段名称";
            worksheet.Cells[1, 2].Value = "原标签";
            worksheet.Cells[1, 3].Value = "原图例";
            worksheet.Cells[1, 4].Value = "翻译结果";
            worksheet.Cells[1, 5].Value = "窗体";
            for (var index = 0; index < fields.Count; index++)
            {
                worksheet.Cells[index + 2, 1].Value = fields[index].Name;
                worksheet.Cells[index + 2, 2].Value = fields[index].Label;
                worksheet.Cells[index + 2, 3].Value = fields[index].Legend;
                worksheet.Cells[index + 2, 4].Value = fields[index].TranslationPreview;
                worksheet.Cells[index + 2, 5].Value = fields[index].FormName;
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
            await _errorLogService.LogErrorAsync("窗体翻译-导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(
        string? userId = null,
        int page = 1,
        int pageSize = 20)
        => _translationLogService.GetTasksAsync(
            userId, TaskType, null, null, page, pageSize);

    private static TranslationProgressInfo CreateProgress(
        int current,
        int total,
        string itemName,
        string targetLanguage)
        => new()
        {
            Phase = $"正在翻译为{targetLanguage}",
            Current = current,
            PhaseTotal = total,
            OverallCurrent = current,
            OverallTotal = total,
            ItemName = itemName
        };

    private static string AppendPreview(string current, string language, string translated)
        => string.IsNullOrWhiteSpace(current)
            ? $"{language}: {translated}"
            : $"{current}；{language}: {translated}";
}
