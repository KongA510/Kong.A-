using System.Globalization;
using System.Text.Json;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Windows.ApplicationModel.DataTransfer;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class FormConfigurationEditPage : Page
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly FormConfigurationEditViewModel _vm;
    private readonly IFormConfigurationEditService _service;
    private readonly List<Stream> _resourceStreams = [];
    private readonly Dictionary<string, FormEditorResource?> _resourceCache = [];
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Dictionary<string, (double X, double Y)> _insertionContainers = [];
    private string _insertionContainer = "";
    private readonly Dictionary<string, TextBox> _coordinateBoxes = [];
    private bool _syncing, _changing, _ready, _disposed, _allowNavigation, _allowClose, _dialogOpen;
    private long _revision;
    private bool _renderPending;
    private const string ShellOrigin = "https://form-editor.local";
    private string? _pendingTool;
    private FormEditorProperty? _pendingProperty;
    private Microsoft.UI.Windowing.AppWindow? _hostWindow;
    private Window? _hostXamlWindow;
    private Frame? _hostFrame;
    private CoreWebView2? _configuredCore;
    private TaskCompletionSource<bool>? _canvasReady;
    private bool _initializingCanvas;

    public FormConfigurationEditPage()
    {
        InitializeComponent();
        _vm = App.Services.GetRequiredService<FormConfigurationEditViewModel>();
        _service = App.Services.GetRequiredService<IFormConfigurationEditService>();
        DataContext = _vm;
        _vm.EditorChanged += OnEditorChanged;
        Loaded += Page_Loaded; Unloaded += Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= Page_Loaded;
        _hostFrame = Frame;
        _hostXamlWindow = App.MainWindow;
        _hostWindow = _hostXamlWindow?.AppWindow;
        if (_hostXamlWindow != null) _hostXamlWindow.Closed += HostWindow_Closed;
        if (_hostFrame != null) _hostFrame.Navigating += Frame_Navigating;
        if (_hostWindow != null) _hostWindow.Closing += Window_Closing;
        await Task.WhenAll(InitializeCanvasAsync(), _vm.InitializeAsync());
        if (!_disposed) RenderInspector();
    }

    private async Task InitializeCanvasAsync()
    {
        if (_disposed || _initializingCanvas) return;
        _initializingCanvas = true;
        _ready = false;
        var token = _lifetime.Token;
        CanvasBrowser.Visibility = Visibility.Visible;
        CanvasBrowser.IsHitTestVisible = false;
        CanvasStatusPanel.Visibility = Visibility.Visible;
        CanvasStatusTitle.Text = "正在加载画布";
        CanvasStatusMessage.Text = "正在初始化窗体预览组件…";
        CanvasLoading.IsActive = true;
        CanvasRetry.Visibility = Visibility.Collapsed;
        try
        {
            var assets = Path.Combine(AppContext.BaseDirectory, "Assets", "FormEditor");
            foreach (var file in new[] { "index.html", "editor.js", "editor.css" })
                if (!File.Exists(Path.Combine(assets, file))) throw new FileNotFoundException($"缺少画布文件：Assets/FormEditor/{file}");
            await CanvasBrowser.EnsureCoreWebView2Async().AsTask().WaitAsync(TimeSpan.FromSeconds(30), token);
            if (_disposed) return;
            var core = CanvasBrowser.CoreWebView2;
            core.SetVirtualHostNameToFolderMapping("form-editor.local", assets, CoreWebView2HostResourceAccessKind.DenyCors);
            if (_configuredCore != core)
            {
                _configuredCore = core;
                core.Settings.AreDevToolsEnabled = false; core.Settings.AreDefaultContextMenusEnabled = false;
                core.Settings.IsStatusBarEnabled = false; core.Settings.IsZoomControlEnabled = false;
                core.WebMessageReceived += Browser_Message;
                core.NavigationStarting += (_, args) => { if (!args.Uri.StartsWith(ShellOrigin + "/", StringComparison.OrdinalIgnoreCase)) args.Cancel = true; };
                core.NavigationCompleted += (_, args) =>
                {
                    if (!args.IsSuccess || args.HttpStatusCode >= 400)
                        _canvasReady?.TrySetException(new InvalidOperationException($"画布页面加载失败：{args.WebErrorStatus}（HTTP {args.HttpStatusCode}）"));
                };
                core.ProcessFailed += async (_, args) =>
                {
                    if (_disposed) return;
                    var error = new InvalidOperationException($"画布预览进程异常：{args.ProcessFailedKind}");
                    if (_initializingCanvas) _canvasReady?.TrySetException(error);
                    else await ReportCanvasErrorAsync(error);
                };
                core.NewWindowRequested += (_, args) => args.Handled = true;
                core.PermissionRequested += (_, args) => args.State = CoreWebView2PermissionState.Deny;
                core.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                core.WebResourceRequested += Browser_ResourceRequested;
            }
            _canvasReady = new(TaskCreationOptions.RunContinuationsAsynchronously);
            core.Navigate(ShellOrigin + "/index.html");
            await _canvasReady.Task.WaitAsync(TimeSpan.FromSeconds(15), token);
            if (_disposed) return;
            _ready = true;
            CanvasLoading.IsActive = false;
            CanvasStatusPanel.Visibility = Visibility.Collapsed;
            CanvasBrowser.IsHitTestVisible = true;
            QueueCanvas();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (TimeoutException ex)
        {
            if (!_disposed) await ReportCanvasErrorAsync(new TimeoutException("画布加载超时，预览组件未能完成初始化。请重试加载。", ex));
        }
        catch (Exception ex) { if (!_disposed) await ReportCanvasErrorAsync(ex); }
        finally { _initializingCanvas = false; }
    }

    private async Task ReportCanvasErrorAsync(Exception error, string area = "初始化画布")
    {
        _ready = false;
        CanvasBrowser.Visibility = Visibility.Collapsed;
        CanvasLoading.IsActive = false;
        CanvasStatusPanel.Visibility = Visibility.Visible;
        CanvasStatusTitle.Text = "画布加载失败";
        CanvasStatusMessage.Text = $"{error.Message}\n可重试加载画布；已读取的配置和未保存修改会保留。若持续失败，请确认使用完整安装包及与程序架构一致的 WebView2 组件。";
        CanvasRetry.Visibility = Visibility.Visible;
        // Canvas failures stay in their own panel; generic data operations must not clear them.
        await _vm.ReportErrorAsync(area, error, showError: false);
    }

    private async void CanvasRetry_Click(object sender, RoutedEventArgs e) => await InitializeCanvasAsync();

    private void Page_Unloaded(object sender, RoutedEventArgs e) => DisposeEditor();
    private void HostWindow_Closed(object sender, WindowEventArgs e) => DisposeEditor();

    private void DisposeEditor()
    {
        if (_disposed) return; _disposed = true;
        if (_hostXamlWindow != null) _hostXamlWindow.Closed -= HostWindow_Closed;
        if (_hostFrame != null) _hostFrame.Navigating -= Frame_Navigating;
        if (_hostWindow != null) _hostWindow.Closing -= Window_Closing;
        _vm.EditorChanged -= OnEditorChanged; _vm.Dispose();
        _lifetime.Cancel();
        CanvasBrowser.Close();
        foreach (var stream in _resourceStreams) stream.Dispose();
        _resourceStreams.Clear();
        _resourceCache.Clear(); _lifetime.Dispose();
    }

    private async void Browser_ResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
        var uri = new Uri(args.Request.Uri);
        if (uri.Host == "form-editor.local") return;
        using var deferral = args.GetDeferral();
        try
        {
            FormEditorResource? resource = null;
            if (uri.Host == "form-resources.local" && _vm.Session != null && uri.AbsolutePath.StartsWith("/Client/", StringComparison.OrdinalIgnoreCase) && args.Request.Method == "GET")
            {
                var cacheKey = _vm.Session.Document.ConnectionKey + "|" + uri.AbsoluteUri;
                if (!_resourceCache.TryGetValue(cacheKey, out resource))
                {
                    resource = await _service.GetPreviewResourceAsync(uri.AbsolutePath[8..] + uri.Query, _vm.Session.Document.ConnectionKey, _lifetime.Token);
                    if (_disposed) return;
                    _resourceCache[cacheKey] = resource;
                }
            }
            var stream = new MemoryStream(resource?.Content ?? []); _resourceStreams.Add(stream);
            args.Response = sender.Environment.CreateWebResourceResponse(stream.AsRandomAccessStream(), resource == null ? 404 : 200,
                resource == null ? "Not available" : "OK", $"Content-Type: {resource?.ContentType ?? "text/plain"}\r\nCache-Control: no-store\r\nX-Content-Type-Options: nosniff");
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { if (!_disposed) await _vm.ReportErrorAsync("读取预览资源", ex); }
    }

    private async void Browser_Message(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        if (_disposed || !args.Source.StartsWith(ShellOrigin + "/", StringComparison.OrdinalIgnoreCase)) return;
        try
        {
            using var document = JsonDocument.Parse(args.WebMessageAsJson);
            var message = document.RootElement; var kind = message.GetProperty("kind").GetString();
            if (kind == "ready") { _canvasReady?.TrySetResult(true); return; }
            if (_vm.Session == null || !message.TryGetProperty("revision", out var revision) || revision.GetInt64() != _revision) return;
            switch (kind)
            {
                case "selection":
                    _vm.Select(message.GetProperty("ids").EnumerateArray().Select(id => id.GetString() ?? "")); break;
                case "dragStart": _vm.BeginDrag(); break;
                case "move":
                    var points = message.GetProperty("points").EnumerateArray().ToDictionary(point => point.GetProperty("id").GetString()!,
                        point => (point.GetProperty("x").GetInt32(), point.GetProperty("y").GetInt32()));
                    _vm.Move(points); break;
                case "dragEnd": _vm.EndDrag(message.GetProperty("cancel").GetBoolean()); break;
                case "delete": await DeleteAsync(); break;
                case "undo": _vm.Undo(); break;
                case "redo": _vm.Redo(); break;
                case "insertPoint":
                    ReadInsertionContainers(message);
                    await AddDialogAsync(_pendingProperty, _pendingTool, message.GetProperty("x").GetInt32(), message.GetProperty("y").GetInt32()); break;
                case "addDrop":
                    ReadInsertionContainers(message);
                    var propertyId = message.TryGetProperty("propertyId", out var prop) ? prop.GetString() : "";
                    var property = _vm.Session.Document.Properties.FirstOrDefault(item => item.Id == propertyId);
                    var type = message.TryGetProperty("fieldType", out var typeValue) ? typeValue.GetString() : null;
                    await AddDialogAsync(property, type, message.GetProperty("x").GetInt32(), message.GetProperty("y").GetInt32()); break;
            }
        }
        catch (Exception ex) { await _vm.ReportErrorAsync("画布交互", ex); }
    }
    private void ReadInsertionContainers(JsonElement message)
    {
        _insertionContainers.Clear();
        _insertionContainer = message.TryGetProperty("containerName", out var name) ? name.GetString() ?? "" : "";
        if (!message.TryGetProperty("containers", out var containers)) return;
        foreach (var container in containers.EnumerateArray())
            _insertionContainers[container.GetProperty("name").GetString()!] = (container.GetProperty("x").GetDouble(), container.GetProperty("y").GetDouble());
    }

    private void OnEditorChanged(string kind)
    {
        if (_disposed) return;
        SyncLayoutPreview();
        switch (kind)
        {
            case "sources": SyncPickers(); break;
            case "coordinates": UpdateCoordinateBoxes(); break;
            case "selection": SyncFieldSelection(); RenderInspector(); SendSelection(); RevealSelectedPreview(); break;
            case "zoom": _ = ScriptAsync($"window.formEditor.setZoom({_vm.Zoom.ToString(CultureInfo.InvariantCulture)})"); break;
            default: SyncPickers(); RenderFieldList(); RenderInspector(); QueueCanvas(); break;
        }
    }
    private void SyncPickers()
    {
        _syncing = true;
        try
        {
            FormPicker.SelectedItem = _vm.SelectedForm;
            BodyPicker.Items.Clear();
            if (_vm.Session != null)
                foreach (var body in _vm.Session.Document.Bodies)
                    BodyPicker.Items.Add(new ComboBoxItem { Content = $"Body · {body.Id[..8]}", Tag = body.Id });
            BodyPicker.SelectedItem = BodyPicker.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _vm.Session?.BodyId);
            if (BodyPicker.Items.Count == 0) BodyPicker.PlaceholderText = "保存新增控件时创建";
            PropertyList.ItemsSource = _vm.Session?.Document.Properties;
            string[] toolbox = ["label", "button", "html", "image", "groupbox", "nested form"];
            ToolboxList.ItemsSource = _vm.Metadata?.FieldTypes.Where(option => toolbox.Contains(option.Value)).ToList();
        }
        finally { _syncing = false; }
    }
    private void RenderFieldList()
    {
        var query = FieldSearch.Text?.Trim() ?? "";
        _syncing = true;
        FieldList.ItemsSource = _vm.Session?.Fields.Where(field => field.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        _syncing = false; SyncFieldSelection();
    }
    private void SyncFieldSelection()
    {
        _syncing = true;
        FieldList.SelectedItems.Clear();
        foreach (var field in FieldList.Items.OfType<FormEditorItem>())
            if (_vm.Session?.SelectedIds.Contains(field.Id) == true) FieldList.SelectedItems.Add(field);
        _syncing = false;
    }
    private void QueueCanvas()
    {
        if (!_ready || _renderPending || _disposed) return;
        _renderPending = true;
        DispatcherQueue.TryEnqueue(async () =>
        {
            _renderPending = false;
            if (_disposed) return;
            if (_vm.Session == null) { await ScriptAsync("window.formEditor.clear()"); return; }
            _revision++;
            var state = BuildCanvasState(_vm.Session.Document, _vm.Session.Body, true);
            state["revision"] = _revision;
            state["zoom"] = _vm.Zoom; state["selected"] = _vm.Session.SelectedIds;
            state["editable"] = _vm.CanEdit; state["responsive"] = _vm.Session.Document.IsResponsive;
            state["containerProperty"] = _vm.Metadata?.ContainerProperty;
            state["nestedFormProperty"] = _vm.Metadata?.NestedFormProperty;
            state["nestedPreviewIds"] = _vm.NestedPreviewIds;
            state["imageProperty"] = _vm.Metadata?.ImageProperty;
            state["serverUrl"] = App.Services.GetRequiredService<IArasConnectionService>().CurrentConnection?.Url;
            state["nestedForms"] = _vm.NestedForms.ToDictionary(pair => pair.Key, pair => BuildCanvasState(pair.Value, pair.Value.Bodies.FirstOrDefault(), false));
            await ScriptAsync($"window.formEditor.setState({JsonSerializer.Serialize(state, JsonOptions)})");
        });
    }
    private static Dictionary<string, object?> BuildCanvasState(FormEditorDefinition document, FormEditorItem? body, bool editable) => new()
    {
        ["formId"] = document.Form.Id, ["form"] = Values(document.Form), ["body"] = body == null ? new Dictionary<string, string?>() : Values(body),
        ["responsive"] = document.IsResponsive,
        ["fields"] = document.IsResponsive ? [] : body?.Children.Where(field => !field.IsDeleted).Select(field => new { field.Id, Values = Values(field) }).ToArray() ?? [],
        ["properties"] = document.Properties.Select(property => new { property.Id, property.Name, property.Label, property.DefaultValue, Options = property.Values })
    };
    private static Dictionary<string, string?> Values(FormEditorItem item)
    {
        var values = new Dictionary<string, string?>(item.Original);
        foreach (var pair in item.Changes) values[pair.Key] = pair.Value;
        return values;
    }
    private async Task ScriptAsync(string script)
    {
        if (!_ready || _disposed) return;
        try
        {
            // ExecuteScriptAsync otherwise returns null for both success and JavaScript exceptions.
            var result = await CanvasBrowser.CoreWebView2.ExecuteScriptAsync($"(() => {{ try {{ {script}; return null; }} catch (error) {{ return String(error.stack || error); }} }})()");
            if (result != "null") throw new InvalidOperationException($"画布渲染失败：{JsonSerializer.Deserialize<string>(result)}");
        }
        catch (Exception ex) { if (!_disposed) await ReportCanvasErrorAsync(ex, "更新画布"); }
    }
    private void SendSelection() => _ = ScriptAsync($"window.formEditor.setSelection({JsonSerializer.Serialize(_vm.Session?.SelectedIds ?? [])})");

    private static readonly Dictionary<string, string> Labels = new()
    {
        ["name"] = "控件名称", ["label"] = "标签", ["field_type"] = "控件类型", ["propertytype_id"] = "绑定属性 ID",
        ["x"] = "X 坐标", ["y"] = "Y 坐标", ["positioning"] = "定位方式", ["display_length"] = "显示长度",
        ["display_length_unit"] = "长度单位", ["width"] = "宽度（px）", ["height"] = "高度（px）",
        ["textarea_rows"] = "行尺寸（px）", ["textarea_cols"] = "列尺寸（px）", ["z_index"] = "层级 Z", ["tab_index"] = "Tab 顺序", ["tab_stop"] = "参与 Tab 导航",
        ["is_visible"] = "可见", ["is_disabled"] = "不可编辑", ["label_position"] = "标签位置", ["text_align"] = "文字对齐",
        ["font_family"] = "字体", ["font_size"] = "字号", ["font_color"] = "标题颜色", ["font_weight"] = "字体粗细", ["font_style"] = "字体样式",
        ["html_code"] = "HTML 内容", ["legend"] = "边框标题", ["border_width"] = "边框宽度", ["border_style"] = "边框样式", ["border_color"] = "边框颜色", ["background_color"] = "背景颜色", ["bg_color"] = "背景颜色"
    };
    private void RenderInspector()
    {
        var wasSyncing = _syncing;
        _syncing = true;
        try
        {
        Inspector.Children.Clear(); _coordinateBoxes.Clear();
        if (_vm.Session == null || _vm.Metadata == null)
        { Inspector.Children.Add(new TextBlock { Text = "选择窗体后，在此修改控件或窗体属性。", TextWrapping = TextWrapping.Wrap }); return; }
        var selected = _vm.Session.Selected.ToList(); var isForm = selected.Count == 0;
        var targets = isForm ? new List<FormEditorItem> { _vm.Session.Document.Form } : selected;
        Inspector.Children.Add(new TextBlock { Text = isForm ? "窗体属性" : selected.Count == 1 ? selected[0].Name : $"已选 {selected.Count} 个控件", FontSize = 17, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, TextWrapping = TextWrapping.Wrap });
        var descriptors = isForm ? _vm.Metadata.Form : _vm.Metadata.Field;
        var groups = isForm
            ? new[] { ("窗体尺寸", new[] { "label", "width", "height" }) }
            : new[]
            {
                ("控件", new[] { "name", "field_type", "label", "propertytype_id" }),
                ("位置与尺寸", new[] { "x", "y", "positioning", "display_length", "display_length_unit", "width", "height", "textarea_rows", "textarea_cols", "z_index", "tab_index", "tab_stop" }),
                ("标签与外观", new[] { "is_visible", "is_disabled", "label_position", "text_align", "font_family", "font_size", "font_color", "font_weight", "font_style", "bg_color", "background_color" }),
                ("类型专属设置", new[] { "html_code", _vm.Metadata.ImageProperty, _vm.Metadata.NestedFormProperty, _vm.Metadata.ContainerProperty, "legend", "border_width", "border_style", "border_color" })
            };
        foreach (var (title, names) in groups)
        {
            var controls = new StackPanel { Spacing = 10 };
            foreach (var name in names.Distinct())
            {
                if (name == "" || !descriptors.TryGetValue(name, out var descriptor) || !Applicable(name, targets, isForm)) continue;
                controls.Children.Add(CreateEditor(descriptor, targets, isForm));
            }
            if (controls.Children.Count == 0) continue;
            Inspector.Children.Add(new TextBlock { Text = title, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Margin = new Thickness(0, 10, 0, 0) });
            Inspector.Children.Add(controls);
        }
        if (selected.Count == 1 && selected[0].Get("field_type") == "nested form" && _vm.Metadata.NestedFormProperty == "")
        {
            var field = selected[0];
            var picker = new ComboBox { Header = "嵌套窗体预览（不写入配置）", HorizontalAlignment = HorizontalAlignment.Stretch };
            foreach (var form in _vm.NestedPreviewOptions.GetValueOrDefault(field.Id) ?? [])
                picker.Items.Add(new ComboBoxItem { Content = form.DisplayName, Tag = form.Id });
            picker.SelectedItem = picker.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _vm.NestedPreviewIds.GetValueOrDefault(field.Id));
            picker.SelectionChanged += async (_, _) => { if (!_syncing && picker.SelectedItem is ComboBoxItem { Tag: string id }) await _vm.RefreshNestedPreviewAsync(field.Id, id); };
            Inspector.Children.Add(picker);
            Inspector.Children.Add(new TextBlock { Text = "此系统的嵌套窗体由绑定 Item 属性及运行时 View 决定。这里可切换关联窗体预览。", FontSize = 11, TextWrapping = TextWrapping.Wrap });
        }
        Inspector.Children.Add(new TextBlock { Text = "事件与高级 CSS 保留原值。不存在于目标系统元数据的配置不会显示或写入。", FontSize = 11, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 12, 0, 0) });
        }
        finally { _syncing = wasSyncing; }
    }
    private bool Applicable(string name, List<FormEditorItem> targets, bool isForm)
    {
        if (isForm) return true;
        if (name is "name" or "propertytype_id") return targets.Count == 1;
        if (name is "textarea_rows" or "textarea_cols") return targets.All(item => ArasFormConfigurationOptions.SupportsTextAreaDimensions(item.Get("field_type")));
        if (name == "html_code") return targets.All(item => item.Get("field_type") == "html");
        if (name == _vm.Metadata!.ImageProperty) return targets.All(item => item.Get("field_type") == "image");
        if (name == _vm.Metadata.NestedFormProperty) return targets.All(item => item.Get("field_type") == "nested form");
        return true;
    }
    private FrameworkElement CreateEditor(FormEditorPropertyDescriptor descriptor, List<FormEditorItem> targets, bool isForm)
    {
        var name = descriptor.Name;
        var values = targets.Select(item => item.Get(name)).Distinct().ToList();
        var mixed = values.Count != 1; var current = mixed ? "" : values[0];
        var label = Labels.GetValueOrDefault(name, string.IsNullOrEmpty(descriptor.Label) ? name : descriptor.Label);
        var enabled = _vm.CanEdit && !(name == "name" && targets.Any(item => !item.IsNew)) && name != "propertytype_id";
        var options = descriptor.Options.ToList();
        if (name == "field_type")
            options = _vm.Metadata!.FieldTypes.Where(option => targets.All(item =>
                FormEditorRules.CompatibleTypes(_vm.Metadata, _vm.Session!.Document.Properties.FirstOrDefault(property => property.Id == item.Get("propertytype_id")))
                    .Any(allowed => allowed.Value == option.Value))).ToList();
        if (name == _vm.Metadata!.NestedFormProperty)
            options = _vm.Metadata.AvailableForms.Where(form => form.Id != _vm.Session!.Document.Form.Id && !form.Classification.Contains("responsive", StringComparison.OrdinalIgnoreCase)).Select(form => new ArasFormOption(form.Name, form.Id)).ToList();
        if (name == _vm.Metadata.ContainerProperty)
            options = new[] { new ArasFormOption("窗体根部", "") }.Concat(_vm.Session!.Fields.Where(item => item.Get("field_type") == "groupbox" && !targets.Contains(item)).Select(item => new ArasFormOption(item.Name, item.Name))).ToList();
        if (descriptor.DataType == "boolean") options = [new("是", "1"), new("否", "0")];
        if (name == "font_weight" && options.Count == 0) options = [new("正常", "normal"), new("粗体", "bold")];
        if (options.Count > 0)
        {
            if (!mixed && options.All(option => option.Value != current)) options.Insert(0, new($"原值：{current}", current));
            var combo = new ComboBox { Header = label, HorizontalAlignment = HorizontalAlignment.Stretch, IsEnabled = enabled, PlaceholderText = mixed ? "多个值" : "未设置" };
            foreach (var option in options) combo.Items.Add(new ComboBoxItem { Content = option.Label, Tag = option.Value });
            combo.SelectedItem = mixed ? null : combo.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == current);
            combo.SelectionChanged += (_, _) => { if (!_syncing && combo.SelectedItem is ComboBoxItem item) _vm.SetProperty(name, (string)item.Tag); };
            return combo;
        }
        var panel = new StackPanel { Spacing = 5 };
        var box = new TextBox { Header = label, Text = current, PlaceholderText = mixed ? "多个值" : "未设置", IsEnabled = enabled, AcceptsReturn = name == "html_code", TextWrapping = name == "html_code" ? TextWrapping.Wrap : TextWrapping.NoWrap, MaxHeight = name == "html_code" ? 220 : double.PositiveInfinity };
        if (name is "x" or "y") _coordinateBoxes[name] = box;
        void Commit()
        {
            if (_syncing || !enabled || box.Text == current || (mixed && box.Text == "")) return;
            _vm.SetProperty(name, string.IsNullOrEmpty(box.Text) ? null : box.Text);
        }
        box.LostFocus += (_, _) => Commit();
        box.KeyDown += (_, args) => { if (args.Key == Windows.System.VirtualKey.Enter && name != "html_code") { Commit(); args.Handled = true; } };
        panel.Children.Add(box);
        if (name == "font_color")
        {
            var colors = new ComboBox { PlaceholderText = "选择常用颜色", HorizontalAlignment = HorizontalAlignment.Stretch, IsEnabled = enabled };
            foreach (var color in ArasFormConfigurationOptions.FontColors) colors.Items.Add(new ComboBoxItem { Content = color.DisplayText, Tag = color.Value });
            colors.SelectionChanged += (_, _) => { if (colors.SelectedItem is ComboBoxItem color) _vm.SetProperty(name, (string)color.Tag); };
            panel.Children.Add(colors);
        }
        return panel;
    }
    private void UpdateCoordinateBoxes()
    {
        _syncing = true;
        foreach (var (name, box) in _coordinateBoxes)
        {
            var values = _vm.Session!.Selected.Select(item => item.Get(name)).Distinct().ToList();
            box.Text = values.Count == 1 ? values[0] : "";
        }
        _syncing = false;
    }

    private async Task<bool> ConfirmLeaveAsync()
    {
        if (_vm.IsBusy || _dialogOpen) return false;
        if (!_vm.HasChanges) return true;
        _dialogOpen = true;
        try
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot, Title = "有未保存的窗体修改", Content = "保存后继续，或放弃当前草稿。取消会留在编辑器中。",
                PrimaryButtonText = "保存", SecondaryButtonText = "放弃", CloseButtonText = "取消", IsPrimaryButtonEnabled = _vm.CanSave,
                DefaultButton = ContentDialogButton.Close
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return await _vm.SaveAsync();
            if (result == ContentDialogResult.Secondary) { _vm.DiscardChanges(); return true; }
            return false;
        }
        finally { _dialogOpen = false; }
    }
    private async Task GuardedAsync(Func<Task> action)
    {
        if (_changing) return;
        _changing = true;
        try { if (await ConfirmLeaveAsync()) await action(); }
        catch (Exception ex) { await _vm.ReportErrorAsync("切换", ex); }
        finally { _changing = false; SyncPickers(); }
    }
    private async void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
        if (_allowNavigation || (!_vm.HasChanges && !_vm.IsBusy)) return;
        e.Cancel = true;
        var target = e.SourcePageType; var parameter = e.Parameter; var mode = e.NavigationMode;
        await GuardedAsync(() =>
        {
            _allowNavigation = true;
            try { if (mode == NavigationMode.Back) Frame.GoBack(); else if (mode == NavigationMode.Forward) Frame.GoForward(); else Frame.Navigate(target, parameter); }
            finally { _allowNavigation = false; }
            return Task.CompletedTask;
        });
    }
    private async void Window_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        if (_allowClose || (!_vm.HasChanges && !_vm.IsBusy)) return;
        args.Cancel = true;
        await GuardedAsync(() => { _allowClose = true; App.MainWindow?.Close(); return Task.CompletedTask; });
    }
    private void ItemTypeSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        sender.ItemsSource = _vm.ItemTypes.Where(item => item.DisplayName.Contains(sender.Text.Trim(), StringComparison.OrdinalIgnoreCase)).Take(100).ToList();
    }
    private async void ItemTypeSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var item = args.ChosenSuggestion as ArasItemTypeInfo ?? _vm.ItemTypes.FirstOrDefault(item => item.DisplayName.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase));
        if (item != null) await GuardedAsync(async () => { await _vm.SelectItemTypeAsync(item); sender.Text = item.DisplayName; });
    }
    private async void FormPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_syncing && FormPicker.SelectedItem is FormEditorSummary form && form.Id != _vm.SelectedForm?.Id)
            await GuardedAsync(() => _vm.SelectFormAsync(form));
    }
    private async void BodyPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_syncing && BodyPicker.SelectedItem is ComboBoxItem { Tag: string id } && id != _vm.Session?.BodyId)
            await GuardedAsync(() => { _vm.SwitchBody(id); return Task.CompletedTask; });
    }
    private async void Refresh_Click(object sender, RoutedEventArgs e) => await GuardedAsync(async () =>
    {
        _resourceCache.Clear();
        if (_vm.SelectedForm != null && _vm.ConnectionMatches) await _vm.SelectFormAsync(_vm.SelectedForm);
        else { await _vm.InitializeAsync(); if (_vm.SelectedItemType != null) await _vm.SelectItemTypeAsync(_vm.SelectedItemType); }
    });
    private void FieldSearch_TextChanged(object sender, TextChangedEventArgs e) { if (_vm != null) RenderFieldList(); }
    private void FieldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_syncing || _vm.Session == null) return;
        var ids = _vm.Session.SelectedIds.Except(e.RemovedItems.OfType<FormEditorItem>().Select(item => item.Id)).ToList();
        ids.AddRange(e.AddedItems.OfType<FormEditorItem>().Select(item => item.Id)); _vm.Select(ids);
    }
    private void Undo_Click(object sender, RoutedEventArgs e) => _vm.Undo();
    private void Redo_Click(object sender, RoutedEventArgs e) => _vm.Redo();
    private void Align_Click(object sender, RoutedEventArgs e) => _vm.Align();
    private async void Save_Click(object sender, RoutedEventArgs e) => await _vm.SaveAsync();
    private async void Delete_Click(object sender, RoutedEventArgs e) => await DeleteAsync();
    private async Task DeleteAsync()
    {
        if (!_vm.CanDelete || _dialogOpen) return;
        _dialogOpen = true;
        try
        {
            var probe = new FormEditorSession(_vm.Session!.Document.Clone(), _vm.Metadata!) { BodyId = _vm.Session.BodyId };
            probe.Select(_vm.Session.SelectedIds); var all = probe.DeleteSelection(true);
            var hasChildren = all.Count > _vm.Session.SelectedIds.Count;
            var dialog = new ContentDialog { XamlRoot = XamlRoot, Title = hasChildren ? "删除控件及其子控件" : "删除所选控件",
                Content = new ScrollViewer { MaxHeight = 300, Content = new TextBlock { Text = string.Join("\n", all.Select(item => item.Name)), TextWrapping = TextWrapping.Wrap } },
                PrimaryButtonText = hasChildren ? "整组删除" : "删除", CloseButtonText = "取消", DefaultButton = ContentDialogButton.Close };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary) _vm.Delete(hasChildren);
        }
        catch (Exception ex) { await _vm.ReportErrorAsync("删除控件", ex); }
        finally { _dialogOpen = false; }
    }
    private async void Changes_Click(object sender, RoutedEventArgs e)
    {
        if (_dialogOpen) return; _dialogOpen = true;
        try
        {
            var text = _vm.ChangesSummary();
            await new ContentDialog { XamlRoot = XamlRoot, Title = "待保存变更", CloseButtonText = "关闭",
                Content = new ScrollViewer { MaxHeight = 480, Content = new TextBox { Text = string.IsNullOrEmpty(text) ? "没有待保存修改。" : text, IsReadOnly = true, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinWidth = 420 } } }.ShowAsync();
        }
        catch (Exception ex) { await _vm.ReportErrorAsync("查看变更", ex); }
        finally { _dialogOpen = false; }
    }
    private void ZoomPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_vm != null && ZoomPicker.SelectedItem is ComboBoxItem item && double.TryParse(item.Content.ToString()?.TrimEnd('%'), out var value)) _vm.Zoom = value;
    }
    private bool _compactWorkspace;
    private void ToolsToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_compactWorkspace && ToolsToggle.IsChecked == true) { EditorToggle.IsChecked = false; SetEditorVisible(false); }
        SetToolsVisible(ToolsToggle.IsChecked == true);
    }
    private void SetToolsVisible(bool visible)
    {
        if (Workspace == null) return;
        ToolsPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        Workspace.ColumnDefinitions[0].Width = new GridLength(visible ? 225 : 0);
    }
    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ToolsToggle == null || EditorToggle == null) return;
        var compact = e.NewSize.Width < 1100;
        if (compact && !_compactWorkspace) { ToolsToggle.IsChecked = false; SetToolsVisible(false); }
        _compactWorkspace = compact;
        SetEditorVisible(EditorToggle.IsChecked == true);
    }
    private void EditorToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_compactWorkspace && EditorToggle.IsChecked == true) { ToolsToggle.IsChecked = false; SetToolsVisible(false); }
        SetEditorVisible(EditorToggle.IsChecked == true);
    }
    private void SetEditorVisible(bool visible)
    {
        EditorDock.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        Workspace.ColumnDefinitions[2].Width = new GridLength(visible ? Math.Clamp(ActualWidth * .33, 340, 440) : 0);
    }
    private void LayoutMode_Click(object sender, RoutedEventArgs e) => SetInspectorMode(false);
    private void InspectorMode_Click(object sender, RoutedEventArgs e) => SetInspectorMode(true);
    private void SetInspectorMode(bool inspector)
    {
        LayoutModeButton.IsChecked = !inspector; InspectorModeButton.IsChecked = inspector;
        LayoutPreviewPanel.Visibility = inspector ? Visibility.Collapsed : Visibility.Visible;
        InspectorScroll.Visibility = inspector ? Visibility.Visible : Visibility.Collapsed;
        if (!inspector) RevealSelectedPreview();
    }
    private void SelectedOnly_Click(object sender, RoutedEventArgs e) => SyncLayoutPreview();
    private async void Add_Click(object sender, RoutedEventArgs e) => await RequestAddAsync(null, null);
    private async void PropertyList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    { if (PropertyList.SelectedItem is FormEditorProperty property) await RequestAddAsync(property, null); }
    private async void ToolboxList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    { if (ToolboxList.SelectedItem is ArasFormOption type) await RequestAddAsync(null, type.Value); }
    private async Task RequestAddAsync(FormEditorProperty? property, string? type)
    {
        if (!_vm.CanEdit || _dialogOpen) return;
        _pendingProperty = property; _pendingTool = type;
        await ScriptAsync("window.formEditor.requestInsert()");
    }
    private void PropertyList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (!_vm.CanEdit) { e.Cancel = true; return; }
        if (e.Items.FirstOrDefault() is FormEditorProperty property)
        { e.Data.SetText(JsonSerializer.Serialize(new { formEditor = true, propertyId = property.Id })); e.Data.RequestedOperation = DataPackageOperation.Copy; }
    }
    private void ToolboxList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (!_vm.CanEdit) { e.Cancel = true; return; }
        if (e.Items.FirstOrDefault() is ArasFormOption type)
        { e.Data.SetText(JsonSerializer.Serialize(new { formEditor = true, fieldType = type.Value })); e.Data.RequestedOperation = DataPackageOperation.Copy; }
    }

    private async Task AddDialogAsync(FormEditorProperty? property, string? initialType, int x, int y)
    {
        if (!_vm.CanEdit || _dialogOpen || _vm.Metadata == null || _vm.Session == null) return;
        _dialogOpen = true;
        try
        {
            var metadata = _vm.Metadata; var panel = new StackPanel { Spacing = 10, MinWidth = 380 };
            var propertyPicker = new ComboBox { Header = "绑定对象类属性", HorizontalAlignment = HorizontalAlignment.Stretch };
            propertyPicker.Items.Add(new ComboBoxItem { Content = "不绑定属性" });
            foreach (var option in _vm.Session.Document.Properties) propertyPicker.Items.Add(new ComboBoxItem { Content = option.DisplayName, Tag = option });
            propertyPicker.SelectedItem = propertyPicker.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Tag == property) ?? propertyPicker.Items[0];
            var types = new ComboBox { Header = "控件类型", HorizontalAlignment = HorizontalAlignment.Stretch };
            var extraPanel = new StackPanel { Spacing = 10 }; var error = new TextBlock { TextWrapping = TextWrapping.Wrap };
            var extras = new Dictionary<string, Func<string?>>();
            TextBox? staticImage = null;
            void TextSetting(string name, string label, string initial = "", bool multiline = false)
            {
                if (name == "" || !metadata.Field.ContainsKey(name)) return;
                var box = new TextBox { Header = label, Text = initial, AcceptsReturn = multiline, TextWrapping = multiline ? TextWrapping.Wrap : TextWrapping.NoWrap, MaxHeight = multiline ? 170 : double.PositiveInfinity };
                extraPanel.Children.Add(box); extras[name] = () => box.Text;
            }
            void UpdateExtras()
            {
                extras.Clear(); extraPanel.Children.Clear(); staticImage = null;
                var type = (types.SelectedItem as ComboBoxItem)?.Tag as string;
                if (type == "html") TextSetting("html_code", "HTML 内容（预览不执行脚本）", "<div>新内容</div>", true);
                if (type == "image" && property == null) TextSetting(metadata.ImageProperty, "图片来源（Aras 路径或图片 URL）");
                if (type == "image" && property == null && metadata.ImageProperty == "")
                {
                    staticImage = new TextBox { Header = "静态图片来源（保存为 HTML 图片控件）", PlaceholderText = "../images/Information.svg" };
                    extraPanel.Children.Add(staticImage);
                    extraPanel.Children.Add(new TextBlock { Text = "此系统的 Image 类型用于属性绑定；独立静态图片通过 HTML 控件保存。", FontSize = 12, TextWrapping = TextWrapping.Wrap });
                }
                if (type == "nested form" && metadata.NestedFormProperty == "")
                    extraPanel.Children.Add(new TextBlock { Text = "请选择一个 Item 类型属性作为数据源。运行时使用该属性引用对象的窗体；右侧可切换关联窗体预览。", FontSize = 12, TextWrapping = TextWrapping.Wrap });
                if (type == "groupbox") { TextSetting("legend", "分组标题", "新分组"); TextSetting("width", "宽度", "300"); TextSetting("height", "高度", "150"); TextSetting("border_width", "边框宽度", "1"); }
                if (type == "nested form" && metadata.NestedFormProperty != "")
                {
                    var forms = new ComboBox { Header = "嵌套窗体", HorizontalAlignment = HorizontalAlignment.Stretch };
                    foreach (var form in metadata.AvailableForms.Where(form => form.Id != _vm.Session.Document.Form.Id && !form.Classification.Contains("responsive", StringComparison.OrdinalIgnoreCase)))
                        forms.Items.Add(new ComboBoxItem { Content = form.Name, Tag = form.Id });
                    extraPanel.Children.Add(forms); extras[metadata.NestedFormProperty] = () => (forms.SelectedItem as ComboBoxItem)?.Tag as string;
                }
                if (metadata.ContainerProperty != "")
                {
                    var containers = new ComboBox { Header = "所属容器", HorizontalAlignment = HorizontalAlignment.Stretch };
                    containers.Items.Add(new ComboBoxItem { Content = "窗体根部", Tag = "" });
                    foreach (var group in _vm.Session.Fields.Where(field => field.Get("field_type") == "groupbox"))
                        containers.Items.Add(new ComboBoxItem { Content = group.Name, Tag = group.Name });
                    containers.SelectedItem = containers.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == _insertionContainer) ?? containers.Items[0];
                    extraPanel.Children.Add(containers);
                    extras[metadata.ContainerProperty] = () => (containers.SelectedItem as ComboBoxItem)?.Tag as string;
                }
            }
            void UpdateTypes()
            {
                property = (propertyPicker.SelectedItem as ComboBoxItem)?.Tag as FormEditorProperty;
                types.Items.Clear();
                var options = FormEditorRules.CompatibleTypes(metadata, property);
                if (property == null) options = options.Where(option => new[] { "label", "button", "html", "image", "groupbox", "nested form" }.Contains(option.Value));
                foreach (var option in options) types.Items.Add(new ComboBoxItem { Content = option.Label, Tag = option.Value });
                types.SelectedItem = types.Items.OfType<ComboBoxItem>().FirstOrDefault(item => (string)item.Tag == initialType) ?? types.Items.FirstOrDefault();
                UpdateExtras();
            }
            propertyPicker.SelectionChanged += (_, _) => UpdateTypes(); types.SelectionChanged += (_, _) => UpdateExtras();
            panel.Children.Add(propertyPicker); panel.Children.Add(types);
            panel.Children.Add(new TextBlock { Text = $"放置位置 X={x} / Y={y}；选择容器后使用容器内坐标。", FontSize = 12, TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(extraPanel); panel.Children.Add(error); UpdateTypes();
            var dialog = new ContentDialog { XamlRoot = XamlRoot, Title = "新增控件", PrimaryButtonText = "添加", CloseButtonText = "取消", Content = new ScrollViewer { MaxHeight = 520, Content = panel } };
            dialog.PrimaryButtonClick += (dialogSender, args) =>
            {
                if (types.SelectedItem is not ComboBoxItem { Tag: string type }) { args.Cancel = true; error.Text = "请选择可用的控件类型。"; return; }
                var values = extras.ToDictionary(pair => pair.Key, pair => pair.Value());
                if (staticImage != null)
                {
                    if (string.IsNullOrWhiteSpace(staticImage.Text)) { args.Cancel = true; error.Text = "请填写静态图片来源。"; return; }
                    type = "html";
                    values["html_code"] = new System.Xml.Linq.XElement("img", new System.Xml.Linq.XAttribute("src", staticImage.Text.Trim()), new System.Xml.Linq.XAttribute("alt", "图片")).ToString();
                    values["label"] = "";
                }
                var containerName = metadata.ContainerProperty == "" ? "" : values.GetValueOrDefault(metadata.ContainerProperty) ?? "";
                var origin = _insertionContainers.GetValueOrDefault(containerName);
                var localX = Math.Max(0, (int)Math.Round(x - origin.X)); var localY = Math.Max(0, (int)Math.Round(y - origin.Y));
                if (!_vm.Add(type, property, localX, localY, values)) { args.Cancel = true; error.Text = _vm.Error; }
                else if (type == "nested form") _ = _vm.RefreshNestedPreviewAsync();
            };
            await dialog.ShowAsync();
        }
        catch (Exception ex) { await _vm.ReportErrorAsync("新增控件", ex); }
        finally { _dialogOpen = false; }
    }
}
