using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

public class PropertyTranslationViewModel : ObservableObject
{
    private readonly IPropertyTranslationService _service;
    private readonly IErrorLogService _errorLogService;

    private string _statusMessage = "";
    private bool _isBusy;
    private string _selectedQueryMode = "Aras连接";
    private ItemTypeItem? _selectedItemType;
    private string _sourceLanguage = "英文";
    private string _targetLanguages = "简体中文,繁体中文";
    private TranslationProgressInfo? _progress;
    private CancellationTokenSource? _cts;

    public PropertyTranslationViewModel(IPropertyTranslationService service, IErrorLogService errorLogService)
    {
        _service = service;
        _errorLogService = errorLogService;

        ItemTypeList = new ObservableCollection<ItemTypeItem>();
        PropertyList = new ObservableCollection<PropertyItem>();
        HistoryList = new ObservableCollection<TranslationTask>();
        QueryModes = new ObservableCollection<string> { "Aras连接", "AML查询", "SQL查询" };

        LoadItemTypesCommand = new RelayCommand(async _ => await LoadItemTypesAsync(), _ => !IsBusy);
        LoadPropertiesCommand = new RelayCommand(async _ => await LoadPropertiesAsync(), _ => !IsBusy && SelectedItemType != null);
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsBusy && PropertyList.Count > 0);
        CancelCommand = new RelayCommand(_ => CancelTranslate(), _ => IsBusy);
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => !IsBusy && PropertyList.Count > 0);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());

        _ = LoadHistoryAsync();
    }

    public ObservableCollection<ItemTypeItem> ItemTypeList { get; }
    public ObservableCollection<PropertyItem> PropertyList { get; }
    public ObservableCollection<TranslationTask> HistoryList { get; }
    public ObservableCollection<string> QueryModes { get; }

    public string SelectedQueryMode
    {
        get => _selectedQueryMode;
        set => SetProperty(ref _selectedQueryMode, value);
    }

    public ItemTypeItem? SelectedItemType
    {
        get => _selectedItemType;
        set { if (SetProperty(ref _selectedItemType, value)) ((RelayCommand)LoadPropertiesCommand).RaiseCanExecuteChanged(); }
    }

    public string SourceLanguage { get => _sourceLanguage; set => SetProperty(ref _sourceLanguage, value); }
    public string TargetLanguages { get => _targetLanguages; set => SetProperty(ref _targetLanguages, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((RelayCommand)LoadItemTypesCommand).RaiseCanExecuteChanged();
                ((RelayCommand)LoadPropertiesCommand).RaiseCanExecuteChanged();
                ((RelayCommand)TranslateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ExportCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    public TranslationProgressInfo? Progress
    {
        get => _progress;
        set
        {
            if (SetProperty(ref _progress, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressText));
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }

    public double ProgressPercentage => Progress?.Percentage ?? 0;
    public string ProgressText => Progress?.StatusText ?? "";
    public bool IsProgressIndeterminate => Progress == null && IsBusy;

    public ICommand LoadItemTypesCommand { get; }
    public ICommand LoadPropertiesCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }

    private async Task LoadItemTypesAsync()
    {
        IsBusy = true;
        StatusMessage = "正在加载对象类列表...";
        try
        {
            var items = await _service.GetItemTypeListAsync();
            ItemTypeList.Clear();
            foreach (var i in items) ItemTypeList.Add(i);
            SelectedItemType = ItemTypeList.FirstOrDefault();
            StatusMessage = $"已加载 {items.Count} 个对象类";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("属性翻译-加载对象类", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadPropertiesAsync()
    {
        if (SelectedItemType == null) return;
        IsBusy = true;
        StatusMessage = $"正在加载对象类 [{SelectedItemType.Name}] 的属性...";
        try
        {
            var props = await _service.GetPropertiesByItemTypeIdAsync(SelectedItemType.Id);
            PropertyList.Clear();
            foreach (var p in props) PropertyList.Add(p);
            StatusMessage = $"已加载 {props.Count} 个属性";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载属性失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("属性翻译-加载属性", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private async Task TranslateAsync()
    {
        if (PropertyList.Count == 0) return;
        IsBusy = true;
        Progress = null;
        _cts = new CancellationTokenSource();
        try
        {
            var task = await _service.CreateTaskAsync(
                $"属性翻译-{SelectedItemType?.Name}", SelectedQueryMode,
                SelectedItemType?.Id ?? "", SourceLanguage, TargetLanguages, PropertyList.Count);

            var progress = new Progress<TranslationProgressInfo>(p =>
            {
                Progress = p;
                StatusMessage = p.StatusText;
            });

            var props = PropertyList.ToList();
            await _service.TranslateAsync(task, props, SourceLanguage, TargetLanguages, progress, _cts.Token);
            StatusMessage = "翻译完成";
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException) { StatusMessage = "翻译已取消"; }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("属性翻译-执行翻译", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; _cts?.Dispose(); _cts = null; }
    }

    private void CancelTranslate() { _cts?.Cancel(); }

    private async Task ExportAsync()
    {
        try
        {
            var task = await _service.CreateTaskAsync(
                $"属性导出-{SelectedItemType?.Name}", SelectedQueryMode,
                SelectedItemType?.Id ?? "", SourceLanguage, TargetLanguages, PropertyList.Count);
            var path = await _service.ExportToExcelAsync(task, PropertyList.ToList());
            StatusMessage = $"已导出: {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"导出失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("属性翻译-导出", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, _) = await _service.GetTaskHistoryAsync();
            HistoryList.Clear();
            foreach (var t in items) HistoryList.Add(t);
        }
        catch { }
    }
}
