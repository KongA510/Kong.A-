using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 图表数据服务接口
/// </summary>
public interface IChartService
{
    /// <summary>
    /// 获取待办任务按状态分组的统计数据
    /// </summary>
    Task<ChartData> GetTaskStatusChartDataAsync();

    /// <summary>
    /// 获取待办任务按项目分组的统计数据
    /// </summary>
    Task<ChartData> GetTaskByProjectChartDataAsync();
}
