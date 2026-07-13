using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 数据库导出服务 — SQL查询+Excel导出+日志记录
/// SqlDataReader.GetName(i) 自动返回AS别名作为Excel列标题
/// </summary>
public class DatabaseExportService : IDatabaseExportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ExportBaseDir = "Config/DatabaseExports";

    public DatabaseExportService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<DatabaseExportResult> ExportAsync(
        string outputDir, string exportMode, int batchSize,
        string sqlQuery, string connectionString,
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new DatabaseExportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dateDir = Path.Combine(baseDir, ExportBaseDir, dateFolder);
        var logsDir = Path.Combine(dateDir, "logs");
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(outputDir);

        var timestamp = DateTime.Now.ToString("HHmmss");
        var logFile = Path.Combine(logsDir, $"export_{timestamp}.log");
        result.LogFilePath = logFile;

        using var writer = new StreamWriter(logFile, false);
        var startTime = DateTime.Now;

        try
        {
            await writer.WriteLineAsync("===== 数据库导出日志 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"开始时间: {startTime:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"导出模式: {exportMode}").ConfigureAwait(false);
            if (exportMode == DatabaseExportLog.ModeBatch)
                await writer.WriteLineAsync($"每批条数: {batchSize}").ConfigureAwait(false);
            await writer.WriteLineAsync($"SQL: {sqlQuery}").ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            progress?.Report(new ImportProgressInfo
            {
                Phase = "连接数据库", Current = 0,
                ItemName = "正在连接目标数据库..."
            });

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using var command = new SqlCommand(sqlQuery, connection)
            {
                CommandTimeout = 600
            };

            progress?.Report(new ImportProgressInfo
            {
                Phase = "执行查询", Current = 0,
                ItemName = "正在执行SQL查询..."
            });

            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            // 列标题 — SqlDataReader.GetName(i) 自动返回AS别名
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columnNames.Add(reader.GetName(i));

            await writer.WriteLineAsync($"列数: {columnNames.Count}").ConfigureAwait(false);
            await writer.WriteLineAsync($"列名: {string.Join(", ", columnNames)}").ConfigureAwait(false);

            // 读取所有行
            var allRows = new List<List<object?>>();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var row = new List<object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
                allRows.Add(row);

                if (allRows.Count % 1000 == 0)
                {
                    progress?.Report(new ImportProgressInfo
                    {
                        Phase = "读取数据", Current = allRows.Count,
                        ItemName = $"已读取 {allRows.Count} 行..."
                    });
                }
            }

            result.TotalRows = allRows.Count;
            await writer.WriteLineAsync($"总行数: {allRows.Count}").ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // 生成Excel
            var filePrefix = $"export_{DateTime.Now:yyyyMMdd}_{timestamp}";

            if (exportMode == DatabaseExportLog.ModeSingle || allRows.Count <= batchSize)
            {
                var filePath = Path.Combine(outputDir, $"{filePrefix}.xlsx");
                await WriteExcelFileAsync(filePath, columnNames, allRows).ConfigureAwait(false);
                result.ExportedFiles.Add(filePath);
                result.ExportedRows = allRows.Count;
                result.BatchCount = 1;
                await writer.WriteLineAsync($"[OK] {Path.GetFileName(filePath)} ({allRows.Count}行)").ConfigureAwait(false);
            }
            else
            {
                int batchIndex = 0;
                for (int i = 0; i < allRows.Count; i += batchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    batchIndex++;
                    var batch = allRows.Skip(i).Take(batchSize).ToList();
                    var filePath = Path.Combine(outputDir, $"{filePrefix}_part{batchIndex:D3}.xlsx");
                    await WriteExcelFileAsync(filePath, columnNames, batch).ConfigureAwait(false);
                    result.ExportedFiles.Add(filePath);
                    result.ExportedRows += batch.Count;
                    await writer.WriteLineAsync($"[OK] {Path.GetFileName(filePath)} ({batch.Count}行)").ConfigureAwait(false);

                    progress?.Report(new ImportProgressInfo
                    {
                        Phase = $"导出批次 {batchIndex}",
                        Current = Math.Min(i + batchSize, allRows.Count),
                        PhaseTotal = allRows.Count,
                        OverallCurrent = Math.Min(i + batchSize, allRows.Count),
                        OverallTotal = allRows.Count,
                        ItemName = $"已导出 {Math.Min(i + batchSize, allRows.Count)}/{allRows.Count} 行"
                    });
                }
                result.BatchCount = batchIndex;
            }

            result.IsSuccess = true;
            var elapsed = DateTime.Now - startTime;
            await writer.WriteLineAsync($"===== 导出完成 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"耗时: {elapsed.TotalSeconds:F1}秒").ConfigureAwait(false);
            await writer.WriteLineAsync($"导出: {result.ExportedRows}行 → {result.BatchCount}个文件").ConfigureAwait(false);

            // 每个文件保存一条日志记录
            var logId = Guid.NewGuid().ToString("N")[..12];
            int totalRows = result.TotalRows;
            int fileIndex = 0;
            foreach (var filePath in result.ExportedFiles)
            {
                fileIndex++;
                var log = new DatabaseExportLog
                {
                    ConnectionString = MaskConnectionString(connectionString),
                    SqlQuery = sqlQuery,
                    ExportMode = exportMode,
                    BatchSize = batchSize,
                    TotalRows = totalRows,
                    ExportTime = startTime,
                    FilePath = filePath,
                    FileIndex = fileIndex,
                    FileCount = result.BatchCount,
                    Status = DatabaseExportLog.StatusSuccess,
                    UserId = CurrentUserContext.CurrentUserId ?? "system",
                    CreatorOn = DateTime.Now
                };
                await SaveLogAsync(log).ConfigureAwait(false);
            }

            await _operationLogService.LogAsync("Export", "DatabaseExport", logId,
                $"数据库导出: {result.TotalRows}行SQL结果 → {result.BatchCount}个Excel文件").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成", Current = result.TotalRows,
                PhaseTotal = result.TotalRows, OverallCurrent = result.TotalRows,
                OverallTotal = result.TotalRows, ItemName = $"导出完成: {result.ExportedRows}行"
            });
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "导出已被用户取消";
            await writer.WriteLineAsync("[取消] 导出已被用户取消").ConfigureAwait(false);
            await TrySaveFailedLogAsync(connectionString, sqlQuery, exportMode,
                batchSize, result.TotalRows, "用户取消导出").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}").ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("数据库导出-导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            await TrySaveFailedLogAsync(connectionString, sqlQuery, exportMode,
                batchSize, result.TotalRows, $"{ex.Message}\n{ex.StackTrace}").ConfigureAwait(false);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
            writer.Close();
        }

        return result;
    }

    public async Task<(List<DatabaseExportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.DatabaseExportLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(r => r.UserId == userId);

        var total = await query.CountAsync().ConfigureAwait(false);
        var items = await query.OrderByDescending(r => r.CreatorOn)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync().ConfigureAwait(false);
        return (items, total);
    }

    public async Task<DatabaseExportLog?> GetLogByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.DatabaseExportLogs.FindAsync(id).ConfigureAwait(false);
    }

    // ==================== private helpers ====================

    private static async Task WriteExcelFileAsync(
        string filePath, List<string> columnNames, List<List<object?>> rows)
    {
        await Task.Run(() =>
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("导出数据");

            for (int i = 0; i < columnNames.Count; i++)
            {
                var cell = ws.Cells[1, i + 1];
                cell.Value = columnNames[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(
                    System.Drawing.Color.FromArgb(253, 235, 208));
            }

            for (int r = 0; r < rows.Count; r++)
                for (int c = 0; c < columnNames.Count; c++)
                    ws.Cells[r + 2, c + 1].Value =
                        c < rows[r].Count ? rows[r][c] : null;

            if (rows.Count > 0)
                ws.Cells[1, 1, rows.Count + 1, columnNames.Count].AutoFitColumns(8, 40);
            else
                ws.Cells[1, 1, 1, columnNames.Count].AutoFitColumns(8, 25);

            package.SaveAs(new FileInfo(filePath));
        });
    }

    private async Task SaveLogAsync(DatabaseExportLog log)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            db.DatabaseExportLogs.Add(log);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseExport] 保存日志失败: {ex.Message}");
        }
    }

    private async Task TrySaveFailedLogAsync(
        string cs, string sql, string mode, int batchSize, int totalRows, string error)
    {
        try
        {
            var log = new DatabaseExportLog
            {
                ConnectionString = MaskConnectionString(cs), SqlQuery = sql,
                ExportMode = mode, BatchSize = batchSize, TotalRows = totalRows,
                ExportTime = DateTime.Now, Status = DatabaseExportLog.StatusFailed,
                ErrorMessage = error,
                UserId = CurrentUserContext.CurrentUserId ?? "system", CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
        }
        catch { }
    }

    private static string MaskConnectionString(string cs)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(cs);
            if (!string.IsNullOrEmpty(builder.Password))
                builder.Password = "***";
            return builder.ConnectionString;
        }
        catch { return cs; }
    }
}
