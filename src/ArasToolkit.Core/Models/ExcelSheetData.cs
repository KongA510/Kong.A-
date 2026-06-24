using System.Collections.ObjectModel;
using System.Data;

namespace ArasToolkit.Core.Models;

/// <summary>
/// Excel表格数据模型
/// </summary>
public class ExcelSheetData
{
    public string SheetName { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new();
    public ObservableCollection<string> ColumnHeaders { get; set; } = new();
 
     /// <summary>列映射列表（A→真实列头）</summary>
     public ObservableCollection<ColumnMapping> ColumnMappings { get; set; } = new();
}
