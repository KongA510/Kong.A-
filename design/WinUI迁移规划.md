# ArasToolkit WPF → WinUI 3 迁移规划

> 版本：v1.0  编制日期：2026-07-23
> 目标：在**功能完全不变**的前提下，将 UI 层从 WPF 迁移到 WinUI 3（Windows App SDK），获得更现代、更符合用户审美习惯的 Fluent Design 界面。

---

## 一、背景与目标

### 1.1 为什么迁移
- WinUI 3 是微软当前主推的 Windows 原生 UI 框架，原生 Fluent Design（云母材质、圆角、动效、明暗主题），观感比 WPF 自绘样式更统一、更现代。
- 现有项目虽然手写了一套 Fluent 风格样式，但本质上是在"模仿" WinUI 原生就提供的能力；迁移后可直接复用系统级控件与主题资源，长期维护成本更低。

### 1.2 核心约束
- **功能零变更**：所有现有功能（登录、翻译、配置、导入导出、日志、图表、任务、资料库等）行为保持一致。
- **Core / Services 层尽量不动**：业务逻辑、EF Core、EPPlus、Aras IOM 全部保留。
- **可回滚**：采用"并行新项目"策略，旧 WPF 项目在新项目验收前完整保留。

---

## 二、可行性结论（已实测）

| 项目 | 结论 |
|------|------|
| 本环境能否编译 WinUI 3 | 可以 |
| 必备 SDK | 已装 .NET 8.0.422（用 global.json 固定，避免误用 .NET 10 preview） |
| 关键报错 | NETSDK1083（不识别 win10-* RID）→ 在 csproj 加 RuntimeIdentifiers=win-x86;win-x64;win-arm64 解决 |
| 打包方式 | 采用非打包（Unpackaged）：WindowsPackageType=None，与现有绿色运行方式一致 |
| Windows 10 SDK | 未单独安装，但 WindowsAppSDK NuGet 自带 WinRT 投影，实测可完成 XAML 编译 |

> 实测探针已成功生成 WinUIProbe.dll、apphost.exe、.xbf，证明 XAML 编译器在本环境可正常工作。

---

## 三、现状盘点（基于全量代码扫描）

### 3.1 三层架构与迁移影响

| 层 | 项目 | TFM | UI 耦合 | 迁移影响 |
|----|------|-----|---------|----------|
| 核心 | ArasToolkit.Core | net8.0 | 无 | 基本不动（ObservableObject/RelayCommand 为纯 INPC/ICommand，WinUI 通用） |
| 服务 | ArasToolkit.Services | net8.0 | 无 | 完全不动（EF Core / EPPlus / IOM / DI 全部保留） |
| 应用 | ArasToolkit.App | net8.0-windows (WPF) | 全部 UI | 整体重写 |

### 3.2 重大利好
- **MaterialDesign 未使用**：MaterialDesignThemes / MaterialDesignColors 虽在 csproj 中引用，但全量 XAML 扫描 xmlns:materialDesign 与 materialDesign: 零匹配，属死引用，直接删除即可，不构成迁移障碍。
- **样式已是 Fluent 风格**：调色板键名（SystemAccentColor、ControlFillColorDefault、TextFillColorPrimary、ControlStrokeColorDefault 等）与 WinUI 3 内置 ThemeResource 高度同名，大量自绘控件模板可被 WinUI 原生控件替代。

### 3.3 需重点改造的 WPF 专属特性

