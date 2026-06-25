using System.Data;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Aras.IOM;

namespace ArasToolkit.Services.Services;

public class DataImportService : IDataImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
    private readonly IErrorLogService _errorLogService;
    private readonly IOperationLogService _operationLogService;
    private readonly ArasConnectionService _connectionService;
    private readonly ArasConnectionPool _connectionPool;

    public DataImportService(
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IErrorLogService errorLogService,
        IOperationLogService operationLogService,
        ArasConnectionService connectionService,
        ArasConnectionPool connectionPool)
    {
        _contextFactory = contextFactory;
        _errorLogService = errorLogService;
        _operationLogService = operationLogService;
        _connectionService = connectionService;
        _connectionPool = connectionPool;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<DataImportConfig>> GetConfigsAsync()
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Set<DataImportConfig>().AsQueryable();
        if (!CurrentUserContext.IsAdmin)
            query = query.Where(c => c.UserId == CurrentUserContext.CurrentUserId);
        return await query.OrderByDescending(c => c.CreatorOn).ToListAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据导入-获取配置列表", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }
    /// <summary>
    /// 保存AML模板到数据库并记录保存日志
    /// </summary>
    /// <param name="config">传入的实体对象</param>
    /// <returns></returns>
    public async Task<DataImportConfig> SaveConfigAsync(DataImportConfig config)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            config.UserId = CurrentUserContext.CurrentUserId;
            config.CreatorOn = DateTime.Now;

            bool isNew = string.IsNullOrEmpty(config.Id) || config.Id.Length < 12;
            if (isNew)
            {
                config.Id = Guid.NewGuid().ToString("N")[..12];
                context.Set<DataImportConfig>().Add(config);
            }
            else
            {
                var existing = await context.Set<DataImportConfig>().FindAsync(config.Id);
                if (existing != null)
                {
                    existing.ConfigName = config.ConfigName;
                    existing.AmlContent = config.AmlContent;
                }
                else
                {
                    // 如果调用方传入了 Id 但数据库中不存在，作为新记录插入
                    context.Set<DataImportConfig>().Add(config);
                    isNew = true;
                }
            }
            var z = await context.SaveChangesAsync();
            await _operationLogService.LogAsync(isNew ? "Create" : "Update", "DataImportConfig", config.Id,
                "保存数据导入配置: " + config.ConfigName);
            return config;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据导入-保存配置", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task DeleteConfigAsync(string id)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var config = await context.Set<DataImportConfig>().FindAsync(id);
            if (config != null)
            {
                var configName = config.ConfigName;
                context.Set<DataImportConfig>().Remove(config);
                await context.SaveChangesAsync();
                await _operationLogService.LogAsync("Delete", "DataImportConfig", id,
                    "删除数据导入配置: " + configName);
            }
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据导入-删除配置", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    public Task<List<string>> GetSheetNamesAsync(string filePath)
    {
        return Task.Run(() =>
        {
            using var package = new ExcelPackage(new FileInfo(filePath), true);
            return package.Workbook.Worksheets.Select(s => s.Name).ToList();
        });
    }
   /// <summary>
   /// 
   /// </summary>
   /// <param name="filePath"></param>
   /// <param name="sheetName"></param>
   /// <param name="startRow"></param>
   /// <param name="endRow"></param>
   /// <param name="startCol"></param>
   /// <param name="endCol"></param>
   /// <returns></returns>
    public Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
        int startRow, int endRow, int startCol, int endCol)
    {
        return Task.Run(() =>
        {
            var result = new ExcelSheetData { SheetName = sheetName };
            using var package = new ExcelPackage(new FileInfo(filePath), true);
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet?.Dimension == null) return result;

            int maxRow = worksheet.Dimension.Rows;
            int maxCol = worksheet.Dimension.Columns;
            if (endRow == -1 || endRow > maxRow) endRow = maxRow;
            if (endCol == -1 || endCol > maxCol) endCol = maxCol;

            var usedHeaders = new HashSet<string>();
            for (int c = startCol; c <= endCol; c++)
            {
                var letter = ColumnIndexToLetter(c - 1);
                var rawHeader = SanitizeHeader(worksheet.Cells[1, c].Text?.Trim() ?? "");
                if (string.IsNullOrEmpty(rawHeader))
                    rawHeader = "col_" + letter;
                var uniqueHeader = rawHeader;
                int suffix = 2;
                while (usedHeaders.Contains(uniqueHeader))
                {
                    uniqueHeader = rawHeader + "_" + suffix;
                    suffix++;
                }
                usedHeaders.Add(uniqueHeader);

                result.ColumnMappings.Add(new ColumnMapping
                {
                    Letter = letter,
                    Header = uniqueHeader,
                    Index = c - 1
                });
            }

            result.Data = new DataTable();
            foreach (var m in result.ColumnMappings)
                result.Data.Columns.Add(m.Header);
            foreach (var m in result.ColumnMappings)
                result.ColumnHeaders.Add(m.Header);

            int previewEndRow = Math.Min(endRow, startRow + 29);
            for (int r = startRow; r <= previewEndRow; r++)
            {
                var row = result.Data.NewRow();
                bool hasData = false;
                foreach (var m in result.ColumnMappings)
                {
                    var val = worksheet.Cells[r, m.Index + 1].Text?.Trim() ?? "";
                    row[m.Header] = val;
                    if (!string.IsNullOrEmpty(val)) hasData = true;
                }
                if (hasData) result.Data.Rows.Add(row);
            }
            return result;
        });
    }

    public Task<List<ColumnMapping>> GetColumnMappingsAsync(string filePath, string sheetName,
        int startCol, int endCol)
    {
        return Task.Run(() =>
        {
            var result = new List<ColumnMapping>();
            using var package = new ExcelPackage(new FileInfo(filePath), true);
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet?.Dimension == null) return result;

            int maxCol = worksheet.Dimension.Columns;
            if (endCol == -1 || endCol > maxCol) endCol = maxCol;

            for (int c = startCol; c <= endCol; c++)
            {
                var letter = ColumnIndexToLetter(c - 1);
                var header = SanitizeHeader(worksheet.Cells[1, c].Text?.Trim() ?? "");
                result.Add(new ColumnMapping { Letter = letter, Header = header, Index = c - 1 });
            }
            return result;
        });
    }

   public string ReplaceAmlPlaceholders(string amlTemplate, Dictionary<string, string> rowData)
   {
       // 反转义AML模板中的 \n \t \r（文字反斜杠序列 → 实际控制字符）
       amlTemplate = amlTemplate.Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\r", "\r");
       var result = amlTemplate;
       foreach (var kv in rowData)
           result = result.Replace("@" + kv.Key, kv.Value);
        return result;
    }

    public string PreviewAml(string amlTemplate, Dictionary<string, string> firstRowData)
    {
        return ReplaceAmlPlaceholders(amlTemplate, firstRowData);
    }

    /// <summary>
    /// 执行导入 — 支持多线程并发 + 暂停/继续（通过 CancellationToken）
    /// </summary>
    public async Task<ImportResult> ExecuteImportAsync(
        string filePath, string? sheetName,
        int startRow, int endRow, int startCol, int endCol,
        string amlContent,
        int maxConcurrency = 1,
        CancellationToken cancellationToken = default,
        Func<int, int, Task>? progressCallback = null)
    {
        var result = new ImportResult
        {
            ImportTime = DateTime.Now,
            TotalRows = 0,
            SuccessCount = 0,
            FailureCount = 0,
            SkippedCount = 0
        };

        var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Import");
        Directory.CreateDirectory(logDir);
        var logFile = Path.Combine(logDir, $"import_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        result.LogFilePath = logFile;

        using var writer = new StreamWriter(logFile, false);
        await writer.WriteLineAsync("===== 数据导入日志 =====");
        await writer.WriteLineAsync("文件: " + Path.GetFileName(filePath));
        await writer.WriteLineAsync("Sheet: " + (sheetName ?? "N/A"));
        await writer.WriteLineAsync("开始时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await writer.WriteLineAsync("范围: 行" + startRow + "~" + endRow + ", 列" + startCol + "~" + endCol);
        await writer.WriteLineAsync("并发线程数: " + maxConcurrency);

        try
        {
            // 连接池大小验证
            if (maxConcurrency > 1 && _connectionPool.PoolSize < maxConcurrency)
            {
                await writer.WriteLineAsync("[警告] 连接池大小(" + _connectionPool.PoolSize + ")不足，回退为单线程");
                maxConcurrency = 1;
            }

            using var package = new ExcelPackage(new FileInfo(filePath), true);
            var worksheet = sheetName != null ? package.Workbook.Worksheets[sheetName] : package.Workbook.Worksheets[0];
            if (worksheet?.Dimension == null)
            {
                await writer.WriteLineAsync("[错误] 工作表无数据");
                return result;
            }

            int maxCol = endCol == -1 ? worksheet.Dimension.Columns : Math.Min(endCol, worksheet.Dimension.Columns);
            int maxRow = endRow == -1 ? worksheet.Dimension.Rows : Math.Min(endRow, worksheet.Dimension.Rows);
            result.TotalRows = maxRow - startRow + 1;

            // 先串行收集所有行数据到 List（Excel 读取串行更安全）
            var colMap = new Dictionary<string, int>();
            for (int c = startCol; c <= maxCol; c++)
                colMap[ColumnIndexToLetter(c - 1)] = c;

            var rows = new List<(int rowNum, Dictionary<string, string> rowData)>();
            for (int r = startRow; r <= maxRow; r++)
            {
                var rowData = new Dictionary<string, string>();
                foreach (var kv in colMap)
                    rowData[kv.Key] = worksheet.Cells[r, kv.Value].Text?.Trim() ?? "";
                rows.Add((r, rowData));
            }

            // 并行选项
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxConcurrency,
                CancellationToken = cancellationToken
            };

            // 使用 Interlocked 保证多线程计数安全
            int processed = 0, success = 0, failure = 0;

            await Parallel.ForEachAsync(rows, parallelOptions, async (item, ct) =>
            {
                // 检查取消令牌（暂停时快速退出）
                ct.ThrowIfCancellationRequested();

                // 从连接池租用一个独立连接（多线程时），单线程用全局连接
                PooledConnection? pooledConn = null;
                Innovator? innovator;
                if (maxConcurrency > 1)
                {
                    pooledConn = _connectionPool.Rent();
                    innovator = pooledConn.Innovator;
                }
                else
                {
                    // 单线程时：从全局连接获取，使用标准登录链
                    var httpConn = _connectionService.HttpConnection as HttpServerConnection;
                    if (httpConn != null)
                    {
                        var loginResult = httpConn.Login();
                        innovator = loginResult.getInnovator();
                    }
                    else
                    {
                        innovator = null;
                    }
                }

                try
                {
                    if (innovator == null)
                    {
                        Interlocked.Increment(ref failure);
                        return;
                    }

                    // 替换占位符并执行 AML（同步 HTTP 调用）
                    var replacedAml = ReplaceAmlPlaceholders(amlContent, item.rowData);
                    var resultItem = innovator.applyAML(replacedAml);

                    if (!resultItem.isError())
                    {
                        Interlocked.Increment(ref success);
                    }
                    else
                    {
                        Interlocked.Increment(ref failure);
                        var errMsg = resultItem.getErrorString();
                        await writer.WriteLineAsync("[失败] 行" + item.rowNum + ": " + errMsg);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw; // 重新抛出，让 Parallel.ForEachAsync 处理暂停
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failure);
                    await writer.WriteLineAsync("[失败] 行" + item.rowNum + ": " + ex.Message);
                }
                finally
                {
                    // 归还连接池
                    if (pooledConn != null)
                        _connectionPool.Return(pooledConn);

                    // 更新进度（线程安全）
                    var current = Interlocked.Increment(ref processed);
                    if (progressCallback != null)
                        await progressCallback(current, result.TotalRows);
                }
            });

            result.SuccessCount = success;
            result.FailureCount = failure;
            result.ProcessedRows = processed;
        }
        catch (OperationCanceledException)
        {
            // 暂停/取消：保留已处理数据
            await writer.WriteLineAsync("[暂停] 用户取消，已处理: " + result.ProcessedRows + "/" + result.TotalRows);
        }
        catch (Exception ex)
        {
            await writer.WriteLineAsync("[错误] 导入过程异常: " + ex.Message);
            await _errorLogService.LogErrorAsync("数据导入-执行", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }

        await writer.WriteLineAsync("-----");
        await writer.WriteLineAsync("结束时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await writer.WriteLineAsync("总计: " + result.TotalRows + "  成功: " + result.SuccessCount + "  失败: " + result.FailureCount + "  跳过: " + result.SkippedCount);
        await writer.WriteLineAsync("===== 日志结束 =====");

        return result;
    }    private static string SanitizeHeader(string rawHeader)
    {
        if (string.IsNullOrEmpty(rawHeader)) return rawHeader;
        return rawHeader
            .Replace('(', '（')
            .Replace(')', '）')
            .Replace('[', '【')
            .Replace(']', '】')
            .Replace('.', '．')
            .Replace('/', '／')
            .Replace(',', '，')
            .Replace(':', '：');
    }

    private static string ColumnIndexToLetter(int index)
    {
        string letter = "";
        while (index >= 0)
        {
            letter = (char)('A' + index % 26) + letter;
            index = index / 26 - 1;
        }
        return letter;
    }
}
