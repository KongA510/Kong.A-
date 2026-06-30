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
/// 对象类配置页面 ViewModel
/// 负责: Excel模板下载、文件选择、批量导入Aras、导入历史查询
/// </summary>
public class ObjectClassConfigViewModel : ObservableObject
{
    private readonly IObjectClassImportService _importService;
    private readonly IErrorLogService _errorLogService;

    // ===== 文件选择 =====
    private string _selectedFilePath = "";

    // ===== 状态显示 =====
    private string _statusMessage = "";
    private string _errorMessage = "";

    // ===== 导入控制 =====
    private bool _isImporting;
    private bool _isLoading;
    private CancellationTokenSource? _cts;                          // 取消令牌源

    // ===== 导入进度 =====
    private ImportProgressInfo? _importProgress;

    // ===== 导入结果 =====
    private ObjectClassImportResult? _lastResult;

    // ===== 导入模式 =====
    private string _importMode = "覆盖";

    // ===== 分页 =====
    private int _currentPage = 1;
    private const int PageSize = 20;
    private int _totalCount;

    public ObjectClassConfigViewModel(
        IObjectClassImportService importService,
        IErrorLogService errorLogService)
    {
        _importService = importService;
        _errorLogService = errorLogService;
        HistoryRecords = new ObservableCollection<ObjectClassImportLog>();

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

        // 初始加载
        _ = LoadHistoryAsync();
    }

    // ==================== 绑定属性 ====================

