namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 文件对话框服务抽象 — 解耦 ViewModel 与 Win32 OpenFileDialog/SaveFileDialog。
/// </summary>
public interface IFileDialogService
{
    /// <summary>选择单个文件，返回完整路径（取消返回 null）。extensions 形如 ".xlsx" ".csv"。</summary>
    Task<string?> PickOpenFileAsync(string title = "", params string[] extensions);

    /// <summary>选择保存路径，返回完整路径（取消返回 null）。</summary>
    Task<string?> PickSaveFileAsync(string defaultFileName = "", params string[] extensions);

    /// <summary>选择文件夹，返回完整路径（取消返回 null）。</summary>
    Task<string?> PickFolderAsync(string title = "");
}
