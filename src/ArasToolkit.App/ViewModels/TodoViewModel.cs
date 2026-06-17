using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using Microsoft.Win32;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 待办项目 ViewModel — 分页、CRUD、导入导出、到期高亮
/// </summary>
public class TodoViewModel : ObservableObject
{
    private readonly ITodoService _todoService;
    private readonly IErrorLogService _errorLogService;

    // ===== 分页字段 =====
    private int _currentPage = 1, _pageSize = 6, _totalCount;
    private ObservableCollection<PersonalTask> _entries = [];

    // ===== 筛选字段 =====
    private string _filterStatus = "全部";
    private string _filterProjectName = "全部";
    private string _searchKeyword = string.Empty;
    private DateTime? _filterCompletionDate;
    private DateTime? _filterDueDate;
    private PersonalTask? _detailItem;
    private bool _isDetailOpen;
    private int _dueTodayCount;
    private int _upcomingDueCount;
    private ObservableCollection<string> _projectNameOptions = ["全部"];

    // ===== 编辑字段 =====
    private bool _isEditing;
    private bool _isAdding;
    private PersonalTask _editingItem = new();
    private string _editDialogTitle = "新增待办项";

    // ===== 状态字段 =====
    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private int _selectedCount;
    private bool _isAllSelected;

    public TodoViewModel(ITodoService todoService, IErrorLogService errorLogService)
    {
        _todoService = todoService;
        _errorLogService = errorLogService;

        // 分页命令
        RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
        FirstPageCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); }, _ => CurrentPage > 1);
        PrevPageCommand = new RelayCommand(async _ => { CurrentPage--; await LoadDataAsync(); }, _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => { CurrentPage++; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);
        LastPageCommand = new RelayCommand(async _ => { CurrentPage = TotalPages; await LoadDataAsync(); }, _ => CurrentPage < TotalPages);

        // 筛选命令
        ApplyFilterCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); });
        SearchCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadDataAsync(); });
        ClearCompletionDateCommand = new RelayCommand(_ => { FilterCompletionDate = null; });
        ClearDueDateCommand = new RelayCommand(_ => { FilterDueDate = null; });

        // CRUD 命令
        AddCommand = new RelayCommand(_ => StartAdd());
        EditCommand = new RelayCommand(StartEdit, _ => !IsEditing);
        DeleteCommand = new RelayCommand(async p => await DeleteItemAsync(p?.ToString() ?? ""), _ => !IsEditing);
        SaveCommand = new RelayCommand(async _ => await SaveItemAsync());
        CancelCommand = new RelayCommand(_ => CancelEdit());

        // 导入/导出命令
        ImportExcelCommand = new RelayCommand(async _ => await ImportExcelAsync());
        ExportTemplateCommand = new RelayCommand(async _ => await ExportTemplateAsync());

        // 多选 / 批量删除命令
        SelectAllCommand = new RelayCommand(_ => ToggleSelectAll());
        BatchDeleteCommand = new RelayCommand(async _ => await BatchDeleteAsync(), _ => SelectedCount > 0);

            OpenDetailCommand = new RelayCommand(p => { if (p is PersonalTask item) { DetailItem = item; IsDetailOpen = true; } });
            CloseDetailCommand = new RelayCommand(_ => { IsDetailOpen = false; DetailItem = null; });

        // 状态选项
        StatusOptions = new ObservableCollection<string> { "全部", "未开始", "处理中", "延期", "已完成" };
        EditStatusOptions = new ObservableCollection<string> { "未开始", "处理中", "延期", "已完成" };

        // 初始加载
        _ = LoadDataAsync();
        _ = LoadProjectNamesAsync();
    }

    // ===== 集合属性 =====
    public ObservableCollection<PersonalTask> Entries { get => _entries; set => SetProperty(ref _entries, value); }
    public ObservableCollection<string> StatusOptions { get; }
    public ObservableCollection<string> EditStatusOptions { get; }

    // ===== 分页属性 =====
    public int CurrentPage { get => _currentPage; set { SetProperty(ref _currentPage, value); UpdatePagingCommands(); } }
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / _pageSize));
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页  共 {TotalCount} 条";
    public int DueTodayCount { get => _dueTodayCount; set => SetProperty(ref _dueTodayCount, value); }
    public int UpcomingDueCount { get => _upcomingDueCount; set { SetProperty(ref _upcomingDueCount, value); OnPropertyChanged(nameof(UpcomingDueHint)); } }

    // ===== 筛选属性 =====
    public string FilterStatus
    {
        get => _filterStatus;
        set { SetProperty(ref _filterStatus, value); CurrentPage = 1; _ = LoadDataAsync(); }
    }

    public string SearchKeyword
    {
        get => _searchKeyword;
        set { SetProperty(ref _searchKeyword, value); }
    }

    public DateTime? FilterCompletionDate
    {
        get => _filterCompletionDate;
        set
        {
            SetProperty(ref _filterCompletionDate, value);
            OnPropertyChanged(nameof(HasCompletionDateFilter));
            CurrentPage = 1;
            _ = LoadDataAsync();
        }
    }

    public DateTime? FilterDueDate
    {
        get => _filterDueDate;
        set
        {
            SetProperty(ref _filterDueDate, value);
            OnPropertyChanged(nameof(HasDueDateFilter));
            CurrentPage = 1;
            _ = LoadDataAsync();
        }
    }

    public bool HasCompletionDateFilter => FilterCompletionDate.HasValue;
    public bool HasDueDateFilter => FilterDueDate.HasValue;

    public string FilterProjectName
    {
        get => _filterProjectName;
        set { SetProperty(ref _filterProjectName, value); CurrentPage = 1; _ = LoadDataAsync(); }
    }

    public ObservableCollection<string> ProjectNameOptions
    {
        get => _projectNameOptions;
        set => SetProperty(ref _projectNameOptions, value);
    }

    // ===== 编辑属性 =====
    public bool IsEditing { get => _isEditing; set { SetProperty(ref _isEditing, value); UpdateCommandStates(); } }
    public bool IsAdding { get => _isAdding; set => SetProperty(ref _isAdding, value); }
    public PersonalTask EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string EditDialogTitle { get => _editDialogTitle; set => SetProperty(ref _editDialogTitle, value); }

    // ===== 状态属性 =====
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string DueTodayHint => DueTodayCount > 0 ? $"进入 {DueTodayCount}" : "";
    public string UpcomingDueHint => UpcomingDueCount > 0 ? $"即将到期 {UpcomingDueCount}" : "";

    /// <summary>已选中的条目数量</summary>
    public int SelectedCount
    {
        get => _selectedCount;
        set { SetProperty(ref _selectedCount, value); OnPropertyChanged(nameof(HasSelection)); (BatchDeleteCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    /// <summary>是否有选中项</summary>
    public bool HasSelection => SelectedCount > 0;

    /// <summary>是否全选</summary>
    public bool IsAllSelected
    {
        get => _isAllSelected;
        set => SetProperty(ref _isAllSelected, value);
    }

    // ===== 命令 =====
    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ClearCompletionDateCommand { get; }
    public ICommand ClearDueDateCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ImportExcelCommand { get; }
    public ICommand ExportTemplateCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand BatchDeleteCommand { get; }
    public ICommand OpenDetailCommand { get; }
    public ICommand CloseDetailCommand { get; }

    // ===== 数据加载 =====
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
        var (items, total) = await _todoService.GetPagedItemsAsync(
            CurrentPage, _pageSize,
            FilterStatus == "全部" ? null : FilterStatus,
            FilterProjectName == "全部" ? null : FilterProjectName,
            string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword,
            FilterCompletionDate,
            FilterDueDate);

            Entries = new ObservableCollection<PersonalTask>(items);
            TotalCount = total;
            UpdatePagingCommands();
            DueTodayCount = await _todoService.GetDueTodayCountAsync();
            UpcomingDueCount = await _todoService.GetUpcomingDueCountAsync();
            var filterDesc = FilterProjectName != "全部" ? $"{FilterProjectName} / {FilterStatus}" : FilterStatus;
            StatusMessage = $"{filterDesc}：共 {total} 条";
            OnPropertyChanged(nameof(PageInfo));
            OnPropertyChanged(nameof(DueTodayHint));

            // 页码溢出修正
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadDataAsync();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-加载列表", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadProjectNamesAsync()
    {
        try
        {
            var names = await _todoService.GetDistinctProjectNamesAsync();
            ProjectNameOptions = new ObservableCollection<string>(new[] { "全部" }.Concat(names));
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("Todo-项目列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private void UpdatePagingCommands()
    {
        (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(PageInfo));
    }

    private void UpdateCommandStates()
    {
        (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // ===== 多选操作 =====

    /// <summary>全选 / 取消全选</summary>
    private void ToggleSelectAll()
    {
        IsAllSelected = !IsAllSelected;
        foreach (var item in Entries)
            item.IsSelected = IsAllSelected;
        SelectedCount = IsAllSelected ? Entries.Count : 0;
    }

    /// <summary>批量删除选中项</summary>
    private async Task BatchDeleteAsync()
    {
        var selectedIds = Entries.Where(e => e.IsSelected).Select(e => e.Id).ToList();
        if (selectedIds.Count == 0) return;

        var result = MessageBox.Show(
            $"确定要删除选中的 {selectedIds.Count} 条任务吗？此操作不可撤销。",
            "批量删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var deleted = await _todoService.BatchDeleteAsync(selectedIds);
            StatusMessage = $"🗑️ 已批量删除 {deleted} 条任务";
            IsAllSelected = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ 批量删除失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-批量删除", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    // ===== 详情弹出窗 =====
    public PersonalTask? DetailItem
    {
        get => _detailItem;
        set => SetProperty(ref _detailItem, value);
    }

    public bool IsDetailOpen
    {
        get => _isDetailOpen;
        set => SetProperty(ref _isDetailOpen, value);
    }

    // ===== CRUD 操作 =====
    private void StartAdd()
    {
        IsAdding = true;
        IsEditing = true;
        EditDialogTitle = "➕ 新增待办项";
        EditingItem = new PersonalTask { Status = "未开始", CompletionPercent = 0 };
    }

    private void StartEdit(object? parameter)
    {
        if (parameter is PersonalTask item)
        {
            // 深拷贝一份用于编辑（避免直接修改列表项）
            EditingItem = new PersonalTask
            {
                Id = item.Id,
                TaskName = item.TaskName,
                Description = item.Description,
                ExpectedDate = item.ExpectedDate,
                StartDate = item.StartDate,
                CompletionDate = item.CompletionDate,
                ProjectName = item.ProjectName,
                Remarks = item.Remarks,
                CompletionPercent = item.CompletionPercent,
                Status = item.Status,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate
            };
            IsAdding = false;
            IsEditing = true;
            EditDialogTitle = "✏️ 编辑待办项";
        }
    }

    private async Task SaveItemAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingItem.TaskName))
        {
            StatusMessage = "❌ 任务名称不能为空！";
            return;
        }

        try
        {
            if (IsAdding)
            {
                await _todoService.AddItemAsync(EditingItem);
                StatusMessage = "✅ 新增成功";
            }
            else
            {
                await _todoService.UpdateItemAsync(EditingItem);
                StatusMessage = "✅ 保存成功";
            }

            IsEditing = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ 保存失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-保存", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            await LoadDataAsync();
        }
    }

    private void CancelEdit()
    {
        IsEditing = false;
        StatusMessage = "已取消编辑";
    }

    private async Task DeleteItemAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        var result = MessageBox.Show("确定要删除该待办项吗？", "确认删除",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _todoService.DeleteItemAsync(id);
            StatusMessage = "🗑️ 已删除";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"删除失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-删除", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task ImportExcelAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "选择待办项目Excel文件",
            Filter = "Excel文件|*.xlsx;*.xls|所有文件|*.*",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != true) return;

        IsLoading = true;
        StatusMessage = "正在导入Excel...";

        try
        {
            var imported = await _todoService.ImportFromExcelAsync(openFileDialog.FileName);
            StatusMessage = $"✅ 成功导入 {imported.Count} 条待办项";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ 导入失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-Excel导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExportTemplateAsync()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "导出待办项目模板",
            Filter = "Excel文件|*.xlsx",
            FileName = "待办项目导入模板.xlsx"
        };

        if (saveFileDialog.ShowDialog() != true) return;

        try
        {
            await _todoService.ExportTemplateAsync(saveFileDialog.FileName);
            StatusMessage = $"✅ 模板已导出到: {saveFileDialog.FileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ 导出失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Todo-Excel导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }
}
