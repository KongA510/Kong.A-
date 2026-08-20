using System;
using System.Collections.Generic;
using System.Linq;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 工作流程设定页（WinUI 3）。
/// 画布支持拖动节点和转折点、双击路径新增折点、右键折点删除。
/// </summary>
public sealed partial class WorkflowMapPage : Page
{
    private const double NodeVisualWidth = 150;
    private const double NodeVisualHeight = 64;
    private const double NodeRadius = 16;

    private readonly WorkflowMapViewModel _viewModel;
    private readonly IWorkflowMapService _workflowMapService;
    private readonly Dictionary<WorkflowMapPath, Polyline> _pathLines = [];
    private readonly Canvas WorkflowCanvas = new() { Width = 1320, Height = 440 };
    private readonly StackPanel NodeEditorPanel = new() { Spacing = 8 };
    private readonly StackPanel PathEditorPanel = new() { Spacing = 8 };

    private FrameworkElement? _dragElement;
    private WorkflowMapNode? _dragNode;
    private BendHandle? _dragBend;
    private Point _dragOffset;

    public WorkflowMapPage()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<WorkflowMapViewModel>();
        _workflowMapService = App.Services.GetRequiredService<IWorkflowMapService>();
        DataContext = _viewModel;
        BuildPageLayout();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.PreviewChanged -= OnPreviewChanged;
        _viewModel.PreviewChanged += OnPreviewChanged;
        RenderDefinition();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => _viewModel.PreviewChanged -= OnPreviewChanged;

    private void OnPreviewChanged()
        => DispatcherQueue.TryEnqueue(RenderDefinition);

    private void RenderDefinition()
    {
        BuildCanvas();
        BuildEditorPanels();
    }

    /// <summary>
    /// 程序化构建页面布局。复杂编辑行不进入 XAML，可避免 Windows App SDK 1.7
    /// 在大型模板组合下无错误信息直接退出的问题。
    /// </summary>
    private void BuildPageLayout()
    {
        RootLayout.Margin = new Thickness(24);
        RootLayout.RowSpacing = 10;
        RootLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RootLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RootLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        RootLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var header = new StackPanel();
        header.Children.Add(new TextBlock
        {
            Text = "工作流程设定",
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = Brush(17, 24, 39)
        });
        header.Children.Add(new TextBlock
        {
            Text = "读取一份 Excel 生成一份 Workflow Map；先预览调整，再以一个完整 AML 一次性汇入 Aras",
            FontSize = 13,
            Margin = new Thickness(0, 4, 0, 0),
            Foreground = Brush(107, 114, 128)
        });
        RootLayout.Children.Add(header);

        var toolbarCard = Card(new Thickness(16, 12, 16, 12));
        Grid.SetRow(toolbarCard, 1);
        toolbarCard.Child = BuildToolbar();
        RootLayout.Children.Add(toolbarCard);

        var statusPanel = new StackPanel { Spacing = 4 };
        Grid.SetRow(statusPanel, 2);
        statusPanel.Children.Add(StatusLine("StatusMessage", Brush(55, 48, 163), Brush(238, 242, 255)));
        statusPanel.Children.Add(StatusLine("WarningsText", Brush(146, 64, 14), Brush(254, 243, 199)));
        statusPanel.Children.Add(StatusLine("ErrorMessage", Brush(185, 28, 28), Brush(254, 226, 226)));
        RootLayout.Children.Add(statusPanel);

        var content = BuildContentArea();
        Grid.SetRow(content, 3);
        RootLayout.Children.Add(content);
    }

