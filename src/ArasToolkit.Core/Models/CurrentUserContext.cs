namespace ArasToolkit.Core.Models;

/// <summary>
/// 当前应用用户信息
/// </summary>
public class AppUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsAdmin { get; set; }
}

/// <summary>
/// 全局当前用户上下文 — 应用登录成功后设置，后续所有日志/操作从此读取
/// </summary>
public static class CurrentUserContext
{
    /// <summary>当前登录用户信息</summary>
    public static AppUserInfo? Current { get; set; }

    /// <summary>当前用户名（用于日志写入）</summary>
    public static string CurrentUserName => Current?.Username ?? "未知用户";
    public static string CurrentUserId => Current?.Id ?? "unknown";

    /// <summary>是否为管理员</summary>
    public static bool IsAdmin => Current?.IsAdmin ?? false;
}
