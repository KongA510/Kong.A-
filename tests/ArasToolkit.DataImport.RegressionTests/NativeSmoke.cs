#if DISABLE_XAML_GENERATED_MAIN
using System.Reflection;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.App.WinUI.Views;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ToolkitApp = ArasToolkit.App.WinUI.App;

namespace ArasToolkit.DataImport.NativeTests;

public static class NativeProgram
{
    public static string LogPath => Path.Combine(AppContext.BaseDirectory, "data-import-native.log");
    public static void Log(string text) => File.AppendAllText(LogPath, text + Environment.NewLine);
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
        int exitCode = 0;
        try
        {
            var uiThread = Environment.CurrentManagedThreadId;
            var fixture = new ImportFixture();
            var errors = DispatchProxy.Create<IErrorLogService, NativeProxy>();
            ((NativeProxy)(object)errors).Handler = (_, a) => { NativeProgram.Log("ERROR: " + a[1]); return Task.CompletedTask; };
            var dialogs = DispatchProxy.Create<IDialogService, NativeProxy>();
            var files = DispatchProxy.Create<IFileDialogService, NativeProxy>();
            var services = new ServiceCollection();
            services.AddSingleton<IDataImportService>(fixture);
            services.AddSingleton(errors); services.AddSingleton(dialogs); services.AddSingleton(files);
            services.AddTransient<DataImportViewModel>();
            typeof(ToolkitApp).GetField("_serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, services.BuildServiceProvider());
            var page = new DataImportPage();
            var window = new Window { Title = "数据汇入原生验证", Content = new Frame { Content = page } };
            MainWindow = window;
            window.Activate();
            var vm = (DataImportViewModel)page.DataContext;
            vm.PropertyChanged += (_, _) =>
            {
                if (Environment.CurrentManagedThreadId != uiThread) throw new Exception("ViewModel notification escaped the UI thread");
            };
            vm.SelectedFilePath = "native-test.xlsx";
            vm.SelectedSheetName = "Data";
            vm.AmlContent = "<AML><Item type='Part' action='add'/></AML>";
            vm.ExecuteImportCommand.Execute(null);
            await Wait(() => vm.ImportProgress >= 5, "initial progress");
            if (vm.CanEditImport || vm.ExecuteImportCommand.CanExecute(null)) throw new Exception("Inputs still enabled during import");
            vm.PauseCommand.Execute(null);
            await Task.Delay(150);
            var pausedCalls = fixture.Rows;
            await Task.Delay(150);
            if (fixture.Rows != pausedCalls || !vm.IsPaused || !vm.IsImporting) throw new Exception("Pause did not suspend the batch");
            vm.ResumeCommand.Execute(null);
            await Wait(() => !vm.IsImporting, "completed import");
            if (fixture.Executions != 1 || fixture.Rows != 100 || vm.ImportProgress != 100 || vm.LastResult?.SuccessCount != 100 || vm.ErrorMessage.Length > 0)
                throw new Exception("Resume restarted the import or lost progress");
            NativeProgram.Log("PASS: real WinUI page binding, background progress, pause/resume without replay, final counters");
            vm.MaxConcurrency = 4;
            vm.ExecuteImportCommand.Execute(null);
            await Wait(() => !vm.IsImporting, "concurrent import");
            if (fixture.Executions != 2 || fixture.Rows != 200 || vm.ImportProgress != 100 || vm.LastResult?.SuccessCount != 100)
                throw new Exception("Concurrent callbacks lost progress or duplicated rows");
            NativeProgram.Log("PASS: four concurrent workers update WinUI safely");
            fixture.Abort = true;
            vm.ExecuteImportCommand.Execute(null);
            await Wait(() => !vm.IsImporting, "aborted import");
            if (vm.ImportProgress != 20 || !vm.StatusMessage.Contains("中断") || vm.ErrorMessage.Length == 0)
                throw new Exception("Aborted import displayed success");
            NativeProgram.Log("PASS: partial results remain visible and never report completion");
            fixture.Abort = false;
            fixture.Reject = true;
            vm.ExecuteImportCommand.Execute(null);
            await Wait(() => !vm.IsImporting, "server rejected rows");
            if (vm.LastResult?.FailureCount != 117 || !vm.StatusMessage.Contains("存在失败") || vm.ErrorMessage.Length == 0)
                throw new Exception("Server rejection was hidden from the user");
            NativeProgram.Log("PASS: all 117 rejected rows display accurate counts and a failure message");
            window.Close();
        }
        catch (Exception ex) { NativeProgram.Log("FAIL: " + ex); exitCode = 1; }
        Environment.Exit(exitCode);
    }

    private static async Task Wait(Func<bool> ready, string stage)
    {
        for (var i = 0; i < 200 && !ready(); i++) await Task.Delay(25);
        if (!ready()) throw new TimeoutException(stage);
    }
}

public sealed class ImportFixture : IDataImportService
{
    public int Rows;
    public int Executions;
    public bool Abort;
    public bool Reject;
    public Task<ImportResult> ExecuteImportAsync(string filePath, string? sheetName, int startRow, int endRow,
        int startCol, int endCol, string amlContent, int maxConcurrency = 1, CancellationToken cancellationToken = default,
        Func<int, int, Task>? progressCallback = null) => Task.Run(async () =>
    {
        Interlocked.Increment(ref Executions);
        if (Abort) return new ImportResult { TotalRows = 100, ProcessedRows = 20, SuccessCount = 20, ErrorMessage = "Simulated connection interruption" };
        if (Reject) return new ImportResult { TotalRows = 117, ProcessedRows = 117, FailureCount = 117 };
        int processed = 0;
        await Parallel.ForEachAsync(Enumerable.Range(1, 100), new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency }, async (i, ct) =>
        {
            Interlocked.Increment(ref Rows);
            await Task.Delay(i % 4 + 5, ct);
            await progressCallback!(Interlocked.Increment(ref processed), 100);
        });
        return new ImportResult { TotalRows = 100, ProcessedRows = 100, SuccessCount = 100 };
    });
    public Task<List<DataImportConfig>> GetConfigsAsync() => Task.FromResult(new List<DataImportConfig>());
    public Task<DataImportConfig> SaveConfigAsync(DataImportConfig config) => throw new NotSupportedException();
    public Task DeleteConfigAsync(string id) => throw new NotSupportedException();
    public Task<List<string>> GetSheetNamesAsync(string filePath) => throw new NotSupportedException();
    public Task<ExcelSheetData> ReadSheetRangeAsync(string f, string s, int r1, int r2, int c1, int c2) => throw new NotSupportedException();
    public Task<List<ColumnMapping>> GetColumnMappingsAsync(string f, string s, int c1, int c2) => throw new NotSupportedException();
    public string ReplaceAmlPlaceholders(string a, Dictionary<string, string> r) => throw new NotSupportedException();
    public string PreviewAml(string a, Dictionary<string, string> r) => throw new NotSupportedException();
}

public class NativeProxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = (_, _) => throw new NotSupportedException();
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
}
#endif