    private FrameworkElement BuildToolbar()
    {
        var panel = new StackPanel { Spacing = 10 };

        var inputGrid = new Grid { ColumnSpacing = 10 };
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(11, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var nameBox = new TextBox { Header = "工作流程名称（手动输入）", PlaceholderText = "例如：送审流程" };
        Bind(nameBox, TextBox.TextProperty, "WorkflowName", BindingMode.TwoWay);
        inputGrid.Children.Add(nameBox);

        var descriptionBox = new TextBox { Header = "描述（可选）", PlaceholderText = "说明该流程的用途" };
        Bind(descriptionBox, TextBox.TextProperty, "Description", BindingMode.TwoWay);
        Grid.SetColumn(descriptionBox, 1);
        inputGrid.Children.Add(descriptionBox);

        var modeBox = new ComboBox
        {
            Header = "导入模式",
            ItemsSource = _viewModel.ImportModeOptions,
            MinWidth = 110
        };
        Bind(modeBox, ComboBox.SelectedItemProperty, "ImportMode", BindingMode.TwoWay);
        Grid.SetColumn(modeBox, 2);
        inputGrid.Children.Add(modeBox);

        var owner = new StackPanel { VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(4, 0, 0, 6) };
        owner.Children.Add(new TextBlock { Text = "流程所有者", FontSize = 11, Foreground = Brush(107, 114, 128) });
        owner.Children.Add(new TextBlock
        {
            Text = "Creator（节点留空时继承）",
            FontSize = 13,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = Brush(17, 24, 39),
            Margin = new Thickness(0, 6, 0, 0)
        });
        Grid.SetColumn(owner, 3);
        inputGrid.Children.Add(owner);
        panel.Children.Add(inputGrid);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 9 };
        buttons.Children.Add(CommandButton("下载模板", _viewModel.DownloadTemplateCommand, false));
        buttons.Children.Add(CommandButton("选择文件并预览", _viewModel.BrowseFileCommand, true));
        var fileName = new TextBlock
        {
            MaxWidth = 250,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brush(107, 114, 128)
        };
        Bind(fileName, TextBlock.TextProperty, "FileName");
        buttons.Children.Add(fileName);
        buttons.Children.Add(CommandButton("刷新预览", _viewModel.RefreshPreviewCommand, false));
        buttons.Children.Add(CommandButton("生成最终 AML", _viewModel.GenerateAmlCommand, false));
        buttons.Children.Add(CommandButton("导出 AML", _viewModel.ExportAmlCommand, false));
        buttons.Children.Add(CommandButton("确认并汇入 Aras", _viewModel.ImportCommand, true));
        var progress = new ProgressRing { Width = 22, Height = 22 };
        Bind(progress, ProgressRing.IsActiveProperty, "IsBusy");
        buttons.Children.Add(progress);

        panel.Children.Add(new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = buttons
        });
        return panel;
    }

    private FrameworkElement BuildContentArea()
    {
        var content = new Grid { RowSpacing = 12 };
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(520) });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(390) });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(430) });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(260) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var canvasSection = new Grid { RowSpacing = 8, Padding = new Thickness(12) };
        canvasSection.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        canvasSection.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        var canvasHeader = new Grid();
        canvasHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        canvasHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        canvasHeader.Children.Add(new TextBlock
        {
            Text = "流程画布  →  从左到右",
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = Brush(31, 41, 55),
            VerticalAlignment = VerticalAlignment.Center
        });
        var arrangeButton = new Button
        {
            Content = "自动横向布局",
            Height = 32,
            Padding = new Thickness(14, 0, 14, 0),
            IsEnabled = _viewModel.HasPreview
        };
        arrangeButton.Click += (_, _) => _viewModel.ArrangePreviewLeftToRight();
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(WorkflowMapViewModel.HasPreview))
                arrangeButton.IsEnabled = _viewModel.HasPreview;
        };
        Grid.SetColumn(arrangeButton, 1);
        canvasHeader.Children.Add(arrangeButton);
        canvasSection.Children.Add(canvasHeader);

        var canvasHost = new Grid { Background = Brush(248, 250, 252) };
        canvasHost.Children.Add(new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollMode = ScrollMode.Enabled,
            VerticalScrollMode = ScrollMode.Enabled,
            Content = WorkflowCanvas
        });
        var canvasHelp = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(12),
            Padding = new Thickness(10, 6, 10, 6),
            CornerRadius = new CornerRadius(6),
            Background = Brush(255, 255, 255),
            Opacity = 0.9,
            Child = new TextBlock
            {
                Text = "拖动节点或橙色折点调整；双击路径新增折点；右键折点删除。segments 为绝对坐标 x,y|x,y。",
                FontSize = 11,
                Foreground = Brush(107, 114, 128)
            }
        };
        canvasHost.Children.Add(canvasHelp);
        Grid.SetRow(canvasHost, 1);
        canvasSection.Children.Add(canvasHost);
        var canvasBorder = Framed(canvasSection);
        content.Children.Add(canvasBorder);

        var nodeHost = EditorHost("节点参数", NodeEditorPanel);
        Grid.SetRow(nodeHost, 1);
        content.Children.Add(nodeHost);

        var pathHost = EditorHost("路径参数 / 转折点", PathEditorPanel);
        Grid.SetRow(pathHost, 2);
        content.Children.Add(pathHost);

        var amlGrid = new Grid { RowSpacing = 5 };
        amlGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        amlGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        amlGrid.Children.Add(new TextBlock
        {
            Text = "完整执行 AML：确认导入时会把以下内容交给 Innovator.applyAML，可先导出并在 Nash 中排查。",
            FontSize = 12,
            Foreground = Brush(107, 114, 128)
        });
        var amlBox = new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.NoWrap,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12
        };
        ScrollViewer.SetHorizontalScrollBarVisibility(amlBox, ScrollBarVisibility.Auto);
        ScrollViewer.SetVerticalScrollBarVisibility(amlBox, ScrollBarVisibility.Auto);
        Bind(amlBox, TextBox.TextProperty, "AmlText");
        Grid.SetRow(amlBox, 1);
        amlGrid.Children.Add(amlBox);
        Grid.SetRow(amlGrid, 3);
        content.Children.Add(amlGrid);

        return new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollMode = ScrollMode.Enabled,
            Content = content
        };
    }

    private void BuildEditorPanels()
    {
        NodeEditorPanel.Children.Clear();
        PathEditorPanel.Children.Clear();
        var definition = _viewModel.PreviewDefinition;
        if (definition == null)
        {
            NodeEditorPanel.Children.Add(EmptyEditorText("选择 Excel 后显示节点参数"));
            PathEditorPanel.Children.Add(EmptyEditorText("选择 Excel 后显示路径和 segments"));
            return;
        }

        NodeEditorPanel.Children.Add(NodeHeader());
        foreach (var node in definition.Nodes)
            NodeEditorPanel.Children.Add(NodeRow(node));

        PathEditorPanel.Children.Add(PathHeader());
        foreach (var path in definition.Paths)
            PathEditorPanel.Children.Add(PathRow(path));
    }

    private static Grid NodeHeader()
    {
        var grid = EditorGrid([120, 180, 110, 200, 70, 90, 90, 260]);
        AddHeader(grid, 0, "编码");
        AddHeader(grid, 1, "节点名称");
        AddHeader(grid, 2, "类型");
        AddHeader(grid, 3, "执行角色（空=所有者）");
        AddHeader(grid, 4, "自动");
        AddHeader(grid, 5, "X");
        AddHeader(grid, 6, "Y");
        AddHeader(grid, 7, "提示信息");
        return grid;
    }

    private Grid NodeRow(WorkflowMapNode node)
    {
        var grid = EditorGrid([120, 180, 110, 200, 70, 90, 90, 260]);
        AddTextEditor(grid, 0, node.Code, value => node.Code = value);
        AddTextEditor(grid, 1, node.Name, value => node.Name = value);

        var type = new ComboBox
        {
            ItemsSource = node.ActivityTypeOptions,
            SelectedItem = node.ActivityType,
            FontSize = 12,
            MinHeight = 36
        };
        type.SelectionChanged += (_, _) => node.ActivityType = type.SelectedItem?.ToString() ?? WorkflowMapNode.NodeTypeNormal;
        AddEditor(grid, 2, type);
        AddTextEditor(grid, 3, node.Assignee, value => node.Assignee = value);

        var automatic = new CheckBox { IsChecked = node.IsAutomatic, HorizontalAlignment = HorizontalAlignment.Center };
        automatic.Checked += (_, _) => node.IsAutomatic = true;
        automatic.Unchecked += (_, _) => node.IsAutomatic = false;
        AddEditor(grid, 4, automatic);
        AddNumberEditor(grid, 5, node.X, value => node.X = value);
        AddNumberEditor(grid, 6, node.Y, value => node.Y = value);
        AddTextEditor(grid, 7, node.Message, value => node.Message = value);
        return grid;
    }

    private static Grid PathHeader()
    {
        var grid = EditorGrid([120, 120, 190, 65, 65, 120, 300, 90, 90]);
        AddHeader(grid, 0, "来源");
        AddHeader(grid, 1, "目标");
        AddHeader(grid, 2, "路径名称");
        AddHeader(grid, 3, "默认");
        AddHeader(grid, 4, "覆盖");
        AddHeader(grid, 5, "认证");
        AddHeader(grid, 6, "segments（x,y|x,y）");
        AddHeader(grid, 7, "偏移X");
        AddHeader(grid, 8, "偏移Y");
        return grid;
    }

    private Grid PathRow(WorkflowMapPath path)
    {
        var grid = EditorGrid([120, 120, 190, 65, 65, 120, 300, 90, 90]);
        AddTextEditor(grid, 0, path.SourceCode, value => path.SourceCode = value);
        AddTextEditor(grid, 1, path.TargetCode, value => path.TargetCode = value);
        AddTextEditor(grid, 2, path.Name, value => path.Name = value);

        var isDefault = new CheckBox { IsChecked = path.IsDefault, HorizontalAlignment = HorizontalAlignment.Center };
        isDefault.Checked += (_, _) => path.IsDefault = true;
        isDefault.Unchecked += (_, _) => path.IsDefault = false;
        AddEditor(grid, 3, isDefault);
        var isOverride = new CheckBox { IsChecked = path.IsOverride, HorizontalAlignment = HorizontalAlignment.Center };
        isOverride.Checked += (_, _) => path.IsOverride = true;
        isOverride.Unchecked += (_, _) => path.IsOverride = false;
        AddEditor(grid, 4, isOverride);

        var authentication = new ComboBox
        {
            ItemsSource = path.AuthenticationOptions,
            SelectedItem = path.Authentication,
            FontSize = 12,
            MinHeight = 36
        };
        authentication.SelectionChanged += (_, _) => path.Authentication = authentication.SelectedItem?.ToString() ?? "none";
        AddEditor(grid, 5, authentication);
        AddTextEditor(grid, 6, path.Segments, value => path.Segments = value, "390,75|220,75");
        AddNullableNumberEditor(grid, 7, path.LabelOffsetX, value => path.LabelOffsetX = value);
        AddNullableNumberEditor(grid, 8, path.LabelOffsetY, value => path.LabelOffsetY = value);
        return grid;
    }

    private static Border EditorHost(string title, StackPanel panel)
    {
        var grid = new Grid { RowSpacing = 8, Padding = new Thickness(14) };
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 15,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = Brush(31, 41, 55)
        });
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = panel
        };
        Grid.SetRow(scroll, 1);
        grid.Children.Add(scroll);
        return Framed(grid);
    }

    private static Grid EditorGrid(IReadOnlyList<double> widths)
    {
        var grid = new Grid
        {
            ColumnSpacing = 8,
            MinHeight = 38,
            MinWidth = widths.Sum() + widths.Count * 8
        };
        foreach (var width in widths)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width) });
        return grid;
    }

    private static void AddHeader(Grid grid, int column, string text)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = 11,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = Brush(75, 85, 99),
            VerticalAlignment = VerticalAlignment.Center
        };
        AddEditor(grid, column, block);
    }

    private static void AddTextEditor(
        Grid grid,
        int column,
        string value,
        Action<string> update,
        string placeholder = "")
    {
        var editor = new TextBox { Text = value, FontSize = 12, PlaceholderText = placeholder, MinHeight = 36 };
        editor.TextChanged += (_, _) => update(editor.Text);
        AddEditor(grid, column, editor);
    }

    private static void AddNumberEditor(Grid grid, int column, int value, Action<int> update)
    {
        var editor = new NumberBox
        {
            Value = value,
            Minimum = -5000,
            Maximum = 10000,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden,
            FontSize = 12,
            MinHeight = 36
        };
        editor.ValueChanged += (_, args) =>
        {
            if (!double.IsNaN(args.NewValue)) update((int)Math.Round(args.NewValue));
        };
        AddEditor(grid, column, editor);
    }

    private static void AddNullableNumberEditor(Grid grid, int column, int? value, Action<int?> update)
    {
        var editor = new NumberBox
        {
            Value = value ?? double.NaN,
            Minimum = -5000,
            Maximum = 10000,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden,
            FontSize = 12,
            MinHeight = 36
        };
        editor.ValueChanged += (_, args) => update(double.IsNaN(args.NewValue)
            ? null
            : (int)Math.Round(args.NewValue));
        AddEditor(grid, column, editor);
    }

    private static void AddEditor(Grid grid, int column, FrameworkElement editor)
    {
        Grid.SetColumn(editor, column);
        grid.Children.Add(editor);
    }

    private static TextBlock EmptyEditorText(string text) => new()
    {
        Text = text,
        FontSize = 12,
        Foreground = Brush(156, 163, 175),
        Margin = new Thickness(4, 8, 4, 4)
    };

    private static Border StatusLine(string bindingPath, Brush foreground, Brush background)
    {
        var text = new TextBlock { FontSize = 12, TextWrapping = TextWrapping.Wrap, Foreground = foreground };
        Bind(text, TextBlock.TextProperty, bindingPath);
        return new Border
        {
            Padding = new Thickness(10, 6, 10, 6),
            CornerRadius = new CornerRadius(6),
            Background = background,
            Child = text
        };
    }

    private static Button CommandButton(string text, System.Windows.Input.ICommand command, bool accent)
    {
        var button = new Button { Content = text, Command = command, Height = 34, Padding = new Thickness(14, 0, 14, 0) };
        var styleKey = accent ? "AccentButton" : "PrimaryButton";
        if (Application.Current.Resources.TryGetValue(styleKey, out var value) && value is Style style)
            button.Style = style;
        return button;
    }

    private static Border Card(Thickness padding) => new()
    {
        Padding = padding,
        CornerRadius = new CornerRadius(10),
        Background = Brush(255, 255, 255),
        BorderBrush = Brush(229, 231, 235),
        BorderThickness = new Thickness(1)
    };

    private static Border Framed(UIElement child) => new()
    {
        Child = child,
        CornerRadius = new CornerRadius(6),
        BorderBrush = Brush(229, 231, 235),
        BorderThickness = new Thickness(1),
        Background = Brush(255, 255, 255)
    };

    private static SolidColorBrush Brush(byte red, byte green, byte blue)
        => new(Color.FromArgb(255, red, green, blue));

    private static void Bind(
        FrameworkElement target,
        DependencyProperty property,
        string path,
        BindingMode mode = BindingMode.OneWay)
    {
        target.SetBinding(property, new Binding
        {
            Path = new PropertyPath(path),
            Mode = mode,
            UpdateSourceTrigger = mode == BindingMode.TwoWay
                ? UpdateSourceTrigger.PropertyChanged
                : UpdateSourceTrigger.Default
        });
    }

    private void BuildCanvas()
    {
        WorkflowCanvas.Children.Clear();
        _pathLines.Clear();
        var definition = _viewModel.PreviewDefinition;
        if (definition == null) return;

        var allPoints = definition.Nodes.Select(node => new WorkflowMapPoint(node.X, node.Y)).ToList();
        foreach (var path in definition.Paths)
        {
            try { allPoints.AddRange(_workflowMapService.ParseSegments(path.Segments)); }
            catch { /* 无效输入由 ViewModel 校验并显示，不阻断画布其余内容。 */ }
        }

        WorkflowCanvas.Width = Math.Max(1320, allPoints.DefaultIfEmpty().Max(point => point.X) + 220);
        WorkflowCanvas.Height = Math.Max(440, allPoints.DefaultIfEmpty().Max(point => point.Y) + 120);

        foreach (var path in definition.Paths)
            DrawPath(definition, path);
        foreach (var node in definition.Nodes)
            DrawNode(node);
    }

    private void DrawPath(WorkflowMapDefinition definition, WorkflowMapPath path)
    {
        var source = definition.Nodes.FirstOrDefault(node =>
            string.Equals(node.Code, path.SourceCode, StringComparison.OrdinalIgnoreCase));
        var target = definition.Nodes.FirstOrDefault(node =>
            string.Equals(node.Code, path.TargetCode, StringComparison.OrdinalIgnoreCase));
        if (source == null || target == null) return;

        var points = BuildPathPoints(source, target, path);
        var isReturnPath = target.SortOrder <= source.SortOrder;
        var line = new Polyline
        {
            Stroke = new SolidColorBrush(isReturnPath
                ? Color.FromArgb(255, 217, 119, 6)
                : Color.FromArgb(255, 100, 116, 139)),
            StrokeThickness = isReturnPath || path.IsOverride ? 2.5 : 1.9,
            StrokeLineJoin = PenLineJoin.Round,
            Tag = path,
            Points = ToPointCollection(points)
        };
        if (isReturnPath)
            line.StrokeDashArray = new DoubleCollection { 7, 4 };
        line.DoubleTapped += Path_DoubleTapped;
        WorkflowCanvas.Children.Add(line);
        _pathLines[path] = line;

        DrawArrow(points);

        var label = new Border
        {
            Padding = new Thickness(6, 2, 6, 2),
            CornerRadius = new CornerRadius(5),
            Background = new SolidColorBrush(isReturnPath
                ? Color.FromArgb(255, 255, 247, 237)
                : Color.FromArgb(244, 255, 255, 255)),
            BorderBrush = new SolidColorBrush(isReturnPath
                ? Color.FromArgb(255, 253, 186, 116)
                : Color.FromArgb(255, 226, 232, 240)),
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = path.Name,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 31, 41, 55))
            }
        };
        Canvas.SetLeft(label, source.X + (path.LabelOffsetX ?? 0));
        Canvas.SetTop(label, source.Y + (path.LabelOffsetY ?? 0));
        WorkflowCanvas.Children.Add(label);

        IReadOnlyList<WorkflowMapPoint> bends;
        try { bends = _workflowMapService.ParseSegments(path.Segments); }
        catch { bends = Array.Empty<WorkflowMapPoint>(); }
        for (var index = 0; index < bends.Count; index++)
            DrawBendHandle(path, index, bends[index]);
    }

    private void DrawArrow(IReadOnlyList<WorkflowMapPoint> points)
    {
        if (points.Count < 2) return;
        var from = points[^2];
        var to = points[^1];
        var dx = to.X - from.X;
        var dy = to.Y - from.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 1) return;

        var ux = dx / length;
        var uy = dy / length;
        var tipX = to.X - ux * (NodeRadius + 2);
        var tipY = to.Y - uy * (NodeRadius + 2);
        var baseX = tipX - ux * 10;
        var baseY = tipY - uy * 10;
        var perpendicularX = -uy * 5;
        var perpendicularY = ux * 5;

        var arrow = new Polygon
        {
            Fill = new SolidColorBrush(Color.FromArgb(255, 107, 114, 128)),
            Points = new PointCollection
            {
                new(tipX, tipY),
                new(baseX + perpendicularX, baseY + perpendicularY),
                new(baseX - perpendicularX, baseY - perpendicularY)
            }
        };
        WorkflowCanvas.Children.Add(arrow);
    }

    private void DrawBendHandle(WorkflowMapPath path, int index, WorkflowMapPoint point)
    {
        var handle = new Ellipse
        {
            Width = 14,
            Height = 14,
            Fill = new SolidColorBrush(Color.FromArgb(255, 245, 158, 11)),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 2,
            Tag = new BendHandle(path, index)
        };
        handle.PointerPressed += Bend_PointerPressed;
        handle.PointerMoved += Drag_PointerMoved;
        handle.PointerReleased += Drag_PointerReleased;
        handle.PointerCanceled += Drag_PointerReleased;
        handle.RightTapped += Bend_RightTapped;
        Canvas.SetLeft(handle, point.X - 7);
        Canvas.SetTop(handle, point.Y - 7);
        WorkflowCanvas.Children.Add(handle);
    }

    private void DrawNode(WorkflowMapNode node)
    {
        var foreground = node.IsStart
            ? Color.FromArgb(255, 49, 70, 180)
            : node.IsEnd
                ? Color.FromArgb(255, 220, 38, 38)
                : Color.FromArgb(255, 107, 114, 128);

        var grid = new Grid
        {
            Width = NodeVisualWidth,
            Height = NodeVisualHeight,
            Tag = node,
            Background = new SolidColorBrush(Colors.Transparent)
        };
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(38) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var symbol = new TextBlock
        {
            Text = node.IsStart ? "▶" : node.IsEnd ? "×" : "●",
            FontSize = node.IsEnd ? 28 : 25,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(foreground),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var label = new TextBlock
        {
            Text = node.Name,
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 31, 41, 55)),
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxWidth = NodeVisualWidth
        };
        Grid.SetRow(label, 1);
        grid.Children.Add(symbol);
        grid.Children.Add(label);

        grid.PointerPressed += Node_PointerPressed;
        grid.PointerMoved += Drag_PointerMoved;
        grid.PointerReleased += Drag_PointerReleased;
        grid.PointerCanceled += Drag_PointerReleased;
        Canvas.SetLeft(grid, node.X - NodeVisualWidth / 2);
        Canvas.SetTop(grid, node.Y - 21);
        WorkflowCanvas.Children.Add(grid);
    }

    private void Node_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: WorkflowMapNode node } element) return;
        var point = e.GetCurrentPoint(WorkflowCanvas).Position;
        _dragElement = element;
        _dragNode = node;
        _dragBend = null;
        _dragOffset = new Point(point.X - node.X, point.Y - node.Y);
        element.CapturePointer(e.Pointer);
        _viewModel.BeginVisualEdit();
        e.Handled = true;
    }

    private void Bend_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: BendHandle bend } element) return;
        IReadOnlyList<WorkflowMapPoint> points;
        try { points = _workflowMapService.ParseSegments(bend.Path.Segments); }
        catch { return; }
        if (bend.Index < 0 || bend.Index >= points.Count) return;

        var current = points[bend.Index];
        var pointer = e.GetCurrentPoint(WorkflowCanvas).Position;
        _dragElement = element;
        _dragNode = null;
        _dragBend = bend;
        _dragOffset = new Point(pointer.X - current.X, pointer.Y - current.Y);
        element.CapturePointer(e.Pointer);
        _viewModel.BeginVisualEdit();
        e.Handled = true;
    }

    private void Drag_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_dragElement == null || !e.Pointer.IsInContact) return;
        var pointer = e.GetCurrentPoint(WorkflowCanvas).Position;
        var x = Math.Max(24, (int)Math.Round(pointer.X - _dragOffset.X));
        var y = Math.Max(24, (int)Math.Round(pointer.Y - _dragOffset.Y));

        if (_dragNode != null)
        {
            _dragNode.X = x;
            _dragNode.Y = y;
            Canvas.SetLeft(_dragElement, x - NodeVisualWidth / 2);
            Canvas.SetTop(_dragElement, y - 21);
            UpdatePathLines();
        }
        else if (_dragBend != null)
        {
            var points = _workflowMapService.ParseSegments(_dragBend.Path.Segments).ToList();
            if (_dragBend.Index >= 0 && _dragBend.Index < points.Count)
            {
                points[_dragBend.Index] = new WorkflowMapPoint(x, y);
                _dragBend.Path.Segments = _workflowMapService.SerializeSegments(points);
                Canvas.SetLeft(_dragElement, x - 6);
                Canvas.SetTop(_dragElement, y - 6);
                UpdatePathLines();
            }
        }

        e.Handled = true;
    }

    private void Drag_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_dragElement == null) return;
        _dragElement.ReleasePointerCapture(e.Pointer);
        _dragElement = null;
        _dragNode = null;
        _dragBend = null;
        _viewModel.EndVisualEdit();
        e.Handled = true;
    }

    private void Path_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not Polyline { Tag: WorkflowMapPath path }) return;
        var definition = _viewModel.PreviewDefinition;
        if (definition == null) return;
        var source = definition.Nodes.FirstOrDefault(node =>
            string.Equals(node.Code, path.SourceCode, StringComparison.OrdinalIgnoreCase));
        var target = definition.Nodes.FirstOrDefault(node =>
            string.Equals(node.Code, path.TargetCode, StringComparison.OrdinalIgnoreCase));
        if (source == null || target == null) return;

        var click = e.GetPosition(WorkflowCanvas);
        var bends = _workflowMapService.ParseSegments(path.Segments).ToList();
        var polyline = new List<WorkflowMapPoint> { new(source.X, source.Y) };
        polyline.AddRange(bends);
        polyline.Add(new WorkflowMapPoint(target.X, target.Y));

        var insertAt = 0;
        var bestDistance = double.MaxValue;
        for (var index = 0; index < polyline.Count - 1; index++)
        {
            var distance = DistanceToSegment(click, polyline[index], polyline[index + 1]);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                insertAt = index;
            }
        }

        bends.Insert(insertAt, new WorkflowMapPoint((int)Math.Round(click.X), (int)Math.Round(click.Y)));
        path.Segments = _workflowMapService.SerializeSegments(bends);
        _viewModel.NotifyDefinitionEdited();
        e.Handled = true;
    }

    private void Bend_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: BendHandle bend }) return;
        var points = _workflowMapService.ParseSegments(bend.Path.Segments).ToList();
        if (bend.Index < 0 || bend.Index >= points.Count) return;
        points.RemoveAt(bend.Index);
        bend.Path.Segments = _workflowMapService.SerializeSegments(points);
        _viewModel.NotifyDefinitionEdited();
        e.Handled = true;
    }

    private void UpdatePathLines()
    {
        var definition = _viewModel.PreviewDefinition;
        if (definition == null) return;
        foreach (var pair in _pathLines)
        {
            var path = pair.Key;
            var source = definition.Nodes.FirstOrDefault(node =>
                string.Equals(node.Code, path.SourceCode, StringComparison.OrdinalIgnoreCase));
            var target = definition.Nodes.FirstOrDefault(node =>
                string.Equals(node.Code, path.TargetCode, StringComparison.OrdinalIgnoreCase));
            if (source != null && target != null)
                pair.Value.Points = ToPointCollection(BuildPathPoints(source, target, path));
        }
    }

    private List<WorkflowMapPoint> BuildPathPoints(
        WorkflowMapNode source,
        WorkflowMapNode target,
        WorkflowMapPath path)
    {
        var points = new List<WorkflowMapPoint> { new(source.X, source.Y) };
        try { points.AddRange(_workflowMapService.ParseSegments(path.Segments)); }
        catch { /* 页面会显示格式错误；画布仍绘制起终点直线。 */ }
        points.Add(new WorkflowMapPoint(target.X, target.Y));
        return points;
    }

    private static PointCollection ToPointCollection(IEnumerable<WorkflowMapPoint> points)
    {
        var result = new PointCollection();
        foreach (var point in points)
            result.Add(new Point(point.X, point.Y));
        return result;
    }

    private static double DistanceToSegment(Point point, WorkflowMapPoint start, WorkflowMapPoint end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        if (dx == 0 && dy == 0)
            return Math.Sqrt(Math.Pow(point.X - start.X, 2) + Math.Pow(point.Y - start.Y, 2));

        var t = ((point.X - start.X) * dx + (point.Y - start.Y) * dy) / (dx * dx + dy * dy);
        t = Math.Max(0, Math.Min(1, t));
        var projectedX = start.X + t * dx;
        var projectedY = start.Y + t * dy;
        return Math.Sqrt(Math.Pow(point.X - projectedX, 2) + Math.Pow(point.Y - projectedY, 2));
    }

    private sealed record BendHandle(WorkflowMapPath Path, int Index);
}
