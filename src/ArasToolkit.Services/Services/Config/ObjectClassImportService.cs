using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Xml.Linq;
using Aras.IOM;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 对象类汇入服务 — Excel模板生成 + 批量汇入Aras
///
/// 导入流程:
/// 1. 读取 Excel 文件（Sheet1=对象类, Sheet2=关系类）
/// 2. 逐行构造 AML 语句，通过 Aras API 执行
/// 3. 记录详细日志到本地文件 + 操作日志到数据库
///
/// 性能说明:
/// - 单线程串行执行，确保 Aras 数据一致性
/// - 使用 ConfigureAwait(false) 避免不必要的 UI 线程切换
/// - 支持 CancellationToken 取消长时间运行的操作
/// - 通过 IProgress&lt;ImportProgressInfo&gt; 实时报告结构化的进度信息（百分比/阶段/当前项）
///
/// 硬编码常量说明:
/// - auto_search=1: 新创建的对象类默认可自动搜索
/// - default_page_size=50: 对象类默认分页大小
/// - implementation_type=table: 对象类存储为数据库表
/// - enforce_discovery=1: 强制启用搜索发现
/// - revisions GUID: Aras 默认版本序列标识
/// - related_notnull=1: 关系类要求关联对象非空
/// - Can Add related_id GUID: Identity 资源标识
/// </summary>
public class ObjectClassImportService : IObjectClassImportService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private const string ImportBaseDir = "Config/ObjectClassImports";
    // Aras 系统常量（硬编码环境标识符）
    private const string DefaultRevisionsGuid = "7FE395DD8B9F4E1090756A34B733D75E";
    private const string CanAddRelatedIdGuid = "A73B655731924CD0B027E4F4D5FCC0A9";

    // 对象类/关系类默认值
    private const string DefaultAutoSearch = "1";
    private const string DefaultPageSize = "50";
    private const string DefaultRelatedNotNull = "1";
    private const string DefaultImplementationType = "table";
    private const string DefaultEnforceDiscovery = "1";
    private const string DefaultStructureView = "tabs on";

    public ObjectClassImportService(
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
    /// 生成 Excel 模板文件（2个Sheet，含表头加粗+自适应列宽）
    /// Sheet1「对象类新增」: 8列 — 对象类基本属性及简/繁/英多语言标签
    /// Sheet2「关系类新增」: 9列 — 关系类基本属性及简/繁/英多语言标签
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // ===== Sheet 1: 对象类新增（8列）=====
        var ws1 = package.Workbook.Worksheets.Add("对象类新增");
        var headers1 = new[]
        {
            "对象类名称",                    // Col 1 → AML <name>
            "物件显示名称",                  // Col 2 → AML <i18n:label xml:lang='zc'> (简体中文)
            "物件显示名称繁体",              // Col 3 → AML <i18n:label xml:lang='zt'> (繁体中文)
            "物件显示名称英文",              // Col 4 → AML <i18n:label xml:lang='en'>
            "TOC显示文字",                   // Col 5 → AML <i18n:label_plural xml:lang='zc'>
            "TOC显示文字繁体",               // Col 6 → AML <i18n:label_plural xml:lang='zt'>
            "TOC显示文字英文",               // Col 7 → AML <i18n:label_plural xml:lang='en'>
            "可换版(1=可以 0=不可以)"         // Col 8 → AML <is_versionable>
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 30);

        // ===== Sheet 2: 关系类新增（9列）=====
        var ws2 = package.Workbook.Worksheets.Add("关系类新增");
        var headers2 = new[]
        {
            "父对象名称",                                        // Col 1 → AML source_id (父对象ItemType名称)
            "关系类名称",                                        // Col 2 → AML <name>
            "页签序号",                                          // Col 3 → AML <sort_order>
            "页签标签",                                          // Col 4 → AML <i18n:label xml:lang='zc'>
            "页签标签繁体",                                      // Col 5 → AML <i18n:label xml:lang='zt'>
            "页签标签英文",                                      // Col 6 → AML <i18n:label xml:lang='en'>
            "新建关系选项(1=仅选取 2=仅创建 3=均可)",              // Col 7 → AML <for_related_option>
            "打开相关窗体",                                      // Col 8 → AML <new_show_related>
            "相关对象类"                                         // Col 9 → AML <related_id>
        };
        WriteHeaders(ws2, headers2);
        ws2.Cells[1, 1, 1, headers2.Length].AutoFitColumns(8, 30);

        return package.GetAsByteArray();
    }

    // ==================== 导入执行 ====================

    /// <summary>
    /// 执行导入汇入到 Aras 系统
    ///
    /// 执行流程:
    /// 1. 创建按日期隔离的目录结构 (uploads/logs)
    /// 2. 复制源文件到 uploads 目录作为归档
    /// 3. 打开日志文件记录详细执行过程
    /// 4. 获取 Aras 连接并验证有效性
    /// 5. 依次处理 Sheet1（对象类）→ Sheet2（关系类）
    /// 6. 每行构造 AML 并通过 applyAML 执行
    /// 7. 记录成功/失败明细
    /// 8. 保存导入记录到数据库
    /// </summary>
    /// <param name="filePath">Excel 文件完整路径</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <param name="progress">结构化进度回调（百分比 + 阶段 + 当前条目）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>导入结果汇总</returns>
    public async Task<ObjectClassImportResult> ImportAsync(
        string filePath,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new ObjectClassImportResult();
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

        // ===== 3. 归档源文件到 uploads =====
        var savedFileName = $"{timestamp}_{Path.GetFileName(filePath)}";
        var savedFilePath = Path.Combine(uploadsDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportBaseDir}/{dateFolder}/uploads/{savedFileName}";

        using var writer = new StreamWriter(logFile, false);
        try
        {
            // 写入日志头
            await writer.WriteLineAsync("===== 对象类汇入日志 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"导入模式: {importMode}").ConfigureAwait(false);

            // ===== 4. 获取并验证 Aras 连接 =====
            var innovator = _connectionService.TypedInnovator
                ?? throw new InvalidOperationException("未连接到 Aras 系统，请先登录。");

            // 取消检查点
            cancellationToken.ThrowIfCancellationRequested();

            progress?.Report(new ImportProgressInfo
            {
                Phase = "初始化",
                Current = 0,
                OverallCurrent = 0,
                ItemName = "正在连接 Aras..."
            });

            // ===== 5. 读取 Excel 数据 =====
            progress?.Report(new ImportProgressInfo
            {
                Phase = "读取Excel",
                Current = 0,
                OverallCurrent = 0,
                ItemName = "正在读取 Excel 文件..."
            });

            using var package = new ExcelPackage(new FileInfo(filePath));

            // Sheet1: 对象类（8列）
            var sheet1Rows = ReadSheetRows(package, "对象类新增", 8);
            // Sheet2: 关系类（9列）
            var sheet2Rows = ReadSheetRows(package, "关系类新增", 9);

            var totalRows = sheet1Rows.Count + sheet2Rows.Count;
            result.Sheet1Total = sheet1Rows.Count;
            result.Sheet2Total = sheet2Rows.Count;

            await writer.WriteLineAsync($"Sheet1 对象类新增: {sheet1Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"Sheet2 关系类新增: {sheet2Rows.Count} 行").ConfigureAwait(false);
            await writer.WriteLineAsync($"合计: {totalRows} 行").ConfigureAwait(false);

            // ===== 6. 处理 Sheet1: 对象类 =====
            int sheet1Success = 0;
            for (int i = 0; i < sheet1Rows.Count; i++)
            {
                // 取消检查（每条记录处理前检查）
                cancellationToken.ThrowIfCancellationRequested();

                var row = sheet1Rows[i];
                var itemName = row.GetValueOrDefault(1, "");           // 对象类名称（Col 1）
                var overallIdx = i + 1;                                // 全局序号（1-based）

                // 报告结构化进度
                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"对象类{importMode}",
                    Current = i + 1,
                    PhaseTotal = sheet1Rows.Count,
                    OverallCurrent = overallIdx,
                    OverallTotal = totalRows,
                    ItemName = itemName
                });

                try
                {
                    // 构造 AML 语句
                    // 使用 applyAML 一次性提交整个 Item（含 Relationships 嵌套），
                    // 比逐个调用 applyItem 更高效且能正确创建关联关系
                    var aml = BuildObjectClassAml(row, importMode);
                    var amlResult = innovator.applyAML(aml);

                    if (amlResult.isError())
                    {
                        var failMsg = $"[Sheet1 行{i + 2}] {itemName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                        await _errorLogService.LogErrorAsync("对象类汇入-写入", failMsg, ErrorLog.LevelP1).ConfigureAwait(false);
                    }
                    else
                    {
                        VerifyImportedLabels(innovator, aml);
                        sheet1Success++;
                    }
                }
                catch (OperationCanceledException)
                {
                    // 取消操作：记录到日志后重新抛出
                    await writer.WriteLineAsync($"[取消] 用户在对象类第 {i + 1}/{sheet1Rows.Count} 条处取消导入")
                        .ConfigureAwait(false);
                    throw;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet1 行{i + 2}] {itemName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                    await _errorLogService.LogErrorAsync("对象类汇入-行处理", failMsg,
                        ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
                }
            }

            result.Sheet1Count = sheet1Success;
            await writer.WriteLineAsync($"Sheet1 成功: {sheet1Success}/{sheet1Rows.Count}").ConfigureAwait(false);

            // ===== 7. 处理 Sheet2: 关系类 =====
            int sheet2Success = 0;
            for (int i = 0; i < sheet2Rows.Count; i++)
            {
                // 取消检查
                cancellationToken.ThrowIfCancellationRequested();

                var row = sheet2Rows[i];
                var relName = row.GetValueOrDefault(2, "");            // 关系类名称（Col 2）
                var overallIdx = sheet1Rows.Count + i + 1;             // 全局序号（Sheet1完成后继续计数）

                // 报告结构化进度
                progress?.Report(new ImportProgressInfo
                {
                    Phase = $"关系类{importMode}",
                    Current = i + 1,
                    PhaseTotal = sheet2Rows.Count,
                    OverallCurrent = overallIdx,
                    OverallTotal = totalRows,
                    ItemName = relName
                });

                try
                {
                    // 构造关系类 AML
                    // 注意: source_id 通过 get 动作动态查询父对象 ItemType ID
                    var aml = BuildRelationshipTypeAml(row, importMode);
                    var amlResult = innovator.applyAML(aml);

                    if (amlResult.isError())
                    {
                        var failMsg = $"[Sheet2 行{i + 2}] {relName} — Aras错误: {amlResult.getErrorString()}";
                        await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                        result.FailedDetails.Add(failMsg);
                        await _errorLogService.LogErrorAsync("对象类汇入-写入", failMsg, ErrorLog.LevelP1).ConfigureAwait(false);
                    }
                    else
                    {
                        VerifyImportedLabels(innovator, aml);
                        sheet2Success++;
                    }
                }
                catch (OperationCanceledException)
                {
                    await writer.WriteLineAsync($"[取消] 用户在关系类第 {i + 1}/{sheet2Rows.Count} 条处取消导入")
                        .ConfigureAwait(false);
                    throw;
                }
                catch (Exception ex)
                {
                    var failMsg = $"[Sheet2 行{i + 2}] {relName} — 异常: {ex.Message}";
                    await writer.WriteLineAsync(failMsg).ConfigureAwait(false);
                    result.FailedDetails.Add(failMsg);
                    await _errorLogService.LogErrorAsync("对象类汇入-行处理", failMsg,
                        ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
                }
            }

            result.Sheet2Count = sheet2Success;
            await writer.WriteLineAsync($"Sheet2 成功: {sheet2Success}/{sheet2Rows.Count}").ConfigureAwait(false);

            // ===== 8. 保存实际汇入结果；部分行失败不能显示为全部成功 =====
            result.IsSuccess = result.FailedDetails.Count == 0;
            result.ErrorMessage = result.IsSuccess ? null : $"汇入完成，{result.FailedDetails.Count} 行失败，请查看日志。";
            var log = new ObjectClassImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = result.IsSuccess ? ObjectClassImportLog.StatusSuccess : ObjectClassImportLog.StatusFailed,
                ErrorLog = string.Join(Environment.NewLine, result.FailedDetails),
                Sheet1Count = sheet1Success,
                Sheet2Count = sheet2Success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            await writer.WriteLineAsync("===== 导入完成 =====").ConfigureAwait(false);

            // 最终进度报告
            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = totalRows,
                PhaseTotal = totalRows,
                OverallCurrent = totalRows,
                OverallTotal = totalRows,
                ItemName = $"对象类{sheet1Success}条 / 关系类{sheet2Success}条"
            });
        }
        catch (OperationCanceledException)
        {
            // 用户主动取消 — 记录部分结果
            result.IsSuccess = false;
            result.ErrorMessage = "导入已被用户取消";
            await writer.WriteLineAsync("[取消] 导入已被用户取消").ConfigureAwait(false);

            // 保存取消记录
            await TrySaveFailedLogAsync(relativePath, "用户取消导入", result).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // 异常 — 记录完整错误信息
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await writer.WriteLineAsync($"[堆栈] {ex.StackTrace}").ConfigureAwait(false);

            // 保存失败记录（静默处理保存失败）
            await TrySaveFailedLogAsync(relativePath, $"{ex.Message}\n{ex.StackTrace}", result)
                .ConfigureAwait(false);

            // 写入错误日志系统
            await _errorLogService.LogErrorAsync("对象类汇入-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            // 日志收尾
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"对象类: {result.Sheet1Count}/{result.Sheet1Total} | 关系类: {result.Sheet2Count}/{result.Sheet2Total}")
                .ConfigureAwait(false);
            await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
            writer.Close();
        }

        return result;
    }

    // ==================== 历史记录查询 ====================

    /// <summary>
    /// 获取导入历史记录（分页，按创建时间倒序）
    /// </summary>
    public async Task<(List<ObjectClassImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = db.ObjectClassImportLogs.AsQueryable();

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

    /// <summary>
    /// 按 ID 获取单条导入记录
    /// </summary>
    public async Task<ObjectClassImportLog?> GetLogByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        return await db.ObjectClassImportLogs.FindAsync(id).ConfigureAwait(false);
    }

    // ==================== 私有AML构建方法 ====================

    /// <summary>
    /// 构建对象类（ItemType）的 AML 语句
    ///
    /// 新增模式: action='add' — 创建全新对象类
    /// 覆盖模式: action='merge' where="ItemType.name='{name}'" — 按名称匹配，存在则更新，不存在则创建
    ///
    /// 包含的嵌套关系:
    /// - Can Add: 指定哪些 Identity 可以添加此对象类
    /// </summary>
    /// <param name="row">Excel 行数据（字典键为1-based列号）</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <returns>AML 字符串</returns>
    private static string BuildObjectClassAml(Dictionary<int, string> row, string importMode)
    {
        var item = CreateImportItem("ItemType", row.GetValueOrDefault(1, ""), importMode);
        item.Add(
            MultilingualAml.Values("label", row.GetValueOrDefault(4, ""),
                row.GetValueOrDefault(2, ""), row.GetValueOrDefault(3, "")),
            MultilingualAml.Values("label_plural", row.GetValueOrDefault(7, ""),
                row.GetValueOrDefault(5, ""), row.GetValueOrDefault(6, "")),
            new XElement("structure_view", DefaultStructureView),
            new XElement("is_versionable", row.GetValueOrDefault(8, "0")),
            new XElement("auto_search", DefaultAutoSearch),
            new XElement("default_page_size", DefaultPageSize),
            new XElement("implementation_type", DefaultImplementationType),
            new XElement("enforce_discovery", DefaultEnforceDiscovery),
            new XElement("revisions", DefaultRevisionsGuid));
        if (importMode == "新增")
        {
            item.Add(new XElement("Relationships",
                new XElement("Item", new XAttribute("type", "Can Add"),
                    new XAttribute("action", "add"),
                    new XElement("related_id", CanAddRelatedIdGuid))));
        }
        return WrapImportItem(item);
    }

    /// <summary>关系类页签标签使用同一套多语系写法；相关对象为空时省略 related_id。</summary>
    private static string BuildRelationshipTypeAml(Dictionary<int, string> row, string importMode)
    {
        var item = CreateImportItem("RelationshipType", row.GetValueOrDefault(2, ""), importMode);
        item.Add(
            new XElement("source_id", ItemTypeByName(row.GetValueOrDefault(1, ""))),
            MultilingualAml.Values("label", row.GetValueOrDefault(6, ""),
                row.GetValueOrDefault(4, ""), row.GetValueOrDefault(5, "")),
            new XElement("for_related_option", row.GetValueOrDefault(7, "")),
            new XElement("related_notnull", DefaultRelatedNotNull),
            new XElement("auto_search", DefaultAutoSearch),
            new XElement("default_page_size", DefaultPageSize),
            new XElement("new_show_related", row.GetValueOrDefault(8, "")),
            new XElement("sort_order", row.GetValueOrDefault(3, "")));
        var relatedName = row.GetValueOrDefault(9, "");
        if (!string.IsNullOrWhiteSpace(relatedName))
            item.Add(new XElement("related_id", ItemTypeByName(relatedName)));
        return WrapImportItem(item);
    }

    private static XElement CreateImportItem(string type, string name, string importMode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidDataException($"{type} 名称不能为空。");
        if (importMode is not ("新增" or "覆盖"))
            throw new InvalidDataException($"不支持的汇入模式：{importMode}");
        var item = new XElement("Item", new XAttribute("type", type),
            new XAttribute("action", importMode == "新增" ? "add" : "merge"),
            new XAttribute("language", MultilingualAml.Languages));
        if (importMode == "覆盖")
            item.Add(new XAttribute("where", $"{type}.name='{name.Replace("'", "''")}'"));
        item.Add(new XElement("name", name));
        return item;
    }

    private static XElement ItemTypeByName(string name)
        => new("Item", new XAttribute("type", "ItemType"), new XAttribute("action", "get"),
            new XAttribute("select", "id"), new XElement("name", name));

    private static string WrapImportItem(XElement item)
        => new XElement("AML", new XAttribute(XNamespace.Xmlns + "i18n", MultilingualAml.Namespace),
            item).ToString(SaveOptions.DisableFormatting);

    /// <summary>回读明确语言的实际值，避免会话语言回退掩盖漏写译文。</summary>
    private static void VerifyImportedLabels(Innovator innovator, string submittedAml)
    {
        var submitted = XElement.Parse(submittedAml).Element("Item")!;
        var expected = submitted.Elements().Where(e => e.Name.Namespace == MultilingualAml.Namespace).ToList();
        if (expected.Count == 0) return;
        var query = new XElement("Item",
            new XAttribute("type", submitted.Attribute("type")!.Value),
            new XAttribute("action", "get"),
            new XAttribute("select", "id," + string.Join(',', expected.Select(e => e.Name.LocalName).Distinct())),
            new XAttribute("language", MultilingualAml.Languages),
            new XElement("name", submitted.Element("name")!.Value));
        var result = innovator.applyAML(WrapImportItem(query));
        if (result.isError() || result.getItemCount() != 1)
            throw new InvalidOperationException($"汇入已提交，但多语系回读失败：{result.getErrorString()}");
        var actual = XElement.Parse(result.getItemByIndex(0).node.OuterXml);
        foreach (var value in expected)
        {
            var language = value.Attribute(XNamespace.Xml + "lang")!.Value;
            // 必须匹配 i18n 节点，不使用 getProperty 的会话语言回退值。
            var saved = actual.Elements(value.Name).SingleOrDefault(
                e => (string?)e.Attribute(XNamespace.Xml + "lang") == language);
            if (saved == null || (string?)saved.Attribute("is_null") == "1" || saved.Value != value.Value)
                throw new InvalidOperationException(
                    $"汇入已提交，但 {value.Name.LocalName}[{language}] 未正确保存，请检查后使用覆盖模式重试。");
        }
    }

    // ==================== 私有辅助方法 ====================

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
    /// 读取指定 Sheet 的数据行（跳过表头行）
    /// 返回每行的单元格数据字典（key=列号1-based, value=文本内容），空行自动跳过
    /// </summary>
    /// <param name="package">Excel 包</param>
    /// <param name="sheetName">Sheet 名称</param>
    /// <param name="columnCount">预期列数（仅读取前N列）</param>
    /// <returns>行数据列表，每行为 Dictionary&lt;列号, 值&gt;</returns>
    private static List<Dictionary<int, string>> ReadSheetRows(
        ExcelPackage package, string sheetName, int columnCount)
    {
        var rows = new List<Dictionary<int, string>>();
        var ws = package.Workbook.Worksheets[sheetName];
        if (ws?.Dimension == null) return rows;

        int row = 2; // 从第2行开始（跳过第1行表头）
        while (row <= ws.Dimension.End.Row)
        {
            var dict = new Dictionary<int, string>();
            for (int col = 1; col <= columnCount; col++)
            {
                var val = ws.Cells[row, col].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(val))
                    dict[col] = val;
            }

            // 仅当该行至少有一个非空单元格时才加入结果集
            if (dict.Count > 0)
                rows.Add(dict);

            row++;
        }
        return rows;
    }

    /// <summary>
    /// 保存导入日志到数据库（使用短生命周期 DbContext 确保线程安全）
    /// </summary>
    private async Task SaveLogAsync(ObjectClassImportLog log)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            db.ObjectClassImportLogs.Add(log);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类汇入-保存历史", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
        try
        {
            await _operationLogService.LogAsync("Import", "ObjectClassImportLog", log.Id,
                $"对象类汇入: 对象类{log.Sheet1Count}条 / 关系类{log.Sheet2Count}条，状态：{log.Status}")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类汇入-操作日志", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 尝试保存失败日志 — 写入失败不抛异常，避免覆盖原始错误
    /// </summary>
    private async Task TrySaveFailedLogAsync(string relativePath, string errorDetail,
        ObjectClassImportResult result)
    {
        try
        {
            var log = new ObjectClassImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = ObjectClassImportLog.StatusFailed,
                ErrorLog = errorDetail,
                Sheet1Count = result.Sheet1Count,
                Sheet2Count = result.Sheet2Count,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类汇入-保存失败记录", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
        }
    }
}