| 特性 | 涉及范围 | WinUI 3 方案 |
|------|----------|--------------|
| DataGrid | 13 个视图 | CommunityToolkit.WinUI.Controls.DataGrid（API 与 WPF 几乎一致） |
| 样式触发器 Trigger/DataTrigger | 23 个文件 | 优先用 WinUI 原生控件内置视觉状态；必要的改 VisualStateManager 或 x:Bind+转换器 |
| 多窗口 Window + ShowDialog | 5 个窗口类、12 处调用 | ContentDialog（模态）/ AppWindow+WindowEx（独立窗口） |
| Win32 文件对话框 | 11 个 ViewModel | 抽象 IFileDialogService，实现用 FileOpenPicker/FileSavePicker（需 HWND） |
| MessageBox | 5 个文件 | 收敛进 IDialogService，用 ContentDialog 实现 |
| MultiBinding | 1 处（ArasLoginWindow） | 改 x:Bind 静态函数 / 在 VM 预计算属性 |
| OxyPlot 图表 | 1 个视图（ChartView）+ ChartViewModel | 换 LiveChartsCore.SkiaSharpView.WinUI（免费 MIT、原生 WinUI、MVVM 友好） |
| TreeView 导航 | MainWindow | NavigationView + Frame |
| 自定义标题栏（DragMove/WindowState/WindowStyle=None） | MainWindow | ExtendsContentIntoTitleBar + AppWindow.SetPresenter，配合 WinUIEx 简化 |
| pack:// URI / x:Static SystemColors | App.xaml | ms-appx:///；系统色改用 WinUI ThemeResource |
| DropShadowEffect（卡片阴影） | Cards.xaml | ThemeShadow + Translation |
| 值转换器 | Converters.cs（5 个） | 命名空间 System.Windows.Data → Microsoft.UI.Xaml.Data；IMultiValueConverter 无对应，需重构 |

### 3.4 视图清单（按 XAML 行数 / 复杂度）

| 视图 | 行数 | 关键控件 | 迁移批次 |
|------|------|----------|----------|
| KnowledgeBaseView | 566 | 复杂布局+对话框 | 第3批 |
| TodoView | 461 | DataGrid+编辑表单 | 第2批 |
| Controls.xaml（样式） | 344 | 控件模板 | 阶段2 |
| TextTranslationView | 299 | DataGrid+进度 | 第3批 |
| ListConfigView | 269 | DataGrid | 第2批 |
| LifecycleConfigView | 266 | DataGrid | 第2批 |
| MainWindow | 262 | TreeView导航+标题栏 | 阶段2 |
| ObjectClassConfigView | 253 | DataGrid | 第2批 |
| FileExplorerView | 243 | 树+列表 | 第3批 |
| PropertyConfigView | 238 | DataGrid | 第2批 |
| PermissionConfigView | 238 | DataGrid | 第2批 |
| ArasLoginWindow | 236 | 窗口+MultiBinding | 阶段3 |
| ChangelogView | 234 | 列表 | 第1批 |
| FieldTranslationView | 221 | DataGrid | 第2批 |
| PropertyTranslationView | 221 | DataGrid | 第2批 |
| TranslationApiKeyView | 221 | 表单 | 第3批 |
| DataImportView | 198 | DataGrid+进度 | 第2批 |
| DatabaseExportConfigView | 195 | 表单 | 第3批 |
| DatabaseExportView | 188 | DataGrid | 第2批 |
| LoginView | 172 | 表单 | 阶段3 |
| SettingsView | 148 | 多模式表单 | 第3批 |
| OperationLogView | 144 | DataGrid | 第1批 |
| DashboardView | 137 | 卡片网格 | 第1批 |
| ErrorLogView | 120 | DataGrid | 第1批 |
| ChartView | 105 | OxyPlot | 第4批 |
| AppLoginView | 99 | 表单 | 阶段3 |
| TranslationHistoryView | 97 | DataGrid | 第2批 |
| ConfigSelectWindow | 79 | 窗口 | 阶段5 |
| PlaceholderView | 51 | 卡片网格 | 第1批 |
| TextPromptWindow | 19 | 窗口 | 阶段5 |

---

## 四、技术选型

### 4.1 NuGet 依赖（新项目）

| 包 | 用途 | 替代/说明 |
|----|------|-----------|
| Microsoft.WindowsAppSDK | WinUI 3 运行时 | 核心 |
| Microsoft.Windows.SDK.BuildTools | 构建工具 | 核心 |
| CommunityToolkit.WinUI.Controls.DataGrid | DataGrid | 替代 WPF 内置 DataGrid |
| CommunityToolkit.WinUI.Controls.*（按需） | Expander/Segmented 等 | 可选 |
| CommunityToolkit.Mvvm | MVVM 基类 | 已在用，保留 |
| CommunityToolkit.WinUI.Converters | 常用转换器 | 可选，减少自写 |
| LiveChartsCore.SkiaSharpView.WinUI | 图表 | 替代 OxyPlot.Wpf |
| WinUIEx | 窗口增强（标题栏/尺寸/置顶） | 简化自定义标题栏迁移 |
| Microsoft.Extensions.DependencyInjection | DI | 已在用，保留 |

