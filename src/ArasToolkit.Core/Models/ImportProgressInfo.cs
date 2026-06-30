namespace ArasToolkit.Core.Models;

/// <summary>
/// 导入进度信息 — 用于实时报告导入进度到 UI
/// </summary>
public class ImportProgressInfo
{
    /// <summary>
    /// 当前阶段描述（如 "对象类覆盖"、"关系类新增"）
    /// </summary>
    public string Phase { get; set; } = string.Empty;

    /// <summary>
    /// 当前阶段已处理条目数（1-based）
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// 当前阶段总条目数
    /// </summary>
    public int PhaseTotal { get; set; }

    /// <summary>
    /// 全局已处理条目数（Sheet1 + Sheet2 累计）
    /// </summary>
    public int OverallCurrent { get; set; }

    /// <summary>
    /// 全局总条目数（Sheet1 + Sheet2 合计）
    /// </summary>
    public int OverallTotal { get; set; }

    /// <summary>
    /// 当前正在处理的条目名称
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// 全局进度百分比（0-100，精确到 1 位小数）
    /// </summary>
    public double Percentage =>
        OverallTotal > 0
            ? Math.Round((double)OverallCurrent / OverallTotal * 100, 1)
            : 0;

    /// <summary>
    /// 友好状态文本，如 "对象类覆盖: 5/20 — Part_BOM"
    /// </summary>
    public string StatusText => $"{Phase}: {Current}/{PhaseTotal} — {ItemName}";

    /// <summary>
    /// 阶段进度百分比（0-100）
    /// </summary>
    public double PhasePercentage =>
        PhaseTotal > 0
            ? Math.Round((double)Current / PhaseTotal * 100, 1)
            : 0;
}
