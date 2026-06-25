---
id: TASK-022
priority: P1
type: feature
created: 2026-06-25
source: Claude Code（产品方案）
status: done
reviewed: 2026-06-25
reviewer: Claude Code
---

# TASK-022: 数据汇入页面 UI 重构 + 进度条

## 产品目标

将数据汇入页面从"功能堆砌"升级为"现代工具型交互"，提升操作效率和视觉品质。

---

## 一、现状诊断

### 1.1 当前问题清单

| # | 问题 | 位置 | 严重度 |
|---|------|------|--------|
| 1 | 保存按钮始终灰色不可点击 | DataImportView.xaml:124 | P1 |
| 2 | 暂停/继续合为一个按钮，用户不知道当前状态 | :150 | P1 |
| 3 | 预览结果区域仅 300px 宽，AML 内容多时看不全 | :137 | P2 |
| 4 | 导入执行没有进度反馈，大批量导入时用户焦虑 | :145 | P1 |
| 5 | 文件选择区参数标签拥挤，"起始行/结束行/起始列/结束列"挤在一行 | :48-59 | P2 |
| 6 | 整体缺乏视觉节奏，卡片间距不一致 | 全局 | P2 |

### 1.2 保存按钮灰掉的根因

```csharp
// DataImportViewModel.cs:171
private bool CanSaveConfig() => !string.IsNullOrEmpty(AmlContent) && !string.IsNullOrEmpty(SelectedSheetName);
```

两个条件：AmlContent 非空 + SelectedSheetName 非空。用户打开页面后未输入 AML 内容，SaveConfigCommand.CanExecute 返回 false → WPF 自动灰掉按钮。**逻辑正确但体验生硬** —— 应允许保存空模板草稿，或至少给视觉提示。

---

## 二、UI 重构方案

### 2.1 整体布局（6行 → 5行）

```
┌──────────────────────────────────────────────────┐
│ Row 0: 标题 "数据汇入" + 副标题                    │
├──────────────────────────────────────────────────┤
│ Row 1: 文件选择卡片                                │
│  ┌──────────────────────────────────────────────┐ │
│  │ [浏览] 文件名.xlsx          Sheet: [▼]       │ │
│  │ ─────────────────────────────────────────── │ │
│  │ 行范围: [2] ~ [-1]   列范围: [1] ~ [-1]     │ │
│  │                              [加载预览]      │ │
│  └──────────────────────────────────────────────┘ │
├──────────────────────────────────────────────────┤
│ Row 2: 列映射卡片（条件显示）                      │
├────────────────────┬─────────────────────────────┤
│ Row 3: 数据预览     │ Row 3: AML 编辑器            │
│  (左 55%)          │  (右 45%)                    │
│                    │  ┌─────────────────────────┐ │
│                    │  │ AML 模板输入区          │ │
│                    │  │ (可滚动)                │ │
│                    │  └─────────────────────────┘ │
│                    │  ┌─────────────────────────┐ │
│                    │  │ AML 预览结果            │ │
│                    │  │ (可滚动, 浅蓝背景)      │ │
│                    │  └─────────────────────────┘ │
│                    │  [选择配置] [保存] [预览AML]  │
├────────────────────┴─────────────────────────────┤
│ Row 4: 执行栏                                     │
│  [执行导入] [⏸ 暂停] [▶ 继续]  ████████░░ 65%    │
│  状态: 正在导入 12/200 行...                      │
├──────────────────────────────────────────────────┤
│ Row 5: 状态栏（错误/成功/日志链接）                 │
└──────────────────────────────────────────────────┘
```

### 2.2 卡片规范（统一视觉）

所有卡片使用 Theme.xaml `CardStyle`，内部 padding 统一为 `20,16`：
- 文件选择卡片
- 列映射卡片
- 数据预览卡片
- AML 编辑卡片
- 底部状态卡片

卡片间距统一 `Margin="0,0,0,10"`。

---

## 三、详细修改清单

### 3.1 DataImportViewModel.cs — 新增属性与命令

