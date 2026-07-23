using System.IO;
using System.Text.Json;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Entities;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 设置窗口 ViewModel — 数据库检查 / 退出登录 / 连接字符串管理 / 资料文件夹地址
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private readonly IErrorLogService _errorLogService;
    private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;
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
        IDbContextFactory<ArasToolkitDbContext> contextFactory,
        IConfigService configService,
        IFileDialogService fileDialogService)
    {
        _errorLogService = errorLogService;
        _contextFactory = contextFactory;
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
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.EnsureSchemaAsync();
            StatusMessage = "数据库检查完成，表结构已同步";
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

            // 保存到输出目录（运行时生效）
            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DBSeeting.json");
            await File.WriteAllTextAsync(outputPath, json);

            // 保存到源码目录（构建后不丢失）
            var sourcePath = FindSourceConfigPath();
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

    /// <summary>从 DBSeeting.json 读取当前连接字符串</summary>
    private static string ReadConnectionString()
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, "DBSeeting.json");

            if (!File.Exists(configPath))
            {
                configPath = FindSourceConfigPath();
                if (configPath == null || !File.Exists(configPath))
                    return "";
            }

            var json = File.ReadAllText(configPath);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("sql").GetString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>向上遍历寻找解决方案根目录，定位源码中的 DBSeeting.json</summary>
    private static string? FindSourceConfigPath()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        for (int i = 0; i < 10; i++)
        {
            if (File.Exists(Path.Combine(dir, "ArasToolkit.slnx")))
                return Path.Combine(dir, "src", "ArasToolkit.Core", "DBSeeting.json");
            var parent = Directory.GetParent(dir);
            if (parent == null) break;
            dir = parent.FullName;
        }
        return null;
    }
}
