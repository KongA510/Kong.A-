using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 应用登录页（一级登录）。DataContext 由导航参数（AppLoginViewModel）提供。
/// </summary>
public sealed partial class AppLoginPage : Page
{
    public AppLoginPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter != null)
            DataContext = e.Parameter;
    }
}