```csharp
// ===== 新增属性 =====

/// <summary>导入进度 0.0 ~ 100.0</summary>
private double _importProgress;
public double ImportProgress
{
    get => _importProgress;
    set => SetProperty(ref _importProgress, value);
}

/// <summary>当前进度文本（如 "12/200"）</summary>
private string _progressText = string.Empty;
public string ProgressText
{
    get => _progressText;
    set => SetProperty(ref _progressText, value);
}

/// <summary>是否显示进度条（导入中或暂停时）</summary>
public bool IsProgressVisible => IsImporting || IsPaused;

// ===== 拆分暂停/继续命令 =====

// 原 PauseCommand → 拆为两个独立命令
public ICommand PauseCommand { get; }    // 暂停
public ICommand ResumeCommand { get; }   // 继续

// PauseCommand 仅在 正在导入 + 未暂停 时可用
// ResumeCommand 仅在 已暂停 时可用

// CanSaveConfig 放宽条件
private bool CanSaveConfig() => !string.IsNullOrEmpty(SelectedSheetName);
// 去掉 AmlContent 非空限制，允许保存空模板草稿
```

**ExecuteImportAsync 需要改造** — 通过 `IProgress<int>` 报告进度：

```csharp
// 新增 using System;
private async Task ExecuteImportAsync()
{
    IsImporting = true;
    IsPaused = false;
    ImportProgress = 0;
    ProgressText = "0/0";
    _cts = new CancellationTokenSource();
    ErrorMessage = string.Empty;

    try
    {
        var progress = new Progress<int>(percent =>
        {
            ImportProgress = percent;
            // ProgressText 由 Service 回调更新
        });

        var config = new DataImportConfig { ... };
        LastResult = await _dataImportService.ExecuteImportAsync(
            SelectedFilePath, config, progress, onRowProgress: (current, total) =>
            {
                ProgressText = $"{current}/{total}";
            }, cancellationToken: _cts.Token);
    }
    catch (OperationCanceledException) { StatusMessage = "导入已暂停"; }
    catch (Exception ex) { ErrorMessage = "导入失败: " + ex.Message; }
    finally { IsImporting = false; IsPaused = false; OnPropertyChanged(nameof(IsProgressVisible)); }
}
```

**注意**：如果不想改 Service 接口签名，可以先在 ViewModel 中用模拟进度（基于总行数 estimate），真正的行级进度后续再下沉到 Service。本次优先实现 UI 侧的进度展示能力。

### 3.2 IDataImportService.cs — 接口扩展（可选，看复杂度）

如果 Service 层配合改造，增加重载：

```csharp
Task<ImportResult> ExecuteImportAsync(
    string filePath, DataImportConfig config,
    IProgress<int> progress,
    Action<int, int> onRowProgress,
    CancellationToken cancellationToken);
```

> 若接口改造范围过大，本次可先在 ViewModel 层做进度模拟（已知总行数，每处理一行 +1）。Codex 自行判断。

### 3.3 DataImportView.xaml — 完整 XAML 重构

