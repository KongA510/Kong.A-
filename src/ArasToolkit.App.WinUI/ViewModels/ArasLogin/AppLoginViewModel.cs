using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 应用登录 ViewModel — 一级登录（本地应用账号）。逻辑与 WPF 版一致，UI 无关。
/// </summary>
public class AppLoginViewModel : ObservableObject
{
    private readonly IAppUserService _appUserService;
    private readonly IConfigService _configService;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _displayName = string.Empty;
    private bool _isRegisterMode;
    private bool _isProcessing;
    private bool _rememberMe;
    private string _errorMessage = string.Empty;
    private string _statusMessage = string.Empty;

    public AppLoginViewModel(IAppUserService appUserService, IConfigService configService)
    {
        _appUserService = appUserService;
        _configService = configService;

        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanSubmit());
        RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => CanSubmit());
        ToggleModeCommand = new RelayCommand(ToggleMode);

        _ = InitializeAsync();
    }

    public string Username
    {
        get => _username;
        set { SetProperty(ref _username, value); RefreshCommands(); }
    }

    public string Password
    {
        get => _password;
        set { SetProperty(ref _password, value); RefreshCommands(); }
    }

    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    public bool IsRegisterMode
    {
        get => _isRegisterMode;
        set
        {
            SetProperty(ref _isRegisterMode, value);
            OnPropertyChanged(nameof(IsLoginMode));
            OnPropertyChanged(nameof(ToggleButtonText));
        }
    }

    public bool IsLoginMode => !_isRegisterMode;

    public string ToggleButtonText => _isRegisterMode ? "切换到登录" : "注册新账号";

    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
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

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand ToggleModeCommand { get; }

    /// <summary>登录成功事件 — MainWindow 监听后切换主界面。</summary>
    public event Action? LoginSucceeded;

    private async Task InitializeAsync()
    {
        var saved = await _configService.LoadAppSettingAsync<AppLoginCredential>("appLogin");
        if (saved != null && saved.RememberMe)
        {
            Username = saved.Username;
            Password = saved.Password;
            RememberMe = true;
        }
    }

    private bool CanSubmit()
    {
        return !IsProcessing
            && !string.IsNullOrWhiteSpace(Username)
            && !string.IsNullOrWhiteSpace(Password);
    }

    private void RefreshCommands()
    {
        (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private async Task LoginAsync()
    {
        IsProcessing = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在登录...";
        try
        {
            var user = await _appUserService.LoginAsync(Username.Trim(), Password);
            if (user != null)
            {
                CurrentUserContext.Current = new AppUserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    IsAdmin = user.IsAdmin
                };
                if (RememberMe)
                {
                    await _configService.SaveAppSettingAsync("appLogin", new AppLoginCredential
                    {
                        Username = Username.Trim(),
                        Password = Password,
                        RememberMe = true
                    });
                }
                else
                {
                    await _configService.SaveAppSettingAsync<AppLoginCredential?>("appLogin", null);
                }
                StatusMessage = $"欢迎回来，{user.DisplayName ?? user.Username}";
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = "账号或密码错误，请重试。";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"登录失败: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task RegisterAsync()
    {
        IsProcessing = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在注册...";
        try
        {
            var exists = await _appUserService.UserExistsAsync(Username.Trim());
            if (exists)
            {
                ErrorMessage = "该账号已被注册，请换一个用户名。";
                return;
            }
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                ErrorMessage = "请填写显示名称。";
                return;
            }
            var user = await _appUserService.RegisterAsync(Username.Trim(), Password, DisplayName.Trim());
            CurrentUserContext.Current = new AppUserInfo
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };
            StatusMessage = $"注册成功，欢迎 {user.DisplayName ?? user.Username}";
            LoginSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"注册失败: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private void ToggleMode()
    {
        IsRegisterMode = !IsRegisterMode;
        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;
    }
}
