using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 敏感操作日志 ViewModel — 分页 + 按操作类型筛选（WinUI 3）。
/// </summary>
public class OperationLogViewModel : ObservableObject
{
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    private ObservableCollection<OperationLog> _entries = [];
    private int _currentPage = 1, _pageSize = 20, _totalCount;
    private string _filterOperationType = "全部";
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public OperationLogViewModel(IOperationLogService operationLogService, IErrorLogService errorLogService)
    {
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;

        RefreshCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); });
        FirstPageCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); }, _ => CurrentPage > 1);
        PrevPageCommand = new RelayCommand(async _ => { CurrentPage--; await LoadDataAsync(); }, _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => { CurrentPage++; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(async _ => { CurrentPage = TotalPages; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);

        OperationTypeOptions = new ObservableCollection<string> { "全部", "Create", "Update", "Delete", "Import" };

        _ = LoadDataAsync();
    }

    public ObservableCollection<OperationLog> Entries { get => _entries; set => SetProperty(ref _entries, value); }
    public ObservableCollection<string> OperationTypeOptions { get; }

    public int CurrentPage { get => _currentPage; set { SetProperty(ref _currentPage, value); UpdatePagingCommands(); } }
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / _pageSize));
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页  共 {TotalCount} 条";

    public string FilterOperationType
    {
        get => _filterOperationType;
        set { SetProperty(ref _filterOperationType, value); CurrentPage = 1; _ = LoadDataAsync(); }
    }

    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }

    private void UpdatePagingCommands()
    {
        (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(PageInfo));
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var (items, total) = await _operationLogService.GetPagedAsync(
                CurrentPage, _pageSize,
                FilterOperationType == "全部" ? null : FilterOperationType);

            Entries = new ObservableCollection<OperationLog>(items);
            TotalCount = total;
            UpdatePagingCommands();
            StatusMessage = $"共 {total} 条操作记录";

            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("操作日志-加载列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(PageInfo));
        }
    }
}
