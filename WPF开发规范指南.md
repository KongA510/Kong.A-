# ArasToolkit WPF 开发规范指南

> 适用读者：熟悉 ASP.NET Core Web API 开发的工程师  
> 目标：快速上手 WPF MVVM 开发，理解与 Web API / WinForms 的本质区别

---

## 目录

1. [三层架构对照](#1-三层架构对照)
2. [WPF vs Web API vs WinForms 核心区别](#2-wpf-vs-web-api-vs-windows-form)
3. [MVVM 模式详解](#3-mvvm-模式详解)
4. [数据绑定（最核心概念）](#4-数据绑定最核心概念)
5. [命令系统（≈ 路由绑定）](#5-命令系统-路由绑定)
6. [依赖注入配置](#6-依赖注入配置)
7. [样式与控件模板](#7-样式与控件模板)
8. [新增功能 7 步流程](#8-新增功能-7-步流程)
9. [常用代码片段](#9-常用代码片段)
10. [常见坑与解决方案](#10-常见坑与解决方案)

---

## 1. 三层架构对照

```
Web API 项目结构:              WPF 本项目结构:

Controllers/                  ViewModels/        ← UI逻辑（≈Controller Action）
Services/                     Services/          ← 业务逻辑（完全一致）
Models/DTOs/                  Core/Models/       ← 数据模型（完全一致）
appsettings.json              Config/*.json      ← 配置文件
Startup.cs                    App.xaml.cs        ← 启动+DI
Middleware                    Services/Services  ← 横切关注点
```

### 引用关系

```
Web API:   Web → Services → Domain
WPF:       App → Services → Core  （完全相同的分层依赖）
```

---

## 2. WPF vs Web API vs Windows Form

### 三者本质

| 维度 | Web API | WPF | WinForms |
|:---|:---|:---|:---|
| **运行环境** | 服务端，响应HTTP请求 | 客户端桌面应用 | 客户端桌面应用 |
| **UI技术** | 无UI，返回JSON/XML | XAML标记语言 | 拖拽控件 + 代码 |
| **通信方式** | HTTP 请求-响应 | 数据绑定（Data Binding） | 事件处理（Event Handler） |
| **架构模式** | MVC / 三层 | **MVVM** | 事件驱动（无强制模式） |
| **数据流** | 请求→返回→结束 | 属性变化→自动刷新UI | 手动 `textBox1.Text = ...` |
| **线程模型** | 每次请求独立线程 | **UI线程** + 后台Task | 单线程（UI线程阻塞） |
| **状态管理** | 无状态（Stateless） | **有状态**（VM常驻内存） | 有状态（Form常驻内存） |
| **可测试性** | 高（Controller可独立测试） | 高（ViewModel可独立测试） | 低（UI逻辑耦合） |
| **学习曲线** | 中 | **陡**（XAML+绑定+命令） | 低 |

### 最大思维转变

```
Web API 开发思维:
    "收到请求 → 处理 → 返回数据 → 结束"（无状态，一步一清）

WPF MVVM 开发思维:
    "启动 → ViewModel常驻内存 → 属性变化自动驱动UI → 等待用户操作"
    （有状态，ViewModel就是内存中一直活着的"页面状态"）
```

```csharp
// ===== Web API 做法 =====
[HttpGet]
public IActionResult GetUsers()
{
    var users = _userService.GetAll();    // 查数据
    return Ok(users);                      // 返回，结束
}

// ===== WPF 做法 =====
public class UserViewModel
{
    // ViewModel 一直存在于内存中
    private ObservableCollection<User> _users = new();
    public ObservableCollection<User> Users
    {
        get => _users;
        set { _users = value; OnPropertyChanged(); }  // 赋值 → UI自动刷新
    }
    
    public async Task LoadUsersAsync()  // 不是return，是赋值给属性
    {
        Users = new ObservableCollection<User>(await _userService.GetAllAsync());
    }
}
```

### WinForms 转 WPF 核心变化

| WinForms 习惯 | WPF 正确做法 |
|:---|:---|
| `textBox1.Text = "hello"` | 绑定：`Text="{Binding Name}"`，代码只改 `Name` 属性 |
| `button1_Click(object, EventArgs)` | `Command="{Binding SaveCommand}"` |
| 拖控件到设计器 | 手写 XAML（声明式UI） |
| `Form1_Load` 初始化 | ViewModel 构造函数中初始化 |
| `this.Invoke(() => ...)` | 自动（Binding自动封送UI线程） |
| `using` 管理控件生命周期 | XAML 自动管理视觉树 |

---

## 3. MVVM 模式详解

```
┌──────────────────────────────────────────────────┐
│                      MVVM                         │
│                                                   │
│  ┌──────────┐    DataBinding    ┌──────────────┐ │
│  │   View   │◄────────────────►│   ViewModel  │ │
│  │  .xaml   │    Command        │   .cs 类     │ │
│  │ (纯UI)   │◄─────────────────│  (UI逻辑)    │ │
│  └──────────┘                   └──────┬───────┘ │
│                                        │         │
│                                        │ 调用    │
│                                        ▼         │
│                                 ┌──────────────┐ │
│                                 │   Service    │ │
│                                 │  (业务逻辑)  │ │
│                                 └──────┬───────┘ │
│                                        │         │
│                                        ▼         │
│                                 ┌──────────────┐ │
│                                 │    Model     │ │
│                                 │  (数据结构)  │ │
│                                 └──────────────┘ │
└──────────────────────────────────────────────────┘

View 绝不直接调用 Service，必须通过 ViewModel
ViewModel 绝不操作 UI 控件，只操作属性
Service 绝不引用 WPF 命名空间（System.Windows）
```

### 本项目的 MVVM 实现

| 层 | 本项目基类/接口 | 职责 |
|:---|:---|:---|
| **M**odel | `Core/Models/*.cs` | 纯数据类，只有属性没有行为 |
| **V**iew | `Views/*.xaml` + `.xaml.cs` | 纯XAML布局，code-behind尽可能为空 |
| **V**iew **M**odel | `ViewModels/*.cs`，继承 `ObservableObject` | 数据+命令+状态，驱动UI |

---

## 4. 数据绑定（最核心概念）

> 数据绑定是 WPF 的灵魂。理解它，就理解了 WPF。

### Binding 语法速查

```xml
<!-- 基础绑定 -->
<TextBlock Text="{Binding UserName}" />

<!-- 双向绑定（输入框必须用） -->
<TextBox Text="{Binding UserName, UpdateSourceTrigger=PropertyChanged}" />

<!-- 绑定到命令 -->
<Button Command="{Binding SaveCommand}" />

<!-- 绑定到集合 -->
<DataGrid ItemsSource="{Binding Users}" />

<!-- 元素间绑定 -->
<TextBlock Text="{Binding Text, ElementName=InputBox}" />
```

### ViewModel 属性写法（标准模板）

```csharp
public class MyViewModel : ObservableObject
{
    // 完整属性（支持通知）
    private string _userName = string.Empty;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);  // 赋值 + 自动通知UI
    }

    // 只读属性（派生计算）
    public bool HasUserName => !string.IsNullOrEmpty(UserName);
    // 当 UserName 变化需要通知 HasUserName 也变化时：
    // private string _userName...
    // set { SetProperty(ref _userName, value); OnPropertyChanged(nameof(HasUserName)); }
}
```

### 集合绑定（必须用 ObservableCollection）

```csharp
// ❌ 错误：UI 不会更新
public List<User> Users { get; set; } = new();

// ✅ 正确：UI 自动同步增删改
public ObservableCollection<User> Users { get; set; } = new();
```

### 控件到 ViewModel 的绑定模式

| 控件 | 绑定模式 | 必须项 |
|:---|:---|:---|
| `TextBlock` / `Label` | 单向（默认） | `Text="{Binding ...}"` |
| `TextBox` | **双向** | `Text="{Binding ..., UpdateSourceTrigger=PropertyChanged}"` |
| `CheckBox` | 双向 | `IsChecked="{Binding ...}"` |
| `PasswordBox` | **无法绑定** | 必须用 code-behind 事件同步 |
| `ComboBox` | 双向 | `ItemsSource` + `SelectedItem` |
| `DataGrid` | 单向集合 | `ItemsSource` |
| `Button` | 命令 | `Command="{Binding ...}"` |

---

## 5. 命令系统（≈ 路由绑定）

> Command ≈ `[HttpPost]` Action，把 UI 事件路由到 ViewModel 方法

### RelayCommand 用法

```csharp
public class MyViewModel : ObservableObject
{
    // 命令声明
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public MyViewModel()
    {
        // 无参数命令
        SaveCommand = new RelayCommand(async _ => await SaveAsync());

        // 带 CanExecute 的命令（按钮自动灰显）
        DeleteCommand = new RelayCommand(
            execute: async p => await DeleteAsync(p?.ToString()),
            canExecute: p => !string.IsNullOrEmpty(SelectedId)
        );
    }

    private async Task SaveAsync()
    {
        // 点击按钮后执行的逻辑
    }
}
```

### 命令生命周期

```
1. View加载 → Command绑定
2. CanExecute 自动被调用 → 决定按钮是否可用
3. 用户点击按钮 → Execute 被调用
4. 调用 RaiseCanExecuteChanged() → 重新评估 CanExecute
```

---

## 6. 依赖注入配置

### 两处注册

```csharp
// ★ 第一处: Services/ServiceCollectionExtensions.cs
// 注册所有业务服务
public static IServiceCollection AddArasToolkitServices(this IServiceCollection services)
{
    services.AddSingleton<IConfigService, ConfigService>();     // 全局共享
    services.AddTransient<IExcelService, ExcelService>();      // 每次新建
    // ...
}

// ★ 第二处: App/App.xaml.cs
// 注册 ViewModels 和 Views
private void ConfigureServices()
{
    services.AddArasToolkitServices();  // 先注册业务层
    services.AddSingleton<MainViewModel>();
    services.AddTransient<LoginViewModel>();
    services.AddTransient<LoginView>();
    // ...
}
```

### 生命周期选择

| 生命周期 | 何时用 | 示例 |
|:---|:---|:---|
| `Singleton` | 全局唯一状态 | ConfigService, ArasConnectionService |
| `Transient` | 每次获取新实例 | ViewModel, View（页面切换） |
| `Scoped` | WPF中不使用 | (仅Web场景) |

### 获取服务

```csharp
// ViewModel 中（构造注入，和 Web API 完全一样）
public ExcelImportViewModel(IExcelService excelService) { ... }

// 代码中手动获取（导航时用）
var vm = App.Services.GetRequiredService<DashboardViewModel>();
```

---

## 7. 样式与控件模板

### 样式层级

```
Theme.xaml        ← 全局样式（所有页面生效）
Window.Resources  ← 窗口级样式（当前窗口生效）
UserControl.Resources ← 页面级样式（当前页面生效）
直接写属性        ← 内联样式（仅此控件）
```

### 修改主题色（四步）

```xml
<!-- 1. 定义颜色 → Theme.xaml 顶部 -->
<Color x:Key="AccentColor">#6366F1</Color>

<!-- 2. 定义画刷 -->
<SolidColorBrush x:Key="AccentBrush" Color="{StaticResource AccentColor}" />

<!-- 3. 在样式中使用画刷 -->
<Setter Property="Background" Value="{StaticResource AccentBrush}" />

<!-- 4. 在页面中引用样式 -->
<Button Style="{StaticResource PrimaryButton}" />
```

### 控件模板（ControlTemplate）

> 控件模板决定控件的**视觉外观**，不改动其**行为**

```xml
<!-- 自定义圆角按钮 -->
<Style x:Key="RoundedButton" TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}" CornerRadius="8">
                    <ContentPresenter HorizontalAlignment="Center"
                                      VerticalAlignment="Center" />
                </Border>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

---

## 8. 新增功能 7 步流程

```
┌──────────────────────────────────────────────────────────┐
│ Step 1: Core/Models/XxxModel.cs                          │
│         定义数据结构（纯 C# 类，无任何依赖）                │
├──────────────────────────────────────────────────────────┤
│ Step 2: Core/Interfaces/IXxxService.cs                   │
│         定义服务接口 Task<Xxx> MethodName();              │
├──────────────────────────────────────────────────────────┤
│ Step 3: Services/Services/XxxService.cs                  │
│         ★ 实现接口，所有业务逻辑、数据处理在这里           │
├──────────────────────────────────────────────────────────┤
│ Step 4: 注册 DI                                          │
│         Services/ServiceCollectionExtensions.cs           │
│         App/App.xaml.cs                                   │
├──────────────────────────────────────────────────────────┤
│ Step 5: App/ViewModels/XxxViewModel.cs                   │
│         ★ 页面状态 + 命令 + 调用Service获取数据            │
├──────────────────────────────────────────────────────────┤
│ Step 6: App/Views/XxxView.xaml + XxxView.xaml.cs         │
│         创建 XAML 布局 + code-behind                      │
├──────────────────────────────────────────────────────────┤
│ Step 7: 注册导航                                          │
│         MainViewModel.cs → 菜单项                         │
│         MainWindow.xaml.cs → 路由                         │
└──────────────────────────────────────────────────────────┘
```

---

## 9. 常用代码片段

### ViewModel 标准模板

```csharp
public class XxxViewModel : ObservableObject
{
    // ===== 依赖服务 =====
    private readonly IXxxService _xxxService;

    // ===== 属性（UI 绑定） =====
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private ObservableCollection<XxxModel> _items = new();
    public ObservableCollection<XxxModel> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    // ===== 命令 =====
    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }

    // ===== 构造函数（DI注入） =====
    public XxxViewModel(IXxxService xxxService)
    {
        _xxxService = xxxService;
        LoadCommand = new RelayCommand(async _ => await LoadAsync());
        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        _ = LoadAsync();  // 启动时自动加载
    }

    // ===== 业务方法 =====
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _xxxService.GetAllAsync();
            Items = new ObservableCollection<XxxModel>(data);
        }
        catch (Exception ex)
        {
            // 可接入 ErrorLogService
        }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        // ...
    }
}
```

### View 标准模板

```xml
<UserControl x:Class="ArasToolkit.App.Views.XxxView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             mc:Ignorable="d"
             d:DesignHeight="600" d:DesignWidth="900"
             Background="Transparent">

    <Grid Margin="32,28">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />   <!-- 标题 -->
            <RowDefinition Height="Auto" />   <!-- 工具栏 -->
            <RowDefinition Height="*" />      <!-- 主内容 -->
        </Grid.RowDefinitions>

        <!-- 标题 -->
        <TextBlock Grid.Row="0" Text="XXX管理" FontSize="26" FontWeight="Bold"
                   Foreground="{StaticResource TextPrimaryBrush}" />

        <!-- 加载中 -->
        <ProgressBar Grid.Row="1"
                     Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}"
                     Style="{StaticResource DarkProgressBar}" />

        <!-- 主内容 -->
        <DataGrid Grid.Row="2"
                  ItemsSource="{Binding Items}"
                  Style="{StaticResource DarkDataGrid}" />
    </Grid>
</UserControl>
```

### Code-behind 标准模板

```csharp
using System.Windows.Controls;
namespace ArasToolkit.App.Views;
public partial class XxxView : UserControl
{
    public XxxView()
    {
        InitializeComponent();
    }
}
```

---

## 10. 常见坑与解决方案

### 坑1: PasswordBox 不能绑定

```csharp
// ❌ XAML 绑定无效
<PasswordBox Password="{Binding Password}" />

// ✅ 必须用 code-behind 事件同步
TxtPassword.PasswordChanged += (s, e) => ViewModel.Password = TxtPassword.Password;
```

### 坑2: 集合不更新

```csharp
// ❌ List<T> 不支持 UI 通知
Items = new List<User>(data);

// ✅ 必须用 ObservableCollection<T>
Items = new ObservableCollection<User>(data);
```

### 坑3: 线程问题

```csharp
// ❌ 后台线程直接改 UI 属性
Task.Run(() => { Name = "新值"; });  // 跨线程异常

// ✅ ViewModel 是 UI 线程安全的
await Task.Run(() => _service.GetData());  // 数据在后台获取
Name = result;  // 赋值回 UI 线程（await 自动恢复上下文）
```

### 坑4: 数据未触发更新

```csharp
// ❌ 直接修改集合项（不会通知UI）
Items[0].Name = "新名称";

// ✅ 替换整个集合并触发
var list = Items.ToList();
list[0].Name = "新名称";
Items = new ObservableCollection<Xxx>(list);

// 或让 Model 也实现 INotifyPropertyChanged
```

### 坑5: XAML 中无法使用泛型

```xml
<!-- ❌ XAML 不支持泛型 -->
<DataTemplate DataType="{x:Type local:MyClass<T>}">

<!-- ✅ 必须用非泛型或 Object -->
```

### 坑6: 绑定路径不匹配

```csharp
// ViewModel 有属性 public string UserName { get; set; }
```

```xml
<!-- ✅ 正确 -->
<TextBlock Text="{Binding UserName}" />

<!-- ❌ 错误：大小写、拼写必须完全一致 -->
<TextBlock Text="{Binding Username}" />
```

---

## 附录：项目文件地图

```
d:\博威\项目\ICS\个人工具箱\
├── ArasToolkit.slnx                     ← 解决方案文件
├── WPF开发规范指南.md                    ← ★ 本文件
└── src/
    ├── ArasToolkit.Core/                ← ★★★ 核心层
    │   ├── Models/                      ← 数据结构
    │   ├── Interfaces/                  ← 服务接口（I前缀）
    │   └── Extensions/                  ← 扩展类
    ├── ArasToolkit.Services/            ← ★★★ 业务逻辑层
    │   ├── Services/                    ← 接口实现（去掉I前缀）
    │   ├── Config/                      ← 运行时配置
    │   └── ServiceCollectionExtensions  ← DI注册
    └── ArasToolkit.App/                 ← ★★★ 表现层
        ├── App.xaml + .cs               ← 启动入口 + DI
        ├── MainWindow.xaml + .cs        ← 主窗口 + 导航
        ├── ViewModels/                  ← ViewModel（≈Controller）
        ├── Views/                       ← XAML 页面
        │   └── Placeholder/             ← 占位页面
        ├── Styles/Theme.xaml            ← 全局样式
        └── Converters/                  ← 值转换器
```

---

> 此文件路径: `d:\博威\项目\ICS\个人工具箱\WPF开发规范指南.md`  
> 随时 `@workspace` 引用本文件获取开发规范。
