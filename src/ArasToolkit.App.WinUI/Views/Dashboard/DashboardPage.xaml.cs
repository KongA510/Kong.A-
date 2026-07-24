using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 仪表盘页（WinUI 3）— 连接/用户信息卡片 + 全部功能入口（点击导航）。
/// </summary>
public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<DashboardViewModel>();
    }

    /// <summary>功能卡片点击 → 导航到对应页面。</summary>
    private void FeatureGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is QuickAction action)
        {
            var nav = App.Services.GetRequiredService<NavigationService>();
            nav.Navigate(action.Name);
        }
    }
}
