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

    /// <summary>数据库记录ID（DB存储后使用，新建时为空）</summary>
    public string? Id { get; set; }

    /// <summary>是否启用（启用后应用启动时自动连接）</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Password 是否已经是32位小写MD5哈希（DB加载时为true，用户输入时为false）</summary>
    public bool IsPasswordHashed { get; set; }
}
