using System.Windows;
using ArasToolkit.App.ViewModels;
using ArasToolkit.App.Views;
using ArasToolkit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// 全局服务提供器
    /// </summary>
    public static ServiceProvider Services => 
        ((App)Current)._serviceProvider ?? throw new InvalidOperationException("服务尚未初始化");

    public App()
    {
        ConfigureServices();
    }

    /// <summary>
    /// 配置依赖注入容器
    /// </summary>
    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // 注册服务层
        services.AddArasToolkitServices();

        // 注册ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ExcelImportViewModel>();
        services.AddTransient<ChangelogViewModel>();
        services.AddTransient<ErrorLogViewModel>();
        services.AddTransient<OperationLogViewModel>();
        services.AddTransient<TodoViewModel>();
        services.AddTransient<ChartViewModel>();

        // 注册Views
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginView>();
        services.AddTransient<DashboardView>();
        services.AddTransient<ExcelImportView>();
        services.AddTransient<ChangelogView>();
        services.AddTransient<ErrorLogView>();
        services.AddTransient<OperationLogView>();
        services.AddTransient<TodoView>();
        services.AddTransient<ChartView>();

        _serviceProvider = services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

