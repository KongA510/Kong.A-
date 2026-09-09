using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>属性配置四段式工作台 ViewModel。</summary>
public sealed class PropertyConfigViewModel : ObservableObject, IDisposable
{
    private const int PageSize = 20;

    private readonly IPropertyImportService _importService;
    private readonly IErrorLogService _errorLogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IArasConnectionService _connectionService;
    private readonly DispatcherQueue? _dispatcherQueue;

    private string _selectedFilePath = string.Empty;
    private ArasItemTypeInfo? _selectedItemType;
    private string _itemTypeSearchText = string.Empty;
    private string _importMode = "覆盖";
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isImporting;
    private bool _initialized;
    private bool _disposed;
    private bool _reloadPending;
    private bool _isCancellationRequested;
    private bool _isFinalizing;
    private readonly Stopwatch _busyStopwatch = new();
    private DispatcherQueueTimer? _busyTimer;
    private PropertyImportPreview? _preview;
    private PropertyImportResult? _lastResult;
    private ImportProgressInfo? _importProgress;
    private CancellationTokenSource? _cancellationSource;
    private int _currentPage = 1;
    private int _totalCount;

    public PropertyConfigViewModel(
        IPropertyImportService importService,
        IErrorLogService errorLogService,
        IFileDialogService fileDialogService,
        IDialogService dialogService,
        IArasConnectionService connectionService)
    {
        _importService = importService;
        _errorLogService = errorLogService;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _connectionService = connectionService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _connectionService.ConnectionChanged += OnConnectionChanged;

        DownloadTemplateCommand = new RelayCommand(async _ => await DownloadTemplateAsync(), _ => !IsBusy);
        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => !IsBusy);
        RefreshItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        PrepareAmlCommand = new RelayCommand(async _ => await PrepareAmlAsync(),
            _ => CanPrepare);
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(),
            _ => CanExecuteImport);
        CancelImportCommand = new RelayCommand(_ => CancelImport(), _ => CanCancelImport);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync(), _ => !IsBusy);
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1),
            _ => CurrentPage > 1 && !IsBusy);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1),
            _ => CurrentPage < TotalPages && !IsBusy);
    }

    public ObservableCollection<ArasItemTypeInfo> ItemTypes { get; } = [];
    public ObservableCollection<ArasItemTypeInfo> FilteredItemTypes { get; } = [];
    public ObservableCollection<PropertyImportPreviewRow> PreviewRows { get; } = [];
    public ObservableCollection<PropertyImportLog> HistoryRecords { get; } = [];
    public IReadOnlyList<string> ImportModeOptions { get; } = ["覆盖", "新增"];

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        private set
        {
            if (!SetProperty(ref _selectedFilePath, value))
                return;
            OnPropertyChanged(nameof(FileName));
            OnPropertyChanged(nameof(HasSelectedFile));
            InvalidatePreparation();
            RefreshCommands();
        }
    }

    public string FileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "尚未选择模板"
        : Path.GetFileName(SelectedFilePath);
    public bool HasSelectedFile => !string.IsNullOrWhiteSpace(SelectedFilePath);

    public ArasItemTypeInfo? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value))
                return;
            InvalidatePreparation();
            OnPropertyChanged(nameof(SelectedObjectSummary));
            RefreshCommands();
        }
    }

    public string ItemTypeSearchText
    {
        get => _itemTypeSearchText;
        set
        {
            if (SetProperty(ref _itemTypeSearchText, value))
                ApplyItemTypeFilter(value);
        }
    }

    public string ImportMode
    {
        get => _importMode;
        set
        {
            if (!SetProperty(ref _importMode, value))
                return;
            InvalidatePreparation();
            OnPropertyChanged(nameof(ModeDescription));
            RefreshCommands();
        }
    }

    public string ModeDescription => ImportMode == "新增"
        ? "新增：对象类中已存在同名属性时停止预检，不做覆盖。"
        : "覆盖：以 source_id + 属性名称唯一匹配；命中后编辑，未命中则新增。";

    public string SelectedObjectSummary => SelectedItemType == null
        ? "尚未选择系统对象类"
        : $"目标 source_id：{SelectedItemType.DisplayName}";

    public PropertyImportPreview? Preview
    {
        get => _preview;
        private set
        {
            if (!SetProperty(ref _preview, value))
                return;
            OnPropertyChanged(nameof(HasPreview));
            OnPropertyChanged(nameof(IsPrepared));
            OnPropertyChanged(nameof(PreviewSummary));
            OnPropertyChanged(nameof(HasInvalidRows));
            RefreshCommands();
        }
    }

    public bool HasPreview => PreviewRows.Count > 0;
    public bool IsPrepared => Preview?.IsPrepared == true;
    public bool HasInvalidRows => Preview?.InvalidCount > 0;
    public string PreviewSummary => Preview?.Summary ?? "选择模板后将在此显示逐行校验结果";
    public bool CanPrepare => HasSelectedFile && SelectedItemType != null && !IsBusy;
    public bool CanExecuteImport => Preview?.CanImport == true && !IsBusy;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value))
                return;
            OnPropertyChanged(nameof(CanPrepare));
            OnPropertyChanged(nameof(CanExecuteImport));
            OnPropertyChanged(nameof(IsIdle));
            if (value)
            {
                _busyStopwatch.Restart();
                if (_busyTimer == null && _dispatcherQueue != null)
                {
                    _busyTimer = _dispatcherQueue.CreateTimer();
                    _busyTimer.Interval = TimeSpan.FromSeconds(1);
                    _busyTimer.Tick += OnBusyTimerTick;
                }
                _busyTimer?.Start();
            }
            else
            {
                _busyStopwatch.Stop();
                _busyTimer?.Stop();
            }
            OnPropertyChanged(nameof(BusyElapsedText));
            RefreshBusyPresentation();
            RefreshCommands();
        }
    }

    public bool IsIdle => !IsBusy;
    public string BusyElapsedText => $"已用时 {_busyStopwatch.Elapsed:mm\\:ss}";
    public bool CanCancelImport => IsImporting && !_isCancellationRequested && !_isFinalizing;
    public string CancelButtonText => _isFinalizing ? "正在保存对象类" : _isCancellationRequested ? "正在停止…" : "停止后续提交";
    public string BusyTitle => !IsImporting ? "正在处理，请稍候"
        : _isFinalizing ? "正在保存对象类"
        : _isCancellationRequested ? "正在停止后续提交"
        : ImportProgress?.Phase ?? "正在准备汇入";
    public string PreparationStageText => ImportProgress?.OverallTotal > 0 ? "✓ 预检" : "① 预检";
    public string SubmissionStageText => _isFinalizing
        ? _isCancellationRequested ? "已停止后续提交" : "② 提交结束"
        : "② 属性提交";
    public string SaveStageText => _isFinalizing ? "③ 保存中…" : "③ 保存对象类";

    private void OnBusyTimerTick(DispatcherQueueTimer sender, object args) => OnPropertyChanged(nameof(BusyElapsedText));

    private void RefreshBusyPresentation()
    {
        OnPropertyChanged(nameof(BusyTitle));
        OnPropertyChanged(nameof(CanCancelImport));
        OnPropertyChanged(nameof(CancelButtonText));
        OnPropertyChanged(nameof(PreparationStageText));
        OnPropertyChanged(nameof(SubmissionStageText));
        OnPropertyChanged(nameof(SaveStageText));
        OnPropertyChanged(nameof(IsProgressIndeterminate));
        (CancelImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public bool IsImporting
    {
        get => _isImporting;
        private set
        {
            if (!SetProperty(ref _isImporting, value))
                return;
            RefreshBusyPresentation();
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

    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public PropertyImportResult? LastResult
    {
        get => _lastResult;
        private set
        {
            if (!SetProperty(ref _lastResult, value))
                return;
            OnPropertyChanged(nameof(HasResult));
            OnPropertyChanged(nameof(HasFailures));
            OnPropertyChanged(nameof(ResultSummary));
            OnPropertyChanged(nameof(ObjectSaveSummary));
        }
    }

    public bool HasResult => LastResult != null;
    public bool HasFailures => LastResult?.HasFailures == true;
    public string ResultSummary => LastResult == null
        ? string.Empty
        : $"已提交 {LastResult.Sheet1Count}/{LastResult.Sheet1Total} · 新增 {LastResult.AddedCount} · 覆盖 {LastResult.UpdatedCount} · 失败 {LastResult.FailedRowCount} · 未提交 {LastResult.UnsubmittedCount}";
    public string ObjectSaveSummary => LastResult == null ? string.Empty
        : LastResult.ItemTypeSaved ? "✓ 对象类已保存"
        : LastResult.ItemTypeSaveAttempted ? "对象类保存失败，请查看详细日志并重新保存对象类。"
        : "尚未成功写入属性，无需保存对象类。";

    public ImportProgressInfo? ImportProgress
    {
        get => _importProgress;
        private set
        {
            if (!SetProperty(ref _importProgress, value))
                return;
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(ImportErrorCount));
            OnPropertyChanged(nameof(IsProgressIndeterminate));
            RefreshBusyPresentation();
        }
    }

    public double ProgressPercentage => ImportProgress?.Percentage ?? 0;
    public string ProgressText => ImportProgress?.StatusText ?? "正在准备逐条提交...";
    public int ImportErrorCount => ImportProgress?.ErrorCount ?? 0;
    public bool IsProgressIndeterminate => IsBusy && (!IsImporting || ImportProgress?.OverallTotal is null or 0 || _isFinalizing);

    public int CurrentPage
    {
        get => _currentPage;
        private set
        {
            if (SetProperty(ref _currentPage, value))
                RefreshPagingCommands();
        }
    }

    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / PageSize);
    public int TotalCount
    {
        get => _totalCount;
        private set
        {
            if (!SetProperty(ref _totalCount, value))
                return;
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(PageInfo));
        }
    }
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand RefreshItemTypesCommand { get; }
    public ICommand PrepareAmlCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand CancelImportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        _initialized = true;
        await LoadItemTypesAsync();
        await LoadHistoryAsync();
    }

    public void SelectItemTypeFromSearch(ArasItemTypeInfo itemType)
    {
        SelectedItemType = itemType;
        ItemTypeSearchText = itemType.DisplayName;
    }

    private async Task DownloadTemplateAsync()
    {
        try
        {
            ClearMessages();
            var filePath = await _fileDialogService.PickSaveFileAsync("属性配置模板.xlsx", ".xlsx");
            if (filePath == null)
                return;

            IsBusy = true;
            StatusMessage = "正在生成模板…";
            var template = await Task.Run(() => _importService.GenerateTemplate());
            await File.WriteAllBytesAsync(filePath, template);
            StatusMessage = $"模板已保存：{filePath}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-下载模板", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BrowseFileAsync()
    {
        try
        {
            ClearMessages();
            var filePath = await _fileDialogService.PickOpenFileAsync(
                "选择属性配置模板", ".xlsx");
            if (filePath == null)
                return;

            SelectedFilePath = filePath;
            IsBusy = true;
            StatusMessage = "正在读取并校验模板...";
            SetPreview(await Task.Run(() => _importService.PreviewAsync(filePath)));
            StatusMessage = PreviewSummary;
        }
        catch (Exception ex)
        {
            SetPreview(null);
            ErrorMessage = $"模板读取失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-选择模板", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadItemTypesAsync()
    {
        if (!_connectionService.IsConnected)
        {
            ItemTypes.Clear();
            FilteredItemTypes.Clear();
            SelectedItemType = null;
            StatusMessage = "尚未连接 Aras；连接当前用户的默认配置后将自动加载系统对象类。";
            return;
        }

        if (IsBusy)
        {
            _reloadPending = true;
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            StatusMessage = "正在从 Aras 加载系统已有对象类...";
            var itemTypes = await Task.Run(() => _importService.GetItemTypesAsync());
            ItemTypes.Clear();
            foreach (var itemType in itemTypes)
                ItemTypes.Add(itemType);
            ApplyItemTypeFilter(ItemTypeSearchText);
            StatusMessage = $"已加载 {ItemTypes.Count} 个系统对象类，请选择本次导入目标。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"对象类加载失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-加载对象类", ex);
        }
        finally
        {
            IsBusy = false;
            if (_reloadPending && _connectionService.IsConnected && !_disposed)
            {
                _reloadPending = false;
                await LoadItemTypesAsync();
            }
        }
    }

    private async Task PrepareAmlAsync()
    {
        if (SelectedItemType == null || string.IsNullOrWhiteSpace(SelectedFilePath))
            return;

        try
        {
            IsBusy = true;
            ClearMessages();
            LastResult = null;
            ImportProgress = null;
            _isFinalizing = false;
            var filePath = SelectedFilePath;
            var target = SelectedItemType;
            var mode = ImportMode;
            StatusMessage = $"正在为 {SelectedItemType.DisplayName} 解析引用并组装 AML...";
            var preview = await Task.Run(() => _importService.PrepareAsync(
                filePath, target.Id, target.Name, mode));
            SetPreview(preview);
            StatusMessage = preview.CanImport
                ? $"AML 已组装完成：{preview.ValidCount} 行可逐条提交。"
                : $"AML 组装完成，但有 {preview.InvalidCount} 行需要修正。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"AML 组装失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-组装AML", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteImportAsync()
    {
        if (IsBusy || SelectedItemType == null || Preview?.CanImport != true)
            return;

        try
        {
            IsBusy = true;
            var filePath = SelectedFilePath;
            var target = SelectedItemType;
            var mode = ImportMode;
            var confirmed = await _dialogService.ConfirmAsync(
                "确认逐条写入 Aras",
                $"目标对象类：{target.DisplayName}\n模式：{mode}\n属性：{Preview.Rows.Count} 条\n\n属性逐条提交后会自动保存目标对象类。停止不会回滚已提交的属性，仍会完成这些属性的对象类保存。是否继续？",
                "开始逐条提交", "返回检查");
            if (!confirmed)
                return;

            _isCancellationRequested = false;
            _isFinalizing = false;
            IsImporting = true;
            ClearMessages();
            LastResult = null;
            ImportProgress = null;
            foreach (var row in PreviewRows)
                row.SubmitStatus = "待提交";

            _cancellationSource = new CancellationTokenSource();
            var token = _cancellationSource.Token;
            var progress = new Progress<ImportProgressInfo>(UpdateProgress);
            // IOM 请求为同步调用；整个导入放到后台，进度通过 UI 上创建的 Progress 回送。
            LastResult = await Task.Run(() => _importService.ImportAsync(
                filePath,
                target.Id,
                target.Name,
                mode,
                progress,
                token));

            ApplyFinalRowStatuses(LastResult);
            if (LastResult.IsSuccess)
            {
                StatusMessage = LastResult.HasFailures
                    ? $"逐条提交结束，存在失败：{ResultSummary}"
                    : $"导入完成，对象类已保存：{ResultSummary}";
            }
            else
            {
                StatusMessage = string.Empty;
                ErrorMessage = LastResult.ErrorMessage ?? "导入未完成。";
            }

            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            ErrorMessage = "导入已取消；取消前成功提交的属性已保存在 Aras。";
            await LogViewModelErrorAsync("属性配置-取消导入",
                new OperationCanceledException("用户取消属性配置导入"));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-导入", ex);
        }
        finally
        {
            IsImporting = false;
            _isCancellationRequested = false;
            _isFinalizing = false;
            IsBusy = false;
            _cancellationSource?.Dispose();
            _cancellationSource = null;
        }
    }

    private void UpdateProgress(ImportProgressInfo progress)
    {
        // 后台排队的进度不能覆盖最终结果或已卸载的页面。
        if (_disposed || !IsImporting || LastResult != null)
            return;
        if (progress is PropertyImportProgressInfo { IsFinalizing: true })
            _isFinalizing = true;
        ImportProgress = progress;
        StatusMessage = progress.StatusText;
        if (progress is PropertyImportProgressInfo rowProgress && rowProgress.ExcelRowNumber > 0)
        {
            var current = PreviewRows.FirstOrDefault(row => row.ExcelRowNumber == rowProgress.ExcelRowNumber);
            if (current != null)
                current.SubmitStatus = rowProgress.SubmitStatus;
        }
    }

    private void ApplyFinalRowStatuses(PropertyImportResult result)
    {
        foreach (var row in PreviewRows)
        {
            row.SubmitStatus = result.RowStatuses.GetValueOrDefault(row.ExcelRowNumber, "未提交");
        }
    }

    private void CancelImport()
    {
        try
        {
            if (!CanCancelImport)
                return;
            _isCancellationRequested = true;
            _cancellationSource?.Cancel();
            RefreshBusyPresentation();
            StatusMessage = "正在停止后续提交；当前请求结束后会保存已提交属性的对象类…";
        }
        catch (Exception ex)
        {
            _ = LogViewModelErrorAsync("属性配置-请求取消", ex);
        }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, total) = await _importService.GetHistoryAsync(
                CurrentUserContext.CurrentUserId, CurrentPage, PageSize);
            HistoryRecords.Clear();
            foreach (var item in items)
                HistoryRecords.Add(item);
            TotalCount = total;
            RefreshPagingCommands();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入历史加载失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-加载历史", ex);
        }
    }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > TotalPages)
            return;
        CurrentPage = page;
        await LoadHistoryAsync();
    }

    private void SetPreview(PropertyImportPreview? preview)
    {
        PreviewRows.Clear();
        if (preview != null)
        {
            foreach (var row in preview.Rows)
                PreviewRows.Add(row);
        }
        Preview = preview;
        OnPropertyChanged(nameof(HasPreview));
        OnPropertyChanged(nameof(PreviewSummary));
        OnPropertyChanged(nameof(HasInvalidRows));
    }

    private void InvalidatePreparation()
    {
        if (Preview == null)
            return;
        Preview.IsPrepared = false;
        foreach (var row in PreviewRows)
        {
            row.AmlPreview = string.Empty;
            row.PlannedAction = row.IsValid ? "待组装" : "校验失败";
            row.SubmitStatus = "待提交";
        }
        OnPropertyChanged(nameof(IsPrepared));
        OnPropertyChanged(nameof(PreviewSummary));
        RefreshCommands();
    }

    private void ApplyItemTypeFilter(string? searchText)
    {
        FilteredItemTypes.Clear();
        if (string.IsNullOrWhiteSpace(searchText))
            return;

        var terms = searchText.Split(' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var itemType in ItemTypes.Where(item => terms.All(term =>
                     item.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     item.Label.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     item.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))).Take(30))
        {
            FilteredItemTypes.Add(itemType);
        }
    }

    private void OnConnectionChanged()
    {
        if (_disposed || _dispatcherQueue == null)
            return;
        if (_dispatcherQueue.HasThreadAccess)
            _ = RefreshForConnectionChangeAsync();
        else
            _dispatcherQueue.TryEnqueue(() => _ = RefreshForConnectionChangeAsync());
    }

    private async Task RefreshForConnectionChangeAsync()
    {
        if (!_initialized || _disposed)
            return;
        if (!_connectionService.IsConnected)
        {
            ItemTypes.Clear();
            FilteredItemTypes.Clear();
            SelectedItemType = null;
            StatusMessage = "Aras 连接已断开；恢复默认连接后将自动重新加载对象类。";
            return;
        }
        await LoadItemTypesAsync();
    }

    private void ClearMessages()
    {
        StatusMessage = string.Empty;
        ErrorMessage = string.Empty;
    }

    private async Task LogViewModelErrorAsync(string functionName, Exception exception)
    {
        try
        {
            await _errorLogService.LogErrorAsync(functionName, exception.Message,
                ErrorLog.LevelP1, exception.StackTrace);
        }
        catch
        {
            // 错误日志失败不能覆盖原业务异常。
        }
    }

    private void RefreshCommands()
    {
        (DownloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshItemTypesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrepareAmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshHistoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        RefreshPagingCommands();
    }

    private void RefreshPagingCommands()
    {
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _busyTimer?.Stop();
        if (_busyTimer != null)
            _busyTimer.Tick -= OnBusyTimerTick;
        _connectionService.ConnectionChanged -= OnConnectionChanged;
        _cancellationSource?.Cancel();
        _cancellationSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
