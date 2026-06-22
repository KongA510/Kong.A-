namespace ArasToolkit.Core.Models;

/// <summary>
/// 登录信息模型
/// </summary>
public class LoginInfo
{
    public string Url { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    
    /// <summary>
    /// 保存的登录配置名称（用于标识不同配置）
    /// </summary>
    public string? ConfigName { get; set; }
}
