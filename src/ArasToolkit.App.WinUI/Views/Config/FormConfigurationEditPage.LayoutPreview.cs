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
        ("sequence", "#", 38), ("name", "字段", 180), ("field_type", "控件类型", 150),
        ("x", "X", 76), ("y", "Y", 76), ("display_length", "显示长度", 88),
        ("textarea_rows", "行尺寸", 82), ("textarea_cols", "列尺寸", 82),
        ("is_disabled", "不可编辑", 76), ("font_color", "标题颜色", 220)
    ];
    private readonly Dictionary<string, Action<FormEditorItem, int>> _previewRefresh = [];
    private string[] _previewIds = [];
    private bool _syncingPreview;

    private Grid CreatePreviewGrid()
    {
        var grid = new Grid { Padding = new Thickness(6), ColumnSpacing = 0 };
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
                LayoutPreviewRows.Children.Clear();
                var header = CreatePreviewGrid();
                header.Background = (Brush)Application.Current.Resources["SurfaceBrush"];
                for (var index = 0; index < PreviewColumns.Length; index++)
                {
                    var label = new TextBlock { Text = PreviewColumns[index].Label, FontSize = 12, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
                    Grid.SetColumn(label, index); header.Children.Add(label);
                }
                LayoutPreviewRows.Children.Add(header);
                foreach (var field in fields) CreatePreviewRow(field);
            }
            for (var index = 0; index < fields.Count; index++) _previewRefresh[fields[index].Id](fields[index], index + 1);
            LayoutPreviewSummary.Text = _vm.Session == null ? "选择窗体后显示当前主体的控件。" :
                $"当前主体 {fields.Count} 个控件 · 点击字段定位 · Text Area / FormattedText 支持行列尺寸 · 待保存 {_vm.PendingChangeCount} 项";
        }
        finally { _syncingPreview = false; }
    }

    private void CreatePreviewRow(FormEditorItem field)
    {
        // Each cell resolves the current draft by ID. Undo/save may replace every model instance.
        var id = field.Id;
        var row = CreatePreviewGrid();
        var refreshers = new List<Action<FormEditorItem, int>>();
        void Add(FrameworkElement control, int column)
        {
            control.Margin = new Thickness(2, 0, 6, 0);
            control.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(control, column); row.Children.Add(control);
        }
        var number = new TextBlock { FontSize = 12 };
        Add(number, 0); refreshers.Add((_, sequence) => number.Text = sequence.ToString());
        var label = new TextBlock { FontSize = 13, TextTrimming = TextTrimming.CharacterEllipsis };
        var name = new TextBlock { FontSize = 11, TextTrimming = TextTrimming.CharacterEllipsis };
        var title = new StackPanel(); title.Children.Add(label); title.Children.Add(name);
        var select = new Button { Content = title, MinWidth = 0, Padding = new Thickness(4), HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch, Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent), BorderThickness = new Thickness(0) };
        select.Click += (_, _) => _vm.Select([id]);
        Add(select, 1);
        refreshers.Add((item, _) =>
        {
            label.Text = item.Get("label", item.Name); name.Text = item.Name;
            ToolTipService.SetToolTip(select, item.DisplayName);
            row.Background = _vm.Session?.SelectedIds.Contains(id) == true ?
                (Brush)Application.Current.Resources["SurfaceBrush"] : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
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
                var check = new CheckBox { MinWidth = 0, Width = 32, Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Center };
                AutomationProperties.SetName(check, $"{field.Name} 不可编辑"); Add(check, index);
                refreshers.Add((item, _) => { check.IsChecked = item.Get(key) == "1"; check.IsEnabled = _vm.Metadata?.Field.ContainsKey(key) == true; });
                check.Click += (_, _) => { if (!_syncingPreview) _vm.SetFieldProperty(id, key, check.IsChecked == true ? "1" : "0"); };
            }
            else
            {
                var box = new TextBox { MinWidth = 0, Padding = new Thickness(6), FontSize = 13 };
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
                    panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(92) });
                    panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    var colors = new ComboBox { MinWidth = 0, PlaceholderText = "常用颜色", HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(6, 0, 0, 0) };
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
        LayoutPreviewRows.Children.Add(row);
        _previewRefresh[id] = (item, sequence) => { foreach (var refresh in refreshers) refresh(item, sequence); };
    }
}
