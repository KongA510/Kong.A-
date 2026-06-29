using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

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

    public DashboardViewModel(IArasConnectionService connectionService)
    {
        _connectionService = connectionService;
        RefreshConnectionInfo();
        
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

    public ObservableCollection<QuickAction> AllFeatures { get; } = new()
    {
        new() { Name = "导入表格", Description = "读取Excel文件并按Sheet显示数据", Icon = "📊" },
        new() { Name = "文本翻译", Description = "AI 驱动的 Excel 批量翻译工具", Icon = "📝" },
        new() { Name = "字段翻译", Description = "Aras字段翻译工具", Icon = "🔤" },
        new() { Name = "表单翻译", Description = "Aras表单翻译工具", Icon = "📝" },
        new() { Name = "窗体翻译", Description = "Aras窗体翻译工具", Icon = "🪟" },
        new() { Name = "窗体配置", Description = "Aras窗体配置工具", Icon = "⚙️" },
        new() { Name = "对象类配置", Description = "批量创建Aras对象类和关系类", Icon = "📦" },
        new() { Name = "属性配置", Description = "Aras属性配置工具", Icon = "🔧" },
        new() { Name = "数据汇入", Description = "Aras 数据汇入工具", Icon = "📀" },
        new() { Name = "权限配置", Description = "Aras权限配置工具", Icon = "🔒" },
        new() { Name = "更新日志", Description = "系统更新日志与版本记录", Icon = "📜" },
        new() { Name = "错误日志", Description = "系统错误记录与排查", Icon = "🐛" },
        new() { Name = "敏感操作日志", Description = "业务操作审计轨迹记录", Icon = "🔒" },
        new() { Name = "数据报表", Description = "数据统计与图表可视化", Icon = "📊" },
        new() { Name = "待办项目", Description = "个人待办任务管理与追踪", Icon = "📀" },
        new() { Name = "我的资料", Description = "文件资源管理器", Icon = "📁" },
    };

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
    }
}

/// <summary>
/// 快速操作项
/// </summary>
public class QuickAction
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
