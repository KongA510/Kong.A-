using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 属性配置导入服务。以用户选中的现有 ItemType 为唯一 source_id，先预检并组装 AML，
/// 再逐条调用 IOM 提交；已成功的行立即保存在 Aras，不依赖后续行的执行结果。
/// </summary>
public sealed class PropertyImportService : IPropertyImportService
{
    private const string ImportBaseDir = "Config/PropertyImports";
    private const string PropertySheetName = "属性配置";
    private const string DictionarySheetName = "数据字典";
    private const int TemplateRowCount = 500;
    private const int FirstDataRow = 2;
    private const int LastDataRow = FirstDataRow + TemplateRowCount - 1;

    private static readonly XNamespace I18n = "http://www.aras.com/I18N";
    private static readonly XNamespace Xml = XNamespace.Xml;
    private static readonly Regex ArasNamePattern = new(
        "^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ArasIdPattern = new(
        "^[0-9A-Fa-f]{32}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly string[] TemplateHeaders =
    [
        "名称", "标签(简中)", "数据类型", "数据源", "长度", "精度", "小数位数", "必填", "唯一",
        "标签(繁中)", "标签(英文)", "引用外部属性", "键名顺序", "显示顺序(sort_order)"
    ];

    private static readonly string[] AmlFieldNames =
    [
        "name", "label[zc]", "data_type", "data_source", "stored_length", "prec", "scale",
        "is_required", "is_keyed", "label[zt]", "label[en]", "foreign_property",
        "keyed_name_order", "sort_order"
    ];

    private static readonly string[] BooleanHeaders =
    [
        "必填", "唯一"
    ];

    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public PropertyImportService(
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

    public byte[] GenerateTemplate()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add(PropertySheetName);
        var dictionary = package.Workbook.Worksheets.Add(DictionarySheetName);

        BuildDictionarySheet(package, dictionary);
        BuildPropertySheet(sheet);
        AddTemplateValidations(package, sheet, dictionary);

        return package.GetAsByteArray();
    }

    public async Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();
            var query = innovator.newItem("ItemType", "get");
            query.setAttribute("select", "id,name,label,is_relationship");
            query.setAttribute("orderBy", "label");

            var response = query.apply();
            ThrowIfError(response, "获取系统对象类失败");

            var result = new List<ArasItemTypeInfo>();
            for (var index = 0; index < response.getItemCount(); index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var item = response.getItemByIndex(index);
                var name = item.getProperty("name", string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                result.Add(new ArasItemTypeInfo
                {
                    Id = item.getID(),
                    Name = name,
                    Label = item.getProperty("label", name).Trim()
                });
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-获取对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<PropertyImportPreview> PreviewAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ReadAndValidateTemplate(filePath, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-模板预检", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<PropertyImportPreview> PrepareAsync(
        string filePath,
        string itemTypeId,
        string itemTypeName,
        string importMode = "覆盖",
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSelectedItemTypeArguments(itemTypeId, itemTypeName);
            var mode = NormalizeImportMode(importMode);
            var preview = ReadAndValidateTemplate(filePath, cancellationToken);
            preview.ItemTypeId = itemTypeId;
            preview.ItemTypeName = itemTypeName;
            preview.ImportMode = mode;

            var innovator = GetInnovator();
            VerifySelectedItemType(innovator, itemTypeId, itemTypeName);

            var referenceCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in preview.Rows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!row.IsValid)
                {
                    row.PlannedAction = "校验失败";
                    continue;
                }

                try
                {
                    ResolveDataSourceReferences(innovator, itemTypeId, row, referenceCache);
                    var existing = FindProperty(innovator, itemTypeId, row.Name);
                    row.ExistingPropertyId = existing?.getID() ?? string.Empty;

                    if (mode == "新增" && existing != null)
                    {
                        AddValidationError(row, $"对象类中已存在属性“{row.Name}”；新增模式不会覆盖同名属性");
                        row.PlannedAction = "名称冲突";
                        continue;
                    }

                    var willUpdate = mode == "覆盖" && existing != null;
                    row.PlannedAction = willUpdate ? "覆盖现有属性" : "新增属性";
                    row.AmlPreview = BuildPropertyAml(row, itemTypeId, willUpdate);
                }
                catch (Exception ex)
                {
                    AddValidationError(row, ex.Message);
                    row.PlannedAction = "组装失败";
                    await _errorLogService.LogErrorAsync(
                        $"属性配置-组装AML-行{row.ExcelRowNumber}", ex.Message,
                        ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
                }
            }

            preview.IsPrepared = true;
            return preview;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-组装AML", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<PropertyImportResult> ImportAsync(
        string filePath,
        string itemTypeId,
        string itemTypeName,
        string importMode = "覆盖",
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new PropertyImportResult();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dateFolder = DateTime.Now.ToString("yyyy_M_d", CultureInfo.InvariantCulture);
        var dateDir = Path.Combine(baseDir, ImportBaseDir, dateFolder);
        var logsDir = Path.Combine(dateDir, "logs");
        var uploadsDir = Path.Combine(dateDir, "uploads");
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(uploadsDir);

        var timestamp = DateTime.Now.ToString("HHmmssfff", CultureInfo.InvariantCulture);
        var logFile = Path.Combine(logsDir, $"import_{timestamp}.log");
        result.LogFilePath = logFile;

        var savedFileName = $"{timestamp}_{Path.GetFileName(filePath)}";
        var savedFilePath = Path.Combine(uploadsDir, savedFileName);
        File.Copy(filePath, savedFilePath, overwrite: true);
        var relativePath = $"{ImportBaseDir}/{dateFolder}/uploads/{savedFileName}";

        await using var writer = new StreamWriter(logFile, false);
        try
        {
            var mode = NormalizeImportMode(importMode);
            await writer.WriteLineAsync("===== 属性配置逐条导入日志 =====").ConfigureAwait(false);
            await writer.WriteLineAsync($"文件: {Path.GetFileName(filePath)}").ConfigureAwait(false);
            await writer.WriteLineAsync($"对象类: {itemTypeName} ({itemTypeId})").ConfigureAwait(false);
            await writer.WriteLineAsync($"导入模式: {mode}").ConfigureAwait(false);
            await writer.WriteLineAsync($"开始时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "预检与组装",
                ItemName = "正在解析模板、数据源和覆盖条件..."
            });

            var preview = await PrepareAsync(filePath, itemTypeId, itemTypeName, mode, cancellationToken)
                .ConfigureAwait(false);
            result.Sheet1Total = preview.Rows.Count;
            if (!preview.CanImport)
            {
                var details = preview.Rows
                    .Where(row => !row.IsValid)
                    .Select(row => $"[行{row.ExcelRowNumber}] {row.Name}: {row.ValidationMessage}")
                    .ToList();
                result.FailedDetails.AddRange(details);
                throw new InvalidDataException(
                    $"模板预检未通过，共 {preview.InvalidCount} 行需要修正。请在预览中查看明细后重新选择模板。");
            }

            var innovator = GetInnovator();
            for (var index = 0; index < preview.Rows.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var row = preview.Rows[index];
                progress?.Report(new ImportProgressInfo
                {
                    Phase = row.PlannedAction,
                    Current = index + 1,
                    PhaseTotal = preview.Rows.Count,
                    OverallCurrent = index + 1,
                    OverallTotal = preview.Rows.Count,
                    ItemName = row.Name,
                    ErrorCount = result.FailedDetails.Count
                });

                try
                {
                    var response = innovator.applyAML(row.AmlPreview);
                    if (response.isError())
                        throw new InvalidOperationException(response.getErrorString());

                    row.SubmitStatus = row.PlannedAction.StartsWith("覆盖", StringComparison.Ordinal)
                        ? "已覆盖"
                        : "已新增";
                    if (row.SubmitStatus == "已覆盖") result.UpdatedCount++;
                    else result.AddedCount++;
                    result.Sheet1Count++;
                    await writer.WriteLineAsync(
                        $"[成功][行{row.ExcelRowNumber}] {row.SubmitStatus} {row.Name}").ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    row.SubmitStatus = "提交失败";
                    var failure = $"[行{row.ExcelRowNumber}] {row.Name} — {ex.Message}";
                    result.FailedDetails.Add(failure);
                    await writer.WriteLineAsync($"[失败]{failure}").ConfigureAwait(false);
                    await _errorLogService.LogErrorAsync("属性配置-逐条提交", failure,
                        ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
                }
            }

            result.IsSuccess = true;
            var status = result.HasFailures
                ? PropertyImportLog.StatusFailed
                : PropertyImportLog.StatusSuccess;
            var log = new PropertyImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = status,
                ErrorLog = result.HasFailures ? string.Join(Environment.NewLine, result.FailedDetails) : null,
                Sheet1Count = result.Sheet1Count,
                CreatorOn = DateTime.Now
            };
            await SaveLogAsync(log).ConfigureAwait(false);
            await TryLogOperationAsync(log.Id,
                $"属性配置导入 {itemTypeName}: 新增{result.AddedCount}条，覆盖{result.UpdatedCount}条，失败{result.Sheet1Failed}条")
                .ConfigureAwait(false);

            progress?.Report(new ImportProgressInfo
            {
                Phase = "完成",
                Current = preview.Rows.Count,
                PhaseTotal = preview.Rows.Count,
                OverallCurrent = preview.Rows.Count,
                OverallTotal = preview.Rows.Count,
                ItemName = $"新增 {result.AddedCount} · 覆盖 {result.UpdatedCount}",
                ErrorCount = result.FailedDetails.Count
            });
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "导入已取消；取消前成功提交的属性已即时保存在 Aras。";
            await writer.WriteLineAsync($"[取消] {result.ErrorMessage}").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, result.ErrorMessage, result).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            await writer.WriteLineAsync($"[错误] {ex.Message}").ConfigureAwait(false);
            await TrySaveFailedLogAsync(relativePath, ex.ToString(), result).ConfigureAwait(false);
            await _errorLogService.LogErrorAsync("属性配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
        finally
        {
            await writer.WriteLineAsync($"结束时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").ConfigureAwait(false);
            await writer.WriteLineAsync(
                $"汇总: 成功 {result.Sheet1Count}/{result.Sheet1Total}，新增 {result.AddedCount}，覆盖 {result.UpdatedCount}")
                .ConfigureAwait(false);
            await writer.WriteLineAsync("===== 日志结束 =====").ConfigureAwait(false);
        }

        return result;
    }

    public async Task<(List<PropertyImportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var query = db.PropertyImportLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(record => record.UserId == userId);

            var total = await query.CountAsync().ConfigureAwait(false);
            var items = await query
                .OrderByDescending(record => record.CreatorOn)
                .Skip((Math.Max(1, page) - 1) * Math.Max(1, pageSize))
                .Take(Math.Max(1, pageSize))
                .ToListAsync().ConfigureAwait(false);
            return (items, total);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-查询历史", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<PropertyImportLog?> GetLogByIdAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            return await db.PropertyImportLogs.FindAsync(id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-查询日志详情", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private static void BuildPropertySheet(ExcelWorksheet sheet)
    {
        sheet.View.ShowGridLines = false;
        sheet.View.FreezePanes(2, 1);
        sheet.Cells.Style.Font.Name = "Microsoft YaHei UI";
        sheet.Cells.Style.Font.Size = 10;

        for (var column = 1; column <= TemplateHeaders.Length; column++)
        {
            var cell = sheet.Cells[1, column];
            cell.Value = TemplateHeaders[column - 1];
            cell.AddComment($"AML 字段：{AmlFieldNames[column - 1]}\n空白字段在覆盖模式下保持原值；新增模式下使用 Aras 默认值。", "开发团队");
        }

        using (var header = sheet.Cells[1, 1, 1, TemplateHeaders.Length])
        {
            header.Style.Font.Bold = true;
            header.Style.Font.Color.SetColor(System.Drawing.Color.White);
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 70, 229));
            header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.WrapText = true;
            header.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            header.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(67, 56, 202));
        }

        sheet.Row(1).Height = 42;
        sheet.Cells[1, 1, 1, TemplateHeaders.Length].AutoFilter = true;

        var nameColumn = FindHeaderColumn("名称");
        var dataTypeColumn = FindHeaderColumn("数据类型");
        var lengthColumn = FindHeaderColumn("长度");
        var precisionColumn = FindHeaderColumn("精度");
        var scaleColumn = FindHeaderColumn("小数位数");
        var sortOrderColumn = FindHeaderColumn("显示顺序(sort_order)");

        // 由模板实时给出默认值；服务端仍会再次执行同一规则，防止公式被删除或未重算。
        for (var row = FirstDataRow; row <= LastDataRow; row++)
        {
            var nameCell = sheet.Cells[row, nameColumn].Address;
            var dataTypeCell = sheet.Cells[row, dataTypeColumn].Address;
            sheet.Cells[row, precisionColumn].Formula = $"IF({dataTypeCell}=\"Decimal\",10,\"\")";
            sheet.Cells[row, scaleColumn].Formula = $"IF({dataTypeCell}=\"Decimal\",2,\"\")";
            sheet.Cells[row, sortOrderColumn].Formula =
                $"IF({nameCell}=\"\",\"\",100+(ROW()-{FirstDataRow})*10)";
            sheet.Cells[row, lengthColumn].Formula =
                $"IF({dataTypeCell}=\"\",\"\",IF(OR({dataTypeCell}=\"String\",{dataTypeCell}=\"Multilingual String\"),256,IF(OR({dataTypeCell}=\"List\",{dataTypeCell}=\"Filter List\",{dataTypeCell}=\"Color List\",{dataTypeCell}=\"Multi Value List\"),64,\"\")))";
        }

        using (var automaticRules = sheet.Cells[FirstDataRow, lengthColumn, LastDataRow, scaleColumn])
        {
            automaticRules.Style.Fill.PatternType = ExcelFillStyle.Solid;
            automaticRules.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(238, 242, 255));
            automaticRules.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(67, 56, 202));
        }
        using (var automaticSortOrder = sheet.Cells[FirstDataRow, sortOrderColumn, LastDataRow, sortOrderColumn])
        {
            automaticSortOrder.Style.Fill.PatternType = ExcelFillStyle.Solid;
            automaticSortOrder.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(238, 242, 255));
            automaticSortOrder.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(67, 56, 202));
        }

        var widths = new[]
        {
            20d, 20d, 22d, 24d, 12d, 12d, 14d, 12d, 12d, 20d, 20d, 24d, 14d, 20d
        };
        for (var column = 1; column <= widths.Length; column++)
            sheet.Column(column).Width = widths[column - 1];

        sheet.Cells[FirstDataRow, 1, LastDataRow, TemplateHeaders.Length].Style.VerticalAlignment =
            ExcelVerticalAlignment.Center;
        sheet.Cells[FirstDataRow, 1, LastDataRow, TemplateHeaders.Length].Style.Border.Bottom.Style =
            ExcelBorderStyle.Hair;
        sheet.Cells[FirstDataRow, 1, LastDataRow, TemplateHeaders.Length].Style.Border.Bottom.Color.SetColor(
            System.Drawing.Color.FromArgb(229, 231, 235));
    }

    private static void BuildDictionarySheet(ExcelPackage package, ExcelWorksheet dictionary)
    {
        dictionary.View.ShowGridLines = false;
        dictionary.Cells.Style.Font.Name = "Microsoft YaHei UI";
        dictionary.Cells.Style.Font.Size = 10;
        dictionary.Cells[1, 1, 1, 6].Merge = true;
        dictionary.Cells[1, 1].Value = "属性配置数据字典 · 模板填写标签，AML 自动转换为真实值";
        dictionary.Cells[1, 1].Style.Font.Bold = true;
        dictionary.Cells[1, 1].Style.Font.Size = 16;
        dictionary.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        dictionary.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        dictionary.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(49, 46, 129));
        dictionary.Row(1).Height = 32;

        var typeHeaders = new[] { "标签", "AML值", "默认长度", "默认精度", "默认小数位数", "数据源填写规则" };
        for (var column = 1; column <= typeHeaders.Length; column++)
            dictionary.Cells[3, column].Value = typeHeaders[column - 1];
        StyleDictionaryHeader(dictionary.Cells[3, 1, 3, typeHeaders.Length]);

        var firstTypeRow = 4;
        for (var index = 0; index < PropertyDataTypeOptions.All.Count; index++)
        {
            var option = PropertyDataTypeOptions.All[index];
            var row = firstTypeRow + index;
            dictionary.Cells[row, 1].Value = option.Label;
            dictionary.Cells[row, 2].Value = option.Value;
            dictionary.Cells[row, 3].Value = option.DefaultStoredLength;
            dictionary.Cells[row, 4].Value = option.DefaultPrecision;
            dictionary.Cells[row, 5].Value = option.DefaultScale;
            if (!string.IsNullOrWhiteSpace(option.DataSourceHint))
                dictionary.Cells[row, 6].Value = option.DataSourceHint;
        }

        dictionary.Cells[3, 8].Value = "布尔标签";
        dictionary.Cells[3, 9].Value = "AML值";
        StyleDictionaryHeader(dictionary.Cells[3, 8, 3, 9]);
        dictionary.Cells[4, 8].Value = "是";
        dictionary.Cells[4, 9].Value = "1";
        dictionary.Cells[5, 8].Value = "否";
        dictionary.Cells[5, 9].Value = "0";

        var notesRow = firstTypeRow + PropertyDataTypeOptions.All.Count + 2;
        dictionary.Cells[notesRow, 1, notesRow, 9].Merge = true;
        dictionary.Cells[notesRow, 1].Value =
            "填写规则：数据类型必须从下拉框选择；String/Multilingual String 默认长度 256，List/Filter List/Color List/Multi Value List 默认长度 64，Decimal 默认精度 10、小数位数 2；显示顺序从 100 起每行递增 10。Item 填 ItemType 名称，List 系列填 List 名称，Foreign 的数据源填当前对象类中已有 Item 属性名称，并填写引用外部属性名称。";
        dictionary.Cells[notesRow, 1].Style.WrapText = true;
        dictionary.Cells[notesRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
        dictionary.Cells[notesRow, 1].Style.Fill.BackgroundColor.SetColor(
            System.Drawing.Color.FromArgb(238, 242, 255));
        dictionary.Cells[notesRow, 1].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(49, 46, 129));
        dictionary.Row(notesRow).Height = 58;

        dictionary.View.FreezePanes(4, 1);
        dictionary.Column(1).Width = 24;
        dictionary.Column(2).Width = 22;
        dictionary.Column(3).Width = 14;
        dictionary.Column(4).Width = 14;
        dictionary.Column(5).Width = 16;
        dictionary.Column(6).Width = 38;
        dictionary.Column(8).Width = 16;
        dictionary.Column(9).Width = 12;

        package.Workbook.Names.Add("PropertyDataTypeLabels",
            dictionary.Cells[firstTypeRow, 1, firstTypeRow + PropertyDataTypeOptions.All.Count - 1, 1]);
        package.Workbook.Names.Add("PropertyBooleanLabels", dictionary.Cells[4, 8, 5, 8]);
    }

    private static void AddTemplateValidations(
        ExcelPackage package,
        ExcelWorksheet sheet,
        ExcelWorksheet dictionary)
    {
        _ = package;
        _ = dictionary;

        var dataTypeColumn = FindHeaderColumn("数据类型");
        var typeValidation = sheet.DataValidations.AddListValidation(
            sheet.Cells[FirstDataRow, dataTypeColumn, LastDataRow, dataTypeColumn].Address);
        typeValidation.Formula.ExcelFormula = "PropertyDataTypeLabels";
        typeValidation.AllowBlank = false;
        typeValidation.ShowErrorMessage = true;
        typeValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
        typeValidation.ErrorTitle = "数据类型无效";
        typeValidation.Error = "请从下拉清单选择 Aras 支持的数据类型标签，不能输入清单外的值。";
        typeValidation.ShowInputMessage = true;
        typeValidation.PromptTitle = "选择数据类型";
        typeValidation.Prompt = "模板保存标签，导入时自动转换为 AML 的小写真实值。";

        foreach (var header in BooleanHeaders)
        {
            var column = FindHeaderColumn(header);
            var validation = sheet.DataValidations.AddListValidation(
                sheet.Cells[FirstDataRow, column, LastDataRow, column].Address);
            validation.Formula.ExcelFormula = "PropertyBooleanLabels";
            validation.AllowBlank = true;
            validation.ShowErrorMessage = true;
            validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            validation.ErrorTitle = "布尔值无效";
            validation.Error = "请从下拉框选择“是”或“否”。";
        }

    }

    private static void StyleDictionaryHeader(ExcelRange range)
    {
        range.Style.Font.Bold = true;
        range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    }

    private static PropertyImportPreview ReadAndValidateTemplate(
        string filePath,
        CancellationToken cancellationToken)
    {
        ValidateTemplatePath(filePath);
        using var package = new ExcelPackage(new FileInfo(filePath));
        var sheet = package.Workbook.Worksheets[PropertySheetName]
            ?? throw new InvalidDataException($"Excel 中缺少“{PropertySheetName}”工作表，请使用最新模板。");
        if (sheet.Dimension == null)
            return new PropertyImportPreview();

        var headerMap = ReadHeaderMap(sheet);
        RequireHeaders(headerMap, "名称", "标签(简中)", "标签(繁中)", "标签(英文)", "数据类型");

        var rows = new List<PropertyImportPreviewRow>();
        for (var excelRow = FirstDataRow; excelRow <= sheet.Dimension.End.Row; excelRow++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!HasUserData(sheet, excelRow, headerMap))
                continue;

            var rowIndex = rows.Count;
            var rawDataType = ReadCell(sheet, excelRow, headerMap, "数据类型", "属性类型");
            var errors = new List<string>();
            if (!PropertyDataTypeOptions.TryResolve(rawDataType, out var option))
            {
                errors.Add($"数据类型“{rawDataType}”不在受支持清单中");
                option = new PropertyDataTypeOption(rawDataType, rawDataType.ToLowerInvariant());
            }

            var name = ReadCell(sheet, excelRow, headerMap, "名称", "属性名称");
            var labelZhCn = ReadCell(sheet, excelRow, headerMap, "标签(简中)", "属性标签");
            var labelZhTw = ReadCell(sheet, excelRow, headerMap, "标签(繁中)", "属性标签繁体");
            var labelEn = ReadCell(sheet, excelRow, headerMap, "标签(英文)", "属性标签英文");

            if (string.IsNullOrWhiteSpace(name)) errors.Add("名称不能为空");
            else if (!ArasNamePattern.IsMatch(name)) errors.Add("名称只能包含字母、数字和下划线，且不能以数字开头");
            if (string.IsNullOrWhiteSpace(labelZhCn)) errors.Add("简体中文标签不能为空");
            if (string.IsNullOrWhiteSpace(labelZhTw)) errors.Add("繁体中文标签不能为空");
            if (string.IsNullOrWhiteSpace(labelEn)) errors.Add("英文标签不能为空");

            var storedLength = NormalizeInteger(
                ReadCell(sheet, excelRow, headerMap, "长度"), option.DefaultStoredLength, "长度", errors, 1);
            var precision = NormalizeInteger(
                ReadCell(sheet, excelRow, headerMap, "精度"), option.DefaultPrecision, "精度", errors, 1);
            var scale = NormalizeInteger(
                ReadCell(sheet, excelRow, headerMap, "小数位数"), option.DefaultScale, "小数位数", errors, 0);
            var sortOrder = NormalizeInteger(
                ReadCell(sheet, excelRow, headerMap, "显示顺序(sort_order)", "显示顺序"),
                100 + rowIndex * 10, "显示顺序", errors, 0);

            if (option.Value == "decimal" &&
                int.TryParse(precision, NumberStyles.Integer, CultureInfo.InvariantCulture, out var precisionValue) &&
                int.TryParse(scale, NumberStyles.Integer, CultureInfo.InvariantCulture, out var scaleValue) &&
                scaleValue > precisionValue)
            {
                errors.Add("Decimal 小数位数不能大于精度");
            }

            var dataSource = ReadCell(sheet, excelRow, headerMap, "数据源");
            var foreignProperty = ReadCell(sheet, excelRow, headerMap, "引用外部属性");
            if (RequiresDataSource(option.Value) && string.IsNullOrWhiteSpace(dataSource))
                errors.Add($"{option.Label} 类型必须填写数据源");
            if (option.Value == "foreign" && string.IsNullOrWhiteSpace(foreignProperty))
                errors.Add("Foreign 类型必须填写引用外部属性");
            if (option.Value != "foreign" && !string.IsNullOrWhiteSpace(foreignProperty))
                errors.Add("仅 Foreign 类型可以填写引用外部属性");

            // 已从精简模板移除的字段仍按表头读取，以兼容上一版 33 列模板。
            var previewRow = new PropertyImportPreviewRow
            {
                ExcelRowNumber = excelRow,
                Name = name,
                LabelZhCn = labelZhCn,
                LabelZhTw = labelZhTw,
                LabelEn = labelEn,
                DataTypeLabel = option.Label,
                DataTypeValue = option.Value,
                DataSource = dataSource,
                ForeignProperty = foreignProperty,
                ClassPath = ReadCell(sheet, excelRow, headerMap, "对应分类"),
                ColumnAlignment = NormalizeAlignment(ReadCell(sheet, excelRow, headerMap, "文字对齐"), errors),
                ColumnWidth = NormalizeInteger(ReadCell(sheet, excelRow, headerMap, "显示宽度"), null, "显示宽度", errors, 0),
                DefaultSearch = ReadCell(sheet, excelRow, headerMap, "默认搜索值"),
                DefaultValueZhCn = ReadCell(sheet, excelRow, headerMap, "默认值(简中)", "默认值"),
                DefaultValueZhTw = ReadCell(sheet, excelRow, headerMap, "默认值(繁中)"),
                DefaultValueEn = ReadCell(sheet, excelRow, headerMap, "默认值(英文)"),
                HelpText = ReadCell(sheet, excelRow, headerMap, "帮助文本"),
                HelpTooltipZhCn = ReadCell(sheet, excelRow, headerMap, "提示(简中)"),
                HelpTooltipZhTw = ReadCell(sheet, excelRow, headerMap, "提示(繁中)"),
                HelpTooltipEn = ReadCell(sheet, excelRow, headerMap, "提示(英文)"),
                IsCopy = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "复制到新物件"), "复制到新物件", errors),
                IsFederated = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "Federated"), "Federated", errors),
                IsHidden = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "搜索中隐藏"), "搜索中隐藏", errors),
                IsHidden2 = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "关系中隐藏"), "关系中隐藏", errors),
                IsKeyed = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "唯一", "设置为键名"), "唯一", errors),
                IsRequired = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "必填"), "必填", errors),
                ItemBehavior = ReadCell(sheet, excelRow, headerMap, "对象行为"),
                KeyedNameOrder = NormalizeInteger(ReadCell(sheet, excelRow, headerMap, "键名顺序"), null, "键名顺序", errors, 0),
                OrderBy = NormalizeInteger(ReadCell(sheet, excelRow, headerMap, "排序顺序(order_by)"), null, "排序顺序", errors, 0),
                Pattern = ReadCell(sheet, excelRow, headerMap, "式样"),
                Precision = precision,
                Scale = scale,
                SortOrder = sortOrder,
                StoredLength = storedLength,
                TrackHistory = NormalizeBoolean(ReadCell(sheet, excelRow, headerMap, "追踪变更纪录"), "追踪变更纪录", errors),
                ValidationMessage = string.Join("；", errors.Distinct(StringComparer.Ordinal))
            };
            rows.Add(previewRow);
        }

        foreach (var duplicateGroup in rows
                     .Where(row => !string.IsNullOrWhiteSpace(row.Name))
                     .GroupBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            foreach (var duplicate in duplicateGroup)
                AddValidationError(duplicate, "模板内属性名称重复；同一 source_id + name 必须唯一");
        }

        return new PropertyImportPreview { Rows = rows };
    }

    private static string BuildPropertyAml(
        PropertyImportPreviewRow row,
        string itemTypeId,
        bool updateExisting)
    {
        var item = new XElement("Item",
            new XAttribute("type", "Property"),
            new XAttribute("action", updateExisting ? "edit" : "add"));
        if (updateExisting)
            item.Add(new XAttribute("id", row.ExistingPropertyId));

        AddValue(item, "class_path", row.ClassPath);
        AddValue(item, "column_alignment", row.ColumnAlignment);
        AddValue(item, "column_width", row.ColumnWidth);
        AddValue(item, "data_source", row.ResolvedDataSourceId);
        AddValue(item, "data_type", row.DataTypeValue);
        AddValue(item, "default_search", row.DefaultSearch);
        AddMultilingualValue(item, "default_value",
            row.DefaultValueEn, row.DefaultValueZhCn, row.DefaultValueZhTw);
        AddValue(item, "foreign_property", row.ResolvedForeignPropertyId);
        AddValue(item, "help_text", row.HelpText);
        AddMultilingualValue(item, "help_tooltip",
            row.HelpTooltipEn, row.HelpTooltipZhCn, row.HelpTooltipZhTw);
        AddValue(item, "is_copy", row.IsCopy);
        AddValue(item, "is_federated", row.IsFederated);
        AddValue(item, "is_hidden", row.IsHidden);
        AddValue(item, "is_hidden2", row.IsHidden2);
        AddValue(item, "is_keyed", row.IsKeyed);
        AddValue(item, "is_required", row.IsRequired);
        AddValue(item, "item_behavior", row.ItemBehavior);
        AddValue(item, "keyed_name_order", row.KeyedNameOrder);
        AddMultilingualValue(item, "label", row.LabelEn, row.LabelZhCn, row.LabelZhTw);
        AddValue(item, "name", row.Name);
        AddValue(item, "order_by", row.OrderBy);
        AddValue(item, "pattern", row.Pattern);
        AddValue(item, "prec", row.Precision);
        AddValue(item, "scale", row.Scale);
        AddValue(item, "sort_order", row.SortOrder);
        AddValue(item, "source_id", itemTypeId);
        AddValue(item, "stored_length", row.StoredLength);
        AddValue(item, "track_history", row.TrackHistory);

        var aml = new XElement("AML",
            new XAttribute(XNamespace.Xmlns + "i18n", I18n),
            item);
        return aml.ToString(SaveOptions.DisableFormatting);
    }

    private static void ResolveDataSourceReferences(
        Innovator innovator,
        string itemTypeId,
        PropertyImportPreviewRow row,
        Dictionary<string, string> cache)
    {
        if (string.IsNullOrWhiteSpace(row.DataSource))
            return;

        switch (row.DataTypeValue)
        {
            case "item":
                row.ResolvedDataSourceId = ResolveNamedItemId(
                    innovator, "ItemType", row.DataSource, cache);
                break;
            case "list":
            case "filter list":
            case "color list":
            case "mv_list":
                row.ResolvedDataSourceId = ResolveNamedItemId(
                    innovator, "List", row.DataSource, cache);
                break;
            case "sequence":
                row.ResolvedDataSourceId = ResolveNamedItemId(
                    innovator, "Sequence", row.DataSource, cache);
                break;
            case "foreign":
                ResolveForeignReferences(innovator, itemTypeId, row);
                break;
            default:
                throw new InvalidDataException(
                    $"{row.DataTypeLabel} 类型不应填写数据源；请清空“数据源”列");
        }
    }

    private static void ResolveForeignReferences(
        Innovator innovator,
        string sourceItemTypeId,
        PropertyImportPreviewRow row)
    {
        var sourceProperty = FindPropertyByNameOrId(
            innovator, sourceItemTypeId, row.DataSource,
            $"Foreign 数据源属性“{row.DataSource}”不存在于当前对象类");
        var sourceDataType = sourceProperty.getProperty("data_type", string.Empty);
        var targetItemTypeId = sourceProperty.getProperty("data_source", string.Empty);
        if (!sourceDataType.Equals("item", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(targetItemTypeId))
        {
            throw new InvalidDataException(
                $"Foreign 数据源“{row.DataSource}”必须是当前对象类中已有且设置了 ItemType 数据源的 Item 属性");
        }

        var targetProperty = FindPropertyByNameOrId(
            innovator, targetItemTypeId, row.ForeignProperty,
            $"目标 ItemType 中不存在外部属性“{row.ForeignProperty}”");
        row.ResolvedDataSourceId = sourceProperty.getID();
        row.ResolvedForeignPropertyId = targetProperty.getID();
    }

    private static Item? FindProperty(Innovator innovator, string sourceItemTypeId, string propertyName)
    {
        var query = innovator.newItem("Property", "get");
        query.setAttribute("select", "id,name,data_type,data_source");
        query.setProperty("source_id", sourceItemTypeId);
        query.setProperty("name", propertyName);
        var response = query.apply();

        // R37 的“No items found”也会使 isError() 为 true，但 getItemCount() 为 0。
        // 先识别空结果，使覆盖模式可以新增；真实查询错误仍由 ThrowIfError 拦截。
        var count = response.getItemCount();
        if (count == 0)
            return null;

        ThrowIfError(response, $"查询属性“{propertyName}”失败");
        if (count > 1)
            throw new InvalidOperationException(
                $"检测到多个同名属性“{propertyName}”；source_id + name 必须唯一，请先在 Aras 中修正数据");
        return count == 1 ? response.getItemByIndex(0) : null;
    }

    private static Item FindPropertyByNameOrId(
        Innovator innovator,
        string sourceItemTypeId,
        string nameOrId,
        string notFoundMessage)
    {
        var query = innovator.newItem("Property", "get");
        query.setAttribute("select", "id,name,data_type,data_source,source_id");
        if (ArasIdPattern.IsMatch(nameOrId))
            query.setID(nameOrId.ToUpperInvariant());
        else
        {
            query.setProperty("source_id", sourceItemTypeId);
            query.setProperty("name", nameOrId);
        }

        var response = query.apply();
        ThrowIfError(response, $"查询 Property“{nameOrId}”失败");
        if (response.getItemCount() != 1)
            throw new InvalidDataException(notFoundMessage);

        var property = response.getItemByIndex(0);
        var actualSourceId = property.getProperty("source_id", string.Empty);
        if (!actualSourceId.Equals(sourceItemTypeId, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(notFoundMessage);
        return property;
    }

    private static string ResolveNamedItemId(
        Innovator innovator,
        string itemType,
        string nameOrId,
        IDictionary<string, string> cache)
    {
        var cacheKey = $"{itemType}:{nameOrId}";
        if (cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var query = innovator.newItem(itemType, "get");
        query.setAttribute("select", "id,name");
        if (ArasIdPattern.IsMatch(nameOrId))
            query.setID(nameOrId.ToUpperInvariant());
        else
            query.setProperty("name", nameOrId);

        var response = query.apply();
        ThrowIfError(response, $"查询 {itemType} 数据源“{nameOrId}”失败");
        if (response.getItemCount() != 1)
            throw new InvalidDataException(
                $"无法唯一解析 {itemType} 数据源“{nameOrId}”；请填写系统中的 name 或 32 位 GUID");

        var id = response.getItemByIndex(0).getID();
        cache[cacheKey] = id;
        return id;
    }

    private static void VerifySelectedItemType(
        Innovator innovator,
        string itemTypeId,
        string itemTypeName)
    {
        var query = innovator.newItem("ItemType", "get");
        query.setAttribute("select", "id,name");
        query.setID(itemTypeId);
        var response = query.apply();
        ThrowIfError(response, "校验所选对象类失败");
        if (response.getItemCount() != 1 ||
            !response.getItemByIndex(0).getProperty("name", string.Empty)
                .Equals(itemTypeName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("所选对象类已不存在或当前连接已切换，请刷新对象类后重新选择。");
        }
    }

    private Innovator GetInnovator() => _connectionService.TypedInnovator
        ?? throw new InvalidOperationException("未连接到 Aras 系统，请先登录当前用户的默认连接。");

    private static void AddValue(XElement item, string propertyName, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            item.Add(new XElement(propertyName, value));
    }

    private static void AddMultilingualValue(
        XElement item,
        string propertyName,
        string english,
        string simplifiedChinese,
        string traditionalChinese)
    {
        if (!string.IsNullOrWhiteSpace(english))
            item.Add(new XElement(propertyName,
                new XAttribute(Xml + "lang", "en"), english));
        if (!string.IsNullOrWhiteSpace(simplifiedChinese))
            item.Add(new XElement(I18n + propertyName,
                new XAttribute(Xml + "lang", "zc"), simplifiedChinese));
        if (!string.IsNullOrWhiteSpace(traditionalChinese))
            item.Add(new XElement(I18n + propertyName,
                new XAttribute(Xml + "lang", "zt"), traditionalChinese));
    }

    private static Dictionary<string, int> ReadHeaderMap(ExcelWorksheet sheet)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var column = 1; column <= sheet.Dimension.End.Column; column++)
        {
            var header = sheet.Cells[1, column].Text?.Trim();
            if (!string.IsNullOrWhiteSpace(header) && !result.ContainsKey(header))
                result[header] = column;
        }
        return result;
    }

    private static void RequireHeaders(
        IReadOnlyDictionary<string, int> headerMap,
        params string[] requiredHeaders)
    {
        var missing = requiredHeaders.Where(header => !headerMap.ContainsKey(header)).ToList();
        if (missing.Count > 0)
            throw new InvalidDataException(
                $"模板缺少必要列：{string.Join("、", missing)}。请重新下载最新模板。");
    }

    private static bool HasUserData(
        ExcelWorksheet sheet,
        int row,
        IReadOnlyDictionary<string, int> headerMap)
    {
        foreach (var column in headerMap.Values)
        {
            if (!string.IsNullOrWhiteSpace(sheet.Cells[row, column].Text))
                return true;
        }
        return false;
    }

    private static string ReadCell(
        ExcelWorksheet sheet,
        int row,
        IReadOnlyDictionary<string, int> headerMap,
        params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (headerMap.TryGetValue(alias, out var column))
                return sheet.Cells[row, column].Text?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }

    private static string NormalizeBoolean(string input, string fieldName, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        return input.Trim().ToLowerInvariant() switch
        {
            "是" or "1" or "true" or "yes" or "y" or "真" => "1",
            "否" or "0" or "false" or "no" or "n" or "假" => "0",
            _ => AddNormalizationError(errors, $"{fieldName}只能填写“是/否”")
        };
    }

    private static string NormalizeAlignment(string input, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        return input.Trim().ToLowerInvariant() switch
        {
            "左对齐" or "left" => "left",
            "居中" or "center" or "centre" => "center",
            "右对齐" or "right" => "right",
            _ => AddNormalizationError(errors, "文字对齐只能选择左对齐、居中或右对齐")
        };
    }

    private static string NormalizeInteger(
        string input,
        int? defaultValue,
        string fieldName,
        ICollection<string> errors,
        int minimum)
    {
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ||
            value < minimum)
        {
            errors.Add($"{fieldName}必须是不小于 {minimum} 的整数");
            return input;
        }
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string AddNormalizationError(ICollection<string> errors, string message)
    {
        errors.Add(message);
        return string.Empty;
    }

    private static bool RequiresDataSource(string dataType) => dataType is
        "item" or "list" or "filter list" or "color list" or "mv_list" or "sequence" or "foreign";

    private static void AddValidationError(PropertyImportPreviewRow row, string message)
    {
        row.ValidationMessage = string.IsNullOrWhiteSpace(row.ValidationMessage)
            ? message
            : $"{row.ValidationMessage}；{message}";
    }

    private static int FindHeaderColumn(string header) =>
        Array.FindIndex(TemplateHeaders, value => value.Equals(header, StringComparison.Ordinal)) + 1;

    private static void ValidateTemplatePath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException("所选模板文件不存在。", filePath);
        if (!Path.GetExtension(filePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("属性配置仅支持 .xlsx 文件，请使用工具下载的最新模板。");
    }

    private static void ValidateSelectedItemTypeArguments(string itemTypeId, string itemTypeName)
    {
        if (!ArasIdPattern.IsMatch(itemTypeId ?? string.Empty))
            throw new ArgumentException("所选对象类 GUID 无效，请刷新后重新选择。", nameof(itemTypeId));
        ArgumentException.ThrowIfNullOrWhiteSpace(itemTypeName);
    }

    private static string NormalizeImportMode(string importMode) => importMode switch
    {
        "新增" => "新增",
        "覆盖" => "覆盖",
        _ => throw new ArgumentException("导入模式只能选择“新增”或“覆盖”。", nameof(importMode))
    };

    private static void ThrowIfError(Item result, string context)
    {
        if (result.isError())
            throw new InvalidOperationException($"{context}: {result.getErrorString()}");
    }

    private async Task SaveLogAsync(PropertyImportLog log)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            db.PropertyImportLogs.Add(log);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-保存导入日志", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private async Task TryLogOperationAsync(string logId, string message)
    {
        try
        {
            await _operationLogService.LogAsync("Import", "PropertyImportLog", logId, message)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("属性配置-记录敏感操作", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
        }
    }

    private async Task TrySaveFailedLogAsync(
        string relativePath,
        string errorDetail,
        PropertyImportResult result)
    {
        try
        {
            await SaveLogAsync(new PropertyImportLog
            {
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ImportTime = DateTime.Now,
                ImportFile = relativePath,
                Status = PropertyImportLog.StatusFailed,
                ErrorLog = errorDetail,
                Sheet1Count = result.Sheet1Count,
                CreatorOn = DateTime.Now
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PropertyImport] 失败日志保存失败: {ex.Message}");
        }
    }
}
