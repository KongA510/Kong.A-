using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>常用 SQL/AML/XML 片段库页面模型。</summary>
public sealed class CommonQuerySnippetViewModel : ObservableObject
{
    private const string AllTypes = "全部";
    private readonly ICommonQuerySnippetService _service;
    private readonly IDialogService _dialogService;
    private readonly IErrorLogService _errorLogService;
    private CommonQuerySnippet? _selectedSnippet;
    private string _selectedFilterType = AllTypes;
    private string _searchKeyword = string.Empty;
    private string _editingId = string.Empty;
    private string _title = string.Empty;
    private string _contentType = CommonQuerySnippetTypes.Sql;
    private string _description = string.Empty;
    private string _content = string.Empty;
    private string _previewContent = string.Empty;
    private string _statusMessage = "正在加载片段库…";
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public CommonQuerySnippetViewModel(
        ICommonQuerySnippetService service,
        IDialogService dialogService,
        IErrorLogService errorLogService)
    {
        _service = service;
        _dialogService = dialogService;
        _errorLogService = errorLogService;

        RefreshCommand = new RelayCommand(async _ => await LoadAsync(), _ => !IsBusy);
        NewCommand = new RelayCommand(_ => BeginNew(), _ => !IsBusy);
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedSnippet != null && !IsBusy);
        FormatCommand = new RelayCommand(_ => FormatAndApply(), _ => CanFormat());

