using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 个人资料库 ViewModel — 笔记列表、搜索、CRUD、FlowDocument 回调
/// </summary>
public class KnowledgeViewModel : ObservableObject
{
    private readonly IKnowledgeService _knowledgeService;
    private readonly IErrorLogService _errorLogService;

    public KnowledgeViewModel(IKnowledgeService knowledgeService, IErrorLogService errorLogService)
    {
        _knowledgeService = knowledgeService;
        _errorLogService = errorLogService;

        SearchCommand = new RelayCommand(async _ => await LoadEntriesAsync());
        NewEntryCommand = new RelayCommand(_ => CreateNewEntry());
        OpenEntryCommand = new RelayCommand(async p => await OpenEntryAsync(p as KnowledgeEntry));
        SaveEntryCommand = new RelayCommand(async _ => await SaveEntryAsync(), _ => IsEditorOpen);
        DeleteEntryCommand = new RelayCommand(async p => await DeleteEntryAsync(p as KnowledgeEntry));
        CloseEditorCommand = new RelayCommand(_ => CloseEditor());
        InsertImageCommand = new RelayCommand(_ => OnInsertImageRequested?.Invoke());
        PinToggleCommand = new RelayCommand(async p => await TogglePinAsync(p as KnowledgeEntry));

        _ = LoadEntriesAsync();
    }

    // ===== 列表属性 =====

