using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.Views;

public partial class FileExplorerView : UserControl
{
    public FileExplorerView()
    {
        InitializeComponent();
    }

    /// <summary>双击进入文件夹 / 打开文件（保留原有交互）</summary>
    private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 2) return;
        if (sender is not Border border) return;
        if (border.Tag is not FileSystemItem item) return;
        if (DataContext is not ViewModels.FileExplorerViewModel vm) return;

        if (item.IsDirectory)
            vm.NavigateIntoCommand.Execute(item);
        else
            vm.OpenFileCommand.Execute(item);

        e.Handled = true;
    }

    /// <summary>点击 📂 按钮 — 在资源管理器中定位（文件夹打开/文件选中）</summary>
    private void OpenInExplorerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.Tag is not FileSystemItem item) return;
        if (DataContext is not ViewModels.FileExplorerViewModel vm) return;

        vm.OpenInExplorerCommand.Execute(item);
        e.Handled = true;
    }
}
