using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>“其他功能”下六项 XML / JSON 开发工具的共享 ViewModel。</summary>
public sealed class DataToolsViewModel : ObservableObject
{
    private readonly IDataToolService _dataToolService;
    private readonly ISavedXmlService _savedXmlService;
    private readonly IErrorLogService _errorLogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private string _toolName = "XML格式化";
    private string _title = "XML格式化";
    private string _subtitle = string.Empty;
    private string _inputLabel = "原始 XML";
    private string _outputLabel = "格式化 XML";
    private string _leftLabel = "XML A";
    private string _rightLabel = "XML B";
    private string _primaryActionText = "格式化 XML";
    private string _inputPlaceholder = string.Empty;
    private string _inputText = string.Empty;
    private string _secondInputText = string.Empty;
    private string _outputText = string.Empty;
    private string _rootClassName = "RootEntity";
    private string _statusMessage = string.Empty;
    private string _comparisonSummary = string.Empty;
    private bool _isCompareMode;
    private bool _isJsonToEntity;
    private bool _hasComparisonResult;
    private bool _isBusy;
    private SavedXml? _selectedSavedXml;

    public DataToolsViewModel(
        IDataToolService dataToolService,
        ISavedXmlService savedXmlService,
        IErrorLogService errorLogService,
        IFileDialogService fileDialogService,
        IDialogService dialogService)
    {
        _dataToolService = dataToolService;
        _savedXmlService = savedXmlService;
        _errorLogService = errorLogService;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        ExecuteCommand = new RelayCommand(async _ => await ExecuteAsync(), _ => !IsBusy);
        ClearCommand = new RelayCommand(Clear, () => !IsBusy);
        SwapCommand = new RelayCommand(SwapInputs, () => !IsBusy && IsCompareMode);
        OpenXmlCommand = new RelayCommand(async _ => await OpenXmlAsync(), _ => !IsBusy && IsXmlFormatter);
        SaveXmlCommand = new RelayCommand(async _ => await SaveXmlAsync(OutputText, "格式化结果"),
            _ => !IsBusy && IsXmlFormatter && HasOutput);
        SaveXmlACommand = new RelayCommand(async _ => await SaveXmlAsync(InputText, "XML A"),
            _ => !IsBusy && IsXmlCompare && !string.IsNullOrWhiteSpace(InputText));
        SaveXmlBCommand = new RelayCommand(async _ => await SaveXmlAsync(SecondInputText, "XML B"),
            _ => !IsBusy && IsXmlCompare && !string.IsNullOrWhiteSpace(SecondInputText));
        RefreshSavedXmlCommand = new RelayCommand(async _ => await RefreshSavedXmlAsync(),
            _ => !IsBusy && IsXmlTool);
        LoadSavedXmlToACommand = new RelayCommand(LoadSavedXmlToA,
            () => !IsBusy && IsXmlTool && SelectedSavedXml != null);
        LoadSavedXmlToBCommand = new RelayCommand(LoadSavedXmlToB,
            () => !IsBusy && IsXmlCompare && SelectedSavedXml != null);
        DeleteSavedXmlCommand = new RelayCommand(async _ => await DeleteSavedXmlAsync(),
            _ => !IsBusy && IsXmlTool && SelectedSavedXml != null);
        Configure(_toolName);
    }

    public ObservableCollection<DataDiffLine> DiffLines { get; } = [];
    public ObservableCollection<SavedXml> SavedXmlItems { get; } = [];

