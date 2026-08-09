using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 读取 Excel 模板并以参数化 SQL 批量修改数据库。
/// 每个执行任务只创建一组短生命周期 Worker，连接禁用 ADO.NET 池化，完成后立即释放。
/// </summary>
public sealed partial class DatabaseModificationService : IDatabaseModificationService
{
    private const string TemplateScope = "[DatabaseModification]";
    private const int MaximumConcurrency = 10;
    private const int CommandTimeoutSeconds = 120;
    private const int MaximumDeadlockRetries = 3;

    private readonly ISqlTemplateService _sqlTemplateService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;
    private readonly ArasConnectionService _arasConnectionService;

    public DatabaseModificationService(
        ISqlTemplateService sqlTemplateService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService,
        ArasConnectionService arasConnectionService)
    {
        _sqlTemplateService = sqlTemplateService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
        _arasConnectionService = arasConnectionService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<string>> GetSheetNamesAsync(string filePath)
    {
        try
        {
            ValidateWorkbookPath(filePath);
            return await Task.Run(() =>
            {
                using var package = new ExcelPackage(new FileInfo(filePath), true);
                return package.Workbook.Worksheets.Select(sheet => sheet.Name).ToList();
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据库修改-读取Sheet", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<DatabaseModificationPreview> PreviewExcelAsync(
        DatabaseModificationRequest request,
        int previewRowCount = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var workbookData = await ReadWorkbookDataAsync(request, previewRowCount, cancellationToken)
                .ConfigureAwait(false);
            return BuildPreview(workbookData, includeSql: false, request.SqlTemplate);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据库修改-Excel预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<DatabaseModificationPreview> PreviewSqlAsync(
        DatabaseModificationRequest request,
        int previewRowCount = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSqlTemplate(request.SqlTemplate);
            var workbookData = await ReadWorkbookDataAsync(request, previewRowCount, cancellationToken)
                .ConfigureAwait(false);
            EnsurePlaceholderColumnsExist(request.SqlTemplate, workbookData.ColumnMappings);
            return BuildPreview(workbookData, includeSql: true, request.SqlTemplate);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据库修改-SQL预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<DatabaseModificationResult> ExecuteAsync(
        DatabaseModificationRequest request,
        string connectionString,
        IProgress<DatabaseModificationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new DatabaseModificationResult();
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Config", "DatabaseModifications", DateTime.Now.ToString("yyyy_M_d"), "logs");
        Directory.CreateDirectory(logDirectory);
        result.LogFilePath = Path.Combine(logDirectory, $"modification_{DateTime.Now:HHmmss}.log");

        await using var logWriter = new StreamWriter(result.LogFilePath, false, new UTF8Encoding(false));
        var logLock = new SemaphoreSlim(1, 1);
        var startedOn = DateTime.Now;

        try
        {
            ValidateSqlTemplate(request.SqlTemplate);
            var isArasMode = request.ExecutionMode == DatabaseModificationExecutionModes.Aras;
            if (!isArasMode && string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("尚未启用数据库导出连接配置。");
            if (isArasMode && (!_arasConnectionService.IsConnected || _arasConnectionService.TypedInnovator == null))
                throw new InvalidOperationException("Aras 方式需要先启动并登录 Aras 连接。");

            var workbookData = await ReadWorkbookDataAsync(request, int.MaxValue, cancellationToken)
                .ConfigureAwait(false);
            EnsurePlaceholderColumnsExist(request.SqlTemplate, workbookData.ColumnMappings);

            var rows = workbookData.Rows;
            result.TotalRows = rows.Count;
            result.SkippedCount = workbookData.SkippedRows;
            var concurrency = isArasMode
                ? 1
                : Math.Clamp(request.MaxConcurrency, 1, MaximumConcurrency);

            await logWriter.WriteLineAsync("===== 数据库修改模式日志 =====").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"开始时间: {startedOn:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"文件: {Path.GetFileName(request.FilePath)}").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"Sheet: {request.SheetName}").ConfigureAwait(false);
            await logWriter.WriteLineAsync(
                $"范围: 行 {request.StartRow}~{workbookData.ResolvedEndRow}, 列 {request.StartColumn}~{workbookData.ResolvedEndColumn}")
                .ConfigureAwait(false);
            await logWriter.WriteLineAsync($"并发 Worker: {concurrency}（最大 {MaximumConcurrency}）").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"执行方式: {request.ExecutionMode}").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"SQL模板: {request.SqlTemplate}").ConfigureAwait(false);

            if (rows.Count == 0)
                throw new InvalidOperationException("所选范围内没有可执行的数据行。");

            var failures = new ConcurrentQueue<string>();
            var processed = 0;
            var success = 0;
            var failure = 0;
            void ReportProcessed()
            {
                var current = Interlocked.Increment(ref processed);
                progress?.Report(new DatabaseModificationProgress
                {
                    ProcessedRows = current,
                    TotalRows = rows.Count,
                    SuccessCount = Volatile.Read(ref success),
                    FailureCount = Volatile.Read(ref failure),
                    Message = $"已处理 {current}/{rows.Count} 行"
                });
            }

            if (isArasMode)
            {
                await ExecuteArasRowsAsync(
                    rows,
                    request.SqlTemplate,
                    _arasConnectionService.TypedInnovator!,
                    logWriter,
                    logLock,
                    failures,
                    ReportProcessed,
                    () => Interlocked.Increment(ref success),
                    () => Interlocked.Increment(ref failure),
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                // Pooling=false 保证本任务结束后物理连接随 Worker 一并释放，不在服务器保留空闲池连接。
                var connectionBuilder = new SqlConnectionStringBuilder(connectionString)
                {
                    Pooling = false
                };
                var partitions = Enumerable.Range(0, concurrency)
                    .Select(workerIndex => rows.Where((_, rowIndex) => rowIndex % concurrency == workerIndex).ToList())
                    .Where(partition => partition.Count > 0)
                    .ToList();
                var workers = partitions.Select(partition => ExecuteOrmPartitionAsync(
                    partition,
                    request.SqlTemplate,
                    connectionBuilder.ConnectionString,
                    logWriter,
                    logLock,
                    failures,
                    ReportProcessed,
                    () => Interlocked.Increment(ref success),
                    () => Interlocked.Increment(ref failure),
                    cancellationToken));
                await Task.WhenAll(workers).ConfigureAwait(false);
            }

            result.ProcessedRows = processed;
            result.SuccessCount = success;
            result.FailureCount = failure;
            result.FailedDetails = failures.OrderBy(message => message, StringComparer.Ordinal).ToList();

            await logWriter.WriteLineAsync("-----").ConfigureAwait(false);
            await logWriter.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await logWriter.WriteLineAsync(
                $"总计: {result.TotalRows}，成功: {success}，失败: {failure}，跳过空行: {result.SkippedCount}")
                .ConfigureAwait(false);

            await TryLogOperationAsync("Import", "DatabaseModification", Guid.NewGuid().ToString("N")[..12],
                $"数据库修改模式（{request.ExecutionMode}）执行完成: {Path.GetFileName(request.FilePath)}，成功 {success}，失败 {failure}")
                .ConfigureAwait(false);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            result.IsCancelled = true;
            await logWriter.WriteLineAsync($"[取消] {DateTime.Now:yyyy-MM-dd HH:mm:ss} 用户取消执行").ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("数据库修改-取消", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            await logWriter.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("数据库修改-执行", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
        finally
        {
            logLock.Dispose();
        }
    }

    public async Task<List<SqlTemplate>> GetSavedTemplatesAsync(string? userId = null)
    {
        var templates = await _sqlTemplateService.GetAllAsync(userId).ConfigureAwait(false);
        return templates
            .Where(template => template.Description?.StartsWith(TemplateScope, StringComparison.Ordinal) == true)
            .OrderByDescending(template => template.CreatorOn)
            .ToList();
    }

    public Task SaveTemplateAsync(SqlTemplate template)
    {
        ValidateSqlTemplate(template.SqlContent);
        template.Description = TemplateScope;
        return _sqlTemplateService.SaveAsync(template);
    }

    public Task DeleteTemplateAsync(string id) => _sqlTemplateService.DeleteAsync(id);

    private async Task ExecuteOrmPartitionAsync(
        IReadOnlyCollection<WorkbookRow> rows,
        string sqlTemplate,
        string connectionString,
        StreamWriter logWriter,
        SemaphoreSlim logLock,
        ConcurrentQueue<string> failures,
        Action reportProcessed,
        Action reportSuccess,
        Action reportFailure,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using (var sessionCommand = connection.CreateCommand())
        {
            sessionCommand.CommandText = "SET DEADLOCK_PRIORITY LOW; SET LOCK_TIMEOUT 30000;";
            sessionCommand.CommandTimeout = 30;
            await sessionCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await ExecuteRowWithRetryAsync(connection, sqlTemplate, row, cancellationToken)
                    .ConfigureAwait(false);
                reportSuccess();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                reportFailure();
                var detail = $"行 {row.RowNumber}: {ex.Message}";
                failures.Enqueue(detail);
                await WriteLogLineAsync(logWriter, logLock, $"[失败] {detail}").ConfigureAwait(false);
                await _errorLogService.LogErrorAsync("数据库修改-单行执行", detail,
                    ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            }
            finally
            {
                reportProcessed();
            }
        }
    }

    private async Task ExecuteArasRowsAsync(
        IReadOnlyCollection<WorkbookRow> rows,
        string sqlTemplate,
        Innovator innovator,
        StreamWriter logWriter,
        SemaphoreSlim logLock,
        ConcurrentQueue<string> failures,
        Action reportProcessed,
        Action reportSuccess,
        Action reportFailure,
        CancellationToken cancellationToken)
    {
        // 当前启动的 Innovator/HttpServerConnection 为共享对象，不跨线程并发使用。
        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var assembledSql = AssembleSql(sqlTemplate, row.Values);
                var arasResult = await Task.Run(() => innovator.applySQL(assembledSql), cancellationToken)
                    .ConfigureAwait(false);
                if (arasResult == null)
                    throw new InvalidOperationException("Aras applySQL 未返回结果。");
                if (arasResult.isError())
                    throw new InvalidOperationException(arasResult.getErrorString());
                reportSuccess();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                reportFailure();
                var detail = $"行 {row.RowNumber}: {ex.Message}";
                failures.Enqueue(detail);
                await WriteLogLineAsync(logWriter, logLock, $"[失败] {detail}").ConfigureAwait(false);
                await _errorLogService.LogErrorAsync("数据库修改-Aras单行执行", detail,
                    ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            }
            finally
            {
                reportProcessed();
            }
        }
    }

    private async Task ExecuteRowWithRetryAsync(
        SqlConnection connection,
        string sqlTemplate,
        WorkbookRow row,
        CancellationToken cancellationToken)
    {
        var placeholders = GetPlaceholderColumns(sqlTemplate);
        for (var attempt = 1; attempt <= MaximumDeadlockRetries; attempt++)
        {
            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = sqlTemplate;
                command.CommandTimeout = CommandTimeoutSeconds;
                foreach (var placeholder in placeholders)
                {
                    var value = row.Values.GetValueOrDefault(placeholder);
                    command.Parameters.Add(new SqlParameter("@" + placeholder, value ?? DBNull.Value));
                }

                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (SqlException ex) when (ex.Number == 1205 && attempt < MaximumDeadlockRetries)
            {
                await _errorLogService.LogErrorAsync("数据库修改-死锁重试",
                    $"行 {row.RowNumber} 第 {attempt} 次执行发生死锁，将自动重试。",
                    ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMilliseconds(150 * attempt * attempt), cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException($"行 {row.RowNumber} 在死锁重试后仍未完成。");
    }

    private async Task<WorkbookData> ReadWorkbookDataAsync(
        DatabaseModificationRequest request,
        int maximumRows,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request);
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var package = new ExcelPackage(new FileInfo(request.FilePath), true);
            var worksheet = package.Workbook.Worksheets[request.SheetName]
                ?? throw new InvalidOperationException($"找不到工作表: {request.SheetName}");
            if (worksheet.Dimension == null)
                throw new InvalidOperationException("所选工作表没有数据。");

            var resolvedEndColumn = request.EndColumn == -1
                ? worksheet.Dimension.End.Column
                : Math.Min(request.EndColumn, worksheet.Dimension.End.Column);
            if (request.StartColumn > resolvedEndColumn)
                throw new ArgumentOutOfRangeException(nameof(request.StartColumn), "起始列超出工作表数据范围。");

            var resolvedEndRow = ResolveLastDataRow(
                worksheet,
                request.StartRow,
                request.EndRow,
                request.StartColumn,
                resolvedEndColumn);

            var mappings = BuildColumnMappings(worksheet, request.StartColumn, resolvedEndColumn);
            var rows = new List<WorkbookRow>();
            var skippedRows = 0;
            for (var rowIndex = request.StartRow; rowIndex <= resolvedEndRow; rowIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                var displayValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var hasData = false;
                foreach (var mapping in mappings)
                {
                    var cell = worksheet.Cells[rowIndex, mapping.Index + 1];
                    var rawValue = NormalizeCellValue(cell.Value, cell.Text);
                    var displayValue = cell.Text?.Trim() ?? string.Empty;
                    values[mapping.Letter] = rawValue;
                    displayValues[mapping.Letter] = displayValue;
                    hasData |= rawValue != null || displayValue.Length > 0;
                }

                if (!hasData)
                {
                    skippedRows++;
                    continue;
                }

                rows.Add(new WorkbookRow(rowIndex, values, displayValues));
                if (rows.Count >= maximumRows)
                    break;
            }

            var totalDataRows = CountDataRows(
                worksheet,
                request.StartRow,
                resolvedEndRow,
                request.StartColumn,
                resolvedEndColumn);

            return new WorkbookData(
                rows,
                mappings,
                totalDataRows,
                resolvedEndRow,
                resolvedEndColumn,
                skippedRows);
        }, cancellationToken).ConfigureAwait(false);
    }

    private static DatabaseModificationPreview BuildPreview(
        WorkbookData workbookData,
        bool includeSql,
        string sqlTemplate)
    {
        var table = new DataTable();
        foreach (var mapping in workbookData.ColumnMappings)
            table.Columns.Add($"@{mapping.Letter} · {mapping.Header}", typeof(string));

        foreach (var row in workbookData.Rows)
        {
            var dataRow = table.NewRow();
            for (var index = 0; index < workbookData.ColumnMappings.Count; index++)
            {
                var mapping = workbookData.ColumnMappings[index];
                dataRow[index] = row.DisplayValues.GetValueOrDefault(mapping.Letter) ?? string.Empty;
            }
            table.Rows.Add(dataRow);
        }

        return new DatabaseModificationPreview
        {
            ExcelData = table,
            ColumnMappings = workbookData.ColumnMappings,
            SqlStatements = includeSql
                ? workbookData.Rows.Select(row => AssembleSql(sqlTemplate, row.Values)).ToList()
                : [],
            TotalDataRows = workbookData.TotalDataRows,
            ResolvedEndRow = workbookData.ResolvedEndRow,
            ResolvedEndColumn = workbookData.ResolvedEndColumn
        };
    }

    private static string AssembleSql(string sqlTemplate, IReadOnlyDictionary<string, object?> values)
    {
        return PlaceholderRegex().Replace(sqlTemplate, match =>
        {
            var column = match.Groups["column"].Value.ToUpperInvariant();
            return ToSqlLiteral(values.GetValueOrDefault(column));
        });
    }

    private static string ToSqlLiteral(object? value)
    {
        return value switch
        {
            null or DBNull => "NULL",
            bool boolValue => boolValue ? "1" : "0",
            byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal
                => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "NULL",
            DateTime dateTime => $"'{dateTime:yyyy-MM-ddTHH:mm:ss.fff}'",
            DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:yyyy-MM-ddTHH:mm:ss.fffzzz}'",
            Guid guid => $"'{guid:D}'",
            _ => "N'" + Convert.ToString(value, CultureInfo.InvariantCulture)?.Replace("'", "''") + "'"
        };
    }

    private static void ValidateRequest(DatabaseModificationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateWorkbookPath(request.FilePath);
        if (string.IsNullOrWhiteSpace(request.SheetName))
            throw new ArgumentException("请选择工作表。", nameof(request.SheetName));
        if (request.StartRow < 1)
            throw new ArgumentOutOfRangeException(nameof(request.StartRow), "起始行必须大于等于 1。");
        if (request.StartColumn < 1)
            throw new ArgumentOutOfRangeException(nameof(request.StartColumn), "起始列必须大于等于 1。");
        if (request.EndRow != -1 && request.EndRow < request.StartRow)
            throw new ArgumentOutOfRangeException(nameof(request.EndRow), "结束行必须为 -1 或大于等于起始行。");
        if (request.EndColumn != -1 && request.EndColumn < request.StartColumn)
            throw new ArgumentOutOfRangeException(nameof(request.EndColumn), "结束列必须为 -1 或大于等于起始列。");
        if (request.ExecutionMode != DatabaseModificationExecutionModes.Aras
            && request.ExecutionMode != DatabaseModificationExecutionModes.Orm)
            throw new ArgumentException("执行方式必须为 Aras方式或 ORM模式。", nameof(request.ExecutionMode));
    }

    private static void ValidateWorkbookPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("请选择有效的 Excel 模板文件。", filePath);
        var extension = Path.GetExtension(filePath);
        if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase)
            && !extension.Equals(".xlsm", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("数据库修改模式支持 .xlsx 与 .xlsm 文件。");
        }
    }

    private static void ValidateSqlTemplate(string sqlTemplate)
    {
        if (string.IsNullOrWhiteSpace(sqlTemplate))
            throw new ArgumentException("请输入 SQL 模板。", nameof(sqlTemplate));
        if (!PlaceholderRegex().IsMatch(sqlTemplate))
            throw new ArgumentException("SQL 模板至少需要一个 @A、@B 等 Excel 列占位符。", nameof(sqlTemplate));
        if (QuotedPlaceholderRegex().IsMatch(sqlTemplate))
            throw new ArgumentException("占位符外侧不要添加引号；系统会按数据类型自动参数化和转义。", nameof(sqlTemplate));
        if (!ModificationStatementRegex().IsMatch(sqlTemplate))
            throw new ArgumentException("仅允许 UPDATE、INSERT、DELETE 或 MERGE 修改语句。", nameof(sqlTemplate));
        if (ForbiddenStatementRegex().IsMatch(sqlTemplate) || BatchSeparatorRegex().IsMatch(sqlTemplate))
            throw new ArgumentException("SQL 模板包含被禁止的 DDL、权限或批处理语句。", nameof(sqlTemplate));
    }

    private static void EnsurePlaceholderColumnsExist(
        string sqlTemplate,
        IReadOnlyCollection<ColumnMapping> mappings)
    {
        var available = mappings.Select(mapping => mapping.Letter)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = GetPlaceholderColumns(sqlTemplate)
            .Where(column => !available.Contains(column))
            .ToList();
        if (missing.Count > 0)
            throw new ArgumentException($"SQL 占位符超出所选列范围: {string.Join(", ", missing.Select(x => "@" + x))}");
    }

    private static List<string> GetPlaceholderColumns(string sqlTemplate)
    {
        return PlaceholderRegex().Matches(sqlTemplate)
            .Select(match => match.Groups["column"].Value.ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int ResolveLastDataRow(
        ExcelWorksheet worksheet,
        int startRow,
        int configuredEndRow,
        int startColumn,
        int endColumn)
    {
        var candidateEndRow = configuredEndRow == -1
            ? worksheet.Dimension.End.Row
            : Math.Min(configuredEndRow, worksheet.Dimension.End.Row);
        if (candidateEndRow < startRow)
            throw new ArgumentOutOfRangeException(nameof(startRow), "起始行超出工作表数据范围。");

        for (var row = candidateEndRow; row >= startRow; row--)
        {
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = worksheet.Cells[row, column];
                if (cell.Value != null || !string.IsNullOrWhiteSpace(cell.Text))
                    return row;
            }
        }

        throw new InvalidOperationException("所选范围内没有数据。");
    }

    private static List<ColumnMapping> BuildColumnMappings(
        ExcelWorksheet worksheet,
        int startColumn,
        int endColumn)
    {
        var usedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var mappings = new List<ColumnMapping>();
        for (var column = startColumn; column <= endColumn; column++)
        {
            var letter = ColumnIndexToLetter(column);
            var baseHeader = worksheet.Cells[1, column].Text?.Trim();
            if (string.IsNullOrWhiteSpace(baseHeader))
                baseHeader = "未命名列";

            var header = baseHeader;
            var suffix = 2;
            while (!usedHeaders.Add(header))
                header = $"{baseHeader}_{suffix++}";

            mappings.Add(new ColumnMapping
            {
                Letter = letter,
                Header = header,
                Index = column - 1
            });
        }

        return mappings;
    }

    private static int CountDataRows(
        ExcelWorksheet worksheet,
        int startRow,
        int endRow,
        int startColumn,
        int endColumn)
    {
        var count = 0;
        for (var row = startRow; row <= endRow; row++)
        {
            var hasData = false;
            for (var column = startColumn; column <= endColumn; column++)
            {
                var cell = worksheet.Cells[row, column];
                if (cell.Value == null && string.IsNullOrWhiteSpace(cell.Text))
                    continue;
                hasData = true;
                break;
            }
            if (hasData)
                count++;
        }
        return count;
    }

    private static object? NormalizeCellValue(object? value, string displayText)
    {
        if (value == null)
            return string.IsNullOrWhiteSpace(displayText) ? null : displayText.Trim();
        return value is ExcelErrorValue ? displayText.Trim() : value;
    }

    private static string ColumnIndexToLetter(int oneBasedColumnIndex)
    {
        var dividend = oneBasedColumnIndex;
        var builder = new StringBuilder();
        while (dividend > 0)
        {
            dividend--;
            builder.Insert(0, (char)('A' + dividend % 26));
            dividend /= 26;
        }
        return builder.ToString();
    }

    private static async Task WriteLogLineAsync(
        StreamWriter writer,
        SemaphoreSlim logLock,
        string message)
    {
        await logLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await writer.WriteLineAsync(message).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }
        finally
        {
            logLock.Release();
        }
    }

    private async Task TryLogOperationAsync(
        string operationType,
        string entityType,
        string entityId,
        string description)
    {
        try
        {
            await _operationLogService.LogAsync(operationType, entityType, entityId, description)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("数据库修改-操作日志", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
    }

    private sealed record WorkbookRow(
        int RowNumber,
        Dictionary<string, object?> Values,
        Dictionary<string, string> DisplayValues);

    private sealed record WorkbookData(
        List<WorkbookRow> Rows,
        List<ColumnMapping> ColumnMappings,
        int TotalDataRows,
        int ResolvedEndRow,
        int ResolvedEndColumn,
        int SkippedRows);

    [GeneratedRegex(@"(?<!@)@(?<column>[A-Z]{1,3})\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"['""]\s*@[A-Z]{1,3}\b|@[A-Z]{1,3}\b\s*['""]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex QuotedPlaceholderRegex();

    [GeneratedRegex(@"^\s*(UPDATE|INSERT\s+INTO|DELETE\s+FROM|MERGE\s+INTO)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ModificationStatementRegex();

    [GeneratedRegex(@"\b(DROP|ALTER|TRUNCATE|CREATE|BACKUP|RESTORE|GRANT|REVOKE|DENY)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ForbiddenStatementRegex();

    [GeneratedRegex(@"(^|\r?\n)\s*GO\s*(\r?\n|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BatchSeparatorRegex();
}
