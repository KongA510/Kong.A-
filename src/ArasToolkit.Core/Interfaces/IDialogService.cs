namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 对话框服务抽象 — 解耦 ViewModel 与具体 UI 框架（WPF MessageBox / WinUI ContentDialog）。
/// </summary>
public interface IDialogService
{
    /// <summary>弹出仅含"确定"的提示框（替代 MessageBox 告知场景）。</summary>
    Task AlertAsync(string title, string message);

    /// <summary>弹出确认框，返回用户是否确认（替代 MessageBox Yes/No）。</summary>
    Task<bool> ConfirmAsync(string title, string message, string confirmText = "确定", string cancelText = "取消");

    /// <summary>弹出输入框，返回用户输入（取消返回 null）。替代 TextPromptWindow。</summary>
    Task<string?> PromptAsync(string title, string placeholder = "", string defaultValue = "");
}
