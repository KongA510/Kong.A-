---
id: TASK-016
priority: P0
type: feature
created: 2026-06-25
source: Claude Code（产品经理）
status: pending
---

# TASK-016: 数据汇入功能（Excel → Aras 数据导入工具）

## 一、需求背景

当前"List配置"为占位项。需将其改造为完整的**数据汇入**功能：

- 读取 Excel 文件（EPPlus），选择 Sheet，预览数据
- 用户可保存/管理 AML 模板配置（按用户隔离，存入数据库）
- 数据列使用 @A、@B、@C... 引用（Excel 列号映射），预览 AML 时替换为实际值
- 支持设置读取行范围、列范围
- 汇入日志记录到本机目录（非数据库），汇入完成后记录汇总信息（时间/成功数/失败数）
- **实际 Aras API 汇入由用户后续自行编写**，只需提供空壳方法 + 调用占位

## 二、性能方案分析（产品经理决策）

| 场景 | 方案 | 说明 |
|------|------|------|
| **预览** | 按范围读取 → DataTable | 只读 startRow~endRow, startCol~endCol，UI 绑定显示 |
| **汇入执行** | 逐行流式读取 | 不一次性加载全部数据到内存，读完一行处理一行 |
| **Excel 读取** | EPPlus 直接单元格访问 | `worksheet.Cells[row, col].Text`，不经过 DataTable 中转 |

**决策理由**：
- 预览时用户需要看表格，DataTable 绑定 DataGrid 最方便，但只加载指定范围（不是全部数据）
- 实际汇入时逐行处理，内存占用恒定，不受文件大小影响
- 列范围支持空/ -1 = 自动检测有数据的最后一列

---

## 三、数据库表设计

### 3.1 新建表: data_import_config（AML配置存储）

```sql
CREATE TABLE data_import_config (
    id NVARCHAR(12) NOT NULL PRIMARY KEY,
    config_name NVARCHAR(200) NOT NULL,          -- 配置名称（用户自定义）
    aml_content NVARCHAR(MAX) NOT NULL,           -- AML 模板内容（含 @A @B 占位符）
    sheet_name NVARCHAR(200) NULL,                -- 关联的 Sheet 名称
    start_row INT NOT NULL DEFAULT 2,             -- 数据起始行（默认2，跳过表头）
    end_row INT NOT NULL DEFAULT -1,              -- 数据结束行（-1=全部）
    start_col INT NOT NULL DEFAULT 1,             -- 数据起始列
    end_col INT NOT NULL DEFAULT -1,              -- 数据结束列（-1=自动检测）
    user_name NVARCHAR(100) NOT NULL,             -- 所属用户（CurrentUserContext）
    creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

**铁律**：
- `end_row = -1` 表示读取至最后一行
- `end_col = -1` 表示自动检测最后一列（从 start_col 开始向右扫描到第一个全空列为止）
- `user_name` 用于用户隔离（管理员可见全部，普通用户仅见自己）

---

## 四、功能模块清单

### 4.1 菜单重命名

| 位置 | 旧值 | 新值 |
|------|------|------|
| `MainViewModel.InitializeMenuItems()` | `Name = "List配置"`, `Icon = "FormatListBulleted"` | `Name = "数据汇入"`, `Icon = "DatabaseImport"`, `IsPlaceholder` 移除 |
| `DashboardViewModel.AllFeatures` | `"List配置"` / `"📋"` / `"Aras List配置工具"` | `"数据汇入"` / `"📥"` / `"Excel数据导入Aras系统"` |
| `MainWindow.xaml.cs` NavigateToPage | `"List配置"` → Placeholder | `"数据汇入"` → DataImportView |

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

### 4.3 Excel 服务扩展: IExcelService + ExcelService

新增方法：

```csharp
// 获取 Sheet 的列数（自动检测数据范围）
Task<int> GetColumnCountAsync(string filePath, string sheetName);

// 生成列字母列表（如 ["A","B","C",...,"Z","AA","AB"...]）
List<string> GetColumnLetters(int count);

// 按范围读取 Sheet 数据（预览用，返回 DataTable）
// endRow=-1 → 读到最后一行, endCol=-1 → 自动检测最后一列
Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
    int startRow, int endRow, int startCol, int endCol);

// 逐行流式读取（汇入用，callback 每行处理）
// 返回: (totalRows, successCount, failureCount)
Task<(int Total, int Success, int Failed)> StreamRowsAsync(
    string filePath, string sheetName,
    int startRow, int endRow, int startCol, int endCol,
    Func<int, Dictionary<string, string>, Task<bool>> rowProcessor);
