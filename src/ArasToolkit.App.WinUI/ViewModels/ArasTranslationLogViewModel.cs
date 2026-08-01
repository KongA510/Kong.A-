using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>当前应用用户的字段、表单、窗体翻译任务和明细日志。</summary>
public sealed class ArasTranslationLogViewModel : ObservableObject
{
    private readonly IArasTranslationLogService _service;
    private readonly IErrorLogService _errorLogService;
    private readonly int _pageSize = 20;
    private TranslationTask? _selectedTask;
    private int _currentPage = 1;
    private int _totalCount;
    private string _selectedTaskType = "全部类型";
    private string _selectedStatus = "全部状态";
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading;

    public ArasTranslationLogViewModel(
        IArasTranslationLogService service,
        IErrorLogService errorLogService)
    {
        _service = service;
        _errorLogService = errorLogService;
        RefreshCommand = new RelayCommand(async _ => await LoadAsync(), _ => !IsLoading);
        ApplyFilterCommand = new RelayCommand(async _ =>
        {
            CurrentPage = 1;
            await LoadAsync();
        }, _ => !IsLoading);
        ClearFilterCommand = new RelayCommand(async _ =>
        {
            SelectedTaskType = "全部类型";
            SelectedStatus = "全部状态";
            SearchText = string.Empty;
            CurrentPage = 1;
            await LoadAsync();
        }, _ => !IsLoading);
        FirstPageCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadAsync(); },
            _ => !IsLoading && CurrentPage > 1);
        PreviousPageCommand = new RelayCommand(async _ => { CurrentPage--; await LoadAsync(); },
            _ => !IsLoading && CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => { CurrentPage++; await LoadAsync(); },
            _ => !IsLoading && CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(async _ => { CurrentPage = TotalPages; await LoadAsync(); },
            _ => !IsLoading && CurrentPage < TotalPages);

        _ = LoadAsync();
    }

    public ObservableCollection<TranslationTask> Tasks { get; } = [];
    public ObservableCollection<TranslationRecord> Records { get; } = [];
    public ObservableCollection<string> TaskTypes { get; } =
        ["全部类型", "字段翻译", "表单翻译", "窗体翻译"];
    public ObservableCollection<string> Statuses { get; } =
        ["全部状态", "等待中", "翻译中", "已完成", "已取消", "失败"];

    public TranslationTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (!SetProperty(ref _selectedTask, value)) return;
            OnPropertyChanged(nameof(SelectedTaskTitle));
            _ = LoadSelectedTaskRecordsAsync();
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (!SetProperty(ref _currentPage, Math.Max(1, value))) return;
            OnPropertyChanged(nameof(PageInfo));
            RaisePagingStates();
        }
    }

    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (!SetProperty(ref _totalCount, value)) return;
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(PageInfo));
            RaisePagingStates();
        }
    }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / _pageSize));
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页 · 共 {TotalCount} 条任务";
    public string SelectedTaskTitle => SelectedTask == null
        ? "翻译明细（请选择上方任务）"
        : $"翻译明细 · {SelectedTask.TaskName}";
    public string SelectedTaskType { get => _selectedTaskType; set => SetProperty(ref _selectedTaskType, value); }
    public string SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }
    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (!SetProperty(ref _isLoading, value)) return;
            RaisePagingStates();
            (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ApplyFilterCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ClearFilterCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var status = SelectedStatus switch
            {
                "等待中" => "Pending",
                "翻译中" => "Translating",
                "已完成" => "Completed",
                "已取消" => "Cancelled",
                "失败" => "Failed",
                _ => null
            };
            var (items, total) = await _service.GetTasksAsync(
                CurrentUserContext.CurrentUserId,
                SelectedTaskType,
                status,
                SearchText,
                CurrentPage,
                _pageSize);
            TotalCount = total;
            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
                await LoadAsync();
                return;
            }

            var selectedId = SelectedTask?.Id;
            Tasks.Clear();
            foreach (var task in items) Tasks.Add(task);
            SelectedTask = Tasks.FirstOrDefault(task => task.Id == selectedId) ?? Tasks.FirstOrDefault();
            StatusMessage = $"已加载当前用户的 {total} 条 Aras 翻译任务。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"日志加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("Aras翻译日志-页面加载", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadSelectedTaskRecordsAsync()
    {
        Records.Clear();
        if (SelectedTask == null) return;
        try
        {
            var records = await _service.GetRecordsAsync(SelectedTask.Id);
            foreach (var record in records) Records.Add(record);
        }
        catch (Exception ex)
        {
            StatusMessage = $"明细加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("Aras翻译日志-明细加载", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private void RaisePagingStates()
    {
        (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
