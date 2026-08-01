using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class ArasTranslationLogPage : Page
{
    public ArasTranslationLogPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ArasTranslationLogViewModel>();
    }
}
