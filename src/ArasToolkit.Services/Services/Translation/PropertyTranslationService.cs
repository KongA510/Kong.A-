using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>按 ItemType 拉取 Property，并通过当前 AI 配置写回 Aras 多语言标签。</summary>
public sealed class PropertyTranslationService : IPropertyTranslationService
{
    private const string TaskType = "字段翻译";
    private const string OutputDir = "Config/PropertyTranslations";
    private const string DefaultLabelLanguages = "en,zc,zt";

    private readonly ArasConnectionService _connectionService;
    private readonly IAiDispatcherService _aiDispatcher;
    private readonly IArasTranslationLogService _translationLogService;
    private readonly IErrorLogService _errorLogService;

    public PropertyTranslationService(
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
            await _errorLogService.LogErrorAsync("字段翻译-获取对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<PropertyItem>> GetPropertiesByItemTypeIdAsync(string itemTypeId)
    {
        try
        {
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "ItemType"),
                    new XAttribute("action", "get"),
                    new XAttribute("select", "id,name,label"),
                    new XElement("id", itemTypeId),
                    new XElement("Relationships",
                        new XElement("Item",
                            new XAttribute("type", "Property"),
                            new XAttribute("action", "get"),
                            new XAttribute("language", DefaultLabelLanguages),
                            new XAttribute("select", "id,name,label,data_type,sort_order")))));
            var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
            ArasTranslationSupport.ThrowIfError(result, "读取对象类字段失败");
            if (result.getItemCount() == 0)
                return [];

            var itemType = result.getItemByIndex(0);
            var itemTypeName = itemType.getProperty("name", string.Empty);
            var relationships = itemType.getRelationships();
            var properties = new List<PropertyItem>();
            for (var index = 0; index < relationships.getItemCount(); index++)
            {
                var property = relationships.getItemByIndex(index);
                properties.Add(new PropertyItem
                {
                    Id = property.getID(),
                    Name = property.getProperty("name", string.Empty),
                    Label = property.getProperty("label", string.Empty, "en"),
                    LabelZc = property.getProperty("label", string.Empty, "zc"),
                    LabelZt = property.getProperty("label", string.Empty, "zt"),
                    DataType = property.getProperty("data_type", string.Empty),
                    ItemTypeName = itemTypeName,
                    IsSelected = true
                });
            }

            return properties
                .Where(property => !string.IsNullOrWhiteSpace(property.Id))
                .OrderBy(property => property.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("字段翻译-获取字段", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public Task<List<PropertyItem>> QueryPropertiesByAmlAsync(string aml)
        => Task.FromResult(new List<PropertyItem>());

    public Task<List<PropertyItem>> QueryPropertiesBySqlAsync(string sql)
        => Task.FromResult(new List<PropertyItem>());

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
        List<PropertyItem> properties,
        string sourceLanguage,
        string targetLanguages,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var selected = properties.Where(property => property.IsSelected).ToList();
        var targets = ArasTranslationSupport.ParseTargets(targetLanguages);
        var records = new List<TranslationRecord>();
        var completedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var totalOperations = selected.Count * targets.Count;
        var completedOperations = 0;

        if (selected.Count == 0)
            throw new InvalidOperationException("至少需要选择一个字段进行翻译。");

        try
        {
            await _translationLogService.SaveOutcomeAsync(
                task, [], "Translating", 0, $"0/{totalOperations}");
            var innovator = ArasTranslationSupport.GetConnectedInnovator(_connectionService);

            foreach (var target in targets)
            {
                foreach (var property in selected)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var original = GetSourceLabel(property, sourceLanguage);
                    var translated = await ArasTranslationSupport.TranslateTextAsync(
                        _aiDispatcher,
                        original,
                        sourceLanguage,
                        target,
                        $"Aras 对象类 {property.ItemTypeName} 的字段标签",
                        cancellationToken);

                    ArasTranslationSupport.ApplyLocalizedValues(
                        innovator,
                        "Property",
                        property.Id,
                        target,
                        new Dictionary<string, string> { ["label"] = translated });
                    SetLocalizedLabel(property, target.LanguageCode, translated);

                    records.Add(ArasTranslationSupport.CreateRecord(
                        task, property.Id, property.Name, original, translated, target));
                    property.TranslationPreview = AppendPreview(
                        property.TranslationPreview, target.Name, translated);
                    completedIds.Add(property.Id);
                    completedOperations++;
                    progress?.Report(CreateProgress(
                        completedOperations, totalOperations, property.Name, target.Name));
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

            await _errorLogService.LogErrorAsync("字段翻译-执行", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<string> ExportToExcelAsync(TranslationTask task, List<PropertyItem> properties)
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
            var worksheet = package.Workbook.Worksheets.Add("字段翻译");
            worksheet.Cells[1, 1].Value = "字段名称";
            worksheet.Cells[1, 2].Value = "英文标签(label/en)";
            worksheet.Cells[1, 3].Value = "简体中文标签(i18n/zc)";
            worksheet.Cells[1, 4].Value = "繁体中文标签(i18n/zt)";
            worksheet.Cells[1, 5].Value = "翻译结果";
            worksheet.Cells[1, 6].Value = "数据类型";
            worksheet.Cells[1, 7].Value = "对象类";
            for (var index = 0; index < properties.Count; index++)
            {
                worksheet.Cells[index + 2, 1].Value = properties[index].Name;
                worksheet.Cells[index + 2, 2].Value = properties[index].Label;
                worksheet.Cells[index + 2, 3].Value = properties[index].LabelZc;
                worksheet.Cells[index + 2, 4].Value = properties[index].LabelZt;
                worksheet.Cells[index + 2, 5].Value = properties[index].TranslationPreview;
                worksheet.Cells[index + 2, 6].Value = properties[index].DataType;
                worksheet.Cells[index + 2, 7].Value = properties[index].ItemTypeName;
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
            await _errorLogService.LogErrorAsync("字段翻译-导出", ex.Message,
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

    private static string GetSourceLabel(PropertyItem property, string sourceLanguage)
    {
        var label = sourceLanguage.Trim() switch
        {
            "简体中文" or "中文" or "zc" => property.LabelZc,
            "繁体中文" or "繁體中文" or "zt" => property.LabelZt,
            "英文" or "英语" or "English" or "en" => property.Label,
            _ => string.Empty
        };

        return string.IsNullOrWhiteSpace(label) ? property.Name : label;
    }

    private static void SetLocalizedLabel(PropertyItem property, string languageCode, string value)
    {
        switch (languageCode.ToLowerInvariant())
        {
            case "en":
                property.Label = value;
                break;
            case "zc":
                property.LabelZc = value;
                break;
            case "zt":
                property.LabelZt = value;
                break;
        }
    }
}
