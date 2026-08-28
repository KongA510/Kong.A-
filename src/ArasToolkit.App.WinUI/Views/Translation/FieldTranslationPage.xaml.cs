using ArasToolkit.App.WinUI.ViewModels;
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

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
        => Vm.NotifySelectionChanged();

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        => ApplyResponsiveLayout(e.NewSize.Width < 760);

    private void ApplyResponsiveLayout(bool compact)
    {
        ConnectionGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        Grid.SetRow(ConnectionStatus, 0);
        Grid.SetColumn(ConnectionStatus, 0);
        Grid.SetRow(AiStatus, compact ? 1 : 0);
        Grid.SetColumn(AiStatus, compact ? 0 : 1);

        QueryGrid.ColumnDefinitions[0].Width = new GridLength(compact ? 1 : 2, GridUnitType.Star);
        QueryGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
        QueryGrid.ColumnDefinitions[2].Width = compact ? new GridLength(0) : GridLength.Auto;
        QueryGrid.ColumnDefinitions[3].Width = compact ? new GridLength(0) : GridLength.Auto;

        Place(ItemTypeComboBox, 0, 0);
        Place(ItemTypeSearchBox, compact ? 1 : 0, compact ? 0 : 1);
        Place(RefreshItemTypesButton, compact ? 2 : 0, compact ? 0 : 2);
        Place(LoadFormsButton, compact ? 3 : 0, compact ? 0 : 3);
        Place(FormsComboBox, compact ? 4 : 1, 0, compact ? 1 : 2);
        Place(LoadFieldsButton, compact ? 5 : 1, compact ? 0 : 3);
        Place(FilterSummaryText, compact ? 6 : 2, 0, compact ? 1 : 4);
    }

    private static void Place(FrameworkElement element, int row, int column, int columnSpan = 1)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
        Grid.SetColumnSpan(element, columnSpan);
    }
}
