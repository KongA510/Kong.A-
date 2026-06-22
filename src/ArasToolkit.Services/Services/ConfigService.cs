using System.Text.Json;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 配置服务实现 - 使用JSON文件持久化配置
/// </summary>
public class ConfigService : IConfigService
{
    private readonly string _configRootPath;
    private readonly string _loginConfigPath;

    public ConfigService()
    {
        _configRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        _loginConfigPath = Path.Combine(_configRootPath, "Login");
        
        // 确保配置目录存在
        Directory.CreateDirectory(_loginConfigPath);
        Directory.CreateDirectory(Path.Combine(_configRootPath, "AppSettings"));
    }

    public async Task SaveLoginInfoAsync(LoginInfo loginInfo)
    {
        var configName = string.IsNullOrEmpty(loginInfo.ConfigName) 
            ? $"login_{DateTime.Now:yyyyMMddHHmmss}" 
            : loginInfo.ConfigName;
        
        loginInfo.ConfigName = configName;
        
        // 保存时不包含明文密码，进行Base64编码
        var saveInfo = new
        {
            loginInfo.Url,
            loginInfo.Database,
            loginInfo.Username,
            Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(loginInfo.Password)),
            loginInfo.RememberMe,
            loginInfo.ConfigName
        };
        
        var filePath = Path.Combine(_loginConfigPath, $"{configName}.json");
        var json = JsonSerializer.Serialize(saveInfo, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public Task<List<LoginInfo>> LoadAllLoginInfosAsync()
    {
        var result = new List<LoginInfo>();
        
        if (!Directory.Exists(_loginConfigPath))
            return Task.FromResult(result);

        var files = Directory.GetFiles(_loginConfigPath, "*.json");
        
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                
                var loginInfo = new LoginInfo
                {
                    Url = data.GetProperty("Url").GetString() ?? "",
                    Database = data.GetProperty("Database").GetString() ?? "",
                    Username = data.GetProperty("Username").GetString() ?? "",
                    Password = DecodePassword(data.GetProperty("Password").GetString() ?? ""),
                    RememberMe = data.GetProperty("RememberMe").GetBoolean(),
                    ConfigName = data.GetProperty("ConfigName").GetString() ?? ""
                };
                result.Add(loginInfo);
            }
            catch
            {
                // 忽略无法解析的配置文件
            }
        }
        
        return Task.FromResult(result);
    }

    public Task DeleteLoginInfoAsync(string configName)
    {
        var filePath = Path.Combine(_loginConfigPath, $"{configName}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task SaveAppSettingAsync<T>(string key, T value)
    {
        var settingsPath = Path.Combine(_configRootPath, "AppSettings", $"{key}.json");
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(settingsPath, json);
    }

    public Task<T?> LoadAppSettingAsync<T>(string key)
    {
        var settingsPath = Path.Combine(_configRootPath, "AppSettings", $"{key}.json");
        if (!File.Exists(settingsPath))
            return Task.FromResult<T?>(default);
        
        var json = File.ReadAllText(settingsPath);
        var value = JsonSerializer.Deserialize<T>(json);
        return Task.FromResult(value);
    }

    private static string DecodePassword(string encoded)
    {
        try
        {
            var bytes = Convert.FromBase64String(encoded);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return encoded;
        }
    }

    public async Task<bool> IsDuplicateLoginInfoAsync(LoginInfo loginInfo)
    {
        var all = await LoadAllLoginInfosAsync();
        return all.Any(l =>
            l.Url.Equals(loginInfo.Url, StringComparison.OrdinalIgnoreCase) &&
            l.Database.Equals(loginInfo.Database, StringComparison.OrdinalIgnoreCase) &&
            l.Username.Equals(loginInfo.Username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<LoginInfo?> LoadLastLoginInfoAsync()
    {
        var lastConfigName = await GetLastLoginConfigNameAsync();
        if (string.IsNullOrEmpty(lastConfigName))
            return null;

        var all = await LoadAllLoginInfosAsync();
        return all.FirstOrDefault(l => l.ConfigName == lastConfigName);
    }

    public async Task SaveLastLoginConfigAsync(string configName)
    {
        var lastLoginPath = Path.Combine(_configRootPath, "AppSettings", "lastLogin.json");
        var json = JsonSerializer.Serialize(new { LastConfigName = configName }, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(lastLoginPath, json);
    }

    public Task<string?> GetLastLoginConfigNameAsync()
    {
        var lastLoginPath = Path.Combine(_configRootPath, "AppSettings", "lastLogin.json");
        if (!File.Exists(lastLoginPath))
            return Task.FromResult<string?>(null);

        try
        {
            var json = File.ReadAllText(lastLoginPath);
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            return Task.FromResult<string?>(data.GetProperty("LastConfigName").GetString());
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }
}
