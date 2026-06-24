using System.Windows;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// Aras 登录信息管理窗口 — 查看/切换/删除已保存的 Aras 连接
/// </summary>
public partial class ArasLoginWindow : Window
{
    public ArasLoginWindow(ArasLoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
