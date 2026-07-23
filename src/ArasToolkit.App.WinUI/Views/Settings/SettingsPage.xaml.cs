using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 设置页（WinUI 3）— 按导航参数显示对应面板：资料文件夹 / 数据库检查 / 连接字符串 / 退出登录。
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        var vm = App.Services.GetRequiredService<SettingsViewModel>();
        vm.LogoutRequested += OnLogoutRequested;
        DataContext = vm;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ShowPanel(e.Parameter as string ?? "");
    }

    private void ShowPanel(string menuName)
    {
        DataFolderPanel.Visibility = Visibility.Collapsed;
        DatabasePanel.Visibility = Visibility.Collapsed;
        ConnectionPanel.Visibility = Visibility.Collapsed;
        LogoutPanel.Visibility = Visibility.Collapsed;

        switch (menuName)
        {
            case "设置-资料文件夹地址":
                DataFolderPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "资料文件夹地址";
                break;
            case "设置-数据库检查":
                DatabasePanel.Visibility = Visibility.Visible;
                PageTitle.Text = "数据库检查";
                break;
            case "设置-数据库连接字符串":
                ConnectionPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "数据库连接字符串";
                break;
            case "设置-退出登录":
                LogoutPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "退出登录";
                break;
            default:
                PageTitle.Text = "设置";
                break;
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        App.Services.GetRequiredService<NavigationService>().Navigate("设置");
    }

    private void OnLogoutRequested()
    {
        (App.MainWindow as MainWindow)?.ResetToLogin();
    }
}
