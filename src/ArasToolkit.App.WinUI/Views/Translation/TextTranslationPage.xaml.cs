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

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 760;
        ApplyGridMode(SourceGrid, compact,
            [GridLength.Auto, new GridLength(1, GridUnitType.Star), GridLength.Auto, GridLength.Auto],
            SourceLabel, SourcePathBox, BrowseSourceButton, TranslateButton);
        ApplyGridMode(ModeGrid, compact,
            [GridLength.Auto, new GridLength(160), new GridLength(1, GridUnitType.Star), GridLength.Auto],
            ModeLabel, ModeComboBox, SourceLanguagePanel, DownloadTemplateButton);
        ApplyGridMode(SheetGrid, compact,
            [GridLength.Auto, new GridLength(200), new GridLength(1, GridUnitType.Star)],
            SheetLabel, SheetComboBox, SheetHint);
        ApplyGridMode(ColumnMappingGrid, compact,
            [GridLength.Auto, new GridLength(200), GridLength.Auto, new GridLength(200), GridLength.Auto, new GridLength(140)],
            SourceColumnLabel, SourceColumnComboBox,
            TargetColumnLabel, TargetColumnComboBox, TargetLanguageLabel, TargetLanguageComboBox);
    }

    private static void ApplyGridMode(
        Grid grid,
        bool compact,
        GridLength[] originalWidths,
        params FrameworkElement[] elements)
    {
        for (var index = 0; index < grid.ColumnDefinitions.Count; index++)
        {
            grid.ColumnDefinitions[index].Width = compact
                ? index == 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(0)
                : originalWidths[index];
        }

        for (var index = 0; index < elements.Length; index++)
        {
            Grid.SetRow(elements[index], compact ? index : 0);
            Grid.SetColumn(elements[index], compact ? 0 : index);
        }
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
