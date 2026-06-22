using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 错误日志ViewModel - 分页 + 筛选
/// </summary>
public class ErrorLogViewModel : ObservableObject
{
    private readonly IErrorLogService _errorLogService;
    private ObservableCollection<ErrorLog> _entries = new();
    private int _currentPage = 1, _pageSize = 20, _totalCount;
    private string _filterLevel = "全部";
    private DateTime? _filterFromDate, _filterToDate;
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public ErrorLogViewModel(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
        RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
        FirstPageCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); }, _ => CurrentPage > 1);
        PrevPageCommand = new RelayCommand(async _ => { CurrentPage--; await LoadDataAsync(); }, _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => { CurrentPage++; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(async _ => { CurrentPage = TotalPages; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        ClearCommand = new RelayCommand(async _ => await ClearAllAsync());
        ApplyFilterCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); });
        FilterLevels = new ObservableCollection<string> { "全部", ErrorLog.LevelP0, ErrorLog.LevelP1 };
        _ = LoadDataAsync();
    }

    public ObservableCollection<ErrorLog> Entries { get => _entries; set => SetProperty(ref _entries, value); }
    public int CurrentPage { get => _currentPage; set { SetProperty(ref _currentPage, value); UpdatePagingCommands(); } }
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / _pageSize));
    public string FilterLevel { get => _filterLevel; set => SetProperty(ref _filterLevel, value); }
    public DateTime? FilterFromDate { get => _filterFromDate; set => SetProperty(ref _filterFromDate, value); }
    public DateTime? FilterToDate { get => _filterToDate; set => SetProperty(ref _filterToDate, value); }
    public ObservableCollection<string> FilterLevels { get; }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页  共 {TotalCount} 条";

    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ApplyFilterCommand { get; }

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
            var (items, total) = await _errorLogService.GetPagedEntriesAsync(
                CurrentPage, _pageSize, FilterLevel, FilterFromDate, FilterToDate);
           Entries = new ObservableCollection<ErrorLog>(items);
           TotalCount = total;
            UpdatePagingCommands();
           StatusMessage = $"共 {total} 条记录";
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadDataAsync();
                return;
            }
        }
        catch (Exception ex) { StatusMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; OnPropertyChanged(nameof(PageInfo)); }
    }

    private async Task ClearAllAsync()
    {
        await _errorLogService.ClearAllAsync();
        await LoadDataAsync();
        StatusMessage = "日志已清空";
    }
}
