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
// 获取 Sheet 中指定列的列头文本（第一行）
Task<List<string>> GetColumnHeadersAsync(string filePath, string sheetName, int startCol, int endCol);

// 获取 Sheet 实际数据行数
Task<int> GetRowCountAsync(string filePath, string sheetName);

// 按范围读取 Sheet 数据（预览用，返回 DataTable，列头=真实Excel列头）
// endRow=-1 → 读到最后一行, endCol=-1 → 自动检测最后一列
Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
    int startRow, int endRow, int startCol, int endCol);

// 逐行流式读取（汇入用，callback 每行处理）
// 每行数据以 Dictionary<列字母, 值> 传递，如 {"A":"MAT001", "B":"螺栓", "C":"100"}
// 返回: (totalRows, successCount, failureCount)
Task<(int Total, int Success, int Failed)> StreamRowsAsync(
    string filePath, string sheetName,
    int startRow, int endRow, int startCol, int endCol,
    Func<int, Dictionary<string, string>, Task<bool>> rowProcessor);
```

**预览读取逻辑**：
```
1. 读取第1行（表头行）→ 真实列头，用作 DataTable 的 ColumnName
   例如: "物料编号"、"物料名称"、"数量"
2. 构建列映射表: {"A": "物料编号", "B": "物料名称", "C": "数量", ...}
3. 从第2行开始读取数据行，填充到 DataTable
4. DataGrid 显示: 真实列头 + 数据行
```

**逐行汇入逻辑**：
```
1. 从 startRow 开始逐行读取
2. 每行数据: {"A": "MAT001", "B": "螺栓", "C": "100"}
3. 传入 rowProcessor → 内部替换AML: @A→MAT001, @B→螺栓, @C→100
4. rowProcessor 返回 true=成功, false=失败
```

### 4.4 新建 Service: DataImportService

```csharp
public interface IDataImportService
{
    // ---- AML 配置 CRUD ----
    Task<List<DataImportConfig>> GetConfigsAsync();           // 用户隔离
    Task<DataImportConfig> SaveConfigAsync(DataImportConfig config);
    Task DeleteConfigAsync(string id);

    // ---- Excel 读取（委托给 IExcelService） ----
    Task<List<string>> GetSheetNamesAsync(string filePath);
    Task<ExcelSheetData> ReadSheetRangeAsync(string filePath, string sheetName,
        int startRow, int endRow, int startCol, int endCol);
    Task<List<ColumnMapping>> GetColumnMappingsAsync(string filePath, string sheetName,
        int startCol, int endCol);

    // ---- AML 替换 ----
    // 将 AML 中的 @A @B @C... 替换为 rowData 中对应列的值
    // rowData key 是列字母（"A","B","C"...），value 是该行对应列的值
    string ReplaceAmlPlaceholders(string amlTemplate, Dictionary<string, string> rowData);

    // 用第一行数据预览 AML 替换效果（返回替换后的完整 AML 文本）
    string PreviewAml(string amlTemplate, Dictionary<string, string> firstRowData);

    // ---- 汇入执行 ----
    // 逐行读取 Excel → 替换AML中的@A@B → 调用 arasImporter
    // arasImporter 为 null 时，所有行标记为"跳过"
    Task<ImportResult> ExecuteImportAsync(
        string filePath, DataImportConfig config,
        Func<int, string, Task<bool>>? arasImporter = null);
        // arasImporter 参数: (行号, 替换后的AML) → 返回是否成功
}

