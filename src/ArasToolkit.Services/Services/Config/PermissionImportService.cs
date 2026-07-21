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
/// 导入顺序（严格按序执行）:
/// 1. Sheet1「权限配置」→ 创建/更新 Permission 定义
/// 2. Sheet2「详细权限」→ 创建/更新 Access（权限的关联表）
///
/// 模板说明:
/// - Sheet1「权限配置」: 权限配置名称 | 权限名称
/// - Sheet2「详细权限」: 权限名称 | 所属角色 | 允许创建 | 允许读取 | 允许更新 | 允许删除
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位符注入点
/// - 覆盖模式必须带 where 条件精确匹配
/// - Sheet2 的 source_id（Permission ID）和 role_id（Identity ID）均做缓存，避免重复查询
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
    /// 生成 Excel 模板文件（2个Sheet，含加粗表头 + 自适应列宽）
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // ===== Sheet 1: 权限配置（2列）=====
        var ws1 = package.Workbook.Worksheets.Add("权限配置");
        var headers1 = new[]
        {
            "权限配置名称",    // Col 1 → AML source_id（ItemType 名称）
            "权限名称"         // Col 2 → AML <name>
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 40);

        // ===== Sheet 2: 详细权限（6列）=====
        var ws2 = package.Workbook.Worksheets.Add("详细权限");
        var headers2 = new[]
        {
            "权限名称",        // Col 1 → source_id（Permission 的 name，用于查询 Permission ID）
            "所属角色",        // Col 2 → role_id（Identity name，用于查询 Identity ID）
            "允许创建",        // Col 3 → <can_add> (0/1)
            "允许读取",        // Col 4 → <can_get> (0/1)
            "允许更新",        // Col 5 → <can_update> (0/1)
            "允许删除"         // Col 6 → <can_delete> (0/1)
        };
        WriteHeaders(ws2, headers2);
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 按序导入2个Sheet: 权限配置 → 详细权限(Access)
    ///
    /// ⚠️ AML 占位符说明:
    /// 本 Service 中 AML 模板使用占位格式，实际 AML 由调用方自行维护。
    /// 如需自定义 AML（如 Permission 特定属性映射），请修改对应 BuildXxxAml 方法。
    /// </summary>
    public async Task<PermissionImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new PermissionImportResult();
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
            var sheet1Rows = ReadSheetRows(package, "权限配置", 2);
            var sheet2Rows = ReadSheetRows(package, "详细权限", 6);

            var totalRows = sheet1Rows.Count + sheet2Rows.Count;
            result.Sheet1Total = sheet1Rows.Count;
            result.Sheet2Total = sheet2Rows.Count;

            await writer.WriteLineAsync($"Sheet1 权限配置: {sheet1Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"Sheet2 详细权限: {sheet2Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"合计: {totalRows} 行").ConfigureAwait(false);

            ReportPhase(progress, "读取Excel", 0, totalRows, $"共 {totalRows} 条待处理");

            // ===== 6-1. Sheet1: 权限配置（需要 source_id = ItemType ID）=====
            int sheet1Success = await ProcessSheet1Async(
                innovator, sheet1Rows, 0, totalRows,
                importMode, writer, result.FailedDetails,
                progress, cancellationToken).ConfigureAwait(false);
            result.Sheet1Count = sheet1Success;

            // ===== 6-2. Sheet2: 详细权限/Access（需要 source_id = Permission ID + role_id = Identity ID）=====
            int sheet2Success = await ProcessSheet2Async(
                innovator, sheet2Rows, sheet1Rows.Count, totalRows,
                importMode, writer, result.FailedDetails,
                progress, cancellationToken).ConfigureAwait(false);
            result.Sheet2Count = sheet2Success;

            // ===== 7. 保存成功记录 =====
            await SaveSuccessLogAsync(relativePath, sheet1Success, sheet2Success)
                .ConfigureAwait(false);
            await _operationLogService.LogAsync("Import", "PermissionImportLog",
                $"{sheet1Success}/{sheet2Success}",
                $"权限配置导入: 权限配置{sheet1Success}条 / 详细权限{sheet2Success}条")
                .ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            ReportPhase(progress, "完成", totalRows, totalRows,
                $"权限配置{sheet1Success} / 详细权限{sheet2Success}");
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
            await _errorLogService.LogErrorAsync("权限配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await WriteSummaryAsync(writer, result).ConfigureAwait(false);
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

    // ==================== Sheet 处理引擎 ====================

    /// <summary>
    /// Sheet1 处理: 权限配置
    /// 覆盖模式下需要 source_id（ItemType ID），通过权限配置名称查询并缓存
    /// </summary>
    private async Task<int> ProcessSheet1Async(
        dynamic innovator,
        List<Dictionary<int, string>> rows,
        int offset, int totalRows,
        string importMode,
        StreamWriter writer,
        List<string> failedDetails,
        IProgress<ImportProgressInfo>? progress,
        CancellationToken ct)
    {
        int success = 0;
        var itemTypeIdCache = new Dictionary<string, string>();

        for (int i = 0; i < rows.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var row = rows[i];
            var itemName = row.GetValueOrDefault(2, "");  // 权限名称作为显示名
            var overallIdx = offset + i + 1;

            progress?.Report(new ImportProgressInfo
            {
                Phase = $"权限配置-{importMode}",
                Current = i + 1,
                PhaseTotal = rows.Count,
                OverallCurrent = overallIdx,
                OverallTotal = totalRows,
                ItemName = itemName,
                ErrorCount = failedDetails.Count
            });

            try
            {
                // 覆盖模式: 解析 source_id（ItemType ID）用于 where 条件
                var sourceId = "";
                if (importMode == "覆盖")
                {
                    var itemTypeName = row.GetValueOrDefault(1, "");  // 权限配置名称
                    if (!itemTypeIdCache.TryGetValue(itemTypeName, out sourceId!))
                    {
                        sourceId = ResolveItemTypeId(innovator, itemTypeName);
                        itemTypeIdCache[itemTypeName] = sourceId;
                    }
                }

                var aml = BuildPermissionAml(row, importMode, sourceId);
                var amlResult = innovator.applyAML(aml);
                if (amlResult.isError())
                {
                    var failMsg = $"[权限配置 行{i + 2}] {itemName} → Aras错误: {amlResult.getErrorString()}";
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
                await writer.WriteLineAsync($"[取消] 用户在 权限配置 第 {i + 1}/{rows.Count} 条取消")
                    .ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                var failMsg = $"[权限配置 行{i + 2}] {itemName} → 异常: {ex.Message}";
                await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                failedDetails.Add(failMsg);
            }
        }

        await writer.WriteLineAsync($"权限配置 成功: {success}/{rows.Count}").ConfigureAwait(false);
        return success;
    }

    /// <summary>
    /// Sheet2 处理: 详细权限（Access 表）
    /// 需要缓存两个 ID:
    /// - source_id = Permission ID（通过权限名称查询）
    /// - role_id = Identity ID（通过所属角色名称查询）
    /// 覆盖模式下 where 条件: Access.source_id + Access.role_id
    /// </summary>
    private async Task<int> ProcessSheet2Async(
        dynamic innovator,
        List<Dictionary<int, string>> rows,
        int offset, int totalRows,
        string importMode,
        StreamWriter writer,
        List<string> failedDetails,
        IProgress<ImportProgressInfo>? progress,
        CancellationToken ct)
    {
        int success = 0;
        var permissionIdCache = new Dictionary<string, string>();  // 权限名称 → Permission ID
        var roleIdCache = new Dictionary<string, string>();        // 角色名称 → Identity ID

        for (int i = 0; i < rows.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var row = rows[i];
            var itemName = row.GetValueOrDefault(1, "");  // 权限名称作为显示名
            var overallIdx = offset + i + 1;

            progress?.Report(new ImportProgressInfo
            {
                Phase = $"详细权限-{importMode}",
                Current = i + 1,
                PhaseTotal = rows.Count,
                OverallCurrent = overallIdx,
                OverallTotal = totalRows,
                ItemName = itemName,
                ErrorCount = failedDetails.Count
            });

            try
            {
                // 解析 source_id（Permission ID）和 role_id（Identity ID），均带缓存
                var permName = row.GetValueOrDefault(1, "");   // 权限名称
                var roleName = row.GetValueOrDefault(2, "");   // 所属角色

                if (!permissionIdCache.TryGetValue(permName, out var permId))
                {
                    permId = ResolvePermissionId(innovator, permName);
                    permissionIdCache[permName] = permId;
                }

                if (!roleIdCache.TryGetValue(roleName, out var roleId))
                {
                    roleId = ResolveIdentityId(innovator, roleName);
                    roleIdCache[roleName] = roleId;
                }

                var aml = BuildAccessAml(row, importMode, permId, roleId);
                var amlResult = innovator.applyAML(aml);
                if (amlResult.isError())
                {
                    var failMsg = $"[详细权限 行{i + 2}] {itemName} → Aras错误: {amlResult.getErrorString()}";
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
                await writer.WriteLineAsync($"[取消] 用户在 详细权限 第 {i + 1}/{rows.Count} 条取消")
                    .ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                var failMsg = $"[详细权限 行{i + 2}] {itemName} → 异常: {ex.Message}";
                await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                failedDetails.Add(failMsg);
            }
        }

        await writer.WriteLineAsync($"详细权限 成功: {success}/{rows.Count}").ConfigureAwait(false);
        return success;
    }

    // ==================== AML 构建（占位 — 调用方自行维护） ====================

    /// <summary>
    /// 构建 Sheet1「权限配置」AML（Permission 节点）
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// - Col 1 (idx 1): 权限配置名称 → source_id 的 ItemType name
    /// - Col 2 (idx 2): 权限名称 → &lt;name&gt;
    ///
    /// 覆盖模式: where="Permission.name='{permName}' and Permission.source_id='{sourceId}'"
    /// </summary>
    private static string BuildPermissionAml(Dictionary<int, string> row, string importMode, string sourceId)
    {
        var itemTypeName = row.GetValueOrDefault(1, "");   // 权限配置名称
        var permName = row.GetValueOrDefault(2, "");        // 权限名称

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Permission' action='add'>" +
                   $"      <source_id>" +
                   $"          <Item type='ItemType' action='get' select='id'>" +
                   $"              <name>{itemTypeName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"      <name>{permName}</name>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: 必须带 where 精确匹配
        return $"<AML>" +
               $"  <Item type='Permission' action='merge' where=\"Permission.name='{permName}' and Permission.source_id='{sourceId}'\">" +
               $"      <source_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{itemTypeName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               $"      <name>{permName}</name>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建 Sheet2「详细权限」AML（Access 节点 — Permission 的关联表）
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// - Col 1 (idx 1): 权限名称 → source_id（Permission ID，已缓存解析）
    /// - Col 2 (idx 2): 所属角色 → role_id（Identity ID，已缓存解析）
    /// - Col 3 (idx 3): 允许创建 → &lt;can_add&gt; (0/1)
    /// - Col 4 (idx 4): 允许读取 → &lt;can_get&gt; (0/1)
    /// - Col 5 (idx 5): 允许更新 → &lt;can_update&gt; (0/1)
    /// - Col 6 (idx 6): 允许删除 → &lt;can_delete&gt; (0/1)
    ///
    /// 覆盖模式: where="Access.source_id='{permId}' and Access.role_id='{roleId}'"
    /// </summary>
    private static string BuildAccessAml(Dictionary<int, string> row, string importMode,
        string permId, string roleId)
    {
        var canAdd = row.GetValueOrDefault(3, "");        // 允许创建
        var canGet = row.GetValueOrDefault(4, "");        // 允许读取
        var canUpdate = row.GetValueOrDefault(5, "");     // 允许更新
        var canDelete = row.GetValueOrDefault(6, "");     // 允许删除

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Access' action='add'>" +
                   $"      <source_id>{permId}</source_id>" +
                   $"      <role_id>{roleId}</role_id>" +
                   $"      <can_add>{canAdd}</can_add>" +
                   $"      <can_get>{canGet}</can_get>" +
                   $"      <can_update>{canUpdate}</can_update>" +
                   $"      <can_delete>{canDelete}</can_delete>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: 必须带 where 精确匹配（source_id + role_id 唯一确定一条 Access）
        return $"<AML>" +
               $"  <Item type='Access' action='merge' where=\"Access.source_id='{permId}' and Access.role_id='{roleId}'\">" +
               $"      <source_id>{permId}</source_id>" +
               $"      <role_id>{roleId}</role_id>" +
               $"      <can_add>{canAdd}</can_add>" +
               $"      <can_get>{canGet}</can_get>" +
               $"      <can_update>{canUpdate}</can_update>" +
               $"      <can_delete>{canDelete}</can_delete>" +
               $"  </Item>" +
               $"</AML>";
    }

    // ==================== ID 解析（带缓存，减少查询压力） ====================

    /// <summary>
    /// 根据 ItemType 名称查询其 GUID（用于 Sheet1 覆盖模式的 where 条件）
    /// </summary>
    private static string ResolveItemTypeId(dynamic innovator, string itemTypeName)
    {
        if (string.IsNullOrWhiteSpace(itemTypeName)) return "";

        var queryAml = "<AML>" +
                       $"  <Item type='ItemType' action='get' where=\"ItemType.name='{itemTypeName}'\" select='id'/>" +
                       "</AML>";
        var result = innovator.applyAML(queryAml);
        if (result.isError() || result.getItemCount() == 0) return "";

        return result.getItemByIndex(0).getID();
    }

    /// <summary>
    /// 根据 Permission name 查询其 GUID（用于 Sheet2 的 source_id）
    /// </summary>
    private static string ResolvePermissionId(dynamic innovator, string permName)
    {
        if (string.IsNullOrWhiteSpace(permName)) return "";

        var queryAml = "<AML>" +
                       $"  <Item type='Permission' action='get' where=\"Permission.name='{permName}'\" select='id'/>" +
                       "</AML>";
        var result = innovator.applyAML(queryAml);
        if (result.isError() || result.getItemCount() == 0) return "";

        return result.getItemByIndex(0).getID();
    }

    /// <summary>
    /// 根据 Identity name 查询其 GUID（用于 Sheet2 的 role_id）
    /// </summary>
    private static string ResolveIdentityId(dynamic innovator, string identityName)
    {
        if (string.IsNullOrWhiteSpace(identityName)) return "";

        var queryAml = "<AML>" +
                       $"  <Item type='Identity' action='get' where=\"Identity.name='{identityName}'\" select='id'/>" +
                       "</AML>";
        var result = innovator.applyAML(queryAml);
        if (result.isError() || result.getItemCount() == 0) return "";

        return result.getItemByIndex(0).getID();
    }

    // ==================== 私有辅助方法 ====================

    /// <summary>
    /// 获取 Aras Innovator 实例（失败时抛出 InvalidOperationException）
    /// </summary>
    private Innovator GetInnovator()
    {
        var connection = _connectionService.HttpConnection
            ?? throw new InvalidOperationException("未连接到 Aras 系统，请先登录。");
        return connection.Login().getInnovator();
    }

    /// <summary>
    /// 写入 Excel 表头行（加粗 + 淡橙色背景）
    /// </summary>
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
        await writer.WriteLineAsync("===== 权限配置导入日志 =====").ConfigureAwait(false);
        await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
        await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
        await writer.WriteLineAsync($"导入模式: {importMode}").ConfigureAwait(false);
    }

    private static async Task WriteSummaryAsync(StreamWriter writer, PermissionImportResult result)
    {
        await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
        await writer.WriteLineAsync(
            $"权限配置: {result.Sheet1Count}/{result.Sheet1Total} | " +
            $"详细权限: {result.Sheet2Count}/{result.Sheet2Total}")
            .ConfigureAwait(false);
        await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
    }

    // ===== 数据库保存 =====

    private async Task SaveLogAsync(PermissionImportLog log)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.PermissionImportLogs.Add(log);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task SaveSuccessLogAsync(string relativePath,
        int sheet1Count, int sheet2Count)
    {
        var log = new PermissionImportLog
        {
            UserId = CurrentUserContext.CurrentUserId ?? "system",
            ImportTime = DateTime.Now,
            ImportFile = relativePath,
            Status = PermissionImportLog.StatusSuccess,
            Sheet1Count = sheet1Count,
            Sheet2Count = sheet2Count,
            CreatorOn = DateTime.Now
        };
        await SaveLogAsync(log).ConfigureAwait(false);
    }

    private async Task TrySaveFailedLogAsync(string relativePath, string errorDetail)
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
