namespace ArasToolkit.Core.Models;

/// <summary>
/// 应用登录凭据 — 用于记住密码功能
/// </summary>
public class AppLoginCredential
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
