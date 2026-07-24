using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 数据库导出连接配置页（WinUI 3）— 连接配置增删改 + 启用切换。
/// </summary>
public sealed partial class DatabaseExportConfigPage : Page
{
    public DatabaseExportConfigPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<DatabaseExportConfigViewModel>();
    }

    private DatabaseExportConfigViewModel? Vm => DataContext as DatabaseExportConfigViewModel;

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        App.Services.GetRequiredService<NavigationService>().Navigate("设置");
    }

    private void EnableButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DatabaseExportConfig record)
            Vm?.EnableCommand.Execute(record.Id);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DatabaseExportConfig record)
            Vm?.EditCommand.Execute(record);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DatabaseExportConfig record)
            Vm?.DeleteCommand.Execute(record.Id);
    }
}
