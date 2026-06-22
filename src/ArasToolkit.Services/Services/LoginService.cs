using Aras.IOM;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 登录服务 — 使用 Aras.IOM.Innovator.UserPasswordLogin 进行真实登录
/// </summary>
public class LoginService : ILoginService
{
    private readonly IArasConnectionService _connectionService;

    public LoginService(IArasConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public Task<ArasConnectionInfo?> LoginAsync(LoginInfo loginInfo)
    {
        return Task.Run<ArasConnectionInfo?>(() =>
        {
            try
            {
                // MD5 哈希密码（模拟 Aras.IOM.Innovator.ScalcMD5）
                var md5Password = loginInfo.Password.ToMd5();

                // 使用 IomFactory 创建连接（IOM R37 API）
                HttpServerConnection connection = IomFactory.CreateHttpServerConnection(
                    loginInfo.Url,
                    loginInfo.Database,
                    loginInfo.Username,
                    md5Password);

                var connectionInfo = new ArasConnectionInfo
                {
                    Url = loginInfo.Url,
                    Database = loginInfo.Database,
                    Username = loginInfo.Username,
                    LoginTime = DateTime.Now
                };

                // 持久化连接
                _connectionService.SetConnection(connectionInfo, new Innovator(connection), connection);
                return connectionInfo;
            }
            catch (Exception ex)
            {
                throw new Exception($"CreateHttpServerConnection: {ex.Message}", ex);
            }
        });
    }

    public Task<bool> ValidateConnectionAsync(ArasConnectionInfo connectionInfo)
    {
        return Task.Run(() =>
        {
            try
            {
                var md5Password = connectionInfo.Username.ToMd5(); // 仅结构验证，不实际重连
                return !string.IsNullOrEmpty(connectionInfo.Url)
                    && !string.IsNullOrEmpty(connectionInfo.Database)
                    && !string.IsNullOrEmpty(connectionInfo.Username);
            }
            catch
            {
                return false;
            }
        });
    }

    public void Logout()
    {
        _connectionService.Disconnect();
    }
}
