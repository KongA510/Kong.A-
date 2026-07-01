namespace ArasToolkit.Core.Models;

/// <summary>
/// 通用图表数据模型
/// </summary>
public class ChartData
{
    /// <summary>图表标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>X 轴标题</summary>
    public string XAxisTitle { get; set; } = string.Empty;

    /// <summary>Y 轴标题</summary>
    public string YAxisTitle { get; set; } = string.Empty;

    /// <summary>数据点列表</summary>
    public List<ChartDataPoint> DataPoints { get; set; } = new();
}

/// <summary>
/// 单个数据点（标签 + 数值）
/// </summary>
public class ChartDataPoint
{
    /// <summary>分类标签（X 轴）</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>数值（Y 轴）</summary>
    public double Value { get; set; }

    /// <summary>可选：柱状图颜色（十六进制，如 #6366F1）</summary>
    public string? Color { get; set; }
}
