using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 登录ViewModel
/// </summary>
public class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly IConfigService _configService;
    private readonly IArasConnectionService _connectionService;

    private string _url = string.Empty;
    private string _database = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe;
    private bool _isLoggingIn;
    private string _errorMessage = string.Empty;
    private string _statusMessage = string.Empty;
    private ObservableCollection<LoginInfo> _savedLogins = new();

    public LoginViewModel(ILoginService loginService, IConfigService configService, IArasConnectionService connectionService)
    {
        _loginService = loginService;
        _configService = configService;
        _connectionService = connectionService;

        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
        SelectSavedLoginCommand = new RelayCommand(SelectSavedLogin);
        DeleteSavedLoginCommand = new RelayCommand(async p => await DeleteSavedLoginAsync(p?.ToString() ?? ""));

        // 启动时加载已保存的登录信息，并自动填充上次登录
        _ = InitializeAsync();
    }

    public string Url
    {
        get => _url;
        set { SetProperty(ref _url, value); (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public string Database
    {
        get => _database;
        set { SetProperty(ref _database, value); (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public string Username
    {
        get => _username;
        set { SetProperty(ref _username, value); (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public string Password
    {
        get => _password;
        set { SetProperty(ref _password, value); (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set => SetProperty(ref _isLoggingIn, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ObservableCollection<LoginInfo> SavedLogins
    {
        get => _savedLogins;
        set
        {
            SetProperty(ref _savedLogins, value);
            OnPropertyChanged(nameof(HasSavedLogins));
        }
    }

    /// <summary>
    /// 是否有已保存的登录记录
    /// </summary>
    public bool HasSavedLogins => _savedLogins.Count > 0;

    public ICommand LoginCommand { get; }
    public ICommand SelectSavedLoginCommand { get; }
    public ICommand DeleteSavedLoginCommand { get; }

    /// <summary>
    /// 密码框更新回调（由View调用，用于同步PasswordBox）
    /// </summary>
    public Action<string>? PasswordBoxUpdater { get; set; }

    public event Action? LoginSucceeded;

    /// <summary>
    /// 初始化：加载已保存记录 + 自动填充上次登录
    /// </summary>
    private async Task InitializeAsync()
    {
        await LoadSavedLoginsAsync();

        // 自动加载上次登录信息
        var lastLogin = await _configService.LoadLastLoginInfoAsync();
        if (lastLogin != null)
        {
            Url = lastLogin.Url;
            Database = lastLogin.Database;
            Username = lastLogin.Username;
            Password = lastLogin.Password;
            RememberMe = lastLogin.RememberMe;
            PasswordBoxUpdater?.Invoke(lastLogin.Password);
            StatusMessage = "已自动加载上次登录信息";
        }
    }

    private bool CanLogin()
    {
        return !IsLoggingIn
            && !string.IsNullOrWhiteSpace(Url)
            && !string.IsNullOrWhiteSpace(Database)
            && !string.IsNullOrWhiteSpace(Username)
            && !string.IsNullOrWhiteSpace(Password);
    }

    private async Task LoginAsync()
    {
        IsLoggingIn = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在登录...";

        try
        {
            var loginInfo = new LoginInfo
            {
                Url = Url.Trim(),
                Database = Database.Trim(),
                Username = Username.Trim(),
                Password = Password,
                RememberMe = RememberMe
            };

            var result = await _loginService.LoginAsync(loginInfo);

            if (result != null)
            {
                // 如果选择记住密码，保存登录信息（去重处理）
                if (RememberMe)
                {
                    await SaveLoginWithDuplicateCheckAsync(loginInfo);
                }

                StatusMessage = "登录成功！";
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = "登录失败：无法连接到服务器，请检查地址和凭证。";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"CreateHttpServerConnection: {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    /// <summary>
    /// 保存登录信息，自动去重：相同Url+Database+Username不重复记录
    /// </summary>
    private async Task SaveLoginWithDuplicateCheckAsync(LoginInfo loginInfo)
    {
        var isDuplicate = await _configService.IsDuplicateLoginInfoAsync(loginInfo);

        if (isDuplicate)
        {
            // 已存在相同记录，仅更新密码和RememberMe标记
            StatusMessage = "该登录信息已存在，已更新密码";
            // 找到已有记录并更新
            var allLogins = await _configService.LoadAllLoginInfosAsync();
            var existing = allLogins.FirstOrDefault(l =>
                l.Url.Equals(loginInfo.Url, StringComparison.OrdinalIgnoreCase) &&
                l.Database.Equals(loginInfo.Database, StringComparison.OrdinalIgnoreCase) &&
                l.Username.Equals(loginInfo.Username, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Password = loginInfo.Password;
                existing.RememberMe = true;
                await _configService.DeleteLoginInfoAsync(existing.ConfigName ?? "");
                await _configService.SaveLoginInfoAsync(existing);
                await _configService.SaveLastLoginConfigAsync(existing.ConfigName ?? "");
            }
        }
        else
        {
            // 新增记录
            loginInfo.ConfigName = $"login_{DateTime.Now:yyyyMMddHHmmss}";
            await _configService.SaveLoginInfoAsync(loginInfo);
            await _configService.SaveLastLoginConfigAsync(loginInfo.ConfigName);
        }

        await LoadSavedLoginsAsync();
    }

    private async Task LoadSavedLoginsAsync()
    {
        try
        {
            var logins = await _configService.LoadAllLoginInfosAsync();
            SavedLogins = new ObservableCollection<LoginInfo>(logins);
        }
        catch
        {
            SavedLogins = new ObservableCollection<LoginInfo>();
        }
    }

    private void SelectSavedLogin(object? parameter)
    {
        if (parameter is LoginInfo savedLogin)
        {
            Url = savedLogin.Url;
            Database = savedLogin.Database;
            Username = savedLogin.Username;
            Password = savedLogin.Password;
            RememberMe = true;
            PasswordBoxUpdater?.Invoke(savedLogin.Password);
            StatusMessage = $"已选择: {savedLogin.Username}@{savedLogin.Database}";
        }
    }

    private async Task DeleteSavedLoginAsync(string configName)
    {
        if (string.IsNullOrEmpty(configName)) return;
        await _configService.DeleteLoginInfoAsync(configName);
        await LoadSavedLoginsAsync();
        StatusMessage = "已删除登录配置";
    }
}
