using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

#if WPF
namespace ArasToolkit.App.ViewModels;
#else
namespace ArasToolkit.App.WinUI.ViewModels;
#endif

/// <summary>
/// 数据库修改模式视图模型。
/// 同一份平台无关代码由 WinUI 3 与 WPF 复用，界面差异通过 IDialogService/IFileDialogService 隔离。
/// </summary>
public sealed class DatabaseModificationViewModel : ObservableObject
{
    // ===== 业务服务与当前连接 =====
    private readonly IDatabaseModificationService _modificationService;
    private readonly IDatabaseExportConfigService _connectionConfigService;
    private readonly IErrorLogService _errorLogService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IArasConnectionService _arasConnectionService;

    private string _selectedFilePath = string.Empty;
    private string? _selectedSheetName;
    private int _startRow = 2;
    private int _endRow = -1;
    private int _startColumn = 1;
    private int _endColumn = -1;
    private int _maxConcurrency = 4;
    private string _sqlTemplate = "UPDATE dbo.YourTable SET target_column = @B WHERE id = @A;";
    private string _templateName = string.Empty;
    private string _statusMessage = "请选择 Excel 模板并确认已启用数据库导出连接配置。";
    private string _errorMessage = string.Empty;
    private string _sqlPreviewText = string.Empty;
    private string _activeConnectionName = "未启用连接";
    private DataTable? _excelPreview;
    private bool _isBusy;
    private bool _isExecuting;
    private double _progressPercent;
    private string _progressText = string.Empty;
    private DatabaseExportConfig? _activeConnection;
    private SqlTemplate? _selectedTemplate;
    private DatabaseModificationResult? _lastResult;
    private CancellationTokenSource? _cancellationTokenSource;
    private string _selectedExecutionMode = DatabaseModificationExecutionModes.Orm;

    public DatabaseModificationViewModel(
        IDatabaseModificationService modificationService,
        IDatabaseExportConfigService connectionConfigService,
        IErrorLogService errorLogService,
        IDialogService dialogService,
        IFileDialogService fileDialogService,
        IArasConnectionService arasConnectionService)
    {
        _modificationService = modificationService;
        _connectionConfigService = connectionConfigService;
        _errorLogService = errorLogService;
        _dialogService = dialogService;
        _fileDialogService = fileDialogService;
        _arasConnectionService = arasConnectionService;

        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync(), _ => !IsBusy);
        PreviewExcelCommand = new RelayCommand(async _ => await PreviewExcelAsync(), _ => CanPreviewExcel());
        PreviewSqlCommand = new RelayCommand(async _ => await PreviewSqlAsync(), _ => CanPreviewSql());
        SaveTemplateCommand = new RelayCommand(async _ => await SaveTemplateAsync(), _ => CanSaveTemplate());
        NewTemplateCommand = new RelayCommand(_ => ResetTemplate(), _ => !IsBusy);
        DeleteTemplateCommand = new RelayCommand(async _ => await DeleteTemplateAsync(), _ => SelectedTemplate != null && !IsBusy);
        ExecuteCommand = new RelayCommand(async _ => await ExecuteAsync(), _ => CanExecute());
        CancelCommand = new RelayCommand(_ => Cancel(), _ => IsExecuting);
        RefreshConnectionCommand = new RelayCommand(async _ => await RefreshExecutionTargetAsync(), _ => !IsBusy);

