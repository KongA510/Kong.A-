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

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 900;
        WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(320);
        WorkspaceGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.RowDefinitions[1].Height = compact ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        Grid.SetRow(UserListPanel, 0);
        Grid.SetColumn(UserListPanel, 0);
        Grid.SetRow(EditorPanel, compact ? 1 : 0);
        Grid.SetColumn(EditorPanel, compact ? 0 : 1);
        EditorPanel.BorderThickness = compact ? new Thickness(0, 1, 0, 0) : new Thickness(1, 0, 0, 0);
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
