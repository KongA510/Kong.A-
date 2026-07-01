using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.Views;

public partial class TranslationHistoryView : UserControl
{
    public TranslationHistoryView()
    {
        InitializeComponent();
    }

    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is TextTranslationRecord record)
        {
            try
            {
                if (System.IO.File.Exists(record.OutputFilePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = record.OutputFilePath,
                        UseShellExecute = true
                    });
                }
            }
            catch
            {
                // 忽略打开文件失败
            }
        }
    }
}
