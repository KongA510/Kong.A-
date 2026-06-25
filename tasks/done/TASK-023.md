---
id: TASK-023
priority: P0
type: fix
created: 2026-06-25
source: Claude Code
status: done
---

# 数据汇入 — 配置模板保存失败 & 无法打开（5个关联Bug修复）

## 问题描述

用户在"数据汇入"页面操作时遇到两个症状：
1. **选择配置模板无法打开** — 点击"选择配置"按钮，始终提示"暂无已保存的配置"
2. **AML数据无法保存** — 点击"保存"按钮后保存失败，无法保存到对应表单（`data_import_config` 表）

经过 Claude Code 分析，诊断出 **5个关联的 Bug**（见下方）。

---

## Bug 1（P0-致命）：`SaveConfigAsync` 服务方法缺少 try-catch 和错误日志

**文件**: `src/ArasToolkit.Services/Services/DataImportService.cs` 第 45-74 行

**问题**: `SaveConfigAsync()` 方法完全没有 try-catch 包裹。当数据库连接失败、表不存在、列不匹配等异常发生时：
- 异常直接向上传播，不会记录到错误日志
- 违反 CLAUDE.md 第九节第 9.10 条「Service 层所有 catch 块必须调用 `_errorLogService.LogErrorAsync()`」
- 对比同文件的 `GetConfigsAsync()` 和 `DeleteConfigAsync()` 都有完整的 try-catch + 错误日志

**当前代码**（45-74行）:
```csharp
public async Task<DataImportConfig> SaveConfigAsync(DataImportConfig config)
{
    await using var context = await _contextFactory.CreateDbContextAsync();
    config.UserName = CurrentUserContext.CurrentUserName;
    config.CreatorOn = DateTime.Now;

    if (string.IsNullOrEmpty(config.Id) || config.Id.Length < 12)
    {
        config.Id = Guid.NewGuid().ToString("N")[..12];
        context.Set<DataImportConfig>().Add(config);
    }
    else
    {
        var existing = await context.Set<DataImportConfig>().FindAsync(config.Id);
        if (existing != null)
        {
            existing.ConfigName = config.ConfigName;
            existing.AmlContent = config.AmlContent;
            existing.SheetName = config.SheetName;
            existing.StartRow = config.StartRow;
            existing.EndRow = config.EndRow;
            existing.StartCol = config.StartCol;
            existing.EndCol = config.EndCol;
        }
    }
    await context.SaveChangesAsync();
        await _operationLogService.LogAsync("Create", "DataImportConfig", config.Id,
            "保存数据导入配置: " + config.ConfigName);
    return config;
}
```

**修复要求**:
- 用 try-catch 包裹整个方法体
- catch 中调用 `_errorLogService.LogErrorAsync("数据导入-保存配置", ex.Message, ErrorLog.LevelP1, ex.StackTrace)`
- 保持现有逻辑不变，仅添加错误处理
- 如果 existing 为 null 且 config.Id 不为空（更新不存在的记录），应返回错误而非静默忽略

---

## Bug 2（P0-致命）：ConfigSelectWindow 中删除配置无效

**文件**: 
- `src/ArasToolkit.App/Views/ConfigSelectWindow.xaml.cs` 第 44-53 行（调用端）
- `src/ArasToolkit.App/ViewModels/DataImportViewModel.cs` 第 49 行和第 211-221 行（执行端）

**问题**: ConfigSelectWindow 中点击"删除"按钮时：
1. `DeleteConfig_Click` 调用 `vm.DeleteConfigCommand.Execute(config.Id)` — 传入了要删除的 config.Id
2. 但 ViewModel 的 `DeleteConfigCommand` 定义**不接收参数**: `new RelayCommand(async _ => await DeleteConfigAsync(), _ => SelectedConfig != null)`
3. `DeleteConfigAsync()` 使用 `SelectedConfig.Id` 而非传入的参数
4. 在 ConfigSelectWindow 上下文中，`SelectedConfig` 从未被设置 → 始终为 null
5. 结果为 `if (SelectedConfig == null) return;` — **静默跳过，什么都不做**

**修复方案（二选一）**:
- **方案A**: 修改 ViewModel 的 `DeleteConfigAsync()` 接受命令参数（推荐）
  ```csharp
  DeleteConfigCommand = new RelayCommand(async param => await DeleteConfigAsync(param as string), ...);
  private async Task DeleteConfigAsync(string? configId) { ... }
  ```
- **方案B**: 在 ConfigSelectWindow 中设置 `SelectedConfig` 后再调用删除

**推荐方案A** — 更简洁，不依赖外部状态。

---

## Bug 3（P1-普通）：选择配置后 ConfigSelectWindow 未回写 SelectedConfig

**文件**: `src/ArasToolkit.App\Views\ConfigSelectWindow.xaml.cs` 第 25-42 行

