using System.Windows;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// 设置窗口 — 数据库检查 / 退出登录 / 连接字符串管理
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.LogoutRequested += OnLogoutRequested;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnLogoutRequested()
    {
        DialogResult = true;
        Close();
    }
}
