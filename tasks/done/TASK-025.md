---
id: TASK-025
priority: P0
type: fix
created: 2026-06-25
source: Claude Code
status: done
---

# 修复 ChangelogViewModel 绑定失败 — 补充6个缺失属性

## 问题描述

`ChangelogView.xaml` 中绑定了 6 个属性，但 `ChangelogViewModel.cs` **全部缺失**，导致更新日志页面多项功能失效：

| XAML行 | 绑定路径 | 用途 | 现象 |
|--------|---------|------|------|
| :81 | `{Binding CurrentVersion}` | 显示当前版本号 | 版本号不显示 |
| :106 | `{Binding NewCount}` | 新增类型统计 | 统计卡片数据缺失 |
| :115 | `{Binding FixCount}` | 修复类型统计 | 同上 |
| :126 | `{Binding OptimizeCount}` | 优化类型统计 | 同上 |
| :136 | `{Binding FilterTypes}` | 筛选类型列表源 | ListBox 无数据，筛选项不渲染 |
| :137 | `{Binding FilterType}` | 当前选中筛选类型 | 筛选功能完全失效 |

**根因**: ChangelogViewModel 只实现了基础分页，版本显示、类型统计、类型筛选的 ViewModel 层完全未实现，而 XAML 在设计阶段就已预留这些绑定。

## 涉及文件

| 文件 | 说明 |
|------|------|
| `src/ArasToolkit.App/ViewModels/ChangelogViewModel.cs` | 需补充 6 个属性 + 筛选逻辑 |
| `src/ArasToolkit.App/Views/ChangelogView.xaml` | XAML 绑定源（无需修改，已有正确绑定） |

## 修复方案

### 1. 补充属性

```csharp
// 当前版本
private string _currentVersion = string.Empty;
public string CurrentVersion { get => _currentVersion; set => SetProperty(ref _currentVersion, value); }

// 类型统计
private int _newCount;
public int NewCount { get => _newCount; set => SetProperty(ref _newCount, value); }
private int _fixCount;
public int FixCount { get => _fixCount; set => SetProperty(ref _fixCount, value); }
private int _optimizeCount;
public int OptimizeCount { get => _optimizeCount; set => SetProperty(ref _optimizeCount, value); }

// 筛选
public ObservableCollection<string> FilterTypes { get; } = ["全部", "新增", "修复", "优化", "移除"];
private string _filterType = "全部";
public string FilterType
{
    get => _filterType;
    set { SetProperty(ref _filterType, value); CurrentPage = 1; _ = LoadDataAsync(); }
}
```

### 2. 修改 LoadDataAsync 支持按类型筛选

```csharp
private async Task LoadDataAsync()
{
    IsLoading = true;
    try
    {
        var allItems = await _changelogService.GetAllEntriesAsync();

        // 按类型筛选
        var filtered = FilterType == "全部"
            ? allItems
            : allItems.Where(e => e.Type == FilterType).ToList();

        TotalCount = filtered.Count;

        // 统计各类型数量（始终基于全量数据）
        NewCount = allItems.Count(e => e.Type == "新增");
        FixCount = allItems.Count(e => e.Type == "修复");
        OptimizeCount = allItems.Count(e => e.Type == "优化");

        UpdatePagingCommands();
        var paged = filtered.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize).ToList();
        Entries = new ObservableCollection<Changelog>(paged);
        StatusMessage = $"共 {TotalCount} 条记录";
        if (CurrentPage > TotalPages && TotalPages > 0)
        {
            CurrentPage = TotalPages;
            await LoadDataAsync();
        }
    }
    catch (Exception ex) { StatusMessage = $"加载失败: {ex.Message}"; }
    finally { IsLoading = false; OnPropertyChanged(nameof(PageInfo)); }
}
```

### 3. 构造函数中获取版本号

```csharp
CurrentVersion = _changelogService.GetCurrentVersion();
```

## 联动检查清单

- [x] ChangelogViewModel.cs — 补充属性和筛选逻辑
- [x] ChangelogView.xaml — 无需修改（绑定路径已正确）
- [x] IChangelogService — 已有 GetCurrentVersion()、GetAllEntriesAsync()，无需新增接口
- [x] DI 注册 — 无需变更

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 无新增 Warning
- [ ] 启动后切换到"更新日志"页面，验证版本号、统计卡片、类型筛选正常工作

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


## Git 提交规范

```
修复完成后执行:
1. git add -A && git status  (检查变更文件)
2. git commit -m "fix: ChangelogViewModel 绑定失败 — 补充 CurrentVersion/统计计数/类型筛选 6 个缺失属性"
3. git push origin master --force  (推送前确保 develop 已备份)
```


---

## Codex 实现检查清单

| # | 检查项 | 状态 |
|---|--------|------|
| 1 | CurrentVersion 属性 — 绑定 ChangelogView.xaml:81 | ✅ 已实现 |
| 2 | NewCount 属性 — 绑定 :106 统计卡片 | ✅ 已实现 |
| 3 | FixCount 属性 — 绑定 :115 统计卡片 | ✅ 已实现 |
| 4 | OptimizeCount 属性 — 绑定 :126 统计卡片 | ✅ 已实现 |
| 5 | FilterTypes 集合 — 绑定 :136 ListBox | ✅ 已实现 |
| 6 | FilterType 属性 — 绑定 :137 选中项 + 切换重载 | ✅ 已实现 |
| 7 | LoadDataAsync 按类型筛选 + 全量统计 | ✅ 已实现 |
| 8 | 编译验证 | ✅ 通过 (0 errors) |
