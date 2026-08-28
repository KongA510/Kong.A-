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

/// <summary>WPF 版类结构全量覆盖汇入页面模型。</summary>
public sealed class ClassStructureImportViewModel : ObservableObject
{
    private readonly IClassStructureImportService _service;
    private readonly IArasConnectionService _connectionService;
    private readonly IErrorLogService _errorLogService;
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
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _errorLogService = errorLogService;
        DownloadTemplateCommand = new RelayCommand(async _ => await DownloadTemplateAsync(), _ => !IsBusy);
        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => !IsBusy);
        QueryCommand = new RelayCommand(async _ => await QueryAsync(), _ => !IsBusy && _connectionService.IsConnected);
        ImportCommand = new RelayCommand(async _ => await ImportAsync(), _ => CanImport);
    }

    public ObservableCollection<ClassStructureItemType> ItemTypes { get; } = [];
    public string QueryKeyword { get => _queryKeyword; set => SetProperty(ref _queryKeyword, value); }

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        private set
        {
            if (!SetProperty(ref _selectedFilePath, value)) return;
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
            if (!SetProperty(ref _selectedItemType, value)) return;
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
        private set
        {
            if (!SetProperty(ref _preview, value)) return;
            OnPropertyChanged(nameof(PreviewSummary));
            OnPropertyChanged(nameof(PreviewTreeText));
            RefreshCommands();
        }
    }

    public string PreviewSummary => Preview?.Summary ?? "选择模板后自动解析层级路径";
    public string PreviewTreeText => Preview?.TreeText ?? string.Empty;
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            RefreshCommands();
        }
    }

    public bool CanImport => !IsBusy && Preview != null && SelectedItemType != null &&
                             !string.IsNullOrWhiteSpace(SelectedFilePath) &&
                             _connectionService.IsConnected;

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand QueryCommand { get; }
    public ICommand ImportCommand { get; }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;
        if (_connectionService.IsConnected)
            await QueryAsync();
        else
            StatusMessage = "请先连接 Aras 后再查询目标对象类。";
    }

    private async Task DownloadTemplateAsync()
    {
        var dialog = new SaveFileDialog
        {
            FileName = "Aras类结构汇入模板.xlsx",
            Filter = "Excel 工作簿 (*.xlsx)|*.xlsx",
            DefaultExt = ".xlsx"
        };
        if (dialog.ShowDialog() != true) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await File.WriteAllBytesAsync(dialog.FileName, _service.GenerateTemplate());
            StatusMessage = $"模板已保存：{dialog.FileName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板下载失败：{ex.Message}";
            await LogErrorBestEffortAsync("类结构汇入-WPF下载模板", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task BrowseFileAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择类结构汇入模板",
            Filter = "Excel 工作簿 (*.xlsx)|*.xlsx"
        };
        if (dialog.ShowDialog() != true) return;

        SelectedFilePath = dialog.FileName;
        Preview = null;
        IsBusy = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在解析类结构路径…";
        try
        {
            Preview = await _service.AnalyzeTemplateAsync(dialog.FileName);
            StatusMessage = $"模板解析完成：{Preview.Summary}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板解析失败：{ex.Message}";
            StatusMessage = string.Empty;
            await LogErrorBestEffortAsync("类结构汇入-WPF解析模板", ex);
        }
        finally { IsBusy = false; }
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
        try
        {
            var items = await _service.QueryItemTypesAsync(QueryKeyword);
            ItemTypes.Clear();
            foreach (var item in items) ItemTypes.Add(item);
            SelectedItemType = null;
            StatusMessage = $"查询完成，共 {ItemTypes.Count} 个对象类。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询失败：{ex.Message}";
            await LogErrorBestEffortAsync("类结构汇入-WPF查询对象类", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task ImportAsync()
    {
        if (!CanImport || SelectedItemType == null || Preview == null) return;
        var target = SelectedItemType;
        var confirmed = MessageBox.Show(
            $"将用模板中的 {Preview.PathCount} 条路径、{Preview.NodeCount} 个节点完整替换“{target.DisplayName}”现有的 class_structure。\n\n旧结构不会合并或自动保留，是否继续？",
            "全量覆盖类结构",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
        if (!confirmed) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在生成全新 GUID 并覆盖 class_structure…";
        try
        {
            var result = await _service.ImportAsync(SelectedFilePath, target);
            StatusMessage = $"汇入完成：{result.ItemTypeName}.class_structure 已全量覆盖，共 {result.NodeCount} 个节点，最深 {result.MaxDepth} 级。";
            MessageBox.Show(StatusMessage, "类结构汇入完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"汇入失败：{ex.Message}";
            StatusMessage = string.Empty;
            await LogErrorBestEffortAsync("类结构汇入-WPF执行", ex);
        }
        finally { IsBusy = false; }
    }

    private async Task LogErrorBestEffortAsync(string operation, Exception ex)
    {
        try
        {
            await _errorLogService.LogErrorAsync(operation, ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        catch
        {
            // 错误日志失败不覆盖原始异常。
        }
    }

    private void RefreshCommands()
    {
        (DownloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (QueryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
