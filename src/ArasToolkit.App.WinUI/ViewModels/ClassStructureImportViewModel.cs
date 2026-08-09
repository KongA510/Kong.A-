using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>类结构模板解析、对象类选择与全量覆盖汇入。</summary>
public sealed class ClassStructureImportViewModel : ObservableObject, IDisposable
{
    private readonly IClassStructureImportService _service;
    private readonly IArasConnectionService _connectionService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;
    private readonly DispatcherQueue _dispatcherQueue;

    private string _queryKeyword = string.Empty;
    private string _selectedFilePath = string.Empty;
    private ClassStructureItemType? _selectedItemType;
    private ClassStructurePreview? _preview;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _initialized;

    public ClassStructureImportViewModel(
        IClassStructureImportService service,
        IArasConnectionService connectionService,
        IFileDialogService fileDialogService,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _errorLogService = errorLogService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _connectionService.ConnectionChanged += OnConnectionChanged;

        DownloadTemplateCommand = new RelayCommand(async _ => await DownloadTemplateAsync(), _ => !IsBusy);
        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => !IsBusy);
        QueryCommand = new RelayCommand(async _ => await QueryAsync(), _ => !IsBusy && _connectionService.IsConnected);
        ImportCommand = new RelayCommand(async _ => await ImportAsync(), _ => CanImport);
    }

    public ObservableCollection<ClassStructureItemType> ItemTypes { get; } = [];

    public string QueryKeyword
    {
        get => _queryKeyword;
        set => SetProperty(ref _queryKeyword, value);
    }

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (!SetProperty(ref _selectedFilePath, value))
                return;
            OnPropertyChanged(nameof(SelectedFileName));
            RefreshCommands();
        }
    }

    public string SelectedFileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "尚未选择 .xlsx 文件"
        : Path.GetFileName(SelectedFilePath);

    public ClassStructureItemType? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value))
                return;
            OnPropertyChanged(nameof(SelectedTargetText));
            RefreshCommands();
        }
    }

    public string SelectedTargetText => SelectedItemType == null
        ? "尚未选择目标对象类"
        : $"将覆盖：{SelectedItemType.DisplayName}.class_structure";

    public ClassStructurePreview? Preview
    {
        get => _preview;
        set
        {
            if (!SetProperty(ref _preview, value))
                return;
            OnPropertyChanged(nameof(PreviewSummary));
            OnPropertyChanged(nameof(PreviewTreeText));
            OnPropertyChanged(nameof(HasPreview));
            RefreshCommands();
        }
    }

    public string PreviewSummary => Preview?.Summary ?? "选择模板后自动解析层级路径";
    public string PreviewTreeText => Preview?.TreeText ?? "第1级\n└─ 第2级\n   └─ 第3级";
    public bool HasPreview => Preview != null;

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (!SetProperty(ref _errorMessage, value))
                return;
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (!SetProperty(ref _isBusy, value))
                return;
            OnPropertyChanged(nameof(CanImport));
            RefreshCommands();
        }
    }

    public bool CanImport => !IsBusy && Preview != null && SelectedItemType != null &&
                             !string.IsNullOrWhiteSpace(SelectedFilePath) &&
                             _connectionService.IsConnected;

    public string ConnectionText => _connectionService.IsConnected && _connectionService.CurrentConnection != null
        ? $"Aras：{_connectionService.CurrentConnection.Database} / {_connectionService.CurrentConnection.Username}"
        : "Aras：未连接";

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand QueryCommand { get; }
    public ICommand ImportCommand { get; }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        _initialized = true;
        OnPropertyChanged(nameof(ConnectionText));
        if (_connectionService.IsConnected)
            await QueryAsync();
        else
            StatusMessage = "请先在“Aras连接”页面登录，再查询目标对象类。";
    }

    private async Task DownloadTemplateAsync()
    {
        var path = await _fileDialogService.PickSaveFileAsync("Aras类结构汇入模板.xlsx", ".xlsx");
        if (string.IsNullOrWhiteSpace(path))
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await File.WriteAllBytesAsync(path, _service.GenerateTemplate());
            StatusMessage = $"模板已保存：{path}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板下载失败：{ex.Message}";
            await LogErrorBestEffortAsync("类结构汇入-页面下载模板", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BrowseFileAsync()
    {
        var path = await _fileDialogService.PickOpenFileAsync("选择类结构汇入模板", ".xlsx");
        if (string.IsNullOrWhiteSpace(path))
            return;

        SelectedFilePath = path;
        Preview = null;
        IsBusy = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在解析类结构路径…";
        try
        {
            Preview = await _service.AnalyzeTemplateAsync(path);
            StatusMessage = $"模板解析完成：{Preview.Summary}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板解析失败：{ex.Message}";
            StatusMessage = string.Empty;
            await LogErrorBestEffortAsync("类结构汇入-页面解析模板", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task QueryAsync()
    {
        if (!_connectionService.IsConnected)
        {
            StatusMessage = "尚未连接 Aras，无法查询对象类。";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在查询对象类…";
        try
        {
            var previousId = SelectedItemType?.Id;
            var items = await _service.QueryItemTypesAsync(QueryKeyword);
            ItemTypes.Clear();
            foreach (var item in items)
                ItemTypes.Add(item);
            SelectedItemType = ItemTypes.FirstOrDefault(item => item.Id == previousId);
            StatusMessage = $"查询完成，共 {ItemTypes.Count} 个对象类；请选择一个覆盖目标。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询失败：{ex.Message}";
            StatusMessage = string.Empty;
            await LogErrorBestEffortAsync("类结构汇入-页面查询对象类", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportAsync()
    {
        if (!CanImport || SelectedItemType == null || Preview == null)
            return;

        var target = SelectedItemType;
        var confirmed = await _dialogService.ConfirmAsync(
            "全量覆盖类结构",
            $"将用模板中的 {Preview.PathCount} 条路径、{Preview.NodeCount} 个节点完整替换“{target.DisplayName}”现有的 class_structure。旧结构不会合并或自动保留，是否继续？",
            "覆盖并汇入",
            "取消");
        if (!confirmed)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在生成全新 GUID 并覆盖 class_structure…";
        try
        {
            var result = await _service.ImportAsync(SelectedFilePath, target);
            StatusMessage = $"汇入完成：{result.ItemTypeName}.class_structure 已全量覆盖，共 {result.NodeCount} 个节点，最深 {result.MaxDepth} 级。";
            await _dialogService.AlertAsync("类结构汇入完成", StatusMessage);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"汇入失败：{ex.Message}";
            StatusMessage = string.Empty;
            await LogErrorBestEffortAsync("类结构汇入-页面执行", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnConnectionChanged()
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(ConnectionText));
            OnPropertyChanged(nameof(CanImport));
            if (!_connectionService.IsConnected)
            {
                ItemTypes.Clear();
                SelectedItemType = null;
                StatusMessage = "Aras 连接已断开。";
            }
            RefreshCommands();
        });
    }

    private async Task LogErrorBestEffortAsync(string operation, Exception ex)
    {
        try
        {
            await _errorLogService.LogErrorAsync(
                operation, ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        catch
        {
            // 页面错误日志失败不能覆盖原始错误。
        }
    }

    private void RefreshCommands()
    {
        (DownloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (QueryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public void Dispose()
        => _connectionService.ConnectionChanged -= OnConnectionChanged;
}
