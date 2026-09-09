using System.Data;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1); // StreamWriter 异步线程安全锁
    private static readonly Regex ColumnPlaceholder = new(@"@(?<column>[A-Z]+)", RegexOptions.CultureInvariant);

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
                    var val = worksheet.Cells[r, m.Index + 1].Text ?? "";
                    row[m.Header] = val;
                    if (!string.IsNullOrWhiteSpace(val)) hasData = true;
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
        return RenderAml(ParseAmlTemplate(amlTemplate), rowData);
    }

    private static XDocument ParseAmlTemplate(string amlTemplate)
    {
        // 兼容旧模板在标签之间保存的文字换行符，不改写属性/文本中的文件路径。
        amlTemplate = Regex.Replace(amlTemplate, @"(^|>)(?<space>(?:\s|\\[nrt])+)(?=<|$)", match =>
            match.Groups[1].Value + match.Groups["space"].Value
                .Replace(@"\n", "\n").Replace(@"\r", "\r").Replace(@"\t", "\t"));
        using var reader = XmlReader.Create(new StringReader(amlTemplate), new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private static string RenderAml(XDocument template, Dictionary<string, string> rowData)
    {
        // 只替换数据节点，每行使用独立副本；单元格内容不能变成 AML 结构。
        var document = new XDocument(template);
        string ReplaceValue(string value) => ColumnPlaceholder.Replace(value, match =>
        {
            var column = match.Groups["column"].Value;
            if (!rowData.TryGetValue(column, out var cellValue))
                throw new ArgumentException($"AML 占位符 @{column} 不在所选 Excel 列范围内。");
            // XML 1.0 不允许的控制字符不能通过实体引用变合法，保留错误供逐行定位。
            return XmlConvert.VerifyXmlChars(cellValue ?? string.Empty);
        });

        foreach (var attribute in document.Descendants().Attributes().Where(a => !a.IsNamespaceDeclaration))
            attribute.Value = ReplaceValue(attribute.Value);
        foreach (var node in document.DescendantNodes().OfType<XText>().ToList())
        {
            var replaced = ReplaceValue(node.Value);
            if (node is XCData && replaced != node.Value)
                node.ReplaceWith(new XText(replaced)); // 数据中含 ]]> 时也能安全序列化。
            else
                node.Value = replaced;
        }

        var output = new StringBuilder();
        using (var writer = XmlWriter.Create(output, new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Entitize
        }))
            document.WriteTo(writer);
        return output.ToString();
    }

    public string PreviewAml(string amlTemplate, Dictionary<string, string> firstRowData)
    {
        return ReplaceAmlPlaceholders(amlTemplate, firstRowData);
    }

    /// <summary>
    /// 执行导入 — 支持并发、取消；进度回调可等待暂停信号。
    /// </summary>
    public Task<ImportResult> ExecuteImportAsync(
        string filePath, string? sheetName,
        int startRow, int endRow, int startCol, int endCol,
        string amlContent,
        int maxConcurrency = 1,
        CancellationToken cancellationToken = default,
        Func<int, int, Task>? progressCallback = null)
    {
        // Excel 解包、连接初始化及同步 IOM 请求全部在后台执行。
        return Task.Run(() => ExecuteImportCoreAsync(filePath, sheetName, startRow, endRow,
            startCol, endCol, amlContent, maxConcurrency, cancellationToken, progressCallback));
    }

    private async Task<ImportResult> ExecuteImportCoreAsync(
        string filePath, string? sheetName, int startRow, int endRow, int startCol, int endCol,
        string amlContent, int maxConcurrency, CancellationToken cancellationToken,
        Func<int, int, Task>? progressCallback)
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
        var logFile = Path.Combine(logDir, $"import_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid():N}.log");
        result.LogFilePath = logFile;

        using var writer = new StreamWriter(logFile, false) { AutoFlush = true };
        await writer.WriteLineAsync("===== 数据导入日志 =====");
        await writer.WriteLineAsync("文件: " + Path.GetFileName(filePath));
        await writer.WriteLineAsync("Sheet: " + (sheetName ?? "N/A"));
        await writer.WriteLineAsync("开始时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await writer.WriteLineAsync("范围: 行" + startRow + "~" + endRow + ", 列" + startCol + "~" + endCol);
        await writer.WriteLineAsync("并发线程数: " + maxConcurrency);
        await writer.WriteLineAsync("状态\t信息\tExcel行号\t失败行号");

        int processed = 0, success = 0, failure = 0, skipped = 0;
        var failedRows = new ConcurrentBag<int>();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(amlContent))
                throw new ArgumentException("请先填写 AML 模板。");
            if (startRow < 1 || startCol < 1 || (endRow != -1 && endRow < startRow)
                || (endCol != -1 && endCol < startCol))
                throw new ArgumentException("行列范围无效，起始值必须大于 0，结束值必须为 -1 或不小于起始值。");
            maxConcurrency = Math.Clamp(maxConcurrency, 1, 10);
            var amlTemplate = ParseAmlTemplate(amlContent);
            // 复用登录时持久化的 Innovator，不能每一行再次 Login。
            var currentInnovator = _connectionService.TypedInnovator
                ?? throw new InvalidOperationException("尚未连接 Aras，请先登录目标数据库。");
            await WriteImportLogAsync(writer, "信息", "数据库: " + _connectionService.CurrentConnection?.Database);

            // 连接池懒初始化 — 仅在多线程导入时按需创建
            if (maxConcurrency > 1)
            {
                // 每次从当前连接重建，避免切换数据库后继续使用旧池。
                await _connectionPool.ReinitializeAsync(maxConcurrency);
                if (_connectionPool.PoolSize < maxConcurrency)
                {
                    await WriteImportLogAsync(writer, "警告", "连接池初始化失败，回退为单线程");
                    maxConcurrency = 1;
                }
            }

            using var package = new ExcelPackage(new FileInfo(filePath), true);
            var worksheet = sheetName != null ? package.Workbook.Worksheets[sheetName] : package.Workbook.Worksheets[0];
            if (worksheet?.Dimension == null)
            {
                throw new InvalidOperationException("工作表不存在或无数据。");
            }

            int maxCol = endCol == -1 ? worksheet.Dimension.End.Column : Math.Min(endCol, worksheet.Dimension.End.Column);
            int maxRow = endRow == -1 ? worksheet.Dimension.End.Row : Math.Min(endRow, worksheet.Dimension.End.Row);
            if (startRow > maxRow || startCol > maxCol)
                throw new ArgumentException("所选范围内没有数据。");
            result.TotalRows = maxRow - startRow + 1;

            // 先串行收集所有行数据到 List（Excel 读取串行更安全）
            var colMap = new Dictionary<string, int>();
            for (int c = startCol; c <= maxCol; c++)
                colMap[ColumnIndexToLetter(c - 1)] = c;

            var rows = new List<(int rowNum, Dictionary<string, string> rowData)>();
            for (int r = startRow; r <= maxRow; r++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rowData = new Dictionary<string, string>();
                foreach (var kv in colMap)
                    rowData[kv.Key] = worksheet.Cells[r, kv.Value].Text ?? "";
                rows.Add((r, rowData));
            }

            // 并行选项
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxConcurrency,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(rows, parallelOptions, async (item, ct) =>
            {
                // 取消时不再启动新请求。
                ct.ThrowIfCancellationRequested();

                // 从连接池租用一个独立连接（多线程时），单线程用全局连接
                PooledConnection? pooledConn = null;
                bool counted = false;
                try
                {
                    if (item.rowData.Values.All(string.IsNullOrWhiteSpace))
                    {
                        Interlocked.Increment(ref skipped);
                        counted = true;
                        await WriteImportLogAsync(writer, "跳过", "空行", item.rowNum);
                        return;
                    }
                    if (maxConcurrency > 1)
                        pooledConn = _connectionPool.Rent();
                    var innovator = pooledConn?.Innovator ?? currentInnovator;

                    // 替换占位符并执行 AML（同步 HTTP 调用）
                    var replacedAml = RenderAml(amlTemplate, item.rowData);
                    var resultItem = innovator.applyAML(replacedAml);

                    if (!resultItem.isError())
                    {
                        Interlocked.Increment(ref success);
                        counted = true;
                        await WriteImportLogAsync(writer, "成功", resultItem.getID(), item.rowNum);
                        try
                        {
                            await _operationLogService.LogAsync("Import", "DataImport", resultItem.getID(),
                                $"数据汇入: {Path.GetFileName(filePath)} / {sheetName} / 行{item.rowNum}");
                        }
                        catch (Exception ex)
                        {
                            await _errorLogService.LogErrorAsync("数据导入-操作日志", DescribeException(ex), ErrorLog.LevelP1, ex.ToString());
                        }
                    }
                    else
                    {
                        Interlocked.Increment(ref failure);
                        counted = true;
                        failedRows.Add(item.rowNum);
                        var errMsg = resultItem.getErrorString();
                        if (string.IsNullOrWhiteSpace(errMsg)) errMsg = resultItem.ToString();
                        await WriteImportLogAsync(writer, "失败", errMsg, item.rowNum, isFailure: true);
                        await _errorLogService.LogErrorAsync($"数据导入-行{item.rowNum}", errMsg, ErrorLog.LevelP1);
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    if (!counted)
                    {
                        Interlocked.Increment(ref failure);
                        failedRows.Add(item.rowNum);
                        await WriteImportLogAsync(writer, "失败", "请求被取消，服务端结果未确认", item.rowNum, isFailure: true);
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    // 已收到服务端结果后日志异常不能改变该行的成功/失败计数。
                    if (counted) throw;
                    Interlocked.Increment(ref failure);
                    failedRows.Add(item.rowNum);
                    await WriteImportLogAsync(writer, "失败", ex.ToString(), item.rowNum, isFailure: true);
                    await _errorLogService.LogErrorAsync($"数据导入-行{item.rowNum}", DescribeException(ex), ErrorLog.LevelP1, ex.ToString());
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

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            result.IsCancelled = true;
            await WriteImportLogAsync(writer, "取消", "已处理: " + processed + "/" + result.TotalRows);
        }
        catch (Exception ex)
        {
            result.ErrorMessage = DescribeException(ex);
            await WriteImportLogAsync(writer, "错误", "导入过程异常: " + ex);
            await _errorLogService.LogErrorAsync("数据导入-执行", result.ErrorMessage, ErrorLog.LevelP1, ex.ToString());
        }
        finally
        {
            // 中断也必须保留已提交数据的计数，避免误报 0 成功/0 失败。
            result.SuccessCount = success;
            result.FailureCount = failure;
            result.SkippedCount = skipped;
            result.ProcessedRows = processed;
            result.FailedRowNumbers = failedRows.Distinct().OrderBy(row => row).ToList();
        }

        await writer.WriteLineAsync("-----");
        await writer.WriteLineAsync("结束时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        await writer.WriteLineAsync("总计: " + result.TotalRows + "  成功: " + result.SuccessCount + "  失败: " + result.FailureCount + "  跳过: " + result.SkippedCount);
        await writer.WriteLineAsync("失败行号汇总（Excel原始行号）: " + (result.FailedRowNumbers.Count == 0
            ? "无" : string.Join(",", result.FailedRowNumbers)));
        await writer.WriteLineAsync("===== 日志结束 =====");

        return result;
    }

    private static string DescribeException(Exception ex) => string.IsNullOrWhiteSpace(ex.Message)
        ? $"{ex.GetType().Name} (0x{ex.HResult:X8})" : ex.Message;

    private async Task WriteImportLogAsync(StreamWriter writer, string status, string message,
        int? excelRow = null, bool isFailure = false)
    {
        // 每条记录固定四列，换行/制表符转为可见文字，避免错误详情挤走最后的行号列。
        var singleLineMessage = message.Replace("\\", "\\\\").Replace("\r", @"\r")
            .Replace("\n", @"\n").Replace("\t", @"\t");
        await _writeSemaphore.WaitAsync();
        try
        {
            await writer.WriteLineAsync($"[{status}]\t{singleLineMessage}\t{excelRow}\t{(isFailure ? excelRow?.ToString() : string.Empty)}");
        }
        finally { _writeSemaphore.Release(); }
    }

    private static string SanitizeHeader(string rawHeader)
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
