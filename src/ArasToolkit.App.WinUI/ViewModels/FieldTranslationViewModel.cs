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

public class FieldTranslationViewModel : ObservableObject
{
    private readonly IFieldTranslationService _service;
    private readonly IErrorLogService _errorLogService;

    private string _statusMessage = "";
    private bool _isBusy;
    private string _selectedQueryMode = "Aras连接";
    private ArasFormItem? _selectedForm;
    private string _sourceLanguage = "英文";
    private string _targetLanguages = "简体中文,繁体中文";
    private TranslationProgressInfo? _progress;
    private CancellationTokenSource? _cts;

    public FieldTranslationViewModel(IFieldTranslationService service, IErrorLogService errorLogService)
    {
        _service = service;
        _errorLogService = errorLogService;

        FormList = new ObservableCollection<ArasFormItem>();
        FieldList = new ObservableCollection<FieldItem>();
        HistoryList = new ObservableCollection<TranslationTask>();
        QueryModes = new ObservableCollection<string> { "Aras连接", "AML查询", "SQL查询" };

        LoadFormsCommand = new RelayCommand(async _ => await LoadFormsAsync(), _ => !IsBusy);
        LoadFieldsCommand = new RelayCommand(async _ => await LoadFieldsAsync(), _ => !IsBusy && SelectedForm != null);
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsBusy && FieldList.Count > 0);
        CancelCommand = new RelayCommand(_ => CancelTranslate(), _ => IsBusy);
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => !IsBusy && FieldList.Count > 0);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());

        _ = LoadHistoryAsync();
    }

    public ObservableCollection<ArasFormItem> FormList { get; }
    public ObservableCollection<FieldItem> FieldList { get; }
    public ObservableCollection<TranslationTask> HistoryList { get; }
    public ObservableCollection<string> QueryModes { get; }

    public string SelectedQueryMode
    {
        get => _selectedQueryMode;
        set => SetProperty(ref _selectedQueryMode, value);
    }

    public ArasFormItem? SelectedForm
    {
        get => _selectedForm;
        set { if (SetProperty(ref _selectedForm, value)) ((RelayCommand)LoadFieldsCommand).RaiseCanExecuteChanged(); }
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
                ((RelayCommand)LoadFormsCommand).RaiseCanExecuteChanged();
                ((RelayCommand)LoadFieldsCommand).RaiseCanExecuteChanged();
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

    public ICommand LoadFormsCommand { get; }
    public ICommand LoadFieldsCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }

    private async Task LoadFormsAsync()
    {
        IsBusy = true;
        StatusMessage = "正在加载窗体列表...";
        try
        {
            var forms = await _service.GetFormListAsync();
            FormList.Clear();
            foreach (var f in forms) FormList.Add(f);
            SelectedForm = FormList.FirstOrDefault();
            StatusMessage = $"已加载 {forms.Count} 个窗体";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-加载窗体", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadFieldsAsync()
    {
        if (SelectedForm == null) return;
        IsBusy = true;
        StatusMessage = $"正在加载窗体 [{SelectedForm.Name}] 的字段...";
        try
        {
            var fields = await _service.GetFieldsByFormIdAsync(SelectedForm.Id);
            FieldList.Clear();
            foreach (var f in fields) FieldList.Add(f);
            StatusMessage = $"已加载 {fields.Count} 个字段";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载字段失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-加载字段", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; }
    }

    private async Task TranslateAsync()
    {
        if (FieldList.Count == 0) return;
        IsBusy = true;
        Progress = null;
        _cts = new CancellationTokenSource();
        try
        {
            var task = await _service.CreateTaskAsync(
                $"字段翻译-{SelectedForm?.Name}", SelectedQueryMode,
                SelectedForm?.Id ?? "", SourceLanguage, TargetLanguages, FieldList.Count);

            var progress = new Progress<TranslationProgressInfo>(p =>
            {
                Progress = p;
                StatusMessage = p.StatusText;
            });

            var fields = FieldList.ToList();
            await _service.TranslateAsync(task, fields, SourceLanguage, TargetLanguages, progress, _cts.Token);
            StatusMessage = "翻译完成";
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException) { StatusMessage = "翻译已取消"; }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-执行翻译", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsBusy = false; _cts?.Dispose(); _cts = null; }
    }

    private void CancelTranslate() { _cts?.Cancel(); }

    private async Task ExportAsync()
    {
        try
        {
            var task = await _service.CreateTaskAsync(
                $"字段导出-{SelectedForm?.Name}", SelectedQueryMode,
                SelectedForm?.Id ?? "", SourceLanguage, TargetLanguages, FieldList.Count);
            var path = await _service.ExportToExcelAsync(task, FieldList.ToList());
            StatusMessage = $"已导出: {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"导出失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("字段翻译-导出", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
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
