using System.Windows;
using ArasToolkit.App.ViewModels;
using ArasToolkit.App.ViewModels.Translation;
using ArasToolkit.App.Views.Translation;
using ArasToolkit.App.Views.TaskLoadAnalysis;
using ArasToolkit.App.Views;
using ArasToolkit.App.Views.TaskLoadAnalysis;
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
        services.AddTransient<ChangelogViewModel>();
        services.AddTransient<ErrorLogViewModel>();
        services.AddTransient<OperationLogViewModel>();
        services.AddTransient<TodoViewModel>();
        services.AddTransient<ChartViewModel>();
        services.AddTransient<TextTranslationViewModel>();
        services.AddTransient<FieldTranslationViewModel>();
        services.AddTransient<PropertyTranslationViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ArasLoginViewModel>();
        services.AddTransient<TranslationApiKeyViewModel>();
        services.AddTransient<TranslationApiKeyView>();
        services.AddTransient<DataImportViewModel>();
        services.AddTransient<ObjectClassConfigViewModel>();
        services.AddTransient<ListConfigViewModel>();
        services.AddTransient<PropertyConfigViewModel>();
        services.AddTransient<PermissionConfigViewModel>();
        services.AddTransient<LifecycleConfigViewModel>();
        services.AddTransient<FileExplorerViewModel>();
        services.AddTransient<KnowledgeViewModel>();
        services.AddTransient<DatabaseExportViewModel>();
        services.AddTransient<DatabaseExportConfigViewModel>();
        services.AddTransient<ArasLoginWindow>();
        services.AddTransient<TranslationApiKeyWindow>();
        services.AddTransient<DataImportView>();
        services.AddTransient<TaskLoadAnalysisViewModel>();
        services.AddTransient<TaskLoadAnalysisView>();
        services.AddTransient<ConfigSelectWindow>();
        services.AddTransient<TextPromptWindow>();

        // 注册Views
        services.AddTransient<MainWindow>();
        services.AddTransient<AppLoginView>();
        services.AddTransient<LoginView>();
        services.AddTransient<DashboardView>();
        services.AddTransient<ChangelogView>();
        services.AddTransient<ErrorLogView>();
        services.AddTransient<OperationLogView>();
        services.AddTransient<TodoView>();
        services.AddTransient<ChartView>();
        services.AddTransient<TextTranslationView>();
        services.AddTransient<FieldTranslationView>();
        services.AddTransient<PropertyTranslationView>();
        services.AddTransient<TranslationHistoryView>();
        services.AddTransient<ObjectClassConfigView>();
        services.AddTransient<ListConfigView>();
        services.AddTransient<PropertyConfigView>();
        services.AddTransient<PermissionConfigView>();
        services.AddTransient<LifecycleConfigView>();
        services.AddTransient<FileExplorerView>();
        services.AddTransient<KnowledgeBaseView>();
        services.AddTransient<DatabaseExportView>();
        services.AddTransient<DatabaseExportConfigView>();
        services.AddTransient<TaskLoadAnalysisView>();
        services.AddTransient<TaskLoadAnalysisViewModel>();

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


