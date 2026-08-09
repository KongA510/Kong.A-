using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using ArasToolkit.App.ViewModels;

namespace ArasToolkit.App.Views;

/// <summary>
/// PropertyConfigView.xaml 的交互逻辑
/// </summary>
public partial class PropertyConfigView : UserControl
{
    public PropertyConfigView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PropertyConfigView] 初始化失败: {ex.Message}");
            throw;
        }
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PropertyConfigViewModel viewModel)
            await viewModel.InitializeAsync();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PropertyConfigViewModel viewModel)
            viewModel.Dispose();
    }

    private void ShowAml_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: PropertyImportPreviewRow row } || !row.HasAmlPreview)
            return;

        var textBox = new TextBox
        {
            Text = row.AmlPreview,
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new System.Windows.Media.FontFamily("Cascadia Mono"),
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        var window = new Window
        {
            Title = $"第 {row.ExcelRowNumber} 行 · {row.Name} · {row.PlannedAction}",
            Content = textBox,
            Width = 820,
            Height = 480,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Window.GetWindow(this)
        };
        window.ShowDialog();
    }

    /// <summary>
    /// 打开导入文件 — 从历史记录 DataGrid 中点击"打开"按钮
    /// </summary>
    private void OpenImportFile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is PropertyImportLog record)
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
                // 静默处理
            }
        }
    }

    /// <summary>
    /// 打开导入日志文件 — 从结果卡片中点击"查看导入日志"按钮
    /// </summary>
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
                // 静默处理
            }
        }
    }
}
