using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.App.ViewModels;

public class DatabaseExportViewModel : ObservableObject
{
    private readonly IDatabaseExportService _exportService;
    private readonly ISqlTemplateService _templateService;
    private readonly IDatabaseExportConfigService _exportConfigService;
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IErrorLogService _errorLogService;

    private string _connectionString = "";
    private string _sqlQuery = "";
    private string _selectedExportMode = "一次导出";
    private int _batchSize = 500;
    private string _statusMessage = "";
    private string _errorMessage = "";
    private bool _isExporting;
    private CancellationTokenSource? _cts;
    private ImportProgressInfo? _exportProgress;
    private DatabaseExportResult? _lastResult;
    private ObservableCollection<SqlTemplate> _templates = new();
    private SqlTemplate? _selectedTemplate;
    private string _newTemplateName = "";
    private string _newTemplateDescription = "";
    private int _currentPage = 1;
    private int _totalCount;
    private const int PageSize = 20;

    public DatabaseExportViewModel(
        IDatabaseExportService exportService, ISqlTemplateService templateService,
        IDatabaseExportConfigService exportConfigService,
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IErrorLogService errorLogService)
    {
        _exportService = exportService;
        _templateService = templateService;
        _exportConfigService = exportConfigService;
        _dbFactory = dbFactory;
        _errorLogService = errorLogService;
        HistoryRecords = new ObservableCollection<DatabaseExportLog>();

        ExecuteExportCommand = new RelayCommand(async _ => await ExecuteExportAsync(),
            _ => !IsExporting && !string.IsNullOrWhiteSpace(SqlQuery));
        CancelExportCommand = new RelayCommand(_ => CancelExport(), _ => IsExporting);
        LoadTemplatesCommand = new RelayCommand(async _ => await LoadTemplatesAsync());
        SaveTemplateCommand = new RelayCommand(async _ => await SaveTemplateAsync(),
            _ => !string.IsNullOrWhiteSpace(NewTemplateName) && !string.IsNullOrWhiteSpace(SqlQuery));
        DeleteTemplateCommand = new RelayCommand(async _ => await DeleteTemplateAsync(),
            _ => SelectedTemplate != null);
        UseTemplateCommand = new RelayCommand(_ => { if (SelectedTemplate != null) SqlQuery = SelectedTemplate.SqlContent; },
            _ => SelectedTemplate != null);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => CurrentPage < TotalPages);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await db.EnsureSchemaAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DBExport] Schema同步失败: {ex.Message}");
        }
        await LoadConfigAsync();
        await LoadTemplatesAsync();
        await LoadHistoryAsync();
    }

    public string ConnectionString { get => _connectionString; set => SetProperty(ref _connectionString, value); }
    public string SqlQuery { get => _sqlQuery; set { if (SetProperty(ref _sqlQuery, value)) (ExecuteExportCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public string SelectedExportMode { get => _selectedExportMode; set { if (SetProperty(ref _selectedExportMode, value)) OnPropertyChanged(nameof(IsBatchMode)); } }
    public int BatchSize { get => _batchSize; set { if (SetProperty(ref _batchSize, value)) OnPropertyChanged(nameof(BatchSize)); } }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsExporting { get => _isExporting; set { if (SetProperty(ref _isExporting, value)) { (ExecuteExportCommand as RelayCommand)?.RaiseCanExecuteChanged(); (CancelExportCommand as RelayCommand)?.RaiseCanExecuteChanged(); OnPropertyChanged(nameof(IsProgressIndeterminate)); } } }
    public DatabaseExportResult? LastResult { get => _lastResult; set { if (SetProperty(ref _lastResult, value)) { OnPropertyChanged(nameof(HasResult)); OnPropertyChanged(nameof(HasFailures)); } } }
    public bool HasResult => LastResult != null;
    public bool HasFailures => LastResult?.HasFailures ?? false;
    public bool IsBatchMode => SelectedExportMode == "多次导出";
    public List<string> ExportModeOptions { get; } = new() { "一次导出", "多次导出" };
    public ImportProgressInfo? ExportProgress { get => _exportProgress; set { if (SetProperty(ref _exportProgress, value)) { OnPropertyChanged(nameof(ProgressPercentage)); OnPropertyChanged(nameof(ProgressText)); OnPropertyChanged(nameof(IsProgressIndeterminate)); } } }
    public double ProgressPercentage => ExportProgress?.Percentage ?? 0;
    public string ProgressText => ExportProgress?.StatusText ?? "";
    public bool IsProgressIndeterminate => ExportProgress == null && IsExporting;
    public ObservableCollection<DatabaseExportLog> HistoryRecords { get; }
    public int CurrentPage { get => _currentPage; set { if (SetProperty(ref _currentPage, value)) RefreshPagingCommands(); } }
    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / PageSize);
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";
    public ObservableCollection<SqlTemplate> Templates { get => _templates; set => SetProperty(ref _templates, value); }
    public SqlTemplate? SelectedTemplate { get => _selectedTemplate; set { if (SetProperty(ref _selectedTemplate, value)) { (DeleteTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged(); (UseTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged(); } } }
    public string NewTemplateName { get => _newTemplateName; set { if (SetProperty(ref _newTemplateName, value)) (SaveTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public string NewTemplateDescription { get => _newTemplateDescription; set => SetProperty(ref _newTemplateDescription, value); }

    public ICommand ExecuteExportCommand { get; }
    public ICommand CancelExportCommand { get; }
    public ICommand LoadTemplatesCommand { get; }
    public ICommand SaveTemplateCommand { get; }
    public ICommand DeleteTemplateCommand { get; }
    public ICommand UseTemplateCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private void RefreshPagingCommands() { (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); OnPropertyChanged(nameof(TotalPages)); OnPropertyChanged(nameof(PageInfo)); }
    private async Task GoToPageAsync(int page) { if (page < 1 || page > TotalPages) return; CurrentPage = page; await LoadHistoryAsync(); }

    private static string GetDefaultOutputDir()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var now = DateTime.Now;
        var dir = Path.Combine(baseDir, "Config", "DatabaseExports",
            now.ToString("yyyy_M_d"), now.ToString("HHmmss"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private async Task LoadConfigAsync()
    {
        try
        {
            var config = await _exportConfigService.GetEnabledAsync(CurrentUserContext.CurrentUserId);
            if (config != null)
            {
                ConnectionString = config.ConnectionString;
                StatusMessage = $"已启用: {config.ConfigName} — 点击「执行导出」将结果保存到 Config/DatabaseExports/ 目录";
            }
            else StatusMessage = "未配置导出数据库连接（请在设置中启用一个配置）";
        }
        catch (Exception ex)
        {
            StatusMessage = "加载导出配置失败";
            await _errorLogService.LogErrorAsync("数据库导出-加载配置", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task ExecuteExportAsync()
    {
        if (string.IsNullOrWhiteSpace(SqlQuery)) { ErrorMessage = "请输入SQL查询语句"; return; }
        if (string.IsNullOrWhiteSpace(ConnectionString)) { ErrorMessage = "未配置导出数据库连接，请先在设置中配置"; return; }

        var outputDir = GetDefaultOutputDir();
        IsExporting = true; ErrorMessage = ""; ExportProgress = null; StatusMessage = "正在准备导出..."; LastResult = null;
        _cts = new CancellationTokenSource(); var token = _cts.Token;

        try
        {
            var progress = new Progress<ImportProgressInfo>(p => { ExportProgress = p; StatusMessage = p.StatusText; });
            LastResult = await Task.Run(() => _exportService.ExportAsync(outputDir, SelectedExportMode, BatchSize, SqlQuery, ConnectionString, progress, token), token);
            StatusMessage = LastResult.IsSuccess
                ? $"导出完成: {LastResult.ExportedRows}行 → {LastResult.BatchCount}个文件"
                : $"导出失败: {LastResult.ErrorMessage}";
            if (!LastResult.IsSuccess) ErrorMessage = LastResult.ErrorMessage ?? "导出失败";
            CurrentPage = 1; await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "导出已取消"; ErrorMessage = "导出已被用户取消";
            await _errorLogService.LogErrorAsync("数据库导出-取消", "用户取消导出", ErrorLog.LevelP1, null);
            CurrentPage = 1; await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导出失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库导出-导出", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsExporting = false; _cts?.Dispose(); _cts = null; }
    }

    private void CancelExport() { try { _cts?.Cancel(); StatusMessage = "正在取消导出..."; } catch { } }

    private async Task LoadTemplatesAsync()
    {
        try { var items = await _templateService.GetAllAsync(CurrentUserContext.CurrentUserId); Templates = new ObservableCollection<SqlTemplate>(items); }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DBExport] 加载模板失败: {ex.Message}");
            await _errorLogService.LogErrorAsync("数据库导出-加载模板", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task SaveTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTemplateName) || string.IsNullOrWhiteSpace(SqlQuery)) return;
        try
        {
            var t = new SqlTemplate { Id = "", TemplateName = NewTemplateName, SqlContent = SqlQuery, Description = NewTemplateDescription };
            await _templateService.SaveAsync(t); NewTemplateName = ""; NewTemplateDescription = "";
            StatusMessage = $"模板「{t.TemplateName}」已保存"; await LoadTemplatesAsync();
        }
        catch (Exception ex) { ErrorMessage = $"保存模板失败: {ex.Message}"; }
    }

    private async Task DeleteTemplateAsync()
    {
        if (SelectedTemplate == null) return;
        try { var name = SelectedTemplate.TemplateName; await _templateService.DeleteAsync(SelectedTemplate.Id); StatusMessage = $"模板「{name}」已删除"; await LoadTemplatesAsync(); }
        catch (Exception ex) { ErrorMessage = $"删除模板失败: {ex.Message}"; }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, total) = await _exportService.GetHistoryAsync(CurrentUserContext.CurrentUserId, CurrentPage, PageSize);
            HistoryRecords.Clear(); foreach (var i in items) HistoryRecords.Add(i); TotalCount = total; RefreshPagingCommands();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DBExport] 加载历史失败: {ex.Message}");
            await _errorLogService.LogErrorAsync("数据库导出-加载历史", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }
}
