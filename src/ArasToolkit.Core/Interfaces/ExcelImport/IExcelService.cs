using System.Collections.ObjectModel;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Excel服务接口
/// </summary>
public interface IExcelService
{
    /// <summary>
    /// 获取Excel文件中所有Sheet名称
    /// </summary>
    Task<List<string>> GetSheetNamesAsync(string filePath);
    
    /// <summary>
    /// 读取指定Sheet的数据
    /// </summary>
    Task<ExcelSheetData> ReadSheetAsync(string filePath, string sheetName);
    
    /// <summary>
    /// 读取Excel文件并返回所有Sheet数据
    /// </summary>
    Task<List<ExcelSheetData>> ReadAllSheetsAsync(string filePath);
}
