using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 文件浏览器 ViewModel — 懒加载文件夹导航 + 模糊搜索 + 文件上传
/// </summary>
public class FileExplorerViewModel : ObservableObject
{
    private readonly IFileExplorerService _fileService;
    private readonly IConfigService _configService;
    private readonly IFileDialogService _fileDialogService;

    private readonly Stack<string> _navigationStack = new();
    private CancellationTokenSource? _searchCts;
    private string _currentPath = "";
    private string _searchText = "";
    private string _statusMessage = "";
    private bool _isLoading;

    public FileExplorerViewModel(IFileExplorerService fileService, IConfigService configService, IFileDialogService fileDialogService)
    {
        _fileService = fileService;
        _configService = configService;
        _fileDialogService = fileDialogService;

        CurrentItems = new ObservableCollection<FileSystemItem>();

        NavigateIntoCommand = new RelayCommand(item => NavigateInto((FileSystemItem)item!));
        NavigateBackCommand = new RelayCommand(_ => NavigateBack(), _ => CanGoBack);
        OpenFileCommand = new RelayCommand(item => _fileService.OpenFile(((FileSystemItem)item!).FullPath));
        OpenInExplorerCommand = new RelayCommand(item =>
        {
            var i = (FileSystemItem)item!;
            _fileService.OpenInExplorer(i.IsDirectory ? i.FullPath : i.FullPath);
        });
        RefreshCommand = new RelayCommand(async _ => await LoadCurrentPathAsync());
        BrowseFolderCommand = new RelayCommand(async _ => await BrowseFolderAsync());
        UploadFileCommand = new RelayCommand(async _ => await UploadFileAsync());

        _ = InitializeAsync();
    }

    public string CurrentPath
    {
        get => _currentPath;
        set
        {
            if (SetProperty(ref _currentPath, value))
            {
                OnPropertyChanged(nameof(BreadcrumbPath));
                _ = LoadCurrentPathAsync();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                OnPropertyChanged(nameof(IsSearchActive));
                if (string.IsNullOrWhiteSpace(value))
                {
                    _ = LoadCurrentPathAsync();
                }
                else
                {
                    DebounceSearch();
                }
            }
        }
    }

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool CanGoBack => _navigationStack.Count > 0;
    public bool IsSearchActive => !string.IsNullOrWhiteSpace(SearchText);
    public string BreadcrumbPath => CurrentPath;

    public ObservableCollection<FileSystemItem> CurrentItems { get; }

    public ICommand NavigateIntoCommand { get; }
    public ICommand NavigateBackCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand OpenInExplorerCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand BrowseFolderCommand { get; }
    public ICommand UploadFileCommand { get; }

    private async Task InitializeAsync()
    {
        var savedPath = await _configService.LoadAppSettingAsync<string>("DataFolderPath");
        if (!string.IsNullOrWhiteSpace(savedPath) && _fileService.DirectoryExists(savedPath))
        {
            CurrentPath = savedPath;
        }
        else
        {
            CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StatusMessage = "未配置资料文件夹，使用默认文档目录";
        }
    }

    private void NavigateInto(FileSystemItem item)
    {
        if (!item.IsDirectory) return;

        _navigationStack.Push(CurrentPath);
        CurrentPath = item.FullPath;
        OnPropertyChanged(nameof(CanGoBack));
        ((RelayCommand)NavigateBackCommand).RaiseCanExecuteChanged();
    }

    private void NavigateBack()
    {
        if (_navigationStack.Count == 0) return;

        _searchText = "";
        OnPropertyChanged(nameof(SearchText));
        OnPropertyChanged(nameof(IsSearchActive));

        CurrentPath = _navigationStack.Pop();
        OnPropertyChanged(nameof(CanGoBack));
        ((RelayCommand)NavigateBackCommand).RaiseCanExecuteChanged();
    }

    private async Task LoadCurrentPathAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPath)) return;

        IsLoading = true;
        try
        {
            _searchCts?.Cancel();
            _searchCts = null;

            List<FileSystemItem> items;
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                items = await _fileService.GetDirectoryContentsAsync(CurrentPath);
            }
            else
            {
                _searchCts = new CancellationTokenSource();
                items = await _fileService.SearchAsync(CurrentPath, SearchText, _searchCts.Token);
            }

            CurrentItems.Clear();
            foreach (var item in items)
                CurrentItems.Add(item);

            StatusMessage = string.IsNullOrWhiteSpace(SearchText)
                ? $"{CurrentPath} — 共 {items.Count} 个项目"
                : $"搜索「{SearchText}」— 找到 {items.Count} 个结果";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void DebounceSearch()
    {
        var term = SearchText;
        await Task.Delay(300);

        if (SearchText == term)
        {
            await LoadCurrentPathAsync();
        }
    }

    private async Task BrowseFolderAsync()
    {
        var picked = await _fileDialogService.PickFolderAsync("选择文件夹");
        if (string.IsNullOrEmpty(picked) == false)
        {
            _navigationStack.Clear();
            CurrentPath = picked;
            OnPropertyChanged(nameof(CanGoBack));
        }
    }

    private async Task UploadFileAsync()
    {
        var pickedFiles = await _fileDialogService.PickOpenFilesAsync("选择要上传的文件");

        if (pickedFiles.Count == 0)
            return;

        IsLoading = true;
        StatusMessage = "正在上传文件...";

        try
        {
            int success = 0;
            foreach (var file in pickedFiles)
            {
                try
                {
                    await _fileService.CopyFileAsync(file, CurrentPath);
                    success++;
                }
                catch (Exception ex)
                {
                    DebugWrite($"上传失败: {file} - {ex.Message}");
                }
            }

            StatusMessage = $"上传完成: {success}/{pickedFiles.Count} 个文件";
            await LoadCurrentPathAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"上传失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static void DebugWrite(string msg)
    {
        System.Diagnostics.Debug.WriteLine($"[FileExplorerVM] {msg}");
    }
}
