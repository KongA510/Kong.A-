using System.Windows;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// AI 模型配置管理窗口 — 多 API Key 的增删改启用
/// </summary>
public partial class TranslationApiKeyWindow : Window
{
    public TranslationApiKeyWindow(TranslationApiKeyViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
