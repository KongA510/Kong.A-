using System.Linq;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 通用占位页 — 显示导航参数（菜单名），用于尚未迁移的功能页面。
/// </summary>
public sealed partial class PlaceholderPage : Page
{
    public PlaceholderPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var name = e.Parameter?.ToString() ?? "页面";
        TitleText.Text = name;
        var mainViewModel = App.Services.GetRequiredService<MainViewModel>();
        var menu = mainViewModel.MenuItems.FirstOrDefault(item => item.Name == name);

        if (menu?.Children.Count > 0)
        {
            DescText.Text = menu.Description;
            FeatureGrid.ItemsSource = menu.Children;
            FeatureGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            EmptyState.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            return;
        }

        DescText.Text = $"{name} — 当前功能入口已保留。";
        FeatureGrid.ItemsSource = null;
        FeatureGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        EmptyState.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    }

    private void FeatureGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not MenuItemInfo item)
            return;

        App.Services.GetRequiredService<NavigationService>().Navigate(item.Name);
    }
}
