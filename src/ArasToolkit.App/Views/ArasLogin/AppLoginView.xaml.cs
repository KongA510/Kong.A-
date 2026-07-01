using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// AppLoginView.xaml 的交互逻辑
/// </summary>
public partial class AppLoginView : UserControl
{
    private AppLoginViewModel? ViewModel => DataContext as AppLoginViewModel;

    public AppLoginView()
    {
        InitializeComponent();

        // 密码框与ViewModel双向同步
        TxtPassword.PasswordChanged += (s, e) =>
        {
            if (ViewModel != null)
                ViewModel.Password = TxtPassword.Password;
        };

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
}
