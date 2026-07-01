using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Win32;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 生命周期配置页面 ViewModel
/// 负责: Excel模板下载、文件选择、批量导入Aras、导入历史分页查询
/// 功能与对象类配置、属性配置完全一致，仅 AML 部分由调用方自行维护
/// </summary>
public class LifecycleConfigViewModel : ObservableObject
{
    private readonly ILifecycleImportService _importService;
    private readonly IErrorLogService _errorLogService;

    // ===== 文件选择 =====
    private string _selectedFilePath = "";

    // ===== 状态显示 =====
    private string _statusMessage = "";
    private string _errorMessage = "";

    // ===== 导入控制 =====
    private bool _isImporting;
    private bool _isLoading;
    private CancellationTokenSource? _cts;

    // ===== 导入进度 =====
    private ImportProgressInfo? _importProgress;

    // ===== 导入结果 =====
    private LifecycleImportResult? _lastResult;

    // ===== 导入模式 =====
    private string _importMode = "覆盖";

    // ===== 分页 =====
    private int _currentPage = 1;
    private const int PageSize = 20;
    private int _totalCount;

    public LifecycleConfigViewModel(
        ILifecycleImportService importService,
        IErrorLogService errorLogService)
    {
        _importService = importService;
        _errorLogService = errorLogService;
        HistoryRecords = new ObservableCollection<LifecycleImportLog>();

        // 命令初始化
        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());
        BrowseFileCommand = new RelayCommand(_ => BrowseFile());
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(),
            _ => !IsImporting && !string.IsNullOrWhiteSpace(SelectedFilePath));
        CancelImportCommand = new RelayCommand(_ => CancelImport(), _ => IsImporting);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1),
            _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1),
            _ => CurrentPage < TotalPages);

        _ = LoadHistoryAsync();
    }

    // ==================== 绑定属性 ====================

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (SetProperty(ref _selectedFilePath, value))
            {
                OnPropertyChanged(nameof(FileName));
                StatusMessage = string.IsNullOrEmpty(value) ? "" : $"已选择: {Path.GetFileName(value)}";
                (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string FileName =>
        string.IsNullOrEmpty(SelectedFilePath) ? "(未选择文件)" : Path.GetFileName(SelectedFilePath);

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public bool IsImporting
    {
        get => _isImporting;
        set
        {
            if (SetProperty(ref _isImporting, value))
            {
                (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public LifecycleImportResult? LastResult
    {
        get => _lastResult;
        set
        {
            if (SetProperty(ref _lastResult, value))
            {
                OnPropertyChanged(nameof(HasResult));
                OnPropertyChanged(nameof(HasFailures));
            }
        }
    }

    public bool HasResult => LastResult != null;
    public bool HasFailures => LastResult?.HasFailures ?? false;
    public ObservableCollection<LifecycleImportLog> HistoryRecords { get; }

    // ===== 导入模式 =====
    public string ImportMode { get => _importMode; set => SetProperty(ref _importMode, value); }
    public List<string> ImportModeOptions { get; } = ["新增", "覆盖"];

    // ===== 导入进度 =====
    public ImportProgressInfo? ImportProgress
    {
        get => _importProgress;
        set
        {
            if (SetProperty(ref _importProgress, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressText));
                OnPropertyChanged(nameof(ImportErrorCount));
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    public double ProgressPercentage => ImportProgress?.Percentage ?? 0;
    public string ProgressText => ImportProgress?.StatusText ?? "";
    public int ImportErrorCount => ImportProgress?.ErrorCount ?? 0;
    public bool IsProgressIndeterminate => ImportProgress == null && IsImporting;

    // ===== 分页 =====
    public int CurrentPage
    {
        get => _currentPage;
        set { if (SetProperty(ref _currentPage, value)) RefreshPagingCommands(); }
    }

    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / PageSize);

    public int TotalCount
    {
        get => _totalCount;
        set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); }
    }

    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";

    // ==================== 命令 ====================

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand CancelImportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    // ==================== 分页辅助 ====================

    private void RefreshPagingCommands()
    {
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
    }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > TotalPages) return;
        CurrentPage = page;
        await LoadHistoryAsync();
    }

    // ==================== 业务方法 ====================

    private void DownloadTemplate()
    {
        try
        {
            var dlg = new SaveFileDialog
            {
                Title = "保存生命周期配置模板",
                Filter = "Excel|*.xlsx",
                DefaultExt = ".xlsx",
                FileName = "生命周期配置模板.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            IsLoading = true;
            File.WriteAllBytes(dlg.FileName, _importService.GenerateTemplate());
            StatusMessage = $"模板已保存: {dlg.FileName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败: {ex.Message}";
            _ = _errorLogService.LogErrorAsync("生命周期配置-下载模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsLoading = false; }
    }

    private void BrowseFile()
    {
        var dlg = new OpenFileDialog
        {
            Title = "选择要导入的生命周期配置 Excel 文件",
            Filter = "Excel|*.xlsx;*.xls",
            CheckFileExists = true
        };
        if (dlg.ShowDialog() == true)
            SelectedFilePath = dlg.FileName;
    }

    private async Task ExecuteImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFilePath))
        {
            ErrorMessage = "请先选择要导入的 Excel 文件";
            return;
        }
        if (!File.Exists(SelectedFilePath))
        {
            ErrorMessage = "所选文件不存在，请重新选择";
            return;
        }

        IsImporting = true;
        ErrorMessage = "";
        ImportProgress = null;
        StatusMessage = "正在准备导入...";
        LastResult = null;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            var progress = new Progress<ImportProgressInfo>(p =>
            {
                ImportProgress = p;
                StatusMessage = p.StatusText;
            });

            var mode = ImportMode;
            var path = SelectedFilePath;

            LastResult = await Task.Run(
                () => _importService.ImportAsync(path, mode, progress, token),
                token);

            if (LastResult.IsSuccess)
            {
                StatusMessage = LastResult.HasFailures
                    ? $"导入完成（部分失败）: 生命周期{LastResult.Sheet1Count}/{LastResult.Sheet1Total}"
                    : $"导入成功: 生命周期{LastResult.Sheet1Count}条";
            }
            else
            {
                ErrorMessage = LastResult.ErrorMessage ?? "导入失败（未知错误）";
            }

            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "导入已取消";
            ErrorMessage = "导入已被用户取消，已完成的行已保存到 Aras 系统。";
            await _errorLogService.LogErrorAsync("生命周期配置-导入取消", "用户取消导入",
                ErrorLog.LevelP1, null);
            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("生命周期配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsImporting = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelImport()
    {
        try
        {
            _cts?.Cancel();
            StatusMessage = "正在取消导入...";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LifecycleConfig] 取消导入异常: {ex.Message}");
        }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, total) = await _importService.GetHistoryAsync(
                CurrentUserContext.CurrentUserId, CurrentPage, PageSize);
            HistoryRecords.Clear();
            foreach (var item in items) HistoryRecords.Add(item);
            TotalCount = total;
            RefreshPagingCommands();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载历史记录失败: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[LifecycleConfig] 加载历史失败: {ex.Message}");
        }
    }
}
