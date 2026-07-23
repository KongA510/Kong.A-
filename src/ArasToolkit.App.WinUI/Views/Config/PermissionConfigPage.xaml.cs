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
/// 权限配置页（WinUI 3）— 模板下载/文件导入/进度/历史表格。
/// </summary>
public sealed partial class PermissionConfigPage : Page
{
    public PermissionConfigPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<PermissionConfigViewModel>();
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
        if (sender is Button { Tag: PermissionImportLog record })
            OpenWithShell(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, record.ImportFile));
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string logPath } && string.IsNullOrWhiteSpace(logPath) == false)
            OpenWithShell(logPath);
    }
}
