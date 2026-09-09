// Compiled only by the opt-in WinUI native host, never by the console regression runner.
#if DISABLE_XAML_GENERATED_MAIN
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
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
        return Task.FromResult(new FormEditorMetadata
        {ConnectionKey=Key,Field=new(){{"field_type",new(){Name="field_type",Options=[new("Text","text")]}}},Form=new(){{"width",new(){Name="width",DataType="integer"}},{"height",new(){Name="height",DataType="integer"}}}});
    }
    public FormEditorChangeSet BuildChangeSet(FormEditorDefinition document,FormEditorMetadata metadata)=>new();
    public Task<FormEditorDefinition> SaveAsync(FormEditorDefinition document,FormEditorMetadata metadata,CancellationToken token=default)=>throw new InvalidOperationException("Native smoke must not write");
    public Task<FormEditorResource?> GetPreviewResourceAsync(string url,string key,CancellationToken token=default)=>Task.FromResult<FormEditorResource?>(null);
}
#endif