        _ = InitializeAsync();
    }

    // ===== Excel 范围与执行方式 =====
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (SetProperty(ref _selectedFilePath, value))
            {
                OnPropertyChanged(nameof(FileName));
                RefreshCommands();
            }
        }
    }

    public string FileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "未选择 Excel 文件"
        : Path.GetFileName(SelectedFilePath);

    public ObservableCollection<string> SheetNames { get; } = [];
    public IReadOnlyList<string> ExecutionModes { get; } =
        [DatabaseModificationExecutionModes.Aras, DatabaseModificationExecutionModes.Orm];

    public string SelectedExecutionMode
    {
        get => _selectedExecutionMode;
        set
        {
            if (SetProperty(ref _selectedExecutionMode, value))
            {
                UpdateExecutionTarget();
                RefreshCommands();
            }
        }
    }

    public string? SelectedSheetName
    {
        get => _selectedSheetName;
        set
        {
            if (SetProperty(ref _selectedSheetName, value))
                RefreshCommands();
        }
    }

    public int StartRow { get => _startRow; set => SetProperty(ref _startRow, value); }
    public int EndRow { get => _endRow; set => SetProperty(ref _endRow, value); }
    public int StartColumn { get => _startColumn; set => SetProperty(ref _startColumn, value); }
    public int EndColumn { get => _endColumn; set => SetProperty(ref _endColumn, value); }

    public int MaxConcurrency
    {
        get => _maxConcurrency;
        set => SetProperty(ref _maxConcurrency, Math.Clamp(value, 1, 10));
    }

    public string SqlTemplate
    {
        get => _sqlTemplate;
        set
        {
            if (SetProperty(ref _sqlTemplate, value))
                RefreshCommands();
        }
    }

    public string TemplateName
    {
        get => _templateName;
        set
        {
            if (SetProperty(ref _templateName, value))
                RefreshCommands();
        }
    }

    // ===== 可复用 SQL 方案；Service 会用作用域标记与只读导出模板隔离 =====
    public ObservableCollection<SqlTemplate> SavedTemplates { get; } = [];

    public SqlTemplate? SelectedTemplate
    {
        get => _selectedTemplate;
        set
        {
            if (!SetProperty(ref _selectedTemplate, value))
                return;
            if (value != null)
            {
                TemplateName = value.TemplateName;
                SqlTemplate = value.SqlContent;
                StatusMessage = $"已加载 SQL 方案: {value.TemplateName}";
            }
            RefreshCommands();
        }
    }

    public ObservableCollection<ColumnMapping> ColumnMappings { get; } = [];
    public bool HasColumnMappings => ColumnMappings.Count > 0;

    public string ColumnMappingHint => ColumnMappings.Count == 0
        ? "加载 Excel 预览后显示占位符映射"
        : string.Join("    ", ColumnMappings.Select(mapping => $"@{mapping.Letter} = {mapping.Header}"));

    public DataTable? ExcelPreview
    {
        get => _excelPreview;
        set
        {
            if (SetProperty(ref _excelPreview, value))
                OnPropertyChanged(nameof(HasExcelPreview));
        }
    }

    public bool HasExcelPreview => ExcelPreview?.Rows.Count > 0;

    public string SqlPreviewText
    {
        get => _sqlPreviewText;
        set
        {
            if (SetProperty(ref _sqlPreviewText, value))
                OnPropertyChanged(nameof(HasSqlPreview));
        }
    }

    public bool HasSqlPreview => !string.IsNullOrWhiteSpace(SqlPreviewText);
    public string ActiveConnectionName { get => _activeConnectionName; set => SetProperty(ref _activeConnectionName, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                RefreshCommands();
        }
    }

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            if (SetProperty(ref _isExecuting, value))
                RefreshCommands();
        }
    }

    public double ProgressPercent { get => _progressPercent; set => SetProperty(ref _progressPercent, value); }
    public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }

    public DatabaseModificationResult? LastResult
    {
        get => _lastResult;
        set
        {
            if (SetProperty(ref _lastResult, value))
                OnPropertyChanged(nameof(HasResult));
        }
    }

    public bool HasResult => LastResult != null;

    public ICommand BrowseFileCommand { get; }
    public ICommand PreviewExcelCommand { get; }
    public ICommand PreviewSqlCommand { get; }
    public ICommand SaveTemplateCommand { get; }
    public ICommand NewTemplateCommand { get; }
    public ICommand DeleteTemplateCommand { get; }
    public ICommand ExecuteCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand RefreshConnectionCommand { get; }

    /// <summary>页面启动时并行加载 SQL 方案与 ORM 连接配置。</summary>
    private async Task InitializeAsync()
    {
        await Task.WhenAll(LoadTemplatesAsync(), LoadActiveConnectionAsync());
        UpdateExecutionTarget();
    }

    /// <summary>选择工作簿并加载 Sheet 名称，不在此阶段读取全部数据。</summary>
    private async Task BrowseFileAsync()
    {
        try
        {
            var path = await _fileDialogService.PickOpenFileAsync("选择数据库修改模板", ".xlsx", ".xlsm");
            if (string.IsNullOrWhiteSpace(path))
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            SelectedFilePath = path;
            SheetNames.Clear();
            var sheets = await _modificationService.GetSheetNamesAsync(path);
            foreach (var sheet in sheets)
                SheetNames.Add(sheet);
            SelectedSheetName = sheets.FirstOrDefault();
            ExcelPreview = null;
            SqlPreviewText = string.Empty;
            StatusMessage = $"已读取 {sheets.Count} 个工作表，请预览前 20 行。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"读取 Excel 失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-选择文件", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>预览所选范围前 20 行，同时刷新 @A/@B 列映射。</summary>
    private async Task PreviewExcelAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var preview = await _modificationService.PreviewExcelAsync(BuildRequest(), 20);
            ApplyPreview(preview);
            SqlPreviewText = string.Empty;
            StatusMessage = $"Excel 预览已加载：显示 {preview.ExcelData.Rows.Count} 行，共 {preview.TotalDataRows} 行有效数据。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Excel 预览失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-Excel预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>把预览行转换成可读 SQL；真正执行时 ORM 仍使用参数化命令。</summary>
    private async Task PreviewSqlAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var preview = await _modificationService.PreviewSqlAsync(BuildRequest(), 20);
            ApplyPreview(preview);
            SqlPreviewText = string.Join(Environment.NewLine + Environment.NewLine,
                preview.SqlStatements.Select((sql, index) => $"-- 预览 {index + 1}{Environment.NewLine}{sql}"));
            StatusMessage = $"SQL 预组装完成：显示 {preview.SqlStatements.Count} 条。实际执行使用参数化命令。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"SQL 预览失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-SQL预览", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>仅加载数据库修改作用域内的 SQL 方案。</summary>
    private async Task LoadTemplatesAsync()
    {
        try
        {
            var templates = await _modificationService.GetSavedTemplatesAsync(CurrentUserContext.CurrentUserId);
            SavedTemplates.Clear();
            foreach (var template in templates)
                SavedTemplates.Add(template);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载 SQL 方案失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-加载SQL方案", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task SaveTemplateAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var template = new SqlTemplate
            {
                Id = SelectedTemplate?.Id ?? string.Empty,
                TemplateName = TemplateName.Trim(),
                SqlContent = SqlTemplate.Trim(),
                UserId = CurrentUserContext.CurrentUserId,
                CreatorOn = SelectedTemplate?.CreatorOn ?? DateTime.Now
            };
            await _modificationService.SaveTemplateAsync(template);
            await LoadTemplatesAsync();
            SelectedTemplate = SavedTemplates.FirstOrDefault(item => item.Id == template.Id)
                ?? SavedTemplates.FirstOrDefault(item => item.TemplateName == template.TemplateName);
            StatusMessage = $"SQL 方案已保存: {template.TemplateName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存 SQL 方案失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-保存SQL方案", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteTemplateAsync()
    {
        if (SelectedTemplate == null)
            return;

        try
        {
            var confirmed = await _dialogService.ConfirmAsync(
                "删除 SQL 方案",
                $"确定删除“{SelectedTemplate.TemplateName}”吗？",
                "删除");
            if (!confirmed)
                return;

            IsBusy = true;
            var id = SelectedTemplate.Id;
            await _modificationService.DeleteTemplateAsync(id);
            ResetTemplate();
            await LoadTemplatesAsync();
            StatusMessage = "SQL 方案已删除。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除 SQL 方案失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-删除SQL方案", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>刷新数据库导出模块中当前启用的连接，供 ORM 模式直接复用。</summary>
    private async Task LoadActiveConnectionAsync()
    {
        try
        {
            _activeConnection = await _connectionConfigService.GetEnabledAsync(CurrentUserContext.CurrentUserId);
            UpdateExecutionTarget();
            RefreshCommands();
        }
        catch (Exception ex)
        {
            _activeConnection = null;
            UpdateExecutionTarget();
            ErrorMessage = ex.Message;
            await _errorLogService.LogErrorAsync("数据库修改-加载连接配置", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    /// <summary>刷新当前所选方式对应的连接；Aras 方式不额外访问 ORM 配置库。</summary>
    private async Task RefreshExecutionTargetAsync()
    {
        if (SelectedExecutionMode == DatabaseModificationExecutionModes.Orm)
            await LoadActiveConnectionAsync();
        else
            UpdateExecutionTarget();
    }

    /// <summary>
    /// 二次预览并确认后执行：Aras 方式调用当前 Innovator.applySQL，ORM 模式直连 SQL Server。
    /// </summary>
    private async Task ExecuteAsync()
    {
        try
        {
            if (SelectedExecutionMode == DatabaseModificationExecutionModes.Orm)
                await LoadActiveConnectionAsync();
            else
                UpdateExecutionTarget();

            if (SelectedExecutionMode == DatabaseModificationExecutionModes.Orm && _activeConnection == null)
                throw new InvalidOperationException("未找到已启用的数据库导出连接配置。");
            if (SelectedExecutionMode == DatabaseModificationExecutionModes.Aras && !_arasConnectionService.IsConnected)
                throw new InvalidOperationException("Aras 方式需要先启动并登录 Aras 连接。");

            var preview = await _modificationService.PreviewSqlAsync(BuildRequest(), 20);
            ApplyPreview(preview);
            SqlPreviewText = string.Join(Environment.NewLine + Environment.NewLine,
                preview.SqlStatements.Select((sql, index) => $"-- 预览 {index + 1}{Environment.NewLine}{sql}"));

            var targetName = SelectedExecutionMode == DatabaseModificationExecutionModes.Aras
                ? _arasConnectionService.CurrentConnection?.Database ?? "当前 Aras 连接"
                : _activeConnection!.ConfigName;
            var workerCount = SelectedExecutionMode == DatabaseModificationExecutionModes.Aras ? 1 : MaxConcurrency;
            var confirmed = await _dialogService.ConfirmAsync(
                "执行数据库修改",
                $"即将通过“{SelectedExecutionMode}”使用“{targetName}”处理 {preview.TotalDataRows} 行数据，"
                + $"并发 Worker 为 {workerCount}。此操作会直接修改数据库，请确认已检查 SQL 预览。",
                "确认执行");
            if (!confirmed)
                return;

            IsExecuting = true;
            IsBusy = true;
            ErrorMessage = string.Empty;
            ProgressPercent = 0;
            ProgressText = "正在创建短生命周期执行 Worker...";
            _cancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<DatabaseModificationProgress>(value =>
            {
                ProgressPercent = value.TotalRows == 0
                    ? 0
                    : (double)value.ProcessedRows / value.TotalRows * 100;
                ProgressText = $"{value.Message}，成功 {value.SuccessCount}，失败 {value.FailureCount}";
            });

            LastResult = await _modificationService.ExecuteAsync(
                BuildRequest(),
                SelectedExecutionMode == DatabaseModificationExecutionModes.Orm
                    ? _activeConnection!.ConnectionString
                    : string.Empty,
                progress,
                _cancellationTokenSource.Token);

            if (LastResult.IsCancelled)
            {
                StatusMessage = "执行已取消，已完成的数据库语句不会回滚。";
            }
            else
            {
                ProgressPercent = 100;
                ProgressText = $"完成：成功 {LastResult.SuccessCount}，失败 {LastResult.FailureCount}";
                StatusMessage = $"数据库修改完成。日志：{LastResult.LogFilePath}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据库修改失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("数据库修改-执行", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsExecuting = false;
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        ProgressText = "正在停止新任务并释放数据库连接...";
    }

    private void ResetTemplate()
    {
        SelectedTemplate = null;
        TemplateName = string.Empty;
        SqlTemplate = "UPDATE dbo.YourTable SET target_column = @B WHERE id = @A;";
        SqlPreviewText = string.Empty;
        StatusMessage = "已新建空白 SQL 方案。";
    }

    private void ApplyPreview(DatabaseModificationPreview preview)
    {
        ExcelPreview = preview.ExcelData;
        ColumnMappings.Clear();
        foreach (var mapping in preview.ColumnMappings)
            ColumnMappings.Add(mapping);
        OnPropertyChanged(nameof(HasColumnMappings));
        OnPropertyChanged(nameof(ColumnMappingHint));
        RefreshCommands();
    }

    /// <summary>集中组装请求，保证双预览与正式执行使用完全相同的范围参数。</summary>
    private DatabaseModificationRequest BuildRequest() => new()
    {
        FilePath = SelectedFilePath,
        SheetName = SelectedSheetName ?? string.Empty,
        StartRow = StartRow,
        EndRow = EndRow,
        StartColumn = StartColumn,
        EndColumn = EndColumn,
        SqlTemplate = SqlTemplate,
        MaxConcurrency = MaxConcurrency,
        ExecutionMode = SelectedExecutionMode
    };

    private bool CanPreviewExcel() => !IsBusy
        && !string.IsNullOrWhiteSpace(SelectedFilePath)
        && !string.IsNullOrWhiteSpace(SelectedSheetName);

    private bool CanPreviewSql() => CanPreviewExcel() && !string.IsNullOrWhiteSpace(SqlTemplate);
    private bool CanSaveTemplate() => !IsBusy
        && !string.IsNullOrWhiteSpace(TemplateName)
        && !string.IsNullOrWhiteSpace(SqlTemplate);

    private bool CanExecute() => !IsBusy
        && (SelectedExecutionMode == DatabaseModificationExecutionModes.Aras
            ? _arasConnectionService.IsConnected
            : _activeConnection != null)
        && CanPreviewSql();

    /// <summary>根据执行方式展示真实目标，避免用户误把 Aras 与 ORM 连接混淆。</summary>
    private void UpdateExecutionTarget()
    {
        ActiveConnectionName = SelectedExecutionMode == DatabaseModificationExecutionModes.Aras
            ? (_arasConnectionService.IsConnected
                ? $"当前 Aras：{_arasConnectionService.CurrentConnection?.Url} / {_arasConnectionService.CurrentConnection?.Database}"
                : "当前没有已启动的 Aras 连接")
            : (_activeConnection == null
                ? "未启用 ORM 连接，请到设置 → 数据库导出连接配置"
                : $"ORM 已启用：{_activeConnection.ConfigName}");
    }

    private void RefreshCommands()
    {
        (BrowseFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviewExcelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviewSqlCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NewTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ExecuteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshConnectionCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
