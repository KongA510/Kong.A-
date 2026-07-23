using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 错误日志页（WinUI 3）— 级别/日期筛选 + 原生表格 + 分页。
/// </summary>
public sealed partial class ErrorLogPage : Page
{
    public ErrorLogPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<ErrorLogViewModel>();
    }
}
