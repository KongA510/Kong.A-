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

        // 注册密码回调 — 从 PasswordBox 获取当前输入
        viewModel.OnRequestPassword = () => NewPasswordBox.Password;

        // 每次打开表单清空密码框（编辑模式留空=保留原密码；新增模式空白输入）
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ArasLoginViewModel.IsFormVisible) && viewModel.IsFormVisible)
            {
                NewPasswordBox.Password = "";
            }
        };
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
