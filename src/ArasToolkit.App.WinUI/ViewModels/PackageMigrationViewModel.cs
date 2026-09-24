using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

public sealed class PackageMigrationViewModel : ObservableObject, IDisposable
{
    private readonly IArasPackageService _packages;
    private readonly IArasMigrationSessionFactory _sessions;
    private readonly IOfficialPackageEngine _engine;
    private readonly IFileDialogService _files;
    private readonly IDialogService _dialogs;
    private readonly IErrorLogService _errors;
    private readonly List<RelayCommand> _commands = [];
    private CancellationTokenSource? _cancellation;
    private bool _initialized, _busy, _disposed;
    private MigrationEndpoint? _source, _target;
    private PackageAnalysis? _analysis;
    private MigrationPackage? _package;
    private PackageMigrationPlan? _plan;
    private PackageDifference? _selectedDifference;
    private string _engineDirectory = "", _engineStatus = "请选择官方导包工具目录。", _keyword = "", _type = "ItemType";
    private string _status = "选择源系统并勾选配置，开始分析迁移依赖。", _error = "", _packagePath = "", _details = "选择差异查看字段与关系变化。";
    private double _percent;
    private int _mode;

    public PackageMigrationViewModel(IArasPackageService packages, IArasMigrationSessionFactory sessions, IOfficialPackageEngine engine,
        IFileDialogService files, IDialogService dialogs, IErrorLogService errors)
    {
        _packages = packages; _sessions = sessions; _engine = engine; _files = files; _dialogs = dialogs; _errors = errors;
        BrowseEngineCommand = Command(async () => { var path = await _files.PickFolderAsync("选择官方导包工具目录"); if (path != null) { EngineDirectory = path; await ProbeAsync(); } });
        ProbeCommand = Command(ProbeAsync);
        SearchCommand = Command(SearchAsync, () => Source != null);
        AnalyzeCommand = Command(AnalyzeAsync, () => Source != null && SelectedRoots.Count > 0);
        PrepareCommand = Command(async () => { await PrepareAsync(); if (Target != null) await CompareAsync(); }, () => Source != null && SelectedRoots.Count > 0);
        ExportCommand = Command(ExportAsync, () => Source != null && SelectedRoots.Count > 0);
        OpenPackageCommand = Command(OpenAsync);
        CompareCommand = Command(CompareAsync, () => _package != null && Target != null);
        ExecuteCommand = Command(ExecuteAsync, () => _plan?.CanExecute == true);
        RefreshHistoryCommand = Command(RefreshHistoryAsync);
        CancelCommand = new RelayCommand(() => { _cancellation?.Cancel(); StatusMessage = "正在取消，等待当前官方操作结束并核对目标。"; }, () => IsBusy);
    }

