using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Aras连接管理服务接口 - 管理HttpServerConnection + Innovator 的持久化
/// </summary>
public interface IArasConnectionService
{
    /// <summary>当前是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>获取当前连接信息</summary>
    ArasConnectionInfo? CurrentConnection { get; }

    /// <summary>获取 Aras Innovator 实例（用于 API 调用）</summary>
    object? InnovatorInstance { get; }

    /// <summary>获取 HttpServerConnection 实例（用于底层操作如Logout）</summary>
    object? HttpConnection { get; }

    /// <summary>登录成功后保存连接</summary>
    void SetConnection(ArasConnectionInfo connectionInfo, object innovator, object httpConnection);

    /// <summary>断开连接</summary>
    void Disconnect();
}
