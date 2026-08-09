using ArasToolkit.Core.Interfaces;
using Microsoft.Win32;

namespace ArasToolkit.App.Services;

/// <summary>WPF 文件选择器适配器，与 WinUI 文件服务实现同一接口。</summary>
public sealed class FileDialogService : IFileDialogService
{
    public Task<string?> PickOpenFileAsync(string title = "", params string[] extensions)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            Filter = BuildFilter(extensions),
            Multiselect = false
        };
        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
    }

    public Task<IReadOnlyList<string>> PickOpenFilesAsync(string title = "", params string[] extensions)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            Filter = BuildFilter(extensions),
            Multiselect = true
        };
        IReadOnlyList<string> result = dialog.ShowDialog() == true ? dialog.FileNames : [];
        return Task.FromResult(result);
    }

    public Task<string?> PickSaveFileAsync(string defaultFileName = "", params string[] extensions)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultFileName,
            Filter = BuildFilter(extensions)
        };
        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
    }

    public Task<string?> PickFolderAsync(string title = "")
    {
        var dialog = new OpenFolderDialog { Title = title };
        return Task.FromResult(dialog.ShowDialog() == true ? dialog.FolderName : null);
    }

    private static string BuildFilter(IReadOnlyCollection<string>? extensions)
    {
        if (extensions == null || extensions.Count == 0)
            return "所有文件|*.*";
        var patterns = extensions.Select(extension => "*" + (extension.StartsWith('.') ? extension : "." + extension));
        var filter = string.Join(';', patterns);
        return $"支持的文件|{filter}|所有文件|*.*";
    }
}