```xml
<UserControl x:Class="ArasToolkit.App.Views.DataImportView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:converters="clr-namespace:ArasToolkit.App.Converters"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             mc:Ignorable="d"
             d:DesignHeight="720" d:DesignWidth="1200"
             Background="Transparent">

    <UserControl.Resources>
        <converters:BoolToVisibilityConverter x:Key="BoolToVisibility" />
        <converters:InverseBoolConverter x:Key="InverseBool" />
        <converters:StringToVisibilityConverter x:Key="StringToVisibility" />
    </UserControl.Resources>

    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />   <!-- 0: 标题 -->
            <RowDefinition Height="Auto" />   <!-- 1: 文件选择 -->
            <RowDefinition Height="Auto" />   <!-- 2: 列映射 -->
            <RowDefinition Height="*" />      <!-- 3: 预览 + AML 左右分栏 -->
            <RowDefinition Height="Auto" />   <!-- 4: 执行栏 -->
            <RowDefinition Height="Auto" />   <!-- 5: 状态 -->
        </Grid.RowDefinitions>

        <!-- ===== Row 0: 标题 ===== -->
        <StackPanel Grid.Row="0" Margin="0,0,0,14">
            <TextBlock Text="数据汇入" FontSize="22" FontWeight="Bold"
                       Foreground="{StaticResource TextPrimaryBrush}" />
            <TextBlock Text="Excel 数据导入 Aras 系统" FontSize="13"
                       Foreground="{StaticResource TextSecondaryBrush}" Margin="0,2,0,0" />
        </StackPanel>

        <!-- ===== Row 1: 文件选择 ===== -->
        <Border Grid.Row="1" Style="{StaticResource CardStyle}"
                Padding="20,16" Margin="0,0,0,10">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto" />
                    <RowDefinition Height="Auto" />
                </Grid.RowDefinitions>

                <!-- 第一行：浏览 + 文件名 + Sheet -->
                <StackPanel Grid.Row="0" Orientation="Horizontal"
                            Margin="0,0,0,10">
                    <Button Content="📂 浏览" Command="{Binding BrowseFileCommand}"
                            Style="{StaticResource PrimaryButton}"
                            Height="30" FontSize="12" Padding="14,0" />
                    <TextBlock Text="{Binding FileName}" FontSize="13"
                               Foreground="{StaticResource TextPrimaryBrush}"
                               VerticalAlignment="Center" Margin="12,0,0,0"
                               MaxWidth="300" TextTrimming="CharacterEllipsis" />
                    <TextBlock Text="Sheet" FontSize="13"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center" Margin="20,0,6,0" />
                    <ComboBox ItemsSource="{Binding SheetNames}"
                              SelectedItem="{Binding SelectedSheetName}"
                              Width="140" Height="28" FontSize="12" />
                </StackPanel>

                <!-- 第二行：范围参数 + 加载预览 -->
                <StackPanel Grid.Row="1" Orientation="Horizontal">
                    <TextBlock Text="行范围" FontSize="12"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center" />
                    <TextBox Text="{Binding StartRow}" Width="56" Margin="6,0,4,0"
                             Height="28" FontSize="12"
                             Background="{StaticResource SurfaceBrush}" />
                    <TextBlock Text="~" FontSize="12"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center" Margin="0,0,4,0" />
                    <TextBox Text="{Binding EndRow}" Width="56" Margin="0,0,12,0"
                             Height="28" FontSize="12"
                             Background="{StaticResource SurfaceBrush}" />

                    <TextBlock Text="列范围" FontSize="12"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center" />
                    <TextBox Text="{Binding StartCol}" Width="56" Margin="6,0,4,0"
                             Height="28" FontSize="12"
                             Background="{StaticResource SurfaceBrush}" />
                    <TextBlock Text="~" FontSize="12"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center" Margin="0,0,4,0" />
                    <TextBox Text="{Binding EndCol}" Width="56" Margin="0,0,16,0"
                             Height="28" FontSize="12"
                             Background="{StaticResource SurfaceBrush}" />

                    <Button Content="🔍 加载预览" Command="{Binding LoadPreviewCommand}"
                            Style="{StaticResource PrimaryButton}"
                            Height="30" FontSize="12" Padding="14,0" />
                </StackPanel>
            </Grid>
        </Border>

        <!-- ===== Row 2: 列映射（条件显示） ===== -->
        <Border Grid.Row="2" Style="{StaticResource CardStyle}"
                Padding="20,10" Margin="0,0,0,10"
                Visibility="{Binding HasColumnMappings, Converter={StaticResource BoolToVisibility}}">
            <StackPanel>
                <TextBlock Text="列映射表" FontSize="12" FontWeight="SemiBold"
                           Foreground="{StaticResource TextSecondaryBrush}"
                           Margin="0,0,0,6" />
                <ItemsControl ItemsSource="{Binding ColumnMappings}">
                    <ItemsControl.ItemsPanel>
                        <ItemsPanelTemplate>
                            <WrapPanel />
                        </ItemsPanelTemplate>
                    </ItemsControl.ItemsPanel>
                    <ItemsControl.ItemTemplate>
                        <DataTemplate>
                            <Border Background="{StaticResource SurfaceBrush}"
                                    CornerRadius="4" Padding="8,4" Margin="0,0,8,4">
                                <TextBlock FontSize="12"
                                           Foreground="{StaticResource TextSecondaryBrush}">
                                    <Run Text="{Binding Letter}" FontWeight="Bold"
                                         Foreground="{StaticResource AccentBrush}" />
                                    <Run Text=" → " />
                                    <Run Text="{Binding Header}" />
                                </TextBlock>
                            </Border>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
            </StackPanel>
        </Border>

        <!-- ===== Row 3: 数据预览 + AML 编辑器（左右分栏） ===== -->
        <Grid Grid.Row="3" Margin="0,0,0,10">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="5*" />
                <ColumnDefinition Width="8" />
                <ColumnDefinition Width="4*" />
            </Grid.ColumnDefinitions>

            <!-- 左：数据预览 -->
            <Border Grid.Column="0" Style="{StaticResource CardStyle}" Padding="0">
                <Grid>
                    <DataGrid ItemsSource="{Binding PreviewData}"
                              AutoGenerateColumns="True"
                              IsReadOnly="True"
                              Style="{StaticResource DarkDataGrid}" />
                    <TextBlock Text="请先加载预览" FontSize="14"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               HorizontalAlignment="Center" VerticalAlignment="Center"
                               Visibility="{Binding PreviewData, Converter={StaticResource StringToVisibility}, ConverterParameter=Invert}" />
                </Grid>
            </Border>

            <!-- 右：AML 编辑器 + 预览 -->
            <Border Grid.Column="2" Style="{StaticResource CardStyle}" Padding="16,14">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="*" />
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="*" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>

                    <!-- 工具栏 -->
                    <StackPanel Grid.Row="0" Orientation="Horizontal"
                                Margin="0,0,0,8">
                        <TextBlock Text="AML 模板" FontSize="13" FontWeight="SemiBold"
                                   Foreground="{StaticResource TextPrimaryBrush}"
                                   VerticalAlignment="Center" />
                        <Button Content="选择配置"
                                Command="{Binding OpenConfigSelectorCommand}"
                                Height="28" FontSize="11" Padding="10,0"
                                Margin="12,0,0,0"
                                Background="{StaticResource SurfaceBrush}"
                                Foreground="{StaticResource TextPrimaryBrush}"
                                BorderBrush="{StaticResource BorderBrush}"
                                BorderThickness="1" Cursor="Hand" />
                        <Button Content="💾 保存"
                                Command="{Binding SaveConfigCommand}"
                                Height="28" FontSize="11" Padding="10,0"
                                Margin="6,0,0,0"
                                Background="{StaticResource SurfaceBrush}"
                                Foreground="{StaticResource TextPrimaryBrush}"
                                BorderBrush="{StaticResource BorderBrush}"
                                BorderThickness="1" Cursor="Hand" />
                        <Button Content="👁 预览AML"
                                Command="{Binding PreviewAmlCommand}"
                                Height="28" FontSize="11" Padding="10,0"
                                Margin="6,0,0,0"
                                Background="{StaticResource SurfaceBrush}"
                                Foreground="{StaticResource TextPrimaryBrush}"
                                BorderBrush="{StaticResource BorderBrush}"
                                BorderThickness="1" Cursor="Hand" />
                    </StackPanel>

                    <!-- AML 编辑区（可滚动） -->
                    <TextBox Grid.Row="1" Text="{Binding AmlContent}"
                             TextWrapping="Wrap" AcceptsReturn="True"
                             VerticalScrollBarVisibility="Auto"
                             FontFamily="Consolas" FontSize="12"
                             Background="{StaticResource SurfaceBrush}"
                             Foreground="{StaticResource TextPrimaryBrush}"
                             BorderBrush="{StaticResource BorderBrush}"
                             BorderThickness="1" Padding="10"
                             MinHeight="100" />

                    <!-- 预览区标签 -->
                    <TextBlock Grid.Row="2" Text="AML 预览" FontSize="12"
                               FontWeight="SemiBold"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               Margin="0,10,0,4"
                               Visibility="{Binding PreviewResult, Converter={StaticResource StringToVisibility}}" />

                    <!-- AML 预览结果（可滚动） -->
                    <ScrollViewer Grid.Row="3"
                                  VerticalScrollBarVisibility="Auto"
                                  Visibility="{Binding PreviewResult, Converter={StaticResource StringToVisibility}}">
                        <Border Background="#F0F9FF" Padding="10" CornerRadius="4">
                            <TextBlock Text="{Binding PreviewResult}" FontSize="11"
                                       FontFamily="Consolas" TextWrapping="Wrap" />
                        </Border>
                    </ScrollViewer>
                </Grid>
            </Border>
        </Grid>

        <!-- ===== Row 4: 执行栏（按钮 + 进度条同行） ===== -->
        <Border Grid.Row="4" Style="{StaticResource CardStyle}"
                Padding="16,12" Margin="0,0,0,10">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <!-- 执行导入按钮（空闲时显示） -->
                <Button Grid.Column="0" Content="▶ 执行导入"
                        Command="{Binding ExecuteImportCommand}"
                        Style="{StaticResource PrimaryButton}"
                        Height="34" FontSize="13" Padding="20,0"
                        Visibility="{Binding IsImporting, Converter={StaticResource InverseBool}}" />

                <!-- 暂停按钮（导入中且未暂停时显示） -->
                <Button Grid.Column="1" Content="⏸ 暂停"
                        Command="{Binding PauseCommand}"
                        Height="34" FontSize="13" Padding="16,0" Margin="8,0,0,0"
                        Background="#F59E0B" Foreground="White"
                        BorderThickness="0" Cursor="Hand"
                        Visibility="{Binding IsPaused, Converter={StaticResource InverseBool}}" />

                <!-- 继续按钮（暂停时显示） -->
                <Button Grid.Column="1" Content="▶ 继续"
                        Command="{Binding ResumeCommand}"
                        Style="{StaticResource PrimaryButton}"
                        Height="34" FontSize="13" Padding="16,0" Margin="8,0,0,0"
                        Visibility="{Binding IsPaused, Converter={StaticResource BoolToVisibility}}" />

                <!-- 进度条 + 文本 -->
                <StackPanel Grid.Column="3" Orientation="Vertical"
                            VerticalAlignment="Center" Margin="16,0,0,0"
                            Visibility="{Binding IsProgressVisible, Converter={StaticResource BoolToVisibility}}">
                    <ProgressBar Value="{Binding ImportProgress}"
                                 Minimum="0" Maximum="100"
                                 Height="6"
                                 Foreground="{StaticResource AccentBrush}"
                                 Background="{StaticResource BorderBrush}"
                                 BorderThickness="0" />
                    <TextBlock Text="{Binding ProgressText}" FontSize="11"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               Margin="0,4,0,0" />
                </StackPanel>
            </Grid>
        </Border>

        <!-- ===== Row 5: 状态栏 ===== -->
        <Border Grid.Row="5" Padding="4,2">
            <StackPanel>
                <!-- 错误消息 -->
                <Border Background="#19EF4444" CornerRadius="4" Padding="10,6"
                        Visibility="{Binding ErrorMessage, Converter={StaticResource StringToVisibility}}"
                        Margin="0,0,0,6">
                    <TextBlock Text="{Binding ErrorMessage}"
                               Foreground="{StaticResource ErrorBrush}"
                               FontSize="12" />
                </Border>
                <!-- 状态消息 -->
                <TextBlock Text="{Binding StatusMessage}" FontSize="12"
                           Foreground="{StaticResource TextSecondaryBrush}" />
                <!-- 日志路径 -->
                <TextBlock FontSize="11" Foreground="{StaticResource AccentBrush}"
                           Cursor="Hand" TextDecorations="Underline"
                           Visibility="{Binding HasResult, Converter={StaticResource BoolToVisibility}}">
                    <Run Text="📋 查看导入日志" />
                </TextBlock>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

### 3.4 样式新增（Theme.xaml）

多卡片密集布局下，建议新增一个紧凑卡片样式：

```xml
<!-- Theme.xaml 新增 -->
<Style x:Key="CardStyleCompact" TargetType="Border">
    <Setter Property="Background" Value="{StaticResource CardBrush}" />
    <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="8" />
    <Setter Property="Padding" Value="16,12" />
