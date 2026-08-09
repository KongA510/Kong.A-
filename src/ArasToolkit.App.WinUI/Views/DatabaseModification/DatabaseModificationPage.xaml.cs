using System.ComponentModel;
using System.Data;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class DatabaseModificationPage : Page
{
    public DatabaseModificationPage()
    {
        InitializeComponent();
        var viewModel = App.Services.GetRequiredService<DatabaseModificationViewModel>();
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        DataContext = viewModel;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DatabaseModificationViewModel viewModel)
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DatabaseModificationViewModel.ExcelPreview)
            && sender is DatabaseModificationViewModel viewModel)
        {
            RenderExcelPreview(viewModel.ExcelPreview);
        }
    }

    /// <summary>窄屏时把双预览由左右分栏切换为上下排列。</summary>
    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 920;
        RangePanel.Orientation = compact ? Orientation.Vertical : Orientation.Horizontal;
        ExecutionPanel.Orientation = compact ? Orientation.Vertical : Orientation.Horizontal;

        WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.ColumnDefinitions[1].Width = compact
            ? new GridLength(0)
            : new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.RowDefinitions[0].Height = GridLength.Auto;
        WorkspaceGrid.RowDefinitions[1].Height = compact
            ? GridLength.Auto
            : new GridLength(0);

        Grid.SetRow(ExcelPreviewPanel, 0);
        Grid.SetColumn(ExcelPreviewPanel, 0);
        Grid.SetRow(SqlWorkspacePanel, compact ? 1 : 0);
        Grid.SetColumn(SqlWorkspacePanel, compact ? 0 : 1);
    }

    /// <summary>WinUI 3 无原生自动列 DataGrid，因此按 DataTable 动态绘制轻量预览表。</summary>
    private void RenderExcelPreview(DataTable? table)
    {
        ExcelPreviewContainer.Children.Clear();
        if (table == null || table.Columns.Count == 0)
        {
            ExcelPreviewPlaceholder.Visibility = Visibility.Visible;
            return;
        }

        ExcelPreviewPlaceholder.Visibility = Visibility.Collapsed;
        const double cellWidth = 150;
        var textBrush = (Brush)Application.Current.Resources["TextPrimaryBrush"];
        var secondaryBrush = (Brush)Application.Current.Resources["TextSecondaryBrush"];

        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xEE, 0xF2, 0xFF))
        };
        foreach (DataColumn column in table.Columns)
        {
            header.Children.Add(new TextBlock
            {
                Text = column.ColumnName,
                Width = cellWidth,
                Padding = new Thickness(8, 9, 8, 9),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = textBrush,
                TextTrimming = TextTrimming.CharacterEllipsis
            });
        }
        ExcelPreviewContainer.Children.Add(header);

        for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
        {
            var rowPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Background = rowIndex % 2 == 0
                    ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xFF, 0xFF, 0xFF))
                    : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xF8, 0xFA, 0xFC))
            };
            foreach (DataColumn column in table.Columns)
            {
                rowPanel.Children.Add(new TextBlock
                {
                    Text = table.Rows[rowIndex][column]?.ToString() ?? string.Empty,
                    Width = cellWidth,
                    Padding = new Thickness(8, 7, 8, 7),
                    FontSize = 11,
                    Foreground = string.IsNullOrWhiteSpace(table.Rows[rowIndex][column]?.ToString())
                        ? secondaryBrush
                        : textBrush,
                    TextTrimming = TextTrimming.CharacterEllipsis
                });
            }
            ExcelPreviewContainer.Children.Add(rowPanel);
        }
    }
}