### 4.2 移除的依赖
- MaterialDesignThemes / MaterialDesignColors（未使用）
- OxyPlot.Wpf（被 LiveCharts2 替代）
- UseWPF（改为 UseWinUI）

### 4.3 图表库选型对比

| 方案 | WinUI 3 支持 | 许可 | MVVM | 结论 |
|------|--------------|------|------|------|
| LiveCharts2 | 原生 | MIT 免费 | 强 | 推荐（当前仅柱状图，轻松覆盖） |
| OxyPlot.Windows | 仅 UWP，WinUI 实验性/停更 | MIT | 中 | 不推荐 |
| Win2D | 支持 | MIT | 需自绘 | 过重 |
| Syncfusion/FlexChart | 支持 | 商业 | 强 | 引入授权成本 |

---

## 五、架构策略

### 5.1 并行新项目（推荐）
- 新建 src/ArasToolkit.App.WinUI/，与现有 ArasToolkit.App 并存。
- 两个项目同引用 Core + Services，业务逻辑零分叉。
- 新项目逐功能迁移并验证，全部通过后再移除旧 WPF 项目。
- 优点：随时可回退、可对照行为、风险隔离。

### 5.2 导航重构
- 现状：MainWindow.NavigateToPage(name) 用 switch 字符串 → new XxxView{DataContext=...}，塞进 ContentControl。
- 目标：NavigationView（侧边栏）+ Frame.Navigate(typeof(XxxPage))，配合 DI 页面工厂（ActivatorUtilities.CreateInstance）在导航时注入 ViewModel。
- 菜单数据源沿用 MainViewModel.MenuItems（MenuItemInfo 模型不变），CardIcon emoji 继续用于仪表盘卡片。

### 5.3 对话框/文件服务抽象（关键解耦）
当前 11 个 ViewModel 直接 new OpenFileDialog() / MessageBox.Show()，强耦合 WPF。迁移时抽象为接口（放 Core/Interfaces）：

```csharp
public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message);
    Task<string?> PromptAsync(string title, string placeholder);   // 替代 TextPromptWindow
    Task AlertAsync(string title, string message);                 // 替代 MessageBox
}

public interface IFileDialogService
{
    Task<string?> PickOpenFileAsync(params string[] extensions);
    Task<string?> PickSaveFileAsync(string defaultName, params string[] extensions);
    Task<string?> PickFolderAsync();
}
```
- 实现放 App.WinUI，用 ContentDialog + FileOpenPicker/FileSavePicker（通过 InitializeWithWindow 绑定 HWND）。
- 这是一次正向重构：ViewModel 不再依赖任何 UI 框架，未来再换 UI 也无需改动。

### 5.4 ViewModel 层影响
- ObservableObject / RelayCommand（Core 自实现）基于 INPC + ICommand，WinUI 完全通用，不改。
- {Binding} 在 WinUI 仍受支持，VM 属性/命令绑定基本不改；新页面可选择性升级 x:Bind 提升性能。
- 仅需替换 VM 内的 MessageBox / 文件对话框调用为 IDialogService / IFileDialogService。

---

## 六、WPF → WinUI 3 映射速查