    public ObservableCollection<MigrationEndpoint> Connections { get; } = [];
    public ObservableCollection<PackageItemSelection> Catalog { get; } = [];
    public ObservableCollection<PackageItemSelection> SelectedRoots { get; } = [];
    public ObservableCollection<PackageDependency> Dependencies { get; } = [];
    public ObservableCollection<PackageDifference> Differences { get; } = [];
    public ObservableCollection<PackageMigrationResult> History { get; } = [];
    public IReadOnlyList<string> Types => _packages.SelectableTypes;
    public event Action? DependenciesChanged;
    public MigrationEndpoint? Source { get => _source; set { if (SetProperty(ref _source, value)) { Catalog.Clear(); SelectedRoots.Clear(); InvalidateSource(); OnPropertyChanged(nameof(Direction)); } } }
    public MigrationEndpoint? Target { get => _target; set { if (SetProperty(ref _target, value)) { InvalidatePlan(); OnPropertyChanged(nameof(Direction)); } } }
    public string Direction => $"{Source?.Database ?? "源系统"}  →  {Target?.Database ?? "目标系统"}";
    public string SourceVersion => string.IsNullOrEmpty(_analysis?.Source.Version ?? Source?.Version) ? "分析时读取实际版本" : _analysis?.Source.Version ?? Source!.Version;
    public string TargetVersion => string.IsNullOrEmpty(_plan?.Target.Version ?? Target?.Version) ? "比较时读取实际版本" : _plan?.Target.Version ?? Target!.Version;
    public string EngineDirectory { get => _engineDirectory; set { if (SetProperty(ref _engineDirectory, value)) InvalidateSource(); } }
    public string EngineStatus { get => _engineStatus; set => SetProperty(ref _engineStatus, value); }
    public string Keyword { get => _keyword; set => SetProperty(ref _keyword, value); }
    public string SelectedType { get => _type; set => SetProperty(ref _type, value); }
    public string StatusMessage { get => _status; set => SetProperty(ref _status, value); }
    public string ErrorMessage { get => _error; set => SetProperty(ref _error, value); }
    public string PackagePath { get => _packagePath; set => SetProperty(ref _packagePath, value); }
    public string Details { get => _details; set => SetProperty(ref _details, value); }
    public double Percent { get => _percent; set => SetProperty(ref _percent, value); }
    public bool IsBusy { get => _busy; private set { if (SetProperty(ref _busy, value)) { OnPropertyChanged(nameof(IsIdle)); OnPropertyChanged(nameof(CanChooseSource)); RefreshCommands(); } } }
    public bool IsIdle => !IsBusy;
    public string SelectionSummary => $"已选 {SelectedRoots.Count} 项 · 依赖 {Dependencies.Count} 项 · 差异 {Differences.Count(d => d.Kind != "相同")} 项";
    public string IssuesText => string.Join(Environment.NewLine, (_plan?.Issues ?? _analysis?.Issues ?? _package?.Issues ?? []).Select(i => i.ToString()));
    public int ModeIndex { get => _mode; set { if (SetProperty(ref _mode, value)) { OnPropertyChanged(nameof(IsHistory)); OnPropertyChanged(nameof(IsWorkspace)); OnPropertyChanged(nameof(CanChooseSource)); } } }
    public bool IsHistory => ModeIndex == 3;
    public bool IsWorkspace => !IsHistory;
    public bool CanChooseSource => IsIdle && ModeIndex is 0 or 1;
    public PackageDifference? SelectedDifference { get => _selectedDifference; set { if (SetProperty(ref _selectedDifference, value)) Details = value == null ? "选择差异查看字段与关系变化。" : value.Details.Length == 0 ? "配置内容相同。" : value.Details; } }
    public ICommand BrowseEngineCommand { get; }
    public ICommand ProbeCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AnalyzeCommand { get; }
    public ICommand PrepareCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand OpenPackageCommand { get; }
    public ICommand CompareCommand { get; }
    public ICommand ExecuteCommand { get; }
    public ICommand RefreshHistoryCommand { get; }
    public ICommand CancelCommand { get; }

    private RelayCommand Command(Func<Task> action, Func<bool>? can = null)
    {
        var command = new RelayCommand(async () => await RunAsync(action), () => !_disposed && !IsBusy && (can?.Invoke() ?? true));
        _commands.Add(command); return command;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;
        await RunAsync(async () =>
        {
            var settings = await _packages.LoadSettingsAsync();
            foreach (var connection in await _sessions.GetConnectionsAsync(Token)) Connections.Add(connection);
            Source = Connections.FirstOrDefault(c => c.Id == settings.SourceConnectionId) ?? Connections.FirstOrDefault(c => c.Database == "JSAB") ?? Connections.FirstOrDefault();
            Target = Connections.FirstOrDefault(c => c.Id == settings.TargetConnectionId) ?? Connections.FirstOrDefault(c => c.Database == "BLPLM");
            EngineDirectory = settings.EngineDirectory;
            if (EngineDirectory.Length > 0) await ProbeAsync();
            await RefreshHistoryAsync();
        });
    }

