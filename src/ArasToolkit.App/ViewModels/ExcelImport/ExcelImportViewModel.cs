using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Win32;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// Excel导入ViewModel
/// </summary>
public class ExcelImportViewModel : ObservableObject
{
    private readonly IExcelService _excelService;

    private string _selectedFilePath = string.Empty;
    private ObservableCollection<string> _sheetNames = new();
    private string? _selectedSheetName;
    private DataTable? _excelData;
    private ObservableCollection<string> _columnHeaders = new();
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public ExcelImportViewModel(IExcelService excelService)
    {
        _excelService = excelService;

        BrowseFileCommand = new RelayCommand(async _ => await BrowseFileAsync());
        LoadSheetCommand = new RelayCommand(async _ => await LoadSheetAsync(), _ => !string.IsNullOrEmpty(SelectedSheetName));
        LoadAllSheetsCommand = new RelayCommand(async _ => await LoadAllSheetsAsync(), _ => !string.IsNullOrEmpty(SelectedFilePath));
    }

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set => SetProperty(ref _selectedFilePath, value);
    }

    public ObservableCollection<string> SheetNames
    {
        get => _sheetNames;
        set => SetProperty(ref _sheetNames, value);
    }

    public string? SelectedSheetName
    {
        get => _selectedSheetName;
        set
        {
            SetProperty(ref _selectedSheetName, value);
            (LoadSheetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public DataTable? ExcelData
    {
        get => _excelData;
        set => SetProperty(ref _excelData, value);
    }

    public ObservableCollection<string> ColumnHeaders
    {
        get => _columnHeaders;
        set => SetProperty(ref _columnHeaders, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand BrowseFileCommand { get; }
    public ICommand LoadSheetCommand { get; }
    public ICommand LoadAllSheetsCommand { get; }

    private async Task BrowseFileAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "选择Excel文件",
            Filter = "Excel文件|*.xlsx;*.xls|所有文件|*.*",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            SelectedFilePath = openFileDialog.FileName;
            StatusMessage = "正在读取Sheet信息...";
            IsLoading = true;

            try
            {
                var sheets = await _excelService.GetSheetNamesAsync(SelectedFilePath);
                SheetNames = new ObservableCollection<string>(sheets);
                StatusMessage = $"已加载 {sheets.Count} 个Sheet，请选择要读取的Sheet。";
            }
            catch (Exception ex)
            {
                StatusMessage = $"读取失败: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task LoadSheetAsync()
    {
        if (string.IsNullOrEmpty(SelectedSheetName) || string.IsNullOrEmpty(SelectedFilePath))
            return;

        IsLoading = true;
        StatusMessage = $"正在读取 Sheet: {SelectedSheetName}...";

        try
        {
            var sheetData = await _excelService.ReadSheetAsync(SelectedFilePath, SelectedSheetName);
            ExcelData = sheetData.Data;
            ColumnHeaders = sheetData.ColumnHeaders;
            StatusMessage = $"成功读取 Sheet '{SelectedSheetName}'，共 {sheetData.Data.Rows.Count} 行数据。";
        }
        catch (Exception ex)
        {
            StatusMessage = $"读取失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllSheetsAsync()
    {
        if (string.IsNullOrEmpty(SelectedFilePath)) return;

        IsLoading = true;
        StatusMessage = "正在读取所有Sheet...";

        try
        {
            var allData = await _excelService.ReadAllSheetsAsync(SelectedFilePath);
            if (allData.Count > 0)
            {
                // 默认显示第一个Sheet
                ExcelData = allData[0].Data;
                ColumnHeaders = allData[0].ColumnHeaders;
                StatusMessage = $"成功读取所有 Sheet，共 {allData.Count} 个Sheet。当前显示: {allData[0].SheetName}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"读取失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
