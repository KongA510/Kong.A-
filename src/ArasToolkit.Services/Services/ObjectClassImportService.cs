using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 对象类配置导入服务 — Excel模板生成 + 批量汇入Aras
/// 单线程串行执行，Aras汇入逻辑使用占位符由用户自行维护
/// </summary>
public class ObjectClassImportService : IObjectClassImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ImportDir = "Config/ObjectClassImports";
    private const string TemplateDir = "Config/ObjectClassTemplates";

    public ObjectClassImportService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IArasConnectionService connectionService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _connectionService = connectionService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    // ==================== 模板生成 ====================

    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // ===== Sheet 1: 对象类新增 =====
        var ws1 = package.Workbook.Worksheets.Add("对象类新增");
        var headers1 = new[]
        {
            "对象类名称", "物件显示名称", "物件显示名称繁体", "物件显示名称英文",
            "TOC显示文字", "TOC显示文字繁体", "TOC显示文字英文",
            "可换版(1=可以 0=不可以)", "自动搜索", "页面默认大小"
        };
        for (int i = 0; i < headers1.Length; i++)
        {
            ws1.Cells[1, i + 1].Value = headers1[i];
            ws1.Cells[1, i + 1].Style.Font.Bold = true;
        }
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 30);

        // ===== Sheet 2: 关系类新增 =====
        var ws2 = package.Workbook.Worksheets.Add("关系类新增");
        var headers2 = new[]
        {
            "父对象名称", "关系类名称", "页签序号", "页签标签",
            "页签标签繁体", "页签标签英文",
            "新建关系选项(1=仅选取 2=仅创建 3=均可)", "必须",
            "打开相关窗体", "自动搜索(默认1)", "源对象类", "相关对象类"
        };
        for (int i = 0; i < headers2.Length; i++)
        {
            ws2.Cells[1, i + 1].Value = headers2[i];
            ws2.Cells[1, i + 1].Style.Font.Bold = true;
        }
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    public async Task<ObjectClassImportResult> ImportAsync(string filePath, IProgress<string>? progress = null)
    {
        var result = new ObjectClassImportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // 准备日志目录
        var importDir = Path.Combine(baseDir, ImportDir);
        Directory.CreateDirectory(importDir);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var logFile = Path.Combine(importDir, $"import_{timestamp}.log");
        result.LogFilePath = logFile;

        // 复制文件到导入目录（相对路径存储）
        var savedFileName = $"import_{timestamp}.xlsx";
        var savedFilePath = Path.Combine(importDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportDir}/{savedFileName}";

        using var writer = new StreamWriter(logFile, false);
        try
        {
            await writer.WriteLineAsync("===== 对象类配置导入日志 =====");
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}");
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            progress?.Report("正在读取 Excel 文件...");

            using var package = new ExcelPackage(new FileInfo(filePath));

            // ===== Step 1: 读取 Sheet1「对象类新增」=====
            progress?.Report("正在处理 Sheet1「对象类新增」...");
            var sheet1Rows = ReadSheetRows(package, "对象类新增", 10);
            await writer.WriteLineAsync($"Sheet1 对象类新增: {sheet1Rows.Count} 行");

            int sheet1Success = 0;
            for (int i = 0; i < sheet1Rows.Count; i++)
            {
                var row = sheet1Rows[i];
                progress?.Report($"对象类新增: {i + 1}/{sheet1Rows.Count} — {row.GetValueOrDefault(0, "")}");

                try
                {
                    // ===== TODO: 用户自行维护 Aras 汇入逻辑 =====
                    // 此处构建 Aras Item 并通过 Innovator.applyItem() 创建对象类
                    // 示例代码（需根据实际 Aras API 调整）:
                    //
                    // var innovator = _connectionService.InnovatorInstance;
                    // var item = innovator.newItem("ItemType", "add");
                    // item.setProperty("name", row.GetValueOrDefault(0, ""));
                    // item.setProperty("label", row.GetValueOrDefault(1, ""));
                    // item.setProperty("label_zh_tw", row.GetValueOrDefault(2, ""));
                    // item.setProperty("label_en", row.GetValueOrDefault(3, ""));
                    // item.setProperty("toc_accession", row.GetValueOrDefault(4, ""));
                    // item.setProperty("toc_accession_zh_tw", row.GetValueOrDefault(5, ""));
                    // item.setProperty("toc_accession_en", row.GetValueOrDefault(6, ""));
                    // item.setProperty("is_versionable", row.GetValueOrDefault(7, "1"));
                    // item.setProperty("auto_search", row.GetValueOrDefault(8, ""));
                    // item.setProperty("instance_data", row.GetValueOrDefault(9, ""));
                    // var resultItem = innovator.applyItem(item);
                    // if (resultItem.isError())
                    //     throw new Exception(resultItem.getErrorString());

                    sheet1Success++;
                }
                catch (Exception ex)
                {
                    await writer.WriteLineAsync($"[Sheet1 失败] 行{i + 2}: {ex.Message}");
                }
            }

            result.Sheet1Count = sheet1Success;
            await writer.WriteLineAsync($"Sheet1 成功: {sheet1Success}/{sheet1Rows.Count}");

            // ===== Step 2: 读取 Sheet2「关系类新增」=====
            progress?.Report("正在处理 Sheet2「关系类新增」...");
            var sheet2Rows = ReadSheetRows(package, "关系类新增", 12);
            await writer.WriteLineAsync($"Sheet2 关系类新增: {sheet2Rows.Count} 行");

            int sheet2Success = 0;
            for (int i = 0; i < sheet2Rows.Count; i++)
            {
                var row = sheet2Rows[i];
                progress?.Report($"关系类新增: {i + 1}/{sheet2Rows.Count} — {row.GetValueOrDefault(1, "")}");

                try
                {
                    // ===== TODO: 用户自行维护 Aras 汇入逻辑 =====
                    // 此处构建 Relationship Item 并通过 Innovator.applyItem() 创建关系类
                    // 示例代码（需根据实际 Aras API 调整）:
                    //
                    // var innovator = _connectionService.InnovatorInstance;
                    // var item = innovator.newItem("RelationshipType", "add");
                    // item.setProperty("source_id", row.GetValueOrDefault(0, ""));
                    // item.setProperty("name", row.GetValueOrDefault(1, ""));
                    // item.setProperty("sort_order", row.GetValueOrDefault(2, ""));
                    // item.setProperty("label", row.GetValueOrDefault(3, ""));
                    // item.setProperty("label_zh_tw", row.GetValueOrDefault(4, ""));
                    // item.setProperty("label_en", row.GetValueOrDefault(5, ""));
                    // item.setProperty("new_relationship_option", row.GetValueOrDefault(6, "3"));
                    // item.setProperty("is_required", row.GetValueOrDefault(7, ""));
                    // item.setProperty("open_related_form", row.GetValueOrDefault(8, ""));
                    // item.setProperty("auto_search", row.GetValueOrDefault(9, "1"));
                    // item.setProperty("related_item_type", row.GetValueOrDefault(10, ""));
                    // item.setProperty("related_rel_item_type", row.GetValueOrDefault(11, ""));
                    // var resultItem = innovator.applyItem(item);
                    // if (resultItem.isError())
                    //     throw new Exception(resultItem.getErrorString());

                    sheet2Success++;
                }
                catch (Exception ex)
                {
                    await writer.WriteLineAsync($"[Sheet2 失败] 行{i + 2}: {ex.Message}");
                }
            }

            result.Sheet2Count = sheet2Success;
            await writer.WriteLineAsync($"Sheet2 成功: {sheet2Success}/{sheet2Rows.Count}");

            // ===== 保存导入记录到数据库 =====
            var log = new ObjectClassImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = ObjectClassImportLog.StatusSuccess,
                Sheet1Count = sheet1Success,
                Sheet2Count = sheet2Success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log);

            // 记录操作日志
            await _operationLogService.LogAsync("Import", "ObjectClassImportLog", log.Id,
                $"对象类配置导入: 对象类{sheet1Success}条 / 关系类{sheet2Success}条");

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====");
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}");
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}");

            // 保存失败记录
            try
            {
                var log = new ObjectClassImportLog
                {
                    UserId = CurrentUserContext.CurrentUserId ?? "system",
                    ImportTime = DateTime.Now,
                    ImportFile = relativePath,
                    Status = ObjectClassImportLog.StatusFailed,
                    ErrorLog = $"{ex.Message}\n{ex.StackTrace}",
                    CreatorOn = DateTime.Now
                };
                await SaveLogAsync(log);
            }
            catch { /* 记录保存失败不阻塞主流程 */ }

            await _errorLogService.LogErrorAsync("对象类配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await writer.WriteLineAsync($"对象类: {result.Sheet1Count}  关系类: {result.Sheet2Count}");
            await writer.WriteLineAsync("===== 日志结束 =====");
            writer.Close();
        }

        return result;
    }

    // ==================== 历史记录 ====================

    public async Task<(List<ObjectClassImportLog> Items, int TotalCount)> GetHistoryAsync(string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.ObjectClassImportLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(r => r.UserId == userId);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.CreatorOn).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<ObjectClassImportLog?> GetLogByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ObjectClassImportLogs.FindAsync(id);
    }

    // ==================== 私有方法 ====================

    /// <summary>
    /// 读取指定 Sheet 的数据行（跳过表头行）
    /// </summary>
    private static List<Dictionary<int, string>> ReadSheetRows(ExcelPackage package, string sheetName, int columnCount)
    {
        var rows = new List<Dictionary<int, string>>();
        var ws = package.Workbook.Worksheets[sheetName];
        if (ws?.Dimension == null) return rows;

        int row = 2; // 跳过表头行
        while (row <= ws.Dimension.End.Row)
        {
            var dict = new Dictionary<int, string>();
            for (int col = 1; col <= columnCount; col++)
            {
                var val = ws.Cells[row, col].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(val))
                    dict[col] = val;
            }
            if (dict.Count > 0)
                rows.Add(dict);
            row++;
        }
        return rows;
    }

    /// <summary>
    /// 保存导入日志到数据库
    /// </summary>
    private async Task SaveLogAsync(ObjectClassImportLog log)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.ObjectClassImportLogs.Add(log);
        await db.SaveChangesAsync();
    }
}