    public string Title { get => _title; private set => SetProperty(ref _title, value); }
    public string Subtitle { get => _subtitle; private set => SetProperty(ref _subtitle, value); }
    public string InputLabel { get => _inputLabel; private set => SetProperty(ref _inputLabel, value); }
    public string OutputLabel { get => _outputLabel; private set => SetProperty(ref _outputLabel, value); }
    public string LeftLabel { get => _leftLabel; private set => SetProperty(ref _leftLabel, value); }
    public string RightLabel { get => _rightLabel; private set => SetProperty(ref _rightLabel, value); }
    public string PrimaryActionText { get => _primaryActionText; private set => SetProperty(ref _primaryActionText, value); }
    public string InputPlaceholder { get => _inputPlaceholder; private set => SetProperty(ref _inputPlaceholder, value); }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value)) RaiseCommandStates();
        }
    }
    public string SecondInputText
    {
        get => _secondInputText;
        set
        {
            if (SetProperty(ref _secondInputText, value)) RaiseCommandStates();
        }
    }
    public string OutputText
    {
        get => _outputText;
        set
        {
            if (!SetProperty(ref _outputText, value)) return;
            OnPropertyChanged(nameof(HasOutput));
            RaiseCommandStates();
        }
    }
    public string RootClassName { get => _rootClassName; set => SetProperty(ref _rootClassName, value); }
    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (!SetProperty(ref _statusMessage, value)) return;
            OnPropertyChanged(nameof(HasStatus));
        }
    }
    public string ComparisonSummary { get => _comparisonSummary; private set => SetProperty(ref _comparisonSummary, value); }

    public bool IsCompareMode
    {
        get => _isCompareMode;
        private set
        {
            if (!SetProperty(ref _isCompareMode, value)) return;
            OnPropertyChanged(nameof(IsTransformMode));
            RaiseCommandStates();
        }
    }
    public bool IsTransformMode => !IsCompareMode;
    public bool IsXmlFormatter => _toolName == "XML格式化";
    public bool IsXmlCompare => _toolName == "XML比对";
    public bool IsXmlTool => IsXmlFormatter || IsXmlCompare;
    public string SavedXmlPrimaryLoadText => IsXmlFormatter ? "加载到输入" : "加载到 A";
    public bool IsJsonToEntity { get => _isJsonToEntity; private set => SetProperty(ref _isJsonToEntity, value); }
    public bool HasComparisonResult { get => _hasComparisonResult; private set => SetProperty(ref _hasComparisonResult, value); }
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool HasOutput => !string.IsNullOrWhiteSpace(OutputText);
    public SavedXml? SelectedSavedXml
    {
        get => _selectedSavedXml;
        set
        {
            if (!SetProperty(ref _selectedSavedXml, value)) return;
            RaiseCommandStates();
        }
    }
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            RaiseCommandStates();
        }
    }

    public ICommand ExecuteCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand SwapCommand { get; }
    public ICommand OpenXmlCommand { get; }
    public ICommand SaveXmlCommand { get; }
    public ICommand SaveXmlACommand { get; }
    public ICommand SaveXmlBCommand { get; }
    public ICommand RefreshSavedXmlCommand { get; }
    public ICommand LoadSavedXmlToACommand { get; }
    public ICommand LoadSavedXmlToBCommand { get; }
    public ICommand DeleteSavedXmlCommand { get; }

    public void Configure(string? toolName)
    {
        _toolName = toolName switch
        {
            "XML格式化" or "XML比对" or "JSON格式化" or "JSON比对" or "JSON转实体类" or "实体类转JSON"
                => toolName,
            _ => "XML格式化"
        };

        IsCompareMode = _toolName is "XML比对" or "JSON比对";
        IsJsonToEntity = _toolName == "JSON转实体类";
        OnPropertyChanged(nameof(IsXmlFormatter));
        OnPropertyChanged(nameof(IsXmlCompare));
        OnPropertyChanged(nameof(IsXmlTool));
        OnPropertyChanged(nameof(SavedXmlPrimaryLoadText));
        RaiseCommandStates();
        Title = _toolName;
        switch (_toolName)
        {
            case "XML格式化":
                Subtitle = "从输入内容中自动提取完整 XML 根节点，校验结构并生成统一缩进。";
                InputLabel = "原始 XML";
                OutputLabel = "格式化 XML";
                InputPlaceholder = "可直接粘贴 XML，或粘贴包含 XML 的日志文本…";
                PrimaryActionText = "格式化 XML";
                break;
            case "XML比对":
                Subtitle = "先解析并归一化两份 XML，再按行对齐差异；属性书写顺序不同不会产生伪差异。";
                LeftLabel = "XML A";
                RightLabel = "XML B";
                PrimaryActionText = "开始比对";
                break;
            case "JSON格式化":
                Subtitle = "校验 JSON 结构并生成统一缩进；支持从说明或日志文本中提取完整对象与数组。";
                InputLabel = "原始 JSON";
                OutputLabel = "格式化 JSON";
                InputPlaceholder = "粘贴 JSON 对象或数组…";
                PrimaryActionText = "格式化 JSON";
                break;
            case "JSON比对":
                Subtitle = "按属性名归一化对象后比对内容，避免仅属性顺序不同造成伪差异；数组顺序仍参与比对。";
                LeftLabel = "JSON A";
                RightLabel = "JSON B";
                PrimaryActionText = "开始比对";
                break;
            case "JSON转实体类":
                Subtitle = "依据 JSON 结构推断 .NET 类型，生成可直接使用的实体类、集合、嵌套类型和 JsonPropertyName 映射。";
                InputLabel = "JSON 数据";
                OutputLabel = ".NET 实体类";
                InputPlaceholder = "根节点需为 JSON 对象，例如 { \"id\": 1, \"name\": \"demo\" }";
                PrimaryActionText = "生成实体类";
                break;
            case "实体类转JSON":
                Subtitle = "读取 public get/set 属性和 JsonPropertyName 映射，生成体现实体结构的 JSON 样例。";
                InputLabel = ".NET 实体类";
                OutputLabel = "JSON 样例";
                InputPlaceholder = "粘贴一个或多个 C# 实体类；首个包含属性的类作为根对象…";
                PrimaryActionText = "生成 JSON";
                break;
        }
        Clear();
    }

    /// <summary>页面导航完成后加载当前用户保存的 XML。</summary>
    public async Task RefreshSavedXmlAsync()
    {
        if (!IsXmlTool) return;

        var selectedId = SelectedSavedXml?.Id;
        try
        {
            IsBusy = true;
            var items = await _savedXmlService.GetAllAsync();
            SavedXmlItems.Clear();
            foreach (var item in items) SavedXmlItems.Add(item);
            SelectedSavedXml = SavedXmlItems.FirstOrDefault(item => item.Id == selectedId)
                ?? SavedXmlItems.FirstOrDefault();
            StatusMessage = SavedXmlItems.Count == 0
                ? "数据库中暂无已保存 XML，可从当前内容新建。"
                : $"已加载 {SavedXmlItems.Count} 份数据库 XML。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载数据库 XML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("XML工具-加载数据库记录", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void NotifyOutputCopied()
        => StatusMessage = "结果已复制到剪贴板。";

    /// <summary>记录 WinUI XML 高亮呈现失败；格式化结果仍保留在 ViewModel 中。</summary>
    public async Task NotifyOutputRenderFailedAsync(Exception exception)
    {
        StatusMessage = "XML 已格式化，但语法高亮显示失败；可继续复制或保存结果。";
        await _errorLogService.LogErrorAsync("XML格式化-语法高亮", exception.Message,
            ErrorLog.LevelP1, exception.StackTrace);
    }

    private async Task OpenXmlAsync()
    {
        string? openedFileName = null;
        try
        {
            var path = await _fileDialogService.PickOpenFileAsync(
                "打开已保存的 XML", ".xml", ".aml", ".txt");
            if (string.IsNullOrWhiteSpace(path)) return;

            IsBusy = true;
            StatusMessage = "正在读取 XML 文件…";
            InputText = await File.ReadAllTextAsync(path, Encoding.UTF8);
            openedFileName = Path.GetFileName(path);
        }
        catch (Exception ex)
        {
            StatusMessage = $"打开 XML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("XML格式化-打开文件", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }

        if (openedFileName == null) return;
        await ExecuteAsync();
        if (HasOutput)
            StatusMessage = $"已打开并格式化：{openedFileName}。";
    }

    private async Task SaveXmlAsync(string xmlContent, string sourceLabel)
    {
        if (string.IsNullOrWhiteSpace(xmlContent)) return;

        try
        {
            var defaultName = SelectedSavedXml?.Name ?? $"XML-{DateTime.Now:yyyyMMdd-HHmmss}";
            var name = await _dialogService.PromptAsync(
                $"保存{sourceLabel}到数据库", "请输入 XML 名称", defaultName);
            if (string.IsNullOrWhiteSpace(name)) return;

            var existing = SavedXmlItems.FirstOrDefault(item =>
                string.Equals(item.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                var confirmed = await _dialogService.ConfirmAsync(
                    "覆盖已保存 XML",
                    $"数据库中已存在“{existing.Name}”，是否覆盖其内容？",
                    "覆盖", "取消");
                if (!confirmed) return;
            }

            IsBusy = true;
            // 比对页允许粘贴带日志前后缀的 XML；入库前统一提取并格式化，保证数据库内容可直接复用。
            var normalizedXml = await _dataToolService.FormatXmlAsync(xmlContent);
            var saved = await _savedXmlService.SaveAsync(name, normalizedXml);
            await ReloadSavedXmlAsync(saved.Id);
            StatusMessage = existing == null
                ? $"已保存到数据库：{saved.Name}。"
                : $"已覆盖数据库 XML：{saved.Name}。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存数据库 XML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("XML工具-保存数据库记录", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReloadSavedXmlAsync(string selectedId)
    {
        var items = await _savedXmlService.GetAllAsync();
        SavedXmlItems.Clear();
        foreach (var item in items) SavedXmlItems.Add(item);
        SelectedSavedXml = SavedXmlItems.FirstOrDefault(item => item.Id == selectedId)
            ?? SavedXmlItems.FirstOrDefault();
    }

    private void LoadSavedXmlToA()
    {
        if (SelectedSavedXml == null) return;
        InputText = SelectedSavedXml.XmlContent;
        ResetComparisonResult();
        StatusMessage = IsXmlFormatter
            ? $"已载入“{SelectedSavedXml.Name}”，正在格式化。"
            : $"已载入“{SelectedSavedXml.Name}”到 XML A。";
        if (IsXmlFormatter) _ = ExecuteAsync();
    }

    private void LoadSavedXmlToB()
    {
        if (SelectedSavedXml == null || !IsXmlCompare) return;
        SecondInputText = SelectedSavedXml.XmlContent;
        ResetComparisonResult();
        StatusMessage = $"已载入“{SelectedSavedXml.Name}”到 XML B。";
    }

    private async Task DeleteSavedXmlAsync()
    {
        var selected = SelectedSavedXml;
        if (selected == null) return;

        try
        {
            var confirmed = await _dialogService.ConfirmAsync(
                "删除已保存 XML", $"确定删除“{selected.Name}”吗？", "删除", "取消");
            if (!confirmed) return;

            IsBusy = true;
            await _savedXmlService.DeleteAsync(selected.Id);
            await ReloadSavedXmlAsync(string.Empty);
            StatusMessage = $"已删除数据库 XML：{selected.Name}。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"删除数据库 XML 失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("XML工具-删除数据库记录", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        StatusMessage = "正在解析结构…";
        HasComparisonResult = false;
        try
        {
            switch (_toolName)
            {
                case "XML格式化":
                    OutputText = await _dataToolService.FormatXmlAsync(InputText);
                    StatusMessage = "XML 已提取并格式化。";
                    break;
                case "JSON格式化":
                    OutputText = await _dataToolService.FormatJsonAsync(InputText);
                    StatusMessage = "JSON 已校验并格式化。";
                    break;
                case "JSON转实体类":
                    OutputText = await _dataToolService.JsonToEntityAsync(InputText, RootClassName);
                    StatusMessage = "已按 .NET 8 实体规范生成类定义。";
                    break;
                case "实体类转JSON":
                    OutputText = await _dataToolService.EntityToJsonAsync(InputText);
                    StatusMessage = "JSON 样例已生成；默认值用于表达实体结构，不代表运行时对象数据。";
                    break;
                case "XML比对":
                    await ApplyComparisonAsync(await _dataToolService.CompareXmlAsync(InputText, SecondInputText));
                    break;
                case "JSON比对":
                    await ApplyComparisonAsync(await _dataToolService.CompareJsonAsync(InputText, SecondInputText));
                    break;
            }
        }
        catch (Exception ex)
        {
            OutputText = string.Empty;
            DiffLines.Clear();
            HasComparisonResult = false;
            StatusMessage = $"处理失败：{ex.Message}";
            await _errorLogService.LogErrorAsync($"{_toolName}-页面处理", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task ApplyComparisonAsync(DataComparisonResult result)
    {
        DiffLines.Clear();
        foreach (var line in result.Lines) DiffLines.Add(line);
        HasComparisonResult = true;
        ComparisonSummary = result.DifferenceCount == 0
            ? $"结构与内容一致 · {result.Lines.Count} 行"
            : $"共 {result.DifferenceCount} 处差异 · B 中缺少 {result.RemovedCount} · 内容差异 {result.ModifiedCount} · A 中缺少 {result.AddedCount}";
        StatusMessage = result.DifferenceCount == 0 ? "比对完成，两份数据一致。" : "比对完成，差异已按颜色标记。";
        return Task.CompletedTask;
    }

    private void Clear()
    {
        InputText = string.Empty;
        SecondInputText = string.Empty;
        OutputText = string.Empty;
        StatusMessage = string.Empty;
        ComparisonSummary = string.Empty;
        DiffLines.Clear();
        HasComparisonResult = false;
    }

    private void SwapInputs()
    {
        (InputText, SecondInputText) = (SecondInputText, InputText);
        ComparisonSummary = string.Empty;
        DiffLines.Clear();
        HasComparisonResult = false;
        StatusMessage = "已交换 A/B 内容，请重新比对。";
    }

    private void ResetComparisonResult()
    {
        ComparisonSummary = string.Empty;
        DiffLines.Clear();
        HasComparisonResult = false;
    }

    private void RaiseCommandStates()
    {
        (ExecuteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SwapCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (OpenXmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveXmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveXmlACommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveXmlBCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshSavedXmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LoadSavedXmlToACommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LoadSavedXmlToBCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteSavedXmlCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
