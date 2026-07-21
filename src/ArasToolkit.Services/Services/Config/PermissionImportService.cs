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
/// 权限配置导入服务 — Excel模板生成 + 批量汇入Aras
///
/// 导入流程:
/// 1. 读取 Excel 文件（Sheet1「权限配置」→ Sheet2「详细权限」）
/// 2. 逐行构造 AML 语句，通过 Aras API 执行
/// 3. 记录详细日志到本地文件 + 操作日志到数据库
///
/// 模板说明:
/// Sheet1「权限配置」2列:
///   权限配置名称 | 权限名称
///
/// Sheet2「详细权限」6列:
///   权限名称 | 所属角色 | 允许创建 | 允许读取 | 允许更新 | 允许删除
///
/// 导入顺序: 先 Sheet1（权限配置），再 Sheet2（详细权限）
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位方法
/// - 使用单线程串行执行，确保 Aras 数据一致性
/// - 支持 CancellationToken 取消 + IProgress&lt;ImportProgressInfo&gt; 进度报告
/// </summary>
public class PermissionImportService : IPermissionImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ImportBaseDir = "Config/PermissionImports";

    public PermissionImportService(
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
    /// 生成 Excel 模板文件（2个Sheet，含加粗表头、自适应列宽）
    /// Sheet1「权限配置」: 权限配置名称 | 权限名称
    /// Sheet2「详细权限」: 权限名称 | 所属角色 | 允许创建 | 允许读取 | 允许更新 | 允许删除
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // Sheet1: 权限配置
        var ws1 = package.Workbook.Worksheets.Add("权限配置");
        var headers1 = new[]
        {
            "权限配置名称",    // Col 1 → AML source_id（ItemType 名称）
            "权限名称"         // Col 2 → AML <name>
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 25);

        // Sheet2: 详细权限
        var ws2 = package.Workbook.Worksheets.Add("详细权限");
        var headers2 = new[]
        {
            "权限名称",        // Col 1 → 关联 Sheet1 的权限名称
            "所属角色",        // Col 2 → AML <role_id>（Identity name）
            "允许创建",        // Col 3 → AML <can_add> (0/1)
            "允许读取",        // Col 4 → AML <can_get> (0/1)
            "允许更新",        // Col 5 → AML <can_update> (0/1)
            "允许删除"         // Col 6 → AML <can_delete> (0/1)
        };
        WriteHeaders(ws2, headers2);
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 25);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 执行权限导入到 Aras 系统（先 Sheet1 权限配置，再 Sheet2 详细权限）
    /// </summary>
    public async Task<PermissionImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new PermissionImportResult();
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
            await writer.WriteLineAsync("===== 权限配置导入日志 =====").ConfigureAwait(false);
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

            // 5. 读取 Excel 数据（两个 Sheet）
            progress?.Report(new ImportProgressInfo
            {
                Phase = "读取Excel",
                Current = 0,
                ItemName = "正在读取 Excel 文件..."
            });

            using var package = new ExcelPackage(new FileInfo(filePath));
            var sheet1Rows = ReadSheetRows(package, "权限配置", 2);
            var sheet2Rows = ReadSheetRows(package, "详细权限", 6);

            result.Sheet1Total = sheet1Rows.Count;
            result.Sheet2Total = sheet2Rows.Count;
            var overallTotal = sheet1Rows.Count + sheet2Rows.Count;

            await writer.WriteLineAsync($"Sheet1 权限配置: {sheet1Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"Sheet2 详细权限: {sheet2Rows.Count} 行").ConfigureAwait(false);

            // ========== 6. 处理 Sheet1: 权限配置 ==========
            await writer.WriteLineAsync("--- Sheet1: 权限配置 ---").ConfigureAwait(false);
            int sheet1Success = 0;

            for (int i = 0; i < sheet1Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = sheet1Rows[i];
                var itemName = row.GetValueOrDefault(2, "");  // 权限名称（Col 2）

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"权限配置{importMode}",
                    Current = i + 1,
                    PhaseTotal = sheet1Rows.Count,
                    OverallCurrent = i + 1,
                    OverallTotal = overallTotal,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var aml = BuildSheet1Aml(row, importMode);
                    var amlResult = innovator.applyAML(aml);

                    if (amlResult.isError())
                    {
                        var failMsg = $"[Sheet1 行{i + 2}] {itemName} → Aras报错: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else
                    {
                        sheet1Success++;
                    }
                }
                catch (OperationCanceledException)
                {
                    await writer.WriteLineAsync($"[取消] 用户在第 {i + 1}/{sheet1Rows.Count} 行取消操作")
                        .ConfigureAwait(false);
                    throw;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet1 行{i + 2}] {itemName} → 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }

            result.Sheet1Count = sheet1Success;
            await writer.WriteLineAsync($"Sheet1 完成: {sheet1Success}/{sheet1Rows.Count}").ConfigureAwait(false);

            // ========== 7. 处理 Sheet2: 详细权限 ==========
            await writer.WriteLineAsync("--- Sheet2: 详细权限 ---").ConfigureAwait(false);
            int sheet2Success = 0;

            for (int i = 0; i < sheet2Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = sheet2Rows[i];
                var itemName = row.GetValueOrDefault(1, "");  // 权限名称（Col 1）

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"详细权限{importMode}",
                    Current = i + 1,
                    PhaseTotal = sheet2Rows.Count,
                    OverallCurrent = sheet1Rows.Count + i + 1,
                    OverallTotal = overallTotal,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var aml = BuildSheet2Aml(row, importMode);
                    var amlResult = innovator.applyAML(aml);

                    if (amlResult.isError())
                    {
                        var failMsg = $"[Sheet2 行{i + 2}] {itemName} → Aras报错: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else
                    {
                        sheet2Success++;
                    }
                }
                catch (OperationCanceledException)
                {
                    await writer.WriteLineAsync($"[取消] 用户在第 {i + 1}/{sheet2Rows.Count} 行取消操作")
                        .ConfigureAwait(false);
                    throw;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet2 行{i + 2}] {itemName} → 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }

            result.Sheet2Count = sheet2Success;
            await writer.WriteLineAsync($"Sheet2 完成: {sheet2Success}/{sheet2Rows.Count}").ConfigureAwait(false);

            // 8. 保存导入成功记录到数据库
            var log = new PermissionImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = PermissionImportLog.StatusSuccess,
                Sheet1Count = sheet1Success,
                Sheet2Count = sheet2Success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            // 记录操作日志
            await _operationLogService.LogAsync("Import", "PermissionImportLog", log.Id,
                $"权限配置导入: 配置{sheet1Success}条, 详细权限{sheet2Success}条").ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = overallTotal,
                PhaseTotal = overallTotal,
                OverallCurrent = overallTotal,
                OverallTotal = overallTotal,
                ItemName = $"权限配置{sheet1Success}条, 详细权限{sheet2Success}条"
            });
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "导入已被用户取消";
            await writer.WriteLineAsync("[取消] 导入已被用户取消").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, "用户取消操作", result).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, $"{ex.Message}\n{ex.StackTrace}", result)
                .ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("权限配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"权限配置: {result.Sheet1Count}/{result.Sheet1Total}")
                .ConfigureAwait(false);
            await writer.WriteLineAsync($"详细权限: {result.Sheet2Count}/{result.Sheet2Total}")
                .ConfigureAwait(false);
            await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
            writer.Close();
        }

        return result;
    }

    // ==================== 历史记录查询 ====================

    public async Task<(List<PermissionImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.PermissionImportLogs.AsQueryable();

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

    public async Task<PermissionImportLog?> GetLogByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.PermissionImportLogs.FindAsync(id).ConfigureAwait(false);
    }

    // ==================== AML 构造（占位方法 — 由调用方自行维护） ====================

    /// <summary>
    /// 构造 Sheet1「权限配置」的 AML 语句
    ///
    /// 列映射（用户可根据需要修改 GetValueOrDefault 的列号）:
    /// Col 1 (idx 1): 权限配置名称 → source_id（ItemType name）
    /// Col 2 (idx 2): 权限名称 → &lt;name&gt;
    ///
    /// TODO: 由调用方自行维护此 AML 语句
    /// </summary>
    private static string BuildSheet1Aml(Dictionary<int, string> row, string importMode)
    {
        var sourceName = row.GetValueOrDefault(1, "");   // 权限配置名称
        var permName = row.GetValueOrDefault(2, "");      // 权限名称

        var action = importMode == "新增" ? "add" : "merge";

        // TODO: 调用方自行维护此 AML 语句
        return $"<AML>" +
               $"  <Item type='Permission' action='{action}'>" +
               $"      <source_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{sourceName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <name>{permName}</name>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构造 Sheet2「详细权限」的 AML 语句
    ///
    /// 列映射（用户可根据需要修改 GetValueOrDefault 的列号）:
    /// Col 1 (idx 1): 权限名称 → 关联 Sheet1 的权限名称
    /// Col 2 (idx 2): 所属角色 → &lt;role_id&gt;（Identity name）
    /// Col 3 (idx 3): 允许创建 → &lt;can_add&gt; (0/1)
    /// Col 4 (idx 4): 允许读取 → &lt;can_get&gt; (0/1)
    /// Col 5 (idx 5): 允许更新 → &lt;can_update&gt; (0/1)
    /// Col 6 (idx 6): 允许删除 → &lt;can_delete&gt; (0/1)
    ///
    /// TODO: 由调用方自行维护此 AML 语句
    /// </summary>
    private static string BuildSheet2Aml(Dictionary<int, string> row, string importMode)
    {
        var permName = row.GetValueOrDefault(1, "");      // 权限名称
        var roleName = row.GetValueOrDefault(2, "");      // 所属角色
        var canAdd = row.GetValueOrDefault(3, "");        // 允许创建
        var canGet = row.GetValueOrDefault(4, "");        // 允许读取
        var canUpdate = row.GetValueOrDefault(5, "");     // 允许更新
        var canDelete = row.GetValueOrDefault(6, "");     // 允许删除

        var action = importMode == "新增" ? "add" : "merge";

        // TODO: 调用方自行维护此 AML 语句
        return $"<AML>" +
               $"  <Item type='Permission' action='{action}'>" +
               $"      <name>{permName}</name>" +
               $"      <role_id>" +
               $"          <Item type='Identity' action='get' select='id'>" +
               $"              <name>{roleName}</name>" +
               $"          </Item>" +
               $"      </role_id>" +
               $"      <can_add>{canAdd}</can_add>" +
               $"      <can_get>{canGet}</can_get>" +
               $"      <can_update>{canUpdate}</can_update>" +
               $"      <can_delete>{canDelete}</can_delete>" +
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

    private async Task SaveLogAsync(PermissionImportLog log)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.PermissionImportLogs.Add(log);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task TrySaveFailedLogAsync(string relativePath, string errorDetail,
        PermissionImportResult result)
    {
        try
        {
            var log = new PermissionImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = PermissionImportLog.StatusFailed,
                ErrorLog = errorDetail,
                Sheet1Count = result.Sheet1Count,
                Sheet2Count = result.Sheet2Count,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
        }
        catch
        {
            // 日志保存失败不阻断主流程
        }
    }
}
