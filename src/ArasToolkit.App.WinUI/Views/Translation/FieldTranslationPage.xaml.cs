using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class FieldTranslationPage : Page
{
    private FieldTranslationViewModel Vm => (FieldTranslationViewModel)DataContext;

    public FieldTranslationPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<FieldTranslationViewModel>();
        Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
        => await Vm.InitializeAsync();

    private void ItemTypeSearch_SuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is not ItemTypeItem itemType) return;
        Vm.SelectedItemType = itemType;
        Vm.ItemTypeSearchText = itemType.DisplayName;
    }

    private async void ItemTypeSearch_QuerySubmitted(
        AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var itemType = args.ChosenSuggestion as ItemTypeItem ?? Vm.FilteredItemTypes.FirstOrDefault();
        await Vm.SelectItemTypeFromSearchAsync(itemType);
    }

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
        => Vm.NotifySelectionChanged();
}
