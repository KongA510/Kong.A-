using System.Collections.ObjectModel;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>“相关代码记录”主题库与代码段编辑工作台。</summary>
public sealed class RelatedCodeRecordViewModel : ObservableObject
{
    private readonly IRelatedCodeRecordService _service;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;
    private RelatedCodeRecord? _selectedRecord;
    private RelatedCodeSegment? _selectedSegment;
    private RelatedCodeSegment? _openedSegment;
    private string _searchKeyword = string.Empty;
    private string _editingRecordId = string.Empty;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _editingSegmentId = string.Empty;
    private string _segmentName = string.Empty;
    private string _selectedCodeType = RelatedCodeTypes.CSharp;
    private string _segmentDescription = string.Empty;
    private string _codeContent = string.Empty;
    private string _statusMessage = "正在加载相关代码主题…";
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isRecordEditorVisible;
    private bool _isSegmentEditorVisible;
    private int _detailLoadVersion;

    public RelatedCodeRecordViewModel(
        IRelatedCodeRecordService service,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _dialogService = dialogService;
        _errorLogService = errorLogService;

        RefreshCommand = new RelayCommand(async _ => await LoadRecordsAsync(), _ => !IsBusy);
        NewRecordCommand = new RelayCommand(_ => BeginNewRecord(), _ => !IsBusy);
        EditRecordCommand = new RelayCommand(_ => BeginEditRecord(),
            _ => IsRecordSelected && !IsBusy);
        SaveRecordCommand = new RelayCommand(async _ => await SaveRecordAsync(), _ => CanSaveRecord());
        DeleteRecordCommand = new RelayCommand(async _ => await DeleteRecordAsync(),
            _ => IsRecordSelected && !IsBusy);
        CancelRecordCommand = new RelayCommand(_ => CloseRecordEditor(),
            _ => IsRecordEditorVisible && !IsBusy);
        NewSegmentCommand = new RelayCommand(_ => BeginNewSegment(),
            _ => IsRecordSelected && !IsBusy);
        SaveSegmentCommand = new RelayCommand(async _ => await SaveSegmentAsync(),
            _ => CanSaveSegment());
        CancelSegmentCommand = new RelayCommand(_ => CloseSegmentEditor(), _ => IsSegmentEditorVisible);

        _ = LoadRecordsAsync();
    }

    public ObservableCollection<RelatedCodeRecord> Records { get; } = [];
    public ObservableCollection<RelatedCodeSegment> Segments { get; } = [];
    public IReadOnlyList<string> CodeTypes { get; } = RelatedCodeTypes.All;

    public RelatedCodeRecord? SelectedRecord
    {
        get => _selectedRecord;
        set
        {
            if (!SetProperty(ref _selectedRecord, value))
                return;
            Segments.Clear();
            ResetSegmentReader();
            OnPropertyChanged(nameof(HasSegments));
            OnPropertyChanged(nameof(SegmentSummary));
            if (value != null)
                _ = LoadRecordDetailAsync(value.Id);
            RefreshCommands();
        }
    }

    public RelatedCodeSegment? SelectedSegment
    {
        get => _selectedSegment;
        set => SetProperty(ref _selectedSegment, value);
    }

    public RelatedCodeSegment? OpenedSegment
    {
        get => _openedSegment;
        private set
        {
            if (!SetProperty(ref _openedSegment, value))
                return;
            OnPropertyChanged(nameof(HasOpenedSegment));
        }
    }

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
                RefreshCommands();
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string SegmentName
    {
        get => _segmentName;
        set
        {
            if (SetProperty(ref _segmentName, value))
                RefreshCommands();
        }
    }

    public string SelectedCodeType
    {
        get => _selectedCodeType;
        set
        {
            var normalized = RelatedCodeTypes.Normalize(value);
            if (SetProperty(ref _selectedCodeType, normalized))
                OnPropertyChanged(nameof(EditorAccentColor));
        }
    }

