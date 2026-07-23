using System;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI;

/// <summary>
/// 主窗口（WinUI 3）— NavigationView 侧边导航 + Frame 内容区 + 状态栏。
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _mainVM;
    private readonly NavigationService _navService;

    public MainWindow()
    {
        this.InitializeComponent();

        _mainVM = App.Services.GetRequiredService<MainViewModel>();
        _navService = App.Services.GetRequiredService<NavigationService>();

        // 窗口尺寸（与 WPF 版一致 1280x720）
        TryResize(1280, 720);

        // 状态栏版本号
        VersionText.Text = _mainVM.VersionText;

        // 构建导航菜单
        BuildNavItems();

        // 注册内容 Frame 到导航服务
        _navService.SetFrame(ContentFrame);

        // 初始导航到仪表盘
        if (NavView.MenuItems.Count > 0)
            NavView.SelectedItem = NavView.MenuItems[0];
        _navService.Navigate("仪表盘");
    }

    /// <summary>按 MainViewModel.MenuItems 构建 NavigationView 项（支持二级层级）。</summary>
    private void BuildNavItems()
    {
        foreach (var item in _mainVM.MenuItems)
            NavView.MenuItems.Add(CreateNavItem(item));

        // 设置入口置于底部 Footer
        NavView.FooterMenuItems.Add(new NavigationViewItem
        {
            Content = "设置",
            Tag = "设置",
            Icon = new SymbolIcon(Symbol.Setting)
        });
    }

    private static NavigationViewItem CreateNavItem(MenuItemInfo item)
    {
        var nvi = new NavigationViewItem
        {
            Content = item.Name,
            Tag = item.Name,
            Icon = new FontIcon { Glyph = item.CardIcon, FontFamily = new FontFamily("Segoe UI Emoji") }
        };
        foreach (var child in item.Children)
            nvi.MenuItems.Add(CreateNavItem(child));
        return nvi;
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem { Tag: string name })
            _navService.Navigate(name);
    }

    private void TryResize(int width, int height)
    {
        try
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MainWindow] 调整窗口尺寸失败: {ex.Message}");
        }
    }
}
