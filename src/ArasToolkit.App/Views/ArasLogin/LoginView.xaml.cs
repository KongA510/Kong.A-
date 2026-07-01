using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.Views;

/// <summary>
/// LoginView.xaml 的交互逻辑
/// </summary>
public partial class LoginView : UserControl
{
    private LoginViewModel? ViewModel => DataContext as LoginViewModel;

    public LoginView()
    {
        InitializeComponent();
        
        // 密码框与ViewModel双向同步
        TxtPassword.PasswordChanged += (s, e) =>
        {
            if (ViewModel != null)
                ViewModel.Password = TxtPassword.Password;
        };

        // 注册PasswordBox更新回调：当ViewModel选择已保存记录时，同步更新密码框
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.PasswordBoxUpdater = (password) =>
            {
                Dispatcher.Invoke(() => TxtPassword.Password = password);
            };
        }
    }

    private void SavedLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is LoginInfo loginInfo)
        {
            ViewModel?.SelectSavedLoginCommand.Execute(loginInfo);
        }
    }

    private void DeleteSavedLogin_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string configName)
        {
            ViewModel?.DeleteSavedLoginCommand.Execute(configName);
        }
    }
}