// 列映射模型
public class ColumnMapping
{
    public string Letter { get; set; }  // "A", "B", "C", ..., "AA", "AB"...
    public string Header { get; set; }  // 真实列头名，如 "物料编号"
    public int Index { get; set; }      // 列索引（0=第1列即A列）
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
DataTable? PreviewData                    // 预览数据（显示真实列头）
ObservableCollection<string> ColumnHeaders // 真实列头名称

// ---- 列映射表（@A ↔ 真实列头） ----
ObservableCollection<ColumnMapping> ColumnMappings // @A=物料编号, @B=物料名称 ...
// ColumnMapping 包含: Letter("A"), Header("物料编号"), Index(0)

// ---- AML 配置管理 ----
ObservableCollection<DataImportConfig> SavedConfigs  // 已保存配置列表
DataImportConfig? SelectedConfig          // 当前选中配置
string AmlContent                         // 当前 AML 模板内容（双向绑定）
ICommand SaveConfigCommand             // 保存当前 AML + 范围设置
ICommand DeleteConfigCommand           // 删除选中配置
ICommand OpenConfigSelectorCommand    // 打开弹窗选择配置 → 带入
ICommand PreviewAmlCommand             // 用第一行数据替换AML中的@A@B → 展示替换后AML

// ---- 汇入执行 ----
bool IsImporting                          // 汇入进行中
bool IsPaused                             // 暂停中
ICommand ExecuteImportCommand           // 执行汇入
ICommand PauseCommand                   // 暂停/继续汇入（Toggle）
CancellationTokenSource? _cts             // 取消令牌，暂停时取消当前行

// ---- 结果 ----
ImportResult? LastResult                  // 上次汇入结果

// ---- 状态 ----
string StatusMessage
bool IsLoading
```

**汇入流程（逐行替换 @A@B → 执行）：**

```
1. 用户加载文件 → 选择 Sheet → 设置范围 → 加载预览
2. 界面显示: 列映射表（@A=物料编号） + 数据表格（真实列头）
3. 用户编写/选择 AML 模板（含 @A @B 占位符）
4. 点击"预览AML" → 取第一行数据替换 @A,@B → 展示替换后的完整 AML
5. 点击"执行汇入" → 从 StartRow 开始逐行:
   a. 检查 IsPaused，若暂停则 await 等待取消令牌
   b. 读取当前行 A列值 → 替换 AML 中 @A
   c. 读取 B列值 → 替换 @B ...（替换全部占位符）
   d. 调用 arasImporter(rowNumber, replacedAml) 
   e. 成功 → successCount++; 失败 → 写入日志文件
6. 汇入完成 → 显示结果汇总
```

**暂停机制**：
- `PauseCommand` → `IsPaused = true` → `_cts.Cancel()`
- 当前正在执行的行完成后阻塞等待
- 再次点击 → `IsPaused = false` → 重新创建 `_cts` → 继续下一行

### 4.7 View: DataImportView.xaml

布局结构（上下分区）：

```
┌──────────────────────────────────────────────┐
│  📥 数据汇入                                  │
│  Excel数据导入Aras系统                         │
├──────────────────────────────────────────────┤
│  文件: [__________] 📂浏览   Sheet: [___▼]    │
│  起始行: [2]  结束行: [-1]  起始列: [1] 结束列: [-1]  │
│  [加载预览]                                    │
├──────────────────────────────────────────────┤
│  ┌─ 列映射表（@A @B ... ↔ 真实列头）──┐      │
│  │  @A = 物料编号    @D = 规格        │      │
│  │  @B = 物料名称    @E = 单位        │      │
│  │  @C = 数量        @F = 备注        │      │
│  └────────────────────────────────────┘      │
├──────────────────────────────────────────────┤
│  ┌─ 数据预览（显示真实列头）─────────┐        │
│  │  物料编号 │ 物料名称 │ 数量 │ ... │        │
│  │  ────────┼──────────┼──────┼──── │        │
│  │  MAT001  │ 螺栓     │ 100  │ ... │        │
│  │  MAT002  │ 螺母     │ 200  │ ... │        │
│  └──────────────────────────────────┘        │
├──────────────────────────────────────────────┤
│  AML模板:                     [选择配置▼]     │
│  ┌──────────────────────────────────┐       │
│  │ <AML>                            │       │
│  │   <Item type="Part">             │       │
│  │     <item_number>@A</item_number>│  ← @A将被替换为A列当前行的值
│  │     <name>@B</name>              │  ← @B将被替换为B列当前行的值
│  │   </Item>                        │       │
│  │ </AML>                           │       │
│  └──────────────────────────────────┘       │
│  [预览AML（用第一行数据替换@A@B）]              │
│  [保存配置] [删除配置]                        │
├──────────────────────────────────────────────┤
│  汇入结果: 总行100  成功95  失败5             │
│  日志: Logs/Import/import_20260625_143052.log│
│  [执行汇入]  [⏸ 暂停]                        │
└──────────────────────────────────────────────┘
```

**@A @B 占位符机制说明：**

```
AML模板中的 @A → 汇入时替换为当前行第1列（A列）的值
AML模板中的 @B → 汇入时替换为当前行第2列（B列）的值
...
以此类推，@Z → 第26列，@AA → 第27列 ...

示例：
  模板: <item_number>@A</item_number><name>@B</name>
  第3行数据: A列=MAT001, B列=螺栓
  替换后AML: <item_number>MAT001</item_number><name>螺栓</name>
  → 将替换后的AML发送到Aras执行汇入
```

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
