using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.Views;

public partial class DatabaseExportView : UserControl
{
    public DatabaseExportView() { try { InitializeComponent(); } catch (Exception ex) { Debug.WriteLine($"[DatabaseExportView] {ex.Message}"); throw; } }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string logPath && !string.IsNullOrWhiteSpace(logPath))
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
                if (!string.IsNullOrWhiteSpace(record.FilePath) && File.Exists(record.FilePath))
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
                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                {
                    Process.Start(new ProcessStartInfo { FileName = dir, UseShellExecute = true });
                }
            }
            catch { }
        }
    }
}