    private CancellationToken Token => _cancellation?.Token ?? CancellationToken.None;
    private IProgress<PackageMigrationProgress> Progress() => new Progress<PackageMigrationProgress>(p => { StatusMessage = p.Stage + " · " + p.Message; Percent = p.Percent; });
    private async Task RunAsync(Func<Task> work)
    {
        if (IsBusy || _disposed) return;
        IsBusy = true; ErrorMessage = ""; Percent = 0;
        _cancellation = new CancellationTokenSource();
        try { await work(); }
        catch (OperationCanceledException ex) { StatusMessage = "操作已取消。"; await _errors.LogErrorAsync("导包页面-取消", ex.Message, ErrorLog.LevelP1); }
        catch (Exception ex) { ErrorMessage = ex.Message; StatusMessage = "操作未完成，请查看原因。"; await _errors.LogErrorAsync("导包页面", ex.Message, ErrorLog.LevelP1); }
        finally { _cancellation.Dispose(); _cancellation = null; IsBusy = false; NotifySummary(); }
    }

    private async Task ProbeAsync()
    {
        var info = await _engine.ProbeAsync(EngineDirectory, Token); EngineStatus = info.Message;
        if (info.IsSupported) await SaveSettingsAsync();
    }

    private Task SaveSettingsAsync() => _packages.SaveSettingsAsync(new PackageMigrationSettings
    { EngineDirectory = EngineDirectory, SourceConnectionId = Source?.Id ?? "", TargetConnectionId = Target?.Id ?? "" });

