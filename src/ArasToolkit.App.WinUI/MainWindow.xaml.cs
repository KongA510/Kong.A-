using System;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI;

/// <summary>
/// 主窗口（WinUI 3）— 登录层 / NavigationView 侧边导航 + Frame 内容区 + 状态栏。
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _mainVM;
    private readonly NavigationService _navService;
    private readonly AppLoginViewModel _appLoginVM;

    public MainWindow()
    {
        _mainVM = App.Services.GetRequiredService<MainViewModel>();
        _navService = App.Services.GetRequiredService<NavigationService>();
        _appLoginVM = App.Services.GetRequiredService<AppLoginViewModel>();

        this.InitializeComponent();

        // 根容器 DataContext 供 {Binding IsLoggedIn} 使用
        RootGrid.DataContext = _mainVM;

        TryResize(1280, 720);
        VersionText.Text = _mainVM.VersionText;

        // 主界面导航
        BuildNavItems();
        _navService.SetFrame(ContentFrame);

        // 登录层
        _appLoginVM.LoginSucceeded += OnLoginSucceeded;
        LoginFrame.Navigate(typeof(AppLoginPage), _appLoginVM);
    }

    /// <summary>登录成功 → 切换到主界面并导航到仪表盘（切回 UI 线程）。</summary>
    private void OnLoginSucceeded()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _mainVM.IsLoggedIn = true;
            _mainVM.RefreshVersion();
            VersionText.Text = _mainVM.VersionText;

            if (NavView.MenuItems.Count > 0)
                NavView.SelectedItem = NavView.MenuItems[0];
            _navService.Navigate("仪表盘");
        });
    }

    private void BuildNavItems()
    {
        foreach (var item in _mainVM.MenuItems)
            NavView.MenuItems.Add(CreateNavItem(item));

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
