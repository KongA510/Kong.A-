using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Aras连接管理服务 — 全局单例，持有真实的 HttpServerConnection 和 Innovator
/// </summary>
public class ArasConnectionService : IArasConnectionService
{
    private ArasConnectionInfo? _currentConnection;
    private Innovator? _innovator;
    private HttpServerConnection? _httpConnection;

    public bool IsConnected => _currentConnection != null && _httpConnection != null;
    public ArasConnectionInfo? CurrentConnection => _currentConnection;
    public object? InnovatorInstance => _innovator;
    public object? HttpConnection => _httpConnection;

    /// <summary>
    /// 登录成功后保存连接
    /// </summary>
    public void SetConnection(ArasConnectionInfo connectionInfo, object innovator, object httpConnection)
    {
        _currentConnection = connectionInfo;
        _currentConnection.LoginTime = DateTime.Now;
        _innovator = innovator as Innovator;
        _httpConnection = httpConnection as HttpServerConnection;
    }

    /// <summary>
    /// 断开连接（调用 HttpServerConnection.Logout）
    /// </summary>
    public void Disconnect()
    {
        try
        {
            _httpConnection?.Logout();
        }
        catch
        {
            // 静默处理注销异常
        }
        _innovator = null;
        _httpConnection = null;
        _currentConnection = null;
    }
}
