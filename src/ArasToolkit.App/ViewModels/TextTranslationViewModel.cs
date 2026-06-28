using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
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

    public TextTranslationViewModel(
        ITextTranslationService translationService,
        IErrorLogService errorLogService)
    {
        _translationService = translationService;
        _errorLogService = errorLogService;

        TemplateTypes = new ObservableCollection<string> { "Aras翻译", "自定义翻译" };
        SelectedTemplateType = TemplateTypes[0];
        SourceLanguages = new ObservableCollection<string>
        {
            "中文", "English", "Tiếng Việt", "日本語", "한국어",
            "ไทย", "Bahasa Indonesia", "Français", "Deutsch", "Español"
        };
        SelectedSourceLanguage = SourceLanguages[0];
        HistoryRecords = new ObservableCollection<TextTranslationRecord>();

        BrowseSourceCommand = new RelayCommand(_ => BrowseSourceFile());
        TranslateCommand = new RelayCommand(async _ => await TranslateAsync(), _ => !IsTranslating);
        OpenOutputFileCommand = new RelayCommand(_ => OpenOutputFile());
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());

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
        set { if (SetProperty(ref _selectedTemplateType, value)) { OnPropertyChanged(nameof(IsCustomMode)); OnPropertyChanged(nameof(IsArasMode)); } }
    }
    public string? SelectedSourceLanguage { get => _selectedSourceLanguage; set => SetProperty(ref _selectedSourceLanguage, value); }
    public string CustomPrompt { get => _customPrompt; set => SetProperty(ref _customPrompt, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsTranslating { get => _isTranslating; set { if (SetProperty(ref _isTranslating, value)) ((RelayCommand)TranslateCommand).RaiseCanExecuteChanged(); } }
    public ObservableCollection<string> TemplateTypes { get; }
    public ObservableCollection<string> SourceLanguages { get; }
    public ObservableCollection<TextTranslationRecord> HistoryRecords { get; }
    public bool IsArasMode => SelectedTemplateType == "Aras翻译";
    public bool IsCustomMode => SelectedTemplateType == "自定义翻译";

    public ICommand BrowseSourceCommand { get; }
    public ICommand TranslateCommand { get; }
    public ICommand OpenOutputFileCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand DownloadTemplateCommand { get; }

    private void BrowseSourceFile()
    {
        var dialog = new OpenFileDialog { Title = "选择翻译源文件", Filter = "Excel 文件|*.xlsx;*.xls", CheckFileExists = true };
        if (dialog.ShowDialog() == true) SourceFilePath = dialog.FileName;
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
        if (SelectedTemplateType == "自定义翻译" && string.IsNullOrWhiteSpace(SelectedSourceLanguage)) { StatusMessage = "请选择源语言"; return; }

        IsTranslating = true;
        StatusMessage = "正在翻译...";
        try
        {
            var progress = new Progress<string>(msg => StatusMessage = msg);
            var record = await _translationService.TranslateAsync(SourceFilePath, SelectedTemplateType, SelectedSourceLanguage, CustomPrompt, progress);
            StatusMessage = $"翻译完成！共 {record.SourceRowCount} 条，分 {record.BatchCount} 批";
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"翻译失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("文本翻译-翻译", ex.Message, ArasToolkit.Core.Entities.ErrorLog.LevelP1, ex.StackTrace);
        }
        finally { IsTranslating = false; }
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
            var list = await _translationService.GetHistoryAsync(CurrentUserContext.CurrentUserId);
            HistoryRecords.Clear();
            foreach (var item in list) HistoryRecords.Add(item);
        }
        catch (Exception ex) { StatusMessage = $"加载历史失败: {ex.Message}"; }
    }
}
