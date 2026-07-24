using ArasToolkit.App.WinUI.Services;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// AI 模型配置管理页（WinUI 3）— API Key 增删改 + 启用切换。
/// </summary>
public sealed partial class TranslationApiKeyPage : Page
{
    public TranslationApiKeyPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<TranslationApiKeyViewModel>();
    }

    private TranslationApiKeyViewModel? Vm => DataContext as TranslationApiKeyViewModel;

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        App.Services.GetRequiredService<NavigationService>().Navigate("设置");
    }

    private void EnableButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AiModelConfig record)
            Vm?.EnableCommand.Execute(record.Id);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AiModelConfig record)
            Vm?.EditCommand.Execute(record);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AiModelConfig record)
            Vm?.DeleteCommand.Execute(record.Id);
    }
}
