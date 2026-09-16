// Compiled only by the opt-in WinUI native host, never by the console regression runner.
#if DISABLE_XAML_GENERATED_MAIN
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using ArasToolkit.App.WinUI;
using ToolkitApp = ArasToolkit.App.WinUI.App;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace ArasToolkit.FormEditor.NativeTests;

public static class NativeProgram
{
    public static readonly string Output = Environment.GetEnvironmentVariable("FORM_EDITOR_SMOKE_OUTPUT") ?? Path.Combine(Path.GetTempPath(), "form-editor-native-smoke");
    public static void Log(string text) { Directory.CreateDirectory(Output); File.AppendAllText(Path.Combine(Output,"native.log"), DateTime.Now.ToString("HH:mm:ss.fff ") + text + Environment.NewLine); }
    [STAThread]
    public static void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ =>
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
            new NativeApp();
        });
    }
}

public sealed class NativeApp : ToolkitApp
{
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window? window = null;
        FormConfigurationEditViewModel? vm = null;
        var asset = Path.Combine(AppContext.BaseDirectory, "Assets", "FormEditor", "editor.js");
        byte[]? originalScript = null;
        try
        {
            NativeProgram.Log("Starting native page");
            using (var nativeDll = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Microsoft.Web.WebView2.Core.dll")))
            using (var pe = new PEReader(nativeDll))
            {
                var expectedMachine = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => Machine.Amd64, Architecture.X86 => Machine.I386,
                    Architecture.Arm64 => Machine.Arm64, _ => throw new Exception("Unsupported process architecture")
                };
                if (pe.PEHeaders.CoffHeader.Machine != expectedMachine) throw new Exception("WebView2 native DLL architecture does not match process");
                NativeProgram.Log($"PASS native DLL matches process: {expectedMachine}");
            }
            var services = new ServiceCollection();
            var connection = DispatchProxy.Create<IArasConnectionService, NativeProxy>();
            ((NativeProxy)(object)connection).Handler = (method, _) => method.Name switch
            {
                "get_CurrentConnection" => new ArasConnectionInfo { Url="https://example.invalid/Innovator",Database="NativeSmoke",Username="tester" },
                "get_IsConnected" => true, _ => null
            };
            var errors = DispatchProxy.Create<IErrorLogService, NativeProxy>();
            ((NativeProxy)(object)errors).Handler = (method, values) => { NativeProgram.Log("ERROR " + string.Join(" | ", values)); return Task.CompletedTask; };
            services.AddSingleton(connection); services.AddSingleton(errors);
            services.AddSingleton<IFormConfigurationEditService, NativeFixture>();
            services.AddTransient<FormConfigurationEditViewModel>();
            var generator = DispatchProxy.Create<IFormConfigurationService, NativeProxy>();
            ((NativeProxy)(object)generator).Handler = (method, _) => method.Name == "GetItemTypesAsync"
                ? Task.FromResult<IReadOnlyList<ArasItemTypeInfo>>([]) : null;
            services.AddSingleton(generator);
            services.AddSingleton(DispatchProxy.Create<IDialogService, NativeProxy>());
            services.AddTransient<FormConfigurationViewModel>();
            typeof(ToolkitApp).GetField("_serviceProvider", BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(this, services.BuildServiceProvider());
            var page = new FormConfigurationEditPage();
            var web = (WebView2)page.FindName("CanvasBrowser");
            web.CoreWebView2Initialized += (_, e) =>
            {
                NativeProgram.Log("Core initialized " + e.Exception?.Message);
                if (web.CoreWebView2 == null) return;
                web.CoreWebView2.NavigationStarting += (_, a) => NativeProgram.Log("NavigationStarting " + a.Uri + " cancelled=" + a.Cancel);
                web.CoreWebView2.NavigationCompleted += (_, a) => NativeProgram.Log("NavigationCompleted " + a.IsSuccess + " " + a.WebErrorStatus + " " + a.HttpStatusCode);
                web.CoreWebView2.WebMessageReceived += (_, a) => NativeProgram.Log("Message " + a.WebMessageAsJson);
            };
            window = new Window { Title = "窗体编辑器原生回归", Content = new Frame { Content = page } }; MainWindow = window;
            var scale = window.Content.XamlRoot?.RasterizationScale ?? 2;
            window.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(1500 * scale), (int)(950 * scale))); window.Activate();
            vm=(FormConfigurationEditViewModel)page.DataContext;
            for(int i=0;i<120&&vm.ItemTypes.Count==0;i++) await Task.Delay(100);
            await vm.SelectItemTypeAsync(new ArasItemTypeInfo{Id=NativeFixture.TypeId,Name="NativeFixture"});
            await vm.SelectFormAsync(new FormEditorSummary{Id=NativeFixture.FormId,Name="NativeFixture"});
            await WaitUntilAsync(() => ((Border)page.FindName("CanvasStatusPanel")).Visibility == Visibility.Collapsed, "canvas ready");
            await Task.Delay(500);
            NativeProgram.Log($"Layout: page={page.ActualWidth}x{page.ActualHeight}; WebView={web.ActualWidth}x{web.ActualHeight}; loaded={web.IsLoaded}; visible={web.Visibility}; source={web.Source}; coreSource={web.CoreWebView2?.Source}");
            if(web.CoreWebView2!=null)
            {
                var state=await web.CoreWebView2.ExecuteScriptAsync("JSON.stringify({url:location.href,ready:document.readyState,width:innerWidth,height:innerHeight,editor:!!window.formEditor,fields:document.querySelectorAll('.handle').length,body:document.body?.innerText.slice(0,400)})");
                NativeProgram.Log("STATE " + state);
                using var capture=new MemoryStream();
                await web.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png,capture.AsRandomAccessStream());
                File.WriteAllBytes(Path.Combine(NativeProgram.Output,"canvas.png"),capture.ToArray());
                var decoded=JsonSerializer.Deserialize<string>(state);
                using var json=JsonDocument.Parse(decoded??"{}");
                if(json.RootElement.GetProperty("fields").GetInt32()!=vm.Session!.Fields.Count()||web.ActualWidth<100||web.ActualHeight<100) throw new Exception("Native canvas did not render every fixture field");
            }
            else throw new Exception("WebView core not initialized");
            NativeProgram.Log("PASS native canvas");
            await CheckWorkspaceAsync(page, vm, web);
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FORM_EDITOR_SMOKE_SNAPSHOT")))
                await CheckLayoutPreviewAsync(page, vm, web);
            await CheckGeneratorAsync();
            var panel = (Border)page.FindName("CanvasStatusPanel");
            var title = (TextBlock)page.FindName("CanvasStatusTitle");
            var retry = (Button)page.FindName("CanvasRetry");
            bool HasCanvasError() => panel.Visibility == Visibility.Visible && title.Text == "画布加载失败" && retry.Visibility == Visibility.Visible;
            async Task RetryAsync()
            {
                var peer = new Microsoft.UI.Xaml.Automation.Peers.ButtonAutomationPeer(retry);
                ((Microsoft.UI.Xaml.Automation.Provider.IInvokeProvider)peer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Invoke)).Invoke();
                await WaitUntilAsync(() => panel.Visibility == Visibility.Collapsed, "retry ready");
                await Task.Delay(300);
                var count = await web.CoreWebView2.ExecuteScriptAsync("document.querySelectorAll('.handle').length");
                if (count != vm.Session!.Fields.Count().ToString()) throw new Exception("Retry did not restore all controls");
                if (!vm.HasChanges || vm.Session.Document.Form.Get("height") != "421" || !vm.CanUndo) throw new Exception("Retry lost draft or undo history");
            }

            // Exercise JavaScript exception reporting through the production host, then the real retry button.
            await web.CoreWebView2.ExecuteScriptAsync("window.formEditor.setState = () => { throw new Error('native smoke render failure'); }");
            vm.SetProperty("height", "421");
            await WaitUntilAsync(HasCanvasError, "render error");
            await vm.InitializeAsync(); // Querying data clears the generic VM error, but must not hide the canvas failure.
            if (vm.HasError || !HasCanvasError()) throw new Exception("Canvas error was hidden by successful data query");
            NativeProgram.Log("PASS render error remains visible after data refresh");
            await RetryAsync();
            NativeProgram.Log("PASS retry restores controls and preserves draft / undo");

            if (Environment.GetEnvironmentVariable("FORM_EDITOR_SMOKE_ASSET_FAULTS") == "1")
            {
                var relative = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), ".codex"), asset);
                if (Path.IsPathRooted(relative) || relative.StartsWith("..")) throw new Exception("Asset fault tests require isolated output inside workspace .codex");
                originalScript = File.ReadAllBytes(asset);
                File.Delete(asset);
                await (Task)typeof(FormConfigurationEditPage).GetMethod("InitializeCanvasAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;
                if (!HasCanvasError() || !((TextBlock)page.FindName("CanvasStatusMessage")).Text.Contains("editor.js")) throw new Exception("Missing asset did not show persistent failure");
                File.WriteAllBytes(asset, originalScript);
                await RetryAsync();
                NativeProgram.Log("PASS missing asset / recovery");

                File.WriteAllText(asset, "// Deliberately omit the ready handshake in this isolated test output.");
                await web.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
                await (Task)typeof(FormConfigurationEditPage).GetMethod("InitializeCanvasAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;
                if (!HasCanvasError()) throw new Exception("Missing ready handshake left a blank canvas");
                File.WriteAllBytes(asset, originalScript);
                await web.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
                await RetryAsync();
                NativeProgram.Log("PASS startup handshake timeout / recovery");
            }
            vm.DiscardChanges();
            await Task.Delay(100);
            window.Close(); window = null;
            if (!(bool)typeof(FormConfigurationEditPage).GetField("_disposed", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!)
                throw new Exception("Closing the window did not dispose the editor");
            NativeProgram.Log("PASS native window closes cleanly");
        }
        catch(Exception ex) { NativeProgram.Log("FAIL " + ex); Environment.ExitCode=1; }
        finally
        {
            if (originalScript != null) File.WriteAllBytes(asset, originalScript);
            vm?.DiscardChanges();
            window?.Close(); Exit();
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string operation)
    {
        for (var i = 0; i < 200; i++)
        {
            if (condition()) return;
            await Task.Delay(100);
        }
        throw new TimeoutException(operation);
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
    {
        for (var i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(root, i);
            if (child is T match) yield return match;
            foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }

    private static async Task CheckLayoutPreviewAsync(FormConfigurationEditPage page, FormConfigurationEditViewModel vm, WebView2 web)
    {
        var fields = vm.Session!.Fields.ToArray(); var first = fields[0].Id;
        var rows = (StackPanel)page.FindName("LayoutPreviewRows");
        if (rows.Children.Count != fields.Length) throw new Exception("Layout preview omitted fields");
        await Task.Delay(100);
        TextBox Cell(string label) => Descendants<TextBox>(rows).Single(box => Microsoft.UI.Xaml.Automation.AutomationProperties.GetName(box) == fields[0].Name + " " + label);
        vm.Select(fields.Select(field => field.Id));
        var x = Cell("X"); x.Focus(FocusState.Programmatic); x.Text = "81";
        Cell("Y").Focus(FocusState.Programmatic); await Task.Delay(150);
        if (vm.Session.Fields.First().Get("x") != "81" || vm.Session.Fields.Skip(1).First().Get("x") != "300") throw new Exception("Bottom edit affected wrong selection");
        vm.Undo(); if (Cell("X").Text != "35") throw new Exception("Undo did not refresh layout row");
        vm.Redo(); if (Cell("X").Text != "81") throw new Exception("Redo did not refresh layout row");
        NativeProgram.Log("PASS layout coordinate edits target one ID, preserve multi-selection, undo / redo refresh rows");
        vm.Select([first]); vm.SetProperty("font_color", "#123abc");
        if (Cell("标题颜色").Text != "#123abc") throw new Exception("Inspector color did not reach layout table");
        var color = Cell("标题颜色"); color.Focus(FocusState.Programmatic); color.Text = "#ff0000"; Cell("Y").Focus(FocusState.Programmatic); await Task.Delay(200);
        if (vm.Session.Selected.Single().Get("font_color") != "#ff0000") throw new Exception("Bottom color not in draft");
        vm.BeginDrag(); vm.Move(new Dictionary<string,(int,int)> { [first] = (92, 77) });
        if (Cell("X").Text != "92" || Cell("Y").Text != "77") throw new Exception("Drag coordinates not linked during move");
        vm.EndDrag(false);
        NativeProgram.Log("PASS inspector colors, layout colors and live drag coordinates stay linked");
        vm.SetFieldProperty(first, "field_type", "formatted text");
        if (Cell("行尺寸").Visibility != Visibility.Visible || Cell("列尺寸").Visibility != Visibility.Visible) throw new Exception("Rich dimension editors hidden");
        vm.SetFieldProperty(first, "textarea_rows", "180"); vm.SetFieldProperty(first, "textarea_cols", "460");
        var check = Descendants<CheckBox>(rows).First();
        var peer = new Microsoft.UI.Xaml.Automation.Peers.CheckBoxAutomationPeer(check);
        ((Microsoft.UI.Xaml.Automation.Provider.IToggleProvider)peer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Toggle)).Toggle();
        if (vm.Session.Fields.First().Get("is_disabled") != "1") throw new Exception("Bottom checkbox did not commit");
        await Task.Delay(300);
        var state = await web.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#preview').contentDocument.getElementById('{first}').querySelector('.sys_f_value > *').getBoundingClientRect().height");
        if (state != "180") throw new Exception("Native rich preview height not linked: " + state);
        await CaptureAsync(page, "edit-linked.png");
        vm.SetFieldProperty(first, "textarea_rows", "0");
        if (!vm.HasError || Cell("行尺寸").Text != "180") throw new Exception("Invalid dimensions did not restore current draft");
        vm.DiscardChanges(); vm.Select([]);
        NativeProgram.Log("PASS rich dimensions, disabled toggle, validation rollback and discard synchronize");
    }

    private static async Task CheckGeneratorAsync()
    {
        var page = new FormConfigurationPage();
        var window = new Window { Content = page };
        window.AppWindow.Resize(new Windows.Graphics.SizeInt32(1300, 850)); window.Activate();
        try
        {
            var vm = (FormConfigurationViewModel)page.DataContext;
            await Task.Delay(200);
            var field = new ArasFormFieldLayout { Name = "rich", Label = "富文本", FieldType = "formatted text", Sequence = 1 };
            vm.LayoutFields.Add(field);
            var list = (ListView)page.FindName("LayoutList"); list.ScrollIntoView(field);
            await Task.Delay(200);
            var check = Descendants<CheckBox>(list).Single();
            if (check.ActualWidth != 32 || check.Visibility != Visibility.Visible) throw new Exception("Generator checkbox is clipped");
            var peer = new Microsoft.UI.Xaml.Automation.Peers.CheckBoxAutomationPeer(check);
            var toggle = (Microsoft.UI.Xaml.Automation.Provider.IToggleProvider)peer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Toggle);
            toggle.Toggle(); if (!field.IsDisabled) throw new Exception("Generator toggle did not set read-only");
            toggle.Toggle(); if (field.IsDisabled) throw new Exception("Generator toggle did not restore editable");
            var dimensions = Descendants<NumberBox>(list).Where(box => Grid.GetColumn(box) is 7 or 8).ToArray();
            if (dimensions.Length != 2 || dimensions.Any(box => box.Visibility != Visibility.Visible)) throw new Exception("Generator rich dimensions hidden");
            dimensions[0].Value = 180; dimensions[1].Value = 460;
            if (field.TextAreaRows != 180 || field.TextAreaColumns != 460) throw new Exception("Generator rich dimensions not bound");
            await CaptureAsync(page, "generator-rich.png");
            NativeProgram.Log("PASS real generator checkbox visible / toggles both ways; rich dimensions editable");
        }
        finally { window.Close(); }
    }

    private static void Toggle(Microsoft.UI.Xaml.Controls.Primitives.ToggleButton button)
    {
        var peer = new Microsoft.UI.Xaml.Automation.Peers.ToggleButtonAutomationPeer(button);
        ((Microsoft.UI.Xaml.Automation.Provider.IToggleProvider)peer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Toggle)).Toggle();
    }

    private static async Task CheckWorkspaceAsync(FormConfigurationEditPage page, FormConfigurationEditViewModel vm, WebView2 web)
    {
        var dock = (Border)page.FindName("EditorDock");
        var rows = (StackPanel)page.FindName("LayoutPreviewRows");
        var first = vm.Session!.Fields.First();
        var layoutMode = (Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)page.FindName("LayoutModeButton");
        var inspectorMode = (Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)page.FindName("InspectorModeButton");
        var editorToggle = (Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)page.FindName("EditorToggle");
        var toolsToggle = (Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)page.FindName("ToolsToggle");
        var selectedOnly = (CheckBox)page.FindName("SelectedOnlyToggle");
        void CheckViewport()
        {
            var position = dock.TransformToVisual(page).TransformPoint(new Windows.Foundation.Point());
            if (position.Y + dock.ActualHeight > page.ActualHeight + 1 || web.ActualHeight < 200 || web.ActualWidth < 200)
                throw new Exception("Editor does not keep canvas and dock in the same viewport");
        }
        CheckViewport();
        vm.Select([first.Id]);
        vm.SetFieldProperty(first.Id, "font_color", "#123abc");
        Toggle(inspectorMode); await Task.Delay(100);
        if (((ScrollViewer)page.FindName("InspectorScroll")).Visibility != Visibility.Visible || layoutMode.IsChecked == true)
            throw new Exception("Detailed inspector tab did not open");
        var inspector = (StackPanel)page.FindName("Inspector");
        if (!Descendants<TextBox>(inspector).Any(box => box.Header?.ToString() == "标题颜色" && box.Text == "#123abc"))
            throw new Exception("Detailed inspector lost quick edit");
        Toggle(layoutMode); await Task.Delay(100);
        if (((Grid)page.FindName("LayoutPreviewPanel")).Visibility != Visibility.Visible || !vm.HasChanges || !vm.CanUndo)
            throw new Exception("Tab switching lost draft or history");
        NativeProgram.Log("PASS canvas / dock share viewport; layout and detailed tabs preserve draft and undo");
        var filterPeer = new Microsoft.UI.Xaml.Automation.Peers.CheckBoxAutomationPeer(selectedOnly);
        var filter = (Microsoft.UI.Xaml.Automation.Provider.IToggleProvider)filterPeer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Toggle);
        filter.Toggle();
        if (rows.Children.OfType<Border>().Count(card => card.Visibility == Visibility.Visible) != 1) throw new Exception("Selected-only filter failed");
        vm.Select([]);
        if (rows.Children.OfType<Border>().Any(card => card.Visibility == Visibility.Visible)) throw new Exception("Cleared selection left stale cards");
        filter.Toggle();
        if (rows.Children.OfType<Border>().Count(card => card.Visibility == Visibility.Visible) != vm.Session.Fields.Count()) throw new Exception("All-fields view did not recover");
        var wideCanvas = web.ActualWidth;
        Toggle(editorToggle); await Task.Delay(120);
        if (dock.Visibility != Visibility.Collapsed || web.ActualWidth <= wideCanvas) throw new Exception("Collapsing editor did not release canvas width");
        Toggle(editorToggle); await Task.Delay(120);
        CheckViewport();
        NativeProgram.Log("PASS selected-only filter and collapsible editor preserve shared draft");
        await CaptureAsync(dock, "layout-dock.png");
        var window = ToolkitApp.MainWindow!;
        var oldSize = window.AppWindow.Size;
        var scale = page.XamlRoot.RasterizationScale;
        try
        {
            window.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(900 * scale), (int)(760 * scale)));
            await Task.Delay(250); CheckViewport();
            if (toolsToggle.IsChecked == true || dock.Visibility != Visibility.Visible) throw new Exception("Narrow workspace failed to prioritize layout and canvas");
            var scroll = (ScrollViewer)page.FindName("LayoutPreviewScroll");
            if (scroll.ScrollableWidth < 500) throw new Exception("Layout table did not expose all columns horizontally");
            var firstRow = rows.Children.OfType<Border>().First();
            var boxes = Descendants<TextBox>(firstRow).ToList();
            var yPositions = boxes.Where(box => box.Visibility == Visibility.Visible).Select(box => box.TransformToVisual(firstRow).TransformPoint(new Windows.Foundation.Point()).Y).ToArray();
            if (yPositions.Max() - yPositions.Min() > 1) throw new Exception("Layout editors wrapped onto multiple rows");
            scroll.ChangeView(scroll.ScrollableWidth, null, null, true); await Task.Delay(120);
            var columnOffset = scroll.HorizontalOffset;
            vm.Select([first.Id]); await Task.Delay(120);
            if (Math.Abs(scroll.HorizontalOffset - columnOffset) > 1) throw new Exception("Selecting a field reset the visible column");
            await CaptureAsync(dock, "layout-dock-style-columns.png");
            scroll.ChangeView(0, null, null, true); await Task.Delay(100);
            Toggle(toolsToggle); await Task.Delay(120);
            if (dock.Visibility != Visibility.Collapsed || toolsToggle.IsChecked != true) throw new Exception("Narrow tools toggle did not release editor width");
            Toggle(editorToggle); await Task.Delay(120);
            if (toolsToggle.IsChecked == true || dock.Visibility != Visibility.Visible) throw new Exception("Narrow editor toggle did not release tools width");
            CheckViewport();
            await CaptureAsync(dock, "layout-dock-compact.png");
            NativeProgram.Log("PASS 900px workspace keeps canvas visible; single-line table scrolls horizontally and retains column on selection");
        }
        finally
        {
            window.AppWindow.Resize(oldSize); await Task.Delay(150);
            if (toolsToggle.IsChecked != true) Toggle(toolsToggle);
            vm.DiscardChanges(); vm.Select([]);
        }
    }

    private static async Task CaptureAsync(UIElement element, string name)
    {
        var bitmap = new Microsoft.UI.Xaml.Media.Imaging.RenderTargetBitmap();
        await bitmap.RenderAsync(element);
        var pixels = (await bitmap.GetPixelsAsync()).ToArray();
        using var file = File.Create(Path.Combine(NativeProgram.Output, name));
        var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, file.AsRandomAccessStream());
        encoder.SetPixelData(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied,
            (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, pixels);
        await encoder.FlushAsync();
    }
}

