---
id: TASK-004
priority: P0
type: fix
created: 2026-06-22
source: Claude Code
status: done
---

# 修复柱状图无法渲染 — BarSeries → ColumnSeries

## 问题描述

界面不闪退后，图表区域无法显示柱状图，OxyPlot 抛出异常：

```
BarSeries requires a CategoryAxis on the Y Axis.
at OxyPlot.Series.BarSeriesBase`1.GetCategoryAxis()
at OxyPlot.PlotModel.UpdateBarSeriesManagers()
```

## 根因分析

OxyPlot 中 `BarSeries` 默认绘制**水平条形图**（horizontal bars），其设计要求 `CategoryAxis` 必须在 **Y 轴**（`AxisPosition.Left` 或 `AxisPosition.Right`）。

当前 `ChartViewModel.BuildBarChart()` 代码将 `CategoryAxis` 放在了 `AxisPosition.Bottom`（X 轴），与 `BarSeries` 的要求冲突，导致 OxyPlot 渲染管线在 `UpdateBarSeriesManagers()` 阶段抛出异常。

**修复方案**：将 `BarSeries` 替换为 `ColumnSeries`，`BarItem` 替换为 `ColumnItem`。`ColumnSeries` 绘制**垂直柱状图**（vertical columns），分类轴在 X 轴（Bottom），数值轴在 Y 轴（Left），符合用户期望的传统柱状图样式。

## 涉及文件

- `src/ArasToolkit.App/ViewModels/ChartViewModel.cs` — `BuildBarChart()` 方法

## 具体修改

### 1. 类型替换（第161-171行）

```csharp
// 修改前
var barSeries = new BarSeries
{
    Title = data.Title,
    FillColor = OxyColor.FromRgb(0x63, 0x66, 0xF1),
    StrokeColor = OxyColor.FromRgb(0x4F, 0x46, 0xE5),
    StrokeThickness = 1,
    LabelPlacement = LabelPlacement.Outside,
    LabelFormatString = "{0}",
    FontSize = 12,
    FontWeight = FontWeights.Bold,
};

// 修改后
var columnSeries = new ColumnSeries
{
    Title = data.Title,
    FillColor = OxyColor.FromRgb(0x63, 0x66, 0xF1),
    StrokeColor = OxyColor.FromRgb(0x4F, 0x46, 0xE5),
    StrokeThickness = 1,
    LabelPlacement = LabelPlacement.Outside,
    LabelFormatString = "{0}",
    FontSize = 12,
    FontWeight = FontWeights.Bold,
};
```

### 2. 数据项类型替换（第173-197行）

```csharp
// 修改前
var item = new BarItem { Value = dp.Value };

// 修改后
var item = new ColumnItem { Value = dp.Value };
```

### 3. 添加到 Series（第200行）

```csharp
// 修改前
model.Series.Add(barSeries);

// 修改后
model.Series.Add(columnSeries);
```

### 4. 变量名统一

将方法内所有 `barSeries` 引用改为 `columnSeries`。

## 关键说明

- `ColumnSeries` 与 `BarSeries` 的 API 完全一致（均继承自 `BarSeriesBase<T>`），属性名 `FillColor`、`StrokeColor`、`LabelPlacement` 等无需修改
- `CategoryAxis` 保持在 `AxisPosition.Bottom`（X 轴 = 分类轴）✓
- `LinearAxis` 保持在 `AxisPosition.Left`（Y 轴 = 数值轴）✓
- 不改动 `Color` 属性设置逻辑（`BarItem`/`ColumnItem` 均有 `Color` 属性）

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 运行后「状态分布」图表正常显示垂直柱状图
- [ ] 切换到「项目分布」正常显示
- [ ] 各柱颜色正确（灰/紫/红/绿）
- [ ] 数值标签显示在柱上方

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]
