using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 数据报表页（WinUI 3 / LiveCharts2）— 待办任务状态/项目分布柱状图。
/// </summary>
public sealed partial class ChartPage : Page
{
    public ChartPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<ChartViewModel>();
    }
}
