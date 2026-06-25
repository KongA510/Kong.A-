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

    /// <summary>
    /// ＋ 新增按钮 — 展开表单区域
    /// </summary>
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ArasLoginViewModel vm)
            vm.NewUrl = vm.NewUrl.Length > 0 ? "" : " "; // 切换表单显示
    }

    /// <summary>
    /// 密码输入事件 — 将 PasswordBox 的密码写入 ViewModel 回调
    /// </summary>
    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ArasLoginViewModel vm)
            vm.OnRequestPassword = () => NewPasswordBox.Password;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
