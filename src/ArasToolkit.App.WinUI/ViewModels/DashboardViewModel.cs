using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 仪表盘ViewModel
/// </summary>
public class DashboardViewModel : ObservableObject
{
    private readonly IArasConnectionService _connectionService;

    private string _connectionStatus = "未连接";
    private string _connectedServer = "";
    private string _connectedDatabase = "";
    private string _connectedUser = "";
    private string _loginTime = "";
    private string _appUsername = "";
    private string _appDisplayName = "";
    private string _appLoginTime = "";

    public DashboardViewModel(IArasConnectionService connectionService)
    {
        _connectionService = connectionService;
        _connectionService.ConnectionChanged += RefreshConnectionInfo;
        RefreshConnectionInfo();
        RefreshAppUserInfo();

        RefreshCommand = new RelayCommand(_ => RefreshConnectionInfo());
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    public string ConnectedServer
    {
        get => _connectedServer;
        set => SetProperty(ref _connectedServer, value);
    }

    public string ConnectedDatabase
    {
        get => _connectedDatabase;
        set => SetProperty(ref _connectedDatabase, value);
    }

    public string ConnectedUser
    {
        get => _connectedUser;
        set => SetProperty(ref _connectedUser, value);
    }

    public string LoginTime
    {
        get => _loginTime;
        set => SetProperty(ref _loginTime, value);
    }

    public string AppUsername
    {
        get => _appUsername;
        set => SetProperty(ref _appUsername, value);
    }

    public string AppDisplayName
    {
        get => _appDisplayName;
        set => SetProperty(ref _appDisplayName, value);
    }

    public string AppLoginTime
    {
        get => _appLoginTime;
        set => SetProperty(ref _appLoginTime, value);
    }

    public ObservableCollection<QuickAction> AllFeatures { get; } = new()
    {
        new() { Name = "文本翻译", Description = "AI 驱动的 Excel 批量翻译工具", Icon = "\uE70F" },
        new() { Name = "字段翻译", Description = "按对象类翻译 Aras Property 标签", Icon = "\uE8D2" },
        new() { Name = "表单翻译", Description = "按对象类翻译关联 Form 标签", Icon = "\uE70F" },
        new() { Name = "窗体翻译", Description = "翻译 Form Field 标签与说明", Icon = "\uE8A7" },
        new() { Name = "窗体配置", Description = "按四列坐标规范生成 Aras 经典窗体", Icon = "\uE8B9" },
        new() { Name = "对象类汇入", Description = "通过 Excel 批量创建 Aras 对象类和关系类", Icon = "\uE8B5" },
        new() { Name = "对象类配置", Description = "批量配置权限页签、可创建者和标准生命周期", Icon = "\uE7B8" },
        new() { Name = "List配置", Description = "批量创建Aras List主档和菜单项", Icon = "\uE8FD" },
        new() { Name = "属性配置", Description = "批量创建Aras对象类属性定义", Icon = "\uE90F" },
        new() { Name = "数据汇入", Description = "Aras 数据汇入工具", Icon = "\uE8B5" },
        new() { Name = "权限配置", Description = "Aras权限配置工具", Icon = "\uE72E" },
        new() { Name = "生命周期配置", Description = "批量创建Aras对象类的生命周期状态定义", Icon = "\uE895" },
        new() { Name = "数据库导出", Description = "执行SQL查询并导出为Excel文件", Icon = "\uE898" },
        new() { Name = "更新日志", Description = "系统更新日志与版本记录", Icon = "\uE81C" },
        new() { Name = "错误日志", Description = "系统错误记录与排查", Icon = "\uEBE8" },
        new() { Name = "敏感操作日志", Description = "业务操作审计轨迹记录", Icon = "\uE72E" },
        new() { Name = "Aras翻译日志", Description = "字段、表单、窗体翻译任务与明细", Icon = "\uE81C" },
        new() { Name = "个人任务记录", Description = "个人待办任务管理与追踪", Icon = "\uE73E" },
        new() { Name = "我的资料", Description = "文件资源管理器", Icon = "\uE8B7" },
        new() { Name = "用户管理", Description = "管理员创建用户与角色分配", Icon = "\uE716" },
   };

    public ObservableCollection<DashboardMetric> ArasMetrics { get; } = [];
    public ObservableCollection<DashboardMetric> UserMetrics { get; } = [];

    public ICommand RefreshCommand { get; }

    private void RefreshConnectionInfo()
    {
        var conn = _connectionService.CurrentConnection;
        if (conn != null && _connectionService.IsConnected)
        {
            ConnectionStatus = "已连接";
            ConnectedServer = conn.Url;
            ConnectedDatabase = conn.Database;
            ConnectedUser = conn.Username;
            LoginTime = conn.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        }
        else
        {
            ConnectionStatus = "未连接";
            ConnectedServer = "";
            ConnectedDatabase = "";
            ConnectedUser = "";
            LoginTime = "";
        }

        ReplaceMetrics(ArasMetrics,
            new DashboardMetric("服务器", ConnectedServer, "\uE968"),
            new DashboardMetric("数据库", ConnectedDatabase, "\uE8B5"),
            new DashboardMetric("Aras 用户", ConnectedUser, "\uE77B"),
            new DashboardMetric("登录时间", LoginTime, "\uE823"));
    }

    private void RefreshAppUserInfo()
    {
        var user = CurrentUserContext.Current;
        if (user != null)
        {
            AppUsername = user.Username;
            AppDisplayName = user.DisplayName ?? user.Username;
            AppLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            AppUsername = "未登录";
            AppDisplayName = "未登录";
            AppLoginTime = "";
        }

        ReplaceMetrics(UserMetrics,
            new DashboardMetric("应用账号", AppUsername, "\uE77B"),
            new DashboardMetric("显示名称", AppDisplayName, "\uE716"),
            new DashboardMetric("本次登录", AppLoginTime, "\uE823"));
    }

    private static void ReplaceMetrics(
        ObservableCollection<DashboardMetric> target,
        params DashboardMetric[] values)
    {
        target.Clear();
        foreach (var value in values)
            target.Add(value);
    }
}

public sealed record DashboardMetric(string Label, string Value, string Icon);

/// <summary>
/// 快速操作项
/// </summary>
public class QuickAction
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

