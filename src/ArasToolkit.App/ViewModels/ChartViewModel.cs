using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 图表报表 ViewModel — 柱状图数据展示
/// </summary>
public class ChartViewModel : ObservableObject
{
    private readonly IChartService _chartService;
    private readonly IErrorLogService _errorLogService;

    private PlotModel? _plotModel;
    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private string _selectedChartType = "状态分布";
    private ChartData? _currentChartData;

    public ChartViewModel(IChartService chartService, IErrorLogService errorLogService)
    {
        try
        {
            _chartService = chartService;
            _errorLogService = errorLogService;

            ChartTypeOptions = new ObservableCollection<string> { "状态分布", "项目分布" };

            RefreshCommand = new RelayCommand(async _ => await LoadChartAsync());
            SwitchChartCommand = new RelayCommand(async _ => await LoadChartAsync());

            // 初始加载
            _ = LoadChartAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ChartViewModel] 初始化失败: {ex.Message}");
            throw;
        }
    }

    // ===== 属性 =====
    public PlotModel? PlotModel
    {
        get => _plotModel;
        set => SetProperty(ref _plotModel, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

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
            PlotModel = BuildBarChart(data);
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

    /// <summary>
    /// 将 ChartData 转换为 OxyPlot PlotModel（柱状图）
    /// </summary>
    private static PlotModel BuildBarChart(ChartData data)
    {
        var model = new PlotModel
        {
            Title = data.Title,
            TitleFontSize = 16,
            TitleFontWeight = FontWeights.Bold,
            TextColor = OxyColor.FromRgb(0x11, 0x18, 0x27),
            PlotAreaBorderColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
            PlotAreaBorderThickness = new OxyThickness(1),
        };

        // X 轴（分类轴）— 底部
        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Title = data.XAxisTitle,
            TitleFontSize = 13,
            FontSize = 11,
            TextColor = OxyColor.FromRgb(0x37, 0x41, 0x51),
            AxislineColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
            TicklineColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
        };

        foreach (var dp in data.DataPoints)
            categoryAxis.Labels.Add(dp.Label);
        model.Axes.Add(categoryAxis);

        // Y 轴（数值轴）— 左侧
        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = data.YAxisTitle,
            TitleFontSize = 13,
            FontSize = 11,
            TextColor = OxyColor.FromRgb(0x37, 0x41, 0x51),
            AxislineColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
            TicklineColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
            Minimum = 0,
            MinimumPadding = 0,
            AbsoluteMinimum = 0,
            MajorGridlineStyle = LineStyle.Dash,
            MajorGridlineColor = OxyColor.FromRgb(0xE5, 0xE7, 0xEB),
            MinorGridlineStyle = LineStyle.None,
        };
        model.Axes.Add(valueAxis);

        // 柱状图系列 — LinearBarSeries（垂直柱状图：X=分类, Y=数值）
        // LinearBarSeries 使用 DataPoint X/Y 坐标，通过 ItemsSource + Mapping 添加数据
        var barSeries = new LinearBarSeries
        {
            Title = data.Title,
            FillColor = OxyColor.FromRgb(0x63, 0x66, 0xF1),
            StrokeColor = OxyColor.FromRgb(0x4F, 0x46, 0xE5),
            StrokeThickness = 1,
            BarWidth = 3,
        };

        var items = new List<BarItem>();
        foreach (var dp in data.DataPoints)
        {
            items.Add(new BarItem { Value = dp.Value });
        }
        barSeries.ItemsSource = items;
        barSeries.Mapping = item => new DataPoint(items.IndexOf((BarItem)item), ((BarItem)item).Value);

        model.Series.Add(barSeries);
        return model;
    }
}
