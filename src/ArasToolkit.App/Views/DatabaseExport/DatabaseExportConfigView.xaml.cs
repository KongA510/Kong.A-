using System.Windows;
using System.Windows.Controls;

namespace ArasToolkit.App.Views;

public partial class DatabaseExportConfigView : UserControl
{
    public DatabaseExportConfigView()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;
        mainWindow?.NavigateToPage("设置");
    }
}
