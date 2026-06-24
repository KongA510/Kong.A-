using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

public class ArasLoginViewModel : ObservableObject
{
    private readonly IConfigService _configService;
    private readonly ILoginService _loginService;
    private readonly IArasConnectionService _connectionService;
    private readonly IErrorLogService _errorLogService;

    private ObservableCollection<LoginInfo> _loginInfos = [];
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isProcessing;

    public ArasLoginViewModel(
        IConfigService configService,
        ILoginService loginService,
        IArasConnectionService connectionService,
        IErrorLogService errorLogService)
    {
        _configService = configService;
        _loginService = loginService;
        _connectionService = connectionService;
        _errorLogService = errorLogService;

        ConnectCommand = new RelayCommand(async p => await ConnectAsync(p), _ => !IsProcessing);
        DeleteCommand = new RelayCommand(async p => await DeleteAsync(p), _ => !IsProcessing);
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());

        _ = LoadAsync();
    }

    public ObservableCollection<LoginInfo> LoginInfos
    {
        get => _loginInfos;
        set => SetProperty(ref _loginInfos, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set { SetProperty(ref _isProcessing, value); RefreshCommands(); }
    }

    public ICommand ConnectCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    private void RefreshCommands()
    {
        (ConnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private async Task LoadAsync()
    {
        try
        {
            var logins = await _configService.LoadAllLoginInfosAsync();
            LoginInfos = new ObservableCollection<LoginInfo>(logins);
            StatusMessage = "已加载 " + logins.Count + " 条连接记录";
        }
        catch (Exception ex)
        {
            ErrorMessage = "加载失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("Aras登录-加载列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task ConnectAsync(object? parameter)
    {
        if (parameter is not LoginInfo info) return;

        IsProcessing = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在连接到 " + info.Username + "@" + info.Database + "...";

        try
        {
            await _loginService.LoginAsync(info);
            StatusMessage = "已切换到 " + info.Username + "@" + info.Database;
        }
        catch (Exception ex)
        {
            ErrorMessage = "连接失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("Aras登录-切换连接", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not string configName || string.IsNullOrEmpty(configName)) return;

        IsProcessing = true;
        try
        {
            await _configService.DeleteLoginInfoAsync(configName);
            StatusMessage = "已删除连接配置";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "删除失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("Aras登录-删除配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
