using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class ObjectClassConfigurationPage : Page
{
    private ObjectClassConfigurationViewModel Vm => (ObjectClassConfigurationViewModel)DataContext;

    public ObjectClassConfigurationPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ObjectClassConfigurationViewModel>();
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
        => await Vm.InitializeAsync();

    private void Page_Unloaded(object sender, RoutedEventArgs e)
        => Vm.Dispose();

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
        => Vm.NotifySelectionChanged();
}
