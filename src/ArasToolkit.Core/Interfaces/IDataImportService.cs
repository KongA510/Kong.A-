using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 数据导入服务接口 — Excel→Aras数据导入
/// </summary>
public interface IDataImportService
{
    // ---- AML 配置 CRUD ----
    Task<List<DataImportConfig>> GetConfigsAsync();
    Task<DataImportConfig> SaveConfigAsync(DataImportConfig config);
    Task DeleteConfigAsync(string id);

    // ---- Excel 读取 ----
    Task<List<string>> GetSheetNamesAsync(string filePath);
    Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
        int startRow, int endRow, int startCol, int endCol);
    Task<List<ColumnMapping>> GetColumnMappingsAsync(string filePath, string sheetName,
        int startCol, int endCol);

    // ---- AML 替换 ----
    string ReplaceAmlPlaceholders(string amlTemplate, Dictionary<string, string> rowData);
    string PreviewAml(string amlTemplate, Dictionary<string, string> firstRowData);

  // ---- 导入执行 ----
   /// <summary>
   /// 执行导入 — AML 在 Service 内部通过 Aras API 执行，回调仅通知进度
   /// </summary>
   /// <param name="progressCallback">进度回调：(当前行号, 总行数)，为 null 时不汇报进度</param>
   Task<ImportResult> ExecuteImportAsync(
        string filePath, string? sheetName,
        int startRow, int endRow, int startCol, int endCol,
        string amlContent,
        Func<int, int, Task>? progressCallback = null);
}
