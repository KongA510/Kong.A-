using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>“汇入项目计划模板”页面 ViewModel。</summary>
public class ProjectPlanImportViewModel : ObservableObject
{
    private readonly IProjectPlanImportService _projectPlanImportService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;

    private ProjectPlanTemplateDefinition? _previewDefinition;
    private ProjectPlanPreparedImport? _preparedImport;
    private string _selectedFilePath = string.Empty;
    private string _statusMessage = "请依次执行：导出模板 → 填写并上传 → 预检通过后汇入 Aras。";
    private string _errorMessage = string.Empty;
    private string _arasValidationMessage = "尚未执行 Aras 预检";
    private bool _isBusy;

    public ProjectPlanImportViewModel(
        IProjectPlanImportService projectPlanImportService,
        IFileDialogService fileDialogService,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _projectPlanImportService = projectPlanImportService;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _errorLogService = errorLogService;

        DownloadTemplateCommand = new RelayCommand(async _ => await DownloadTemplateAsync(), _ => !IsBusy);
        UploadTemplateCommand = new RelayCommand(async _ => await UploadTemplateAsync(), _ => !IsBusy);
        ValidateArasCommand = new RelayCommand(async _ => await ValidateAgainstArasAsync(), _ => HasPreview && !IsBusy);
        ImportCommand = new RelayCommand(async _ => await ImportAsync(), _ => IsReadyToImport && !IsBusy);
    }

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        private set
        {
            if (SetProperty(ref _selectedFilePath, value))
                OnPropertyChanged(nameof(FileName));
        }
    }

    public string FileName => string.IsNullOrWhiteSpace(SelectedFilePath)
        ? "（未选择文件）"
        : Path.GetFileName(SelectedFilePath);

    public ProjectPlanTemplateDefinition? PreviewDefinition
    {
        get => _previewDefinition;
        private set
        {
            if (!SetProperty(ref _previewDefinition, value)) return;
            OnPropertyChanged(nameof(Nodes));
            OnPropertyChanged(nameof(HasPreview));
            OnPropertyChanged(nameof(TemplateName));
            OnPropertyChanged(nameof(TemplateDescription));
            OnPropertyChanged(nameof(SummaryText));
            OnPropertyChanged(nameof(WarningsText));
            OnPropertyChanged(nameof(HasWarnings));
            RefreshCommands();
        }
    }

    public ObservableCollection<ProjectPlanNode> Nodes => PreviewDefinition?.Nodes ?? [];
    public bool HasPreview => PreviewDefinition != null;
    public bool IsReadyToImport => _preparedImport != null;
    public string TemplateName => PreviewDefinition?.TemplateName ?? string.Empty;
    public string TemplateDescription => PreviewDefinition?.Description ?? string.Empty;
    public string SummaryText => PreviewDefinition == null
        ? "尚未读取项目计划"
        : $"阶段 {PreviewDefinition.PhaseCount} 个，任务 {PreviewDefinition.ActivityCount} 个，里程碑 {PreviewDefinition.MilestoneCount} 个";
    public string WarningsText => PreviewDefinition == null
        ? string.Empty
        : string.Join(Environment.NewLine, PreviewDefinition.Warnings);
    public bool HasWarnings => PreviewDefinition?.Warnings.Count > 0;

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public string ArasValidationMessage
    {
        get => _arasValidationMessage;
        private set => SetProperty(ref _arasValidationMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value)) RefreshCommands();
        }
    }

    public ICommand DownloadTemplateCommand { get; }
    public ICommand UploadTemplateCommand { get; }
    public ICommand ValidateArasCommand { get; }
    public ICommand ImportCommand { get; }

    private async Task DownloadTemplateAsync()
    {
        try
        {
            var path = await _fileDialogService.PickSaveFileAsync("Aras项目计划模板.xlsx", ".xlsx");
            if (string.IsNullOrWhiteSpace(path)) return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            await File.WriteAllBytesAsync(path, _projectPlanImportService.GenerateTemplate());
            StatusMessage = $"项目计划模板已导出：{path}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"导出模板失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("项目计划模板-页面导出", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UploadTemplateAsync()
    {
        try
        {
            var path = await _fileDialogService.PickOpenFileAsync("选择项目计划模板", ".xlsx");
            if (string.IsNullOrWhiteSpace(path)) return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            ArasValidationMessage = "正在校验 Excel 与当前 Aras 实例...";
            SelectedFilePath = path;
            PreviewDefinition = await _projectPlanImportService.ParseTemplateAsync(path);
            InvalidatePreparedImport();
            _preparedImport = await _projectPlanImportService.PrepareImportAsync(PreviewDefinition);
            OnPreparedImportChanged();
            StatusMessage = $"上传与预检完成：{SummaryText}。现在可以汇入 Aras。";
            ArasValidationMessage = "预检通过：标准 Project Management ItemType、同名模板和项目角色均已核对";
        }
        catch (Exception ex)
        {
            InvalidatePreparedImport();
            ErrorMessage = HasPreview
                ? $"Excel 本地校验已完成，但 Aras 预检失败：{ex.Message}"
                : $"上传或解析失败：{ex.Message}";
            ArasValidationMessage = "预检未通过，不允许汇入";
            await _errorLogService.LogErrorAsync("项目计划模板-页面上传预检", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ValidateAgainstArasAsync()
    {
        if (PreviewDefinition == null) return;
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            ArasValidationMessage = "正在重新核对当前 Aras 实例...";
            _preparedImport = await _projectPlanImportService.PrepareImportAsync(PreviewDefinition);
            OnPreparedImportChanged();
            StatusMessage = $"Aras 预检通过：{SummaryText}。";
            ArasValidationMessage = "预检通过：可以安全汇入";
        }
        catch (Exception ex)
        {
            InvalidatePreparedImport();
            ErrorMessage = $"Aras 预检失败：{ex.Message}";
            ArasValidationMessage = "预检未通过，不允许汇入";
            await _errorLogService.LogErrorAsync("项目计划模板-页面重新预检", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ImportAsync()
    {
        if (PreviewDefinition == null || _preparedImport == null) return;
        var confirmed = await _dialogService.ConfirmAsync(
            "确认汇入项目计划模板",
            $"即将在当前 Aras 数据库新增 Project Template“{TemplateName}”。\n\n{SummaryText}\n\n" +
            "系统会按 Aras 项目模型顺序创建 WBS、Activity2、层级关系、角色分配和前置关系；" +
            "任一步失败将回滚本次已建数据。是否继续？",
            "确认汇入",
            "取消");
        if (!confirmed) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = "正在重新预检并汇入 Aras，请勿重复操作...";

            // 在真正写入前再次核对，避免上传后目标数据库或同名模板发生变化。
            _preparedImport = await _projectPlanImportService.PrepareImportAsync(PreviewDefinition);
            var result = await _projectPlanImportService.ImportAsync(_preparedImport, TemplateName);
            InvalidatePreparedImport();
            ArasValidationMessage = "汇入完成；如需再次汇入，请修改模板名称并重新上传";
            StatusMessage = $"{result.Message} Project Template ID：{result.TemplateId}";
            await _dialogService.AlertAsync("汇入完成", StatusMessage);
        }
        catch (Exception ex)
        {
            InvalidatePreparedImport();
            ErrorMessage = $"汇入失败：{ex.Message}";
            ArasValidationMessage = "汇入未完成，请修正问题后重新预检";
            await _errorLogService.LogErrorAsync("项目计划模板-页面汇入", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void InvalidatePreparedImport()
    {
        _preparedImport = null;
        OnPreparedImportChanged();
    }

    private void OnPreparedImportChanged()
    {
        OnPropertyChanged(nameof(IsReadyToImport));
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        (DownloadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (UploadTemplateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ValidateArasCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ImportCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
