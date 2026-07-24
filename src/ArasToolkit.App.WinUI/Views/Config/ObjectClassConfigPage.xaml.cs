using System;
using System.Diagnostics;
using System.IO;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 对象类配置页（WinUI 3）— 模板下载/文件导入/进度/历史表格。
/// </summary>
public sealed partial class ObjectClassConfigPage : Page
{
    public ObjectClassConfigPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<ObjectClassConfigViewModel>();
    }

    private static void OpenWithShell(string path)
    {
        try
        {
            if (File.Exists(path))
                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
        catch
        {
            // 打开文件失败静默处理
        }
    }

    private void OpenImportFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: ObjectClassImportLog record })
            OpenWithShell(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, record.ImportFile));
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string logPath } && !string.IsNullOrWhiteSpace(logPath))
            OpenWithShell(logPath);
    }
}