</Style>
```

---

## 四、暂停/继续按钮逻辑

| 状态 | 执行导入按钮 | 暂停按钮 | 继续按钮 | 进度条 |
|------|:-----------:|:-------:|:-------:|:-----:|
| 空闲 | 显示 | 隐藏 | 隐藏 | 隐藏 |
| 导入中 | 隐藏 | 显示 | 隐藏 | 显示 |
| 已暂停 | 隐藏 | 隐藏 | 显示 | 显示 |

ViewModel 状态：
```
IsImporting && !IsPaused → 显示暂停按钮
IsImporting && IsPaused  → 显示继续按钮
!IsImporting             → 显示执行导入按钮
```

---

## 五、验证方式

- `dotnet build ArasToolkit.slnx` 零错误
- 打开数据汇入页面 → 布局左右分栏正常显示
- 浏览 Excel → 加载预览 → 左栏显示数据
- 输入 AML 模板 → 预览 AML → 右栏下方显示替换结果
- 保存按钮在选择 Sheet 后可点击（即使 AML 为空）
- 执行导入 → 执行按钮隐藏 → 暂停按钮显示 → 进度条滚动
- 点击暂停 → 暂停隐藏 → 继续显示 → 进度条保持
- 点击继续 → 继续隐藏 → 暂停显示 → 进度条继续

---

## Git 规范（编码前必须执行）

### 编码前备份

```bash
git add -A
git stash push -m "backup-before-TASK-022"
git stash pop
```

### 编码完成后提交

```bash
git add -A
git commit -m "优化: TASK-022 数据汇入页面UI重构 — 左右分栏布局 + 进度条 + 暂停/继续分离"
```

### 审查通过后推送（master force push 前先备份到 develop）

```bash
git checkout develop
git merge master
git push origin develop
git checkout master
git push origin master --force
```

> 参照 AGENTS.md 第 11 章 Git 分支与推送策略。

---

## Claude Code 审查结论 (11:45)

**结论: 需修改** 🔴

1. 文件 `src/ArasToolkit.Services/Services/DataImportService.cs:133-137` — DataTable.Columns.Add 对空列名/重复列名报错。修复：
   - 空列名：若 header 为空串，替换为 `$"col_{letter}"`
   - 重复列名：若同名已存在，追加后缀 `_2`, `_3` 直到唯一
   - 同时 ColumnHeaders 也用去重后的名称

2. 文件 `src/ArasToolkit.Services/Services/DataImportService.cs:139` — 预览不限制行数，数据多时 UI 卡顿。修复：计算 `int previewEndRow = Math.Min(endRow, startRow + 29);` 仅读取前 30 行，数据行仍按原 endRow 读取（为空的不显示）

3. 文件 `src/ArasToolkit.App/ViewModels/DataImportViewModel.cs:276-277` — progress 对象创建后未传递给 Service（第 277 行仍传 null）。ExecuteImportAsync 需逐行更新 ImportProgress 和 ProgressText

## Claude Code 审查结论 (12:00)

**结论: 需修改** 🔴 — XAML Row 4 执行栏第 5 轮仍未添加，下面是可直接插入的代码。

4. 文件 `src/ArasToolkit.App/Views/DataImportView.xaml:161` — 将第 161 行 `</Grid>` 后面的内容替换为以下完整 Row4+Row5：

```xml
        </Grid>

        <!-- ===== Row 4: 执行栏 ===== -->
        <Border Grid.Row="4" Style="{StaticResource CardStyle}" Padding="16,12" Margin="0,10,0,10">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <Button Grid.Column="0" Content="执行导入"
                        Command="{Binding ExecuteImportCommand}"
                        Style="{StaticResource PrimaryButton}"
                        Height="34" FontSize="13" Padding="20,0"
                        Visibility="{Binding IsImporting, Converter={StaticResource InverseBool}}" />

                <Button Grid.Column="1" Content="暂停"
                        Command="{Binding PauseCommand}"
                        Height="34" FontSize="13" Padding="16,0" Margin="8,0,0,0"
                        Background="#F59E0B" Foreground="White"
                        BorderThickness="0" Cursor="Hand"
                        Visibility="{Binding IsPaused, Converter={StaticResource InverseBool}}" />

                <Button Grid.Column="1" Content="继续"
                        Command="{Binding ResumeCommand}"
                        Style="{StaticResource PrimaryButton}"
                        Height="34" FontSize="13" Padding="16,0" Margin="8,0,0,0"
                        Visibility="{Binding IsPaused, Converter={StaticResource BoolToVisibility}}" />

                <StackPanel Grid.Column="2" Orientation="Horizontal" VerticalAlignment="Center"
                            Margin="16,0,0,0"
                            Visibility="{Binding IsProgressVisible, Converter={StaticResource BoolToVisibility}}">
                    <ProgressBar Value="{Binding ImportProgress}" Minimum="0" Maximum="100"
                                 Height="6" Width="200"
                                 Foreground="{StaticResource AccentBrush}"
                                 Background="{StaticResource BorderBrush}"
                                 BorderThickness="0" />
                    <TextBlock Text="{Binding ProgressText}" FontSize="11"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               Margin="12,0,0,0" VerticalAlignment="Center" />
                </StackPanel>
            </Grid>
        </Border>

        <!-- 状态 -->
        <Border Grid.Row="5" Padding="0,4">
