using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Core.Models.Translation;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ArasToolkit.App.ViewModels;

public class TextTranslationViewModel : ObservableObject
{
    private readonly ITextTranslationService _translationService;
    private readonly IErrorLogService _errorLogService;

    private string _sourceFilePath = "";
    private string? _selectedTemplateType;
    private string? _selectedSourceLanguage;
    private string _customPrompt = "";
    private string _statusMessage = "";
    private bool _isTranslating;

    // ===== 「源文本自定义翻译」模式专属 =====
    private string? _selectedSheetName;
    private ExcelColumnInfo? _selectedSourceColumn;
    private ExcelColumnInfo? _selectedTargetColumn;
    private string? _selectedTargetLanguage;
    private string _sourceTextPreview = "";
    private ObservableCollection<string>? _sheetNames;
    private ObservableCollection<ExcelColumnInfo>? _sourceColumns;
    private ObservableCollection<ExcelColumnInfo>? _targetColumns;
    private ObservableCollection<string>? _targetLanguages;

    // ===== 分页相关 =====
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalCount;
    private const int PageSizeOptions = 20;

    // ===== 进度 + 取消 =====
    private TranslationProgressInfo? _progress;
    private CancellationTokenSource? _cts;

    public TextTranslationViewModel(
        ITextTranslationService translationService,
        IErrorLogService errorLogService)
    {
        _translationService = translationService;
        _errorLogService = errorLogService;

        TemplateTypes = new ObservableCollection<string> { "Aras翻译", "自定义翻译", "源文本自定义翻译" };
        SelectedTemplateType = TemplateTypes[0];
        SourceLanguages = new ObservableCollection<string>
        {
            "中文", "繁體中文", "英文", "越南", "柬埔寨",
            "日本", "韩国", "泰国", "印尼", "法国", "德国", "西班牙"
        };
        SelectedSourceLanguage = SourceLanguages[0];
        HistoryRecords = new ObservableCollection<TextTranslationRecord>();

        BrowseSourceCommand = new RelayCommand(_ => BrowseSourceFile());
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsTranslating);
        CancelCommand = new RelayCommand(_ => CancelTranslate(), _ => IsTranslating);
        OpenOutputFileCommand = new RelayCommand(_ => OpenOutputFile());
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());
        PrevPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => CurrentPage > 1);
        NextPageCommand = new RelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => CurrentPage < TotalPages);
        LoadColumnsCommand = new RelayCommand(async _ => await LoadColumnsAsync(SelectedSheetName));
        CopySourceTextCommand = new RelayCommand(_ => CopySourceText());

        _ = LoadHistoryAsync();
    }

    public string SourceFilePath
    {
        get => _sourceFilePath;
        set { if (SetProperty(ref _sourceFilePath, value)) StatusMessage = string.IsNullOrEmpty(value) ? "" : $"已选择: {Path.GetFileName(value)}"; }
    }
    public string? SelectedTemplateType
    {
        get => _selectedTemplateType;
        set { if (SetProperty(ref _selectedTemplateType, value)) { OnPropertyChanged(nameof(IsCustomMode)); OnPropertyChanged(nameof(IsArasMode)); OnPropertyChanged(nameof(IsSourceCustomMode)); } }
    }
    public string? SelectedSourceLanguage { get => _selectedSourceLanguage; set => SetProperty(ref _selectedSourceLanguage, value); }
    public string CustomPrompt { get => _customPrompt; set => SetProperty(ref _customPrompt, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsTranslating
    {
        get => _isTranslating;
        set
        {
            if (SetProperty(ref _isTranslating, value))
            {
                ((RelayCommand)TranslateCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsProgressIndeterminate));
            }
        }
    }
    public ObservableCollection<string> TemplateTypes { get; }
    public ObservableCollection<string> SourceLanguages { get; }
    public ObservableCollection<TextTranslationRecord> HistoryRecords { get; }
    public bool IsArasMode => SelectedTemplateType == "Aras翻译";
    public bool IsCustomMode => SelectedTemplateType == "自定义翻译";
    public bool IsSourceCustomMode => SelectedTemplateType == "源文本自定义翻译";

    // ===== 「源文本自定义翻译」模式专属属性 =====
    public ObservableCollection<string> SheetNames
    {
        get => _sheetNames ??= new ObservableCollection<string>();
        set
        {
            if (SetProperty(ref _sheetNames, value))
            {
                _selectedSheetName = null;
                OnPropertyChanged(nameof(SelectedSheetName));
            }
        }
    }
    public string? SelectedSheetName
    {
        get => _selectedSheetName;
        set { if (SetProperty(ref _selectedSheetName, value) && !string.IsNullOrEmpty(value)) _ = LoadColumnsAsync(value); }
    }
    public ObservableCollection<ExcelColumnInfo> SourceColumns
    {
        get => _sourceColumns ??= new ObservableCollection<ExcelColumnInfo>();
        set => SetProperty(ref _sourceColumns, value);
    }
    public ExcelColumnInfo? SelectedSourceColumn
    {
        get => _selectedSourceColumn;
        set { if (SetProperty(ref _selectedSourceColumn, value) && value != null) _ = LoadSourcePreviewAsync(); }
    }
    public ObservableCollection<ExcelColumnInfo> TargetColumns
    {
        get => _targetColumns ??= new ObservableCollection<ExcelColumnInfo>();
        set => SetProperty(ref _targetColumns, value);
    }
    public ExcelColumnInfo? SelectedTargetColumn
    {
        get => _selectedTargetColumn;
        set => SetProperty(ref _selectedTargetColumn, value);
    }
    public ObservableCollection<string> TargetLanguages
    {
        get => _targetLanguages ??= new ObservableCollection<string>(SourceLanguages);
        set => SetProperty(ref _targetLanguages, value);
    }
    public string? SelectedTargetLanguage
    {
        get => _selectedTargetLanguage ??= "中文";
        set => SetProperty(ref _selectedTargetLanguage, value);
    }
    public string SourceTextPreview
    {
        get => _sourceTextPreview;
        set { SetProperty(ref _sourceTextPreview, value); OnPropertyChanged(nameof(HasSourceTextPreview)); }
    }
    public bool HasSourceTextPreview => !string.IsNullOrWhiteSpace(SourceTextPreview);

    public ICommand LoadColumnsCommand { get; }
    public ICommand CopySourceTextCommand { get; }

    // ===== 分页属性 =====
    public int CurrentPage { get => _currentPage; set { if (SetProperty(ref _currentPage, value)) RefreshPagingCommands(); } }
    public int TotalPages => _totalCount == 0 ? 1 : (int)Math.Ceiling((double)_totalCount / _pageSize);
    public int TotalCount { get => _totalCount; set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(TotalPages)); } }
    public string PageInfo => $"第 {CurrentPage}/{TotalPages} 页，共 {TotalCount} 条";

    // ===== 进度属性 =====
    /// <summary>当前翻译进度（null=未开始/已完成）</summary>
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
    /// <summary>进度百分比 0-100</summary>
    public double ProgressPercentage => Progress?.Percentage ?? 0;
    /// <summary>进度状态文本</summary>
    public string ProgressText => Progress?.StatusText ?? "";
    /// <summary>是否显示不确定进度条</summary>
    public bool IsProgressIndeterminate => Progress == null && IsTranslating;

    public ICommand BrowseSourceCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand OpenOutputFileCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand DownloadTemplateCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private void RefreshPagingCommands()
    {
        (PrevPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
    }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > TotalPages) return;
        CurrentPage = page;
        await LoadHistoryAsync();
    }

    private void BrowseSourceFile()
    {
        var dialog = new OpenFileDialog { Title = "选择翻译源文件", Filter = "Excel 文件|*.xlsx;*.xls", CheckFileExists = true };
        if (dialog.ShowDialog() == true)
        {
            SourceFilePath = dialog.FileName;
            if (IsSourceCustomMode)
                _ = LoadSheetsAsync();
        }
    }

    private async Task LoadSheetsAsync()
    {
        try
        {
            var names = await _translationService.GetSheetNamesAsync(SourceFilePath);
            SheetNames = new ObservableCollection<string>(names);
            SelectedSheetName = names.FirstOrDefault();
            StatusMessage = $"已加载 {names.Count} 个 Sheet";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载 Sheet 失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("文本翻译-加载Sheet", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task LoadColumnsAsync(string? sheetName)
    {
        if (string.IsNullOrWhiteSpace(sheetName)) return;
        try
        {
            var cols = await _translationService.GetSheetColumnsAsync(SourceFilePath, sheetName);
            var sorted = new ObservableCollection<ExcelColumnInfo>(cols);
            SourceColumns = sorted;
            TargetColumns = sorted; // 同一份列列表，供两个下拉共用
            StatusMessage = $"已加载 {sheetName} 的 {cols.Count} 个列";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载列信息失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("文本翻译-加载列", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private async Task LoadSourcePreviewAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedSheetName) || SelectedSourceColumn == null) return;
        try
        {
            var texts = await _translationService.ReadColumnTextAsync(SourceFilePath, SelectedSheetName, SelectedSourceColumn.Index);
            SourceTextPreview = string.Join(Environment.NewLine, texts);
            StatusMessage = $"已加载 {SelectedSourceColumn.Label} 列，共 {texts.Count} 条文本";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载源文本预览失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("文本翻译-源文本预览", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private void CopySourceText()
    {
        if (string.IsNullOrWhiteSpace(SourceTextPreview)) return;
        try
        {
            Clipboard.SetText(SourceTextPreview);
            StatusMessage = "已复制源文本到剪贴板";
        }
        catch (Exception ex)
        {
            StatusMessage = $"复制失败: {ex.Message}";
        }
    }

    private void DownloadTemplate()
    {
        var dialog = new SaveFileDialog
        {
            Title = "保存翻译模板", Filter = "Excel 文件|*.xlsx", DefaultExt = ".xlsx",
            FileName = SelectedTemplateType == "Aras翻译" ? "Aras翻译模板.xlsx" : "自定义翻译模板.xlsx"
        };
        if (dialog.ShowDialog() != true) return;

        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("翻译模板");
            if (SelectedTemplateType == "Aras翻译")
            {
                ws.Cells[1, 1].Value = "属性名称"; ws.Cells[1, 2].Value = "ID";
                ws.Cells[1, 3].Value = "简体中文"; ws.Cells[1, 4].Value = "繁体中文"; ws.Cells[1, 5].Value = "英文";
                ws.Cells[1, 1, 1, 5].Style.Font.Bold = true;
            }
            else
            {
                ws.Cells[1, 1].Value = "属性名称"; ws.Cells[1, 2].Value = "ID";
                ws.Cells[1, 3].Value = "原标签"; ws.Cells[1, 4].Value = "翻译后标签";
                ws.Cells[1, 1, 1, 4].Style.Font.Bold = true;
            }
            ws.Cells[1, 1, 1, 5].AutoFitColumns(8, 30);
            package.SaveAs(new FileInfo(dialog.FileName));
            StatusMessage = $"模板已保存: {dialog.FileName}";
        }
        catch (Exception ex) { StatusMessage = $"模板保存失败: {ex.Message}"; }
    }

    private async Task TranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(SourceFilePath)) { StatusMessage = "请先选择源文件"; return; }
        if (string.IsNullOrWhiteSpace(SelectedTemplateType)) { StatusMessage = "请选择翻译模式"; return; }

        // 「源文本自定义翻译」模式专属校验
        if (IsSourceCustomMode)
        {
            if (string.IsNullOrWhiteSpace(SelectedSheetName)) { StatusMessage = "请选择 Sheet"; return; }
            if (SelectedSourceColumn == null) { StatusMessage = "请选择数据列（源文本列）"; return; }
            if (SelectedTargetColumn == null) { StatusMessage = "请选择翻译列（译文写入列）"; return; }
        }
        else if (SelectedTemplateType == "自定义翻译" && string.IsNullOrWhiteSpace(SelectedSourceLanguage))
        {
            StatusMessage = "请选择源语言"; return;
        }

        IsTranslating = true;
        Progress = null;
        StatusMessage = "正在准备翻译...";

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            var progress = new Progress<TranslationProgressInfo>(p =>
            {
                Progress = p;
                StatusMessage = p.StatusText;
            });

            TextTranslationRecord record;

            if (IsSourceCustomMode)
            {
                record = await _translationService.TranslateSourceCustomAsync(
                    SourceFilePath,
                    SelectedSheetName!,
                    SelectedSourceColumn!.Index,
                    SelectedTargetColumn!.Index,
                    SelectedTargetLanguage ?? "中文",
                    progress,
                    token);
            }
            else
            {
                record = await _translationService.TranslateAsync(
                    SourceFilePath, SelectedTemplateType, SelectedSourceLanguage,
                    CustomPrompt, progress, token);
            }

            StatusMessage = $"翻译完成！共 {record.SourceRowCount} 条，分 {record.BatchCount} 批";
            CurrentPage = 1;
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "翻译已取消";
            await _errorLogService.LogErrorAsync("文本翻译-取消", "用户取消翻译",
                ErrorLog.LevelP1, null);
        }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("文本翻译-翻译", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsTranslating = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelTranslate()
    {
        try
        {
            _cts?.Cancel();
            StatusMessage = "正在取消翻译...";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TextTranslation] 取消异常: {ex.Message}");
        }
    }

    private void OpenOutputFile()
    {
        var first = HistoryRecords.FirstOrDefault();
        if (first == null) return;
        try { if (File.Exists(first.OutputFilePath)) Process.Start(new ProcessStartInfo { FileName = first.OutputFilePath, UseShellExecute = true }); }
        catch { }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var (items, totalCount) = await _translationService.GetHistoryAsync(CurrentUserContext.CurrentUserId, CurrentPage, _pageSize);
            HistoryRecords.Clear();
            foreach (var item in items) HistoryRecords.Add(item);
            TotalCount = totalCount;
            RefreshPagingCommands();
        }
        catch (Exception ex) { StatusMessage = $"加载历史失败: {ex.Message}"; }
    }
}
