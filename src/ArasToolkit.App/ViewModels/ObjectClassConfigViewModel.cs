using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Win32;

namespace ArasToolkit.App.ViewModels;

public class ObjectClassConfigViewModel : ObservableObject
{
    private readonly IObjectClassImportService _importService;
    private readonly IErrorLogService _errorLogService;

    private string _selectedFilePath = "";
    private string _statusMessage = "";
    private string _errorMessage = "";
    private bool _isImporting;
    private bool _isLoading;
    private ObjectClassImportResult? _lastResult;

    // 分页
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalCount;

    public ObjectClassConfigViewModel(IObjectClassImportService importService, IErrorLogService errorLogService)
    {
        _importService = importService; _errorLogService = errorLogService;
        HistoryRecords = new ObservableCollection<ObjectClassImportLog>();
        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());
        BrowseFileCommand = new RelayCommand(_ => BrowseFile());
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(), _ => !IsImporting);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => CurrentPage < TotalPages);
        _ = LoadHistoryAsync();
    }

    public string SelectedFilePath { get => _selectedFilePath; set { if (SetProperty(ref _selectedFilePath, value)) StatusMessage = string.IsNullOrEmpty(value) ? "" : $"已选择: {Path.GetFileName(value)}"; } }
    public string FileName => string.IsNullOrEmpty(SelectedFilePath) ? "(未选择文件)" : Path.GetFileName(SelectedFilePath);
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsImporting { get => _isImporting; set { if (SetProperty(ref _isImporting, value)) (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ObjectClassImportResult? LastResult { get => _lastResult; set { if (SetProperty(ref _lastResult, value)) OnPropertyChanged(nameof(HasResult)); } }
    public bool HasResult => LastResult != null;
    public ObservableCollection<ObjectClassImportLog> HistoryRecords { get; }

    // 分页
    public int CurrentPage { get => _currentPage; set { if (SetProperty(ref _currentPage, value)) RefreshPagingCommands(); } }
    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / _pageSize);
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";

    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private void RefreshPagingCommands() { (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); OnPropertyChanged(nameof(TotalPages)); OnPropertyChanged(nameof(PageInfo)); }
    private async Task GoToPageAsync(int page) { if (page < 1 || page > TotalPages) return; CurrentPage = page; await LoadHistoryAsync(); }

    private void DownloadTemplate()
    {
        try
        {
            var dlg = new SaveFileDialog { Title = "保存模板", Filter = "Excel|*.xlsx", DefaultExt = ".xlsx", FileName = "对象类配置模板.xlsx" };
            if (dlg.ShowDialog() != true) return;
            IsLoading = true;
            File.WriteAllBytes(dlg.FileName, _importService.GenerateTemplate());
            StatusMessage = $"模板已保存: {dlg.FileName}";
        }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; _ = _errorLogService.LogErrorAsync("对象类配置-模板", ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
        finally { IsLoading = false; }
    }

    private void BrowseFile()
    {
        var dlg = new OpenFileDialog { Title = "选择文件", Filter = "Excel|*.xlsx;*.xls", CheckFileExists = true };
        if (dlg.ShowDialog() == true) SelectedFilePath = dlg.FileName;
    }

    private async Task ExecuteImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFilePath)) { ErrorMessage = "请先选择文件"; return; }
        IsImporting = true; ErrorMessage = "";
        try
        {
            var prog = new Progress<string>(m => StatusMessage = m);
            LastResult = await _importService.ImportAsync(SelectedFilePath, prog);
            StatusMessage = LastResult.IsSuccess ? $"完成: 对象类{LastResult.Sheet1Count}/关系类{LastResult.Sheet2Count}" : $"失败: {LastResult.ErrorMessage}";
            CurrentPage = 1; await LoadHistoryAsync();
        }
        catch (Exception ex) { ErrorMessage = ex.Message; await _errorLogService.LogErrorAsync("对象类配置-导入", ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
        finally { IsImporting = false; }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, total) = await _importService.GetHistoryAsync(CurrentUserContext.CurrentUserId, CurrentPage, _pageSize);
            HistoryRecords.Clear(); foreach (var i in items) HistoryRecords.Add(i);
            TotalCount = total; RefreshPagingCommands();
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}
