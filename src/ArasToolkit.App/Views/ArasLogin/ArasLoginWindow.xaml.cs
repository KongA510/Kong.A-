using System.Windows;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// Aras 登录信息管理窗口 — 查看/切换/删除已保存的 Aras 连接
/// </summary>
public partial class ArasLoginWindow : Window
{
    private bool _isPasswordPlaceholder;
    private bool _isSettingPasswordProgrammatically;

    public ArasLoginWindow(ArasLoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // 注册密码回调
        viewModel.OnRequestPassword = () => NewPasswordBox.Password;
        viewModel.OnIsPasswordPlaceholder = () => _isPasswordPlaceholder;

        // 监听表单显示状态变化，编辑模式时设置密码占位符
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ArasLoginViewModel.IsFormVisible))
            {
                if (viewModel.IsFormVisible && viewModel.IsEditMode)
                {
                    // 编辑模式 — 密码框显示占位符（程序赋值，不触发用户输入逻辑）
                    _isSettingPasswordProgrammatically = true;
                    NewPasswordBox.Password = "••••••••";
                    _isPasswordPlaceholder = true;
                }
                else if (viewModel.IsFormVisible && !viewModel.IsEditMode)
                {
                    // 新增模式 — 清空密码框
                    _isSettingPasswordProgrammatically = true;
                    NewPasswordBox.Password = "";
                    _isPasswordPlaceholder = false;
                }
            }
        };
    }

    /// <summary>
    /// 密码输入事件 — 仅在用户真实输入时清除占位符标记
    /// </summary>
    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_isSettingPasswordProgrammatically)
        {
            _isSettingPasswordProgrammatically = false;
            return;
        }
        _isPasswordPlaceholder = false;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
