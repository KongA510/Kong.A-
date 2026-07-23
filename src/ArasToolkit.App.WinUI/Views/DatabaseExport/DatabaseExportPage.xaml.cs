using System.Diagnostics;
using System.IO;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 数据库导出页（WinUI 3）— SQL 查询导出 Excel + 模板管理 + 历史记录。
/// </summary>
public sealed partial class DatabaseExportPage : Page
{
    public DatabaseExportPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<DatabaseExportViewModel>();
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string logPath && string.IsNullOrWhiteSpace(logPath) == false)
        {
            try { if (File.Exists(logPath)) Process.Start(new ProcessStartInfo { FileName = logPath, UseShellExecute = true }); } catch { }
        }
    }

    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DatabaseExportLog record)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(record.FilePath) == false && File.Exists(record.FilePath))
                {
                    Process.Start(new ProcessStartInfo { FileName = record.FilePath, UseShellExecute = true });
                }
            }
            catch { }
        }
    }

    private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DatabaseExportLog record)
        {
            try
            {
                var dir = record.FileDirectory;
                if (string.IsNullOrWhiteSpace(dir) == false && Directory.Exists(dir))
                {
                    Process.Start(new ProcessStartInfo { FileName = dir, UseShellExecute = true });
                }
            }
            catch { }
        }
    }
}
