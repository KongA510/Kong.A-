using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;
using ArasToolkit.App.ViewModels;
using ArasToolkit.App.Views;
using ArasToolkit.App.Views.Placeholder;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.App;

/// <summary>
/// MainWindow.xaml 的交互逻辑 - 主窗口（含左侧导航）
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _mainVM;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        _mainVM = mainViewModel;
        DataContext = _mainVM;

       // 初始化显示登录界面
        ShowAppLoginView();
   }

   /// <summary>
    /// TreeView 鼠标点击事件 — 父节点：展开/折叠 + 子级仪表盘；叶节点：导航到功能页
    /// </summary>
    private void NavTree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = FindParentTreeViewItem(e.OriginalSource as DependencyObject);
        if (treeViewItem?.DataContext is not MenuItemInfo menuItem)
            return;

        if (menuItem.HasChildren)
        {
            treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
            if (treeViewItem.IsExpanded)
                ShowChildDashboard(menuItem);
            e.Handled = true;
        }
        else
        {
            NavigateToPage(menuItem);
        }
    }

    private static TreeViewItem? FindParentTreeViewItem(DependencyObject? child)
    {
        while (child != null)
        {
            if (child is TreeViewItem tvi)
                return tvi;
            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }

    private void ShowChildDashboard(MenuItemInfo parentItem)
    {
        var dashboard = new PlaceholderView();
        dashboard.SetTitle(parentItem.Name, $"共 {parentItem.Children.Count} 个子功能");
        dashboard.AddChildItems(parentItem.Children, NavigateToPage);
        MainContentControl.Content = dashboard;
        _mainVM.SelectedMenuItem = parentItem;
    }

   public void NavigateToPage(string name)
   {
       NavigateToPage(new MenuItemInfo { Name = name });
   }

    private void ShowAppLoginView()
   {
        var appLoginVM = App.Services.GetRequiredService<AppLoginViewModel>();
        appLoginVM.LoginSucceeded += OnAppLoginSucceeded;
        LoginContentControl.Content = new AppLoginView { DataContext = appLoginVM };
   }

    private void OnAppLoginSucceeded()
   {
       _mainVM.IsLoggedIn = true;

       if (_mainVM.MenuItems.Count > 0)
       {
           _mainVM.SelectedMenuItem = _mainVM.MenuItems[0];
           NavigateToPage(_mainVM.MenuItems[0]);
       }
   }

    /// <summary>
    /// 根据菜单项切换右侧内容
    /// </summary>
    private void NavigateToPage(MenuItemInfo menuItem)
    {
        try
        {
            object? view = menuItem.Name switch
            {
                "仪表盘" => new DashboardView { DataContext = App.Services.GetRequiredService<DashboardViewModel>() },
                "导入表格" => new ExcelImportView { DataContext = App.Services.GetRequiredService<ExcelImportViewModel>() },
                "文本翻译" => new TextTranslationView { DataContext = App.Services.GetRequiredService<TextTranslationViewModel>() },
                "翻译历史" => new TranslationHistoryView { DataContext = App.Services.GetRequiredService<TextTranslationViewModel>() },
                "字段翻译" => CreatePlaceholder("字段翻译", "Aras字段翻译工具 - 功能开发中，敬请期待..."),
                "表单翻译" => CreatePlaceholder("表单翻译", "Aras表单翻译工具 - 功能开发中，敬请期待..."),
                "窗体翻译" => CreatePlaceholder("窗体翻译", "Aras窗体翻译工具 - 功能开发中，敬请期待..."),
                "窗体配置" => CreatePlaceholder("窗体配置", "Aras窗体配置工具 - 功能开发中，敬请期待..."),
                "对象类配置" => CreatePlaceholder("对象类配置", "Aras对象类配置工具 - 功能开发中，敬请期待..."),
                "属性配置" => CreatePlaceholder("属性配置", "Aras属性配置工具 - 功能开发中，敬请期待..."),
                "数据汇入" => new DataImportView { DataContext = App.Services.GetRequiredService<DataImportViewModel>() },
                "权限配置" => CreatePlaceholder("权限配置", "Aras权限配置工具 - 功能开发中，敬请期待..."),
                "更新日志" => new ChangelogView { DataContext = App.Services.GetRequiredService<ChangelogViewModel>() },
                "错误日志" => new ErrorLogView { DataContext = App.Services.GetRequiredService<ErrorLogViewModel>() },
                "敏感操作日志" => new OperationLogView { DataContext = App.Services.GetRequiredService<OperationLogViewModel>() },
                "数据报表" => new ChartView { DataContext = App.Services.GetRequiredService<ChartViewModel>() },
                "待办项目" => new TodoView { DataContext = App.Services.GetRequiredService<TodoViewModel>() },
                _ => null
            };

            if (view != null)
            {
                MainContentControl.Content = view;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NavigateToPage] 导航失败 ({menuItem.Name}): {ex.Message}");
            MainContentControl.Content = new TextBlock
            {
                Text = $"页面加载失败: {ex.Message}",
                FontSize = 16,
                Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26)),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }
    }


    private static PlaceholderView CreatePlaceholder(string title, string description)
    {
        var view = new PlaceholderView();
        view.SetTitle(title, description);
        return view;
    }

    #region 标题栏事件
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
        else if (e.ClickCount == 2)
            ToggleMaximize();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsVM = App.Services.GetRequiredService<SettingsViewModel>();
        var window = new SettingsWindow(settingsVM);
        window.Owner = this;
            settingsVM.LogoutRequested += () =>
            {
                _mainVM.IsLoggedIn = false;
                MainContentControl.Content = null;
                _mainVM.SelectedMenuItem = null;
                ShowAppLoginView();
            };
            window.ShowDialog();
        }
 
         private void ArasLoginButton_Click(object sender, RoutedEventArgs e)
         {
             var vm = App.Services.GetRequiredService<ArasLoginViewModel>();
             var window = new ArasLoginWindow(vm);
             window.Owner = this;
             window.ShowDialog();
         }
        #endregion
    }
