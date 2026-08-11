using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// “工作流程设定”页面 ViewModel。
/// 严格执行“读取文件 → 生成并调整预览 → 生成完整 AML → 用户确认 → 一次性导入”的流程。
/// </summary>
public class WorkflowMapViewModel : ObservableObject
{
    private readonly IWorkflowMapService _workflowMapService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;

    private string _selectedFilePath = string.Empty;
    private string _workflowName = string.Empty;
    private string _description = string.Empty;
    private string _importMode = "覆盖";
    private string _statusMessage = "请下载模板或选择已有的工作流程 Excel 文件。";
    private string _errorMessage = string.Empty;
    private string _amlText = string.Empty;
    private bool _isBusy;
    private bool _isVisualEditing;
    private WorkflowMapDefinition? _previewDefinition;
    private WorkflowMapPreparedAml? _preparedAml;

    public WorkflowMapViewModel(
        IWorkflowMapService workflowMapService,
        IFileDialogService fileDialogService,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _workflowMapService = workflowMapService;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _errorLogService = errorLogService;

        DownloadTemplateCommand = new RelayCommand(async _ => await DownloadTemplateAsync(), _ => !IsBusy);
        BrowseFileCommand = new RelayCommand(async _ => await BrowseAndPreviewAsync(), _ => !IsBusy);
        RefreshPreviewCommand = new RelayCommand(_ => RefreshPreview(), _ => HasPreview && !IsBusy);
        GenerateAmlCommand = new RelayCommand(async _ => await GenerateAmlAsync(), _ => CanGenerateAml);
        ExportAmlCommand = new RelayCommand(async _ => await ExportAmlAsync(), _ => CanExportAml);
        ImportCommand = new RelayCommand(async _ => await ImportAsync(), _ => CanImport);
    }

