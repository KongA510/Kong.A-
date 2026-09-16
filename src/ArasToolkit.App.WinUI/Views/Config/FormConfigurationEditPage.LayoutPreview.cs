using ArasToolkit.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class FormConfigurationEditPage
{
    private static readonly (string Name, string Label, double Width)[] PreviewColumns =
    [
        ("sequence", "#", 30), ("name", "字段", 200), ("field_type", "控件类型", 140),
        ("x", "X", 70), ("y", "Y", 70), ("display_length", "显示长度", 84),
        ("textarea_rows", "行尺寸", 80), ("textarea_cols", "列尺寸", 80),
        ("is_disabled", "不可编辑", 76), ("font_color", "标题颜色", 220)
    ];
    private readonly Dictionary<string, Action<FormEditorItem, int>> _previewRefresh = [];
    private readonly Dictionary<string, Border> _previewRowBorders = [];
    private string[] _previewIds = [];
    private bool _syncingPreview;

    private void RevealSelectedPreview()
    {
        if (EditorDock.Visibility != Visibility.Visible || LayoutPreviewPanel.Visibility != Visibility.Visible) return;
        var id = _vm.Session?.SelectedIds.FirstOrDefault();
        if (id == null || !_previewRowBorders.TryGetValue(id, out var row) || row.Visibility != Visibility.Visible) return;
        // Reveal the selected row vertically without moving away from the column being edited.
        var top = row.TransformToVisual(LayoutPreviewRows).TransformPoint(new Windows.Foundation.Point()).Y;
        var scroll = LayoutPreviewBodyScroll;
        if (top < scroll.VerticalOffset) scroll.ChangeView(null, top, null, true);
        else if (top + row.ActualHeight > scroll.VerticalOffset + scroll.ViewportHeight)
            scroll.ChangeView(null, Math.Max(0, top + row.ActualHeight - scroll.ViewportHeight), null, true);
    }

    private static Grid CreatePreviewGrid()
    {
        var grid = new Grid { Padding = new Thickness(6), Width = PreviewColumns.Sum(column => column.Width) + 12, HorizontalAlignment = HorizontalAlignment.Left };
        foreach (var column in PreviewColumns) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(column.Width) });
        return grid;
    }

    private void SyncLayoutPreview()
    {
        if (_syncingPreview) return;
        _syncingPreview = true;
        try
        {
            var fields = _vm.Session?.Fields.ToList() ?? [];
            var ids = fields.Select(field => field.Id).ToArray();
            if (!_previewIds.SequenceEqual(ids) || LayoutPreviewRows.Children.Count == 0)
            {
                _previewIds = ids;
                _previewRefresh.Clear();
                _previewRowBorders.Clear();
                LayoutPreviewRows.Children.Clear();
                var header = CreatePreviewGrid();
                for (var column = 0; column < PreviewColumns.Length; column++)
                {
                    var text = new TextBlock { Text = PreviewColumns[column].Label, FontSize = 12, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, TextWrapping = TextWrapping.NoWrap };
                    Grid.SetColumn(text, column); header.Children.Add(text);
                }
                LayoutPreviewHeader.Child = header;
                LayoutPreviewTable.Width = header.Width + 16;
                foreach (var field in fields) CreatePreviewRow(field);
            }
            for (var index = 0; index < fields.Count; index++) _previewRefresh[fields[index].Id](fields[index], index + 1);
            var selectedCount = _vm.Session?.SelectedIds.Count ?? 0;
            LayoutPreviewSummary.Text = _vm.Session == null ? "选择窗体后显示控件。" :
                SelectedOnlyToggle.IsChecked == true && selectedCount == 0 ? "请先在画布中选择控件。" :
                $"{fields.Count} 个控件 · 已选 {selectedCount}";
        }
        finally { _syncingPreview = false; }
    }

    private void CreatePreviewRow(FormEditorItem field)
    {
        // Each cell resolves the current draft by ID. Undo/save may replace every model instance.
        var id = field.Id;
        var row = CreatePreviewGrid();
        var rowBorder = new Border { BorderThickness = new Thickness(0, 0, 0, 1), Child = row };
        _previewRowBorders[id] = rowBorder;
        var refreshers = new List<Action<FormEditorItem, int>>();
        void Add(FrameworkElement control, int column)
        {
            control.VerticalAlignment = VerticalAlignment.Center;
            control.Margin = new Thickness(0, 0, 8, 0);
            Grid.SetColumn(control, column); row.Children.Add(control);
        }
        var number = new TextBlock { FontSize = 11, Width = 24, VerticalAlignment = VerticalAlignment.Center };
        Add(number, 0);
        refreshers.Add((_, sequence) => number.Text = sequence.ToString("00"));
        var label = new TextBlock { FontSize = 13, TextTrimming = TextTrimming.CharacterEllipsis, TextWrapping = TextWrapping.NoWrap };
        var select = new Button { Content = label, MinWidth = 0, Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch, Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent), BorderThickness = new Thickness(0) };
        select.Click += (_, _) => _vm.Select([id]);
        Add(select, 1);
        refreshers.Add((item, _) =>
        {
            var caption = item.Get("label", item.Name);
            label.Text = caption == item.Name ? item.Name : $"{caption} · {item.Name}";
            ToolTipService.SetToolTip(select, item.DisplayName);
            var selected = _vm.Session?.SelectedIds.Contains(id) == true;
            rowBorder.BorderBrush = (Brush)Application.Current.Resources[selected ? "AccentBrush" : "BorderBrush"];
            rowBorder.Background = selected ? (Brush)Application.Current.Resources["SurfaceBrush"] : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
            rowBorder.Visibility = SelectedOnlyToggle.IsChecked == true && !selected ? Visibility.Collapsed : Visibility.Visible;
        });
        for (var index = 2; index < PreviewColumns.Length; index++)
        {
            var key = PreviewColumns[index].Name;
            var caption = PreviewColumns[index].Label;
            if (key == "field_type")
            {
                var combo = new ComboBox { MinWidth = 0, HorizontalAlignment = HorizontalAlignment.Stretch };
                Add(combo, index);
                string signature = "";
                refreshers.Add((item, _) =>
                {
                    var metadata = _vm.Metadata;
                    var options = metadata == null ? [] : FormEditorRules.CompatibleTypes(metadata,
                        _vm.Session!.Document.Properties.FirstOrDefault(property => property.Id == item.Get("propertytype_id"))).ToList();
                    var current = item.Get(key);
                    if (options.All(option => option.Value != current)) options.Insert(0, new(current, current));
                    var nextSignature = string.Join("|", options.Select(option => option.DisplayText));
                    if (signature != nextSignature)
                    {
                        signature = nextSignature; combo.Items.Clear();
                        foreach (var option in options) combo.Items.Add(new ComboBoxItem { Content = option.Label, Tag = option.Value });
                    }
                    combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().FirstOrDefault(option => (string)option.Tag == current);
                    combo.IsEnabled = metadata?.Field.ContainsKey(key) == true;
                });
                combo.SelectionChanged += (_, _) => { if (!_syncingPreview && combo.SelectedItem is ComboBoxItem { Tag: string value }) _vm.SetFieldProperty(id, key, value); };
            }
            else if (key == "is_disabled")
            {
                var check = new CheckBox { MinWidth = 0, Width = 32, Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Left };
                AutomationProperties.SetName(check, $"{field.Name} 不可编辑"); Add(check, index);
                refreshers.Add((item, _) => { check.IsChecked = item.Get(key) == "1"; check.IsEnabled = _vm.Metadata?.Field.ContainsKey(key) == true; });
                check.Click += (_, _) => { if (!_syncingPreview) _vm.SetFieldProperty(id, key, check.IsChecked == true ? "1" : "0"); };
            }
            else
            {
                var box = new TextBox { MinWidth = 0, Padding = new Thickness(6), FontSize = 13, FontFamily = new FontFamily("Consolas") };
                AutomationProperties.SetName(box, $"{field.Name} {caption}");
                var current = "";
                void Commit()
                {
                    if (_syncingPreview || !box.IsEnabled || box.Text == current) return;
                    _vm.SetFieldProperty(id, key, string.IsNullOrWhiteSpace(box.Text) ? null : box.Text.Trim());
                }
                box.LostFocus += (_, _) => Commit();
                box.KeyDown += (_, args) => { if (args.Key == Windows.System.VirtualKey.Enter) { Commit(); args.Handled = true; } };
                var panel = new Grid();
                var dash = new TextBlock { Text = "—", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Collapsed };
                panel.Children.Add(box); panel.Children.Add(dash);
                refreshers.Add((item, _) =>
                {
                    current = item.Get(key);
                    if (box.Text != current) box.Text = current;
                    var applicable = _vm.Metadata?.Field.ContainsKey(key) == true &&
                        (key is not ("textarea_rows" or "textarea_cols") || ArasFormConfigurationOptions.SupportsTextAreaDimensions(item.Get("field_type")));
                    box.IsEnabled = applicable;
                    box.Visibility = applicable ? Visibility.Visible : Visibility.Collapsed;
                    dash.Visibility = applicable ? Visibility.Collapsed : Visibility.Visible;
                });
                if (key == "font_color")
                {
                    panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    var colors = new ComboBox { MinWidth = 0, PlaceholderText = "色板", HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(6, 0, 0, 0) };
                    foreach (var color in ArasFormConfigurationOptions.FontColors) colors.Items.Add(new ComboBoxItem { Content = color.Label, Tag = color.Value });
                    Grid.SetColumn(colors, 1); panel.Children.Add(colors);
                    refreshers.Add((item, _) =>
                    {
                        colors.IsEnabled = _vm.Metadata?.Field.ContainsKey(key) == true;
                        colors.SelectedItem = colors.Items.OfType<ComboBoxItem>().FirstOrDefault(option => string.Equals((string)option.Tag, item.Get(key), StringComparison.OrdinalIgnoreCase));
                    });
                    colors.SelectionChanged += (_, _) => { if (!_syncingPreview && colors.SelectedItem is ComboBoxItem { Tag: string color }) _vm.SetFieldProperty(id, key, color); };
                }
                Add(panel, index);
            }
        }
        LayoutPreviewRows.Children.Add(rowBorder);
        _previewRefresh[id] = (item, sequence) => { foreach (var refresh in refreshers) refresh(item, sequence); };
    }
}
