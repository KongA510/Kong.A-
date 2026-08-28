using System.Diagnostics;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>属性配置四段式导入工作台。</summary>
public sealed partial class PropertyConfigPage : Page
{
    private const double CompactWidth = 760;
    private const double WideWidth = 1180;
    private PropertyConfigViewModel ViewModel { get; }

    public PropertyConfigPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PropertyConfigViewModel>();
        DataContext = ViewModel;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await ViewModel.InitializeAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= OnUnloaded;
        ViewModel.Dispose();
    }

    private void ItemTypeSearchBox_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is ArasItemTypeInfo itemType)
            ViewModel.SelectItemTypeFromSearch(itemType);
    }

    private void ItemTypeSearchBox_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var itemType = args.ChosenSuggestion as ArasItemTypeInfo
                       ?? ViewModel.FilteredItemTypes.FirstOrDefault();
        if (itemType != null)
            ViewModel.SelectItemTypeFromSearch(itemType);
    }

    private async void ShowAml_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: PropertyImportPreviewRow row } || !row.HasAmlPreview)
            return;

        var content = new TextBox
        {
            Text = row.AmlPreview,
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinWidth = 640,
            MinHeight = 260,
            FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Cascadia Mono")
        };
        var dialog = new ContentDialog
        {
            Title = $"第 {row.ExcelRowNumber} 行 · {row.Name} · {row.PlannedAction}",
            Content = content,
            CloseButtonText = "关闭",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = XamlRoot
        };
        await dialog.ShowAsync();
    }

    private static void OpenWithShell(string path)
    {
        try
        {
            if (File.Exists(path))
                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
        catch
        {
            // 打开归档文件不影响导入流程。
        }
    }

    private void OpenImportFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: PropertyImportLog record })
            OpenWithShell(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, record.ImportFile));
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string logPath } && !string.IsNullOrWhiteSpace(logPath))
            OpenWithShell(logPath);
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width >= WideWidth)
        {
            PageContent.Margin = new Thickness(32, 24, 32, 36);
            SetStepColumns(4);
            PlaceStep(StepOneCard, 0, 0);
            PlaceStep(StepTwoCard, 0, 1);
            PlaceStep(StepThreeCard, 0, 2);
            PlaceStep(StepFourCard, 0, 3);
            return;
        }

        if (e.NewSize.Width >= CompactWidth)
        {
            PageContent.Margin = new Thickness(24, 20, 24, 32);
            SetStepColumns(2);
            PlaceStep(StepOneCard, 0, 0);
            PlaceStep(StepTwoCard, 0, 1);
            PlaceStep(StepThreeCard, 1, 0);
            PlaceStep(StepFourCard, 1, 1);
            return;
        }

        PageContent.Margin = new Thickness(16, 16, 16, 28);
        SetStepColumns(1);
        PlaceStep(StepOneCard, 0, 0);
        PlaceStep(StepTwoCard, 1, 0);
        PlaceStep(StepThreeCard, 2, 0);
        PlaceStep(StepFourCard, 3, 0);
    }

    private void SetStepColumns(int visibleColumns)
    {
        for (var index = 0; index < StepGrid.ColumnDefinitions.Count; index++)
            StepGrid.ColumnDefinitions[index].Width = index < visibleColumns
                ? new GridLength(1, GridUnitType.Star)
                : new GridLength(0);
    }

    private static void PlaceStep(FrameworkElement element, int row, int column)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
    }
}
