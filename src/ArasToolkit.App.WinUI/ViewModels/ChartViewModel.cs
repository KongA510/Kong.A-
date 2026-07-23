using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 图表报表 ViewModel（WinUI 3 / LiveCharts2）— 柱状图数据展示。
/// </summary>
public class ChartViewModel : ObservableObject
{
    private readonly IChartService _chartService;
    private readonly IErrorLogService _errorLogService;

    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private string _selectedChartType = "状态分布";
    private ChartData? _currentChartData;

    public ChartViewModel(IChartService chartService, IErrorLogService errorLogService)
    {
        _chartService = chartService;
        _errorLogService = errorLogService;

        ChartTypeOptions = new ObservableCollection<string> { "状态分布", "项目分布" };
        Series = new ObservableCollection<ISeries>();
        XAxes = new ObservableCollection<Axis>();
        YAxes = new ObservableCollection<Axis>();

        RefreshCommand = new RelayCommand(async _ => await LoadChartAsync());
        SwitchChartCommand = new RelayCommand(async _ => await LoadChartAsync());

        _ = LoadChartAsync();
    }

    // ===== LiveCharts2 绑定属性 =====
    public ObservableCollection<ISeries> Series { get; }
    public ObservableCollection<Axis> XAxes { get; }
    public ObservableCollection<Axis> YAxes { get; }

    // ===== 属性 =====
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public string SelectedChartType
    {
        get => _selectedChartType;
        set
        {
            if (SetProperty(ref _selectedChartType, value))
                _ = LoadChartAsync();
        }
    }

    public ObservableCollection<string> ChartTypeOptions { get; }

    public ChartData? CurrentChartData
    {
        get => _currentChartData;
        set => SetProperty(ref _currentChartData, value);
    }

    // ===== 命令 =====
    public ICommand RefreshCommand { get; }
    public ICommand SwitchChartCommand { get; }

    // ===== 方法 =====
    private async Task LoadChartAsync()
    {
        IsLoading = true;
        try
        {
            ChartData data = SelectedChartType switch
            {
                "项目分布" => await _chartService.GetTaskByProjectChartDataAsync(),
                _ => await _chartService.GetTaskStatusChartDataAsync()
            };

            CurrentChartData = data;
            BuildBarChart(data);
            StatusMessage = $"✓ {data.Title} — 共 {data.DataPoints.Sum(d => d.Value)} 条记录";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ 加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("Chart-加载图表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>将 ChartData 转换为 LiveCharts2 柱状图（Series + XAxes + YAxes）。</summary>
    private void BuildBarChart(ChartData data)
    {
        var values = data.DataPoints.Select(d => d.Value).ToArray();
        var labels = data.DataPoints.Select(d => d.Label).ToList();

        Series.Clear();
        Series.Add(new ColumnSeries<double>
        {
            Values = values,
            Name = data.Title,
            Fill = new SolidColorPaint(new SKColor(0x63, 0x66, 0xF1)),
            MaxBarWidth = 60,
        });

        XAxes.Clear();
        XAxes.Add(new Axis
        {
            Name = data.XAxisTitle,
            Labels = labels,
            LabelsRotation = 15,
        });

        YAxes.Clear();
        YAxes.Add(new Axis
        {
            Name = data.YAxisTitle,
            MinLimit = 0,
        });
    }
}
