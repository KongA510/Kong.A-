using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 敏感操作日志页（WinUI 3）— 操作类型筛选 + 原生表格 + 分页。
/// </summary>
public sealed partial class OperationLogPage : Page
{
    public OperationLogPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<OperationLogViewModel>();
    }
}