    public string SegmentDescription
    {
        get => _segmentDescription;
        set => SetProperty(ref _segmentDescription, value);
    }

    public string CodeContent
    {
        get => _codeContent;
        set
        {
            if (SetProperty(ref _codeContent, value))
                RefreshCommands();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (SetProperty(ref _statusMessage, value))
                OnPropertyChanged(nameof(HasStatus));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
                RefreshCommands();
        }
    }

    public bool IsSegmentEditorVisible
    {
        get => _isSegmentEditorVisible;
        private set
        {
            if (SetProperty(ref _isSegmentEditorVisible, value))
            {
                OnPropertyChanged(nameof(IsCodeWorkspaceVisible));
                RefreshCommands();
            }
        }
    }

    public bool IsRecordEditorVisible
    {
        get => _isRecordEditorVisible;
        private set
        {
            if (SetProperty(ref _isRecordEditorVisible, value))
            {
                OnPropertyChanged(nameof(IsCodeWorkspaceVisible));
                RefreshCommands();
            }
        }
    }

    public bool IsRecordSelected => !string.IsNullOrWhiteSpace(_editingRecordId);
    public bool HasSegments => Segments.Count > 0;
    public bool HasOpenedSegment => OpenedSegment != null;
    public bool IsCodeWorkspaceVisible => !IsRecordEditorVisible && !IsSegmentEditorVisible;
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string LibrarySummary => $"当前 {Records.Count} 个主题";
    public string SegmentSummary => $"共 {Segments.Count} 个代码段";
    public string RecordEditorTitle => IsRecordSelected ? "编辑主题" : "新建主题";
    public string SegmentEditorTitle => string.IsNullOrWhiteSpace(_editingSegmentId)
        ? "新增代码段"
        : "编辑代码段";
    public string EditorAccentColor => SelectedCodeType;

    public ICommand RefreshCommand { get; }
    public ICommand NewRecordCommand { get; }
    public ICommand EditRecordCommand { get; }
    public ICommand SaveRecordCommand { get; }
    public ICommand DeleteRecordCommand { get; }
    public ICommand CancelRecordCommand { get; }
    public ICommand NewSegmentCommand { get; }
    public ICommand SaveSegmentCommand { get; }
    public ICommand CancelSegmentCommand { get; }

    public Task SearchAsync() => LoadRecordsAsync();

    public void OpenSegment(RelatedCodeSegment segment)
    {
        SelectedSegment = segment;
        OpenedSegment = segment;
        CloseRecordEditor();
        CloseSegmentEditor();
        ErrorMessage = string.Empty;
        StatusMessage = $"已打开 {segment.CodeType} 代码段“{segment.SegmentName}”。";
    }

    private void BeginEditRecord()
    {
        if (!IsRecordSelected)
            return;
        CloseSegmentEditor();
        IsRecordEditorVisible = true;
        ErrorMessage = string.Empty;
        StatusMessage = $"正在编辑主题“{Title}”。";
    }

    public void BeginEditSegment(RelatedCodeSegment segment)
    {
        CloseRecordEditor();
        _editingSegmentId = segment.Id;
        SegmentName = segment.SegmentName;
        SelectedCodeType = segment.CodeType;
        SegmentDescription = segment.Description ?? string.Empty;
        CodeContent = segment.CodeContent;
        IsSegmentEditorVisible = true;
        ErrorMessage = string.Empty;
        StatusMessage = $"正在编辑 {segment.CodeType} 代码段“{segment.SegmentName}”。";
        OnPropertyChanged(nameof(SegmentEditorTitle));
    }

