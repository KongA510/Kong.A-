using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// Aras 登录信息管理页（WinUI 3）— 查看/切换/编辑/删除已保存的 Aras 连接配置。
/// </summary>
public sealed partial class ArasLoginPage : Page
{
    public ArasLoginPage()
    {
        this.InitializeComponent();
        var vm = App.Services.GetRequiredService<ArasLoginViewModel>();
        DataContext = vm;

        // 注册密码回调 — 从 PasswordBox 获取当前输入
        vm.OnRequestPassword = () => NewPasswordBox.Password;

        // 每次打开表单清空密码框（编辑模式留空=保留原密码；新增模式空白输入）
        vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ArasLoginViewModel.IsFormVisible) && vm.IsFormVisible)
                NewPasswordBox.Password = "";
        };
    }

    private ArasLoginViewModel? Vm => DataContext as ArasLoginViewModel;

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: LoginInfo item })
            Vm?.EditCommand.Execute(item);
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: LoginInfo item })
            Vm?.ConnectCommand.Execute(item);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: LoginInfo item })
            Vm?.DeleteCommand.Execute(item.Id);
    }
}
