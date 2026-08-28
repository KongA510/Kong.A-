using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Win32;

namespace ArasToolkit.App.ViewModels;

/// <summary>WPF 版属性配置四段式导入工作台。</summary>
public sealed class PropertyConfigViewModel : ObservableObject, IDisposable
{
    private const int PageSize = 20;
    private readonly IPropertyImportService _importService;
    private readonly IErrorLogService _errorLogService;
    private readonly IArasConnectionService _connectionService;

    private string _selectedFilePath = string.Empty;
    private ArasItemTypeInfo? _selectedItemType;
    private string _importMode = "覆盖";
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isImporting;
    private bool _initialized;
    private bool _disposed;
    private PropertyImportPreview? _preview;
    private PropertyImportResult? _lastResult;
    private ImportProgressInfo? _importProgress;
    private CancellationTokenSource? _cancellationSource;
    private int _currentPage = 1;
    private int _totalCount;

    public PropertyConfigViewModel(
        IPropertyImportService importService,
        IErrorLogService errorLogService,
        IArasConnectionService connectionService)
    {
        _importService = importService;
        _errorLogService = errorLogService;
        _connectionService = connectionService;
        _connectionService.ConnectionChanged += OnConnectionChanged;

        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate(), _ => !IsBusy);
        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => !IsBusy);
        RefreshItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        PrepareAmlCommand = new RelayCommand(async _ => await PrepareAmlAsync(), _ => CanPrepare);
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(), _ => CanExecuteImport);
        CancelImportCommand = new RelayCommand(_ => CancelImport(), _ => IsImporting);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync(), _ => !IsBusy);
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1),
            _ => CurrentPage > 1 && !IsBusy);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1),
            _ => CurrentPage < TotalPages && !IsBusy);
    }

    public ObservableCollection<ArasItemTypeInfo> ItemTypes { get; } = [];
    public ObservableCollection<PropertyImportPreviewRow> PreviewRows { get; } = [];
    public ObservableCollection<PropertyImportLog> HistoryRecords { get; } = [];
    public IReadOnlyList<string> ImportModeOptions { get; } = ["覆盖", "新增"];

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        private set
        {
            if (!SetProperty(ref _selectedFilePath, value)) return;
            OnPropertyChanged(nameof(FileName));
            InvalidatePreparation();
            RefreshCommands();
        }
    }
    public string FileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "尚未选择模板"
        : Path.GetFileName(SelectedFilePath);

    public ArasItemTypeInfo? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value)) return;
            InvalidatePreparation();
            RefreshCommands();
        }
    }

    public string ImportMode
    {
        get => _importMode;
        set
        {
            if (!SetProperty(ref _importMode, value)) return;
            InvalidatePreparation();
            OnPropertyChanged(nameof(ModeDescription));
            RefreshCommands();
        }
    }

    public string ModeDescription => ImportMode == "新增"
        ? "新增模式：同名属性会在预检阶段被阻止。"
        : "覆盖模式：source_id + name 唯一匹配，命中编辑、未命中新增。";

    public PropertyImportPreview? Preview
    {
        get => _preview;
        private set
        {
            if (!SetProperty(ref _preview, value)) return;
            OnPropertyChanged(nameof(HasPreview));
            OnPropertyChanged(nameof(PreviewSummary));
            OnPropertyChanged(nameof(IsPrepared));
            RefreshCommands();
        }
    }
    public bool HasPreview => PreviewRows.Count > 0;
    public bool IsPrepared => Preview?.IsPrepared == true;
    public string PreviewSummary => Preview?.Summary ?? "选择模板后显示逐行校验结果";
    public bool CanPrepare => !string.IsNullOrWhiteSpace(SelectedFilePath) && SelectedItemType != null && !IsBusy;
    public bool CanExecuteImport => Preview?.CanImport == true && !IsBusy;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            RefreshCommands();
        }
    }

    public bool IsImporting
    {
        get => _isImporting;
        private set
        {
            if (!SetProperty(ref _isImporting, value)) return;
            (CancelImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }

    public PropertyImportResult? LastResult
    {
        get => _lastResult;
        private set
        {
            if (!SetProperty(ref _lastResult, value)) return;
            OnPropertyChanged(nameof(HasResult));
            OnPropertyChanged(nameof(HasFailures));
            OnPropertyChanged(nameof(ResultSummary));
        }
    }
    public bool HasResult => LastResult != null;
    public bool HasFailures => LastResult?.HasFailures == true;
    public string ResultSummary => LastResult == null ? string.Empty
        : $"成功 {LastResult.Sheet1Count}/{LastResult.Sheet1Total} · 新增 {LastResult.AddedCount} · 覆盖 {LastResult.UpdatedCount} · 失败 {LastResult.Sheet1Failed}";

    public ImportProgressInfo? ImportProgress
    {
        get => _importProgress;
        private set
        {
            if (!SetProperty(ref _importProgress, value)) return;
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(ImportErrorCount));
            OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }
    public double ProgressPercentage => ImportProgress?.Percentage ?? 0;
    public string ProgressText => ImportProgress?.StatusText ?? "正在准备逐条提交...";
    public int ImportErrorCount => ImportProgress?.ErrorCount ?? 0;
    public bool IsProgressIndeterminate => IsImporting && ImportProgress == null;

    public int CurrentPage
    {
        get => _currentPage;
        private set { if (SetProperty(ref _currentPage, value)) RefreshPagingCommands(); }
    }
    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / PageSize);
    public int TotalCount
    {
        get => _totalCount;
        private set
        {
            if (!SetProperty(ref _totalCount, value)) return;
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
        if (_initialized) return;
        _initialized = true;
        await LoadItemTypesAsync();
        await LoadHistoryAsync();
    }

    private void DownloadTemplate()
    {
        try
        {
            ClearMessages();
            var dialog = new SaveFileDialog
            {
                Title = "保存属性配置模板",
                Filter = "Excel 工作簿|*.xlsx",
                DefaultExt = ".xlsx",
                FileName = "属性配置模板.xlsx"
            };
            if (dialog.ShowDialog() != true) return;
            IsBusy = true;
            File.WriteAllBytes(dialog.FileName, _importService.GenerateTemplate());
            StatusMessage = $"模板已保存：{dialog.FileName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败：{ex.Message}";
            _ = LogViewModelErrorAsync("属性配置-下载模板", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task BrowseFileAsync()
    {
        try
        {
            ClearMessages();
            var dialog = new OpenFileDialog
            {
                Title = "选择属性配置模板",
                Filter = "Excel 工作簿|*.xlsx",
                CheckFileExists = true
            };
            if (dialog.ShowDialog() != true) return;
            SelectedFilePath = dialog.FileName;
            IsBusy = true;
            StatusMessage = "正在读取并校验模板...";
            SetPreview(await _importService.PreviewAsync(dialog.FileName));
            StatusMessage = PreviewSummary;
        }
        catch (Exception ex)
        {
            SetPreview(null);
            ErrorMessage = $"模板读取失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-选择模板", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadItemTypesAsync()
    {
        if (!_connectionService.IsConnected)
        {
            ItemTypes.Clear();
            SelectedItemType = null;
            StatusMessage = "尚未连接 Aras；请先恢复当前用户的默认连接。";
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            var itemTypes = await _importService.GetItemTypesAsync();
            ItemTypes.Clear();
            foreach (var itemType in itemTypes) ItemTypes.Add(itemType);
            StatusMessage = $"已加载 {ItemTypes.Count} 个系统对象类。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"对象类加载失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-加载对象类", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task PrepareAmlAsync()
    {
        if (SelectedItemType == null) return;
        try
        {
            IsBusy = true;
            ClearMessages();
            SetPreview(await _importService.PrepareAsync(
                SelectedFilePath, SelectedItemType.Id, SelectedItemType.Name, ImportMode));
            StatusMessage = Preview?.CanImport == true
                ? $"AML 已组装完成：{Preview.ValidCount} 行可提交。"
                : $"AML 组装完成，但有 {Preview?.InvalidCount ?? 0} 行需要修正。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"AML 组装失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-组装AML", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task ExecuteImportAsync()
    {
        if (SelectedItemType == null || Preview?.CanImport != true) return;
        var answer = MessageBox.Show(
            $"目标对象类：{SelectedItemType.DisplayName}\n模式：{ImportMode}\n属性：{Preview.Rows.Count} 条\n\n每条成功请求会立即保存，取消不会回滚已成功属性。是否继续？",
            "确认逐条写入 Aras", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (answer != MessageBoxResult.Yes) return;

        try
        {
            IsBusy = true;
            IsImporting = true;
            ClearMessages();
            LastResult = null;
            ImportProgress = null;
            _cancellationSource = new CancellationTokenSource();
            var progress = new Progress<ImportProgressInfo>(info =>
            {
                ImportProgress = info;
                StatusMessage = info.StatusText;
            });
            LastResult = await _importService.ImportAsync(
                SelectedFilePath, SelectedItemType.Id, SelectedItemType.Name, ImportMode,
                progress, _cancellationSource.Token);
            if (LastResult.IsSuccess) StatusMessage = $"逐条提交完成：{ResultSummary}";
            else ErrorMessage = LastResult.ErrorMessage ?? "导入未完成。";
            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败：{ex.Message}";
            await LogViewModelErrorAsync("属性配置-导入", ex);
        }
        finally
        {
            IsImporting = false;
            IsBusy = false;
            _cancellationSource?.Dispose();
            _cancellationSource = null;
        }
    }

    private void CancelImport()
    {
        try
        {
            _cancellationSource?.Cancel();
            StatusMessage = "正在停止后续提交；已成功属性不会回滚...";
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
            foreach (var item in items) HistoryRecords.Add(item);
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
        if (page < 1 || page > TotalPages) return;
        CurrentPage = page;
        await LoadHistoryAsync();
    }

    private void SetPreview(PropertyImportPreview? preview)
    {
        PreviewRows.Clear();
        if (preview != null)
            foreach (var row in preview.Rows) PreviewRows.Add(row);
        Preview = preview;
        OnPropertyChanged(nameof(HasPreview));
        OnPropertyChanged(nameof(PreviewSummary));
    }

    private void InvalidatePreparation()
    {
        if (Preview == null) return;
        Preview.IsPrepared = false;
        foreach (var row in PreviewRows)
        {
            row.AmlPreview = string.Empty;
            row.PlannedAction = row.IsValid ? "待组装" : "校验失败";
            row.SubmitStatus = "待提交";
        }
        OnPropertyChanged(nameof(IsPrepared));
        RefreshCommands();
    }

    private void OnConnectionChanged()
    {
        if (_disposed || !_initialized) return;
        _ = Application.Current.Dispatcher.InvokeAsync(async () => await LoadItemTypesAsync());
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
        catch { }
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
        if (_disposed) return;
        _disposed = true;
        _connectionService.ConnectionChanged -= OnConnectionChanged;
        _cancellationSource?.Cancel();
        _cancellationSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}
