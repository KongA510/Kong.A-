namespace ArasToolkit.Core.Models;

/// <summary>
/// Aras连接信息（持久化使用）
/// </summary>
public class ArasConnectionInfo
{
    public string Url { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? LoginTime { get; set; }
}