```

**列字母映射表**：
```
1→A, 2→B, ..., 26→Z, 27→AA, 28→AB, ..., 52→AZ, 53→BA, ...
```
EPPlus 自带 `ExcelCellAddress.GetColumnLetter(col)` 可直接使用。

### 4.4 新建 Service: DataImportService

```csharp
public interface IDataImportService
{
    // ---- AML 配置 CRUD ----
    Task<List<DataImportConfig>> GetConfigsAsync();           // 用户隔离
    Task<DataImportConfig> SaveConfigAsync(DataImportConfig config);
    Task DeleteConfigAsync(string id);

    // ---- 列字母工具 ----
    List<string> GetColumnLetters(int count);

    // ---- Excel 读取（委托给 IExcelService） ----
    Task<List<string>> GetSheetNamesAsync(string filePath);
    Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
        int startRow, int endRow, int startCol, int endCol);

    // ---- AML 预览 ----
    // 将 AML 中的 @A @B 替换为实际数据行对应列的值
    string PreviewAml(string amlTemplate, Dictionary<string, string> rowData);

    // ---- 汇入执行（壳方法） ----
    // 逐行读取 Excel → 调用 rowProcessor → 写日志
    // 实际 Aras API 调用由用户在 rowProcessor 中实现
    Task<ImportResult> ExecuteImportAsync(
        string filePath, DataImportConfig config,
        Func<int, Dictionary<string, string>, string, Task<bool>>? arasImporter = null);
}

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public DateTime ImportTime { get; set; }
    public string LogFilePath { get; set; } = string.Empty;
}
```

### 4.5 汇入日志（本机文件）

日志目录：`{BaseDir}/Logs/Import/`
文件名：`import_{yyyyMMdd_HHmmss}.log`

格式：
```
===== 数据汇入日志 =====
配置: {configName}
文件: {fileName}
Sheet: {sheetName}
开始时间: {yyyy-MM-dd HH:mm:ss}
范围: 行{startRow}~{endRow}, 列{startCol}~{endCol}
总行数: {totalRows}
-----
[失败] 行3: 第C列数据格式错误: ...
[失败] 行7: AML执行异常: ...
-----
结束时间: {yyyy-MM-dd HH:mm:ss}
成功: {successCount}  失败: {failureCount}
===== 日志结束 =====
```

**铁律**：
- 成功的行不写日志（减少IO）
- 失败的行立即追加写入
- 汇入完成后写汇总
- 日志目录不存在时自动创建

### 4.6 ViewModel: DataImportViewModel

核心属性/命令：

```csharp
// ---- 文件选择 ----
string SelectedFilePath                    // 选择的 Excel 文件路径
ObservableCollection<string> SheetNames    // Sheet 名称列表
string? SelectedSheetName                  // 当前选中 Sheet
ICommand BrowseFileCommand               // 浏览文件 → 加载 Sheet 列表

// ---- 读取范围 ----
int StartRow { get; set; } = 2           // 数据起始行
int EndRow { get; set; } = -1            // 结束行（-1=全部）
int StartCol { get; set; } = 1           // 起始列
int EndCol { get; set; } = -1            // 结束列（-1=自动）
ICommand LoadPreviewCommand             // 按范围加载数据到预览表格

// ---- 数据预览 ----
DataTable? PreviewData                    // 预览数据（列名为 @A @B ...）
ObservableCollection<string> ColumnLetters // 列字母列表
ICommand LoadSheetsCommand              // 选中 Sheet 后加载列信息

// ---- AML 配置管理 ----
ObservableCollection<DataImportConfig> SavedConfigs  // 已保存配置列表
DataImportConfig? SelectedConfig          // 当前选中配置
string AmlContent                         // 当前 AML 模板内容（双向绑定）
ICommand SaveConfigCommand             // 保存当前 AML + 范围设置
ICommand DeleteConfigCommand           // 删除选中配置
ICommand SelectConfigCommand           // 打开弹窗选择配置 → 带入
ICommand PreviewAmlCommand             // 用第一行数据预览 AML 替换效果

// ---- 汇入执行 ----
bool IsImporting                          // 汇入进行中
bool IsPaused                             // 暂停中（暂未实现，预留）
ImportResult? LastResult                  // 上次汇入结果
ICommand ExecuteImportCommand           // 执行汇入

