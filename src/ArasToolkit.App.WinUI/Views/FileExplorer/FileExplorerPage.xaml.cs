using System.Collections.Specialized;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 我的资料页（WinUI 3）— 文件夹导航 + 模糊搜索 + 文件上传 + 双击打开。
/// </summary>
public sealed partial class FileExplorerPage : Page
{
    public FileExplorerPage()
    {
        this.InitializeComponent();
        var vm = App.Services.GetRequiredService<FileExplorerViewModel>();
        DataContext = vm;
        vm.CurrentItems.CollectionChanged += OnItemsChanged;
        UpdateEmptyLabel();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => UpdateEmptyLabel();

    private void UpdateEmptyLabel()
    {
        if (DataContext is FileExplorerViewModel vm)
            EmptyLabel.Visibility = vm.CurrentItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>双击进入文件夹 / 打开文件。</summary>
    private void Item_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: FileSystemItem item } && DataContext is FileExplorerViewModel vm)
        {
            if (item.IsDirectory)
                vm.NavigateIntoCommand.Execute(item);
            else
                vm.OpenFileCommand.Execute(item);
        }
    }

    /// <summary>📂 按钮 — 在资源管理器中定位。</summary>
    private void OpenInExplorerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: FileSystemItem item } && DataContext is FileExplorerViewModel vm)
            vm.OpenInExplorerCommand.Execute(item);
    }
}
