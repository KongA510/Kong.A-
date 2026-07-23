using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.WinUI.Services;

/// <summary>
/// 文件对话框服务（WinUI Picker 实现）。通过 InitializeWithWindow 绑定主窗口 HWND。
/// </summary>
public class FileDialogService : IFileDialogService
{
    private static IntPtr GetHwnd() =>
        App.MainWindow != null
            ? WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow)
            : IntPtr.Zero;

    public async Task<string?> PickOpenFileAsync(string title = "", params string[] extensions)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop,
            ViewMode = PickerViewMode.List
        };
        WinRT.Interop.InitializeWithWindow.Initialize(picker, GetHwnd());
        if (extensions == null || extensions.Length == 0)
            picker.FileTypeFilter.Add("*");
        else
            foreach (var ext in extensions)
                picker.FileTypeFilter.Add(ext.StartsWith(".") ? ext : "." + ext);
        var file = await picker.PickSingleFileAsync();
        return file?.Path;
    }

    public async Task<string?> PickSaveFileAsync(string defaultFileName = "", params string[] extensions)
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop
        };
        WinRT.Interop.InitializeWithWindow.Initialize(picker, GetHwnd());
        if (!string.IsNullOrEmpty(defaultFileName))
            picker.SuggestedFileName = defaultFileName;
        var exts = (extensions == null || extensions.Length == 0) ? new[] { ".xlsx" } : extensions;
        picker.FileTypeChoices.Add("文件", exts.Select(e => e.StartsWith(".") ? e : "." + e).ToList());
        var file = await picker.PickSaveFileAsync();
        return file?.Path;
    }

    public async Task<string?> PickFolderAsync(string title = "")
    {
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop
        };
        WinRT.Interop.InitializeWithWindow.Initialize(picker, GetHwnd());
        picker.FileTypeFilter.Add("*");
        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }
}
