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

        // 添加 R37lib 子目录到程序集探测路径
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var asmName = new System.Reflection.AssemblyName(args.Name).Name;
            var probePath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory, "R37lib", asmName + ".dll");
            if (System.IO.File.Exists(probePath))
                return System.Reflection.Assembly.LoadFrom(probePath);
            return null;
        };    }

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
        services.AddTransient<AppLoginViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ExcelImportViewModel>();
        services.AddTransient<ChangelogViewModel>();
        services.AddTransient<ErrorLogViewModel>();
        services.AddTransient<OperationLogViewModel>();
        services.AddTransient<TodoViewModel>();
        services.AddTransient<ChartViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ArasLoginViewModel>();
        services.AddTransient<DataImportViewModel>();
        services.AddTransient<SettingsWindow>();
        services.AddTransient<ArasLoginWindow>();
        services.AddTransient<DataImportView>();
       services.AddTransient<ConfigSelectWindow>();
        services.AddTransient<TextPromptWindow>();

        // 注册Views
        services.AddTransient<MainWindow>();
        services.AddTransient<AppLoginView>();
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

