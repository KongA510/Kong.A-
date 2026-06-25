using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.App.Views;
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace ArasToolkit.App.ViewModels;

public class DataImportViewModel : ObservableObject
{
    private readonly IDataImportService _dataImportService;
    private readonly IErrorLogService _errorLogService;

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

    public DataImportViewModel(
        IDataImportService dataImportService,
        IErrorLogService errorLogService)
    {
        _dataImportService = dataImportService;
        _errorLogService = errorLogService;

        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync());
        LoadPreviewCommand = new RelayCommand(async _ => await LoadPreviewAsync(), _ => CanLoadPreview());
        SaveConfigCommand = new RelayCommand(async _ => await SaveConfigAsync(), _ => CanSaveConfig());
        DeleteConfigCommand = new RelayCommand(async param => await DeleteConfigAsync(param as string), _ => SelectedConfig != null);
        OpenConfigSelectorCommand = new RelayCommand(async _ => await OpenConfigSelectorAsync());
        PreviewAmlCommand = new RelayCommand(async _ => await PreviewAmlAsync(), _ => CanPreviewAml());
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(), _ => CanExecuteImport());
        PauseCommand = new RelayCommand(_ => PauseAsync(), _ => IsImporting && !IsPaused);
        ResumeCommand = new RelayCommand(_ => ResumeAsync(), _ => IsImporting && IsPaused);

        _ = LoadConfigsAsync();
    }

    // ---- Properties ----
    public string SelectedFilePath { get => _selectedFilePath; set { SetProperty(ref _selectedFilePath, value); OnPropertyChanged(nameof(FileName)); } }
    public string FileName => string.IsNullOrEmpty(SelectedFilePath) ? "(未选择文件)" : Path.GetFileName(SelectedFilePath);

    private ObservableCollection<string> _sheetNames = [];
    public ObservableCollection<string> SheetNames
    {
        get => _sheetNames;
        set { SetProperty(ref _sheetNames, value); OnPropertyChanged(nameof(SheetNames)); }
    }
    public string? SelectedSheetName { get => _selectedSheetName; set => SetProperty(ref _selectedSheetName, value); }

    public int StartRow { get => _startRow; set => SetProperty(ref _startRow, value); }
    public int EndRow { get => _endRow; set => SetProperty(ref _endRow, value); }
    public int StartCol { get => _startCol; set => SetProperty(ref _startCol, value); }
    public int EndCol { get => _endCol; set => SetProperty(ref _endCol, value); }

    public string AmlContent { get => _amlContent; set { SetProperty(ref _amlContent, value); RefreshCommands(); } }
    public string PreviewResult { get => _previewResult; set => SetProperty(ref _previewResult, value); }

    public DataTable? PreviewData { get => _previewData; set => SetProperty(ref _previewData, value); }
    public ImportResult? LastResult { get => _lastResult; set { SetProperty(ref _lastResult, value); OnPropertyChanged(nameof(HasResult)); } }
    public bool HasResult => LastResult != null;

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set { SetProperty(ref _isLoading, value); RefreshCommands(); } }
    public bool IsImporting { get => _isImporting; set { SetProperty(ref _isImporting, value); RefreshCommands(); OnPropertyChanged(nameof(IsProgressVisible)); } }
    public bool IsPaused { get => _isPaused; set { SetProperty(ref _isPaused, value); RefreshCommands(); OnPropertyChanged(nameof(IsProgressVisible)); } }
    public bool IsProgressVisible => IsImporting || IsPaused;
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
        var dialog = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls" };
        if (dialog.ShowDialog() == true)
        {
            SelectedFilePath = dialog.FileName;
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
            }
            finally { IsLoading = false; }
        }
    }

    private bool CanLoadPreview() => !IsLoading && !string.IsNullOrEmpty(SelectedFilePath) && !string.IsNullOrEmpty(SelectedSheetName);

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

    private bool CanSaveConfig() => !string.IsNullOrEmpty(AmlContent);

    private async Task SaveConfigAsync()
    {
        var prompt = new TextPromptWindow("保存AML模板", "请输入模板名称:");
        prompt.Owner = System.Windows.Application.Current.MainWindow;
        if (prompt.ShowDialog() != true || string.IsNullOrEmpty(prompt.InputText))
            return;

        IsLoading = true;
        try
        {
            var config = new DataImportConfig
            {
                ConfigName = prompt.InputText,
                AmlContent = AmlContent,
            };
            await _dataImportService.SaveConfigAsync(config);
            StatusMessage = "配置已保存: " + prompt.InputText;
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
        catch (Exception ex) { ErrorMessage = "删除失败: " + ex.Message; }
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

        // Open ConfigSelectWindow dialog for selection
        ConfigSelectWindow window = new ConfigSelectWindow(this);
        window.Owner = System.Windows.Application.Current.MainWindow;
        window.ShowDialog();

        if (window.DialogResult == true)
        {
            StatusMessage = "已加载配置";
        }
        }
        catch (Exception ex)
        {
            ErrorMessage = "获取配置列表失败: " + ex.Message;
            await _errorLogService.LogErrorAsync("数据导入-打开配置选择器", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private bool CanPreviewAml() => !string.IsNullOrEmpty(AmlContent) && PreviewData?.Rows.Count > 0;
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
        catch (Exception ex) { PreviewResult = "预览失败: " + ex.Message; }
    }

    private bool CanExecuteImport() => !IsImporting && !string.IsNullOrEmpty(SelectedFilePath) && SelectedSheetName != null;
    private async Task ExecuteImportAsync()
    {
        IsImporting = true;
        IsPaused = false;
        _cts = new CancellationTokenSource();
        ErrorMessage = string.Empty;

        try
        {
            ImportProgress = 0;
            ProgressText = "导入中...";

            int totalRows = EndRow == -1 ? 0 : EndRow - StartRow + 1;

           LastResult = await _dataImportService.ExecuteImportAsync(
               SelectedFilePath, SelectedSheetName,
               StartRow, EndRow, StartCol, EndCol,
               AmlContent,
               async (rowNum, total) =>
               {
                   // 计算百分比并更新进度条和文本
                   ImportProgress = total > 0 ? (double)rowNum / total * 100 : 0;
                   ProgressText = rowNum + "/" + total;
                   await Task.Delay(1);
               });
           // 导入完成后的状态更新
           ImportProgress = 100;
           ProgressText = "完成: " + (LastResult?.TotalRows ?? 0) + " 行";
           StatusMessage = "导入完成: 总计" + (LastResult?.TotalRows ?? 0)
               + " 成功" + (LastResult?.SuccessCount ?? 0)
               + " 失败" + (LastResult?.FailureCount ?? 0);
       }
       catch (Exception ex)
        {
            ErrorMessage = "导入失败: " + ex.Message;
        }
        finally { IsImporting = false; IsPaused = false; }
    }

    private void PauseAsync()
    {
        IsPaused = true;
        _cts?.Cancel();
    }

    private void ResumeAsync()
    {
        IsPaused = false;
        _cts = new CancellationTokenSource();
    }
}
