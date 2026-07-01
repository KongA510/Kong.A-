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
/// List配置导入服务 — Excel模板生成 + 批量汇入Aras
///
/// 导入顺序（严格按序执行）:
/// 1. Sheet1「List主档」→ 创建/更新 List 定义
/// 2. Sheet2「菜单内容」→ 创建/更新 菜单项
/// 3. Sheet3「菜单内容(过滤)」→ 创建/更新 菜单过滤规则
///
/// 模板说明:
/// - Sheet1「List主档」: 名称(英文) | 备注说明
/// - Sheet2「菜单内容」: 父阶名称 | 显示标签 | 显示标签_繁体 | 显示标签_英文 | 数据库存储值 | 序号
/// - Sheet3「菜单内容(过滤)」: 父阶名称 | 显示标签 | 显示标签_繁体 | 显示标签_英文 | 数据库存储值 | 序号
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位符注入点
/// - 使用单线程串行执行，确保 Aras 数据一致性
/// - 支持 CancellationToken 取消 + IProgress&lt;ImportProgressInfo&gt; 进度报告
/// </summary>
public class ListImportService : IListImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ImportBaseDir = "Config/ListImports";

    public ListImportService(
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
    /// 生成 Excel 模板文件（3个Sheet，含加粗表头 + 自适应列宽）
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // ===== Sheet 1: List主档（2列）=====
        var ws1 = package.Workbook.Worksheets.Add("List主档");
        var headers1 = new[]
        {
            "名称(英文)",      // Col 1 → AML <name>（List 唯一标识）
            "备注说明"         // Col 2 → AML <label> 或自定义属性
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 40);

        // ===== Sheet 2: 菜单内容（6列）=====
        var ws2 = package.Workbook.Worksheets.Add("菜单内容");
        var headers2 = new[]
        {
            "父阶名称",           // Col 1 → AML source_id（父List的name）
            "显示标签",           // Col 2 → AML <label>（简体中文）
            "显示标签_繁体",      // Col 3 → AML <i18n:label xml:lang='zt'>（繁体中文）
            "显示标签_英文",      // Col 4 → AML <i18n:label xml:lang='en'>（英文）
            "数据库存储值",       // Col 5 → AML <value>（存储到数据库的值）
            "序号"                // Col 6 → AML <sort_order>（排序序号）
        };
        WriteHeaders(ws2, headers2);
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 30);

        // ===== Sheet 3: 菜单内容(过滤)（7列）=====
        var ws3 = package.Workbook.Worksheets.Add("菜单内容(过滤)");
        var headers3 = new[]
        {
            "父阶名称",           // Col 1 → AML source_id（父List的name）
            "显示标签",           // Col 2 → AML <label>（简体中文）
            "显示标签_繁体",      // Col 3 → AML <i18n:label xml:lang='zt'>（繁体中文）
            "显示标签_英文",      // Col 4 → AML <i18n:label xml:lang='en'>（英文）
            "数据库存储值",       // Col 5 → AML <value>（存储到数据库的值）
            "过滤值",             // Col 6 → AML <filter_value>（过滤条件值）
            "序号"                // Col 7 → AML <sort_order>（排序序号）
        };
        WriteHeaders(ws3, headers3);
        ws3.Cells[1, 1, 1, headers3.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 按序导入3个Sheet: List主档 → 菜单内容 → 菜单内容(过滤)
    ///
    /// ⚠️ AML 占位符说明:
    /// 本 Service 中 AML 模板使用占位符格式 "@列名"，实际 AML 由调用方自行维护。
    /// 当前实现: 为每个行数据构造基础的 Item AML 并提交到 Aras。
    /// 如需自定义 AML（如 List 特定属性映射），请修改对应 BuildXxxAml 方法。
    /// </summary>
    public async Task<ListImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new ListImportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // ===== 1. 创建按日期隔离的目录结构 =====
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dateDir = Path.Combine(baseDir, ImportBaseDir, dateFolder);
        var logsDir = Path.Combine(dateDir, "logs");
        var uploadsDir = Path.Combine(dateDir, "uploads");
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(uploadsDir);

        // ===== 2. 准备日志文件 =====
        var timestamp = DateTime.Now.ToString("HHmmss");
        var logFileName = $"import_{timestamp}.log";
        var logFile = Path.Combine(logsDir, logFileName);
        result.LogFilePath = logFile;

        // ===== 3. 归档源文件 =====
        var savedFileName = $"{timestamp}_{Path.GetFileName(filePath)}";
        var savedFilePath = Path.Combine(uploadsDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportBaseDir}/{dateFolder}/uploads/{savedFileName}";

        using var writer = new StreamWriter(logFile, false);
        try
        {
            await InitLogAsync(writer, filePath, importMode).ConfigureAwait(false);

            // ===== 4. 获取 Aras 连接 =====
            var innovator = GetInnovator();
            cancellationToken.ThrowIfCancellationRequested();

            ReportPhase(progress, "初始化", 0, 0, "正在连接 Aras...");

            // ===== 5. 读取 Excel =====
            using var package = new ExcelPackage(new FileInfo(filePath));
            var sheet1Rows = ReadSheetRows(package, "List主档", 2);
            var sheet2Rows = ReadSheetRows(package, "菜单内容", 6);
            var sheet3Rows = ReadSheetRows(package, "菜单内容(过滤)", 7);

            var totalRows = sheet1Rows.Count + sheet2Rows.Count + sheet3Rows.Count;
            result.Sheet1Total = sheet1Rows.Count;
            result.Sheet2Total = sheet2Rows.Count;
            result.Sheet3Total = sheet3Rows.Count;

            await writer.WriteLineAsync($"Sheet1 List主档: {sheet1Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"Sheet2 菜单内容: {sheet2Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"Sheet3 菜单内容(过滤): {sheet3Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"合计: {totalRows} 行").ConfigureAwait(false);

            ReportPhase(progress, "读取Excel", 0, totalRows, $"共 {totalRows} 条待处理");

            // ===== 6-1. Sheet1: List主档 =====
            int sheet1Success = await ProcessSheetAsync(
                innovator, sheet1Rows, 0, totalRows,
                importMode, "List主档",
                BuildListMasterAml, writer, result.FailedDetails,
                progress, cancellationToken).ConfigureAwait(false);
            result.Sheet1Count = sheet1Success;

            // ===== 6-2. Sheet2: 菜单内容 =====
            int sheet2Success = await ProcessSheetAsync(
                innovator, sheet2Rows, sheet1Rows.Count, totalRows,
                importMode, "菜单内容",
                BuildMenuContentAml, writer, result.FailedDetails,
                progress, cancellationToken).ConfigureAwait(false);
            result.Sheet2Count = sheet2Success;

            // ===== 6-3. Sheet3: 菜单内容(过滤) =====
            int sheet3Success = await ProcessSheetAsync(
                innovator, sheet3Rows, sheet1Rows.Count + sheet2Rows.Count, totalRows,
                importMode, "菜单内容(过滤)",
                BuildMenuFilterAml, writer, result.FailedDetails,
                progress, cancellationToken).ConfigureAwait(false);
            result.Sheet3Count = sheet3Success;

            // ===== 7. 保存成功记录 =====
            await SaveSuccessLogAsync(relativePath, sheet1Success, sheet2Success, sheet3Success)
                .ConfigureAwait(false);
            await _operationLogService.LogAsync("Import", "ListImportLog",
                $"{sheet1Success}/{sheet2Success}/{sheet3Success}",
                $"List配置导入: List主档{sheet1Success}条 / 菜单{sheet2Success}条 / 菜单过滤{sheet3Success}条")
                .ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            ReportPhase(progress, "完成", totalRows, totalRows,
                $"List主档{sheet1Success} / 菜单{sheet2Success} / 过滤{sheet3Success}");
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "导入已被用户取消";
            await writer.WriteLineAsync("[取消] 导入已被用户取消").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, "用户取消导入").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, $"{ex.Message}\n{ex.StackTrace}")
                .ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("List配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await WriteSummaryAsync(writer, result).ConfigureAwait(false);
            writer.Close();
        }

        return result;
    }

    // ==================== 历史记录 ====================

    /// <summary>
    /// 获取导入历史记录（分页，按创建时间倒序）
    /// </summary>
    public async Task<(List<ListImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.ListImportLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(r => r.UserId == userId);
        var total = await query.CountAsync().ConfigureAwait(false);
        var items = await query.OrderByDescending(r => r.CreatorOn)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync().ConfigureAwait(false);
        return (items, total);
    }

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    public async Task<ListImportLog?> GetLogByIdAsync(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.ListImportLogs.FindAsync(id).ConfigureAwait(false);
    }

    // ==================== AML 构建（占位符 — 调用方自行维护） ====================

    /// <summary>
    /// 构建 List 主档 AML
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// - Col 1 (idx 0): 名称(英文) → 映射到 Aras List <name>
    /// - Col 2 (idx 1): 备注说明 → 映射到 Aras List <label>
    /// </summary>
    private static string BuildListMasterAml(Dictionary<int, string> row, string importMode)
    {
        var name = row.GetValueOrDefault(0, "");
        var label = row.GetValueOrDefault(1, "");                         // 备注说明

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='List' action='add'>" +
                   $"      <name>{name}</name>" +
                   $"      <label>{label}</label>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        return $"<AML>" +
               $"  <Item type='List' action='merge' where=\"List.name='{name}'\">" +
               $"      <name>{name}</name>" +
               $"      <label>{label}</label>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建菜单内容 AML（List Value 节点）
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// - Col 1 (idx 0): 父阶名称 → source_id 的 List name
    /// - Col 2 (idx 1): 显示标签 → <label>（简体中文）
    /// - Col 3 (idx 2): 显示标签_繁体 → <i18n:label xml:lang='zt'>
    /// - Col 4 (idx 3): 显示标签_英文 → <i18n:label xml:lang='en'>
    /// - Col 5 (idx 4): 数据库存储值 → <value>
    /// - Col 6 (idx 5): 序号 → <sort_order>
    /// </summary>
    private static string BuildMenuContentAml(Dictionary<int, string> row, string importMode)
    {
        var parentName = row.GetValueOrDefault(0, "");                    // 父阶名称
        var label = row.GetValueOrDefault(1, "");                         // 显示标签（简）
        var labelZt = row.GetValueOrDefault(2, "");                       // 显示标签（繁）
        var labelEn = row.GetValueOrDefault(3, "");                       // 显示标签（英）
        var value = row.GetValueOrDefault(4, "");                         // 数据库存储值
        var sortOrder = row.GetValueOrDefault(5, "");                     // 序号

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Value' action='add'>" +
                   $"      <source_id>" +
                   $"          <Item type='List' action='get' select='id'>" +
                   $"              <name>{parentName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"      <label>{label}</label>" +
                   $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
                   $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
                   $"      <value>{value}</value>" +
                   $"      <sort_order>{sortOrder}</sort_order>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        return $"<AML>" +
               $"  <Item type='Value' action='merge' where=\"Value.value='{value}'\">" +
               $"      <source_id>" +
               $"          <Item type='List' action='get' select='id'>" +
               $"              <name>{parentName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <label>{label}</label>" +
               $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
               $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
               $"      <value>{value}</value>" +
               $"      <sort_order>{sortOrder}</sort_order>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建菜单内容(过滤) AML（7列，含过滤值）
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// 列映射:
    /// - Col 1 (idx 0): 父阶名称 → source_id 的 List name
    /// - Col 2 (idx 1): 显示标签 → <label>（简体中文）
    /// - Col 3 (idx 2): 显示标签_繁体 → <i18n:label xml:lang='zt'>
    /// - Col 4 (idx 3): 显示标签_英文 → <i18n:label xml:lang='en'>
    /// - Col 5 (idx 4): 数据库存储值 → <value>
    /// - Col 6 (idx 5): 过滤值 → <filter_value>（过滤条件值）
    /// - Col 7 (idx 6): 序号 → <sort_order>
    /// </summary>
    private static string BuildMenuFilterAml(Dictionary<int, string> row, string importMode)
    {
        var parentName = row.GetValueOrDefault(0, "");                    // 父阶名称
        var label = row.GetValueOrDefault(1, "");                         // 显示标签（简）
        var labelZt = row.GetValueOrDefault(2, "");                       // 显示标签（繁）
        var labelEn = row.GetValueOrDefault(3, "");                       // 显示标签（英）
        var value = row.GetValueOrDefault(4, "");                         // 数据库存储值
        var filterValue = row.GetValueOrDefault(5, "");                   // 过滤值
        var sortOrder = row.GetValueOrDefault(6, "");                     // 序号

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Value' action='add'>" +
                   $"      <source_id>" +
                   $"          <Item type='List' action='get' select='id'>" +
                   $"              <name>{parentName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"      <label>{label}</label>" +
                   $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
                   $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
                   $"      <value>{value}</value>" +
                   $"      <filter_value>{filterValue}</filter_value>" +
                   $"      <sort_order>{sortOrder}</sort_order>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        return $"<AML>" +
               $"  <Item type='Value' action='merge' where=\"Value.value='{value}'\">" +
               $"      <source_id>" +
               $"          <Item type='List' action='get' select='id'>" +
               $"              <name>{parentName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <label>{label}</label>" +
               $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
               $"      <i18n:label xml:lang='en' xmlns:i18n='http://www.aras.com/I18N/'>{labelEn}</i18n:label>" +
               $"      <value>{value}</value>" +
               $"      <filter_value>{filterValue}</filter_value>" +
               $"      <sort_order>{sortOrder}</sort_order>" +
               $"  </Item>" +
               $"</AML>";
    }

    // ==================== 私有方法 ====================

    /// <summary>
    /// 通用 Sheet 处理引擎 — 遍历行、构造AML、执行、上报进度
    /// </summary>
    /// <param name="innovator">Aras Innovator 实例</param>
    /// <param name="rows">Excel行数据</param>
    /// <param name="offset">全局序号偏移（前面Sheet已处理的行数）</param>
    /// <param name="totalRows">全局总行数</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <param name="phaseName">阶段名称（如 "List主档"）</param>
    /// <param name="buildAml">AML 构建委托</param>
    /// <param name="writer">日志写入器</param>
    /// <param name="failedDetails">失败明细列表</param>
    /// <param name="progress">进度回调</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>成功行数</returns>
    private static async Task<int> ProcessSheetAsync(
        dynamic innovator,
        List<Dictionary<int, string>> rows,
        int offset,
        int totalRows,
        string importMode,
        string phaseName,
        Func<Dictionary<int, string>, string, string> buildAml,
        StreamWriter writer,
        List<string> failedDetails,
        IProgress<ImportProgressInfo>? progress,
        CancellationToken ct)
    {
        int success = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var row = rows[i];
            var itemName = row.GetValueOrDefault(0, "");                // 第一列作为名称
            var overallIdx = offset + i + 1;

            progress?.Report(new ImportProgressInfo
            {
                Phase = $"{phaseName}-{importMode}",
                Current = i + 1,
                PhaseTotal = rows.Count,
                OverallCurrent = overallIdx,
                OverallTotal = totalRows,
                ItemName = itemName,
                ErrorCount = failedDetails.Count
            });

            try
            {
                var aml = buildAml(row, importMode);
                var amlResult = innovator.applyAML(aml);
                if (amlResult.isError())
                {
                    var failMsg = $"[{phaseName} 行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    failedDetails.Add(failMsg);
                }
                else
                {
                    success++;
                }
            }
            catch (OperationCanceledException)
            {
                await writer.WriteLineAsync($"[取消] 用户在 {phaseName} 第 {i + 1}/{rows.Count} 条处取消")
                    .ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                var failMsg = $"[{phaseName} 行{i + 2}] {itemName} — 异常: {ex.Message}";
                await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                failedDetails.Add(failMsg);
            }
        }

        await writer.WriteLineAsync($"{phaseName} 成功: {success}/{rows.Count}").ConfigureAwait(false);
        return success;
    }

    // ===== 连接获取 =====

    /// <summary>
    /// 获取 Aras Innovator 实例（失败时抛出 InvalidOperationException）
    /// </summary>
    private Innovator GetInnovator()
    {
        var connection = _connectionService.HttpConnection
            ?? throw new InvalidOperationException("未连接到 Aras 系统，请先登录。");
        return connection.Login().getInnovator();
    }

    // ===== 工具方法 =====

    /// <summary>
    /// 写入 Excel 表头行（加粗 + 淡橙色背景，便于区分表头与数据区）
    /// </summary>
    private static void WriteHeaders(ExcelWorksheet ws, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cells[1, i + 1];
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            // 淡橙色背景 (#FDEBD0)
            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(253, 235, 208));
        }
    }

    /// <summary>
    /// 读取指定 Sheet 的数据行（跳过表头），空行自动跳过
    /// </summary>
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
            if (dict.Count > 0) rows.Add(dict);
            row++;
        }
        return rows;
    }

    // ===== 进度上报 =====

    private static void ReportPhase(IProgress<ImportProgressInfo>? progress,
        string phase, int current, int total, string itemName)
    {
        progress?.Report(new ImportProgressInfo
        {
            Phase = phase,
            Current = current,
            PhaseTotal = total,
            OverallCurrent = current,
            OverallTotal = total,
            ItemName = itemName
        });
    }

    // ===== 日志写入 =====

    private static async Task InitLogAsync(StreamWriter writer, string filePath, string importMode)
    {
        await writer.WriteLineAsync("===== List配置导入日志 =====").ConfigureAwait(false);
        await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
        await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
        await writer.WriteLineAsync($"导入模式: {importMode}").ConfigureAwait(false);
    }

    private static async Task WriteSummaryAsync(StreamWriter writer, ListImportResult result)
    {
        await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
        await writer.WriteLineAsync(
            $"List主档: {result.Sheet1Count}/{result.Sheet1Total} | " +
            $"菜单内容: {result.Sheet2Count}/{result.Sheet2Total} | " +
            $"菜单过滤: {result.Sheet3Count}/{result.Sheet3Total}")
            .ConfigureAwait(false);
        await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
    }

    // ===== 数据库保存 =====

    /// <summary>
    /// 保存导入日志到数据库
    /// </summary>
    private async Task SaveLogAsync(ListImportLog log)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.ListImportLogs.Add(log);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 保存成功日志
    /// </summary>
    private async Task SaveSuccessLogAsync(string relativePath,
        int sheet1Count, int sheet2Count, int sheet3Count)
    {
        var log = new ListImportLog
        {
            UserId = CurrentUserContext.CurrentUserId ?? "system",
            ImportTime = DateTime.Now,
            ImportFile = relativePath,
            Status = ListImportLog.StatusSuccess,
            Sheet1Count = sheet1Count,
            Sheet2Count = sheet2Count,
            Sheet3Count = sheet3Count,
            CreatorOn = DateTime.Now
        };
        await SaveLogAsync(log).ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试保存失败日志 — 写入失败静默处理，不覆盖原始异常
    /// </summary>
    private async Task TrySaveFailedLogAsync(string relativePath, string errorDetail)
    {
        try
        {
            var log = new ListImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = ListImportLog.StatusFailed,
                ErrorLog = errorDetail,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
        }
        catch
        {
            // 静默处理
        }
    }
}
