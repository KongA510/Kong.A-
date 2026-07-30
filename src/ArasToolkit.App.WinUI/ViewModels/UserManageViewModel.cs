using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 用户管理 ViewModel — 管理员创建/编辑/禁用/删除用户、分配角色
/// </summary>
public class UserManageViewModel : ObservableObject
{
    private readonly IAppUserService _appUserService;
    private readonly IErrorLogService _errorLogService;
    private readonly IDialogService _dialogService;

    private string _statusMessage = string.Empty;
    private bool _isLoading;
    private AppUser? _selectedUser;

    // 新建/编辑表单
    private string _formUsername = string.Empty;
    private string _formPassword = string.Empty;
    private string _formDisplayName = string.Empty;
    private string _formRole = "User";
    private bool _isEditMode;

    public UserManageViewModel(
        IAppUserService appUserService,
        IErrorLogService errorLogService,
        IDialogService dialogService)
    {
        _appUserService = appUserService;
        _errorLogService = errorLogService;
        _dialogService = dialogService;

        LoadCommand = new RelayCommand(async _ => await LoadUsersAsync());
        CreateUserCommand = new RelayCommand(async _ => await CreateUserAsync());
        EditUserCommand = new RelayCommand(async u => await StartEditAsync(u as AppUser));
        SaveEditCommand = new RelayCommand(async _ => await SaveEditAsync());
        CancelEditCommand = new RelayCommand(_ => CancelEdit());
        ResetPasswordCommand = new RelayCommand(async u => await ResetPasswordAsync(u as AppUser));
        ToggleActiveCommand = new RelayCommand(async u => await ToggleActiveAsync(u as AppUser));
        DeleteUserCommand = new RelayCommand(async u => await DeleteUserAsync(u as AppUser));

        _ = LoadUsersAsync();
    }

    public ObservableCollection<AppUser> Users { get; } = new();

    public string[] Roles { get; } = { "Admin", "User", "Viewer" };

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public AppUser? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    // 表单属性
    public string FormUsername
    {
        get => _formUsername;
        set => SetProperty(ref _formUsername, value);
    }

    public string FormPassword
    {
        get => _formPassword;
        set => SetProperty(ref _formPassword, value);
    }

    public string FormDisplayName
    {
        get => _formDisplayName;
        set => SetProperty(ref _formDisplayName, value);
    }

    public string FormRole
    {
        get => _formRole;
        set => SetProperty(ref _formRole, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            SetProperty(ref _isEditMode, value);
            OnPropertyChanged(nameof(IsCreateMode));
            OnPropertyChanged(nameof(FormTitle));
        }
    }

    public bool IsCreateMode => !_isEditMode;
    public string FormTitle => _isEditMode ? "编辑用户" : "新建用户";

    public ICommand LoadCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand SaveEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand ResetPasswordCommand { get; }
    public ICommand ToggleActiveCommand { get; }
    public ICommand DeleteUserCommand { get; }

    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        StatusMessage = "加载中...";
        try
        {
            var users = await _appUserService.GetAllUsersAsync();
            Users.Clear();
            foreach (var u in users)
                Users.Add(u);
            StatusMessage = $"共 {users.Count} 个用户";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("用户管理-加载", ex.Message, "P1-普通", ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateUserAsync()
    {
        if (string.IsNullOrWhiteSpace(FormUsername) || string.IsNullOrWhiteSpace(FormPassword))
        {
            await _dialogService.AlertAsync("提示", "请填写用户名和密码");
            return;
        }

        try
        {
            await _appUserService.CreateUserAsync(
                FormUsername.Trim(), FormPassword, FormDisplayName.Trim(), FormRole);
            StatusMessage = $"用户 {FormUsername} 创建成功";
            ClearForm();
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.AlertAsync("创建失败", ex.Message);
            await _errorLogService.LogErrorAsync("用户管理-创建", ex.Message, "P1-普通", ex.StackTrace);
        }
    }

    private Task StartEditAsync(AppUser? user)
    {
        if (user == null) return Task.CompletedTask;
        SelectedUser = user;
        FormUsername = user.Username;
        FormDisplayName = user.DisplayName ?? string.Empty;
        FormRole = user.Role;
        FormPassword = string.Empty;
        IsEditMode = true;
        return Task.CompletedTask;
    }

    private async Task SaveEditAsync()
    {
        if (SelectedUser == null) return;
        try
        {
            await _appUserService.UpdateUserAsync(SelectedUser.Id, FormDisplayName.Trim(), FormRole);
            StatusMessage = $"用户 {SelectedUser.Username} 已更新";
            CancelEdit();
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.AlertAsync("更新失败", ex.Message);
            await _errorLogService.LogErrorAsync("用户管理-编辑", ex.Message, "P1-普通", ex.StackTrace);
        }
    }

    private void CancelEdit()
    {
        IsEditMode = false;
        SelectedUser = null;
        ClearForm();
    }

    private async Task ResetPasswordAsync(AppUser? user)
    {
        if (user == null) return;
        var newPwd = await _dialogService.PromptAsync("重置密码", $"请输入 {user.Username} 的新密码:");
        if (string.IsNullOrWhiteSpace(newPwd)) return;

        try
        {
            await _appUserService.ResetPasswordAsync(user.Id, newPwd);
            StatusMessage = $"已重置 {user.Username} 的密码";
        }
        catch (Exception ex)
        {
            await _dialogService.AlertAsync("重置失败", ex.Message);
            await _errorLogService.LogErrorAsync("用户管理-重置密码", ex.Message, "P1-普通", ex.StackTrace);
        }
    }

    private async Task ToggleActiveAsync(AppUser? user)
    {
        if (user == null) return;

        if (user.Username == CurrentUserContext.CurrentUserName)
        {
            await _dialogService.AlertAsync("提示", "不能禁用自己的账号");
            return;
        }

        var action = user.IsActive ? "禁用" : "启用";
        var confirmed = await _dialogService.ConfirmAsync("确认", $"确定要{action}用户 {user.Username} 吗？");
        if (!confirmed) return;

        try
        {
            await _appUserService.SetUserActiveAsync(user.Id, !user.IsActive);
            StatusMessage = $"已{action}用户 {user.Username}";
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.AlertAsync("操作失败", ex.Message);
            await _errorLogService.LogErrorAsync("用户管理-切换状态", ex.Message, "P1-普通", ex.StackTrace);
        }
    }

    private async Task DeleteUserAsync(AppUser? user)
    {
        if (user == null) return;

        if (user.Username == CurrentUserContext.CurrentUserName)
        {
            await _dialogService.AlertAsync("提示", "不能删除自己的账号");
            return;
        }

        var confirmed = await _dialogService.ConfirmAsync("确认删除", $"确定要删除用户 {user.Username} 吗？此操作不可恢复。");
        if (!confirmed) return;

        try
        {
            await _appUserService.DeleteUserAsync(user.Id);
            StatusMessage = $"已删除用户 {user.Username}";
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.AlertAsync("删除失败", ex.Message);
            await _errorLogService.LogErrorAsync("用户管理-删除", ex.Message, "P1-普通", ex.StackTrace);
        }
    }

    private void ClearForm()
    {
        FormUsername = string.Empty;
        FormPassword = string.Empty;
        FormDisplayName = string.Empty;
        FormRole = "User";
    }
}
