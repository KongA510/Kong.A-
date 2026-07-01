using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 登录服务接口
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// 使用用户名密码登录Aras
    /// </summary>
    Task<ArasConnectionInfo?> LoginAsync(LoginInfo loginInfo);
    
    /// <summary>
    /// 验证连接是否有效
    /// </summary>
    Task<bool> ValidateConnectionAsync(ArasConnectionInfo connectionInfo);
    
    /// <summary>
    /// 退出登录
    /// </summary>
    void Logout();
}
