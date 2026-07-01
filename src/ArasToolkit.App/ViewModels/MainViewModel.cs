using System;
using System.Collections.ObjectModel;
using System.Windows;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 主窗口ViewModel - 管理导航和全局状态
/// </summary>
public class MainViewModel : ObservableObject
{
    private readonly IChangelogService? _changelogService;
    private object? _currentView;
    private MenuItemInfo? _selectedMenuItem;
    private bool _isLoggedIn;
    private bool _isLoading;
    private string _versionText = "个人工具箱 v1.0";

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public MenuItemInfo? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>左下角版本号文本，从数据库读取最新版本</summary>
    public string VersionText
    {
        get => _versionText;
        set => SetProperty(ref _versionText, value);
    }

    public ObservableCollection<MenuItemInfo> MenuItems { get; } = new();

    public MainViewModel(IChangelogService? changelogService = null)
    {
        _changelogService = changelogService;
        InitializeMenuItems();
        RefreshVersion();
    }

    /// <summary>
    /// 从数据库获取最新版本号并刷新左下角显示
    /// </summary>
    public void RefreshVersion()
    {
        try
        {
            if (_changelogService != null)
            {
                var ver = _changelogService.GetCurrentVersion();
                VersionText = $"个人工具箱 v{ver}";
            }
        }
        catch
        {
            VersionText = "个人工具箱 v1.0";
        }
    }

    private void InitializeMenuItems()
    {
        // ===== 一级菜单：仪表盘 =====
        MenuItems.Add(new MenuItemInfo { Name = "仪表盘", Icon = "Home", CardIcon = "🏠", Description = "系统概览仪表盘" });

        // ===== 一级菜单：导入表格 =====
        MenuItems.Add(new MenuItemInfo { Name = "导入表格", Icon = "FileExcel", CardIcon = "📊", Description = "导入并查看Excel数据" });

        // ===== 二级菜单：系统翻译（字段 / 表单 / 窗体） =====
        var sysTranslation = new MenuItemInfo
        {
            Name = "系统翻译",
            Icon = "Translate",
            CardIcon = "🌐",
            Description = "Aras翻译工具集"
        };
        sysTranslation.Children.Add(new MenuItemInfo { Name = "文本翻译", Icon = "Translate", CardIcon = "📝", Description = "AI 驱动的 Excel 批量翻译工具" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "翻译历史", Icon = "History", CardIcon = "📜", Description = "历史翻译记录查询" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "字段翻译", Icon = "FormTextbox", CardIcon = "🔤", Description = "Aras字段翻译工具", IsPlaceholder = true });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "表单翻译", Icon = "FormTextbox", CardIcon = "📝", Description = "Aras表单翻译工具", IsPlaceholder = true });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "窗体翻译", Icon = "WindowMaximize", CardIcon = "🪟", Description = "Aras窗体翻译工具", IsPlaceholder = true });
        MenuItems.Add(sysTranslation);

        // ===== 二级菜单：系统配置（窗体 / 对象类 / 属性 / List / 权限） =====
        var sysConfig = new MenuItemInfo
        {
            Name = "系统配置",
            Icon = "Cog",
            CardIcon = "⚙️",
            Description = "Aras系统配置工具集"
        };
        sysConfig.Children.Add(new MenuItemInfo { Name = "窗体配置", Icon = "Cog", CardIcon = "🖼️", Description = "Aras窗体配置工具", IsPlaceholder = true });
        sysConfig.Children.Add(new MenuItemInfo { Name = "对象类配置", Icon = "CubeOutline", CardIcon = "📦", Description = "批量创建Aras对象类和关系类" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "List配置", Icon = "FormatListBulleted", CardIcon = "📋", Description = "批量创建Aras List主档和菜单项" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "属性配置", Icon = "Tune", CardIcon = "🔧", Description = "批量创建Aras对象类属性定义" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "数据汇入", Icon = "DatabaseImport", CardIcon = "📀", Description = "Excel数据导入Aras系统" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "权限配置", Icon = "ShieldAccount", CardIcon = "🔒", Description = "Aras权限配置工具", IsPlaceholder = true });
        MenuItems.Add(sysConfig);

        // ===== 二级菜单：系统日志（更新日志 / 错误日志 / 敏感操作日志） =====
        var sysLog = new MenuItemInfo
        {
            Name = "系统日志",
            Icon = "History",
            CardIcon = "📋",
            Description = "系统日志与审计记录"
        };
        sysLog.Children.Add(new MenuItemInfo { Name = "更新日志", Icon = "History", CardIcon = "📜", Description = "系统更新日志与版本记录" });
        sysLog.Children.Add(new MenuItemInfo { Name = "错误日志", Icon = "Bug", CardIcon = "🐛", Description = "系统错误记录与排查" });
        sysLog.Children.Add(new MenuItemInfo { Name = "敏感操作日志", Icon = "ShieldAccount", CardIcon = "🔒", Description = "业务操作审计轨迹记录" });
        MenuItems.Add(sysLog);

        // ===== 数据报表 =====
        MenuItems.Add(new MenuItemInfo { Name = "数据报表", Icon = "ChartBar", CardIcon = "📊", Description = "数据统计与图表可视化" });

        // ===== 待办项目 =====
        MenuItems.Add(new MenuItemInfo { Name = "待办项目", Icon = "ClipboardCheck", CardIcon = "📋", Description = "个人待办任务管理" });

        // ===== 我的资料 =====
        MenuItems.Add(new MenuItemInfo { Name = "我的资料", Icon = "Folder", CardIcon = "📁", Description = "文件资源管理器" });
    }

    public void ShowLogin()
    {
        var loginVM = App.Services.GetService(typeof(LoginViewModel)) as LoginViewModel;
        CurrentView = loginVM;
    }
}
