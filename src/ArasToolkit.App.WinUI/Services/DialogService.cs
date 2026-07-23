using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.WinUI.Services;

/// <summary>
/// 对话框服务（WinUI ContentDialog 实现）。XamlRoot 取自全局主窗口。
/// </summary>
public class DialogService : IDialogService
{
    private static XamlRoot? GetXamlRoot() =>
        (App.MainWindow?.Content as FrameworkElement)?.XamlRoot;

    public async Task AlertAsync(string title, string message)
    {
        var xamlRoot = GetXamlRoot();
        if (xamlRoot == null) return;
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "确定",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = xamlRoot
        };
        await dialog.ShowAsync();
    }

    public async Task<bool> ConfirmAsync(string title, string message, string confirmText = "确定", string cancelText = "取消")
    {
        var xamlRoot = GetXamlRoot();
        if (xamlRoot == null) return false;
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = confirmText,
            CloseButtonText = cancelText,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = xamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<string?> PromptAsync(string title, string placeholder = "", string defaultValue = "")
    {
        var xamlRoot = GetXamlRoot();
        if (xamlRoot == null) return null;
        var textBox = new TextBox { PlaceholderText = placeholder, Text = defaultValue, MinWidth = 320 };
        var dialog = new ContentDialog
        {
            Title = title,
            Content = textBox,
            PrimaryButtonText = "确定",
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = xamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? textBox.Text : null;
    }
}
