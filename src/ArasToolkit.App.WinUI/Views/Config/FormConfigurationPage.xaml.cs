using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>Aras 经典窗体配置页面。</summary>
public sealed partial class FormConfigurationPage : Page
{
    private const double CompactLayoutWidth = 760;
    private const double WideLayoutWidth = 1180;

    private FormConfigurationViewModel ViewModel { get; }

    public FormConfigurationPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<FormConfigurationViewModel>();
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
        if (args.SelectedItem is ArasToolkit.Core.Models.ArasItemTypeInfo itemType)
            ViewModel.SelectItemTypeFromSearch(itemType);
    }

    private void ItemTypeSearchBox_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var itemType = args.ChosenSuggestion as ArasToolkit.Core.Models.ArasItemTypeInfo
                       ?? ViewModel.FilteredItemTypes.FirstOrDefault();
        if (itemType != null)
            ViewModel.SelectItemTypeFromSearch(itemType);
    }

    private void LayoutList_DragItemsCompleted(
        ListViewBase sender,
        DragItemsCompletedEventArgs args)
    {
        if (args.DropResult == Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move)
            ViewModel.NormalizeLayoutAfterReorder();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width >= WideLayoutWidth)
        {
            ApplyWideLayout();
            return;
        }

        ApplyStackedWorkspace();
        if (e.NewSize.Width >= CompactLayoutWidth)
            ApplyMediumLayout();
        else
            ApplyCompactLayout();
    }

    private void ApplyWideLayout()
    {
        PageContent.Margin = new Thickness(32, 24, 32, 36);
        SetSelectionColumnWidths(
            new GridLength(2, GridUnitType.Star),
            new GridLength(2, GridUnitType.Star),
            new GridLength(2, GridUnitType.Star),
            GridLength.Auto,
            GridLength.Auto);

        Place(ItemTypeComboBox, 0, 0);
        Place(ItemTypeSearchBox, 0, 1);
        Place(FormNameTextBox, 0, 2);
        Place(RefreshItemTypesButton, 0, 3);
        Place(LoadPropertiesButton, 0, 4);
        Place(SelectionOptions, 1, 0, 5);
        SelectionOptions.Orientation = Orientation.Horizontal;

        WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(5, GridUnitType.Star);
        WorkspaceGrid.ColumnDefinitions[1].Width = new GridLength(7, GridUnitType.Star);
        Place(PropertiesCard, 0, 0);
        Place(PreviewCard, 0, 1);
        ApplyActionRow(false);
    }

    private void ApplyMediumLayout()
    {
        PageContent.Margin = new Thickness(24, 20, 24, 32);
        SetSelectionColumnWidths(
            new GridLength(2, GridUnitType.Star),
            new GridLength(2, GridUnitType.Star),
            new GridLength(2, GridUnitType.Star),
            new GridLength(0),
            new GridLength(0));

        Place(ItemTypeComboBox, 0, 0);
        Place(ItemTypeSearchBox, 0, 1);
        Place(FormNameTextBox, 0, 2);
        Place(RefreshItemTypesButton, 1, 1);
        Place(LoadPropertiesButton, 1, 2);
        Place(SelectionOptions, 2, 0, 3);
        SelectionOptions.Orientation = Orientation.Horizontal;
        ApplyActionRow(false);
    }

    private void ApplyCompactLayout()
    {
        PageContent.Margin = new Thickness(16, 16, 16, 28);
        SetSelectionColumnWidths(
            new GridLength(1, GridUnitType.Star),
            new GridLength(0),
            new GridLength(0),
            new GridLength(0),
            new GridLength(0));

        Place(ItemTypeComboBox, 0, 0);
        Place(ItemTypeSearchBox, 1, 0);
        Place(FormNameTextBox, 2, 0);
        Place(RefreshItemTypesButton, 3, 0);
        Place(LoadPropertiesButton, 4, 0);
        Place(SelectionOptions, 5, 0);
        SelectionOptions.Orientation = Orientation.Vertical;
        ApplyActionRow(true);
    }

    private void ApplyStackedWorkspace()
    {
        WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.ColumnDefinitions[1].Width = new GridLength(0);
        Place(PropertiesCard, 0, 0);
        Place(PreviewCard, 1, 0);
    }

    private void ApplyActionRow(bool compact)
    {
        Place(ActionHint, 0, 0, compact ? 2 : 1);
        Place(ApplyButton, compact ? 1 : 0, compact ? 0 : 1, compact ? 2 : 1);
    }

    private void SetSelectionColumnWidths(params GridLength[] widths)
    {
        for (var index = 0; index < widths.Length; index++)
            SelectionGrid.ColumnDefinitions[index].Width = widths[index];
    }

    private static void Place(FrameworkElement element, int row, int column, int columnSpan = 1)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
        Grid.SetColumnSpan(element, columnSpan);
    }
}
