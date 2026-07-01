using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// AI 模型配置管理 ViewModel — 多 API Key 的增删改启用
/// 参照 ArasLoginViewModel 的多记录管理模式
/// </summary>
public class TranslationApiKeyViewModel : ObservableObject
{
    private readonly ITextTranslationService _translationService;
    private readonly IErrorLogService _errorLogService;

    private ObservableCollection<AiModelConfig> _models = [];
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isProcessing;

    public TranslationApiKeyViewModel(
        ITextTranslationService translationService,
        IErrorLogService errorLogService)
    {
        _translationService = translationService;
        _errorLogService = errorLogService;

        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => !IsProcessing && !string.IsNullOrEmpty(NewModelName));
        DeleteCommand = new RelayCommand(async p => await DeleteAsync(p), _ => !IsProcessing);
        EnableCommand = new RelayCommand(async p => await EnableAsync(p), _ => !IsProcessing);
        EditCommand = new RelayCommand(p => EditExisting(p), _ => !IsProcessing);
        ToggleFormCommand = new RelayCommand(_ => ToggleForm());
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());
        CancelEditCommand = new RelayCommand(_ => CancelEdit());

        _ = LoadAsync();
    }

    // ===== 列表 =====

    public ObservableCollection<AiModelConfig> Models
    {
        get => _models;
        set => SetProperty(ref _models, value);
    }

    // ===== 状态 =====

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

    // ===== 表单属性 =====

    private string _newModelName = string.Empty;
    public string NewModelName
    {
        get => _newModelName;
        set { SetProperty(ref _newModelName, value); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _newApiKey = string.Empty;
    public string NewApiKey
    {
        get => _newApiKey;
        set => SetProperty(ref _newApiKey, value);
    }

    private string _newApiBaseUrl = "https://apihub.agnes-ai.com/v1/chat/completions";
    public string NewApiBaseUrl
    {
        get => _newApiBaseUrl;
        set => SetProperty(ref _newApiBaseUrl, value);
    }

    private string _newModelIdentifier = "agnes-2.0-flash";
    public string NewModelIdentifier
    {
        get => _newModelIdentifier;
        set => SetProperty(ref _newModelIdentifier, value);
    }

    private bool _isFormVisible;
    public bool IsFormVisible
    {
        get => _isFormVisible;
        set => SetProperty(ref _isFormVisible, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    private string? _editingId;
    /// <summary>编辑中的模型ID（null=新增模式）</summary>
    public string? EditingId
    {
        get => _editingId;
        set => SetProperty(ref _editingId, value);
    }

    /// <summary>表单标题：新增 / 编辑</summary>
    public string FormTitle => IsEditMode ? "编辑模型" : "新增模型";

    // ===== 命令 =====

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand EnableCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand ToggleFormCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand CancelEditCommand { get; }

    // ===== 私有方法 =====

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
            var models = await _translationService.GetAiModelsAsync(userId);
            Models = new ObservableCollection<AiModelConfig>(models);
            StatusMessage = models.Count > 0
                ? $"已加载 {models.Count} 个模型配置"
                : "暂无模型配置，请新增";
        }
        catch (Exception ex)
        {
            ErrorMessage = "加载失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("AI模型-加载列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(NewModelName))
        {
            ErrorMessage = "模型名称不能为空";
            return;
        }

        IsProcessing = true;
        ErrorMessage = string.Empty;
        try
        {
            var config = new AiModelConfig
            {
                Id = IsEditMode && !string.IsNullOrEmpty(EditingId)
                    ? EditingId
                    : Guid.NewGuid().ToString("N")[..12],
                UserId = CurrentUserContext.CurrentUserId,
                ModelName = NewModelName.Trim(),
                ApiKey = NewApiKey.Trim(),
                ApiBaseUrl = string.IsNullOrWhiteSpace(NewApiBaseUrl)
                    ? "https://apihub.agnes-ai.com/v1/chat/completions"
                    : NewApiBaseUrl.Trim(),
                ModelIdentifier = string.IsNullOrWhiteSpace(NewModelIdentifier)
                    ? "agnes-2.0-flash"
                    : NewModelIdentifier.Trim(),
                IsEnabled = false,
                CreatorOn = DateTime.Now
            };

            await _translationService.SaveAiModelAsync(config);
            StatusMessage = IsEditMode ? $"已更新: {config.ModelName}" : $"已新增: {config.ModelName}";
            CancelEdit();
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "保存失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("AI模型-保存配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (parameter is not string id || string.IsNullOrEmpty(id)) return;

        IsProcessing = true;
        ErrorMessage = string.Empty;
        try
        {
            await _translationService.DeleteAiModelAsync(id);
            StatusMessage = "已删除模型配置";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "删除失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("AI模型-删除配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task EnableAsync(object? parameter)
    {
        if (parameter is not string id || string.IsNullOrEmpty(id)) return;

        IsProcessing = true;
        ErrorMessage = string.Empty;
        try
        {
            var userId = CurrentUserContext.CurrentUserId;
            await _translationService.EnableAiModelAsync(id, userId);
            StatusMessage = "已切换启用模型";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "启用失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("AI模型-启用切换", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>点击编辑按钮，填充表单</summary>
    private void EditExisting(object? parameter)
    {
        if (parameter is not AiModelConfig config) return;

        IsEditMode = true;
        EditingId = config.Id;
        NewModelName = config.ModelName;
        NewApiKey = config.ApiKey;
        NewApiBaseUrl = config.ApiBaseUrl;
        NewModelIdentifier = config.ModelIdentifier;
        IsFormVisible = true;
        OnPropertyChanged(nameof(FormTitle));
    }

    private void ToggleForm()
    {
        if (IsFormVisible)
        {
            CancelEdit();
        }
        else
        {
            IsEditMode = false;
            IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }
    }

    private void CancelEdit()
    {
        IsFormVisible = false;
        IsEditMode = false;
        EditingId = null;
        NewModelName = "";
        NewApiKey = "";
        NewApiBaseUrl = "https://apihub.agnes-ai.com/v1/chat/completions";
        NewModelIdentifier = "agnes-2.0-flash";
        OnPropertyChanged(nameof(FormTitle));
    }
}
