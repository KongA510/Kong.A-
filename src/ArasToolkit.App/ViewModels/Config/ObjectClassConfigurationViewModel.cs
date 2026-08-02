using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.ViewModels;

/// <summary>WPF 对象类基础设定页面 ViewModel。</summary>
public sealed class ObjectClassConfigurationViewModel : ObservableObject
{
    private readonly IObjectClassConfigurationService _service;
    private readonly IArasConnectionService _connectionService;
    private readonly IErrorLogService _errorLogService;
    private string _queryKeyword = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _initialized;
    private ObjectClassConfigurationSettings _settings = ObjectClassConfigurationSettings.CreateDefault();
    private ObjectClassConfigurationOptions _options = new();
    private ObjectClassConfigurationProgress? _progress;
    private CancellationTokenSource? _cancellationTokenSource;

    public ObjectClassConfigurationViewModel(
        IObjectClassConfigurationService service,
        IArasConnectionService connectionService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _errorLogService = errorLogService;

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
    public string QueryKeyword { get => _queryKeyword; set => SetProperty(ref _queryKeyword, value); }
    public ObjectClassConfigurationSettings Settings { get => _settings; set => SetProperty(ref _settings, value); }
    public ObjectClassConfigurationOptions Options { get => _options; set => SetProperty(ref _options, value); }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            OnPropertyChanged(nameof(IsProgressIndeterminate));
            RefreshCommands();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ObjectClassConfigurationProgress? Progress
    {
        get => _progress;
        private set
        {
            if (!SetProperty(ref _progress, value)) return;
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

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
        if (_initialized) return;
        _initialized = true;
        Settings = await _service.LoadSettingsAsync();
        if (_connectionService.IsConnected)
            await QueryAsync();
        else
            StatusMessage = "请先连接 Aras 后再查询对象类。";
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
        ErrorMessage = string.Empty;
        Progress = null;
        try
        {
            var items = await _service.QueryItemTypesAsync(QueryKeyword);
            ItemTypes.Clear();
            foreach (var item in items) ItemTypes.Add(item);
            NotifySelectionChanged();
            StatusMessage = $"查询完成，共显示 {ItemTypes.Count} 个对象类。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-WPF查询", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private async Task SaveSettingsAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _service.SaveSettingsAsync(Settings);
            StatusMessage = "角色模板已保存。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-WPF保存模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private void ResetSettings()
    {
        Settings = ObjectClassConfigurationSettings.CreateDefault();
        StatusMessage = "已恢复内置默认角色，保存后持久化。";
    }

    private async Task ExecuteAsync()
    {
        var selected = ItemTypes.Where(item => item.IsSelected).ToList();
        if (selected.Count == 0 || !Options.HasAnySelection)
        {
            StatusMessage = "请选择对象类并至少勾选一项基础设定。";
            return;
        }

        var confirmed = MessageBox.Show(
            $"将对 {selected.Count} 个对象类执行所选基础设定，是否继续？",
            "对象类配置",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
        if (!confirmed) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        Progress = null;
        _cancellationTokenSource = new CancellationTokenSource();
        foreach (var item in selected) item.OperationSummary = "等待执行";
        try
        {
            var progress = new Progress<ObjectClassConfigurationProgress>(value =>
            {
                Progress = value;
                StatusMessage = value.StatusText;
            });
            var result = await _service.ConfigureAsync(
                selected, Options, Settings, progress, _cancellationTokenSource.Token);
            StatusMessage = $"基础设定完成：成功 {result.SuccessCount}，失败 {result.FailedCount}。";
            if (result.FailedCount > 0)
                ErrorMessage = string.Join("；", result.FailedDetails);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "操作已取消；已经写入的配置会保留。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"执行失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-WPF执行", ex.Message,
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
        foreach (var item in ItemTypes) item.IsSelected = selected;
        NotifySelectionChanged();
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
}