**问题**: `SelectConfig_Click` 将 AML 内容、行列范围等回写到 ViewModel，但**没有设置 `vm.SelectedConfig`**。导致：
- 关闭 ConfigSelectWindow 后，主界面的"删除配置"按钮仍处于禁用状态（因为 `CanDeleteConfig()` 依赖 `SelectedConfig != null`）
- 用户无法从主界面删除刚选择的配置

**修复**:
在 `SelectConfig_Click` 方法的 `vm.SelectedSheetName = config.SheetName;` 之后添加：
```csharp
vm.SelectedConfig = config;
```

---

## Bug 4（P1-普通）：OpenConfigSelectorAsync 窗口数据源不一致

**文件**: `src/ArasToolkit.App\ViewModels\DataImportViewModel.cs` 第 223-241 行

**问题**: `OpenConfigSelectorAsync()` 方法：
1. 调用 `GetConfigsAsync()` 获取最新配置列表（局部变量 `configs`）— 用于判断是否有数据
2. 但打开的 ConfigSelectWindow 绑定到 `SavedConfigs` 属性
3. `SavedConfigs` 只在构造函数（`LoadConfigsAsync`）和手动保存/删除后更新
4. 如果页面加载后其他操作修改了配置（或初次加载未完成），`SavedConfigs` 可能是过期的

**修复**:
在打开窗口前刷新 `SavedConfigs`：
```csharp
var configs = await _dataImportService.GetConfigsAsync();
SavedConfigs = new ObservableCollection<DataImportConfig>(configs);  // 先刷新
if (configs.Count == 0)
{
    ErrorMessage = "暂无已保存的配置";
    return;
}
```

---

## Bug 5（P1-普通）：GetConfigsAsync 错误被完全吞没，用户无法感知

**文件**: `src/ArasToolkit.Services\Services\DataImportService.cs` 第 28-43 行

**问题**: `GetConfigsAsync()` 的 catch 块返回空数组 `[]`，且不向上层传递错误信息。结果是：
- 数据库连接失败 → 返回 `[]` → 显示"暂无已保存的配置"
- 表不存在 → 返回 `[]` → 显示"暂无已保存的配置"
- 用户**无法区分**"真的没有配置"和"数据库出错了"

这种设计导致 Bug 1 的保存失败被完全掩盖 — 用户保存时报错，打开配置时又看到"暂无已保存的配置"，形成困惑闭环。

**修复建议**:
- catch 块中除了记录错误日志外，应考虑通过返回值或 out 参数向上层传递错误状态
- 或者在接口中增加一个 `(List<DataImportConfig> Data, string? Error)` 元组返回值
- 至少应在 ViewModel 的 `LoadConfigsAsync` 中捕获并提示（目前它也是完全静默：`catch { }`）

---

## 涉及文件清单

| 文件 | 需要修改的 Bug |
|---|---|
| `src/ArasToolkit.Services/Services/DataImportService.cs` | Bug 1（加 try-catch+错误日志）、Bug 5（改善错误传递） |
| `src/ArasToolkit.App/ViewModels/DataImportViewModel.cs` | Bug 2（DeleteConfig 接受参数）、Bug 4（刷新 SavedConfigs） |
| `src/ArasToolkit.App/Views/ConfigSelectWindow.xaml.cs` | Bug 3（设置 SelectedConfig）、Bug 2（如有需要） |

---

## 预期行为

修复后：
1. 点击"保存"按钮 → 配置成功写入 `data_import_config` 表，即使失败也有详细错误日志和用户提示
2. 点击"选择配置"按钮 → 正确显示已保存的配置列表，可点击选择加载
3. 在配置选择窗口中点击"删除" → 成功删除对应配置
4. 选择配置后关闭窗口 → 主界面"删除配置"按钮可用
5. 数据库异常时 → 用户能看到有意义的错误提示（而非"暂无已保存的配置"）

---

## Claude Code 分析总结

**根因链**:
```
SaveConfigAsync 无 try-catch (Bug 1)
  → 保存失败时异常传播无日志
  → 配置实际未写入数据库
  → OpenConfigSelectorAsync 查到 0 条记录
  → 显示"暂无已保存的配置"（掩盖真实错误）
  → 用户认为"配置模板无法打开"

DeleteConfig 参数被忽略 (Bug 2)
  → 即使用户能看到配置列表
  → 点击删除也无效（SelectedConfig = null）
  → 用户认为"功能不完整"
```

**额外发现**: `ExecuteImportAsync` 中的 `arasImporter` 回调在 ViewModel 层始终返回 `true`（第 283-292 行），这意味着实际的 Aras API 调用被跳过。如果用户期望"导入到 Aras"，需要在回调中调用真正的 `innovator.applyAML()`。

---

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 无新增 Warning
- [ ] 功能验证：保存配置 → 选择配置 → 加载 → 删除配置 → 验证已删除

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]
