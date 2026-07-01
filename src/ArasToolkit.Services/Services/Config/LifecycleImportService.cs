using System;
using System.Collections.Generic;
using System.IO;
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

/// <summary>
/// 生命周期配置导入服务 — Excel模板生成 + 批量汇入Aras
///
/// 导入流程:
/// 1. 读取 Excel 文件（Sheet「生命周期配置」）
/// 2. 逐行构造 AML 语句，通过 Aras API 执行
/// 3. 记录详细日志到本地文件 + 操作日志到数据库
///
/// 模板说明:
/// Sheet「生命周期配置」9列:
///   状态名称 | 状态标签 | 状态标签繁体 | 状态标签英文 |
///   所属对象类 | 是否为初始状态 | 允许流转目标 | 转移条件 | 备注说明
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位符注入点
/// - 使用单线程串行执行，确保 Aras 数据一致性
/// - 支持 CancellationToken 取消 + IProgress&lt;ImportProgressInfo&gt; 进度报告
/// </summary>
public class LifecycleImportService : ILifecycleImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ImportBaseDir = "Config/LifecycleImports";

    public LifecycleImportService(
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

    /// <summary>
    /// 生成 Excel 模板文件（1个Sheet：生命周期配置，9列，含加粗表头+自适应列宽）
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        var ws = package.Workbook.Worksheets.Add("生命周期配置");
        var headers = new[]
        {
            "状态名称",        // Col 1 → AML <name>
            "状态标签",        // Col 2 → AML <label>（简体中文）
            "状态标签繁体",    // Col 3 → AML <i18n:label xml:lang='zt'>
            "状态标签英文",    // Col 4 → AML <i18n:label xml:lang='en'>
            "所属对象类",      // Col 5 → AML source_id（ItemType name）
            "是否为初始状态",  // Col 6 → AML <is_default> (0/1)
            "允许流转目标",    // Col 7 → AML <transition_to>（逗号分隔）
            "转移条件",        // Col 8 → AML <transition_condition>
            "备注说明"         // Col 9 → AML <description>
        };
        WriteHeaders(ws, headers);
        ws.Cells[1, 1, 1, headers.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 执行生命周期导入汇入到 Aras 系统
    /// </summary>
    public async Task<LifecycleImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new LifecycleImportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // 1. 创建按日期隔离的目录结构
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dateDir = Path.Combine(baseDir, ImportBaseDir, dateFolder);
        var logsDir = Path.Combine(dateDir, "logs");
        var uploadsDir = Path.Combine(dateDir, "uploads");
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(uploadsDir);

        // 2. 准备日志文件
        var timestamp = DateTime.Now.ToString("HHmmss");
        var logFileName = $"import_{timestamp}.log";
        var logFile = Path.Combine(logsDir, logFileName);
        result.LogFilePath = logFile;

        // 3. 归档源文件
        var savedFileName = $"{timestamp}_{Path.GetFileName(filePath)}";
        var savedFilePath = Path.Combine(uploadsDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportBaseDir}/{dateFolder}/uploads/{savedFileName}";

        using var writer = new StreamWriter(logFile, false);
        try
        {
            await writer.WriteLineAsync("===== 生命周期配置导入日志 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"导入模式: {importMode}").ConfigureAwait(false);

            // 4. 获取 Aras 连接
            var connection = _connectionService.HttpConnection
                ?? throw new InvalidOperationException("未连接到 Aras 系统，请先登录。");

            cancellationToken.ThrowIfCancellationRequested();

            var login = connection.Login();
            var innovator = login.getInnovator();

            progress?.Report(new ImportProgressInfo
            {
                Phase = "初始化",
                Current = 0,
                ItemName = "正在连接 Aras..."
            });

            // 5. 读取 Excel 数据
            progress?.Report(new ImportProgressInfo
            {
                Phase = "读取Excel",
                Current = 0,
                ItemName = "正在读取 Excel 文件..."
            });

            using var package = new ExcelPackage(new FileInfo(filePath));
            var rows = ReadSheetRows(package, "生命周期配置", 9);

            result.Sheet1Total = rows.Count;

            await writer.WriteLineAsync($"生命周期配置: {rows.Count} 行").ConfigureAwait(false);

            // 6. 处理 Sheet: 生命周期配置
            int success = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = rows[i];
                var itemName = row.GetValueOrDefault(1, "");            // 状态名称（Col 1）
                var overallIdx = i + 1;

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"生命周期{importMode}",
                    Current = i + 1,
                    PhaseTotal = rows.Count,
                    OverallCurrent = overallIdx,
                    OverallTotal = rows.Count,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var aml = BuildLifecycleAml(row, importMode);
                    var amlResult = innovator.applyAML(aml);

                    if (amlResult.isError())
                    {
                        var failMsg = $"[行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else
                    {
                        success++;
                    }
                }
                catch (OperationCanceledException)
                {
                    await writer.WriteLineAsync($"[取消] 用户在第 {i + 1}/{rows.Count} 条处取消导入")
                        .ConfigureAwait(false);
                    throw;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[行{i + 2}] {itemName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }

            result.Sheet1Count = success;
            await writer.WriteLineAsync($"成功: {success}/{rows.Count}").ConfigureAwait(false);

            // 7. 保存导入成功记录到数据库
            var log = new LifecycleImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = LifecycleImportLog.StatusSuccess,
                Sheet1Count = success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            // 记录敏感操作日志
            await _operationLogService.LogAsync("Import", "LifecycleImportLog", log.Id,
                $"生命周期配置导入: {success}条").ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = rows.Count,
                PhaseTotal = rows.Count,
                OverallCurrent = rows.Count,
                OverallTotal = rows.Count,
                ItemName = $"生命周期配置{success}条"
            });
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "导入已被用户取消";
            await writer.WriteLineAsync("[取消] 导入已被用户取消").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, "用户取消导入", result).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, $"{ex.Message}\n{ex.StackTrace}", result)
                .ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("生命周期配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"生命周期配置: {result.Sheet1Count}/{result.Sheet1Total}")
                .ConfigureAwait(false);
            await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
            writer.Close();
        }

        return result;
    }

    // ==================== 历史记录查询 ====================

    public async Task<(List<LifecycleImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.LifecycleImportLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(r => r.UserId == userId);

        var total = await query.CountAsync().ConfigureAwait(false);
        var items = await query
            .OrderByDescending(r => r.CreatorOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync().ConfigureAwait(false);

        return (items, total);
    }

    public async Task<LifecycleImportLog?> GetLogByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.LifecycleImportLogs.FindAsync(id).ConfigureAwait(false);
    }

    // ==================== AML 构建（占位符 — 调用方自行维护） ====================

    /// <summary>
    /// 构建生命周期状态（Life Cycle State）的 AML 语句
    ///
    /// 新增模式: action='add' — 创建全新生命周期状态
    /// 覆盖模式: action='merge' — 按名称匹配，存在则更新
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// 列映射（用户可见列号 → GetValueOrDefault 索引）:
    /// Col 1 (idx 1): 状态名称 → <name>
    /// Col 2 (idx 2): 状态标签 → <label>（简体中文）
    /// Col 3 (idx 3): 状态标签繁体 → <i18n:label xml:lang='zt'>
    /// Col 4 (idx 4): 状态标签英文 → <i18n:label xml:lang='en'>
    /// Col 5 (idx 5): 所属对象类 → source_id 的 ItemType name
    /// Col 6 (idx 6): 是否为初始状态 → <is_default>
    /// Col 7 (idx 7): 允许流转目标 → <transition_to>（逗号分隔）
    /// Col 8 (idx 8): 转移条件 → <transition_condition>
    /// Col 9 (idx 9): 备注说明 → <description>
    /// </summary>
    private static string BuildLifecycleAml(Dictionary<int, string> row, string importMode)
    {
        var stateName = row.GetValueOrDefault(1, "");        // 状态名称
        var label = row.GetValueOrDefault(2, "");             // 状态标签（简）
        var labelZt = row.GetValueOrDefault(3, "");           // 状态标签（繁）
        var labelEn = row.GetValueOrDefault(4, "");           // 状态标签（英）
        var sourceName = row.GetValueOrDefault(5, "");        // 所属对象类
        var isDefault = row.GetValueOrDefault(6, "");         // 是否为初始状态
        var transitionTo = row.GetValueOrDefault(7, "");      // 允许流转目标
        var transitionCond = row.GetValueOrDefault(8, "");    // 转移条件
        var description = row.GetValueOrDefault(9, "");       // 备注说明

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Life Cycle State' action='add'>" +
                   $"      <name>{stateName}</name>" +
                   $"      <label>{label}</label>" +
                   $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
                   $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
                   $"      <source_id>" +
                   $"          <Item type='ItemType' action='get' select='id'>" +
                   $"              <name>{sourceName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"      <is_default>{isDefault}</is_default>" +
                   $"      <transition_to>{transitionTo}</transition_to>" +
                   $"      <transition_condition>{transitionCond}</transition_condition>" +
                   $"      <description>{description}</description>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        return $"<AML>" +
               $"  <Item type='Life Cycle State' action='merge' where=\"Life_Cycle_State.name='{stateName}'\">" +
               $"      <name>{stateName}</name>" +
               $"      <label>{label}</label>" +
               $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
               $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
               $"      <source_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{sourceName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <is_default>{isDefault}</is_default>" +
               $"      <transition_to>{transitionTo}</transition_to>" +
               $"      <transition_condition>{transitionCond}</transition_condition>" +
               $"      <description>{description}</description>" +
               $"  </Item>" +
               $"</AML>";
    }

    // ==================== 私有辅助方法 ====================

    private static void WriteHeaders(ExcelWorksheet ws, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cells[1, i + 1];
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(253, 235, 208));
        }
    }

    private static List<Dictionary<int, string>> ReadSheetRows(
        ExcelPackage package, string sheetName, int columnCount)
    {
        var rows = new List<Dictionary<int, string>>();
        var ws = package.Workbook.Worksheets[sheetName];
        if (ws?.Dimension == null) return rows;

        int row = 2; // 跳过表头
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

    private async Task SaveLogAsync(LifecycleImportLog log)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.LifecycleImportLogs.Add(log);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task TrySaveFailedLogAsync(string relativePath, string errorDetail,
        LifecycleImportResult result)
    {
        try
        {
            var log = new LifecycleImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = LifecycleImportLog.StatusFailed,
                ErrorLog = errorDetail,
                Sheet1Count = result.Sheet1Count,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
        }
        catch
        {
            // 日志保存失败不阻塞主流程
        }
    }
}