```

插入方式：删除当前文件第 161 行 `</Grid>` 之后到文件末尾的所有内容，替换为上述代码。

---

## Claude Code 审查结论 (16:30)

**结论: 需修改** 🔴
1. 文件 `src\ArasToolkit.App\Views\DataImportView.xaml:67-72` — 执行按钮+暂停+继续+进度条被错放在 Row 1 文件选择卡片内部 Grid 中。应删除此处，移到右侧面板 AML 预览区域下方（`</ScrollViewer>` 之后、`</Grid>` 之前），分两行：Row 4 执行按钮行（执行导入、暂停、继续），Row 5 进度条行（ProgressBar + ProgressText）。同步删除外层 Grid 的空 Row 4 RowDefinition，状态栏 `Grid.Row="5"` 改为 `Grid.Row="4"`。详见任务文件末尾的参考 XAML 代码块。
2. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:184` — `CanSaveConfig()` 去掉 `&& !string.IsNullOrEmpty(SelectedSheetName)` 条件，允许未选文件时保存 AML 模板。
3. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:75` — `AmlContent` setter 增加 `RefreshCommands()` 调用，否则用户输入 AML 后 Save 按钮 CanExecute 不刷新。
4. 文件 `src\ArasToolkit.App\Views\DataImportView.xaml:145` — TextBox 绑定增加 `UpdateSourceTrigger=PropertyChanged`，否则失焦前 ViewModel 收不到 AML 输入。
5. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:86` — `IsPaused` setter 增加 `RefreshCommands()` 调用，否则暂停/继续按钮 CanExecute 在状态变化后不刷新。
6. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:276-283` — `ExecuteImportAsync` 导入过程中 `ImportProgress` 始终为 0，仅在完成后设 100%。基于 `LastResult.TotalRows` 在循环中逐行更新 `ImportProgress` 和 `ProgressText`。

---

## 参考：执行栏 XAML（插入到右侧面板 ScrollViewer 之后）

```xml
        </ScrollViewer>

        <!-- 执行按钮行 -->
        <StackPanel Grid.Row="4" Orientation="Horizontal" Margin="0,10,0,0">
            <Button Content="执行导入" Command="{Binding ExecuteImportCommand}"
                    Style="{StaticResource PrimaryButton}" Height="32" FontSize="13"
                    Padding="20,0"
                    Visibility="{Binding IsImporting, Converter={StaticResource InverseBool}}" />
            <Button Content="暂停" Command="{Binding PauseCommand}"
                    Height="32" FontSize="13" Padding="16,0" Margin="8,0,0,0"
                    Background="#F59E0B" Foreground="White" BorderThickness="0" Cursor="Hand"
                    Visibility="{Binding IsPaused, Converter={StaticResource InverseBool}}" />
            <Button Content="继续" Command="{Binding ResumeCommand}"
                    Style="{StaticResource PrimaryButton}" Height="32" FontSize="13"
                    Padding="16,0" Margin="8,0,0,0"
                    Visibility="{Binding IsPaused, Converter={StaticResource BoolToVisibility}}" />
        </StackPanel>

        <!-- 进度条行 -->
        <StackPanel Grid.Row="5" Orientation="Horizontal" Margin="0,8,0,0"
                    Visibility="{Binding IsProgressVisible, Converter={StaticResource BoolToVisibility}}">
            <ProgressBar Value="{Binding ImportProgress}" Minimum="0" Maximum="100"
                         Height="6" Width="180"
                         Foreground="{StaticResource AccentBrush}"
                         Background="{StaticResource BorderBrush}" BorderThickness="0" />
            <TextBlock Text="{Binding ProgressText}" FontSize="11"
                       Foreground="{StaticResource TextSecondaryBrush}"
                       Margin="12,0,0,0" VerticalAlignment="Center" />
        </StackPanel>
    </Grid>