// ---- 状态 ----
string StatusMessage
bool IsLoading
```

### 4.7 View: DataImportView.xaml

布局结构（参考截图风格，上下分区）：

```
┌──────────────────────────────────────────────┐
│  📥 数据汇入                                  │
│  Excel数据导入Aras系统                         │
├──────────────────────────────────────────────┤
│  文件: [__________] 📂浏览   Sheet: [___▼]    │
│  起始行: [2]  结束行: [-1]  起始列: [1] 结束列: [-1]  │
│  [加载预览]                                    │
├──────────────────────────────────────────────┤
│  ┌─ 数据预览（列头 @A @B @C ...）─┐           │
│  │  @A    @B    @C    @D    ...   │           │
│  │  ----  ----  ----  ----       │           │
│  │  ...数据行...                  │           │
│  └────────────────────────────────┘           │
├──────────────────────────────────────────────┤
│  AML模板:                     [选择配置▼]     │
│  ┌──────────────────────────────────┐       │
│  │ <AML>                            │       │
│  │   <Item type="Part">             │       │
│  │     <item_number>@A</item_number>│       │
│  │   </Item>                        │       │
│  │ </AML>                           │       │
│  └──────────────────────────────────┘       │
│  [预览AML] [保存配置] [删除配置]              │
├──────────────────────────────────────────────┤
│  汇入结果: 总行100  成功95  失败5             │
│  日志: Logs/Import/import_20260625_143052.log│
│  [执行汇入]                                    │
└──────────────────────────────────────────────┘
```

**暗色主题**：背景 #1E1E2E, 卡片 #2A2A3C, 文字 #CDD6F4, 输入框 #313244

### 4.8 AML配置选择弹窗: ConfigSelectWindow

```
┌─────────────────────────────┐
│  选择AML配置                 │
├─────────────────────────────┤
│  ┌───────────────────────┐  │
│  │ 配置A  [选择] [删除]  │  │
│  │ 配置B  [选择] [删除]  │  │
│  │ 配置C  [选择] [删除]  │  │
│  └───────────────────────┘  │
│                    [取消]   │
└─────────────────────────────┘
```

点击"选择"→ 关闭弹窗，将选中配置的 aml_content + 范围设置带入主界面。

---

## 五、DI 注册清单

```
Services/ServiceCollectionExtensions.cs:
  services.AddSingleton<IDataImportService, DataImportService>();

App/App.xaml.cs:
  services.AddTransient<DataImportViewModel>();
  services.AddTransient<DataImportView>();
```

## 六、导航注册清单

```
□ MainViewModel.InitializeMenuItems() → "List配置" → "数据汇入", IsPlaceholder=false
□ MainWindow.xaml.cs NavigateToPage → "数据汇入" case
□ DashboardViewModel.AllFeatures → "List配置" → "数据汇入"
```

## 七、DbContext 更新

```
ArasToolkitDbContext.cs:
  public DbSet<DataImportConfig> DataImportConfigs => Set<DataImportConfig>();
  + OnModelCreating 中配置 DataImportConfig 实体映射
  + EnsureSchemaAsync 中添加 data_import_config 表的 CREATE/ALTER 逻辑
```

## 八、编码顺序（Codex 执行顺序）

1. **Entity**: `Core/Entities/DataImportConfig.cs`
2. **DbContext**: 添加 DbSet + OnModelCreating + EnsureSchemaAsync
3. **Interface + Service**: `IDataImportService` + `DataImportService`
4. **Excel 扩展**: `IExcelService`/`ExcelService` 新增范围读取 + 流式读取方法
5. **Model**: `Core/Models/ImportResult.cs`
6. **ViewModel**: `DataImportViewModel.cs`
7. **View**: `DataImportView.xaml` + `.xaml.cs`
8. **ConfigSelectWindow**: `ConfigSelectWindow.xaml` + `.xaml.cs`
9. **菜单重命名**: `MainViewModel.cs` / `DashboardViewModel.cs` / `MainWindow.xaml.cs`
10. **DI 注册**: `ServiceCollectionExtensions.cs` + `App.xaml.cs`

## 九、不实现范围（用户自行完成）

- **Aras API 实际汇入逻辑**：`ExecuteImportAsync` 的 `arasImporter` 参数默认为 null，用户后续传入自己的 Aras 调用逻辑
- `DataImportService.ExecuteImportAsync` 中：若 `arasImporter == null`，则汇入行全部返回"跳过（未配置Aras API）"

## 十、编译验证

完成后执行 `dotnet build ArasToolkit.slnx`，必须 0 错误。
