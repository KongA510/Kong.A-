using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>对象类基础权限、可创建者和生命周期的一键设定页面。</summary>
public sealed class ObjectClassConfigurationViewModel : ObservableObject, IDisposable
{
    private readonly IObjectClassConfigurationService _service;
    private readonly IArasConnectionService _connectionService;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;
    private readonly DispatcherQueue? _dispatcherQueue;
    private string _queryKeyword = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _initialized;
    private bool _disposed;
    private ObjectClassConfigurationSettings _settings = ObjectClassConfigurationSettings.CreateDefault();
    private ObjectClassConfigurationOptions _options = new();
    private ObjectClassConfigurationProgress? _progress;
    private CancellationTokenSource? _cancellationTokenSource;

    public ObjectClassConfigurationViewModel(
        IObjectClassConfigurationService service,
        IArasConnectionService connectionService,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _dialogService = dialogService;
        _errorLogService = errorLogService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _connectionService.ConnectionChanged += OnConnectionChanged;

        QueryCommand = new RelayCommand(async _ => await QueryAsync(), _ => !IsBusy);
        SelectAllCommand = new RelayCommand(_ => SetAllSelected(true), _ => !IsBusy && ItemTypes.Count > 0);
        ClearSelectionCommand = new RelayCommand(_ => SetAllSelected(false), _ => !IsBusy && ItemTypes.Count > 0);
        SaveSettingsCommand = new RelayCommand(async _ => await SaveSettingsAsync(), _ => !IsBusy);
        ResetSettingsCommand = new RelayCommand(_ => ResetSettings(), _ => !IsBusy);
        ExecuteCommand = new RelayCommand(async _ => await ExecuteAsync(),
            _ => !IsBusy && ItemTypes.Any(item => item.IsSelected));
        CancelCommand = new RelayCommand(_ => _cancellationTokenSource?.Cancel(), _ => IsBusy);
    }

    public ObservableCollection<ObjectClassConfigurationItem> ItemTypes { get; } = [];

    public string QueryKeyword
    {
        get => _queryKeyword;
        set => SetProperty(ref _queryKeyword, value);
    }

    public ObjectClassConfigurationSettings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public ObjectClassConfigurationOptions Options
    {
        get => _options;
        set => SetProperty(ref _options, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value))
                return;
            OnPropertyChanged(nameof(IsProgressIndeterminate));
            RefreshCommands();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (SetProperty(ref _statusMessage, value))
                OnPropertyChanged(nameof(HasStatus));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public ObjectClassConfigurationProgress? Progress
    {
        get => _progress;
        private set
        {
            if (!SetProperty(ref _progress, value))
                return;
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

    public string ConnectionText => _connectionService.IsConnected && _connectionService.CurrentConnection != null
        ? $"Aras：{_connectionService.CurrentConnection.Database} / {_connectionService.CurrentConnection.Username}"
        : "Aras：未连接";
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string SelectionSummary => $"共 {ItemTypes.Count} 个对象类，已选择 {ItemTypes.Count(item => item.IsSelected)} 个";
    public double ProgressPercentage => Progress?.Percentage ?? 0;
    public string ProgressText => Progress?.StatusText ?? string.Empty;
    public bool IsProgressIndeterminate => IsBusy && Progress == null;

    public ICommand QueryCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand ClearSelectionCommand { get; }
    public ICommand SaveSettingsCommand { get; }
    public ICommand ResetSettingsCommand { get; }
    public ICommand ExecuteCommand { get; }
    public ICommand CancelCommand { get; }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        _initialized = true;
        Settings = await _service.LoadSettingsAsync();
        OnPropertyChanged(nameof(ConnectionText));
        if (_connectionService.IsConnected)
            await QueryAsync();
        else
            StatusMessage = "请先在 Aras连接 页面登录服务器。";
    }

    public void NotifySelectionChanged()
    {
        OnPropertyChanged(nameof(SelectionSummary));
        RefreshCommands();
    }

    private async Task QueryAsync()
    {
        if (!_connectionService.IsConnected)
        {
            StatusMessage = "尚未连接 Aras，无法查询对象类。";
            return;
        }

        IsBusy = true;
        Progress = null;
        ErrorMessage = string.Empty;
        StatusMessage = "正在查询英文、简体中文、繁体中文对象类标签…";
        try
        {
            var items = await _service.QueryItemTypesAsync(QueryKeyword);
            ItemTypes.Clear();
            foreach (var item in items)
                ItemTypes.Add(item);
            NotifySelectionChanged();
            StatusMessage = $"查询完成，共显示 {ItemTypes.Count} 个对象类。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-页面查询", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveSettingsAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _service.SaveSettingsAsync(Settings);
            StatusMessage = "角色模板已保存，后续执行将默认使用当前配置。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存角色模板失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-页面保存模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ResetSettings()
    {
        Settings = ObjectClassConfigurationSettings.CreateDefault();
        StatusMessage = "已恢复内置默认角色；点击“保存角色配置”后持久化。";
    }

    private async Task ExecuteAsync()
    {
        var selected = ItemTypes.Where(item => item.IsSelected).ToList();
        if (selected.Count == 0)
        {
            StatusMessage = "请先选择至少一个对象类。";
            return;
        }
        if (!Options.HasAnySelection)
        {
            StatusMessage = "请至少勾选一项基础设定。";
            return;
        }

        var confirmed = await _dialogService.ConfirmAsync(
            "一键设定对象类",
            $"将对 {selected.Count} 个对象类执行所选基础设定。系统会复用已有 Permission 和页签关系，并补齐同名生命周期的状态权限，是否继续？",
            "开始设定");
        if (!confirmed)
            return;

        IsBusy = true;
        Progress = null;
        ErrorMessage = string.Empty;
        _cancellationTokenSource = new CancellationTokenSource();
        foreach (var item in selected)
            item.OperationSummary = "等待执行";

        try
        {
            var progress = new Progress<ObjectClassConfigurationProgress>(value =>
            {
                Progress = value;
                StatusMessage = value.StatusText;
            });
            var result = await _service.ConfigureAsync(
                selected,
                Options,
                Settings,
                progress,
                _cancellationTokenSource.Token);
            StatusMessage = result.FailedCount == 0
                ? $"基础设定完成：成功 {result.SuccessCount}/{result.TotalCount}。"
                : $"基础设定完成：成功 {result.SuccessCount}，失败 {result.FailedCount}。";
            if (result.FailedCount > 0)
                ErrorMessage = string.Join("；", result.FailedDetails);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "操作已取消；已成功写入的对象类配置会保留。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"执行失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-页面执行", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsBusy = false;
        }
    }

    private void SetAllSelected(bool selected)
    {
        foreach (var item in ItemTypes)
            item.IsSelected = selected;
        NotifySelectionChanged();
    }

    private void OnConnectionChanged()
    {
        if (_dispatcherQueue is { HasThreadAccess: false })
        {
            _dispatcherQueue.TryEnqueue(RefreshConnectionState);
            return;
        }

        RefreshConnectionState();
    }

    private void RefreshConnectionState()
    {
        OnPropertyChanged(nameof(ConnectionText));
        if (!_connectionService.IsConnected)
        {
            ItemTypes.Clear();
            NotifySelectionChanged();
            StatusMessage = "Aras 连接已断开。";
        }
    }

    private void RefreshCommands()
    {
        (QueryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SelectAllCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearSelectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveSettingsCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ResetSettingsCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExecuteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _connectionService.ConnectionChanged -= OnConnectionChanged;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
