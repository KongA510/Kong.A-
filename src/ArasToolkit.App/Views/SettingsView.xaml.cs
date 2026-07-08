using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// 设置页（内嵌视图）— 替代原 SettingsWindow 模态弹窗
/// </summary>
public partial class SettingsView : UserControl
{
    private readonly SettingsViewModel _settingsVM;

    public SettingsView(SettingsViewModel viewModel, string mode)
    {
        InitializeComponent();
        _settingsVM = viewModel;
        DataContext = viewModel;

        _settingsVM.LogoutRequested += OnLogoutRequested;

        ShowPanel(mode);
    }

    private void ShowPanel(string mode)
    {
        DataFolderPanel.Visibility = Visibility.Collapsed;
        AiModelPanel.Visibility = Visibility.Collapsed;
        DatabasePanel.Visibility = Visibility.Collapsed;
        ConnectionPanel.Visibility = Visibility.Collapsed;
        LogoutPanel.Visibility = Visibility.Collapsed;

        switch (mode)
        {
            case "datafolder":
                DataFolderPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "资料文件夹地址";
                break;
            case "aimodel":
                AiModelPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "AI 模型配置";
                break;
            case "database":
                DatabasePanel.Visibility = Visibility.Visible;
                PageTitle.Text = "数据库检查";
                break;
            case "connection":
                ConnectionPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "数据库连接字符串";
                break;
            case "logout":
                LogoutPanel.Visibility = Visibility.Visible;
                PageTitle.Text = "退出登录";
                break;
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;
        mainWindow?.NavigateToPage("设置");
    }

    private void OnLogoutRequested()
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;
        if (mainWindow != null)
        {
            mainWindow.ResetToLogin();
        }
    }
}
