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
/// 1. 读取 Excel 文件（3个Sheet）
/// 2. 按 Sheet 顺序执行: 生命周期汇入 → 生命周期状态 → 转变
/// 3. 覆盖模式下，预建缓存池（Dictionary按名称缓存ID，同名只查一次Aras）
/// 4. 逐行构造 AML 语句，通过 Aras API 执行
/// 5. 记录详细日志到本地文件 + 操作日志到数据库
///
/// 模板说明（3个Sheet）:
/// Sheet「生命周期汇入」4列: 生命周期名称 | 描述 | 挂载位置 | 分类
/// Sheet「生命周期状态」6列: 生命周期名称 | 状态名称 | 状态标签 | 状态标签繁体 | 状态标签英文 | 是否为初始状态
/// Sheet「转变」6列: 生命周期名称 | 角色 | 转换前 | 转换后 | 转换前执行 | 转换后执行
///
/// 注意:
/// - 具体 AML 逻辑由调用方自行维护，本 Service 提供 AML 占位符注入点
/// - 使用单线程串行执行，确保 Aras 数据一致性
/// - 支持 CancellationToken 取消 + IProgress&lt;ImportProgressInfo&gt; 进度报告
/// - 覆盖模式下通过 Dictionary 缓存池避免重复 Aras 查询
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
    /// 生成 Excel 模板文件（3个Sheet：生命周期汇入 / 生命周期状态 / 转变）
    /// </summary>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // Sheet 1: 生命周期汇入
        var ws1 = package.Workbook.Worksheets.Add("生命周期汇入");
        var headers1 = new[]
        {
            "生命周期名称",   // Col 1 → AML <name>
            "描述",           // Col 2 → AML <description>
            "挂载位置",       // Col 3 → AML source_id（ItemType name）
            "分类"            // Col 4 → AML <classification>
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 40);

        // Sheet 2: 生命周期状态
        var ws2 = package.Workbook.Worksheets.Add("生命周期状态");
        var headers2 = new[]
        {
            "生命周期名称",     // Col 1 → 关联所属生命周期
            "状态名称",         // Col 2 → AML <name>
            "状态标签",         // Col 3 → AML <label>（简体中文）
            "状态标签繁体",     // Col 4 → AML <i18n:label xml:lang='zt'>
            "状态标签英文",     // Col 5 → AML <i18n:label xml:lang='en'>
            "是否为初始状态"    // Col 6 → AML <is_default> (0/1)
        };
        WriteHeaders(ws2, headers2);
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 30);

        // Sheet 3: 转变
        var ws3 = package.Workbook.Worksheets.Add("转变");
        var headers3 = new[]
        {
            "生命周期名称",     // Col 1 → 关联所属生命周期
            "角色",           // Col 2 → AML <role>
            "转换前",         // Col 3 → AML <from_state>
            "转换后",         // Col 4 → AML <to_state>
            "转换前执行",     // Col 5 → AML <before_script>
            "转换后执行"      // Col 6 → AML <after_script>
        };
        WriteHeaders(ws3, headers3);
        ws3.Cells[1, 1, 1, headers3.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 执行生命周期导入汇入到 Aras 系统（3个Sheet串行处理）
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

            // 5. 读取 Excel 三个 Sheet 的数据
            progress?.Report(new ImportProgressInfo
            {
                Phase = "读取Excel",
                Current = 0,
                ItemName = "正在读取 Excel 文件..."
            });

            using var package = new ExcelPackage(new FileInfo(filePath));
            var sheet1Rows = ReadSheetRows(package, "生命周期汇入", 4);
            var sheet2Rows = ReadSheetRows(package, "生命周期状态", 6);
            var sheet3Rows = ReadSheetRows(package, "转变", 6);

            result.Sheet1Total = sheet1Rows.Count;
            result.Sheet2Total = sheet2Rows.Count;
            result.Sheet3Total = sheet3Rows.Count;

            await writer.WriteLineAsync($"生命周期汇入: {sheet1Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"生命周期状态: {sheet2Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"转变: {sheet3Rows.Count} 行").ConfigureAwait(false);

            int totalAll = sheet1Rows.Count + sheet2Rows.Count + sheet3Rows.Count;
            int overallIdx = 0;

            // ===== 缓存池：覆盖模式按名称缓存 ID，同名只查一次 Aras =====
            var itemTypeIdCache = new Dictionary<string, string>();      // ItemType name → id
            var lifecycleMapIdCache = new Dictionary<string, string>(); // Life Cycle name → Life Cycle Map id

            // 6. 处理 Sheet1: 生命周期汇入
            int s1Success = 0;
            for (int i = 0; i < sheet1Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var row = sheet1Rows[i];
                var itemName = row.GetValueOrDefault(1, "");
                overallIdx++;

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"生命周期汇入({importMode})",
                    Current = i + 1, PhaseTotal = sheet1Rows.Count,
                    OverallCurrent = overallIdx, OverallTotal = totalAll,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var sourceName = row.GetValueOrDefault(3, "");     // 挂载位置 → ItemType name
                    string itemTypeId = "", lcMapId = "";
                    if (importMode == "覆盖")
                    {
                        itemTypeId = ResolveItemTypeId(innovator, sourceName, itemTypeIdCache);
                        lcMapId = ResolveLifecycleMapId(innovator, itemName, lifecycleMapIdCache);
                    }

                    var aml = BuildLifecycleAml(row, importMode, itemTypeId, lcMapId);
                    var amlResult = innovator.applyAML(aml);
                    if (amlResult.isError())
                    {
                        var failMsg = $"[生命周期汇入·行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else { 
                        s1Success++;
                        //修复覆盖模式下，新增时未缓存 Life Cycle Map ID 的问题
                        var mapItem=amlResult.getItemsByXPath("//Item[@type='Life Cycle Map']") ;
                        var mapId = mapItem.getID();
                        var mapName = mapItem.getProperty("name", "");
                        lifecycleMapIdCache[mapName] = mapId;
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    var failMsg = $"[生命周期汇入·行{i + 2}] {itemName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }
            result.Sheet1Count = s1Success;
            await writer.WriteLineAsync($"Sheet1 完成: {s1Success}/{sheet1Rows.Count}").ConfigureAwait(false);

            // 7. 处理 Sheet2: 生命周期状态
            int s2Success = 0;
            // X轴坐标缓存: key = 生命周期名称 → 下一个X坐标 (每次+100避免节点重叠)
            var stateXCache = new Dictionary<string, int>();
            for (int i = 0; i < sheet2Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var row = sheet2Rows[i];
                var itemName = row.GetValueOrDefault(2, "");
                overallIdx++;

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"生命周期状态({importMode})",
                    Current = i + 1, PhaseTotal = sheet2Rows.Count,
                    OverallCurrent = overallIdx, OverallTotal = totalAll,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var lifecycleName = row.GetValueOrDefault(1, "");  // 生命周期名称
                    var isDefault = row.GetValueOrDefault(6, "");       // 是否为初始状态
                    var lcMapId = ResolveLifecycleMapId(innovator, lifecycleName, lifecycleMapIdCache);

                    // 动态计算 X 坐标: 同一生命周期下每次+100，避免节点重叠
                    if (!stateXCache.TryGetValue(lcMapId, out var x))
                        x = 0;
                    stateXCache[lcMapId] = x + 200;

                    var aml = BuildLifecycleStateAml(row, importMode, x, lcMapId);
                    var amlResult = innovator.applyAML(aml);
                    if (amlResult.isError())
                    {
                        var failMsg = $"[生命周期状态·行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else
                    {
                        s2Success++;

                        // 如果标记为初始状态，回写 Life Cycle Map 的 start_state
                        if (isDefault == "是" && !string.IsNullOrEmpty(lcMapId))
                        {
                            var stateItem = amlResult.getItemByIndex(0);
                            var stateId = stateItem.getID();
                            var setStartStateAml = "<AML>" +
                                $"  <Item type='Life Cycle Map' action='edit' where=\"Life_Cycle_Map.id='{lcMapId}'\">" +
                                $"      <start_state>{stateId}</start_state>" +
                                $"  </Item>" +
                                "</AML>";
                            var setResult = innovator.applyAML(setStartStateAml);
                            if (setResult.isError())
                            {
                                var failMsg = $"[生命周期状态·行{i + 2}] {itemName} — 设置初始状态失败: {setResult.getErrorString()}";
                                await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                                result.FailedDetails.Add(failMsg);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    var failMsg = $"[生命周期状态·行{i + 2}] {itemName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }
            result.Sheet2Count = s2Success;
            await writer.WriteLineAsync($"Sheet2 完成: {s2Success}/{sheet2Rows.Count}").ConfigureAwait(false);

            // 8. 处理 Sheet3: 转变
            int s3Success = 0;
            // 状态ID缓存: key = "lcMapId|stateName" → Life Cycle State ID
            var stateIdCache = new Dictionary<string, string>();
            for (int i = 0; i < sheet3Rows.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var row = sheet3Rows[i];
                var fromStateName = row.GetValueOrDefault(3, "");
                var toStateName = row.GetValueOrDefault(4, "");
                var itemName = $"{fromStateName} → {toStateName}";
                overallIdx++;

                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"转变({importMode})",
                    Current = i + 1, PhaseTotal = sheet3Rows.Count,
                    OverallCurrent = overallIdx, OverallTotal = totalAll,
                    ItemName = itemName,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var lifecycleName = row.GetValueOrDefault(1, "");  // 生命周期名称
                    string lcMapId = "", fromStateId = "", toStateId = "";
                    if (importMode == "覆盖")
                    {
                        lcMapId = ResolveLifecycleMapId(innovator, lifecycleName, lifecycleMapIdCache);
                        fromStateId = ResolveLifecycleStateId(innovator, lcMapId, fromStateName, stateIdCache);
                        toStateId = ResolveLifecycleStateId(innovator, lcMapId, toStateName, stateIdCache);
                    }

                    var aml = BuildTransitionAml(row, importMode, lcMapId, fromStateId, toStateId);
                    var amlResult = innovator.applyAML(aml);
                    if (amlResult.isError())
                    {
                        var failMsg = $"[转变·行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                    }
                    else { s3Success++; }
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    var failMsg = $"[转变·行{i + 2}] {itemName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                }
            }
            result.Sheet3Count = s3Success;
            await writer.WriteLineAsync($"Sheet3 完成: {s3Success}/{sheet3Rows.Count}").ConfigureAwait(false);

            // 9. 保存导入成功记录到数据库
            var log = new LifecycleImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = LifecycleImportLog.StatusSuccess,
                Sheet1Count = s1Success,
                Sheet2Count = s2Success,
                Sheet3Count = s3Success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            await _operationLogService.LogAsync("Import", "LifecycleImportLog", log.Id,
                $"生命周期配置导入: 生命周期{s1Success} 状态{s2Success} 转变{s3Success}").ConfigureAwait(false);

            result.IsSuccess = true;
            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = totalAll, PhaseTotal = totalAll,
                OverallCurrent = totalAll, OverallTotal = totalAll,
                ItemName = $"生命周期{s1Success} 状态{s2Success} 转变{s3Success}"
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
            await writer.WriteLineAsync($"生命周期汇入: {result.Sheet1Count}/{result.Sheet1Total}").ConfigureAwait(false);
            await writer.WriteLineAsync($"生命周期状态: {result.Sheet2Count}/{result.Sheet2Total}").ConfigureAwait(false);
            await writer.WriteLineAsync($"转变: {result.Sheet3Count}/{result.Sheet3Total}").ConfigureAwait(false);
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
    /// 构建生命周期（Life Cycle）的 AML 语句
    ///
    /// 新增模式: action='add' — 创建全新生命周期
    /// 覆盖模式: action='merge' — 按名称匹配，存在则更新
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// Sheet「生命周期汇入」列映射:
    /// Col 1 (idx 1): 生命周期名称 → <name>
    /// Col 2 (idx 2): 描述 → <description>
    /// Col 3 (idx 3): 挂载位置 → source_id 的 ItemType name
    /// Col 4 (idx 4): 分类 → <classification>
    /// </summary>
    /// <param name="itemTypeId">覆盖模式: 预解析的 ItemType GUID（挂载位置）</param>
    /// <param name="lcMapId">覆盖模式: 预解析的 Life Cycle Map GUID</param>
    private static string BuildLifecycleAml(Dictionary<int, string> row, string importMode,
        string itemTypeId = "", string lcMapId = "")
    {
        var name = row.GetValueOrDefault(1, "");           // 生命周期名称
        var description = row.GetValueOrDefault(2, "");    // 描述
        var sourceName = row.GetValueOrDefault(3, "");     // 挂载位置
        var classification = row.GetValueOrDefault(4, ""); // 分类

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='ItemType Life Cycle' action='add'>" +
                   $"       <related_id>" +
                   $"           <Item type='Life Cycle Map' action='add'>" +
                   $"           <description>{description}</description>" +
                   $"           <name>{name}</name>" +
                   $"           </Item>" +
                   $"       </realted_id>" +
                   $"       <source_id>" +
                   $"           <Item type='ItemType' action='get' select='id'>" +
                   $"               <name>{sourceName}</name>" +
                   $"           </Item>" +
                   $"       </source_id>" +
                   $"       <class_path>{classification}</class_path>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: where 使用 source_id + related_id 双列组合作为唯一键
        return $"<AML>" +
               $"  <Item type='ItemType Life Cycle' action='merge' where=\"source_id='{itemTypeId}' and related_id='{lcMapId}'\">" +
               $"       <related_id>" +
               $"           <Item type='Life Cycle Map' action='merge' where =\"Life_Cycle_Map.ID='{lcMapId}' \">" +
               $"           <description>{description}</description>" +
               $"           <name>{name}</name>" +
               $"           </Item>" +
               $"       </related_id>" +
               $"       <source_id>" +
               $"           <Item type='ItemType' action='get' select='id'>" +
               $"               <name>{sourceName}</name>" +
               $"           </Item>" +
               $"       </source_id>" +
               $"       <class_path>{classification}</class_path>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建生命周期状态（Life Cycle State）的 AML 语句
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// Sheet「生命周期状态」列映射（6列，第1列为生命周期名称）:
    /// Col 1 (idx 1): 生命周期名称 → 关联所属生命周期
    /// Col 2 (idx 2): 状态名称 → <name>
    /// Col 3 (idx 3): 状态标签 → <label>（简体中文）
    /// Col 4 (idx 4): 状态标签繁体 → <i18n:label xml:lang='zt'>
    /// Col 5 (idx 5): 状态标签英文 → <i18n:label xml:lang='en'>
    /// Col 6 (idx 6): 是否为初始状态 → <is_default>
    /// </summary>
    /// <param name="lcMapId">覆盖模式: 预解析的 Life Cycle Map GUID（所属生命周期）</param>
    private static string BuildLifecycleStateAml(Dictionary<int, string> row, string importMode,
        int x, string lcMapId = "")
    {
        var lifecycleName = row.GetValueOrDefault(1, "");  // 生命周期名称
        var stateName = row.GetValueOrDefault(2, "");      // 状态名称
        var label = row.GetValueOrDefault(3, "");           // 状态标签（简）
        var labelZt = row.GetValueOrDefault(4, "");         // 状态标签（繁）
        var labelEn = row.GetValueOrDefault(5, "");         // 状态标签（英）
        var isDefault = row.GetValueOrDefault(6, "");       // 是否为初始状态

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Life Cycle State' action='add'>" +
                   $"      <name>{stateName}</name>" +
                   $"      <label>{label}</label>" +
                   $"      <x>{x}</x>" +
                   $"      <y>100</y>" +
                   $"      <image>../images/LifeCycleState.svg</image>" +
                   $"      <i18n:label  xml:lang=\"en\" xmlns:i18n=\"http://www.aras.com/I18N\">{labelEn}</i18n:label>" +
                   $"      <i18n:label xml:lang=\"zc\" xmlns:i18n=\"http://www.aras.com/I18N\">{label}</i18n:label>" +
                   $"      <i18n:label xml:lang=\"zt\" xmlns:i18n=\"http://www.aras.com/I18N\">{labelZt}</i18n:label>" +
                   $"      <source_id>" +
                   $"          <Item type='Life Cycle Map' action='get' select='id'>" +
                   $"              <name>{lifecycleName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: where 使用 source_id + name 组合作为唯一键
        return $"<AML>" +
               $"  <Item type='Life Cycle State' action='merge' where=\"source_id='{lcMapId}' and name='{stateName}'\">" +
               $"      <x>{x}</x>" +
               $"      <y>100</y>" +
               $"      <image>../images/LifeCycleState.svg</image>" +
               $"      <source_id>{lcMapId}</source_id>" +
               $"      <name>{stateName}</name>" +
               $"      <i18n:label  xml:lang=\"en\" xmlns:i18n=\"http://www.aras.com/I18N\">{labelEn}</i18n:label>" +
               $"      <i18n:label xml:lang=\"zc\" xmlns:i18n=\"http://www.aras.com/I18N\">{label}</i18n:label>" +
               $"      <i18n:label xml:lang=\"zt\" xmlns:i18n=\"http://www.aras.com/I18N\">{labelZt}</i18n:label>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建转变（Life Cycle Transition）的 AML 语句
    ///
    /// TODO: 自行维护 AML 映射逻辑
    /// Sheet「转变」列映射（6列，第1列为生命周期名称）:
    /// Col 1 (idx 1): 生命周期名称 → 关联所属生命周期
    /// Col 2 (idx 2): 角色 → <role>
    /// Col 3 (idx 3): 转换前 → <from_state>
    /// Col 4 (idx 4): 转换后 → <to_state>
    /// Col 5 (idx 5): 转换前执行 → <before_script>
    /// Col 6 (idx 6): 转换后执行 → <after_script>
    /// </summary>
    /// <param name="lcMapId">覆盖模式: 预解析的 Life Cycle Map GUID（所属生命周期）</param>
    /// <param name="fromStateId">覆盖模式: 预解析的 from_state 对应的 Life Cycle State ID</param>
    /// <param name="toStateId">覆盖模式: 预解析的 to_state 对应的 Life Cycle State ID</param>
    private static string BuildTransitionAml(Dictionary<int, string> row, string importMode,
        string lcMapId = "", string fromStateId = "", string toStateId = "")
    {
        var lifecycleName = row.GetValueOrDefault(1, "");  // 生命周期名称
        var role = row.GetValueOrDefault(2, "");           // 角色
        var fromState = row.GetValueOrDefault(3, "");      // 转换前
        var toState = row.GetValueOrDefault(4, "");        // 转换后
        var beforeScript = row.GetValueOrDefault(5, "");   // 转换前执行
        var afterScript = row.GetValueOrDefault(6, "");    // 转换后执行

        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='Life Cycle Transition' action='add'>" +
                   $"      <source_id>{lcMapId}</source_id>" +
                   $"      <role>" +
                   $"        <Item type='Identity' action='get' select='id'>" +
                   $"           <name>{role}</name>" +
                   $"        </Item>" +
                   $"      </role>" +
                   $"      <from_state>{fromState}</from_state>" +
                   $"      <to_state>{toState}</to_state>" +
                   $"      <pre_action>" +
                   $"        <Item type='Method' action='get' select='id'>" +
                   $"           <name>{beforeScript}</name>" +
                   $"        </Item> " +
                   $"      </pre_action>" +
                   $"      <post_action>" +
                   $"        <Item type='Method' action='get' select='id'>" +
                   $"           <name>{afterScript}</name>" +
                   $"        </Item> " +
                   $"      </post_action>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: where 使用 source_id + from_state + to_state 唯一键，值域用预解析的 ID
        return $"<AML>" +
               $"  <Item type='Life Cycle Transition' action='merge' where=\"source_id='{lcMapId}' and from_state='{fromStateId}' and to_state='{toStateId}'\">" +
               $"      <source_id>{lcMapId}</source_id>" +
               $"      <role>" +
               $"        <Item type='Identity' action='get' select='id'>" +
               $"           <name>{role}</name>" +
               $"        </Item>" +
               $"      </role>" +
               $"      <from_state>{fromStateId}</from_state>" +
               $"      <to_state>{toStateId}</to_state>" +
               $"      <pre_action>" +
               $"        <Item type='Method' action='get' select='id'>" +
               $"           <name>{beforeScript}</name>" +
               $"        </Item> " +
               $"      </pre_action>" +
               $"      <post_action>" +
               $"        <Item type='Method' action='get' select='id'>" +
               $"           <name>{afterScript}</name>" +
               $"        </Item> " +
               $"      </post_action>" +
               $"  </Item>" +
               $"</AML>";
    }

    // ==================== ID 解析（缓存池 — 同名只查一次 Aras） ====================

    /// <summary>
    /// 按 ItemType 名称查询其 GUID（带缓存）。空名称返回空字符串。
    /// </summary>
    private static string ResolveItemTypeId(dynamic innovator, string itemTypeName,
        Dictionary<string, string> cache)
    {
        if (string.IsNullOrWhiteSpace(itemTypeName)) return "";

        if (!cache.TryGetValue(itemTypeName, out var id))
        {
            var queryAml = "<AML>" +
                           $"  <Item type='ItemType' action='get' where=\"ItemType.name='{itemTypeName}'\" select='id'/>" +
                           "</AML>";
            var result = innovator.applyAML(queryAml);
            id = (!result.isError() && result.getItemCount() > 0)
                ? result.getItemByIndex(0).getID()
                : "";
            cache[itemTypeName] = id;
        }
        return id;
    }

    /// <summary>
    /// 按 Life Cycle 名称查询其 Life Cycle Map 的 GUID（带缓存）。空名称返回空字符串。
    /// 查询路径: ItemType Life Cycle → related_id → Life Cycle Map.id
    /// </summary>
    private static string ResolveLifecycleMapId(dynamic innovator, string lifecycleName,
        Dictionary<string, string> cache)
    {
        if (string.IsNullOrWhiteSpace(lifecycleName)) return "";

        if (!cache.TryGetValue(lifecycleName, out var id))
        {
            var queryAml = "<AML>" +
                           $"  <Item type='Life Cycle Map' action='get' where=\"Life_Cycle_Map.name='{lifecycleName}'\" select='id'/>" +
                           "</AML>";
            var result = innovator.applyAML(queryAml);
            id = (!result.isError() && result.getItemCount() > 0)
                ? result.getItemByIndex(0).getID()
                : "";
            cache[lifecycleName] = id;
        }
        return id;
    }

    /// <summary>
    /// 按 Life Cycle Map ID + 状态名称查询 Life Cycle State 的 ID（带缓存）。
    /// 缓存键为 "lcMapId|stateName"，同名同生命周期只查一次。
    /// </summary>
    private static string ResolveLifecycleStateId(dynamic innovator, string lcMapId, string stateName,
        Dictionary<string, string> cache)
    {
        if (string.IsNullOrWhiteSpace(lcMapId) || string.IsNullOrWhiteSpace(stateName)) return "";

        var cacheKey = $"{lcMapId}|{stateName}";
        if (!cache.TryGetValue(cacheKey, out var id))
        {
            var queryAml = "<AML>" +
                           $"  <Item type='Life Cycle State' action='get' select='id'>" +
                           $"      <source_id>{lcMapId}</source_id>" +
                           $"      <name>{stateName}</name>" +
                           $"  </Item>" +
                           "</AML>";
            var result = innovator.applyAML(queryAml);
            id = (!result.isError() && result.getItemCount() > 0)
                ? result.getItemByIndex(0).getID()
                : "";
            cache[cacheKey] = id;
        }
        return id;
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
                Sheet2Count = result.Sheet2Count,
                Sheet3Count = result.Sheet3Count,
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
