using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>按 Aras 对象类加载关联 Form，并翻译 Form 多语言标签。</summary>
public sealed class FormTranslationViewModel : ObservableObject
{
    private readonly IFormTranslationService _service;
    private readonly IArasConnectionService _connectionService;
    private readonly IAiDispatcherService _aiDispatcherService;
    private readonly IErrorLogService _errorLogService;
    private ItemTypeItem? _selectedItemType;
    private string _itemTypeSearchText = string.Empty;
    private string _sourceLanguage = "简体中文";
    private string _targetLanguages = "繁体中文,英文";
    private string _connectionText = "Aras：未连接";
    private string _aiModelText = "AI：未配置";
    private string _statusMessage = string.Empty;
    private bool _isBusy;
    private bool _isInitialized;
    private TranslationProgressInfo? _progress;
    private CancellationTokenSource? _cancellationTokenSource;

    public FormTranslationViewModel(
        IFormTranslationService service,
        IArasConnectionService connectionService,
        IAiDispatcherService aiDispatcherService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _aiDispatcherService = aiDispatcherService;
        _errorLogService = errorLogService;

        LoadItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        LoadFormsCommand = new RelayCommand(async _ => await LoadFormsAsync(),
            _ => !IsBusy && SelectedItemType != null);
        SelectAllCommand = new RelayCommand(_ => SetAllSelected(true), _ => !IsBusy && Forms.Count > 0);
        ClearSelectionCommand = new RelayCommand(_ => SetAllSelected(false), _ => !IsBusy && Forms.Count > 0);
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsBusy && Forms.Count > 0);
        CancelCommand = new RelayCommand(_ => _cancellationTokenSource?.Cancel(), _ => IsBusy);
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => !IsBusy && Forms.Count > 0);
    }

    public ObservableCollection<ItemTypeItem> ItemTypes { get; } = [];
    public ObservableCollection<ItemTypeItem> FilteredItemTypes { get; } = [];
    public ObservableCollection<ArasFormItem> Forms { get; } = [];
    public ObservableCollection<string> SourceLanguages { get; } =
        ["简体中文", "繁体中文", "英文", "日文", "韩文", "法文", "德文", "西班牙文"];

    public ItemTypeItem? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value)) return;
            (LoadFormsCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public string ItemTypeSearchText
    {
        get => _itemTypeSearchText;
        set
        {
            if (!SetProperty(ref _itemTypeSearchText, value)) return;
            ApplyItemTypeFilter(value);
        }
    }

    public string SourceLanguage { get => _sourceLanguage; set => SetProperty(ref _sourceLanguage, value); }
    public string TargetLanguages { get => _targetLanguages; set => SetProperty(ref _targetLanguages, value); }
    public string ConnectionText { get => _connectionText; set => SetProperty(ref _connectionText, value); }
    public string AiModelText { get => _aiModelText; set => SetProperty(ref _aiModelText, value); }
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (!SetProperty(ref _statusMessage, value)) return;
            OnPropertyChanged(nameof(HasStatus));
        }
    }

    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            OnPropertyChanged(nameof(IsProgressIndeterminate));
            RaiseCommandStates();
        }
    }

    public TranslationProgressInfo? Progress
    {
        get => _progress;
        set
        {
            if (!SetProperty(ref _progress, value)) return;
            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

    public double ProgressPercentage => Progress?.Percentage ?? 0;
    public string ProgressText => Progress?.StatusText ?? string.Empty;
    public bool IsProgressIndeterminate => IsBusy && Progress == null;
    public string SelectionSummary => $"共 {Forms.Count} 个关联表单，已选择 {Forms.Count(item => item.IsSelected)} 个";

    public ICommand LoadItemTypesCommand { get; }
    public ICommand LoadFormsCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand ClearSelectionCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ExportCommand { get; }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        await RefreshContextAsync();
        if (_connectionService.IsConnected)
            await LoadItemTypesAsync();
        else
            StatusMessage = "请先在 Aras 连接页面连接服务器。";
    }

    public async Task SelectItemTypeFromSearchAsync(ItemTypeItem? itemType)
    {
        if (itemType == null) return;
        SelectedItemType = itemType;
        ItemTypeSearchText = itemType.DisplayName;
        await LoadFormsAsync();
    }

    public void NotifySelectionChanged() => NotifyCollectionState();

    private async Task RefreshContextAsync()
    {
        var connection = _connectionService.CurrentConnection;
        ConnectionText = _connectionService.IsConnected && connection != null
            ? $"Aras：{connection.Database} / {connection.Username}"
            : "Aras：未连接";
        try
        {
            var model = await _aiDispatcherService.GetCurrentModelAsync();
            AiModelText = model == null ? "AI：未配置启用模型" : $"AI：{model.ModelName} / {model.ModelIdentifier}";
        }
        catch (Exception ex)
        {
            AiModelText = "AI：配置读取失败";
            await _errorLogService.LogErrorAsync("表单翻译-读取AI配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task LoadItemTypesAsync()
    {
        IsBusy = true;
        StatusMessage = "正在从当前 Aras 连接加载对象类…";
        try
        {
            await RefreshContextAsync();
            var items = await _service.GetItemTypeListAsync();
            ItemTypes.Clear();
            foreach (var item in items) ItemTypes.Add(item);
            ApplyItemTypeFilter(ItemTypeSearchText);
            StatusMessage = $"已加载 {ItemTypes.Count} 个对象类，可输入名称或标签模糊搜索。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"对象类加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("表单翻译-加载对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadFormsAsync()
    {
        if (SelectedItemType == null) return;
        IsBusy = true;
        StatusMessage = $"正在加载 {SelectedItemType.DisplayName} 的关联表单…";
        try
        {
            var forms = await _service.GetFormsByItemTypeIdAsync(SelectedItemType.Id);
            Forms.Clear();
            foreach (var form in forms) Forms.Add(form);
            NotifyCollectionState();
            StatusMessage = Forms.Count == 0
                ? "该对象类没有关联的 Aras Form。"
                : $"已加载 {Forms.Count} 个关联表单，默认全部选中。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"表单加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("表单翻译-加载表单", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task TranslateAsync()
    {
        var selected = Forms.Where(item => item.IsSelected).ToList();
        if (SelectedItemType == null || selected.Count == 0)
        {
            StatusMessage = "请先选择至少一个表单。";
            return;
        }

        IsBusy = true;
        Progress = null;
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var task = await _service.CreateTaskAsync(
                $"表单翻译-{SelectedItemType.Name}", SelectedItemType.Id,
                SourceLanguage, TargetLanguages, selected.Count);
            var progress = new Progress<TranslationProgressInfo>(value =>
            {
                Progress = value;
                StatusMessage = value.StatusText;
            });
            await _service.TranslateAsync(task, Forms.ToList(), SourceLanguage,
                TargetLanguages, progress, _cancellationTokenSource.Token);
            RefreshRows();
            StatusMessage = "表单翻译完成，Form 多语言标签已写回 Aras；日志明细已保存。";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "翻译已取消，已完成的条目保持写回并记录日志。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("表单翻译-执行翻译", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsBusy = false;
        }
    }

    private async Task ExportAsync()
    {
        if (SelectedItemType == null) return;
        IsBusy = true;
        try
        {
            var task = await _service.CreateTaskAsync(
                $"表单导出-{SelectedItemType.Name}", SelectedItemType.Id,
                SourceLanguage, TargetLanguages, Forms.Count);
            var path = await _service.ExportToExcelAsync(task, Forms.ToList());
            StatusMessage = $"表单清单已导出：{path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"导出失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("表单翻译-导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyItemTypeFilter(string? searchText)
    {
        var terms = (searchText ?? string.Empty)
            .Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var filtered = ItemTypes.Where(item => terms.Length == 0 || terms.All(term =>
            item.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            item.Label.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            item.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(40);
        FilteredItemTypes.Clear();
        foreach (var item in filtered) FilteredItemTypes.Add(item);
    }

    private void SetAllSelected(bool value)
    {
        foreach (var form in Forms) form.IsSelected = value;
        RefreshRows();
    }

    private void RefreshRows()
    {
        var rows = Forms.ToList();
        Forms.Clear();
        foreach (var row in rows) Forms.Add(row);
        NotifyCollectionState();
    }

    private void NotifyCollectionState()
    {
        OnPropertyChanged(nameof(SelectionSummary));
        RaiseCommandStates();
    }

    private void RaiseCommandStates()
    {
        (LoadItemTypesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LoadFormsCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SelectAllCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearSelectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (TranslateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
