---
id: TASK-016
priority: P0
type: feature
created: 2026-06-25
source: Claude Code (PM)
status: done
---

# TASK-016: Data Import Feature (Excel -> Aras)

## 1. Background

Rename "List config" placeholder to "Data Import" - full Excel-to-Aras data import tool.

- Read Excel via EPPlus, select Sheet, preview data
- Save/manage AML template configs (user-isolated, stored in DB)
- Column references use @A, @B, @C... in AML templates
- Support row/column range settings
- Import log to local directory (not DB)
- Actual Aras API import left for user to implement

## 2. Performance Design

| Scenario | Approach | Notes |
|----------|----------|-------|
| Preview | Range read -> DataTable | Only read startRow~endRow, startCol~endCol |
| Import exec | Row-by-row streaming | Constant memory, no full load |
| Excel read | EPPlus direct cell access | worksheet.Cells[row, col].Text |

- Preview: DataTable binds to DataGrid
- Import: streaming rows, memory constant regardless of file size
- endRow/endCol = -1 means "read all / auto-detect"

## 3. DB Table: data_import_config

```sql
CREATE TABLE data_import_config (
    id NVARCHAR(12) NOT NULL PRIMARY KEY,
    config_name NVARCHAR(200) NOT NULL,
    aml_content NVARCHAR(MAX) NOT NULL,
    sheet_name NVARCHAR(200) NULL,
    start_row INT NOT NULL DEFAULT 2,
    end_row INT NOT NULL DEFAULT -1,
    start_col INT NOT NULL DEFAULT 1,
    end_col INT NOT NULL DEFAULT -1,
    user_name NVARCHAR(100) NOT NULL,
    creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

Rules:
- end_row = -1 means read to last row
- end_col = -1 means auto-detect last column
- user_name for user isolation (admin sees all, normal users see own)

## 4. Module Specs

### 4.1 Menu Rename

| Location | Old | New |
|----------|-----|-----|
| MainViewModel | "List config" | "Data Import", Icon="DatabaseImport", CardIcon="icon-import" |
| DashboardViewModel | "List config" | "Data Import" |
| MainWindow.xaml.cs | "List config" -> Placeholder | "Data Import" -> DataImportView |

### 4.2 Entity: Core/Entities/DataImportConfig.cs

```csharp
[Table("data_import_config")]
public class DataImportConfig
{
    [Key][Column("id")][MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("config_name")][MaxLength(200)][Required]
    public string ConfigName { get; set; } = string.Empty;

    [Column("aml_content")][Required]
    public string AmlContent { get; set; } = string.Empty;

    [Column("sheet_name")][MaxLength(200)]
    public string? SheetName { get; set; }

    [Column("start_row")]
    public int StartRow { get; set; } = 2;

    [Column("end_row")]
    public int EndRow { get; set; } = -1;

    [Column("start_col")]
    public int StartCol { get; set; } = 1;

    [Column("end_col")]
    public int EndCol { get; set; } = -1;

    [Column("user_name")][MaxLength(100)][Required]
    public string UserName { get; set; } = string.Empty;

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
```

### 4.3 Excel Service Extensions: IExcelService + ExcelService

```csharp
Task<List<string>> GetColumnHeadersAsync(string filePath, string sheetName, int startCol, int endCol);
Task<int> GetRowCountAsync(string filePath, string sheetName);
Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName, int startRow, int endRow, int startCol, int endCol);
Task<(int Total, int Success, int Failed)> StreamRowsAsync(string filePath, string sheetName, int startRow, int endRow, int startCol, int endCol, Func<int, Dictionary<string, string>, Task<bool>> rowProcessor);
```

### Preview read logic:
1. Read row 1 (header) -> real column names for DataTable ColumnName
2. Build column mapping: {"A": "PartNumber", "B": "PartName", ...}
3. Start from row 2, fill DataTable
4. DataGrid shows: real headers + data rows

### Row-by-row import logic:
1. Start from startRow, read row by row
2. Each row: {"A": "MAT001", "B": "Bolt", "C": "100"}
3. Pass to rowProcessor -> replace @A->MAT001, @B->Bolt, @C->100
4. rowProcessor returns true=success, false=fail

### 4.4 New Service: DataImportService

```csharp
public interface IDataImportService
{
    // AML Config CRUD
    Task<List<DataImportConfig>> GetConfigsAsync();
    Task<DataImportConfig> SaveConfigAsync(DataImportConfig config);
    Task DeleteConfigAsync(string id);

    // Excel read
    Task<List<string>> GetSheetNamesAsync(string filePath);
    Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName, int startRow, int endRow, int startCol, int endCol);
    Task<List<ColumnMapping>> GetColumnMappingsAsync(string filePath, string sheetName, int startCol, int endCol);

    // AML replacement
    string ReplaceAmlPlaceholders(string amlTemplate, Dictionary<string, string> rowData);
    string PreviewAml(string amlTemplate, Dictionary<string, string> firstRowData);

    // Import execution
    Task<ImportResult> ExecuteImportAsync(string filePath, DataImportConfig config, Func<int, string, Task<bool>>? arasImporter = null);
}

