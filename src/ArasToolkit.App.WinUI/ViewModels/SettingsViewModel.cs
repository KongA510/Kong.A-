using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 设置窗口 ViewModel — 数据库检查 / 退出登录 / 连接字符串管理 / 资料文件夹地址
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private readonly IErrorLogService _errorLogService;
    private readonly IDatabaseSchemaService _databaseSchemaService;
    private readonly IConfigService _configService;
    private readonly IFileDialogService _fileDialogService;

    private string _connectionString = "";
    private string _statusMessage = "";
    private string _dataFolderPath = "";
    private bool _isCheckingDb;
    private bool _isSaving;
    private bool _isBrowsingFolder;

    public SettingsViewModel(
        IErrorLogService errorLogService,
        IDatabaseSchemaService databaseSchemaService,
        IConfigService configService,
        IFileDialogService fileDialogService)
    {
        _errorLogService = errorLogService;
        _databaseSchemaService = databaseSchemaService;
        _configService = configService;
        _fileDialogService = fileDialogService;

        ConnectionString = ReadConnectionString();

        CheckDatabaseCommand = new RelayCommand(async _ => await CheckDatabaseAsync(), _ => !IsCheckingDb);
        LogoutCommand = new RelayCommand(_ => OnLogoutRequested());
        SaveConnectionStringCommand = new RelayCommand(async _ => await SaveConnectionStringAsync(), _ => !IsSaving);
        BrowseDataFolderCommand = new RelayCommand(async _ => await BrowseDataFolderAsync(), _ => !IsBrowsingFolder);

        _ = LoadDataFolderPathAsync();
    }

    public string ConnectionString
    {
        get => _connectionString;
        set => SetProperty(ref _connectionString, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string DataFolderPath
    {
        get => _dataFolderPath;
        set => SetProperty(ref _dataFolderPath, value);
    }

    public bool IsCheckingDb
    {
        get => _isCheckingDb;
        set
        {
            if (SetProperty(ref _isCheckingDb, value))
                ((RelayCommand)CheckDatabaseCommand).RaiseCanExecuteChanged();
        }
    }

    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
                ((RelayCommand)SaveConnectionStringCommand).RaiseCanExecuteChanged();
        }
    }

    public bool IsBrowsingFolder
    {
        get => _isBrowsingFolder;
        set
        {
            if (SetProperty(ref _isBrowsingFolder, value))
                ((RelayCommand)BrowseDataFolderCommand).RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<DatabaseSchemaTableResult> SchemaResults { get; } = [];

    public bool HasSchemaResults => SchemaResults.Count > 0;

    public string SchemaSummary
    {
        get
        {
            var changed = SchemaResults.Count(item => item.Status is
                DatabaseSchemaTableStatus.Created or DatabaseSchemaTableStatus.Updated);
            var failed = SchemaResults.Count(item => item.Status == DatabaseSchemaTableStatus.Failed);
            return $"共 {SchemaResults.Count} 张表 · 变更 {changed} · 失败 {failed}";
        }
    }

    /// <summary>退出请求事件（MainWindow 订阅后执行登出逻辑）</summary>
    public event Action? LogoutRequested;

    public ICommand CheckDatabaseCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand SaveConnectionStringCommand { get; }
    public ICommand BrowseDataFolderCommand { get; }

    private async Task LoadDataFolderPathAsync()
    {
        try
        {
            var saved = await _configService.LoadAppSettingAsync<string>("DataFolderPath");
            DataFolderPath = saved ?? "(未设置)";
        }
        catch
        {
            DataFolderPath = "(未设置)";
        }
    }

    private async Task BrowseDataFolderAsync()
    {
        var picked = await _fileDialogService.PickFolderAsync("选择资料文件夹");
        if (string.IsNullOrEmpty(picked) == false)
        {
            DataFolderPath = picked;
            _ = _configService.SaveAppSettingAsync("DataFolderPath", DataFolderPath);
            StatusMessage = $"资料文件夹已设置为: {DataFolderPath}";
        }
    }

    private async Task CheckDatabaseAsync()
    {
        IsCheckingDb = true;
        StatusMessage = "正在检查数据库...";
        try
        {
            var result = await _databaseSchemaService.CheckAndSynchronizeAsync();
            SchemaResults.Clear();
            foreach (var item in result.Tables)
                SchemaResults.Add(item);
            OnPropertyChanged(nameof(HasSchemaResults));
            OnPropertyChanged(nameof(SchemaSummary));
            StatusMessage = result.IsSuccess
                ? $"数据库检查完成：{SchemaSummary}"
                : $"数据库检查未全部通过：{result.ErrorMessage ?? SchemaSummary}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"数据库检查失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("设置-数据库检查", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsCheckingDb = false;
        }
    }

    private void OnLogoutRequested()
    {
        LogoutRequested?.Invoke();
    }

    private async Task SaveConnectionStringAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            StatusMessage = "连接字符串不能为空";
            return;
        }

        IsSaving = true;
        StatusMessage = "正在保存...";
        try
        {
            var json = JsonSerializer.Serialize(new { sql = ConnectionString },
                new JsonSerializerOptions { WriteIndented = true });

            // 仅保存到被 Git 忽略的本地配置，避免连接凭据进入版本库。
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DBSeeting.local.json");
            await File.WriteAllTextAsync(outputPath, json);

            // 保存到源码目录中的本地配置（构建后不丢失）。
            var sourcePath = FindSourceConfigPath("DBSeeting.local.json");
            if (sourcePath != null)
            {
                await File.WriteAllTextAsync(sourcePath, json);
            }

            // 刷新 DbContext 缓存
            ArasToolkitDbContext.RefreshConnectionString();

            StatusMessage = "连接字符串已保存";
            await _errorLogService.LogErrorAsync("设置-连接字符串保存", "连接字符串已更新",
                ErrorLog.LevelP1, null);
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("设置-连接字符串保存", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>优先从本地配置读取连接字符串，未配置时回退到仓库默认配置。</summary>
    private static string ReadConnectionString()
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidatePaths = new[]
            {
                Path.Combine(baseDir, "DBSeeting.local.json"),
                Path.Combine(baseDir, "DBSeeting.json"),
                FindSourceConfigPath("DBSeeting.local.json"),
                FindSourceConfigPath("DBSeeting.json")
            };

            var configPath = candidatePaths.FirstOrDefault(path =>
                path != null && File.Exists(path));
            if (configPath == null)
                return "";

            var json = File.ReadAllText(configPath);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("sql").GetString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>向上遍历寻找解决方案根目录，定位指定的源码配置文件。</summary>
    private static string? FindSourceConfigPath(string fileName)
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        for (int i = 0; i < 10; i++)
        {
            if (File.Exists(Path.Combine(dir, "ArasToolkit.slnx")))
                return Path.Combine(dir, "src", "ArasToolkit.Core", fileName);
            var parent = Directory.GetParent(dir);
            if (parent == null) break;
            dir = parent.FullName;
        }
        return null;
    }
}
