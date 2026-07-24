using System.Diagnostics;
using System.IO;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 文本翻译页（WinUI 3）— AI 驱动的 Excel 批量翻译（多模式 + 进度 + 历史）。
/// </summary>
public sealed partial class TextTranslationPage : Page
{
    public TextTranslationPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<TextTranslationViewModel>();
    }

    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: TextTranslationRecord record })
        {
            try
            {
                if (File.Exists(record.OutputFilePath))
                    Process.Start(new ProcessStartInfo { FileName = record.OutputFilePath, UseShellExecute = true });
            }
            catch
            {
                // 忽略打开文件失败
            }
        }
    }
}
