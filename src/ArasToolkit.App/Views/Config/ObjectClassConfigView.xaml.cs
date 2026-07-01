using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.Views;

/// <summary>
/// ObjectClassConfigView.xaml 的交互逻辑
/// </summary>
public partial class ObjectClassConfigView : UserControl
{
    public ObjectClassConfigView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ObjectClassConfigView] 初始化失败: {ex.Message}");
            throw;
        }
    }

    private void OpenImportFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ObjectClassImportLog record)
        {
            try
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, record.ImportFile);
                if (File.Exists(fullPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    });
                }
            }
            catch
            {
                // 打开文件失败静默处理
            }
        }
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string logPath && !string.IsNullOrWhiteSpace(logPath))
        {
            try
            {
                if (File.Exists(logPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = logPath,
                        UseShellExecute = true
                    });
                }
            }
            catch
            {
                // 打开日志失败静默处理
            }
        }
    }
}
