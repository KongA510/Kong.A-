using ArasToolkit.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class FormConfigurationEditPage
{
    private static readonly (string Name, string Label)[] PreviewColumns =
    [
        ("sequence", "#"), ("name", "字段"), ("field_type", "控件类型"),
        ("x", "X"), ("y", "Y"), ("display_length", "显示长度"),
        ("textarea_rows", "行尺寸"), ("textarea_cols", "列尺寸"),
        ("is_disabled", "不可编辑"), ("font_color", "标题颜色")
    ];
    private readonly Dictionary<string, Action<FormEditorItem, int>> _previewRefresh = [];
    private readonly Dictionary<string, Border> _previewCards = [];
    private string[] _previewIds = [];
    private bool _syncingPreview;

    private void RevealSelectedPreview()
    {
        if (EditorDock.Visibility != Visibility.Visible || LayoutPreviewPanel.Visibility != Visibility.Visible) return;
        var id = _vm.Session?.SelectedIds.FirstOrDefault();
        if (id != null && _previewCards.TryGetValue(id, out var card) && card.Visibility == Visibility.Visible)
            card.StartBringIntoView(new BringIntoViewOptions { AnimationDesired = false });
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
                _previewCards.Clear();
                LayoutPreviewRows.Children.Clear();
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
        var row = new Grid { RowSpacing = 8, ColumnSpacing = 8 };
        for (var i = 0; i < 4; i++) row.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (var i = 0; i < 3; i++) row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var card = new Border { Padding = new Thickness(10), CornerRadius = new CornerRadius(6), BorderThickness = new Thickness(1),
            Background = (Brush)Application.Current.Resources["SurfaceBrush"], Child = row };
        _previewCards[id] = card;
        var extraGrid = new Grid { ColumnSpacing = 8 };
        extraGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) });
        extraGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        extraGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var extra = new Expander { Header = "控件与行列尺寸", Content = extraGrid, MinWidth = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch };
        Grid.SetRow(extra, 3); Grid.SetColumnSpan(extra, 3); row.Children.Add(extra);
        var refreshers = new List<Action<FormEditorItem, int>>();
        void Add(FrameworkElement control, int column)
        {
            control.VerticalAlignment = VerticalAlignment.Center;
            if (column == 1) { Grid.SetColumnSpan(control, 3); row.Children.Add(control); return; }
            var cell = new StackPanel { Spacing = 3 };
            cell.Children.Add(new TextBlock { Text = PreviewColumns[column].Label, FontSize = 11, Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"] });
            cell.Children.Add(control);
            if (column is 2 or 6 or 7)
            {
                Grid.SetColumn(cell, column == 2 ? 0 : column - 5); extraGrid.Children.Add(cell);
            }
            else
            {
                Grid.SetRow(cell, column is 8 or 9 ? 2 : 1);
                Grid.SetColumn(cell, column == 9 ? 0 : column == 8 ? 2 : column - 3);
                if (column == 9) Grid.SetColumnSpan(cell, 2);
                row.Children.Add(cell);
            }
        }
        var number = new TextBlock { FontSize = 11, Width = 24, VerticalAlignment = VerticalAlignment.Center };
        refreshers.Add((_, sequence) => number.Text = sequence.ToString("00"));
        var label = new TextBlock { FontSize = 13, TextTrimming = TextTrimming.CharacterEllipsis };
        var name = new TextBlock { FontSize = 11, TextTrimming = TextTrimming.CharacterEllipsis };
        var title = new StackPanel(); title.Children.Add(label); title.Children.Add(name);
        var heading = new Grid(); heading.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); heading.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        heading.Children.Add(number); Grid.SetColumn(title, 1); heading.Children.Add(title);
        var select = new Button { Content = heading, MinWidth = 0, Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch, Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent), BorderThickness = new Thickness(0) };
        select.Click += (_, _) => _vm.Select([id]);
        Add(select, 1);
        refreshers.Add((item, _) =>
        {
            label.Text = item.Get("label", item.Name); name.Text = item.Name;
            ToolTipService.SetToolTip(select, item.DisplayName);
            var selected = _vm.Session?.SelectedIds.Contains(id) == true;
            card.BorderBrush = (Brush)Application.Current.Resources[selected ? "AccentBrush" : "BorderBrush"];
            card.Visibility = SelectedOnlyToggle.IsChecked == true && !selected ? Visibility.Collapsed : Visibility.Visible;
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
        LayoutPreviewRows.Children.Add(card);
        _previewRefresh[id] = (item, sequence) => { foreach (var refresh in refreshers) refresh(item, sequence); };
    }
}
