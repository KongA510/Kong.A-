using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>Aras 经典窗体配置页面。</summary>
public sealed partial class FormConfigurationPage : Page
{
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
}
