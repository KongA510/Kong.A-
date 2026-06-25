namespace ArasToolkit.Core.Models;

/// <summary>
/// Aras连接信息（持久化使用）
/// </summary>
public class ArasConnectionInfo
{
    public string Url { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
   public string Username { get; set; } = string.Empty;
   /// <summary>MD5加密密码（32位小写十六进制），供连接池初始化使用</summary>
   public string Md5Password { get; set; } = string.Empty;
   public string? Token { get; set; }
    public DateTime? LoginTime { get; set; }
}
