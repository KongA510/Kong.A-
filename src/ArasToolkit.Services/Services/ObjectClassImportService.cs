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
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;


    private const string ImportBaseDir = "Config/ObjectClassImports";
    private const string TemplateDir = "Config/ObjectClassTemplates";

    public ObjectClassImportService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        ArasConnectionService connectionService,
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
    /// <summary>
    /// 执行导入汇入到 Aras 系统
    /// </summary>
    /// <param name="filePath">要导入的 Excel 文件路径</param>
    /// <param name="progress">进度报告回调</param>
    /// <returns>导入结果</returns>
    public async Task<ObjectClassImportResult> ImportAsync(string filePath, string importMode = "覆盖", IProgress<string>? progress = null)
    {
        var result = new ObjectClassImportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // 按日期分目录: Config/ObjectClassImports/2026_6_30/
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dateDir = Path.Combine(baseDir, ImportBaseDir, dateFolder);

        // 子目录: logs / uploads 分离
        var logsDir = Path.Combine(dateDir, "logs");
        var uploadsDir = Path.Combine(dateDir, "uploads");
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(uploadsDir);

        var timestamp = DateTime.Now.ToString("HHmmss");
        var logFileName = $"import_{timestamp}.log";
        var logFile = Path.Combine(logsDir, logFileName);
        result.LogFilePath = logFile;

        // 复制文件到 uploads 目录（相对路径存储）
        var savedFileName = $"{timestamp}_{Path.GetFileName(filePath)}";
        var savedFilePath = Path.Combine(uploadsDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportBaseDir}/{dateFolder}/uploads/{savedFileName}";

        using var writer = new StreamWriter(logFile, false);
        try
        {
            await writer.WriteLineAsync("===== 对象类配置导入日志 =====");
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}");
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            progress?.Report("正在读取 Excel 文件...");

            using var package = new ExcelPackage(new FileInfo(filePath));

            // ===== Step 1: 读取 Sheet1「对象类新增」=====
            //读取时先通过注入获取aras链接对象
            var connection = _connectionService.HttpConnection;
            if (connection == null)
            {
                throw new Exception("未连接到 Aras 系统，请先登录。");
            }
            var login = connection.Login();
            var inn=login.getInnovator();

            progress?.Report("正在处理 Sheet1「对象类新增」...");
            var sheet1Rows = ReadSheetRows(package, "对象类新增", 10);
            await writer.WriteLineAsync($"Sheet1 对象类新增: {sheet1Rows.Count} 行");

            int sheet1Success = 0;
            result.Sheet1Total = sheet1Rows.Count;
            for (int i = 0; i < sheet1Rows.Count; i++)
            {
                var row = sheet1Rows[i];
                progress?.Report($"对象类{importMode}: {i + 1}/{sheet1Rows.Count} — {row.GetValueOrDefault(0, "")}");

                try
                {
                    //Aras 新增这里建议使用AML进行提高效能并且能正确的关系类的创建
                    var itemTypeName = row.GetValueOrDefault(1, ""); // 对象类名称
                    var addAml = importMode == "新增"
                        ? $"<AML>" +
                          $"  <Item type='ItemType' action='add'>" +
                          $"      <name>{row.GetValueOrDefault(1, "")}</name>" +
                          $"      <i18n:label xml:lang='zc' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(2, "")}</i18n:label>" +
                          $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(3, "")}</i18n:label>" +
                          $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(4, "")}</i18n:label>" +
                          $"      <label_plural>{row.GetValueOrDefault(5, "")}</label_plural>" +
                          $"      <structure_view>tabs on</structure_view>" +
                          $"      <is_versionable>{row.GetValueOrDefault(8, "0")}</is_versionable>" +
                          $"      <auto_search>1</auto_search>" +
                          $"      <default_page_size>50</default_page_size>" +
                          $"      <implementation_type>table</implementation_type>" +
                          $"      <enforce_discovery>1</enforce_discovery>" +
                          $"      <revisions>7FE395DD8B9F4E1090756A34B733D75E</revisions>" +
                          $"  </Item>" +
                          $"</AML>"
                        : $"<AML>" +
                          $"  <Item type='ItemType' action='merge' where=\"ItemType.name='{itemTypeName}'\">" +
                          $"      <name>{row.GetValueOrDefault(1, "")}</name>" +
                          $"      <i18n:label xml:lang='zh-CN' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(2, "")}</i18n:label>" +
                          $"      <i18n:label xml:lang='zh-TW' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(3, "")}</i18n:label>" +
                          $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{row.GetValueOrDefault(4, "")}</i18n:label>" +
                          $"      <label_plural>{row.GetValueOrDefault(5, "")}</label_plural>" +
                          $"      <structure_view>tabs on</structure_view>" +
                          $"      <is_versionable>{row.GetValueOrDefault(8, "0")}</is_versionable>" +
                          $"      <auto_search>1</auto_search>" +
                          $"      <default_page_size>50</default_page_size>" +
                          $"      <implementation_type>table</implementation_type>" +
                          $"      <enforce_discovery>1</enforce_discovery>" +
                          $"      <revisions>7FE395DD8B9F4E1090756A34B733D75E</revisions>" +
                          $"  </Item>" +
                          $"</AML>";
                   var addItem=  inn.applyAML(addAml);
                    sheet1Success++;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet1 行{i + 2}] {row.GetValueOrDefault(0, "")}: {ex.Message}";
                    await writer.WriteLineAsync(failMsg);
                    result.FailedDetails.Add(failMsg);
                }
            }

            result.Sheet1Count = sheet1Success;
            await writer.WriteLineAsync($"Sheet1 成功: {sheet1Success}/{sheet1Rows.Count}");

            // ===== Step 2: 读取 Sheet2「关系类新增」=====
            progress?.Report("正在处理 Sheet2「关系类新增」...");
            var sheet2Rows = ReadSheetRows(package, "关系类新增", 12);
            await writer.WriteLineAsync($"Sheet2 关系类新增: {sheet2Rows.Count} 行");

            int sheet2Success = 0;
            result.Sheet2Total = sheet2Rows.Count;
            for (int i = 0; i < sheet2Rows.Count; i++)
            {
                var row = sheet2Rows[i];
                progress?.Report($"关系类新增: {i + 1}/{sheet2Rows.Count} — {row.GetValueOrDefault(1, "")}");

                try
                {
                    // ===== TODO: 用户自行维护 Aras 汇入逻辑 =====
                    // 此处构建 Relationship Item 并通过 Innovator.applyItem() 创建关系类
                    // ...

                    sheet2Success++;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet2 行{i + 2}] {row.GetValueOrDefault(1, "")}: {ex.Message}";
                    await writer.WriteLineAsync(failMsg);
                    result.FailedDetails.Add(failMsg);
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
