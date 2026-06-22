using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// DashboardView.xaml 的交互逻辑
/// </summary>
public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 仪表盘功能卡片点击 → 导航到对应页面
    /// </summary>
    private void FeatureCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is QuickAction action)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(action.Name);
        }
    }
}