    private async Task SearchAsync()
    {
        foreach (var row in Catalog) row.PropertyChanged -= SelectionChanged;
        Catalog.Clear();
        foreach (var item in await _packages.SearchAsync(Source!.Id, SelectedType, Keyword, Token))
        {
            item.IsSelected = SelectedRoots.Any(x => x.Key == item.Key);
            item.PropertyChanged += SelectionChanged; Catalog.Add(item);
        }
        StatusMessage = $"找到 {Catalog.Count} 项（最多显示 500 项），可切换类型继续补选。";
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(PackageItemSelection.IsSelected) || sender is not PackageItemSelection item) return;
        var existing = SelectedRoots.FirstOrDefault(x => x.Key == item.Key);
        if (item.IsSelected && existing == null) SelectedRoots.Add(item);
        else if (!item.IsSelected && existing != null) SelectedRoots.Remove(existing);
        InvalidateSource();
    }

    public void RemoveSelection(PackageItemSelection item)
    {
        if (IsBusy) return;
        SelectedRoots.Remove(item);
        foreach (var row in Catalog.Where(x => x.Key == item.Key)) row.IsSelected = false;
        InvalidateSource();
    }

    private async Task AnalyzeAsync()
    {
        _analysis = await _packages.AnalyzeAsync(Source!.Id, SelectedRoots.ToList(), EngineDirectory, Progress(), Token);
        SetDependencies(_analysis.Items); OnPropertyChanged(nameof(SourceVersion)); OnPropertyChanged(nameof(IssuesText));
        StatusMessage = _analysis.CanExport ? "依赖分析完成，可以生成 AML 包。" : "分析发现阻断问题，请查看依赖说明。";
        await SaveSettingsAsync();
    }

    private async Task PrepareAsync()
    {
        if (_analysis == null) await AnalyzeAsync();
        _package = await _packages.ExportAsync(_analysis!, EngineDirectory, "", Progress(), Token);
        PackagePath = _package.ManifestPath; InvalidatePlan();
    }

    private async Task ExportAsync()
    {
        var path = await _files.PickSaveFileAsync("Aras迁移包-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".zip", ".zip");
        if (path == null) return;
        if (_analysis == null) await AnalyzeAsync();
        _package = await _packages.ExportAsync(_analysis!, EngineDirectory, path, Progress(), Token);
        PackagePath = path; InvalidatePlan(); StatusMessage = "AML 包已导出：" + path;
    }

    private async Task OpenAsync()
    {
        var path = await _files.PickOpenFileAsync("选择官方 AML 清单或迁移 ZIP", ".mf", ".zip");
        if (path == null) return;
        _package = await _packages.ReadPackageAsync(path, Token); _analysis = null;
        PackagePath = path; InvalidatePlan(); SetDependencies(_package.Dependencies);
        StatusMessage = $"已载入 {_package.Items.Count} 项配置，选择目标后比较。";
    }

    private async Task CompareAsync()
    {
        _plan = await _packages.PreflightAsync(_package!, Target!.Id, EngineDirectory, Progress(), Token);
        foreach (var difference in Differences) difference.PropertyChanged -= DifferenceChanged;
        Differences.Clear();
        foreach (var difference in _plan.Differences) { difference.PropertyChanged += DifferenceChanged; Differences.Add(difference); }
        foreach (var node in Dependencies)
        {
            var diff = Differences.FirstOrDefault(d => d.ItemKey == node.Item.Key);
            if (diff != null) node.Status = diff.Kind == "相同" ? "目标已存在" : diff.Kind;
            else if (node.IsExternal) node.Status = _plan.Issues.Any(i => i.ItemKey == node.Item.Key && i.IsBlocking) ? "缺失/冲突" : "目标前置依赖";
        }
        DependenciesChanged?.Invoke();
        OnPropertyChanged(nameof(TargetVersion)); OnPropertyChanged(nameof(IssuesText));
        StatusMessage = _plan.CanExecute ? "比较完成，核对勾选的新增/合并项后执行迁入。" : _plan.Issues.Any(i => i.IsBlocking) || Differences.Any(i => i.IsBlocking) ? "存在阻断问题，暂不能迁入。" : "目标配置已一致，无需迁入。";
        await SaveSettingsAsync();
    }

    private void DifferenceChanged(object? sender, PropertyChangedEventArgs args) { if (args.PropertyName == nameof(PackageDifference.Include)) RefreshCommands(); }

    private async Task ExecuteAsync()
    {
        var count = _plan!.Differences.Count(x => x.Include && x.Kind is "新增" or "合并");
        if (!await _dialogs.ConfirmAsync("迁入已核对的配置", $"目标：{Target!.DisplayName}\n版本：{_plan.Target.Version}\n共 {count} 项新增/合并。\n执行前自动复查并保存目标配置备份。", "备份并迁入")) return;
        var result = await _packages.ExecuteAsync(_plan, EngineDirectory, Progress(), Token);
        StatusMessage = result.Status + " · " + result.Directory;
        Details = result.Message + Environment.NewLine + string.Join(Environment.NewLine, result.Items.Select(i => i.Status + " · " + i.Name + "\n" + i.Message));
        _plan = null; await RefreshHistoryAsync();
    }

    private async Task RefreshHistoryAsync()
    {
        History.Clear(); foreach (var entry in await _packages.GetHistoryAsync(Token)) History.Add(entry);
    }

    public async Task OpenHistoryAsync(PackageMigrationResult entry) => await RunAsync(async () =>
    {
        Details = entry.Message + Environment.NewLine + string.Join(Environment.NewLine, entry.Items.Select(i => i.Status + " · " + i.Name + "\n" + i.Message));
        await Task.CompletedTask;
        Process.Start(new ProcessStartInfo { FileName = entry.Directory, UseShellExecute = true });
    });

    private void SetDependencies(IEnumerable<PackageDependency> nodes)
    { Dependencies.Clear(); foreach (var item in nodes) Dependencies.Add(item); DependenciesChanged?.Invoke(); NotifySummary(); }
    private void InvalidateSource() { _analysis = null; _package = null; PackagePath = ""; Dependencies.Clear(); DependenciesChanged?.Invoke(); InvalidatePlan(); }
    private void InvalidatePlan() { _plan = null; foreach (var row in Differences) row.PropertyChanged -= DifferenceChanged; Differences.Clear(); OnPropertyChanged(nameof(IssuesText)); OnPropertyChanged(nameof(TargetVersion)); OnPropertyChanged(nameof(SourceVersion)); NotifySummary(); }
    private void NotifySummary() { OnPropertyChanged(nameof(SelectionSummary)); OnPropertyChanged(nameof(IssuesText)); RefreshCommands(); }
    private void RefreshCommands() { foreach (var command in _commands) command.RaiseCanExecuteChanged(); (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    public void Dispose() { _disposed = true; _cancellation?.Cancel(); foreach (var row in Catalog) row.PropertyChanged -= SelectionChanged; foreach (var row in Differences) row.PropertyChanged -= DifferenceChanged; RefreshCommands(); }
}
