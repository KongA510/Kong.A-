namespace ArasToolkit.Core.Models;

/// <summary>
/// 翻译进度信息 — 用于 Progress&lt;T&gt; 报告翻译实时状态
/// </summary>
public class TranslationProgressInfo
{
    /// <summary>当前阶段: "读取源文件" / "翻译中" / "生成输出文件"</summary>
    public string Phase { get; set; } = string.Empty;

    /// <summary>当前批次编号（从1开始）</summary>
    public int Current { get; set; }

    /// <summary>阶段总批次数</summary>
    public int PhaseTotal { get; set; }

    /// <summary>全局已处理行数</summary>
    public int OverallCurrent { get; set; }

    /// <summary>全局总行数</summary>
    public int OverallTotal { get; set; }

    /// <summary>当前项目名称（批次标签）</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>进度百分比（0-100，1位小数）</summary>
    public double Percentage => OverallTotal > 0
        ? Math.Round((double)OverallCurrent / OverallTotal * 100, 1)
        : 0;

    /// <summary>状态文本描述</summary>
    public string StatusText => PhaseTotal > 0
        ? $"{Phase}: {Current}/{PhaseTotal} — {ItemName}"
        : $"{Phase} — {ItemName}";
}
