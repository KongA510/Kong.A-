using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 更新日志ViewModel — 分页 + 类型筛选 + 版本 & 统计卡片
/// </summary>
public class ChangelogViewModel : ObservableObject
{
    private readonly IChangelogService _changelogService;
    private ObservableCollection<Changelog> _entries = [];
    private int _currentPage = 1, _pageSize = 10, _totalCount;
    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private string _currentVersion = string.Empty;
    private string _filterType = "全部";
    private int _newCount, _fixCount, _optimizeCount;

    public ChangelogViewModel(IChangelogService changelogService)
    {
        _changelogService = changelogService;
        CurrentVersion = _changelogService.GetCurrentVersion();
        RefreshCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); });
        FirstPageCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); }, _ => CurrentPage > 1);
        PrevPageCommand = new RelayCommand(async _ => { CurrentPage--; await LoadDataAsync(); }, _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => { CurrentPage++; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(async _ => { CurrentPage = TotalPages; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        _ = LoadDataAsync();
    }

    public ObservableCollection<Changelog> Entries { get => _entries; set => SetProperty(ref _entries, value); }
    public int CurrentPage { get => _currentPage; set { SetProperty(ref _currentPage, value); UpdatePagingCommands(); } }
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / _pageSize));
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页  共 {TotalCount} 条";
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public string CurrentVersion { get => _currentVersion; set => SetProperty(ref _currentVersion, value); }

    public int NewCount { get => _newCount; set => SetProperty(ref _newCount, value); }
    public int FixCount { get => _fixCount; set => SetProperty(ref _fixCount, value); }
    public int OptimizeCount { get => _optimizeCount; set => SetProperty(ref _optimizeCount, value); }

    public ObservableCollection<string> FilterTypes { get; } = ["全部", "新增", "修复", "优化", "移除"];

    public string FilterType
    {
        get => _filterType;
        set { SetProperty(ref _filterType, value); CurrentPage = 1; _ = LoadDataAsync(); }
    }

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
            var allItems = await _changelogService.GetAllEntriesAsync();

            var filtered = FilterType == "全部"
                ? allItems
                : allItems.Where(e => e.Type == FilterType).ToList();

            TotalCount = filtered.Count;

            NewCount = allItems.Count(e => e.Type == "新增");
            FixCount = allItems.Count(e => e.Type == "修复");
            OptimizeCount = allItems.Count(e => e.Type == "优化");

            UpdatePagingCommands();
            var paged = filtered.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize).ToList();
            Entries = new ObservableCollection<Changelog>(paged);
            StatusMessage = $"共 {TotalCount} 条记录";
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadDataAsync();
            }
        }
        catch (Exception ex) { StatusMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; OnPropertyChanged(nameof(PageInfo)); }
    }
}
