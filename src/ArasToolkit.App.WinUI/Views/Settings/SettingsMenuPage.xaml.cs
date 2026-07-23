using System.Collections.Generic;
using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 设置子仪表盘（WinUI 3）— 设置功能卡片入口（点击导航到对应设置页）。
/// </summary>
public sealed partial class SettingsMenuPage : Page
{
    public SettingsMenuPage()
    {
        this.InitializeComponent();
        SettingsGrid.ItemsSource = new List<QuickAction>
        {
            new QuickAction { Name = "设置-资料文件夹地址", Icon = "📁", Description = "设置个人资料文件的存储位置" },
            new QuickAction { Name = "设置-AI模型配置", Icon = "🤖", Description = "管理多个AI模型API Key，支持启用/切换" },
            new QuickAction { Name = "设置-数据库检查", Icon = "🔍", Description = "检查数据库表结构完整性，同步缺失字段" },
            new QuickAction { Name = "设置-数据库连接字符串", Icon = "🗄️", Description = "配置应用数据库连接，保存后即时生效" },
            new QuickAction { Name = "设置-数据库导出", Icon = "📤", Description = "配置并执行数据库查询导出" },
            new QuickAction { Name = "设置-退出登录", Icon = "🚪", Description = "退出当前登录，返回登录界面" },
        };
    }

    private void SettingsGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is QuickAction action)
            App.Services.GetRequiredService<NavigationService>().Navigate(action.Name);
    }
}