public class NativeProxy : DispatchProxy
{
    public Func<MethodInfo,object?[],object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? targetMethod,object?[]? args)=>Handler(targetMethod!,args??[]);
}

public sealed class NativeFixture : IFormConfigurationEditService
{
    public const string TypeId="11111111111111111111111111111111", FormId="22222222222222222222222222222222";
    private const string Key="https://example.invalid/Innovator|NativeSmoke|tester|fixture";
    public Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(CancellationToken token=default)=>Task.FromResult<IReadOnlyList<ArasItemTypeInfo>>([new(){Id=TypeId,Name="NativeFixture"}]);
    public Task<IReadOnlyList<FormEditorSummary>> GetFormsAsync(string id,CancellationToken token=default)=>Task.FromResult<IReadOnlyList<FormEditorSummary>>([new(){Id=FormId,Name="NativeFixture"}]);
    private readonly string? _snapshotFolder = Environment.GetEnvironmentVariable("FORM_EDITOR_SMOKE_SNAPSHOT");
    public Task<FormEditorDefinition> GetDefinitionAsync(string itemTypeId,string formId,CancellationToken token=default)
    {
        if (!string.IsNullOrEmpty(_snapshotFolder))
        {
            var snapshot = JsonSerializer.Deserialize<FormEditorDefinition>(File.ReadAllText(Path.Combine(_snapshotFolder, "definition.json")))!;
            snapshot.ConnectionKey = Key;
            return Task.FromResult(snapshot);
        }
        return Task.FromResult(new FormEditorDefinition
    {
        ItemTypeId=TypeId,ConnectionKey=Key,Form=new(){Type="Form",Id=FormId,Original=new(){{"name","NativeFixture"},{"width","850"},{"height","350"}},Children=[new(){Type="Body",Id=new string('3',32),Children=[
            new(){Id=new string('4',32),Original=new(){{"name","item_number"},{"label","编号"},{"field_type","text"},{"x","35"},{"y","45"},{"positioning","absolute"},{"display_length","180"},{"is_visible","1"}}},
            new(){Id=new string('5',32),Original=new(){{"name","name"},{"label","项目名称"},{"field_type","text"},{"x","300"},{"y","45"},{"positioning","absolute"},{"display_length","180"},{"is_visible","1"}}}]}]}
        });
    }
    public Task<FormEditorMetadata> GetEditorMetadataAsync(CancellationToken token=default)
    {
        if (!string.IsNullOrEmpty(_snapshotFolder))
        {
            var snapshot = JsonSerializer.Deserialize<FormEditorMetadata>(File.ReadAllText(Path.Combine(_snapshotFolder, "metadata.json")))!;
            snapshot.ConnectionKey = Key;
            return Task.FromResult(snapshot);
        }
        var metadata = new FormEditorMetadata
        {ConnectionKey=Key,Field=new(){{"field_type",new(){Name="field_type",Options=[new("Text","text"),new("FormattedText","formatted text"),new("Text Area","textarea")]}}},Form=new(){{"width",new(){Name="width",DataType="integer"}},{"height",new(){Name="height",DataType="integer"}}}};
        foreach (var name in new[] { "x", "y", "display_length", "textarea_rows", "textarea_cols" }) metadata.Field[name] = new() { Name = name, DataType = "integer" };
        metadata.Field["is_disabled"] = new() { Name = "is_disabled", DataType = "boolean" };
        metadata.Field["font_color"] = new() { Name = "font_color", DataType = "string" };
        return Task.FromResult(metadata);
    }
    public FormEditorChangeSet BuildChangeSet(FormEditorDefinition document,FormEditorMetadata metadata)=>
        new ArasToolkit.Services.Services.FormConfigurationEditService(new(), null!, null!).BuildChangeSet(document, metadata);
    public Task<FormEditorDefinition> SaveAsync(FormEditorDefinition document,FormEditorMetadata metadata,CancellationToken token=default)=>throw new InvalidOperationException("Native smoke must not write");
    public Task<FormEditorResource?> GetPreviewResourceAsync(string url,string key,CancellationToken token=default)=>Task.FromResult<FormEditorResource?>(null);
}
#endif
