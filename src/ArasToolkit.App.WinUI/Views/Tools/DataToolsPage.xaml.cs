using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>XML / JSON 格式化、比对与实体转换共享页面。</summary>
public sealed partial class DataToolsPage : Page
{
    private DataToolsViewModel Vm => (DataToolsViewModel)DataContext;

    public DataToolsPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<DataToolsViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Vm.Configure(e.Parameter?.ToString());
    }

    private void CopyOutput_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Vm.OutputText)) return;
        var package = new DataPackage();
        package.SetText(Vm.OutputText);
        Clipboard.SetContent(package);
        Vm.NotifyOutputCopied();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        => ApplyResponsiveLayout(e.NewSize.Width < 920);

    private void ApplyResponsiveLayout(bool compact)
    {
        TransformGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        Grid.SetRow(TransformOutputCard, compact ? 1 : 0);
        Grid.SetColumn(TransformOutputCard, compact ? 0 : 1);

        CompareInputGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        Grid.SetRow(CompareRightCard, compact ? 1 : 0);
        Grid.SetColumn(CompareRightCard, compact ? 0 : 1);

        var transformHeight = compact ? 280 : 380;
        TransformInputEditor.MinHeight = transformHeight;
        TransformOutputEditor.MinHeight = transformHeight;
        var compareHeight = compact ? 220 : 270;
        CompareLeftEditor.MinHeight = compareHeight;
        CompareRightEditor.MinHeight = compareHeight;
    }
}