        _ = LoadAsync();
    }

    public ObservableCollection<CommonQuerySnippet> Snippets { get; } = [];
    public IReadOnlyList<string> FilterTypes { get; } =
        [AllTypes, .. CommonQuerySnippetTypes.All];
    public IReadOnlyList<string> ContentTypes { get; } = CommonQuerySnippetTypes.All;

    public CommonQuerySnippet? SelectedSnippet
    {
        get => _selectedSnippet;
        set
        {
            if (!SetProperty(ref _selectedSnippet, value))
                return;
            if (value != null)
                LoadIntoEditor(value);
            RefreshCommands();
        }
    }

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set
        {
            if (SetProperty(ref _selectedFilterType, value))
                _ = LoadAsync();
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

    public string ContentType
    {
        get => _contentType;
        set
        {
            var normalized = CommonQuerySnippetTypes.Normalize(value);
            if (!SetProperty(ref _contentType, normalized))
                return;
            RefreshPreview();
            OnPropertyChanged(nameof(IsXmlBased));
            RefreshCommands();
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Content
    {
        get => _content;
        set
        {
            if (!SetProperty(ref _content, value))
                return;
            RefreshPreview();
            RefreshCommands();
        }
    }

    public string PreviewContent
    {
        get => _previewContent;
        private set => SetProperty(ref _previewContent, value);
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

    public bool IsXmlBased => CommonQuerySnippetTypes.IsXmlBased(ContentType);
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string LibrarySummary => $"当前 {Snippets.Count} 条";
    public string EditorTitle => string.IsNullOrWhiteSpace(_editingId) ? "新建片段" : "编辑片段";

    public ICommand RefreshCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand FormatCommand { get; }

    public async Task SearchAsync() => await LoadAsync();

    public async Task ReportClipboardErrorAsync(Exception ex)
    {
        ErrorMessage = $"复制失败：{ex.Message}";
        await _errorLogService.LogErrorAsync("常用SQL/AML-复制", ex.Message,
            ErrorLog.LevelP1, ex.StackTrace);
    }

    public void ReportCopied()
    {
        ErrorMessage = string.Empty;
        StatusMessage = "已复制预览内容，可直接粘贴到二次开发位置。";
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var filter = SelectedFilterType == AllTypes ? null : SelectedFilterType;
            var items = await _service.GetAllAsync(filter, SearchKeyword);
            Snippets.Clear();
            foreach (var item in items)
                Snippets.Add(item);
            OnPropertyChanged(nameof(LibrarySummary));
            StatusMessage = items.Count == 0
                ? "没有找到片段。点击“新建片段”保存第一条常用内容。"
                : $"已加载 {items.Count} 条常用片段。";

            if (!string.IsNullOrWhiteSpace(_editingId))
            {
                var current = Snippets.FirstOrDefault(item => item.Id == _editingId);
                if (current != null)
                    SelectedSnippet = current;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("常用SQL/AML-页面加载", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BeginNew()
    {
        SelectedSnippet = null;
        _editingId = string.Empty;
        Title = string.Empty;
        ContentType = CommonQuerySnippetTypes.Sql;
        Description = string.Empty;
        Content = string.Empty;
        PreviewContent = string.Empty;
        ErrorMessage = string.Empty;
        StatusMessage = "已打开空白片段，选择类型并输入内容后保存。";
        OnPropertyChanged(nameof(EditorTitle));
        RefreshCommands();
    }

    private async Task SaveAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var snippet = new CommonQuerySnippet
            {
                Id = _editingId,
                Title = Title,
                ContentType = ContentType,
                Description = Description,
                Content = Content
            };
            await _service.SaveAsync(snippet);
            _editingId = snippet.Id;
            await LoadAsync();
            StatusMessage = $"已保存 {snippet.ContentType} 片段“{snippet.Title}”。";
            OnPropertyChanged(nameof(EditorTitle));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("常用SQL/AML-页面保存", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedSnippet == null)
            return;

        var snippet = SelectedSnippet;
        var confirmed = await _dialogService.ConfirmAsync(
            "删除常用片段",
            $"确定删除 {snippet.ContentType} 片段“{snippet.Title}”吗？",
            "删除");
        if (!confirmed)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _service.DeleteAsync(snippet.Id);
            BeginNew();
            await LoadAsync();
            StatusMessage = $"已删除片段“{snippet.Title}”。";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败：{ex.Message}";
            await _errorLogService.LogErrorAsync("常用SQL/AML-页面删除", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadIntoEditor(CommonQuerySnippet snippet)
    {
        _editingId = snippet.Id;
        Title = snippet.Title;
        ContentType = snippet.ContentType;
        Description = snippet.Description ?? string.Empty;
        Content = snippet.Content;
        ErrorMessage = string.Empty;
        StatusMessage = CommonQuerySnippetTypes.IsXmlBased(snippet.ContentType)
            ? $"已加载“{snippet.Title}”，右侧已自动格式化预览。"
            : $"已加载“{snippet.Title}”。";
        OnPropertyChanged(nameof(EditorTitle));
    }

    private void RefreshPreview()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            PreviewContent = string.Empty;
            return;
        }

        if (!IsXmlBased)
        {
            PreviewContent = Content;
            return;
        }

        PreviewContent = TryFormatXml(Content, out var formatted, out _)
            ? formatted
            : Content;
    }

    private void FormatAndApply()
    {
        ErrorMessage = string.Empty;
        if (!TryFormatXml(Content, out var formatted, out var error))
        {
            ErrorMessage = $"{ContentType} 格式无效：{error}";
            return;
        }

        Content = formatted;
        StatusMessage = $"{ContentType} 已格式化；点击“保存片段”持久化修改。";
    }

    private static bool TryFormatXml(string content, out string formatted, out string error)
    {
        try
        {
            var document = XDocument.Parse(content, LoadOptions.None);
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = document.Declaration == null,
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer, settings))
                document.Save(xmlWriter);
            formatted = writer.ToString().TrimEnd();
            error = string.Empty;
            return true;
        }
        catch (Exception ex) when (ex is XmlException or InvalidOperationException)
        {
            formatted = content;
            error = ex.Message;
            return false;
        }
    }

    private bool CanSave() =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Title) &&
        !string.IsNullOrWhiteSpace(Content);

    private bool CanFormat() =>
        !IsBusy && IsXmlBased && !string.IsNullOrWhiteSpace(Content);

    private void RefreshCommands()
    {
        (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NewCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (FormatCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
