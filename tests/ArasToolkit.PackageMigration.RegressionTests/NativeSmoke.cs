#if DISABLE_XAML_GENERATED_MAIN
using System.Reflection;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolkitApp = ArasToolkit.App.WinUI.App;

namespace ArasToolkit.PackageMigration.NativeTests;

public static class NativeProgram
{
    public static void Log(string text) => File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "package-native.log"), text + Environment.NewLine);
    [STAThread] public static void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ => { SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread())); new NativeApp(); });
    }
}
public sealed class NativeApp : ToolkitApp
{
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            int searches = 0, executions = 0, uiThread = Environment.CurrentManagedThreadId;
            bool delaySearch = false;
            var source = new MigrationEndpoint { Id = "source", Database = "JSAB", Url = "http://test/source" };
            var target = new MigrationEndpoint { Id = "target", Database = "BLPLM", Url = "http://test/target" };
            var item = new PackageItemSelection { Type = "ItemType", Id = new string('A', 32), Name = "验证对象类", Label = "验证标签" };
            var dependency = new PackageDependency { Item = item, Status = "主动选择", Reason = "用户勾选" };
            var package = new MigrationPackage { ManifestPath = "native-test/imports.mf", Dependencies = [dependency] };
            var plan = new PackageMigrationPlan { Target = target, Package = package, Differences = [new() { ItemKey = item.Key, Kind = "新增", Name = item.DisplayName, Details = "新增 3 个字段与一个窗体" }] };
            var history = new List<PackageMigrationResult>();
            var packages = Proxy.Create<IArasPackageService>((method, a) => method.Name switch
            {
                "get_SelectableTypes" => new List<string> { "ItemType", "Method" },
                "LoadSettingsAsync" => Task.FromResult(new PackageMigrationSettings { EngineDirectory = "official-test" }),
                "SaveSettingsAsync" => Task.CompletedTask,
                "GetHistoryAsync" => Task.FromResult(history.ToList()),
                "SearchAsync" => Search(),
                "AnalyzeAsync" => Task.FromResult(new PackageAnalysis { Source = source, Items = [dependency] }),
                "ExportAsync" => Task.FromResult(package),
                "PreflightAsync" => Task.FromResult(plan),
                "ExecuteAsync" => Execute((IProgress<PackageMigrationProgress>?)a[2]),
                _ => throw new NotSupportedException(method.Name)
            });
            async Task<List<PackageItemSelection>> Search()
            {
                searches++;
                if (delaySearch) await Task.Delay(300);
                return [item];
            }
            async Task<PackageMigrationResult> Execute(IProgress<PackageMigrationProgress>? progress)
            {
                executions++;
                await Task.Run(() => progress!.Report(new() { Stage = "迁入", Message = "验证进度", Completed = 1, Total = 2 }));
                await Task.Delay(100);
                var result = new PackageMigrationResult { Status = "成功", Items = [new() { Key = item.Key, Status = "已验证" }] };
                history.Add(result); return result;
            }
            var sessions = Proxy.Create<IArasMigrationSessionFactory>((_, _) => Task.FromResult(new List<MigrationEndpoint> { source, target }));
            var engine = Proxy.Create<IOfficialPackageEngine>((_, _) => Task.FromResult(new PackageEngineInfo { IsSupported = true, Message = "测试引擎" }));
            var dialogs = Proxy.Create<IDialogService>((_, _) => Task.FromResult(true));
            var errors = Proxy.Create<IErrorLogService>((_, a) => { NativeProgram.Log("ERROR " + a[1]); return Task.CompletedTask; });
            var files = Proxy.Create<IFileDialogService>((_, _) => Task.FromResult<string?>(null));
            var services = new ServiceCollection();
            services.AddSingleton(packages); services.AddSingleton(sessions); services.AddSingleton(engine);
            services.AddSingleton(dialogs); services.AddSingleton(errors); services.AddSingleton(files); services.AddTransient<PackageMigrationViewModel>();
            typeof(ToolkitApp).GetField("_serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, services.BuildServiceProvider());
            var page = new PackageMigrationPage();
            var window = new Window { Title = "导包工具页面验证", Content = page }; MainWindow = window;
            window.AppWindow.Resize(new Windows.Graphics.SizeInt32(1440, 1000));
            window.Activate();
            var vm = (PackageMigrationViewModel)page.DataContext;
            await vm.InitializeAsync(); await Wait(() => !vm.IsBusy);
            vm.PropertyChanged += (_, _) => { if (Environment.CurrentManagedThreadId != uiThread) throw new Exception("UI notification on worker thread"); };
            vm.SearchCommand.Execute(null); await Wait(() => !vm.IsBusy);
            vm.Catalog[0].IsSelected = true;
            if (vm.SelectedRoots.Count != 1 || !vm.AnalyzeCommand.CanExecute(null)) throw new Exception("selection binding");
            vm.AnalyzeCommand.Execute(null); await Wait(() => !vm.IsBusy);
            await Task.Delay(150);
            var tree = (TreeView)page.FindName("DependencyTree");
            page.UpdateLayout();
            if (tree.RootNodes.Count != 1 || tree.ActualHeight < 90) throw new Exception("dependency tree or clipped layout");
            vm.PrepareCommand.Execute(null); await Wait(() => !vm.IsBusy);
            vm.SelectedDifference = vm.Differences.Single();
            if (!vm.ExecuteCommand.CanExecute(null) || !vm.Details.Contains("3 个字段")) throw new Exception("comparison and details");
            vm.Differences[0].Include = false;
            if (vm.ExecuteCommand.CanExecute(null)) throw new Exception("deselection not reflected");
            vm.Differences[0].Include = true;
            await Task.Delay(150);
            var scroll = (ScrollViewer)page.Content;
            scroll.ChangeView(null, 280, null, true); await Task.Delay(150);
            var bitmap = new RenderTargetBitmap(); await bitmap.RenderAsync(page);
            var pixels = await bitmap.GetPixelsAsync();
            var png = await StorageFile.GetFileFromPathAsync(CreateEmpty(Path.Combine(AppContext.BaseDirectory, "package-page.png")));
            using (var stream = await png.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, pixels.ToArray());
                await encoder.FlushAsync();
            }
            vm.ExecuteCommand.Execute(null); await Wait(() => !vm.IsBusy);
            if (executions != 1 || vm.History.Count != 1 || !vm.StatusMessage.Contains("成功") || vm.Percent != 50) throw new Exception("execution progress/history");
            vm.ModeIndex = 3; if (!vm.IsHistory || vm.IsWorkspace) throw new Exception("history tab");
            vm.ModeIndex = 0; delaySearch = true;
            vm.SearchCommand.Execute(null);
            if (vm.CanChooseSource || vm.SearchCommand.CanExecute(null) || !vm.CancelCommand.CanExecute(null)) throw new Exception("busy input controls");
            vm.CancelCommand.Execute(null); await Wait(() => !vm.IsBusy);
            vm.Target = source;
            if (vm.ExecuteCommand.CanExecute(null)) throw new Exception("stale plan");
            NativeProgram.Log("PASS actual WinUI XBF, search, multiselect, dependency tree, selection-aware diff, progress on UI thread, cancellation command, history, stale-plan invalidation");
            window.Close(); Environment.Exit(0);
        }
        catch (Exception ex) { NativeProgram.Log("FAIL " + ex); Environment.Exit(1); }
    }
    static string CreateEmpty(string path) { File.WriteAllBytes(path, []); return path; }
    static T? Find<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is T found) return found;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++) if (Find<T>(VisualTreeHelper.GetChild(root, i)) is { } value) return value;
        return null;
    }
    static async Task Wait(Func<bool> predicate) { for (int i = 0; i < 200 && !predicate(); i++) await Task.Delay(25); if (!predicate()) throw new TimeoutException(); }
}
public class Proxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
    public static T Create<T>(Func<MethodInfo, object?[], object?> handler) where T : class
    { var result = Create<T, Proxy>(); ((Proxy)(object)result).Handler = handler; return result; }
}
#endif
