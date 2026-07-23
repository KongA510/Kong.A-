using System;
using Microsoft.UI.Xaml;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.App.WinUI;

/// <summary>
/// 应用程序入口（WinUI 3）。负责依赖注入容器、R37lib 程序集探测与主窗口启动。
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>全局服务提供器。</summary>
    public static ServiceProvider Services =>
        ((App)Current)._serviceProvider ?? throw new InvalidOperationException("服务尚未初始化");

    /// <summary>全局主窗口引用（供对话框/文件服务获取 XamlRoot / HWND）。</summary>
    public static Window? MainWindow { get; set; }

    public App()
    {
        this.InitializeComponent();
        ConfigureServices();

        // R37lib 子目录程序集探测（IOM.dll 运行时依赖）
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var asmName = new System.Reflection.AssemblyName(args.Name).Name;
            var probePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "R37lib", asmName + ".dll");
            return System.IO.File.Exists(probePath) ? System.Reflection.Assembly.LoadFrom(probePath) : null;
        };
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // 服务层（EF Core / EPPlus / Aras IOM / 各业务服务）
        services.AddArasToolkitServices();

        // 对话框 / 文件对话框服务
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();

        // 导航服务
        services.AddSingleton<NavigationService>();

        // 主窗口 ViewModel（单例）
        services.AddSingleton<MainViewModel>();
        // 应用登录 ViewModel（单例，MainWindow 与 AppLoginPage 共享实例）
        services.AddSingleton<ViewModels.AppLoginViewModel>();

        // 功能 ViewModel（阶段4-1：简单展示视图）
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ChangelogViewModel>();
        services.AddTransient<ErrorLogViewModel>();
        services.AddTransient<OperationLogViewModel>();
        services.AddTransient<TodoViewModel>();
        services.AddTransient<ObjectClassConfigViewModel>();
        services.AddTransient<ListConfigViewModel>();
        services.AddTransient<PropertyConfigViewModel>();
        services.AddTransient<PermissionConfigViewModel>();
        services.AddTransient<LifecycleConfigViewModel>();
        services.AddTransient<FieldTranslationViewModel>();
        services.AddTransient<PropertyTranslationViewModel>();
        services.AddTransient<TextTranslationViewModel>();
        services.AddTransient<DatabaseExportViewModel>();
        services.AddTransient<DatabaseExportConfigViewModel>();
        services.AddTransient<TranslationApiKeyViewModel>();
        services.AddTransient<DataImportViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<FileExplorerViewModel>();
        services.AddTransient<ChartViewModel>();
        services.AddTransient<ArasLoginViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        // 导航路由表（菜单名 → 页面类型）。未注册菜单回退 PlaceholderPage。
        var nav = _serviceProvider.GetRequiredService<NavigationService>();
        nav.Register("仪表盘", typeof(DashboardPage));
        nav.Register("更新日志", typeof(ChangelogPage));
        nav.Register("错误日志", typeof(ErrorLogPage));
        nav.Register("敏感操作日志", typeof(OperationLogPage));
        nav.Register("个人任务记录", typeof(TodoPage));
        nav.Register("对象类配置", typeof(ObjectClassConfigPage));
        nav.Register("List配置", typeof(ListConfigPage));
        nav.Register("属性配置", typeof(PropertyConfigPage));
        nav.Register("权限配置", typeof(PermissionConfigPage));
        nav.Register("生命周期配置", typeof(LifecycleConfigPage));
        nav.Register("字段翻译", typeof(FieldTranslationPage));
        nav.Register("表单翻译", typeof(FieldTranslationPage));
        nav.Register("窗体翻译", typeof(PropertyTranslationPage));
        nav.Register("文本翻译", typeof(TextTranslationPage));
        nav.Register("翻译历史", typeof(TranslationHistoryPage));
        nav.Register("数据库导出", typeof(DatabaseExportPage));
        nav.Register("设置-数据库导出", typeof(DatabaseExportConfigPage));
        nav.Register("设置-AI模型配置", typeof(TranslationApiKeyPage));
        nav.Register("数据汇入", typeof(DataImportPage));
        nav.Register("设置", typeof(SettingsMenuPage));
        nav.Register("设置-资料文件夹地址", typeof(SettingsPage));
        nav.Register("设置-数据库检查", typeof(SettingsPage));
        nav.Register("设置-数据库连接字符串", typeof(SettingsPage));
        nav.Register("设置-退出登录", typeof(SettingsPage));
        nav.Register("我的资料", typeof(FileExplorerPage));
        nav.Register("数据报表", typeof(ChartPage));
        nav.Register("Aras连接", typeof(ArasLoginPage));
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}