    /// <summary>页面监听该事件并重绘流程图。</summary>
    public event Action? PreviewChanged;

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (SetProperty(ref _selectedFilePath, value ?? string.Empty))
                OnPropertyChanged(nameof(FileName));
        }
    }

    public string FileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "（未选择文件）"
        : Path.GetFileName(SelectedFilePath);

    /// <summary>流程名称只在导入页面手工输入，不写入 Excel 模板。</summary>
    public string WorkflowName
    {
        get => _workflowName;
        set
        {
            if (SetProperty(ref _workflowName, value ?? string.Empty))
                InvalidatePreparedAml();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value ?? string.Empty))
                InvalidatePreparedAml();
        }
    }

    public string ImportMode
    {
        get => _importMode;
        set
        {
            if (SetProperty(ref _importMode, value ?? "覆盖"))
                InvalidatePreparedAml();
        }
    }

    public IReadOnlyList<string> ImportModeOptions { get; } = ["新增", "覆盖"];

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public string AmlText
    {
        get => _amlText;
        set
        {
            if (SetProperty(ref _amlText, value))
            {
                OnPropertyChanged(nameof(HasAml));
                RefreshCommands();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                RefreshCommands();
        }
    }

    public WorkflowMapDefinition? PreviewDefinition
    {
        get => _previewDefinition;
        private set
        {
            if (ReferenceEquals(_previewDefinition, value)) return;
            UnsubscribeDefinition(_previewDefinition);
            _previewDefinition = value;
            SubscribeDefinition(_previewDefinition);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Nodes));
            OnPropertyChanged(nameof(Paths));
            OnPropertyChanged(nameof(HasPreview));
            OnPropertyChanged(nameof(SummaryText));
            OnPropertyChanged(nameof(WarningsText));
            RefreshCommands();
            PreviewChanged?.Invoke();
        }
    }

    public ObservableCollection<WorkflowMapNode> Nodes => PreviewDefinition?.Nodes ?? [];
    public ObservableCollection<WorkflowMapPath> Paths => PreviewDefinition?.Paths ?? [];
    public bool HasPreview => PreviewDefinition != null;
    public bool HasAml => !string.IsNullOrWhiteSpace(AmlText);
    public bool CanGenerateAml => HasPreview && !IsBusy && !string.IsNullOrWhiteSpace(WorkflowName);
    public bool CanExportAml => HasAml && !IsBusy;
    public bool CanImport => HasPreview && !IsBusy && !string.IsNullOrWhiteSpace(WorkflowName);

    public string SummaryText => PreviewDefinition == null
        ? "尚未生成预览"
        : $"节点 {PreviewDefinition.Nodes.Count} 个，路径 {PreviewDefinition.Paths.Count} 条";

    public string WarningsText => PreviewDefinition == null || PreviewDefinition.Warnings.Count == 0
        ? string.Empty
        : string.Join(Environment.NewLine, PreviewDefinition.Warnings);

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand RefreshPreviewCommand { get; }
    public ICommand GenerateAmlCommand { get; }
    public ICommand ExportAmlCommand { get; }
    public ICommand ImportCommand { get; }

    /// <summary>画布开始拖拽时抑制中途重绘，避免销毁正在捕获指针的控件。</summary>
    public void BeginVisualEdit() => _isVisualEditing = true;

    /// <summary>画布拖拽结束后统一校验、失效旧 AML 并重绘。</summary>
    public void EndVisualEdit()
    {
        if (!_isVisualEditing) return;
        _isVisualEditing = false;
        NotifyDefinitionEdited();
    }

    /// <summary>页面在新增/删除转折点后通知 ViewModel。</summary>
    public void NotifyDefinitionEdited()
    {
        if (PreviewDefinition == null) return;
        try
        {
            _workflowMapService.NormalizeAndValidate(PreviewDefinition);
            ErrorMessage = string.Empty;
            StatusMessage = $"预览已更新：{SummaryText}。请重新生成最终 AML。";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _ = _errorLogService.LogErrorAsync("工作流程设定-调整预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }

        InvalidatePreparedAml();
        OnPropertyChanged(nameof(WarningsText));
        PreviewChanged?.Invoke();
    }

    private async Task DownloadTemplateAsync()
    {
        try
        {
            var path = await _fileDialogService.PickSaveFileAsync("工作流程设定模板.xlsx", ".xlsx");
            if (string.IsNullOrWhiteSpace(path)) return;

            IsBusy = true;
            await File.WriteAllBytesAsync(path, _workflowMapService.GenerateTemplate());
            StatusMessage = $"模板已保存：{path}";
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("工作流程设定-下载模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BrowseAndPreviewAsync()
    {
        try
        {
            var path = await _fileDialogService.PickOpenFileAsync(
                "选择工作流程设定 Excel 文件", ".xlsx");
            if (string.IsNullOrWhiteSpace(path)) return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = "正在读取并校验工作流程文件...";
            SelectedFilePath = path;
            PreviewDefinition = await _workflowMapService.ParseTemplateAsync(path);
            InvalidatePreparedAml();
            StatusMessage = $"预览已生成：{SummaryText}。可拖动节点和橙色转折点进行调整。";
        }
        catch (Exception ex)
        {
            PreviewDefinition = null;
            ErrorMessage = $"生成预览失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("工作流程设定-生成预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RefreshPreview()
    {
        NotifyDefinitionEdited();
        StatusMessage = $"预览已按当前表格数据刷新：{SummaryText}。";
    }

    private async Task<WorkflowMapPreparedAml?> GenerateAmlAsync()
    {
        if (PreviewDefinition == null) return null;
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = "正在查询同名 Workflow Map 并组装最终 AML...";

            // 服务内部仅把同步 IOM 查询放到后台线程；预览集合校验仍在 UI 线程完成。
            var prepared = await _workflowMapService.PrepareAmlAsync(
                PreviewDefinition, WorkflowName, Description, ImportMode);
            _preparedAml = prepared;
            AmlText = prepared.Aml;
            StatusMessage = prepared.IsOverwrite
                ? $"最终 AML 已生成：将保留 Map ID，并替换 {prepared.RemovedActivityCount} 个旧节点关系。"
                : "最终 AML 已生成：系统中无同名模板，将执行新增。";
            return prepared;
        }
        catch (Exception ex)
        {
            _preparedAml = null;
            AmlText = string.Empty;
            ErrorMessage = $"生成 AML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("工作流程设定-页面生成AML", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportAmlAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(AmlText))
                return;

            var safeName = string.Join('_', WorkflowName.Split(Path.GetInvalidFileNameChars(),
                StringSplitOptions.RemoveEmptyEntries));
            var path = await _fileDialogService.PickSaveFileAsync(
                $"{(string.IsNullOrWhiteSpace(safeName) ? "WorkflowMap" : safeName)}-完整执行AML.xml", ".xml");
            if (string.IsNullOrWhiteSpace(path)) return;

            // 显式使用无 BOM UTF-8，保证中文节点名称和注释不会因系统默认编码而损坏。
            await File.WriteAllTextAsync(path, AmlText, new UTF8Encoding(false));
            StatusMessage = $"完整执行 AML 已导出：{path}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导出 AML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("工作流程设定-导出AML", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task ImportAsync()
    {
        try
        {
            // 每次确认导入前重新准备，确保用户最后一次预览调整全部进入 AML，
            // 同时重新读取覆盖目标，避免长时间停留页面造成旧状态误覆盖。
            var prepared = await GenerateAmlAsync();
            if (prepared == null || PreviewDefinition == null) return;

            var confirmed = await _dialogService.ConfirmAsync(
                "确认导入工作流程",
                $"名称：{WorkflowName}\n模式：{(prepared.IsOverwrite ? "覆盖现有模板" : "新增模板")}\n" +
                $"节点：{PreviewDefinition.Nodes.Count} 个\n路径：{PreviewDefinition.Paths.Count} 条\n\n" +
                "系统将通过一次 Innovator.applyAML 执行当前 AML。是否继续？",
                "确认导入",
                "返回检查");
            if (!confirmed)
            {
                StatusMessage = "已取消导入，可继续调整预览或检查 AML。";
                return;
            }

            IsBusy = true;
            StatusMessage = "正在一次性执行完整 AML...";
            var result = await Task.Run(() => _workflowMapService.ImportAsync(prepared, WorkflowName));
            AmlText = result.ExecutedAml;
            StatusMessage = result.Message;
            ErrorMessage = string.Empty;
            await _dialogService.AlertAsync("工作流程导入完成", result.Message);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("工作流程设定-页面导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SubscribeDefinition(WorkflowMapDefinition? definition)
    {
        if (definition == null) return;
        foreach (var node in definition.Nodes) node.PropertyChanged += OnDefinitionItemChanged;
        foreach (var path in definition.Paths) path.PropertyChanged += OnDefinitionItemChanged;
    }

    private void UnsubscribeDefinition(WorkflowMapDefinition? definition)
    {
        if (definition == null) return;
        foreach (var node in definition.Nodes) node.PropertyChanged -= OnDefinitionItemChanged;
        foreach (var path in definition.Paths) path.PropertyChanged -= OnDefinitionItemChanged;
    }

    private void OnDefinitionItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvalidatePreparedAml();
        OnPropertyChanged(nameof(WarningsText));
        // 参数表格输入期间不立即重建整张画布，避免 TextBox 每输入一个字符就丢失焦点。
        // 用户可点击“刷新预览”；画布拖拽结束与转折点增删仍会主动触发重绘。
    }

    private void InvalidatePreparedAml()
    {
        _preparedAml = null;
        if (!string.IsNullOrWhiteSpace(AmlText))
            AmlText = string.Empty;
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        OnPropertyChanged(nameof(CanGenerateAml));
        OnPropertyChanged(nameof(CanExportAml));
        OnPropertyChanged(nameof(CanImport));
        (DownloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (GenerateAmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExportAmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
