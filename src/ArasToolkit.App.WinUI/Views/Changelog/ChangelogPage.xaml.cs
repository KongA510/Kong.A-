using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 更新日志页（WinUI 3）— 版本/统计卡片 + 类型筛选 + 时间线列表。
/// </summary>
public sealed partial class ChangelogPage : Page
{
    public ChangelogPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<ChangelogViewModel>();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 760;
        for (var index = 0; index < StatsGrid.ColumnDefinitions.Count; index++)
        {
            StatsGrid.ColumnDefinitions[index].Width = compact && index >= 2
                ? new GridLength(0)
                : new GridLength(1, GridUnitType.Star);
        }

        Place(TotalCard, 0, 0);
        Place(NewCard, 0, 1);
        Place(FixCard, compact ? 1 : 0, compact ? 0 : 2);
        Place(OptimizeCard, compact ? 1 : 0, compact ? 1 : 3);
        FilterToolbar.Orientation = compact ? Orientation.Vertical : Orientation.Horizontal;
    }

    private static void Place(FrameworkElement element, int row, int column)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
    }
}
