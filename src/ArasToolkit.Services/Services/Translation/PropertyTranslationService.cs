using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

public class PropertyTranslationService : IPropertyTranslationService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IAiDispatcherService _aiDispatcher;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;
    private const string OutputDir = "Config/PropertyTranslations";

    public PropertyTranslationService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        ArasConnectionService connectionService,
        IAiDispatcherService aiDispatcher,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _connectionService = connectionService;
        _aiDispatcher = aiDispatcher;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<ItemTypeItem>> GetItemTypeListAsync()
    {
        var items = new List<ItemTypeItem>();
        try
        {
            var innovator = _connectionService.TypedInnovator;
            if (innovator == null) return items;

            var aml = "<AML><Item type='ItemType' action='get' select='id,name,label'></Item></AML>";
            var result = innovator.applyAML(aml);
            if (result.isError()) return items;

            int count = result.getItemCount();
            for (int i = 0; i < count; i++)
            {
                var r = result.getItemByIndex(i);
                items.Add(new ItemTypeItem
                {
                    Id = r.getID(),
                    Name = r.getProperty("name", ""),
                    Label = r.getProperty("label", "")
                });
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性翻译-获取对象类列表", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        return items;
    }

    public async Task<List<PropertyItem>> GetPropertiesByItemTypeIdAsync(string itemTypeId)
    {
        var props = new List<PropertyItem>();
        try
        {
            var innovator = _connectionService.TypedInnovator;
            if (innovator == null) return props;

            var aml = $"<AML><Item type='ItemType' action='get' select='id,name,label'><id>{itemTypeId}</id><Relationships><Item type='Property' action='get' select='id,name,label,data_type'></Item></Relationships></Item></AML>";
            var result = innovator.applyAML(aml);
            if (result.isError()) return props;

            var itemType = result.getItemByIndex(0);
            var itemTypeName = itemType.getProperty("name", "");
            var rels = itemType.getRelationships();
            int count = rels.getItemCount();
            for (int i = 0; i < count; i++)
            {
                var r = rels.getItemByIndex(i);
                props.Add(new PropertyItem
                {
                    Id = r.getID(),
                    Name = r.getProperty("name", ""),
                    Label = r.getProperty("label", ""),
                    DataType = r.getProperty("data_type", ""),
                    ItemTypeName = itemTypeName
                });
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性翻译-获取属性列表", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        return props;
    }

    public Task<List<PropertyItem>> QueryPropertiesByAmlAsync(string aml)
    {
        return Task.FromResult(new List<PropertyItem>());
    }

    public Task<List<PropertyItem>> QueryPropertiesBySqlAsync(string sql)
    {
        return Task.FromResult(new List<PropertyItem>());
    }

    public async Task<TranslationTask> CreateTaskAsync(string taskName, string queryMode, string queryCondition, string sourceLanguage, string targetLanguages, int totalFields)
    {
        var task = new TranslationTask
        {
            TaskName = taskName,
            QueryMode = queryMode,
            QueryCondition = queryCondition,
            SourceLanguage = sourceLanguage,
            TargetLanguages = targetLanguages,
            TotalFields = totalFields,
            Status = "Pending",
            CreatorOn = DateTime.Now
        };

        using var db = await _dbFactory.CreateDbContextAsync();
        db.Set<TranslationTask>().Add(task);
        await db.SaveChangesAsync();
        await _operationLogService.LogAsync("Create", "TranslationTask", task.Id, $"创建属性翻译任务: {taskName}");
        return task;
    }

    public async Task TranslateAsync(TranslationTask task, List<PropertyItem> properties, string sourceLanguage, string targetLanguages, IProgress<TranslationProgressInfo>? progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            task.Status = "Translating";
            var targets = targetLanguages.Split(',', StringSplitOptions.RemoveEmptyEntries);
            int total = properties.Count * targets.Length;
            int done = 0;

            foreach (var target in targets)
            {
                foreach (var prop in properties)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var prompt = $"将以下文本从{sourceLanguage}翻译为{target.Trim()}，只返回翻译结果，不要解释：{prop.Label}";
                    var translated = await _aiDispatcher.ChatAsync(prompt, cancellationToken: cancellationToken);
                    prop.Label = translated.Trim();
                    done++;
                    progress?.Report(new TranslationProgressInfo
                    {
                        Phase = "翻译中",
                        Current = done,
                        PhaseTotal = total,
                        OverallCurrent = done,
                        OverallTotal = total,
                        ItemName = prop.Name
                    });
                }
            }

            task.TranslatedFields = properties.Count;
            task.ProgressText = $"{done}/{total}";
            task.Status = "Completed";

            using var db = await _dbFactory.CreateDbContextAsync();
            db.Set<TranslationTask>().Update(task);
            await db.SaveChangesAsync();
            await _operationLogService.LogAsync("Update", "TranslationTask", task.Id, $"属性翻译完成: {task.TaskName}");
        }
        catch (OperationCanceledException)
        {
            task.Status = "Cancelled";
            using var db = await _dbFactory.CreateDbContextAsync();
            db.Set<TranslationTask>().Update(task);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            task.Status = "Failed";
            await _errorLogService.LogErrorAsync("属性翻译-执行翻译", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            using var db = await _dbFactory.CreateDbContextAsync();
            db.Set<TranslationTask>().Update(task);
            await db.SaveChangesAsync();
        }
    }

    public async Task<string> ExportToExcelAsync(TranslationTask task, List<PropertyItem> properties)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dir = Path.Combine(baseDir, OutputDir, dateFolder);
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, $"{task.TaskName}_{DateTime.Now:HHmmss}.xlsx");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("属性翻译");
        ws.Cells[1, 1].Value = "属性名称";
        ws.Cells[1, 2].Value = "标签";
        ws.Cells[1, 3].Value = "数据类型";
        ws.Cells[1, 4].Value = "对象类";
        for (int i = 0; i < properties.Count; i++)
        {
            ws.Cells[i + 2, 1].Value = properties[i].Name;
            ws.Cells[i + 2, 2].Value = properties[i].Label;
            ws.Cells[i + 2, 3].Value = properties[i].DataType;
            ws.Cells[i + 2, 4].Value = properties[i].ItemTypeName;
        }
        ws.Cells.AutoFitColumns();
        await package.SaveAsAsync(new FileInfo(filePath));

        task.OutputFilePath = filePath;
        using var db = await _dbFactory.CreateDbContextAsync();
        db.Set<TranslationTask>().Update(task);
        await db.SaveChangesAsync();
        return filePath;
    }

    public async Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(string? userId = null, int page = 1, int pageSize = 20)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.Set<TranslationTask>().AsQueryable();
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(x => x.UserId == userId);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatorOn)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


