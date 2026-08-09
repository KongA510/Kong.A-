using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

public partial class ClassStructureImportView : UserControl
{
    public ClassStructureImportView()
    {
        InitializeComponent();
        Loaded += View_Loaded;
    }

    private async void View_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ClassStructureImportViewModel viewModel)
            await viewModel.InitializeAsync();
    }
}
