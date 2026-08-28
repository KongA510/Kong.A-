using System;
using System.Threading.Tasks;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Composition.SystemBackdrops;
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
    private readonly IArasConnectionService _connectionService;

    public MainWindow()
    {
        _mainVM = App.Services.GetRequiredService<MainViewModel>();
        _navService = App.Services.GetRequiredService<NavigationService>();
        _appLoginVM = App.Services.GetRequiredService<AppLoginViewModel>();
        _connectionService = App.Services.GetRequiredService<IArasConnectionService>();

        this.InitializeComponent();

        // 跟随 Windows 主题启用 Mica 背景，失效时由透明/系统背景自然回退。
        SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };

        // 根容器 DataContext 供 {Binding IsLoggedIn} 使用
        RootGrid.DataContext = _mainVM;

        TryResize(1360, 820);
        VersionText.Text = _mainVM.VersionText;

        // 主界面导航
        BuildNavItems();
        _navService.SetFrame(ContentFrame);
        ContentFrame.Navigated += (_, _) => AttachResponsiveLayout(ContentFrame);
        LoginFrame.Navigated += (_, _) => AttachResponsiveLayout(LoginFrame);

        // 登录层
        _appLoginVM.LoginSucceeded += OnLoginSucceeded;
        LoginFrame.Navigate(typeof(AppLoginPage), _appLoginVM);
    }

    private static void AttachResponsiveLayout(Frame frame)
    {
        if (frame.Content is Page page)
        {
            ResponsivePageCoordinator.Attach(page);
        }
    }

    /// <summary>登录成功 → 切换到主界面并导航到仪表盘（切回 UI 线程）。</summary>
    private void OnLoginSucceeded()
    {
        // 应用账号发生切换时不得沿用上一个账号的全局 Aras 会话。
        // 先清除旧连接，再由当前账号的默认配置重新建立连接。
        _connectionService.Disconnect();

        DispatcherQueue.TryEnqueue(() =>
        {
            _mainVM.IsLoggedIn = true;
            _mainVM.RefreshVersion();
            VersionText.Text = _mainVM.VersionText;

            if (NavView.MenuItems.Count > 0)
                NavView.SelectedItem = NavView.MenuItems[0];
            _navService.Navigate("仪表盘");
        });

        // 后台自动连接 Aras（不强制导航，避免干扰用户已手动切换的页面）
        _ = Task.Run(async () =>
        {
            try
            {
                var arasLoginVM = App.Services.GetRequiredService<ArasLoginViewModel>();
                await arasLoginVM.TryAutoConnectAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindow] Aras自动连接异常: {ex.Message}");
            }
        });
    }

    /// <summary>退出登录 → 切回登录界面并重置导航选中态。</summary>
    public void ResetToLogin()
    {
        _connectionService.Disconnect();
        _mainVM.IsLoggedIn = false;
        _mainVM.SelectedMenuItem = null;
        NavView.SelectedItem = null;
        CurrentUserContext.Current = null;
        LoginFrame.Navigate(typeof(AppLoginPage), _appLoginVM);
    }

    private void BuildNavItems()
    {
        foreach (var item in _mainVM.MenuItems)
            NavView.MenuItems.Add(CreateNavItem(item));

        NavView.FooterMenuItems.Add(new NavigationViewItem
        {
            Content = "Aras连接",
            Tag = "Aras连接",
            Icon = new SymbolIcon(Symbol.Link)
        });

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
            Icon = new FontIcon { Glyph = item.CardIcon, FontFamily = new FontFamily("Segoe Fluent Icons") }
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