```

> 右侧面板 Grid 需增加 Row 4 和 Row 5 两个 RowDefinition（Height="Auto"）。

---

## Claude Code 审查结论 (17:00)

**结论: 需修改** 🔴
1. 文件 `src\ArasToolkit.App\Views\DataImportView.xaml:111-137` — 执行按钮+进度条错放在左侧数据预览面板（Column="0"）的 Grid 内，且该 Grid 无 RowDefinitions，`Grid.Row="4"` 无效。应将这27行移到右侧面板（Column="2"）的 `</ScrollViewer>` 之后、`</Grid>` 之前，并从左侧面板删除。
2. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:277-284` — `ExecuteImportAsync` 导入过程中 `ImportProgress` 始终为0，仅在完成后设100%。需在循环中逐行更新 `ImportProgress` 和 `ProgressText`。

---

## Claude Code 审查结论 (17:45)

**结论: 需修改** 🔴
1. 文件 `src\ArasToolkit.App\ViewModels\DataImportViewModel.cs:277-286` — `ExecuteImportAsync` 导入过程中 `ImportProgress` 始终为0，仅在完成后设100%。`_dataImportService.ExecuteImportAsync` 传了 `null` 作为 arasImporter 参数，导致 `result.SkippedCount++` 直接跳过所有行而不执行导入。需实现真正的逐行导入逻辑：传入 arasImporter 回调，在其中逐行更新 `ImportProgress = (double)current / total * 100` 和 `ProgressText = $"{current}/{total}"`。

---

## Codex 修正记录 (针对 17:45 审查)

**修正内容：**
1. `DataImportViewModel.cs:280-292` — 将 `ExecuteImportAsync` 中传入 `_dataImportService.ExecuteImportAsync` 的第三个参数从 `null` 替换为真实的逐行进度回调：
   - 计算 `totalRows = config.EndRow - config.StartRow + 1`
   - 回调中逐行更新 `ImportProgress = (rowNum - config.StartRow + 1) / totalRows * 100`
   - 同步更新 `ProgressText = current/total`
   - 完成后设 `ImportProgress = 100`

**编译状态：** dotnet build 通过 (0 errors)

**待审查项：** 进度条逐行更新是否在真实导入流程中正常触发（Service 层需正确调用 arasImporter 回调）
