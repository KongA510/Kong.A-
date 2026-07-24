using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 数据库导出配置管理 ViewModel — 多连接配置的增删改启用（参照 TranslationApiKeyViewModel）
/// </summary>
public class DatabaseExportConfigViewModel : ObservableObject
{
    private readonly IDatabaseExportConfigService _configService;
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IErrorLogService _errorLogService;

    private ObservableCollection<DatabaseExportConfig> _configs = [];
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isProcessing;

    public DatabaseExportConfigViewModel(
        IDatabaseExportConfigService configService,
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IErrorLogService errorLogService)
    {
        _configService = configService;
        _dbFactory = dbFactory;
        _errorLogService = errorLogService;

        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => !IsProcessing && !string.IsNullOrEmpty(NewConfigName));
        DeleteCommand = new RelayCommand(async p => await DeleteAsync(p), _ => !IsProcessing);
        EnableCommand = new RelayCommand(async p => await EnableAsync(p), _ => !IsProcessing);
        EditCommand = new RelayCommand(p => EditExisting(p), _ => !IsProcessing);
        ToggleFormCommand = new RelayCommand(_ => ToggleForm());
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());
        CancelEditCommand = new RelayCommand(_ => CancelEdit());

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await db.EnsureSchemaAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DBExportConfig] Schema同步失败: {ex.Message}");
        }
        await LoadAsync();
    }

    // ===== 列表 =====
    public ObservableCollection<DatabaseExportConfig> Configs { get => _configs; set => SetProperty(ref _configs, value); }

    // ===== 状态 =====
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public bool IsProcessing
    {
        get => _isProcessing;
        set { SetProperty(ref _isProcessing, value); RefreshCommands(); }
    }

    // ===== 表单属性 =====
    private string _newConfigName = string.Empty;
    public string NewConfigName
    {
        get => _newConfigName;
        set { SetProperty(ref _newConfigName, value); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _newConnectionString = string.Empty;
    public string NewConnectionString { get => _newConnectionString; set => SetProperty(ref _newConnectionString, value); }

    private string _newRemark = string.Empty;
    public string NewRemark { get => _newRemark; set => SetProperty(ref _newRemark, value); }

    private bool _isFormVisible;
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }

    private bool _isEditMode;
    public bool IsEditMode { get => _isEditMode; set => SetProperty(ref _isEditMode, value); }

    private string? _editingId;
    public string? EditingId { get => _editingId; set => SetProperty(ref _editingId, value); }

    public string FormTitle => IsEditMode ? "编辑连接配置" : "新增连接配置";

    // ===== 命令 =====
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand EnableCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand ToggleFormCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand CancelEditCommand { get; }

    private void RefreshCommands()
    {
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (EnableCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private async Task LoadAsync()
    {
        try
        {
            var userId = CurrentUserContext.CurrentUserId;
            var configs = await _configService.GetAllAsync(userId);
            Configs = new ObservableCollection<DatabaseExportConfig>(configs);
            StatusMessage = configs.Count > 0
                ? $"已加载 {configs.Count} 个连接配置"
                : "暂无连接配置，请新增";
        }
        catch (Exception ex)
        {
            ErrorMessage = "加载失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据库导出配置-加载", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(NewConfigName)) { ErrorMessage = "配置名称不能为空"; return; }
        if (string.IsNullOrEmpty(NewConnectionString)) { ErrorMessage = "连接字符串不能为空"; return; }

        IsProcessing = true; ErrorMessage = string.Empty;
        try
        {
            var config = new DatabaseExportConfig
            {
                Id = IsEditMode && !string.IsNullOrEmpty(EditingId) ? EditingId : Guid.NewGuid().ToString("N")[..12],
                UserId = CurrentUserContext.CurrentUserId ?? "system",
                ConfigName = NewConfigName.Trim(),
                ConnectionString = NewConnectionString.Trim(),
                Remark = string.IsNullOrWhiteSpace(NewRemark) ? null : NewRemark.Trim(),
                IsEnabled = false,
                CreatorOn = DateTime.Now
            };
            await _configService.SaveAsync(config);
            StatusMessage = IsEditMode ? $"已更新: {config.ConfigName}" : $"已新增: {config.ConfigName}";
            CancelEdit(); await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "保存失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据库导出配置-保存", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsProcessing = false; }
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not string id || string.IsNullOrEmpty(id)) return;
        IsProcessing = true; ErrorMessage = string.Empty;
        try { await _configService.DeleteAsync(id); StatusMessage = "已删除连接配置"; await LoadAsync(); }
        catch (Exception ex) { ErrorMessage = "删除失败: " + ex.Message; }
        finally { IsProcessing = false; }
    }

    private async Task EnableAsync(object? parameter)
    {
        if (parameter is not string id || string.IsNullOrEmpty(id)) return;
        IsProcessing = true; ErrorMessage = string.Empty;
        try
        {
            var userId = CurrentUserContext.CurrentUserId ?? "system";
            await _configService.EnableAsync(id, userId);
            StatusMessage = "已切换启用配置"; await LoadAsync();
        }
        catch (Exception ex) { ErrorMessage = "启用失败: " + ex.Message; }
        finally { IsProcessing = false; }
    }

    private void EditExisting(object? parameter)
    {
        if (parameter is not DatabaseExportConfig config) return;
        IsEditMode = true; EditingId = config.Id;
        NewConfigName = config.ConfigName;
        NewConnectionString = config.ConnectionString;
        NewRemark = config.Remark ?? string.Empty;
        IsFormVisible = true;
        OnPropertyChanged(nameof(FormTitle));
    }

    private void ToggleForm()
    {
        if (IsFormVisible) CancelEdit();
        else { IsEditMode = false; IsFormVisible = true; OnPropertyChanged(nameof(FormTitle)); }
    }

    private void CancelEdit()
    {
        IsFormVisible = false; IsEditMode = false; EditingId = null;
        NewConfigName = ""; NewConnectionString = ""; NewRemark = "";
        OnPropertyChanged(nameof(FormTitle));
    }
}
