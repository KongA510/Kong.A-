using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class FormTranslationPage : Page
{
    private FormTranslationViewModel Vm => (FormTranslationViewModel)DataContext;

    public FormTranslationPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<FormTranslationViewModel>();
        Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
        => await Vm.InitializeAsync();

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
        => Vm.NotifySelectionChanged();
}
