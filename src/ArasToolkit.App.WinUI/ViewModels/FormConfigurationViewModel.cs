using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>Aras 经典窗体配置页面 ViewModel。</summary>
public sealed class FormConfigurationViewModel : ObservableObject, IDisposable
{
    private readonly IFormConfigurationService _formService;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;
    private readonly IArasConnectionService _connectionService;
    private readonly DispatcherQueue? _dispatcherQueue;

    private ArasItemTypeInfo? _selectedItemType;
    private string _formName = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _replaceExisting;
    private bool _setAsDefaultView = true;
    private bool _initialized;
    private string _lastSuggestedFormName = string.Empty;
    private bool _reloadPending;
    private bool _disposed;
    private string _itemTypeSearchText = string.Empty;

    public FormConfigurationViewModel(
        IFormConfigurationService formService,
        IDialogService dialogService,
        IErrorLogService errorLogService,
        IArasConnectionService connectionService)
    {
        _formService = formService;
        _dialogService = dialogService;
        _errorLogService = errorLogService;
        _connectionService = connectionService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _connectionService.ConnectionChanged += OnConnectionChanged;

        RefreshItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        LoadPropertiesCommand = new RelayCommand(async _ => await LoadPropertiesAsync(),
            _ => SelectedItemType != null && !IsBusy);
        BuildPreviewCommand = new RelayCommand(_ => BuildPreview(), _ => Properties.Count > 0 && !IsBusy);
        SelectAllCommand = new RelayCommand(_ => SetAllSelections(true), _ => Properties.Count > 0 && !IsBusy);
        ClearAllCommand = new RelayCommand(_ => SetAllSelections(false), _ => Properties.Count > 0 && !IsBusy);
        ApplyCommand = new RelayCommand(async _ => await ApplyAsync(),
            _ => SelectedItemType != null && LayoutFields.Count > 0 &&
                 !string.IsNullOrWhiteSpace(FormName) && !IsBusy);
    }

    public ObservableCollection<ArasItemTypeInfo> ItemTypes { get; } = [];
    public ObservableCollection<ArasItemTypeInfo> FilteredItemTypes { get; } = [];
    public ObservableCollection<ArasFormProperty> Properties { get; } = [];
    public ObservableCollection<ArasFormFieldLayout> LayoutFields { get; } = [];

