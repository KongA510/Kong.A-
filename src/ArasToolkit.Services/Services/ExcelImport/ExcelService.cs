using System.Data;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Excel服务实现 - 使用EPPlus读取Excel文件
/// </summary>
public class ExcelService : IExcelService
{
    public ExcelService()
    {
        // 设置EPPlus许可证上下文
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public Task<List<string>> GetSheetNamesAsync(string filePath)
    {
        return Task.Run(() =>
        {
            var sheetNames = new List<string>();
            
            using var package = new ExcelPackage(new FileInfo(filePath));
            foreach (var sheet in package.Workbook.Worksheets)
            {
                sheetNames.Add(sheet.Name);
            }
            
            return sheetNames;
        });
    }

    public Task<ExcelSheetData> ReadSheetAsync(string filePath, string sheetName)
    {
        return Task.Run(() =>
        {
            var sheetData = new ExcelSheetData { SheetName = sheetName };
            
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[sheetName];
            
            if (worksheet == null)
                throw new ArgumentException($"Sheet '{sheetName}' not found.");

            if (worksheet.Dimension == null)
                return sheetData;

            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;
            
            sheetData.Data = new DataTable();
            
            // 读取列头（第一行）
            for (int col = 1; col <= colCount; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim() ?? $"Column{col}";
                sheetData.Data.Columns.Add(header);
                sheetData.ColumnHeaders.Add(header);
            }
            
            // 读取数据行（从第二行开始）
            for (int row = 2; row <= rowCount; row++)
            {
                var dataRow = sheetData.Data.NewRow();
                for (int col = 1; col <= colCount; col++)
                {
                    dataRow[col - 1] = worksheet.Cells[row, col].Text?.Trim() ?? "";
                }
                sheetData.Data.Rows.Add(dataRow);
            }
            
            return sheetData;
        });
    }

    public async Task<List<ExcelSheetData>> ReadAllSheetsAsync(string filePath)
    {
        var sheetNames = await GetSheetNamesAsync(filePath);
        var result = new List<ExcelSheetData>();
        
        foreach (var sheetName in sheetNames)
        {
            var data = await ReadSheetAsync(filePath, sheetName);
            result.Add(data);
        }
        
        return result;
    }
}
