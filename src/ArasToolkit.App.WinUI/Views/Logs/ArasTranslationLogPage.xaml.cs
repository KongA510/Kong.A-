using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class ArasTranslationLogPage : Page
{
    public ArasTranslationLogPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ArasTranslationLogViewModel>();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 760;
        var normalWidths = new[]
        {
            new GridLength(160),
            new GridLength(160),
            new GridLength(1, GridUnitType.Star),
            GridLength.Auto,
            GridLength.Auto
        };

        for (var index = 0; index < FilterGrid.ColumnDefinitions.Count; index++)
        {
            FilterGrid.ColumnDefinitions[index].Width = compact
                ? index == 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(0)
                : normalWidths[index];
        }

        Place(TaskTypeComboBox, compact ? 0 : 0, 0);
        Place(StatusComboBox, compact ? 1 : 0, compact ? 0 : 1);
        Place(SearchTextBox, compact ? 2 : 0, compact ? 0 : 2);
        Place(ApplyFilterButton, compact ? 3 : 0, compact ? 0 : 3);
        Place(ClearFilterButton, compact ? 4 : 0, compact ? 0 : 4);
    }

    private static void Place(FrameworkElement element, int row, int column)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
    }
}