    public ArasItemTypeInfo? SelectedItemType
    {
        get => _selectedItemType;
        set
        {
            if (!SetProperty(ref _selectedItemType, value))
                return;

            Properties.Clear();
            LayoutFields.Clear();
            OnPropertyChanged(nameof(PropertySummary));
            OnPropertyChanged(nameof(LayoutSummary));

            if (value != null &&
                (string.IsNullOrWhiteSpace(FormName) || FormName == _lastSuggestedFormName))
            {
                _lastSuggestedFormName = $"{value.Name} 自动窗体";
                FormName = _lastSuggestedFormName;
            }

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

    public string FormName
    {
        get => _formName;
        set
        {
            if (SetProperty(ref _formName, value))
                RefreshCommands();
        }
    }

    public bool ReplaceExisting
    {
        get => _replaceExisting;
        set => SetProperty(ref _replaceExisting, value);
    }

    public bool SetAsDefaultView
    {
        get => _setAsDefaultView;
        set => SetProperty(ref _setAsDefaultView, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value))
                return;
            RefreshCommands();
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
    public string PropertySummary => $"已加载 {Properties.Count} 个未勾选“搜索中隐藏”的属性";
    public string LayoutSummary => $"预览 {LayoutFields.Count} 个字段 · 四列布局 · 起点 (50, 50)";

    public ICommand RefreshItemTypesCommand { get; }
    public ICommand LoadPropertiesCommand { get; }
    public ICommand BuildPreviewCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand ClearAllCommand { get; }
    public ICommand ApplyCommand { get; }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        _initialized = true;
        await LoadItemTypesAsync();
    }

    private async Task LoadItemTypesAsync()
    {
        if (!_connectionService.IsConnected)
        {
            ClearConnectionData();
            ClearMessages();
            StatusMessage = "尚未连接 Aras；当前用户的默认连接成功后将自动加载对象类。";
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
            StatusMessage = "正在从 Aras 加载对象类...";
            var itemTypes = await _formService.GetItemTypesAsync();

            ItemTypes.Clear();
            foreach (var itemType in itemTypes)
                ItemTypes.Add(itemType);

            ApplyItemTypeFilter(ItemTypeSearchText);

            SelectedItemType = ItemTypes.FirstOrDefault();
            var current = _connectionService.CurrentConnection;
            var connectionName = current == null
                ? "当前 Aras 连接"
                : $"{current.Username}@{current.Database}";
            StatusMessage = $"已通过默认连接 {connectionName} 加载 {ItemTypes.Count} 个对象类。";
        }
        catch (Exception ex)
        {
            StatusMessage = string.Empty;
            ErrorMessage = $"对象类加载失败：{ex.Message}";
            await LogViewModelErrorAsync("窗体配置-加载对象类", ex);
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

    private async Task LoadPropertiesAsync()
    {
        if (SelectedItemType == null)
            return;

        if (!_connectionService.IsConnected)
        {
            StatusMessage = string.Empty;
            ErrorMessage = "Aras 连接已断开，请先在“Aras连接”中恢复当前用户的默认连接。";
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();
            StatusMessage = $"正在加载 {SelectedItemType.DisplayName} 的可见搜索属性...";
            var properties = await _formService.GetVisiblePropertiesAsync(SelectedItemType.Id);

            Properties.Clear();
            foreach (var property in properties)
                Properties.Add(property);

            OnPropertyChanged(nameof(PropertySummary));
            BuildPreview();
            StatusMessage = PropertySummary;
        }
        catch (Exception ex)
        {
            StatusMessage = string.Empty;
            ErrorMessage = $"属性加载失败：{ex.Message}";
            await LogViewModelErrorAsync("窗体配置-加载属性", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BuildPreview()
    {
        LayoutFields.Clear();
        foreach (var field in _formService.BuildDefaultLayout(Properties))
            LayoutFields.Add(field);

        OnPropertyChanged(nameof(LayoutSummary));
        StatusMessage = LayoutFields.Count == 0
            ? "未选择任何属性。"
            : $"布局已生成：{LayoutFields.Count} 个字段，第一行优先为创建者、创建时间、状态、编号。";
        RefreshCommands();
    }

    private void SetAllSelections(bool selected)
    {
        foreach (var property in Properties)
            property.IsSelected = selected;
        BuildPreview();
    }

    private async Task ApplyAsync()
    {
        if (SelectedItemType == null)
            return;

        if (!_connectionService.IsConnected)
        {
            StatusMessage = string.Empty;
            ErrorMessage = "Aras 连接已断开，无法写入窗体配置。";
            return;
        }

        BuildPreview();
        if (LayoutFields.Count == 0)
        {
            ErrorMessage = "至少需要选择一个属性。";
            return;
        }

        var overwriteText = ReplaceExisting ? "；若同名窗体存在，将替换其原有字段" : string.Empty;
        var viewText = SetAsDefaultView ? "，并设为对象类默认视图" : string.Empty;
        var confirmed = await _dialogService.ConfirmAsync(
            "确认写入 Aras",
            $"将为对象类“{SelectedItemType.DisplayName}”写入窗体“{FormName}”，共 {LayoutFields.Count} 个字段{viewText}{overwriteText}。是否继续？",
            "写入窗体",
            "取消");
        if (!confirmed)
            return;

        try
        {
            IsBusy = true;
            ClearMessages();
            StatusMessage = "正在安全写入 Aras 窗体配置...";
            var result = await _formService.ApplyAsync(new ArasFormConfigurationRequest
            {
                ItemTypeId = SelectedItemType.Id,
                ItemTypeName = SelectedItemType.Name,
                FormName = FormName.Trim(),
                FormLabel = FormName.Trim(),
                ReplaceExisting = ReplaceExisting,
                SetAsDefaultView = SetAsDefaultView,
                Fields = LayoutFields.ToList()
            });

            StatusMessage = $"{(result.WasUpdated ? "已覆盖" : "已创建")}窗体“{result.FormName}”，写入 {result.FieldCount} 个字段。";
            await _dialogService.AlertAsync("窗体配置完成", StatusMessage);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Empty;
            ErrorMessage = $"窗体写入失败：{ex.Message}";
            await LogViewModelErrorAsync("窗体配置-写入窗体", ex);
        }
        finally
        {
            IsBusy = false;
        }
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
            // 错误日志服务失败不能覆盖原业务异常。
        }
    }

    private void OnConnectionChanged()
    {
        if (_disposed)
            return;

        if (_dispatcherQueue == null)
            return;

        if (_dispatcherQueue.HasThreadAccess)
        {
            _ = RefreshForConnectionChangeAsync();
            return;
        }

        _dispatcherQueue.TryEnqueue(() => _ = RefreshForConnectionChangeAsync());
    }

    private async Task RefreshForConnectionChangeAsync()
    {
        if (!_initialized || _disposed)
            return;

        if (!_connectionService.IsConnected)
        {
            ClearConnectionData();
            ErrorMessage = string.Empty;
            StatusMessage = "Aras 连接已断开；连接当前用户的默认配置后将自动重新加载。";
            return;
        }

        if (IsBusy)
        {
            _reloadPending = true;
            return;
        }

        await LoadItemTypesAsync();
    }

    private void ClearConnectionData()
    {
        ItemTypes.Clear();
        FilteredItemTypes.Clear();
        SelectedItemType = null;
        Properties.Clear();
        LayoutFields.Clear();
        OnPropertyChanged(nameof(PropertySummary));
        OnPropertyChanged(nameof(LayoutSummary));
        RefreshCommands();
    }

    public void SelectItemTypeFromSearch(ArasItemTypeInfo itemType)
    {
        SelectedItemType = itemType;
        ItemTypeSearchText = itemType.DisplayName;
    }

    private void ApplyItemTypeFilter(string? searchText)
    {
        FilteredItemTypes.Clear();
        if (string.IsNullOrWhiteSpace(searchText))
            return;

        var terms = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var itemType in ItemTypes.Where(item => terms.All(term =>
                     item.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     item.Label.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                     item.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))).Take(30))
        {
            FilteredItemTypes.Add(itemType);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _connectionService.ConnectionChanged -= OnConnectionChanged;
        GC.SuppressFinalize(this);
    }

    private void RefreshCommands()
    {
        (RefreshItemTypesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LoadPropertiesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (BuildPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SelectAllCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ClearAllCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