| WPF | WinUI 3 |
|-----|---------|
| Window | Microsoft.UI.Xaml.Window（+WinUIEx.WindowEx） |
| UserControl（导航页） | Page |
| ContentControl.Content = view | Frame.Navigate(typeof(Page)) |
| TreeView（导航） | NavigationView |
| DataGrid | controls:DataGrid（CommunityToolkit） |
| {Binding} | {Binding} 保留 / {x:Bind} 推荐 |
| {DynamicResource X} | {ThemeResource X} / {StaticResource X} |
| Trigger/DataTrigger | VisualStateManager / 原生控件状态 |
| BooleanToVisibilityConverter | x:Bind bool→Visibility 函数 / CommunityToolkit 转换器 |
| MultiBinding | x:Bind 静态函数 / VM 预计算 |
| MessageBox.Show | ContentDialog |
| OpenFileDialog/SaveFileDialog | FileOpenPicker/FileSavePicker（需 HWND） |
| Window.DragMove() | ExtendsContentIntoTitleBar + SetDragRectangles |
| WindowState=Maximized | AppWindow.SetPresenter(OverlappedPresenter.CreateMaximized()) |
| WindowStyle=None/AllowsTransparency | ExtendsContentIntoTitleBar=true |
| pack://application:,,,/X.xaml | ms-appx:///X.xaml |
| x:Static SystemColors.HighlightBrushKey | WinUI ThemeResource |
| DropShadowEffect | ThemeShadow + Translation |
| IValueConverter(System.Windows.Data) | IValueConverter(Microsoft.UI.Xaml.Data) |

---

## 七、分阶段实施计划

### 阶段 0：准备与技术验证（已完成）
- [x] 环境可行性实测（global.json + RuntimeIdentifiers）
- [x] 全量代码盘点
- [x] 技术选型
- [ ] 编码前 git 备份分支（正式开工时创建）

### 阶段 1：项目脚手架
- [ ] 新建 src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj
  - net8.0-windows10.0.19041.0、UseWinUI、WindowsPackageType=None、RuntimeIdentifiers
  - 引用 Core + Services
  - 加入 4.1 节 NuGet 包
  - 移植 CopyR37Libs / CopyConfigFiles 两个 MSBuild Target
  - 移植 AppDomain.AssemblyResolve（R37lib 探测）
- [ ] 根目录 global.json 固定 SDK 8.0.422
- [ ] 更新 ArasToolkit.slnx 纳入新项目
- [ ] 里程碑：空壳 WinUI 应用能编译并启动显示 Hello 窗口

### 阶段 2：基础设施层
- [ ] App.xaml/cs：DI 容器（AddArasToolkitServices + 注册 VM/页面）、全局资源
- [ ] 主题移植：
  - Colors.Light.xaml → 复用 WinUI 内置 ThemeResource，仅覆盖强调色等少量键
  - Controls.xaml → 删除自绘模板，改用 WinUI 原生控件 + 资源覆盖（工作量大幅缩减）
  - Cards.xaml → ThemeShadow
- [ ] Converters.cs 重写（命名空间迁移 + MultiStringEquality 重构）
- [ ] IDialogService / IFileDialogService（Core 接口 + App 实现）
- [ ] MainWindow：NavigationView + Frame + 自定义标题栏（WinUIEx）+ DI 页面工厂
- [ ] 里程碑：导航骨架可点击切换空白占位页

### 阶段 3：导航与登录
- [ ] 导航映射服务（替代 NavigateToPage switch，按 MenuItemInfo.Name → Page 类型）
- [ ] AppLoginView / LoginView
- [ ] ArasLoginWindow（含 MultiBinding → x:Bind 改造）
- [ ] 里程碑：完整登录流程跑通（应用登录 + Aras 连接）

### 阶段 4：视图分批迁移
- [ ] 第1批（简单展示）：Dashboard、Placeholder、Changelog、ErrorLog、OperationLog
- [ ] 第2批（DataGrid 列表）：Todo、ObjectClass/List/Property/Permission/Lifecycle Config、DataImport、DatabaseExport、FieldTranslation、PropertyTranslation、TranslationHistory
- [ ] 第3批（复杂交互）：TextTranslation、FileExplorer、KnowledgeBase、Settings、TranslationApiKey、DatabaseExportConfig
- [ ] 第4批（图表）：ChartView（LiveCharts2 重写柱状图，保持数据/交互一致）
- [ ] 里程碑：每批迁移后逐功能手工验证通过

### 阶段 5：多窗口与对话框收敛
- [ ] ConfigSelectWindow / TextPromptWindow / TranslationApiKeyWindow → ContentDialog 或 AppWindow
- [ ] 全部 MessageBox → IDialogService
- [ ] 全部文件对话框 → IFileDialogService
- [ ] 里程碑：所有模态交互正常

