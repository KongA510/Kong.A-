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
    private readonly IArasConnectionPool _connectionPool;
    private readonly IArasLoginConfigService _loginConfigService;
    private readonly IErrorLogService _errorLogService;

    private ObservableCollection<LoginInfo> _loginInfos = [];
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isProcessing;

    public ArasLoginViewModel(
        IConfigService configService,
        ILoginService loginService,
        IArasConnectionService connectionService,
        IArasConnectionPool connectionPool,
        IArasLoginConfigService loginConfigService,
        IErrorLogService errorLogService)
    {
        _configService = configService;
        _loginService = loginService;
        _connectionService = connectionService;
        _connectionPool = connectionPool;
        _loginConfigService = loginConfigService;
        _errorLogService = errorLogService;

        ConnectCommand = new RelayCommand(async p => await ConnectAsync(p), _ => !IsProcessing);
        DeleteCommand = new RelayCommand(async p => await DeleteAsync(p), _ => !IsProcessing);
        EditCommand = new RelayCommand(async p => await EditAsync(p), _ => !IsProcessing);
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => !IsProcessing && !string.IsNullOrEmpty(FormUrl));
        ToggleFormCommand = new RelayCommand(_ => ToggleForm());
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());
        CancelEditCommand = new RelayCommand(_ => CloseForm());
        CancelCommand = new RelayCommand(_ => CancelTranslate());

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
    public ICommand EditCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ToggleFormCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand CancelCommand { get; }

    // ---- 表单属性 ----

    private string _formUrl = string.Empty;
    public string FormUrl { get => _formUrl; set { SetProperty(ref _formUrl, value); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _formDatabase = string.Empty;
    public string FormDatabase { get => _formDatabase; set => SetProperty(ref _formDatabase, value); }

    private string _formUsername = string.Empty;
    public string FormUsername { get => _formUsername; set => SetProperty(ref _formUsername, value); }

    private bool _isFormVisible;
    public bool IsFormVisible
    {
        get => _isFormVisible;
        set => SetProperty(ref _isFormVisible, value);
    }

    private string? _editingId;
    /// <summary>正在编辑的记录ID（null=新增模式）</summary>
    public string? EditingId
    {
        get => _editingId;
        set { SetProperty(ref _editingId, value); OnPropertyChanged(nameof(IsEditMode)); OnPropertyChanged(nameof(FormTitle)); }
    }

    /// <summary>是否为编辑模式</summary>
    public bool IsEditMode => EditingId != null;

    /// <summary>表单标题</summary>
    public string FormTitle => IsEditMode ? "编辑连接" : "新增连接";

    private void RefreshCommands()
    {
        (ConnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private async Task LoadAsync()
    {
        try
        {
            var userId = CurrentUserContext.CurrentUserId;
            var configs = await _loginConfigService.GetAllAsync(userId);
            // 映射 ArasLoginConfig → LoginInfo
            var loginInfos = configs.Select(c => new LoginInfo
            {
                Id = c.Id,
                Url = c.Url,
                Database = c.DatabaseName,
                Username = c.Username,
                Password = c.Md5Password,         // DB中的MD5哈希
                IsPasswordHashed = true,           // 标记为已哈希
                RememberMe = false,
                ConfigName = c.Id,
                IsEnabled = c.IsEnabled
            }).ToList();

            LoginInfos = new ObservableCollection<LoginInfo>(loginInfos);
            StatusMessage = $"已加载 {loginInfos.Count} 条连接记录";
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
            // LoginInfo.Password 已是从DB加载的MD5哈希, IsPasswordHashed=true
            await _loginService.LoginAsync(info);
            // 连接成功后设为启用（禁用其他）
            if (!string.IsNullOrEmpty(info.Id))
            {
                await _loginConfigService.EnableAsync(info.Id, CurrentUserContext.CurrentUserId);
            }
            // 重建连接池
            _connectionPool.Reinitialize(10);
            StatusMessage = "已切换到 " + info.Username + "@" + info.Database + "，连接池已更新";
            // 刷新列表以更新启用状态
            await LoadAsync();
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
        // 参数为 LoginInfo.Id (string)
        var id = parameter as string;
        if (string.IsNullOrEmpty(id)) return;

        IsProcessing = true;
        try
        {
            await _loginConfigService.DeleteAsync(id);
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

    /// <summary>编辑已有连接 — 填充表单</summary>
    private async Task EditAsync(object? parameter)
    {
        if (parameter is not LoginInfo info || string.IsNullOrEmpty(info.Id)) return;

        EditingId = info.Id;
        FormUrl = info.Url;
        FormDatabase = info.Database;
        FormUsername = info.Username;
        // 密码框显示占位符（由View层设置）
        IsFormVisible = true;

        await Task.CompletedTask;
    }

    /// <summary>
    /// 新增/更新保存登录信息 — MD5加密密码后持久化到数据库
    /// </summary>
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(FormUrl) || string.IsNullOrEmpty(FormUsername))
        {
            ErrorMessage = "URL 和用户名不能为空";
            return;
        }

        IsProcessing = true;
        ErrorMessage = string.Empty;
        try
        {
            // 从 View 读取密码
            var pwd = OnRequestPassword?.Invoke() ?? "";
            var isPlaceholder = OnIsPasswordPlaceholder?.Invoke() ?? false;

            string md5Password;
            if (IsEditMode && isPlaceholder)
            {
                // 编辑模式且密码未改动 — 保留原有MD5
                var existing = await _loginConfigService.GetByIdAsync(EditingId!);
                md5Password = existing?.Md5Password ?? "";
            }
            else
            {
                // 新密码 → MD5（32位小写十六进制）
                md5Password = pwd.ToMd5();
            }

            var config = new ArasLoginConfig
            {
                Id = IsEditMode ? EditingId! : Guid.NewGuid().ToString("N")[..12],
                Url = FormUrl,
                DatabaseName = FormDatabase,
                Username = FormUsername,
                Md5Password = md5Password,
                IsEnabled = false,  // 保存时不自动启用
                CreatorOn = DateTime.Now,
                UserId = CurrentUserContext.CurrentUserId
            };

            await _loginConfigService.SaveAsync(config);
            StatusMessage = IsEditMode
                ? $"已更新: {FormUsername}@{FormDatabase}"
                : $"已保存: {FormUsername}@{FormDatabase}";
            CloseForm();
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "保存失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("Aras登录-保存配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private void ToggleForm()
    {
        if (IsFormVisible)
            CloseForm();
        else
        {
            EditingId = null;
            FormUrl = "";
            FormDatabase = "";
            FormUsername = "";
            IsFormVisible = true;
        }
    }

    private void CloseForm()
    {
        IsFormVisible = false;
        EditingId = null;
        FormUrl = "";
        FormDatabase = "";
        FormUsername = "";
    }

    private void CancelTranslate() { }

    /// <summary>自动连接 — 应用启动后检查是否有启用的配置并自动连接</summary>
    public async Task TryAutoConnectAsync()
    {
        try
        {
            var enabled = await _loginConfigService.GetEnabledAsync(CurrentUserContext.CurrentUserId);
            if (enabled == null) return;

            var info = new LoginInfo
            {
                Id = enabled.Id,
                Url = enabled.Url,
                Database = enabled.DatabaseName,
                Username = enabled.Username,
                Password = enabled.Md5Password,
                IsPasswordHashed = true,
                IsEnabled = true
            };

            await _loginService.LoginAsync(info);
            _connectionPool.Reinitialize(10);
            StatusMessage = $"自动连接: {enabled.Username}@{enabled.DatabaseName}";
        }
        catch (Exception ex)
        {
            // 自动连接失败不弹窗，仅记录日志
            System.Diagnostics.Debug.WriteLine($"[ArasLogin] 自动连接失败: {ex.Message}");
            await _errorLogService.LogErrorAsync("Aras登录-自动连接", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    /// <summary>
    /// 请求密码回调 — 由 View 层注册，从 PasswordBox 获取当前输入的密码
    /// </summary>
    public Func<string>? OnRequestPassword { get; set; }

    /// <summary>
    /// 密码是否为占位符回调 — View 层注册，判断编辑模式下密码是否被修改
    /// </summary>
    public Func<bool>? OnIsPasswordPlaceholder { get; set; }
}
