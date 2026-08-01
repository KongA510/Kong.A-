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
}