### 阶段 6：收尾
- [ ] 全量编译 0 错误
- [ ] 逐功能回归验证（对照旧 WPF 行为）
- [ ] 移除旧 ArasToolkit.App（或保留为参照，二选一）
- [ ] 更新 AGENTS.md（项目结构、命令、WinUI 规范）
- [ ] 写更新日志（IChangelogService，按版本号规则递增）
- [ ] git 提交（按 Git 分支策略：先备份 master→develop，再推 master）

---

## 八、风险与对策

| 风险 | 等级 | 对策 |
|------|------|------|
| WinUI 3 控件行为与 WPF 差异导致功能偏差 | 高 | 并行项目 + 逐功能对照验证；保留旧项目直至验收 |
| DataGrid（CommunityToolkit）与 WPF DataGrid 细节差异（模板列、编辑、选择） | 中 | 第2批集中攻关，先做一个标杆视图（如 TodoView）固化模式再批量复制 |
| 文件对话框需 HWND，VM 调用链改造面广（11 个 VM） | 中 | 统一走 IFileDialogService，一次性抽象，逐 VM 替换 |
| 图表库更换（OxyPlot→LiveCharts2）可能视觉/交互不一致 | 中 | 仅 1 个视图，单独验证；保持数据源与刷新逻辑不变，仅替换渲染 |
| 自定义标题栏/窗口状态在 WinUI 行为不同 | 中 | 用 WinUIEx 封装，集中在一处实现 |
| 触发器→VisualStateManager 改造繁琐（23 文件） | 中低 | 多数可被 WinUI 原生控件状态替代，真正需手改的少 |
| R37lib（IOM）在非打包 WinUI 下的程序集探测 | 低 | 沿用现有 AssemblyResolve + 复制 Target，已在 WPF 验证可行 |
| 本环境无法目视验证 UI | 中 | 以编译通过+逻辑对照为准；建议用户在本地运行做最终视觉确认 |

---

## 九、验收标准

1. 功能等价：登录、翻译（文本/字段/表单/窗体）、配置（对象类/List/属性/权限/生命周期）、数据汇入、数据库导出、日志（更新/错误/操作）、图表、任务、资料库、设置——逐项行为与旧版一致。
2. 编译干净：dotnet build 0 错误（沿用 MSB3277 抑制）。
3. 架构合规：Core/Services 无 UI 耦合；VM 不再直接引用 System.Windows/Win32 对话框。
4. 规范同步：AGENTS.md、更新日志、Git 提交均按现有规范执行。
5. 视觉：Fluent Design 风格统一，明暗主题资源就位（默认浅色，与现状一致）。

---

## 十、工作量估算（参考）

| 阶段 | 估算 | 说明 |
|------|------|------|
| 阶段1 脚手架 | 0.5 天 | 模板+依赖+空壳启动 |
| 阶段2 基础设施 | 1.5–2 天 | 主题/导航/对话框服务（一次性投入，复用度高） |
| 阶段3 登录导航 | 1 天 | 含 MultiBinding 改造 |
| 阶段4 视图迁移 | 4–6 天 | 30 个视图，DataGrid 批量化后提速 |
| 阶段5 窗口对话框 | 1 天 | 5 窗口 + 收敛 |
| 阶段6 收尾验证 | 1–2 天 | 回归+文档 |
| 合计 | 约 9–13 个工作日 | 视 DataGrid 复杂度与验证严格度浮动 |

> 说明：因本环境无法目视验证 UI，实际视觉微调需用户在本地运行后反馈迭代。

---

## 十一、开工前检查清单

```
□ git 备份分支（backup-before-winui-*）
□ global.json 固定 SDK 8.0.422
□ 新项目 csproj 含 RuntimeIdentifiers 修复
□ Core/Services 以 ProjectReference 复用（不复制代码）
□ 移除 MaterialDesign / OxyPlot.Wpf
□ 加入 WindowsAppSDK / DataGrid / LiveCharts2 / WinUIEx
□ 移植 R37lib 复制 + AssemblyResolve
□ 移植 DBSeeting.json 复制
```

---

> 本规划基于 2026-07-23 对代码库的全量扫描与 WinUI 3 可行性实测编制。正式开工时按阶段推进，每阶段设里程碑验证，确保功能不变、可回滚、逐步交付。