    private ObservableCollection<KnowledgeEntry> _entries = [];
    public ObservableCollection<KnowledgeEntry> Entries
    {
        get => _entries;
        set => SetProperty(ref _entries, value);
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(PageInfo)); }
    }

    public string PageInfo => $"共 {TotalCount} 条";

    private KnowledgeEntry? _selectedEntry;
    public KnowledgeEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            if (SetProperty(ref _selectedEntry, value))
            {
                if (_selectedEntry?.IsSelected == false)
                {
                    _selectedEntry.IsSelected = true;
                }
            }
        }
    }

    private string _searchKeyword = "";
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            if (SetProperty(ref _searchKeyword, value))
            {
                if (string.IsNullOrWhiteSpace(value))
                    _ = LoadEntriesAsync();
                else
                    DebounceSearch();
            }
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // ===== 编辑器属性 =====

    private bool _isEditorOpen;
    public bool IsEditorOpen
    {
        get => _isEditorOpen;
        set
        {
            SetProperty(ref _isEditorOpen, value);
            (SaveEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private string _editorTitle = "";
    public string EditorTitle
    {
        get => _editorTitle;
        set { SetProperty(ref _editorTitle, value); IsEditorDirty = true; }
    }

    private string _editorTags = "";
    public string EditorTags
    {
        get => _editorTags;
        set { SetProperty(ref _editorTags, value); IsEditorDirty = true; }
    }

    private string _editorCategory = "";
    public string EditorCategory
    {
        get => _editorCategory;
        set { SetProperty(ref _editorCategory, value); IsEditorDirty = true; }
    }

    private bool _editorIsPinned;
    public bool EditorIsPinned
    {
        get => _editorIsPinned;
        set { SetProperty(ref _editorIsPinned, value); IsEditorDirty = true; }
    }

    private bool _isEditorDirty;
    public bool IsEditorDirty
    {
        get => _isEditorDirty;
        set => SetProperty(ref _isEditorDirty, value);
    }

    /// <summary>是否为新建模式（编辑已有 vs 新建）</summary>
    public bool IsNewEntry => _editingEntry == null;

    private KnowledgeEntry? _editingEntry;
    public KnowledgeEntry? EditingEntry
    {
        get => _editingEntry;
        set { SetProperty(ref _editingEntry, value); OnPropertyChanged(nameof(IsNewEntry)); }
    }

    // ===== 命令 =====

    public ICommand SearchCommand { get; }
    public ICommand NewEntryCommand { get; }
    public ICommand OpenEntryCommand { get; }
    public ICommand SaveEntryCommand { get; }
    public ICommand DeleteEntryCommand { get; }
    public ICommand CloseEditorCommand { get; }
    public ICommand InsertImageCommand { get; }
    public ICommand PinToggleCommand { get; }

    // ===== FlowDocument 回调（View 层注册） =====

    /// <summary>保存时 View 层回传 RichTextBox.Document</summary>
    public Func<FlowDocument?>? OnRequestDocument { get; set; }

    /// <summary>加载时 View 层设置 RichTextBox.Document</summary>
    public Action<FlowDocument>? OnLoadDocument { get; set; }

    /// <summary>插入图片 — View 层打开文件选择器并插入到 RichTextBox</summary>
    public Action? OnInsertImageRequested { get; set; }

    /// <summary>编辑器内容变更回调（View 层注册）</summary>
    public Action? OnEditorContentChanged { get; set; }

    // ===== 搜索防抖 =====

    private CancellationTokenSource? _searchCts;

    private async void DebounceSearch()
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;
        var term = SearchKeyword;

        try
        {
            await Task.Delay(300, token);
            if (!token.IsCancellationRequested && SearchKeyword == term)
                await LoadEntriesAsync();
        }
        catch (TaskCanceledException) { }
    }

    // ===== 数据加载 =====

    private async Task LoadEntriesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "";

            var (items, total) = await _knowledgeService.GetPagedAsync(
                1, 200, SearchKeyword);

            // 清除旧选中状态
            foreach (var e in Entries) e.IsSelected = false;

            Entries = new ObservableCollection<KnowledgeEntry>(items);
            TotalCount = total;
            StatusMessage = total > 0 ? $"已加载 {total} 条笔记" : "暂无笔记，点击「新建」开始记录";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-加载列表", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ===== CRUD 操作 =====

    private void CreateNewEntry()
    {
        if (IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        EditingEntry = null;
        EditorTitle = "";
        EditorTags = "";
        EditorCategory = "";
        EditorIsPinned = false;
        IsEditorDirty = false;

        // 清空 RichTextBox — View 层通过 OnLoadDocument 传入空 FlowDocument
        OnLoadDocument?.Invoke(new FlowDocument());
        IsEditorOpen = true;
    }

    private async Task OpenEntryAsync(KnowledgeEntry? entry)
    {
        if (entry == null) return;

        // 如果编辑器正在编辑中且有未保存更改
        if (IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        try
        {
            IsLoading = true;

            // 从 DB 获取完整内容（列表项不含 Content 以减少内存）
            var fullEntry = await _knowledgeService.GetByIdAsync(entry.Id);
            if (fullEntry == null)
            {
                StatusMessage = "笔记不存在或已被删除";
                await LoadEntriesAsync();
                return;
            }

            EditingEntry = fullEntry;
            EditorTitle = fullEntry.Title;
            EditorTags = fullEntry.Tags ?? "";
            EditorCategory = fullEntry.Category ?? "";
            EditorIsPinned = fullEntry.Pinned;
            IsEditorDirty = false;

            // 选中高亮
            foreach (var e in Entries) e.IsSelected = e.Id == fullEntry.Id;
            SelectedEntry = Entries.FirstOrDefault(e => e.Id == fullEntry.Id);

            // 反序列化 FlowDocument
            var document = DeserializeFlowDocument(fullEntry.Content);
            OnLoadDocument?.Invoke(document);
            IsEditorOpen = true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"打开笔记失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-打开笔记", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveEntryAsync()
    {
        if (string.IsNullOrWhiteSpace(EditorTitle))
        {
            StatusMessage = "请输入笔记标题";
            return;
        }

        try
        {
            IsLoading = true;

            // 从 RichTextBox 获取 FlowDocument XAML
            var document = OnRequestDocument?.Invoke();
            var content = document != null
                ? System.Windows.Markup.XamlWriter.Save(document)
                : "";

            if (EditingEntry != null)
            {
                // 更新已有笔记
                EditingEntry.Title = EditorTitle;
                EditingEntry.Content = content;
                EditingEntry.Tags = EditorTags;
                EditingEntry.Category = EditorCategory;
                EditingEntry.Pinned = EditorIsPinned;

                await _knowledgeService.UpdateAsync(EditingEntry);
                StatusMessage = $"已更新: {EditorTitle}";
            }
            else
            {
                // 新建笔记
                var entry = new KnowledgeEntry
                {
                    Title = EditorTitle,
                    Content = content,
                    Tags = EditorTags,
                    Category = EditorCategory,
                    Pinned = EditorIsPinned
                };

                await _knowledgeService.AddAsync(entry);
                StatusMessage = $"已保存: {EditorTitle}";
            }

            IsEditorDirty = false;
            IsEditorOpen = false;
            EditingEntry = null;

            await LoadEntriesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-保存笔记", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteEntryAsync(KnowledgeEntry? entry)
    {
        if (entry == null) return;

        var result = MessageBox.Show(
            $"确定要删除「{entry.Title}」吗？\n此操作不可撤销。",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;

            await _knowledgeService.DeleteAsync(entry.Id);

            // 如果正在编辑被删除的笔记，关闭编辑器
            if (EditingEntry?.Id == entry.Id)
            {
                CloseEditor();
            }

            StatusMessage = $"已删除: {entry.Title}";
            await LoadEntriesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"删除失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-删除笔记", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CloseEditor()
    {
        if (IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        IsEditorOpen = false;
        EditingEntry = null;
        IsEditorDirty = false;

        // 清除选中
        foreach (var e in Entries) e.IsSelected = false;
        SelectedEntry = null;
    }

    private async Task TogglePinAsync(KnowledgeEntry? entry)
    {
        if (entry == null) return;

        try
        {
            var fullEntry = await _knowledgeService.GetByIdAsync(entry.Id);
            if (fullEntry == null) return;

            fullEntry.Pinned = !fullEntry.Pinned;
            await _knowledgeService.UpdateAsync(fullEntry);
            await LoadEntriesAsync();
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-置顶切换", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    // ===== FlowDocument 序列化/反序列化 =====

    /// <summary>
    /// 从 XAML 字符串反序列化 FlowDocument
    /// </summary>
    private static FlowDocument DeserializeFlowDocument(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return new FlowDocument();

        try
        {
            return (FlowDocument)System.Windows.Markup.XamlReader.Parse(content);
        }
        catch
        {
            // 如果 XAML 解析失败，返回包含原始文本的空文档
            var doc = new FlowDocument();
            var plainText = System.Text.RegularExpressions.Regex.Replace(content, @"<[^>]+>", " ");
            doc.Blocks.Add(new Paragraph(new Run(plainText.Trim())));
            return doc;
        }
    }
}
