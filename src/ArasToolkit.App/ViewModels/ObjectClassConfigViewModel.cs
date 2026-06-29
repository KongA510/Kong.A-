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

/// <summary>
/// 对象类配置 ViewModel — Excel模板下载 + 批量导入Aras
/// </summary>
public class ObjectClassConfigViewModel : ObservableObject
{
    private readonly IObjectClassImportService _importService;
    private readonly IErrorLogService _errorLogService;

    private string _selectedFilePath = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isImporting;
    private bool _isLoading;
    private ObjectClassImportResult? _lastResult;

    public ObjectClassConfigViewModel(
        IObjectClassImportService importService,
        IErrorLogService errorLogService)
    {
        _importService = importService;
        _errorLogService = errorLogService;

        HistoryRecords = new ObservableCollection<ObjectClassImportLog>();

        DownloadTemplateCommand = new RelayCommand(_ => DownloadTemplate());
        BrowseFileCommand = new RelayCommand(_ => BrowseFile());
        ExecuteImportCommand = new RelayCommand(async _ => await ExecuteImportAsync(), _ => !IsImporting);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());

        _ = LoadHistoryAsync();
    }

    // ===== 文件 =====
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            if (SetProperty(ref _selectedFilePath, value))
            {
                StatusMessage = string.IsNullOrEmpty(value)
                    ? ""
                    : $"已选择: {Path.GetFileName(value)}";
            }
        }
    }

    public string FileName => string.IsNullOrEmpty(SelectedFilePath)
        ? "(未选择文件)"
        : Path.GetFileName(SelectedFilePath);

    // ===== 状态 =====
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsImporting
    {
        get => _isImporting;
        set
        {
            if (SetProperty(ref _isImporting, value))
                (ExecuteImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // ===== 结果 =====
    public ObjectClassImportResult? LastResult
    {
        get => _lastResult;
        set
        {
            if (SetProperty(ref _lastResult, value))
                OnPropertyChanged(nameof(HasResult));
        }
    }

    public bool HasResult => LastResult != null;

    // ===== 历史 =====
    public ObservableCollection<ObjectClassImportLog> HistoryRecords { get; }

    // ===== 命令 =====
    public ICommand DownloadTemplateCommand { get; }
    public ICommand BrowseFileCommand { get; }
    public ICommand ExecuteImportCommand { get; }
    public ICommand RefreshHistoryCommand { get; }

    // ===== 方法 =====

    /// <summary>
    /// 下载模板 — 保存到本地 Config/ObjectClassTemplates 文件夹
    /// </summary>
    private void DownloadTemplate()
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Title = "保存对象类配置模板",
                Filter = "Excel 文件|*.xlsx",
                DefaultExt = ".xlsx",
                FileName = "对象类配置模板.xlsx"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = "正在生成模板...";

            var templateBytes = _importService.GenerateTemplate();
            File.WriteAllBytes(dialog.FileName, templateBytes);

            StatusMessage = $"模板已保存: {dialog.FileName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"模板保存失败: {ex.Message}";
            _ = _errorLogService.LogErrorAsync("对象类配置-下载模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 浏览并选择 Excel 文件
    /// </summary>
    private void BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择对象类配置汇入文件",
            Filter = "Excel 文件|*.xlsx;*.xls",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedFilePath = dialog.FileName;
        }
    }

    /// <summary>
    /// 执行导入 — 单线程串行，先建对象类再建关系类
    /// </summary>
    private async Task ExecuteImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFilePath))
        {
            ErrorMessage = "请先选择导入文件";
            return;
        }

        IsImporting = true;
        ErrorMessage = string.Empty;
        StatusMessage = "正在导入...";

        try
        {
            var progress = new Progress<string>(msg => StatusMessage = msg);
            LastResult = await _importService.ImportAsync(SelectedFilePath, progress);

            if (LastResult.IsSuccess)
            {
                StatusMessage = $"导入完成: 对象类 {LastResult.Sheet1Count} 条 / 关系类 {LastResult.Sheet2Count} 条";
            }
            else
            {
                ErrorMessage = $"导入失败: {LastResult.ErrorMessage}";
            }

            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导入失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("对象类配置-导入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsImporting = false;
        }
    }

    /// <summary>
    /// 加载历史记录
    /// </summary>
    private async Task LoadHistoryAsync()
    {
        try
        {
            var list = await _importService.GetHistoryAsync(CurrentUserContext.CurrentUserId);
            HistoryRecords.Clear();
            foreach (var item in list)
                HistoryRecords.Add(item);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载历史失败: {ex.Message}";
        }
    }
}
