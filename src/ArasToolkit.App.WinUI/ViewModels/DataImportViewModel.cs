using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

public class DataImportViewModel : ObservableObject
{
    private readonly IDataImportService _dataImportService;
    private readonly IErrorLogService _errorLogService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly DispatcherQueue _dispatcherQueue;
    private TaskCompletionSource? _resumeSource;

    private string _selectedFilePath = string.Empty;
    private string? _selectedSheetName;
    private int _startRow = 2;
    private int _endRow = -1;
    private int _startCol = 1;
    private int _endCol = -1;
    private string _amlContent = string.Empty;
    private string _previewResult = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isImporting;
    private bool _isPaused;
    private DataTable? _previewData;
    private ImportResult? _lastResult;
    private double _importProgress;
    private string _progressText = string.Empty;
    private CancellationTokenSource? _cts;
    private int _maxConcurrency = 1; // 并发线程数（1=串行）

    public DataImportViewModel(
        IDataImportService dataImportService,
        IErrorLogService errorLogService,
        IDialogService dialogService,
        IFileDialogService fileDialogService)
    {
        _dataImportService = dataImportService;
        _errorLogService = errorLogService;
        _dialogService = dialogService;
        _fileDialogService = fileDialogService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => CanEditImport);
        LoadPreviewCommand = new RelayCommand(async _ => await LoadPreviewAsync(), _ => CanLoadPreview());
        SaveConfigCommand = new RelayCommand(async _ => await SaveConfigAsync(), _ => CanSaveConfig());
        DeleteConfigCommand = new RelayCommand(async param => await DeleteConfigAsync(param as string), _ => SelectedConfig != null);
        OpenConfigSelectorCommand = new RelayCommand(async _ => await OpenConfigSelectorAsync(), _ => CanEditImport);
        PreviewAmlCommand = new RelayCommand(async _ => await PreviewAmlAsync(), _ => CanPreviewAml());
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(), _ => CanExecuteImport());
        PauseCommand = new RelayCommand(_ => PauseAsync(), _ => IsImporting && !IsPaused);
        ResumeCommand = new RelayCommand(_ => ResumeAsync(), _ => IsImporting && IsPaused);

        _ = LoadConfigsAsync();
    }

    // ---- Properties ----
    public string SelectedFilePath { get => _selectedFilePath; set { SetProperty(ref _selectedFilePath, value); OnPropertyChanged(nameof(FileName)); RefreshCommands(); } }
    public string FileName => string.IsNullOrEmpty(SelectedFilePath) ? "(未选择文件)" : Path.GetFileName(SelectedFilePath);

    private ObservableCollection<string> _sheetNames = [];
    public ObservableCollection<string> SheetNames
    {
        get => _sheetNames;
        set { SetProperty(ref _sheetNames, value); OnPropertyChanged(nameof(SheetNames)); }
    }
    public string? SelectedSheetName { get => _selectedSheetName; set { SetProperty(ref _selectedSheetName, value); RefreshCommands(); } }

    public int StartRow { get => _startRow; set => SetProperty(ref _startRow, value); }
    public int EndRow { get => _endRow; set => SetProperty(ref _endRow, value); }
    public int StartCol { get => _startCol; set => SetProperty(ref _startCol, value); }
    public int EndCol { get => _endCol; set => SetProperty(ref _endCol, value); }

    public string AmlContent { get => _amlContent; set { SetProperty(ref _amlContent, value); RefreshCommands(); } }
    public string PreviewResult { get => _previewResult; set => SetProperty(ref _previewResult, value); }

    public DataTable? PreviewData { get => _previewData; set { SetProperty(ref _previewData, value); RefreshCommands(); } }
    public ImportResult? LastResult { get => _lastResult; set { SetProperty(ref _lastResult, value); OnPropertyChanged(nameof(HasResult)); } }
    public bool HasResult => LastResult != null;

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); RefreshCommands(); } }
    public bool IsImporting { get => _isImporting; set { SetProperty(ref _isImporting, value); RefreshCommands(); OnPropertyChanged(nameof(IsProgressVisible)); OnPropertyChanged(nameof(CanEditImport)); } }
    public bool CanEditImport => !IsImporting && !IsLoading;
    public bool IsPaused { get => _isPaused; set { SetProperty(ref _isPaused, value); RefreshCommands(); OnPropertyChanged(nameof(IsProgressVisible)); } }
    public bool IsProgressVisible => IsImporting || IsPaused;

    /// <summary>并发线程数（1=串行，最大10），导入前可在设置中调整</summary>
    public int MaxConcurrency
    {
        get => _maxConcurrency;
        set => SetProperty(ref _maxConcurrency, Math.Clamp(value, 1, 10));
    }
    public double ImportProgress { get => _importProgress; set => SetProperty(ref _importProgress, value); }
    public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }

    private ObservableCollection<DataImportConfig> _savedConfigs = [];
    public ObservableCollection<DataImportConfig> SavedConfigs
    {
        get => _savedConfigs;
        set => SetProperty(ref _savedConfigs, value);
    }
    private DataImportConfig? _selectedConfig;
    public DataImportConfig? SelectedConfig
    {
        get => _selectedConfig;
        set
        {
            SetProperty(ref _selectedConfig, value);
            (DeleteConfigCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<ColumnMapping> ColumnMappings { get; set; } = [];
    public bool HasColumnMappings => ColumnMappings.Count > 0;

    /// <summary>由视图注入的配置选择对话框回调（替代 WPF ConfigSelectWindow）。返回是否选择了配置。</summary>
    public Func<Task<bool>>? RequestConfigSelectionAsync { get; set; }

    // ---- Commands ----
    public ICommand BrowseFileCommand { get; }
    public ICommand LoadPreviewCommand { get; }
    public ICommand SaveConfigCommand { get; }
    public ICommand DeleteConfigCommand { get; }
    public ICommand OpenConfigSelectorCommand { get; }
    public ICommand PreviewAmlCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand ResumeCommand { get; }

    private void RefreshCommands()
    {
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (OpenConfigSelectorCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CanEditImport));
        (LoadPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveConfigCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviewAmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ResumeCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // ---- Methods ----
    private async Task BrowseFileAsync()
    {
        var picked = await _fileDialogService.PickOpenFileAsync("选择Excel文件", ".xlsx", ".xls");
        if (string.IsNullOrEmpty(picked) == false)
        {
            SelectedFilePath = picked;
            StatusMessage = "正在加载Sheet列表...";
            IsLoading = true;
            try
            {
                var sheets = await _dataImportService.GetSheetNamesAsync(SelectedFilePath);
                SheetNames = new ObservableCollection<string>(sheets);
                SelectedSheetName = sheets.FirstOrDefault();
                StatusMessage = "已加载 " + sheets.Count + " 个Sheet";
            }
            catch (Exception ex)
            {
                ErrorMessage = "加载失败: " + ex.Message;
                await _errorLogService.LogErrorAsync("数据导入-读取文件", ex.Message, ErrorLog.LevelP1, ex.ToString());
            }
            finally { IsLoading = false; }
        }
    }

    private bool CanLoadPreview() => CanEditImport && !string.IsNullOrEmpty(SelectedFilePath) && !string.IsNullOrEmpty(SelectedSheetName);

    private async Task LoadPreviewAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在加载数据预览...";
        try
        {
            var data = await _dataImportService.ReadSheetRangeAsync(SelectedFilePath, SelectedSheetName!, StartRow, EndRow, StartCol, EndCol);
            PreviewData = data.Data;
            ColumnMappings = new ObservableCollection<ColumnMapping>(data.ColumnMappings);
            OnPropertyChanged(nameof(ColumnMappings));
            OnPropertyChanged(nameof(HasColumnMappings));
            StatusMessage = "预览已加载: " + (data.Data?.Rows.Count ?? 0) + " 行数据";
        }
        catch (Exception ex)
        {
            ErrorMessage = "预览加载失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-预览", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsLoading = false; }
    }

    private async Task LoadConfigsAsync()
    {
        try
        {
            var configs = await _dataImportService.GetConfigsAsync();
            SavedConfigs = new ObservableCollection<DataImportConfig>(configs);
        }
        catch (Exception ex)
        {
            ErrorMessage = "加载配置列表失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-加载配置列表", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private bool CanSaveConfig() => CanEditImport && !string.IsNullOrWhiteSpace(AmlContent);

    private async Task SaveConfigAsync()
    {
        var inputName = await _dialogService.PromptAsync("保存AML模板", "请输入模板名称");
        if (string.IsNullOrEmpty(inputName))
            return;

        IsLoading = true;
        try
        {
            var config = new DataImportConfig
            {
                ConfigName = inputName,
                AmlContent = AmlContent,
            };
            await _dataImportService.SaveConfigAsync(config);
            StatusMessage = "配置已保存: " + inputName;
            await LoadConfigsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "保存失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-保存配置", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsLoading = false; }
    }

    private async Task DeleteConfigAsync(string? configId)
    {
        if (string.IsNullOrEmpty(configId)) return;
        try
        {
            await _dataImportService.DeleteConfigAsync(configId);
            StatusMessage = "配置已删除";
            await LoadConfigsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "删除失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-删除配置", ex.Message, ErrorLog.LevelP1, ex.ToString());
        }
    }

    private async Task OpenConfigSelectorAsync()
    {
        try
        {
            var configs = await _dataImportService.GetConfigsAsync();
            SavedConfigs = new ObservableCollection<DataImportConfig>(configs);
            if (configs.Count == 0)
        {
            ErrorMessage = "暂无已保存的配置";
            return;
        }

        // WinUI：由页面注入配置选择对话框回调（替代 ConfigSelectWindow）
        if (RequestConfigSelectionAsync is { } selector)
        {
            var pickedConfig = await selector.Invoke();
            if (pickedConfig) StatusMessage = "已加载配置";
        }
        }
        catch (Exception ex)
        {
            ErrorMessage = "获取配置列表失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-打开配置选择器", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private bool CanPreviewAml() => CanEditImport && !string.IsNullOrWhiteSpace(AmlContent) && PreviewData?.Rows.Count > 0;
    private async Task PreviewAmlAsync()
    {
        try
        {
            var firstRow = PreviewData!.Rows[0];
            var rowData = new Dictionary<string, string>();
            foreach (var m in ColumnMappings)
                rowData[m.Letter] = firstRow[m.Header]?.ToString() ?? "";
            PreviewResult = _dataImportService.PreviewAml(AmlContent, rowData);
        }
        catch (Exception ex)
        {
            PreviewResult = "预览失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-AML预览", ex.Message, ErrorLog.LevelP1, ex.ToString());
        }
    }

    private bool CanExecuteImport() => CanEditImport && !string.IsNullOrEmpty(SelectedFilePath)
        && SelectedSheetName != null && !string.IsNullOrWhiteSpace(AmlContent);
    private async Task ExecuteImportAsync()
    {
        if (!CanExecuteImport()) return;
        IsImporting = true;
        IsPaused = false;
        _cts = new CancellationTokenSource();
        ErrorMessage = string.Empty;
        LastResult = null;
        StatusMessage = "正在汇入...";

        try
        {
            ImportProgress = 0;
            ProgressText = "导入中...";

          LastResult = await _dataImportService.ExecuteImportAsync(
              SelectedFilePath, SelectedSheetName,
              StartRow, EndRow, StartCol, EndCol,
              AmlContent,
              MaxConcurrency,
              _cts.Token,
              UpdateImportProgressAsync);
           ImportProgress = LastResult.TotalRows > 0
               ? (double)LastResult.ProcessedRows / LastResult.TotalRows * 100 : 0;
           var state = LastResult.IsCancelled ? "导入已取消"
               : !LastResult.IsCompleted ? "导入中断"
               : LastResult.FailureCount > 0 ? "导入结束（存在失败）" : "导入完成";
           ProgressText = $"{state}: {LastResult.ProcessedRows}/{LastResult.TotalRows} 行";
           StatusMessage = $"{state}: 总计{LastResult.TotalRows} 已处理{LastResult.ProcessedRows}"
               + $" 成功{LastResult.SuccessCount} 失败{LastResult.FailureCount} 跳过{LastResult.SkippedCount}";
           ErrorMessage = LastResult.ErrorMessage;
           if (string.IsNullOrEmpty(ErrorMessage) && LastResult.FailureCount > 0)
               ErrorMessage = $"{LastResult.FailureCount} 行导入失败，具体行号和原因请查看日志。";
       }
       catch (Exception ex)
        {
            ErrorMessage = "导入失败: " + (string.IsNullOrWhiteSpace(ex.Message)
                ? $"{ex.GetType().Name} (0x{ex.HResult:X8})" : ex.Message);
            StatusMessage = "导入中断";
            await _errorLogService.LogErrorAsync("数据导入-执行界面", ErrorMessage, ErrorLog.LevelP1, ex.ToString());
        }
        finally
        {
            _resumeSource?.TrySetResult();
            _resumeSource = null;
            _cts?.Dispose();
            _cts = null;
            IsImporting = false;
            IsPaused = false;
        }
    }

    private async Task UpdateImportProgressAsync(int processed, int total)
    {
        // 回调来自 Parallel.ForEachAsync；WinUI 绑定属性只能在 DispatcherQueue 上更新。
        var completion = new TaskCompletionSource<Task>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                ImportProgress = Math.Max(ImportProgress, total > 0 ? (double)processed / total * 100 : 0);
                ProgressText = (IsPaused ? "已暂停: " : "")
                    + $"{(int)Math.Round(ImportProgress * total / 100)}/{total}";
                completion.SetResult(_resumeSource?.Task ?? Task.CompletedTask);
            }
            catch (Exception ex)
            {
                _ = _errorLogService.LogErrorAsync("数据导入-更新进度", ex.Message, ErrorLog.LevelP1, ex.ToString());
                completion.SetException(ex);
            }
        }))
            throw new InvalidOperationException("汇入页面已关闭，无法更新进度。");
        await (await completion.Task.ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <summary>等待已发出的请求完成后暂停，继续时仍使用当前批次。</summary>
    private void PauseAsync()
    {
        if (!IsImporting || IsPaused) return;
        _resumeSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsPaused = true;
        ProgressText = "正在暂停，等待当前请求完成...";
    }

    /// <summary>唤醒原批次，避免重新导入已成功的数据。</summary>
    private void ResumeAsync()
    {
        if (!IsImporting || !IsPaused) return;
        IsPaused = false;
        var resumeSource = _resumeSource;
        _resumeSource = null;
        resumeSource?.TrySetResult();
    }
}