    public async Task DeleteSegmentAsync(RelatedCodeSegment segment)
    {
        if (!IsRecordSelected)
            return;
        try
        {
            var confirmed = await _dialogService.ConfirmAsync(
                "删除代码段",
                $"确定删除 {segment.CodeType} 代码段“{segment.SegmentName}”吗？",
                "删除");
            if (!confirmed)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            await _service.DeleteSegmentAsync(_editingRecordId, segment.Id);
            await LoadRecordDetailCoreAsync(_editingRecordId);
            StatusMessage = $"已删除代码段“{segment.SegmentName}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除代码段失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面删除代码段", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task MoveSegmentAsync(RelatedCodeSegment segment, int offset)
    {
        var currentIndex = Segments.IndexOf(segment);
        var targetIndex = currentIndex + offset;
        if (!IsRecordSelected || currentIndex < 0 || targetIndex < 0 || targetIndex >= Segments.Count)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var openedSegmentId = OpenedSegment?.Id;
            var ids = Segments.Select(item => item.Id).ToList();
            (ids[currentIndex], ids[targetIndex]) = (ids[targetIndex], ids[currentIndex]);
            await _service.ReorderSegmentsAsync(_editingRecordId, ids);
            await LoadRecordDetailCoreAsync(_editingRecordId);
            var openedSegment = Segments.FirstOrDefault(item => item.Id == openedSegmentId);
            if (openedSegment != null)
                OpenSegment(openedSegment);
            StatusMessage = "代码段顺序已更新。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"调整顺序失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面调整顺序", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ReportCopied(RelatedCodeSegment segment)
    {
        ErrorMessage = string.Empty;
        StatusMessage = $"已复制 {segment.CodeType} 代码段“{segment.SegmentName}”。";
    }

    public async Task ReportClipboardErrorAsync(Exception ex)
    {
        ErrorMessage = $"复制失败：{ex.Message}";
        await LogUiErrorAsync("相关代码记录-页面复制", ex);
    }

    private async Task LoadRecordsAsync(string? preferredRecordId = null)
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var currentId = preferredRecordId ?? _editingRecordId;
            var items = await _service.GetAllAsync(SearchKeyword);
            Records.Clear();
            foreach (var item in items)
                Records.Add(item);
            OnPropertyChanged(nameof(LibrarySummary));

            var selected = Records.FirstOrDefault(item => item.Id == currentId) ?? Records.FirstOrDefault();
            if (selected == null)
            {
                BeginNewRecord();
                StatusMessage = "没有找到相关代码主题，点击“新建主题”开始记录。";
            }
            else
            {
                _selectedRecord = selected;
                OnPropertyChanged(nameof(SelectedRecord));
                await LoadRecordDetailCoreAsync(selected.Id);
                StatusMessage = $"已加载 {items.Count} 个相关代码主题。";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面加载", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadRecordDetailAsync(string recordId)
    {
        var version = ++_detailLoadVersion;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var record = await _service.GetByIdAsync(recordId);
            if (version != _detailLoadVersion || record == null)
                return;
            ApplyRecordDetail(record);
            StatusMessage = $"已加载主题“{record.Title}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载详情失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面加载详情", ex);
        }
        finally
        {
            if (version == _detailLoadVersion)
                IsBusy = false;
        }
    }

    private async Task LoadRecordDetailCoreAsync(string recordId)
    {
        var record = await _service.GetByIdAsync(recordId)
            ?? throw new InvalidOperationException("相关代码主题不存在或已被删除。");
        ApplyRecordDetail(record);
    }

    private void ApplyRecordDetail(RelatedCodeRecord record)
    {
        _editingRecordId = record.Id;
        Title = record.Title;
        Description = record.Description ?? string.Empty;
        Segments.Clear();
        foreach (var segment in record.Segments.OrderBy(item => item.SortOrder))
            Segments.Add(segment);
        ResetSegmentReader();
        CloseRecordEditor();
        CloseSegmentEditor();
        NotifyRecordStateChanged();
    }

    private void BeginNewRecord()
    {
        _detailLoadVersion++;
        _selectedRecord = null;
        OnPropertyChanged(nameof(SelectedRecord));
        _editingRecordId = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        Segments.Clear();
        ResetSegmentReader();
        CloseSegmentEditor();
        IsRecordEditorVisible = true;
        ErrorMessage = string.Empty;
        StatusMessage = "已打开空白主题，填写标题和说明后保存。";
        NotifyRecordStateChanged();
    }

    private async Task SaveRecordAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var record = new RelatedCodeRecord
            {
                Id = _editingRecordId,
                Title = Title,
                Description = Description
            };
            await _service.SaveRecordAsync(record);
            await LoadRecordsAsync(record.Id);
            StatusMessage = $"已保存相关代码主题“{record.Title}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存主题失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面保存主题", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteRecordAsync()
    {
        if (!IsRecordSelected)
            return;
        try
        {
            var title = Title;
            var confirmed = await _dialogService.ConfirmAsync(
                "删除相关代码主题",
                $"确定删除“{title}”及其全部 {Segments.Count} 个代码段吗？",
                "删除");
            if (!confirmed)
                return;

            IsBusy = true;
            ErrorMessage = string.Empty;
            await _service.DeleteRecordAsync(_editingRecordId);
            BeginNewRecord();
            await LoadRecordsAsync();
            StatusMessage = $"已删除相关代码主题“{title}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除主题失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面删除主题", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BeginNewSegment()
    {
        CloseRecordEditor();
        _editingSegmentId = string.Empty;
        SegmentName = string.Empty;
        SelectedCodeType = RelatedCodeTypes.CSharp;
        SegmentDescription = string.Empty;
        CodeContent = string.Empty;
        IsSegmentEditorVisible = true;
        ErrorMessage = string.Empty;
        StatusMessage = "请选择代码类型并填写代码段内容。";
        OnPropertyChanged(nameof(SegmentEditorTitle));
    }

    private async Task SaveSegmentAsync()
    {
        if (!IsRecordSelected)
            return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var segment = new RelatedCodeSegment
            {
                Id = _editingSegmentId,
                SegmentName = SegmentName,
                CodeType = SelectedCodeType,
                Description = SegmentDescription,
                CodeContent = CodeContent
            };
            var savedSegment = await _service.SaveSegmentAsync(_editingRecordId, segment);
            await LoadRecordDetailCoreAsync(_editingRecordId);
            var openedSegment = Segments.FirstOrDefault(item => item.Id == savedSegment.Id);
            if (openedSegment != null)
                OpenSegment(openedSegment);
            StatusMessage = $"已保存 {segment.CodeType} 代码段“{segment.SegmentName}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存代码段失败：{ex.Message}";
            await LogUiErrorAsync("相关代码记录-页面保存代码段", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CloseSegmentEditor()
    {
        _editingSegmentId = string.Empty;
        SegmentName = string.Empty;
        SelectedCodeType = RelatedCodeTypes.CSharp;
        SegmentDescription = string.Empty;
        CodeContent = string.Empty;
        IsSegmentEditorVisible = false;
        OnPropertyChanged(nameof(SegmentEditorTitle));
    }

    private void CloseRecordEditor()
    {
        IsRecordEditorVisible = false;
    }

    private void ResetSegmentReader()
    {
        SelectedSegment = null;
        OpenedSegment = null;
    }

    private bool CanSaveRecord() =>
        !IsBusy && !string.IsNullOrWhiteSpace(Title);

    private bool CanSaveSegment() =>
        IsRecordSelected &&
        IsSegmentEditorVisible &&
        !IsBusy &&
        !string.IsNullOrWhiteSpace(SegmentName) &&
        !string.IsNullOrWhiteSpace(CodeContent);

    private void NotifyRecordStateChanged()
    {
        OnPropertyChanged(nameof(IsRecordSelected));
        OnPropertyChanged(nameof(HasSegments));
        OnPropertyChanged(nameof(SegmentSummary));
        OnPropertyChanged(nameof(RecordEditorTitle));
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NewRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (EditRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelRecordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NewSegmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveSegmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CancelSegmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private Task LogUiErrorAsync(string functionName, Exception ex) =>
        _errorLogService.LogErrorAsync(functionName, ex.Message, ErrorLog.LevelP1, ex.StackTrace);
}
