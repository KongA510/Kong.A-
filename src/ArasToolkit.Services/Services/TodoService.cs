using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 待办项目服务实现 — EF Core 数据库存储、操作日志、EPPlus 导入导出
/// </summary>
public class TodoService : ITodoService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private static readonly string[] ValidStatuses = ["未开始", "处理中", "延期", "已完成"];

    public TodoService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _contextFactory = contextFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    // ===== 公共方法 =====

    public async Task<(List<PersonalTask> Items, int TotalCount)> GetPagedItemsAsync(
        int page, int pageSize, string? statusFilter = null, string? projectNameFilter = null, string? searchKeyword = null, DateTime? completionDate = null, DateTime? dueDate = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PersonalTasks.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "全部")
                query = query.Where(t => t.Status == statusFilter);

            if (!string.IsNullOrEmpty(projectNameFilter) && projectNameFilter != "全部")
                    query = query.Where(t => t.ProjectName == projectNameFilter);

           if (!string.IsNullOrEmpty(searchKeyword))
               query = query.Where(t => t.TaskName.Contains(searchKeyword));

            if (completionDate.HasValue)
                query = query.Where(t => t.CompletionDate.HasValue
                    && t.CompletionDate.Value.Date == completionDate.Value.Date);

            if (dueDate.HasValue)
                query = query.Where(t => t.ExpectedDate.HasValue
                    && t.ExpectedDate.Value.Date == dueDate.Value.Date);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatorOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-查询列表", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<string>> GetDistinctProjectNamesAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PersonalTasks
                .Select(t => t.ProjectName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-项目列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return [];
        }
    }

    public async Task<PersonalTask?> GetItemByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PersonalTasks.FindAsync(id);
    }

    public async Task AddItemAsync(PersonalTask item)
    {
        if (string.IsNullOrWhiteSpace(item.TaskName))
            throw new ArgumentException("任务名称不能为空");

        if (!ValidStatuses.Contains(item.Status))
            throw new ArgumentException($"无效的状态值: {item.Status}");

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            item.Id = Guid.NewGuid().ToString("N")[..12];
            item.CreatedDate = DateTime.Now;
            item.CreatorOn = DateTime.Now;
            item.CreatedBy = CurrentUserContext.CurrentUserName;

            context.PersonalTasks.Add(item);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Create", "PersonalTask", item.Id,
                $"创建任务: {item.TaskName}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-新增", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task UpdateItemAsync(PersonalTask item)
    {
        if (string.IsNullOrWhiteSpace(item.TaskName))
            throw new ArgumentException("任务名称不能为空");

        if (!ValidStatuses.Contains(item.Status))
            throw new ArgumentException($"无效的状态值: {item.Status}");

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.PersonalTasks.FindAsync(item.Id);
            if (existing == null)
                throw new InvalidOperationException($"未找到ID为 {item.Id} 的待办项");

            existing.TaskName = item.TaskName;
            existing.Description = item.Description;
            existing.ExpectedDate = item.ExpectedDate;
            existing.StartDate = item.StartDate;
            existing.CompletionDate = item.CompletionDate;
            existing.ProjectName = item.ProjectName;
            existing.Remarks = item.Remarks;
            existing.CompletionPercent = item.CompletionPercent;
            existing.Status = item.Status;
            existing.ModifiedDate = DateTime.Now;
            existing.ModifiedBy = CurrentUserContext.CurrentUserName;

            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Update", "PersonalTask", item.Id,
                $"更新任务: {item.TaskName} → 状态: {item.Status}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-更新", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task DeleteItemAsync(string id)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var item = await context.PersonalTasks.FindAsync(id);
            if (item == null) return;

            var taskName = item.TaskName;
            context.PersonalTasks.Remove(item);
            await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Delete", "PersonalTask", id,
                $"删除任务: {taskName}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-删除", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task SyncSchemaAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.EnsureSchemaAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-表结构同步", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    public async Task<int> BatchDeleteAsync(List<string> ids)
    {
        if (ids == null || ids.Count == 0) return 0;

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var items = await context.PersonalTasks
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (items.Count == 0) return 0;

            context.PersonalTasks.RemoveRange(items);
            var count = await context.SaveChangesAsync();

            await _operationLogService.LogAsync("Delete", "PersonalTask", "batch",
                $"批量删除 {count} 条任务");

            return count;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-批量删除", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
            throw;
        }
    }

    public async Task<List<PersonalTask>> ImportFromExcelAsync(string filePath)
    {
        var result = new List<PersonalTask>();

        await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet?.Dimension == null)
                throw new InvalidOperationException("Excel 文件为空");

            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            // 读取列头，建立 列名→列索引 映射
            var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= colCount; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim() ?? "";
                if (!string.IsNullOrEmpty(header))
                    headers[header] = col;
            }

            // 从第二行开始读取数据
            var errors = new List<string>();
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var item = new PersonalTask
                    {
                        Id = Guid.NewGuid().ToString("N")[..12],
                        TaskName = GetCellValue(headers, worksheet, row, "任务名称"),
                        ProjectName = GetCellValue(headers, worksheet, row, "项目名称"),
                        Description = GetCellValue(headers, worksheet, row, "任务说明"),
                        Remarks = GetCellValue(headers, worksheet, row, "备注"),
                        Status = GetCellValue(headers, worksheet, row, "完成状态"),
                        ExpectedDate = ParseDateSafe(GetCellValue(headers, worksheet, row, "任务到期时间"))
                            ?? ParseDateSafe(GetCellValue(headers, worksheet, row, "预计完成时间")),
                        StartDate = ParseDateSafe(GetCellValue(headers, worksheet, row, "开始时间")),
                        CompletionDate = ParseDateSafe(GetCellValue(headers, worksheet, row, "完成时间")),
                        CreatedDate = DateTime.Now,
                        CreatorOn = DateTime.Now
                    };

                    // 验证
                    if (string.IsNullOrWhiteSpace(item.TaskName))
                    {
                        errors.Add($"第{row}行: 任务名称为空，已跳过");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.Status))
                        item.Status = "未开始";
                    else if (!ValidStatuses.Contains(item.Status))
                    {
                        errors.Add($"第{row}行: 无效状态 '{item.Status}'，已设为'未开始'");
                        item.Status = "未开始";
                    }

                    if (item.CompletionPercent < 0) item.CompletionPercent = 0;
                    if (item.CompletionPercent > 100) item.CompletionPercent = 100;

                    result.Add(item);
                }
                catch (Exception ex)
                {
                    errors.Add($"第{row}行: {ex.Message}");
                }
            }

            if (errors.Count > 0 && result.Count == 0)
                throw new InvalidOperationException($"导入失败，共 {errors.Count} 个错误:\n{string.Join("\n", errors.Take(10))}");
        });

        // 批量写入数据库
        if (result.Count > 0)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                context.PersonalTasks.AddRange(result);
                await context.SaveChangesAsync();

                await _operationLogService.LogAsync("Import", "PersonalTask", "batch",
                    $"批量导入 {result.Count} 条任务");
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync("Todo-导入写入", ex.Message,
                    ErrorLog.LevelP0, ex.StackTrace);
                throw;
            }
        }

        return result;
    }

    public async Task ExportTemplateAsync(string filePath)
    {
        try
        {
            await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("待办项目模板");

            // 表头（与 DataGrid 列顺序一致）
            var headers = new[] { "任务名称", "项目名称", "任务说明", "备注", "完成状态", "任务到期时间", "开始时间", "完成时间" };
            for (int i = 0; i < headers.Length; i++)
                worksheet.Cells[1, i + 1].Value = headers[i];

            // 表头样式
            using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // 示例行
            var sampleRow = new[] { "示例任务", "ArasToolkit", "示例说明", "示例备注", "处理中", "2026-06-20", "2026-06-15", "2026-06-20" };
            for (int i = 0; i < sampleRow.Length; i++)
                worksheet.Cells[2, i + 1].Value = sampleRow[i];

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // 数据验证：完成状态列（第5列）
            var statusValidation = worksheet.DataValidations.AddListValidation(
                worksheet.Cells[2, 5, 1000, 5].Address);
            statusValidation.Formula.Values.Add("未开始");
            statusValidation.Formula.Values.Add("处理中");
            statusValidation.Formula.Values.Add("延期");
            statusValidation.Formula.Values.Add("已完成");
            statusValidation.AllowBlank = true;

            package.SaveAs(new FileInfo(filePath));
        });
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-导出模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<int> GetDueTodayCountAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var today = DateTime.Today;
            return await context.PersonalTasks
                .CountAsync(t => t.ExpectedDate.HasValue
                    && t.ExpectedDate.Value.Date == today
                    && t.Status != "已完成");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-到期统计", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return 0;
        }
    }

        public async Task<int> GetUpcomingDueCountAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var today = DateTime.Today;
            var upcoming = today.AddDays(1);
            var deadline = today.AddDays(2);
            return await context.PersonalTasks
                .CountAsync(t => t.ExpectedDate.HasValue
                    && t.ExpectedDate.Value.Date >= upcoming
                    && t.ExpectedDate.Value.Date <= deadline
                    && t.Status != "已完成");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-即将到期统计", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return 0;
        }
    }

    // ===== Excel 辅助方法 =====

    private static string GetCellValue(Dictionary<string, int> headers, ExcelWorksheet worksheet, int row, string columnName)
    {
        if (headers.TryGetValue(columnName, out int col))
            return worksheet.Cells[row, col].Text?.Trim() ?? "";
        return "";
    }

    private static int ParseIntSafe(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        return 0;
    }

    private static DateTime? ParseDateSafe(string value)
    {
        if (DateTime.TryParse(value, out DateTime result))
            return result;
        return null;
    }
}
