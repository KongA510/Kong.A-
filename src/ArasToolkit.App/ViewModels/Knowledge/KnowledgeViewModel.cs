using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.App.ViewModels;

/// <summary>
/// 个人资料库三态视图模式
/// </summary>
public enum KnowledgeViewMode
{
    /// <summary>浏览态 — 分类列表 + 笔记卡片</summary>
    Browse,
    /// <summary>查看态 — 只读 FlowDocument 预览</summary>
    View,
    /// <summary>编辑态 — RichTextBox 富文本编辑器</summary>
    Edit
}

/// <summary>
/// 个人资料库 ViewModel — 三态工作流：浏览 → 查看 → 编辑
/// </summary>
public class KnowledgeViewModel : ObservableObject
{
    private readonly IKnowledgeService _knowledgeService;
    private readonly IErrorLogService _errorLogService;

    public KnowledgeViewModel(IKnowledgeService knowledgeService, IErrorLogService errorLogService)
    {
        _knowledgeService = knowledgeService;
        _errorLogService = errorLogService;

        // 浏览态命令
        SelectCategoryCommand = new RelayCommand(async p => await SelectCategoryAsync(p as string));
        NewEntryCommand = new RelayCommand(_ => CreateNewEntry());
        SearchCommand = new RelayCommand(async _ => await SearchEntriesAsync());

        // 查看态命令
        ViewEntryCommand = new RelayCommand(async p => await ViewEntryAsync(p as KnowledgeEntry));
        EnterEditCommand = new RelayCommand(_ => EnterEditMode());
        BackToBrowseCommand = new RelayCommand(_ => BackToBrowse());
        DeleteFromViewCommand = new RelayCommand(async _ => await DeleteFromViewAsync());

        // 编辑态命令
        SaveEntryCommand = new RelayCommand(async _ => await SaveEntryAsync(), _ => IsEditMode);
        CancelEditCommand = new RelayCommand(_ => CancelEdit());
        InsertImageCommand = new RelayCommand(_ => OnInsertImageRequested?.Invoke());
        PinToggleCommand = new RelayCommand(async p => await TogglePinAsync(p as KnowledgeEntry));

        // 初始化加载
        _ = LoadCategoriesAsync();
    }

    // ===== 模式状态 =====

