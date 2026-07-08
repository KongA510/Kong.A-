using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// AI 模型配置管理页（内嵌视图）— 替代原 TranslationApiKeyWindow 模态弹窗
/// </summary>
public partial class TranslationApiKeyView : UserControl
{
    public TranslationApiKeyView(TranslationApiKeyViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;
        mainWindow?.NavigateToPage("设置");
    }
}
