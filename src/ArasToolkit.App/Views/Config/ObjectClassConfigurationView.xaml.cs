using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

public partial class ObjectClassConfigurationView : UserControl
{
    public ObjectClassConfigurationView()
    {
        InitializeComponent();
        Loaded += View_Loaded;
    }

    private async void View_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ObjectClassConfigurationViewModel viewModel)
            await viewModel.InitializeAsync();
    }

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ObjectClassConfigurationViewModel viewModel)
            viewModel.NotifySelectionChanged();
    }
}
