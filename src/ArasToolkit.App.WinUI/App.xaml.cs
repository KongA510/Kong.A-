using Microsoft.UI.Xaml;

namespace ArasToolkit.App.WinUI;

/// <summary>
/// 应用程序入口（WinUI 3）。阶段1：最小启动外壳，DI 与全局资源在阶段2接入。
/// </summary>
public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    private Window? _window;
}
