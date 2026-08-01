using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>按 Aras 对象类加载 Property，并将 AI 翻译结果写回多语言标签。</summary>
public sealed class PropertyTranslationViewModel : ObservableObject
{
    private readonly IPropertyTranslationService _service;
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

    public PropertyTranslationViewModel(
        IPropertyTranslationService service,
        IArasConnectionService connectionService,
        IAiDispatcherService aiDispatcherService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _connectionService = connectionService;
        _aiDispatcherService = aiDispatcherService;
        _errorLogService = errorLogService;

        LoadItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        LoadPropertiesCommand = new RelayCommand(async _ => await LoadPropertiesAsync(),
            _ => !IsBusy && SelectedItemType != null);
        SelectAllCommand = new RelayCommand(_ => SetAllSelected(true), _ => !IsBusy && Properties.Count > 0);
        ClearSelectionCommand = new RelayCommand(_ => SetAllSelected(false), _ => !IsBusy && Properties.Count > 0);
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsBusy && Properties.Count > 0);
        CancelCommand = new RelayCommand(_ => _cancellationTokenSource?.Cancel(), _ => IsBusy);
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => !IsBusy && Properties.Count > 0);
    }

    public ObservableCollection<ItemTypeItem> ItemTypes { get; } = [];
    public ObservableCollection<ItemTypeItem> FilteredItemTypes { get; } = [];
    public ObservableCollection<PropertyItem> Properties { get; } = [];
    public ObservableCollection<string> SourceLanguages { get; } =
        ["简体中文", "繁体中文", "英文", "日文", "韩文", "法文", "德文", "西班牙文"];

    public ItemTypeItem? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value)) return;
            Properties.Clear();
            NotifyCollectionState();
            (LoadPropertiesCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
    public string SelectionSummary => $"共 {Properties.Count} 个字段，已选择 {Properties.Count(item => item.IsSelected)} 个";
    public string ItemTypeFilterSummary => $"显示 {FilteredItemTypes.Count}/{ItemTypes.Count} 个对象类";

    public ICommand LoadItemTypesCommand { get; }
    public ICommand LoadPropertiesCommand { get; }
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
            await _errorLogService.LogErrorAsync("字段翻译-读取AI配置", ex.Message,
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
            StatusMessage = $"已加载 {ItemTypes.Count} 个对象类；搜索仅筛选本地列表，不会请求 Aras。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"对象类加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-加载对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadPropertiesAsync()
    {
        if (SelectedItemType == null) return;
        IsBusy = true;
        StatusMessage = $"正在加载 {SelectedItemType.DisplayName} 的字段…";
        try
        {
            var properties = await _service.GetPropertiesByItemTypeIdAsync(SelectedItemType.Id);
            Properties.Clear();
            foreach (var property in properties) Properties.Add(property);
            NotifyCollectionState();
            StatusMessage = $"已加载 {Properties.Count} 个字段，默认全部选中。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"字段加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-加载字段", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task TranslateAsync()
    {
        var selected = Properties.Where(item => item.IsSelected).ToList();
        if (SelectedItemType == null || selected.Count == 0)
        {
            StatusMessage = "请先选择至少一个字段。";
            return;
        }

        IsBusy = true;
        Progress = null;
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var task = await _service.CreateTaskAsync(
                $"字段翻译-{SelectedItemType.Name}", "字段翻译", SelectedItemType.Id,
                SourceLanguage, TargetLanguages, selected.Count);
            var progress = new Progress<TranslationProgressInfo>(value =>
            {
                Progress = value;
                StatusMessage = value.StatusText;
            });
            await _service.TranslateAsync(task, Properties.ToList(), SourceLanguage,
                TargetLanguages, progress, _cancellationTokenSource.Token);
            RefreshRows();
            StatusMessage = $"字段翻译完成，已写回 Aras；可在“系统日志 → Aras翻译日志”查看明细。";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "翻译已取消，已完成的条目保持写回并记录日志。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-执行翻译", ex.Message,
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
                $"字段导出-{SelectedItemType.Name}", "字段翻译", SelectedItemType.Id,
                SourceLanguage, TargetLanguages, Properties.Count);
            var path = await _service.ExportToExcelAsync(task, Properties.ToList());
            StatusMessage = $"字段清单已导出：{path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"导出失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-导出", ex.Message,
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
            item.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase)));
        FilteredItemTypes.Clear();
        foreach (var item in filtered) FilteredItemTypes.Add(item);
        OnPropertyChanged(nameof(ItemTypeFilterSummary));

        var selectedId = SelectedItemType?.Id;
        var selected = FilteredItemTypes.FirstOrDefault(item => item.Id == selectedId)
                       ?? FilteredItemTypes.FirstOrDefault();
        if (!ReferenceEquals(selected, SelectedItemType))
            SelectedItemType = selected;
    }

    private void SetAllSelected(bool value)
    {
        foreach (var property in Properties) property.IsSelected = value;
        RefreshRows();
    }

    private void RefreshRows()
    {
        var rows = Properties.ToList();
        Properties.Clear();
        foreach (var row in rows) Properties.Add(row);
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
        (LoadPropertiesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SelectAllCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearSelectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (TranslateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
