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
/// 1. 读取 Excel 文件（Sheet「权限配置」）
/// 2. 逐行构造 AML 语句，通过 Aras API 执行
/// 3. 记录详细日志到本地文件 + 操作日志到数据库
///
/// 模板说明:
/// Sheet「权限配置」10列:
///   对象类 | 权限名称 | 权限标签 | 访问级别 | 所属角色 |
///   适用身份 | 允许创建 | 允许读取 | 允许更新 | 允许删除
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位符注入点
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
    /// 生成 Excel 模板文件（1个Sheet：权限配置，10列，含加粗表头+自适应列宽）
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        var ws = package.Workbook.Worksheets.Add("权限配置");
        var headers = new[]
        {
            "对象类",          // Col 1  → AML source_id（父 ItemType 名称）
            "权限名称",        // Col 2  → AML <name>
            "权限标签",        // Col 3  → AML <label>（简体中文）
            "访问级别",        // Col 4  → AML <access_level> (0-World/1-Authorized/2-Private)
            "所属角色",        // Col 5  → AML <role_id>（Identity name）
            "适用身份",        // Col 6  → AML <identity_id>（Identity name）
            "允许创建",        // Col 7  → AML <can_add> (0/1)
            "允许读取",        // Col 8  → AML <can_get> (0/1)
            "允许更新",        // Col 9  → AML <can_update> (0/1)
            "允许删除"         // Col 10 → AML <can_delete> (0/1)
        };
        WriteHeaders(ws, headers);
        ws.Cells[1, 1, 1, headers.Length].AutoFitColumns(8, 25);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 执行权限导入汇入到 Aras 系统
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

            // 5. 读取 Excel 数据
            progress?.Report(new ImportProgressInfo
            {
                Phase = "读取Excel",
                Current = 0,
                ItemName = "正在读取 Excel 文件..."
            });

            using var package = new ExcelPackage(new FileInfo(filePath));
            var rows = ReadSheetRows(package, "权限配置", 10);

            result.Sheet1Total = rows.Count;

            await writer.WriteLineAsync($"权限配置: {rows.Count} 行").ConfigureAwait(false);

            // 6. 处理 Sheet: 权限配置
            int success = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var row = rows[i];
                var itemName = row.GetValueOrDefault(2, "");            // 权限名称（Col 2）
                var overallIdx = i + 1;

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"权限{importMode}",
                    Current = i + 1,
                    PhaseTotal = rows.Count,
                    OverallCurrent = overallIdx,
                    OverallTotal = rows.Count,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var aml = BuildPermissionAml(row, importMode);
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
            var log = new PermissionImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = PermissionImportLog.StatusSuccess,
                Sheet1Count = success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            // 记录敏感操作日志
            await _operationLogService.LogAsync("Import", "PermissionImportLog", log.Id,
                $"权限配置导入: {success}条").ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = rows.Count,
                PhaseTotal = rows.Count,
                OverallCurrent = rows.Count,
                OverallTotal = rows.Count,
                ItemName = $"权限配置{success}条"
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
            await _errorLogService.LogErrorAsync("权限配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"权限配置: {result.Sheet1Count}/{result.Sheet1Total}")
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

    // ==================== AML 构建（占位符 — 调用方自行维护） ====================

    /// <summary>
    /// 构建权限（Permission）的 AML 语句
    ///
    /// 新增模式: action='add' — 创建全新权限
    /// 覆盖模式: action='merge' — 按名称匹配，存在则更新
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// 列映射（用户可见列号 → GetValueOrDefault 索引）:
    /// Col 1 (idx 1):  对象类 → source_id 的 ItemType name
    /// Col 2 (idx 2):  权限名称 → <name>
    /// Col 3 (idx 3):  权限标签 → <label>（简体中文）
    /// Col 4 (idx 4):  访问级别 → <access_level>
    /// Col 5 (idx 5):  所属角色 → <role_id>（Identity name）
    /// Col 6 (idx 6):  适用身份 → <identity_id>（Identity name）
    /// Col 7 (idx 7):  允许创建 → <can_add>
    /// Col 8 (idx 8):  允许读取 → <can_get>
    /// Col 9 (idx 9):  允许更新 → <can_update>
    /// Col 10 (idx 10): 允许删除 → <can_delete>
    /// </summary>
    private static string BuildPermissionAml(Dictionary<int, string> row, string importMode)
    {
        var sourceName = row.GetValueOrDefault(1, "");       // 对象类
        var permName = row.GetValueOrDefault(2, "");          // 权限名称
        var label = row.GetValueOrDefault(3, "");             // 权限标签
        var accessLevel = row.GetValueOrDefault(4, "");       // 访问级别
        var roleName = row.GetValueOrDefault(5, "");          // 所属角色
        var identityName = row.GetValueOrDefault(6, "");      // 适用身份
        var canAdd = row.GetValueOrDefault(7, "");            // 允许创建
        var canGet = row.GetValueOrDefault(8, "");            // 允许读取
        var canUpdate = row.GetValueOrDefault(9, "");         // 允许更新
        var canDelete = row.GetValueOrDefault(10, "");        // 允许删除

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Permission' action='add'>" +
                   $"      <source_id>" +
                   $"          <Item type='ItemType' action='get' select='id'>" +
                   $"              <name>{sourceName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"      <name>{permName}</name>" +
                   $"      <label>{label}</label>" +
                   $"      <access_level>{accessLevel}</access_level>" +
                   $"      <role_id>" +
                   $"          <Item type='Identity' action='get' select='id'>" +
                   $"              <name>{roleName}</name>" +
                   $"          </Item>" +
                   $"      </role_id>" +
                   $"      <identity_id>" +
                   $"          <Item type='Identity' action='get' select='id'>" +
                   $"              <name>{identityName}</name>" +
                   $"          </Item>" +
                   $"      </identity_id>" +
                   $"      <can_add>{canAdd}</can_add>" +
                   $"      <can_get>{canGet}</can_get>" +
                   $"      <can_update>{canUpdate}</can_update>" +
                   $"      <can_delete>{canDelete}</can_delete>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        return $"<AML>" +
               $"  <Item type='Permission' action='merge' where=\"Permission.name='{permName}'\">" +
               $"      <source_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{sourceName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <name>{permName}</name>" +
               $"      <label>{label}</label>" +
               $"      <access_level>{accessLevel}</access_level>" +
               $"      <role_id>" +
               $"          <Item type='Identity' action='get' select='id'>" +
               $"              <name>{roleName}</name>" +
               $"          </Item>" +
               $"      </role_id>" +
               $"      <identity_id>" +
               $"          <Item type='Identity' action='get' select='id'>" +
               $"              <name>{identityName}</name>" +
               $"          </Item>" +
               $"      </identity_id>" +
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
