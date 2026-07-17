using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 任务负载分析 ViewModel — 支持 AI 流式输出 + 历史记录管理
/// </summary>
public class TaskLoadAnalysisViewModel : ObservableObject
{
    private readonly ITaskLoadAnalysisService _analysisService;
    private readonly IErrorLogService _errorLogService;

    private CancellationTokenSource? _cts;

    // ===== 日期范围 =====
    private DateTime _startDate = DateTime.Today.AddDays(-7);
    private DateTime _endDate = DateTime.Today;

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    // ===== 分析状态 =====
    private bool _isAnalyzing;
    public bool IsAnalyzing
    {
        get => _isAnalyzing;
        set
        {
            SetProperty(ref _isAnalyzing, value);
            AnalyzeCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }
    }

    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // ===== 流式输出内容 =====
    private string _analysisContent = "";
    public string AnalysisContent
    {
        get => _analysisContent;
        set => SetProperty(ref _analysisContent, value);
    }

    // ===== 历史记录 =====
    public ObservableCollection<TaskLoadAnalysisRecord> HistoryRecords { get; } = new();

    private TaskLoadAnalysisRecord? _selectedRecord;
    public TaskLoadAnalysisRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            if (SetProperty(ref _selectedRecord, value) && value != null)
            {
                LoadRecordDetail(value);
            }
        }
    }

    private int _historyPage = 1;
    public int HistoryPage
    {
        get => _historyPage;
        set => SetProperty(ref _historyPage, value);
    }

    private int _historyTotalPages = 1;
    public int HistoryTotalPages
    {
        get => _historyTotalPages;
        set => SetProperty(ref _historyTotalPages, value);
    }

    // ===== 命令 =====
    public RelayCommand AnalyzeCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand RefreshHistoryCommand { get; }
    public RelayCommand DeleteRecordCommand { get; }
    public RelayCommand PrevHistoryPageCommand { get; }
    public RelayCommand NextHistoryPageCommand { get; }

    public TaskLoadAnalysisViewModel(
        ITaskLoadAnalysisService analysisService,
        IErrorLogService errorLogService)
    {
        _analysisService = analysisService;
        _errorLogService = errorLogService;

        AnalyzeCommand = new RelayCommand(async _ => await StartAnalysisAsync(),
            _ => !IsAnalyzing);
        CancelCommand = new RelayCommand(_ => CancelAnalysis(),
            _ => IsAnalyzing);
        RefreshHistoryCommand = new RelayCommand(async _ => await LoadHistoryAsync());
        DeleteRecordCommand = new RelayCommand(async p =>
        {
            if (p is TaskLoadAnalysisRecord record)
                await DeleteRecordAsync(record);
        });
        PrevHistoryPageCommand = new RelayCommand(async _ =>
        {
            if (HistoryPage > 1) { HistoryPage--; await LoadHistoryAsync(); }
        }, _ => HistoryPage > 1);
        NextHistoryPageCommand = new RelayCommand(async _ =>
        {
            if (HistoryPage < HistoryTotalPages) { HistoryPage++; await LoadHistoryAsync(); }
        }, _ => HistoryPage < HistoryTotalPages);

        _ = LoadHistoryAsync();
    }

    // ===== 分析方法 =====

    private async System.Threading.Tasks.Task StartAnalysisAsync()
    {
        if (StartDate > EndDate)
        {
            StatusMessage = "开始日期不能晚于结束日期";
            return;
        }

        IsAnalyzing = true;
        AnalysisContent = "";
        StatusMessage = "正在分析中...";
        _cts = new CancellationTokenSource();

        try
        {
            await _analysisService.AnalyzeStreamAsync(
                StartDate,
                EndDate,
                chunk =>
                {
                    // 在 UI 线程追加内容
                    System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
                    {
                        AnalysisContent += chunk;
                    });
                },
                _cts.Token);

            StatusMessage = "分析完成";
            await LoadHistoryAsync();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "分析已取消";
        }
        catch (Exception ex)
        {
            StatusMessage = $"分析失败: {ex.Message}";
            await _errorLogService.LogErrorAsync(
                "任务负载分析-执行分析",
                ex.Message,
                ErrorLog.LevelP1,
                ex.StackTrace);
        }
        finally
        {
            IsAnalyzing = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelAnalysis()
    {
        _cts?.Cancel();
    }

    private void LoadRecordDetail(TaskLoadAnalysisRecord record)
    {
        StartDate = record.StartDate;
        EndDate = record.EndDate;
        AnalysisContent = record.AnalysisResult;
        StatusMessage = $"已加载历史记录: {record.DisplayDateRange}";
    }

    // ===== 历史记录 =====

    private async System.Threading.Tasks.Task LoadHistoryAsync()
    {
        try
        {
            const int pageSize = 10;
            var (items, total) = await _analysisService.GetPagedRecordsAsync(HistoryPage, pageSize);

            HistoryRecords.Clear();
            foreach (var item in items)
                HistoryRecords.Add(item);

            HistoryTotalPages = Math.Max(1, (int)Math.Ceiling((double)total / pageSize));
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载历史记录失败: {ex.Message}";
            await _errorLogService.LogErrorAsync(
                "任务负载分析-加载历史",
                ex.Message,
                ErrorLog.LevelP1,
                ex.StackTrace);
        }
    }

    private async System.Threading.Tasks.Task DeleteRecordAsync(TaskLoadAnalysisRecord record)
    {
        try
        {
            await _analysisService.DeleteRecordAsync(record.Id);
            StatusMessage = "记录已删除";
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"删除失败: {ex.Message}";
            await _errorLogService.LogErrorAsync(
                "任务负载分析-删除记录",
                ex.Message,
                ErrorLog.LevelP1,
                ex.StackTrace);
        }
    }
}
