using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 用户管理页面（仅管理员可见）
/// </summary>
public sealed partial class UserManagePage : Page
{
    private UserManageViewModel? Vm => DataContext as UserManageViewModel;

    public UserManagePage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<UserManageViewModel>();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: AppUser user })
            Vm?.EditUserCommand.Execute(user);
    }

    private void ResetPwdButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: AppUser user })
            Vm?.ResetPasswordCommand.Execute(user);
    }

    private void ToggleActiveButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: AppUser user })
            Vm?.ToggleActiveCommand.Execute(user);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: AppUser user })
            Vm?.DeleteUserCommand.Execute(user);
    }
}
