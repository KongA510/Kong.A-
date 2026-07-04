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

namespace ArasToolkit.Services.Services;

/// <summary>
/// 对象类配置导入服务 — Excel模板生成 + 批量汇入Aras
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
    private const string TemplateDir = "Config/ObjectClassTemplates";

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
    /// Sheet1「对象类新增」: 10列 — 对象类基本属性
    /// Sheet2「关系类新增」: 10列 — 关系类基本属性（已移除"必须"/"自动搜索"默认值列）
    /// </summary>
    /// <returns>Excel 文件字节数组</returns>
    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();

        // ===== Sheet 1: 对象类新增（10列）=====
        var ws1 = package.Workbook.Worksheets.Add("对象类新增");
        var headers1 = new[]
        {
            "对象类名称",                    // Col 1 → AML <name>
            "物件显示名称",                  // Col 2 → AML <i18n:label xml:lang='zc'> (简体中文)
            "物件显示名称繁体",              // Col 3 → AML <i18n:label xml:lang='zt'> (繁体中文)
            "物件显示名称英文",              // Col 4 → AML <label> (英文)
            "TOC显示文字",                   // Col 5 → AML <label_plural> (复数标签)
            "TOC显示文字繁体",              // Col 6 → 预留（当前AML未使用）
            "TOC显示文字英文",              // Col 7 → 预留（当前AML未使用）
            "可换版(1=可以 0=不可以)",        // Col 8 → AML <is_versionable>
            "自动搜索(默认1)",              // Col 9 → 读取备用，AML硬编码为1
            "页面默认大小(默认50)"           // Col 10 → 读取备用，AML硬编码为50
        };
        WriteHeaders(ws1, headers1);
        ws1.Cells[1, 1, 1, headers1.Length].AutoFitColumns(8, 30);

        // ===== Sheet 2: 关系类新增（10列，已移除"必须""自动搜索"列）=====
        var ws2 = package.Workbook.Worksheets.Add("关系类新增");
        var headers2 = new[]
        {
            "父对象名称",                                        // Col 1 → AML source_id (父对象ItemType名称)
            "关系类名称",                                        // Col 2 → AML <name>
            "页签序号",                                          // Col 3 → AML <sort_order>
            "页签标签",                                          // Col 4 → AML <label>
            "页签标签繁体",                                      // Col 5 → 预留（当前AML未使用）
            "页签标签英文",                                      // Col 6 → 预留（当前AML未使用）
            "新建关系选项(1=仅选取 2=仅创建 3=均可)",              // Col 7 → AML <for_related_option>
            "打开相关窗体",                                      // Col 8 → 预留（当前AML未使用）
            "源对象类",                                          // Col 9 → 预留（当前AML未使用）
            "相关对象类"                                         // Col 10 → 预留（当前AML未使用）
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
            await writer.WriteLineAsync("===== 对象类配置导入日志 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync($"导入模式: {importMode}").ConfigureAwait(false);

            // ===== 4. 获取并验证 Aras 连接 =====
            var connection = _connectionService.HttpConnection;
            if (connection == null)
            {
                throw new InvalidOperationException("未连接到 Aras 系统，请先登录。");
            }

            // 取消检查点
            cancellationToken.ThrowIfCancellationRequested();

            var login = connection.Login();
            var innovator = login.getInnovator();

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

            // Sheet1: 对象类（10列）
            var sheet1Rows = ReadSheetRows(package, "对象类新增", 10);
            // Sheet2: 关系类（10列，已移除"必须""自动搜索"）
            var sheet2Rows = ReadSheetRows(package, "关系类新增", 10);

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
                var itemName = row.GetValueOrDefault(0, "");           // 对象类名称（Col 1）
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
                    }
                    else
                    {
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
                var relName = row.GetValueOrDefault(1, "");            // 关系类名称（Col 2，注意索引从0开始）
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
                    }
                    else
                    {
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
                }
            }

            result.Sheet2Count = sheet2Success;
            await writer.WriteLineAsync($"Sheet2 成功: {sheet2Success}/{sheet2Rows.Count}").ConfigureAwait(false);

            // ===== 8. 保存导入成功记录到数据库 =====
            var log = new ObjectClassImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = ObjectClassImportLog.StatusSuccess,
                Sheet1Count = sheet1Success,
                Sheet2Count = sheet2Success,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);

            // 记录敏感操作日志
            await _operationLogService.LogAsync("Import", "ObjectClassImportLog", log.Id,
                $"对象类配置导入: 对象类{sheet1Success}条 / 关系类{sheet2Success}条")
                .ConfigureAwait(false);

            result.IsSuccess = true;
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
            await _errorLogService.LogErrorAsync("对象类配置-导入", ex.Message,
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
    /// <param name="row">Excel 行数据（列索引从0开始）</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <returns>AML 字符串</returns>
    private static string BuildObjectClassAml(Dictionary<int, string> row, string importMode)
    {
        // 列映射（用户可见列号 → GetValueOrDefault 索引）:
        // Col 1 (idx 0):  对象类名称 → <name>
        // Col 2 (idx 1):  物件显示名称(简) → <i18n:label xml:lang='zc'>
        // Col 3 (idx 2):  物件显示名称(繁) → <i18n:label xml:lang='zt'>
        // Col 4 (idx 3):  物件显示名称(英) → <label>
        // Col 5 (idx 4):  TOC显示文字 → <label_plural>
        // Col 8 (idx 7):  可换版 → <is_versionable>

        var name = row.GetValueOrDefault(1, "");                       // 对象类名称
        var labelZc = row.GetValueOrDefault(2, "");                   // 简体中文标签
        var labelZt = row.GetValueOrDefault(3, "");                   // 繁体中文标签
        var labelEn = row.GetValueOrDefault(4, "");                   // 英文标签
        var labelPlural = row.GetValueOrDefault(5, "");               // TOC 复数标签
        var isVersionable = row.GetValueOrDefault(8, "0");            // 可换版标志

        // 新增模式: 创建全新 ItemType
        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"  <Item type='ItemType' action='add'>" +
                   // 基本标识
                   $"      <name>{name}</name>" +
                   // 多语言标签
                   $"      <i18n:label xml:lang='zc' xmlns:i18n='http://www.aras.com/I18N/'>{labelZc}</i18n:label>" +
                   $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
                   $"      <label>{labelEn}</label>" +
                   $"      <label_plural>{labelPlural}</label_plural>" +
                   // 显示与结构
                   $"      <structure_view>{DefaultStructureView}</structure_view>" +
                   // 版本与搜索
                   $"      <is_versionable>{isVersionable}</is_versionable>" +
                   $"      <auto_search>{DefaultAutoSearch}</auto_search>" +
                   $"      <default_page_size>{DefaultPageSize}</default_page_size>" +
                   // 实现与发现
                   $"      <implementation_type>{DefaultImplementationType}</implementation_type>" +
                   $"      <enforce_discovery>{DefaultEnforceDiscovery}</enforce_discovery>" +
                   // 硬编码系统引用
                   $"      <revisions>{DefaultRevisionsGuid}</revisions>" +
                   // 嵌套关系: 授权 Identity 可添加此对象类
                   $"      <Relationships>" +
                   $"        <Item type='Can Add' action='add'>" +
                   $"            <related_id>{CanAddRelatedIdGuid}</related_id>" +
                   $"        </Item>" +
                   $"      </Relationships>" +
                   $"  </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: 按名称匹配，存在则合并更新
        return $"<AML>" +
               $"  <Item type='ItemType' action='merge' where=\"ItemType.name='{name}'\">" +
               // 基本标识（merge 模式下 name 重复提供以确保匹配）
               $"      <name>{name}</name>" +
               // 多语言标签
               $"      <i18n:label xml:lang='zc' xmlns:i18n='http://www.aras.com/I18N/'>{labelZc}</i18n:label>" +
               $"      <i18n:label xml:lang='zt' xmlns:i18n='http://www.aras.com/I18N/'>{labelZt}</i18n:label>" +
               $"      <label>{labelEn}</label>" +
               $"      <label_plural>{labelPlural}</label_plural>" +
               // 显示与结构
               $"      <structure_view>{DefaultStructureView}</structure_view>" +
               // 版本与搜索
               $"      <is_versionable>{isVersionable}</is_versionable>" +
               $"      <auto_search>{DefaultAutoSearch}</auto_search>" +
               $"      <default_page_size>{DefaultPageSize}</default_page_size>" +
               // 实现与发现
               $"      <implementation_type>{DefaultImplementationType}</implementation_type>" +
               $"      <enforce_discovery>{DefaultEnforceDiscovery}</enforce_discovery>" +
               // 硬编码系统引用
               $"      <revisions>{DefaultRevisionsGuid}</revisions>" +
               // 嵌套关系: 合并模式用 merge where 避免重复创建
               //$"      <Relationships>" +
               //$"        <Item type='Can Add' action='merge' where=\"Can_Add.related_id='{CanAddRelatedIdGuid}'\">" +
               //$"            <related_id>{CanAddRelatedIdGuid}</related_id>" +
               //$"        </Item>" +
               //$"      </Relationships>" +
               $"  </Item>" +
               $"</AML>";
    }

    /// <summary>
    /// 构建关系类（RelationshipType）的 AML 语句
    ///
    /// 结构说明:
    /// - source_id: 父对象 ItemType（通过 get 动作动态查询其 ID）
    /// - related_id: 关联对象 ItemType（覆盖模式下与 source_id 相同）
    ///
    /// 注意: 关系类名称在 Aras 中必须全局唯一
    /// </summary>
    /// <param name="row">Excel 行数据（列索引从0开始）</param>
    /// <param name="importMode">"新增" 或 "覆盖"</param>
    /// <returns>AML 字符串</returns>
    private static string BuildRelationshipTypeAml(Dictionary<int, string> row, string importMode)
    {
        // 列映射（用户可见列号 → GetValueOrDefault 索引，10列体系）:
        // Col 1 (idx 0):  父对象名称 → source_id 中的 ItemType name
        // Col 2 (idx 1):  关系类名称 → <name>
        // Col 3 (idx 2):  页签序号 → <sort_order>
        // Col 4 (idx 3):  页签标签 → <label>
        // Col 7 (idx 6):  新建关系选项 → <for_related_option>

        var sourceName = row.GetValueOrDefault(1, "");               // 父对象 ItemType 名称
        var relName = row.GetValueOrDefault(2, "");                  // 关系类名称
        var sortOrder = row.GetValueOrDefault(3, "");                // 页签序号
        var label = row.GetValueOrDefault(4, "");                    // 页签标签
        var forRelatedOption = row.GetValueOrDefault(7, "");         // 新建关系选项
        var formisOpen = row.GetValueOrDefault(8, "");               // 打开相关窗体 
        var related_name = row.GetValueOrDefault(9, "");             // 相关对象类
        // 新增模式: 创建全新 RelationshipType
        if (importMode == "新增")
        {
            return $"<AML>" +
                   $"   <Item type='RelationshipType' action='add'>" +
                   // 父对象: 通过 get 动态查询 ItemType ID
                   $"      <source_id>" +
                   $"          <Item type='ItemType' action='get' select='id'>" +
                   $"              <name>{sourceName}</name>" +
                   $"          </Item>" +
                   $"      </source_id>" +
                   // 基本属性
                   $"      <name>{relName}</name>" +
                   $"      <label>{label}</label>" +
                   // 行为控制
                   $"      <for_related_option>{forRelatedOption}</for_related_option>" +
                   $"      <related_notnull>{DefaultRelatedNotNull}</related_notnull>" +
                   // 搜索与分页
                   $"      <auto_search>{DefaultAutoSearch}</auto_search>" +
                   $"      <default_page_size>{DefaultPageSize}</default_page_size>" +
                   // 打开相关窗体
                   $"       <new_show_related>{formisOpen}</new_show_related>" +
                   // 排序
                   $"      <sort_order>{sortOrder}</sort_order>" +
                   // 关联对象（覆盖模式下复用关联对象名称）
                   $"      <related_id>" +
                   $"          <Item type='ItemType' action='get' select='id'>" +
                   $"              <name>{related_name}</name>" +
                   $"          </Item>" +
                   $"      </related_id>" +
                   $"   </Item>" +
                   $"</AML>";
        }

        // 覆盖模式: 按关系类名称匹配合并
        return $"<AML>" +
               $"   <Item type='RelationshipType' action='merge' where=\"RelationshipType.name='{relName}'\">" +
               // 父对象: 通过 get 动态查询 ItemType ID
               $"      <source_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{sourceName}</name>" +
               $"          </Item>" +
               $"      </source_id>" +
               // 基本属性
               $"      <name>{relName}</name>" +
               $"      <label>{label}</label>" +
               // 行为控制
               $"      <for_related_option>{forRelatedOption}</for_related_option>" +
               $"      <related_notnull>{DefaultRelatedNotNull}</related_notnull>" +
               // 搜索与分页
               $"      <auto_search>{DefaultAutoSearch}</auto_search>" +
               $"      <default_page_size>{DefaultPageSize}</default_page_size>" +
               // 打开相关窗体
               $"       <new_show_related>{formisOpen}</new_show_related>" +
               // 排序
               $"      <sort_order>{sortOrder}</sort_order>" +
               // 关联对象（覆盖模式下复用关联对象名称）
               $"      <related_id>" +
               $"          <Item type='ItemType' action='get' select='id'>" +
               $"              <name>{related_name}</name>" +
               $"          </Item>" +
               $"      </related_id>" +
               $"   </Item>" +
               $"</AML>";
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
        await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        db.ObjectClassImportLogs.Add(log);
        await db.SaveChangesAsync().ConfigureAwait(false);
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
        catch
        {
            // 日志保存失败不阻塞主流程（errorLogService 内部已静默处理）
        }
    }
}