    /// <summary>选中的 Excel 文件完整路径</summary>
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (SetProperty(ref _selectedFilePath, value))
            {
                OnPropertyChanged(nameof(FileName));
                StatusMessage = string.IsNullOrEmpty(value)
                    ? ""
                    : $"已选择: {Path.GetFileName(value)}";
                // 文件变更后刷新执行按钮状态
                (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>文件名（仅显示用，不含路径）</summary>
    public string FileName =>
        string.IsNullOrEmpty(SelectedFilePath)
            ? "(未选择文件)"
            : Path.GetFileName(SelectedFilePath);

    /// <summary>状态提示消息</summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>错误消息</summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>是否正在导入中</summary>
    public bool IsImporting
    {
        get => _isImporting;
        set
        {
            if (SetProperty(ref _isImporting, value))
            {
                // 状态变更时刷新命令可用性
                (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    /// <summary>是否正在加载（模板下载等短暂操作）</summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>导入结果汇总</summary>
    public ObjectClassImportResult? LastResult
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

    /// <summary>是否有导入结果可显示</summary>
    public bool HasResult => LastResult != null;

    /// <summary>导入结果中是否有失败行</summary>
    public bool HasFailures => LastResult?.HasFailures ?? false;

    /// <summary>导入历史记录列表</summary>
    public ObservableCollection<ObjectClassImportLog> HistoryRecords { get; }

    // ===== 导入模式 =====

    /// <summary>当前导入模式: "新增" 或 "覆盖"</summary>
    public string ImportMode
    {
        get => _importMode;
        set => SetProperty(ref _importMode, value);
    }

    /// <summary>导入模式下拉选项</summary>
    public List<string> ImportModeOptions { get; } = ["新增", "覆盖"];

    // ===== 导入进度 =====

    /// <summary>
    /// 当前导入进度信息（null = 未开始/已完成）
    /// Progress&lt;T&gt; 自动将回调封送到 UI 线程，无需手动 Dispatcher.Invoke
    /// </summary>
    public ImportProgressInfo? ImportProgress
    {
        get => _importProgress;
        set
        {
            if (SetProperty(ref _importProgress, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressText));
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    /// <summary>进度百分比（0-100），绑定到 ProgressBar.Value</summary>
    public double ProgressPercentage => ImportProgress?.Percentage ?? 0;

    /// <summary>进度状态文本，如 "对象类覆盖: 5/20 — Part_BOM"</summary>
    public string ProgressText => ImportProgress?.StatusText ?? "";

    /// <summary>
    /// 进度条是否处于不确定模式（true=动画循环, false=确定百分比）
    /// 初始化阶段（无 ImportProgress）使用不确定模式
    /// </summary>
    public bool IsProgressIndeterminate => ImportProgress == null && IsImporting;

    // ===== 分页 =====

    /// <summary>当前页码</summary>
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
                RefreshPagingCommands();
        }
    }

    /// <summary>总页数（计算值）</summary>
    public int TotalPages =>
        _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / PageSize);

    /// <summary>总记录数</summary>
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            SetProperty(ref _totalCount, value);
            OnPropertyChanged(nameof(TotalPages));
        }
    }

    /// <summary>分页信息文本</summary>
    public string PageInfo =>
        $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";

    // ==================== 命令 ====================

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand CancelImportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    // ==================== 分页辅助方法 ====================

    /// <summary>刷新分页按钮可用性 + 页面信息显示</summary>
    private void RefreshPagingCommands()
    {
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
    }

    /// <summary>跳转到指定页码</summary>
    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > TotalPages) return;
        CurrentPage = page;
        await LoadHistoryAsync();
    }

    // ==================== 业务方法 ====================

    /// <summary>
    /// 下载 Excel 模板
    /// 弹出保存文件对话框，调用服务生成模板字节并写入磁盘
    /// </summary>
    private void DownloadTemplate()
    {
        try
        {
            var dlg = new SaveFileDialog
            {
                Title = "保存模板",
                Filter = "Excel|*.xlsx",
                DefaultExt = ".xlsx",
                FileName = "对象类配置模板.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            IsLoading = true;
            var templateBytes = _importService.GenerateTemplate();
            File.WriteAllBytes(dlg.FileName, templateBytes);
            StatusMessage = $"模板已保存: {dlg.FileName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败: {ex.Message}";
            _ = _errorLogService.LogErrorAsync("对象类配置-下载模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 选择 Excel 文件
    /// 弹出打开文件对话框，仅支持 .xlsx/.xls 格式
    /// </summary>
    private void BrowseFile()
    {
        var dlg = new OpenFileDialog
        {
            Title = "选择要导入的 Excel 文件",
            Filter = "Excel|*.xlsx;*.xls",
            CheckFileExists = true
        };
        if (dlg.ShowDialog() == true)
            SelectedFilePath = dlg.FileName;
    }

    /// <summary>
    /// 执行导入 — 后台线程运行，UI 线程报告进度
    ///
    /// 关键设计:
    /// - 使用 Task.Run 将耗时操作放到后台线程，避免阻塞 UI
    /// - Progress&lt;T&gt; 在构造时捕获 SynchronizationContext，自动将回调封送到 UI 线程
    /// - CancellationTokenSource 支持用户随时取消长时间运行的导入
    /// - 导入完成后自动刷新历史记录
    /// </summary>
    private async Task ExecuteImportAsync()
    {
        // 前置验证
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

        // 初始化状态
        IsImporting = true;
        ErrorMessage = "";
        ImportProgress = null;                  // 先显示不确定进度条
        StatusMessage = "正在准备导入...";
        LastResult = null;

        // 创建取消令牌
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            // Progress<T> 在 UI 线程构造 → 回调自动在 UI 线程执行
            var progress = new Progress<ImportProgressInfo>(p =>
            {
                ImportProgress = p;
                StatusMessage = p.StatusText;
            });

            var mode = ImportMode;              // 捕获当前值，避免闭包问题
            var path = SelectedFilePath;

            // Task.Run 将耗时的同步/异步混合调用放到后台线程执行
            LastResult = await Task.Run(
                () => _importService.ImportAsync(path, mode, progress, token),
                token);

            // 显示导入结果
            if (LastResult.IsSuccess)
            {
                StatusMessage = LastResult.HasFailures
                    ? $"导入完成（部分失败）: 对象类{LastResult.Sheet1Count}/{LastResult.Sheet1Total} | 关系类{LastResult.Sheet2Count}/{LastResult.Sheet2Total}"
                    : $"导入成功: 对象类{LastResult.Sheet1Count}条 / 关系类{LastResult.Sheet2Count}条";
            }
            else
            {
                ErrorMessage = LastResult.ErrorMessage ?? "导入失败（未知错误）";
            }

            // 刷新历史记录
            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "导入已取消";
            ErrorMessage = "导入已被用户取消，已完成的行已保存到 Aras 系统。";
            await _errorLogService.LogErrorAsync("对象类配置-导入取消", "用户取消导入",
                ErrorLog.LevelP1, null);
            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsImporting = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// 取消正在执行的导入操作
    /// 通过 CancellationTokenSource.Cancel() 发出取消信号，
    /// 服务层在每个行处理循环中检查并抛出 OperationCanceledException
    /// </summary>
    private void CancelImport()
    {
        try
        {
            _cts?.Cancel();
            StatusMessage = "正在取消导入...";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ObjectClassConfig] 取消导入异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载导入历史记录（分页）
    /// 仅查询当前用户的记录，按创建时间倒序排列
    /// </summary>
    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, total) = await _importService.GetHistoryAsync(
                CurrentUserContext.CurrentUserId,
                CurrentPage,
                PageSize);

            HistoryRecords.Clear();
            foreach (var item in items)
                HistoryRecords.Add(item);

            TotalCount = total;
            RefreshPagingCommands();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载历史记录失败: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[ObjectClassConfig] 加载历史失败: {ex.Message}");
        }
    }
}