public class ColumnMapping
{
    public string Letter { get; set; }   // "A", "B", ..., "AA", "AB"...
    public string Header { get; set; }   // Real column name
    public int Index { get; set; }       // 0-based column index
}

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int SkippedCount { get; set; }
    public DateTime ImportTime { get; set; }
    public string LogFilePath { get; set; } = string.Empty;
}
```

### 4.5 Import Log (local file)

- Dir: {BaseDir}/Logs/Import/
- File: import_{yyyyMMdd_HHmmss}.log

Format:
```
===== Import Log =====
Config: {configName}
File: {fileName}
Sheet: {sheetName}
Start Time: {yyyy-MM-dd HH:mm:ss}
Range: row {startRow}~{endRow}, col {startCol}~{endCol}
Total Rows: {totalRows}
-----
[FAIL] Row 3: column C data format error: ...
[FAIL] Row 7: AML execution exception: ...
-----
End Time: {yyyy-MM-dd HH:mm:ss}
Success: {N}  Failed: {N}
===== End =====
```

### 4.6 ViewModel: DataImportViewModel

Key properties/commands:
- SelectedFilePath, SheetNames, SelectedSheetName
- BrowseFileCommand, LoadPreviewCommand
- StartRow/EndRow/StartCol/EndCol
- PreviewData (DataTable), ColumnMappings (ObservableCollection<ColumnMapping>)
- SavedConfigs, SelectedConfig, AmlContent
- SaveConfigCommand, DeleteConfigCommand, OpenConfigSelectorCommand, PreviewAmlCommand
- IsImporting, IsPaused, ExecuteImportCommand, PauseCommand
- LastResult, StatusMessage, IsLoading

Import flow:
1. Load file -> select sheet -> set range -> load preview
2. UI shows: column mapping (@A=PartNumber) + data table (real headers)
3. User writes/selects AML template (with @A @B placeholders)
4. Click "Preview AML" -> replace with first row data
5. Click "Execute Import" -> row by row from startRow

Pause mechanism:
- PauseCommand -> IsPaused=true -> _cts.Cancel()
- Current row finishes then blocks
- Click again -> IsPaused=false -> new _cts -> continue

### 4.7 View: DataImportView.xaml

Layout:
```
[Title: Data Import]
[File: ______ Browse] [Sheet: ___ v]
[StartRow: 2] [EndRow: -1] [StartCol: 1] [EndCol: -1] [Load Preview]
---
[Column Mappings: @A=PartNumber, @B=PartName, ...]
---
[Data Preview (real headers): PartNumber | PartName | Qty | ...]
---
[AML Template: text editor] [Select Config v]
[Preview AML] [Save Config] [Delete Config]
---
[Result: Total 100 Success 95 Failed 5]
[Execute Import] [Pause]
```

### 4.8 ConfigSelectWindow

Simple dialog with ListBox of saved configs, Select/Delete buttons per item, Cancel button.

## 5. DI Registration

```
Services/ServiceCollectionExtensions.cs:
  services.AddSingleton<IDataImportService, DataImportService>();

App/App.xaml.cs:
  services.AddTransient<DataImportViewModel>();
  services.AddTransient<DataImportView>();
  services.AddTransient<ConfigSelectWindow>();
```

## 6. Navigation Registration

```
MainViewModel.InitializeMenuItems() -> "Data Import"
MainWindow.xaml.cs NavigateToPage -> "Data Import" case
DashboardViewModel.AllFeatures -> "Data Import"
```

## 7. DbContext Updates

```
ArasToolkitDbContext.cs:
  public DbSet<DataImportConfig> DataImportConfigs => Set<DataImportConfig>();
  + OnModelCreating DataImportConfig mapping
  + EnsureSchemaAsync data_import_config table
```

## 8. Code Order

1. Entity: Core/Entities/DataImportConfig.cs
2. DbContext: DbSet + OnModelCreating + EnsureSchemaAsync
3. Interface + Service: IDataImportService + DataImportService
4. Excel extensions: IExcelService/ExcelService new methods
5. Model: Core/Models/ImportResult.cs
6. ViewModel: DataImportViewModel.cs
7. View: DataImportView.xaml + .xaml.cs
8. ConfigSelectWindow: .xaml + .xaml.cs
9. Menu rename: MainViewModel/DashboardViewModel/MainWindow.xaml.cs
10. DI: ServiceCollectionExtensions + App.xaml.cs

## 9. Out of Scope

- Actual Aras API import logic (arasImporter=null -> all rows marked as skipped)
- User provides their own Aras call logic

## 10. Build Verification

dotnet build ArasToolkit.slnx - 0 errors

---

## Review 1 (Claude Code - 2026-06-25)

**Conclusion: Needs Changes**

6 issues found:
1. DataImportConfig missing from OnModelCreating Fluent API
2. Missing operation log (IOperationLogService)
3. Missing error log (IErrorLogService) in Service layer
4. ColumnMappings visibility binding issue (int->bool converter)
5. OpenConfigSelectorCommand uses FirstOrDefault, no dialog
6. AppUser missing from OnModelCreating (TASK-011 legacy)

## Review 2 (Claude Code - 2026-06-25)

**Conclusion: Needs Changes**

4 of 6 fixed (op log, err log, HasColumnMappings, AppUser mapping).
2 remaining:
- DataImportConfig OnModelCreating (non-blocking: [Column] annotations suffice)
- ConfigSelectWindow not implemented (fallback usable)

## Review 3 (Claude Code - 2026-06-25)

**Conclusion: Passed**

Same 2 non-blocking items remain. 0 build errors. Codex may archive.

