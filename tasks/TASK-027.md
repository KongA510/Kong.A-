---
id: TASK-027
priority: P0
type: feat
created: 2026-06-26
source: Claude Code
status: pending_review
---

# 数据汇入 — DataImportService 内部执行 AML，打通 Aras 全局连接

## 问题描述

`DataImportService.ExecuteImportAsync` 的回调当前是**空壳**：

```csharp
// DataImportViewModel.cs — 回调只更新进度，未调用 Aras API
async (rowNum, aml) =>
{
    ImportProgress = (double)(rowNum - StartRow + 1) / totalRows * 100;
    ProgressText = (rowNum - StartRow + 1) + "/" + totalRows;
    await Task.Delay(1);
    return true;  // ← 假执行
}
```

## 根因分析（架构问题）

当前的 `arasImporter` 回调设计本意是"把 AML 执行权交给调用方"，但这是错误的：

```
之前设计: DataImportService 读Excel → 拼AML → 丢给回调(ViewModel自己做Aras调用)
          ↑ 问题: ViewModel(App层) 拿不到 HttpServerConnection
```

**正确架构**：`DataImportService` 在 Services 层，可以直接注入 `ArasConnectionService`（有 IOM 引用），AML 执行应在 Service 内部完成。回调只负责**进度汇报**，不负责执行：

```
正确设计: DataImportService 读Excel → 拼AML → 自己调 Aras API → 回调仅通知进度
          ↑ Services层有IOM引用，能拿到 HttpServerConnection
```

## 涉及文件

| 文件 | 改动 |
|------|------|
| `src/ArasToolkit.Core/Interfaces/IDataImportService.cs` | 接口 — 回调签名从 `Func<int,string,Task<bool>>` 改为 `Func<int,int,Task>?` |
| `src/ArasToolkit.Services/Services/DataImportService.cs` | 实现 — 注入 `ArasConnectionService`，内部执行 AML |
| `src/ArasToolkit.App/ViewModels/DataImportViewModel.cs` | 回调 — 回归纯进度汇报，移除 `IServiceProvider` |
| `src/ArasToolkit.Services/Services/ArasConnectionService.cs` | 保留 `TypedInnovator`（Services 内部使用） |
| `src/ArasToolkit.App/ArasToolkit.App.csproj` | 移除 IOM 引用（App 层不直接依赖 Aras 库） |

## 修复方案

### 步骤 1：接口修改

```csharp
// IDataImportService.cs — 回调改为纯进度通知
Task<ImportResult> ExecuteImportAsync(
    string filePath, string? sheetName,
    int startRow, int endRow, int startCol, int endCol,
    string amlContent,
    Func<int, int, Task>? progressCallback = null);  // (当前行号, 总行数)
```

### 步骤 2：DataImportService 注入连接并执行 AML

```csharp
// DataImportService.cs 构造函数新增
private readonly ArasConnectionService _connectionService;

// ExecuteImportAsync 核心循环改为:
for (int r = startRow; r <= maxRow; r++)
{
    var rowData = ...; // 读取行数据
    var replacedAml = ReplaceAmlPlaceholders(amlContent, rowData);

    if (progressCallback != null)
        await progressCallback(r, result.TotalRows);

    try
    {
        var innovator = _connectionService.TypedInnovator;
        if (innovator == null)
        {
            result.FailureCount++;
            result.SkippedCount += (maxRow - r);
            break;
        }
        var resultItem = innovator.applyAML(replacedAml);
        if (!resultItem.isError())
            result.SuccessCount++;
        else
            result.FailureCount++;
    }
    catch (Exception ex)
    {
        result.FailureCount++;
        await writer.WriteLineAsync("[失败] 行" + r + ": " + ex.Message);
    }
}
```

### 步骤 3：ViewModel 回调回归简洁

```csharp
// DataImportViewModel.cs — 回调只负责进度
async (rowNum, total) =>
{
    ImportProgress = total > 0 ? (double)rowNum / total * 100 : 0;
    ProgressText = rowNum + "/" + total;
    await Task.Delay(1);
}
// 移除 _serviceProvider、using ArasToolkit.Services.Services
```

### 步骤 4：App.csproj 移除 IOM 引用

撤销上轮的临时改动，App 层不直接依赖 Aras 库。

## 联动检查清单

- [x] IDataImportService — 回调签名变更
- [x] DataImportService — 注入 ArasConnectionService，内部执行 AML
- [x] DataImportViewModel — 回调回归纯进度，移除 IServiceProvider
- [x] ArasConnectionService — TypedInnovator 保留（Services 层内部使用）
- [x] ArasToolkit.App.csproj — 移除 IOM 引用
- [x] DI 注册 — 不修改（ArasConnectionService 已是 Singleton）

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 0 errors
- [ ] 启动后进入数据汇入，加载 Excel + 选择配置 + 执行导入，验证 AML 实际执行

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


## Git 提交规范

```
修复完成后执行:
1. git add -A && git status
2. git commit -m "feat: 数据汇入 Service 层内部执行 AML — DataImportService 注入 ArasConnectionService，回调仅汇报进度"
3. git push origin master --force
```


---

## Codex 实现检查清单

| # | 检查项 | 状态 |
|---|--------|------|
| 1 | IDataImportService 接口 — 回调改为 Func<int,int,Task>? | ✅ 已实现 |
| 2 | ArasConnectionService — 新增 TypedInnovator 属性 | ✅ 已实现 |
| 3 | DataImportService — 注入 ArasConnectionService，内部调用 innovator.applyAML() | ✅ 已实现 |
| 4 | DataImportService — 未连接Aras时跳过剩余行并记录日志 | ✅ 已实现 |
| 5 | DataImportViewModel — 回调回归纯进度汇报，移除 return true 假执行 | ✅ 已实现 |
| 6 | ServiceCollectionExtensions — 注册 ArasConnectionService 具体类型 | ✅ 已实现 |
| 7 | 编译验证 | ✅ 通过 (0 errors) |
