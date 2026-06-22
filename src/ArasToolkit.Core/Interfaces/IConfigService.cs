using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 配置服务接口 - 管理应用配置和登录信息持久化
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// 保存登录信息到本地文件（自动去重）
    /// </summary>
    Task SaveLoginInfoAsync(LoginInfo loginInfo);
    
    /// <summary>
    /// 加载所有已保存的登录配置
    /// </summary>
    Task<List<LoginInfo>> LoadAllLoginInfosAsync();
    
    /// <summary>
    /// 删除指定登录配置
    /// </summary>
    Task DeleteLoginInfoAsync(string configName);
    
    /// <summary>
    /// 保存应用设置
    /// </summary>
    Task SaveAppSettingAsync<T>(string key, T value);
    
    /// <summary>
    /// 读取应用设置
    /// </summary>
    Task<T?> LoadAppSettingAsync<T>(string key);

    /// <summary>
    /// 检查登录信息是否已存在（按Url+Database+Username去重）
    /// </summary>
    Task<bool> IsDuplicateLoginInfoAsync(LoginInfo loginInfo);

    /// <summary>
    /// 加载最近一次使用的登录信息
    /// </summary>
    Task<LoginInfo?> LoadLastLoginInfoAsync();

    /// <summary>
    /// 保存最后使用的登录配置名称
    /// </summary>
    Task SaveLastLoginConfigAsync(string configName);

    /// <summary>
    /// 获取最后使用的登录配置名称
    /// </summary>
    Task<string?> GetLastLoginConfigNameAsync();
}
