using System.Windows;
using ArasToolkit.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.App.Views;

/// <summary>
/// 设置窗口 — 数据库检查 / AI模型配置 / 退出登录 / 连接字符串管理
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

    private void OpenAiModelWindow_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.Services.GetRequiredService<TranslationApiKeyViewModel>();
        var window = new TranslationApiKeyWindow(vm) { Owner = this };
        window.ShowDialog();
    }

    private void OnLogoutRequested()
    {
        DialogResult = true;
        Close();
    }
}