    private KnowledgeViewMode _viewMode = KnowledgeViewMode.Browse;
    public KnowledgeViewMode ViewMode
    {
        get => _viewMode;
        set
        {
            if (SetProperty(ref _viewMode, value))
            {
                OnPropertyChanged(nameof(IsBrowseMode));
                OnPropertyChanged(nameof(IsViewMode));
                OnPropertyChanged(nameof(IsEditMode));
                (SaveEntryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsBrowseMode => ViewMode == KnowledgeViewMode.Browse;
    public bool IsViewMode => ViewMode == KnowledgeViewMode.View;
    public bool IsEditMode => ViewMode == KnowledgeViewMode.Edit;

    // ===== 分类列表属性 =====

    private ObservableCollection<string> _categories = ["全部"];
    public ObservableCollection<string> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    private string _selectedCategory = "全部";
    public string SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    // ===== 笔记列表属性 =====

    private ObservableCollection<KnowledgeEntry> _notesInCategory = [];
    public ObservableCollection<KnowledgeEntry> NotesInCategory
    {
        get => _notesInCategory;
        set => SetProperty(ref _notesInCategory, value);
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set { SetProperty(ref _totalCount, value); OnPropertyChanged(nameof(PageInfo)); }
    }

    public string PageInfo => $"共 {TotalCount} 条";

    private KnowledgeEntry? _selectedNote;
    public KnowledgeEntry? SelectedNote
    {
        get => _selectedNote;
        set
        {
            if (SetProperty(ref _selectedNote, value))
            {
                // 清除其他条目的选中状态
                foreach (var e in NotesInCategory)
                    e.IsSelected = e.Id == value?.Id;
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
                    _ = LoadNotesByCategoryAsync();
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

    // ===== 查看态属性 =====

    private KnowledgeEntry? _viewingEntry;
    public KnowledgeEntry? ViewingEntry
    {
        get => _viewingEntry;
        set
        {
            SetProperty(ref _viewingEntry, value);
            OnPropertyChanged(nameof(IsNewEntry));
        }
    }

    /// <summary>查看态的只读文档预览（纯文本 + 图片渲染）</summary>
    private FlowDocument? _viewDocument;
    public FlowDocument? ViewDocument
    {
        get => _viewDocument;
        set => SetProperty(ref _viewDocument, value);
    }

    // ===== 编辑态属性 =====

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

    /// <summary>是否为新建模式</summary>
    public bool IsNewEntry => _editingEntry == null;

    private KnowledgeEntry? _editingEntry;
    public KnowledgeEntry? EditingEntry
    {
        get => _editingEntry;
        set { SetProperty(ref _editingEntry, value); OnPropertyChanged(nameof(IsNewEntry)); }
    }

    // ===== 命令 =====

    public ICommand SelectCategoryCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand NewEntryCommand { get; }
    public ICommand ViewEntryCommand { get; }
    public ICommand EnterEditCommand { get; }
    public ICommand BackToBrowseCommand { get; }
    public ICommand DeleteFromViewCommand { get; }
    public ICommand SaveEntryCommand { get; }
    public ICommand CancelEditCommand { get; }
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

    // ===== 分类加载 =====

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _knowledgeService.GetCategoriesAsync();
            var list = new ObservableCollection<string> { "全部" };
            foreach (var c in categories)
                list.Add(c);
            Categories = list;

            // 加载全部笔记
            await LoadNotesByCategoryAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载分类失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-加载分类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    // ===== 浏览态操作 =====

    private async Task SelectCategoryAsync(string? category)
    {
        if (category == null) return;

        SelectedCategory = category;
        SearchKeyword = "";
        await LoadNotesByCategoryAsync();
    }

    private async Task LoadNotesByCategoryAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "";

            var category = SelectedCategory == "全部" ? null : SelectedCategory;
            var items = await _knowledgeService.GetByCategoryAsync(category);

            NotesInCategory = new ObservableCollection<KnowledgeEntry>(items);
            TotalCount = items.Count;
            StatusMessage = items.Count > 0
                ? $"'{SelectedCategory}' 下共 {items.Count} 条笔记"
                : $"'{SelectedCategory}' 下暂无笔记";
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-加载分类笔记", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchEntriesAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchKeyword))
        {
            await LoadNotesByCategoryAsync();
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "";

            var (items, total) = await _knowledgeService.GetPagedAsync(1, 200, SearchKeyword);

            NotesInCategory = new ObservableCollection<KnowledgeEntry>(items);
            TotalCount = total;
            StatusMessage = total > 0
                ? $"搜索 [{SearchKeyword}] 找到 {total} 条"
                : $"未找到 [{SearchKeyword}] 相关笔记";
        }
        catch (Exception ex)
        {
            StatusMessage = $"搜索失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-搜索", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

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
        // 新建时预填当前选中的分类
        EditorCategory = SelectedCategory == "全部" ? "" : SelectedCategory;
        EditorIsPinned = false;
        IsEditorDirty = false;
        ViewDocument = null;

        OnLoadDocument?.Invoke(new FlowDocument());
        ViewMode = KnowledgeViewMode.Edit;
    }

    // ===== 查看态操作 =====

    private async Task ViewEntryAsync(KnowledgeEntry? entry)
    {
        if (entry == null) return;

        if (IsEditMode && IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        try
        {
            IsLoading = true;

            var fullEntry = await _knowledgeService.GetByIdAsync(entry.Id);
            if (fullEntry == null)
            {
                StatusMessage = "笔记不存在或已被删除";
                await LoadNotesByCategoryAsync();
                return;
            }

            ViewingEntry = fullEntry;

            // 反序列化 FlowDocument 用于只读预览
            ViewDocument = DeserializeFlowDocument(fullEntry.Content);

            // 选中高亮
            SelectedNote = NotesInCategory.FirstOrDefault(e => e.Id == fullEntry.Id);

            ViewMode = KnowledgeViewMode.View;
        }
        catch (Exception ex)
        {
            StatusMessage = $"打开笔记失败: {ex.Message}";
            await _errorLogService.LogErrorAsync("资料库-查看笔记", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void EnterEditMode()
    {
        if (ViewingEntry == null) return;

        EditingEntry = ViewingEntry;
        EditorTitle = ViewingEntry.Title;
        EditorTags = ViewingEntry.Tags ?? "";
        EditorCategory = ViewingEntry.Category ?? "";
        EditorIsPinned = ViewingEntry.Pinned;
        IsEditorDirty = false;

        // 将查看态的 FlowDocument 加载到编辑器 RichTextBox
        var doc = ViewDocument != null
            ? DeserializeFlowDocument(
                System.Windows.Markup.XamlWriter.Save(ViewDocument))
            : new FlowDocument();
        OnLoadDocument?.Invoke(doc);

        ViewMode = KnowledgeViewMode.Edit;
    }

    private void BackToBrowse()
    {
        if (IsEditMode && IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        ViewingEntry = null;
        ViewDocument = null;
        EditingEntry = null;
        IsEditorDirty = false;
        ViewMode = KnowledgeViewMode.Browse;
    }

    private async Task DeleteFromViewAsync()
    {
        if (ViewingEntry == null) return;

        var result = MessageBox.Show(
            $"确定要删除「{ViewingEntry.Title}」吗？\n此操作不可撤销。",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;

            await _knowledgeService.DeleteAsync(ViewingEntry.Id);
            StatusMessage = $"已删除: {ViewingEntry.Title}";

            ViewingEntry = null;
            ViewDocument = null;
            ViewMode = KnowledgeViewMode.Browse;

            await LoadCategoriesAsync();
            await LoadNotesByCategoryAsync();
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

    // ===== 编辑态操作 =====

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

            var document = OnRequestDocument?.Invoke();
            var content = document != null
                ? System.Windows.Markup.XamlWriter.Save(document)
                : "";

            KnowledgeEntry savedEntry;

            if (EditingEntry != null)
            {
                // 更新已有笔记
                EditingEntry.Title = EditorTitle;
                EditingEntry.Content = content;
                EditingEntry.Tags = EditorTags;
                EditingEntry.Category = EditorCategory;
                EditingEntry.Pinned = EditorIsPinned;

                savedEntry = await _knowledgeService.UpdateAsync(EditingEntry);
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

                savedEntry = await _knowledgeService.AddAsync(entry);
                StatusMessage = $"已保存: {EditorTitle}";
            }

            IsEditorDirty = false;
            EditingEntry = null;

            // 刷新分类列表（可能有新分类）
            await LoadCategoriesAsync();

            // 切换到查看态显示刚保存的内容
            var fullEntry = await _knowledgeService.GetByIdAsync(savedEntry.Id);
            if (fullEntry != null)
            {
                ViewingEntry = fullEntry;
                ViewDocument = DeserializeFlowDocument(fullEntry.Content);
                // 更新列表中对应条目
                var listItem = NotesInCategory.FirstOrDefault(e => e.Id == fullEntry.Id);
                if (listItem != null)
                {
                    listItem.Title = fullEntry.Title;
                    listItem.PlainTextPreview = fullEntry.PlainTextPreview;
                    listItem.Tags = fullEntry.Tags;
                    listItem.Category = fullEntry.Category;
                    listItem.Pinned = fullEntry.Pinned;
                    listItem.ModifiedDate = fullEntry.ModifiedDate;
                }
                else
                {
                    // 新建的笔记，插入列表
                    NotesInCategory.Insert(0, new KnowledgeEntry
                    {
                        Id = fullEntry.Id,
                        Title = fullEntry.Title,
                        PlainTextPreview = fullEntry.PlainTextPreview,
                        Tags = fullEntry.Tags,
                        Category = fullEntry.Category,
                        Pinned = fullEntry.Pinned,
                        CreatedDate = fullEntry.CreatedDate,
                        ModifiedDate = fullEntry.ModifiedDate,
                        CreatorOn = fullEntry.CreatorOn,
                        UserId = fullEntry.UserId
                    });
                }
            }

            ViewMode = KnowledgeViewMode.View;
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

    private void CancelEdit()
    {
        if (IsEditorDirty)
        {
            var result = MessageBox.Show("当前编辑内容尚未保存，是否放弃？", "未保存的更改",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
        }

        IsEditorDirty = false;

        if (IsNewEntry)
        {
            // 新建模式：回到浏览态
            EditingEntry = null;
            ViewingEntry = null;
            ViewDocument = null;
            ViewMode = KnowledgeViewMode.Browse;
        }
        else
        {
            // 编辑已有笔记：回到查看态
            EditingEntry = null;
            ViewMode = KnowledgeViewMode.View;
        }
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

            // 更新列表中对应条目
            var listItem = NotesInCategory.FirstOrDefault(e => e.Id == entry.Id);
            if (listItem != null)
                listItem.Pinned = fullEntry.Pinned;

            if (ViewingEntry?.Id == entry.Id)
                ViewingEntry.Pinned = fullEntry.Pinned;

            // 重新排序
            var sorted = NotesInCategory
                .OrderByDescending(e => e.Pinned)
                .ThenByDescending(e => e.CreatorOn)
                .ToList();
            NotesInCategory = new ObservableCollection<KnowledgeEntry>(sorted);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("资料库-置顶切换", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

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
                await SearchEntriesAsync();
        }
        catch (TaskCanceledException) { }
    }

    // ===== FlowDocument 序列化/反序列化 =====

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
            var doc = new FlowDocument();
            var plainText = System.Text.RegularExpressions.Regex.Replace(content, @"<[^>]+>", " ");
            doc.Blocks.Add(new Paragraph(new Run(plainText.Trim())));
            return doc;
        }
    }
}
