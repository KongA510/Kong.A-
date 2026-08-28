using System.Windows;
using ArasToolkit.App.Views;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.Services;

/// <summary>WPF 对话框适配器，使共享 ViewModel 不直接依赖 MessageBox。</summary>
public sealed class DialogService : IDialogService
{
    public Task AlertAsync(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    public Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText = "确定",
        string cancelText = "取消")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
        return Task.FromResult(result == MessageBoxResult.Yes);
    }

    public Task<string?> PromptAsync(string title, string placeholder = "", string defaultValue = "")
    {
        var dialog = new TextPromptWindow(title, placeholder)
        {
            Owner = Application.Current.MainWindow
        };
        return Task.FromResult(dialog.ShowDialog() == true ? dialog.InputText : null);
    }
}
