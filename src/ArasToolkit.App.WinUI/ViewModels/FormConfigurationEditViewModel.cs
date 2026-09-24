using System.Collections.ObjectModel;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.UI.Dispatching;

namespace ArasToolkit.App.WinUI.ViewModels;

public sealed class FormConfigurationEditViewModel : ObservableObject, IDisposable
{
    private readonly IFormConfigurationEditService _service;
    private readonly IArasConnectionService _connection;
    private readonly IErrorLogService _errors;
    private readonly DispatcherQueue? _dispatcher = DispatcherQueue.GetForCurrentThread();
    private readonly CancellationTokenSource _lifetime = new();
    private bool _busy;
    private bool _disposed;
    private string _status = "选择对象类和已有窗体，开始编辑。";
    private string _error = "";
    private double _zoom = 100;
    public event Action<string>? EditorChanged;
    public ObservableCollection<ArasItemTypeInfo> ItemTypes { get; } = [];
    public ObservableCollection<FormEditorSummary> Forms { get; } = [];
    public Dictionary<string, FormEditorDefinition> NestedForms { get; } = [];
    public Dictionary<string, string> NestedPreviewIds { get; } = [];
    public Dictionary<string, IReadOnlyList<FormEditorSummary>> NestedPreviewOptions { get; } = [];
    public ArasItemTypeInfo? SelectedItemType { get; private set; }
    public FormEditorSummary? SelectedForm { get; private set; }
    public FormEditorMetadata? Metadata { get; private set; }
    public FormEditorSession? Session { get; private set; }
    public bool IsBusy { get => _busy; private set { SetProperty(ref _busy, value); NotifyState(); EditorChanged?.Invoke("state"); } }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }
    public string Error { get => _error; private set { SetProperty(ref _error, value); OnPropertyChanged(nameof(HasError)); } }
    public bool HasError => Error.Length > 0;
    public bool HasChanges => Session?.IsDirty == true;
    public int PendingChangeCount => Session == null ? 0 : CountChanges(Session.Document.Form);
    private static int CountChanges(FormEditorItem item) => (item.IsNew || item.IsDeleted || item.Changes.Count > 0 ? 1 : 0) + item.Children.Sum(CountChanges);
    public bool CanEdit => !IsBusy && Session != null && !Session.Document.IsResponsive && ConnectionMatches;
    public bool CanSave => CanEdit && HasChanges;
    public bool CanUndo => CanEdit && Session?.CanUndo == true;
    public bool CanRedo => CanEdit && Session?.CanRedo == true;
    public bool CanAlign => CanEdit && Session?.CanMoveSelection == true && Session.SelectedIds.Count >= 2;
    public bool CanDelete => CanEdit && Session?.SelectedIds.Count > 0;
    public bool ConnectionMatches
    {
        get
        {
            var info = _connection.CurrentConnection;
            return info != null && Session != null && FormEditorRules.ConnectionIdentity(Session.Document.ConnectionKey) == $"{info.Url.TrimEnd('/')}|{info.Database}|{info.Username}";
        }
    }
    public double Zoom { get => _zoom; set { if (SetProperty(ref _zoom, Math.Clamp(value, 50, 200))) EditorChanged?.Invoke("zoom"); } }
    public string SelectionSummary => Session == null ? "未加载窗体" :
        $"{Session.Document.Form.Name} · {Session.Fields.Count()} 个控件 · 选中 {Session.SelectedIds.Count}" +
        (Session.Selected.FirstOrDefault() is { } first ? $" · 基准 X={first.Get("x", "0")} / Y={first.Get("y", "0")}" : "") +
        $" · 待保存 {PendingChangeCount} 项";
    public string SharedSummary => Session == null ? "" : string.Join("；", Session.Document.SharedViews.Select(view =>
        $"{view.ItemTypeName} / {view.Usage} / {view.Classification} / {view.Identity}").Distinct());

    public FormConfigurationEditViewModel(IFormConfigurationEditService service, IArasConnectionService connection, IErrorLogService errors)
    {
        _service = service; _connection = connection; _errors = errors;
        _connection.ConnectionChanged += OnConnectionChanged;
    }

    public Task InitializeAsync() => RunAsync("加载对象类", async () =>
    {
        var items = await _service.GetItemTypesAsync(_lifetime.Token);
        Metadata = await _service.GetEditorMetadataAsync(_lifetime.Token);
        ItemTypes.Clear(); foreach (var item in items) ItemTypes.Add(item);
        Status = $"已加载 {ItemTypes.Count} 个对象类。请选择对象类。";
        EditorChanged?.Invoke("sources");
    });

    public Task SelectItemTypeAsync(ArasItemTypeInfo itemType) => RunAsync("查询窗体", async () =>
    {
        var forms = await _service.GetFormsAsync(itemType.Id, _lifetime.Token);
        Metadata = await _service.GetEditorMetadataAsync(_lifetime.Token);
        SelectedItemType = itemType; SelectedForm = null; Session = null; NestedForms.Clear(); NestedPreviewIds.Clear(); NestedPreviewOptions.Clear();
        Forms.Clear(); foreach (var form in forms) Forms.Add(form);
        Status = forms.Count == 0 ? "该对象类没有关联窗体，可先使用「窗体配置」生成窗体。" : $"找到 {forms.Count} 个窗体，请选择要编辑的窗体。";
        EditorChanged?.Invoke("sources"); EditorChanged?.Invoke("document");
    });

    public Task SelectFormAsync(FormEditorSummary form) => RunAsync("读取窗体", async () =>
    {
        if (SelectedItemType == null) return;
        var document = await _service.GetDefinitionAsync(SelectedItemType.Id, form.Id, _lifetime.Token);
        Metadata = await _service.GetEditorMetadataAsync(_lifetime.Token);
        Session = new(document, Metadata); SelectedForm = form; NestedForms.Clear(); NestedPreviewIds.Clear(); NestedPreviewOptions.Clear();
        if (document.IsResponsive) Status = "这是响应式窗体。首版仅支持 Classic Form，当前窗体不可编辑。";
        else
        {
            Status = $"已读取 {document.Fields.Count()} 个控件。拖动和配置修改先暂存，点击保存后写回。";
            await LoadNestedAsync(document, new HashSet<string> { form.Id });
        }
        EditorChanged?.Invoke("document");
    });

    private async Task LoadNestedAsync(FormEditorDefinition document, HashSet<string> ancestors)
    {
        if (Metadata == null) return;
        foreach (var field in document.Fields.Where(field => field.Get("field_type") == "nested form"))
        {
            try
            {
                var nestedItemType = "";
                var id = field.Get(Metadata.NestedFormProperty);
                if (Metadata.NestedFormProperty == "")
                {
                    var property = document.Properties.FirstOrDefault(property => property.Id == field.Get("propertytype_id"));
                    if (property?.DataType != "item" || property.DataSource.Length != 32) continue;
                    nestedItemType = property.DataSource;
                    if (!NestedPreviewOptions.TryGetValue(field.Id, out var options))
                        NestedPreviewOptions[field.Id] = options = await _service.GetFormsAsync(nestedItemType, _lifetime.Token);
                    id = NestedPreviewIds.GetValueOrDefault(field.Id) ?? options.FirstOrDefault(form => form.Views.Any(view => view.Usage == "default"))?.Id ?? options.FirstOrDefault()?.Id ?? "";
                    NestedPreviewIds[field.Id] = id;
                }
                if (id.Length != 32 || ancestors.Contains(id) || NestedForms.ContainsKey(id)) continue;
                var nested = await _service.GetDefinitionAsync(nestedItemType, id, _lifetime.Token);
                NestedForms[id] = nested;
                await LoadNestedAsync(nested, new HashSet<string>(ancestors) { id });
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync("窗体配置修改-嵌套预览", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
                Status = "部分嵌套窗体无法读取，将显示占位提示。";
            }
        }
    }

    public Task RefreshNestedPreviewAsync(string? fieldId = null, string? formId = null) => RunAsync("读取嵌套预览", async () =>
    {
        if (Session == null) return;
        if (fieldId != null && formId != null) NestedPreviewIds[fieldId] = formId;
        await LoadNestedAsync(Session.Document, new HashSet<string> { Session.Document.Form.Id });
        EditorChanged?.Invoke("document");
    });

    public void SwitchBody(string id)
    {
        if (Session == null || !Session.Document.Bodies.Any(body => body.Id == id)) return;
        Session.BodyId = id; Session.Select([]); Changed("document");
    }
    public void Select(IEnumerable<string> ids) { Session?.Select(ids); Changed("selection"); }
    public void BeginDrag() { if (CanEdit) Session?.BeginEdit(); }
    public void Move(IReadOnlyDictionary<string, (int X, int Y)> positions)
    { if (CanEdit) { Session?.Move(positions); Changed("coordinates"); } }
    public void EndDrag(bool cancel)
    {
        if (cancel) Session?.CancelEdit(); else Session?.EndEdit();
        Changed("document");
    }
    public void Align() { if (CanAlign) { Session!.AlignY(); Changed("document"); } }
    public void Undo() { if (CanUndo) { Session!.Undo(); Changed("document"); } }
    public void Redo() { if (CanRedo) { Session!.Redo(); Changed("document"); } }
    public void Delete(bool includeDescendants)
    {
        if (!CanDelete) return;
        TryEdit(() => Session!.DeleteSelection(includeDescendants));
    }
    public void SetProperty(string name, string? value)
    {
        if (!CanEdit || Session == null || Metadata == null) return;
        var targets = Session.Selected.Any() ? Session.Selected.ToList() : [Session.Document.Form];
        SetTargetProperty(targets, name, value);
    }

    /// <summary>布局表按稳定 ID 修改当前草稿，不依赖画布多选状态。</summary>
    public void SetFieldProperty(string fieldId, string name, string? value)
    {
        if (!CanEdit || Session == null || Metadata == null ||
            !Metadata.Field.ContainsKey(name) || !FormEditorRules.IsEditableField(name, Metadata)) return;
        var field = Session.Fields.FirstOrDefault(item => item.Id == fieldId);
        if (field == null || field.Get(name) == (value ?? "")) return;
        SetTargetProperty([field], name, value);
    }

    private void SetTargetProperty(List<FormEditorItem> targets, string name, string? value)
    {
        if (Session == null || Metadata == null) return;
        var changed = TryEdit(() =>
        {
            Session.BeginEdit();
            try
            {
                foreach (var target in targets) target.Set(name, value);
                _service.BuildChangeSet(Session.Document, Metadata); Session.EndEdit();
            }
            finally { Session.CancelEdit(); }
        });
        if (changed && name == Metadata.NestedFormProperty) _ = RefreshNestedPreviewAsync();
    }
    public bool Add(string type, FormEditorProperty? property, int x, int y, IReadOnlyDictionary<string, string?> extra)
    {
        if (!CanEdit || Session == null) return false;
        return TryEdit(() =>
        {
            var probe = new FormEditorSession(Session.Document.Clone(), Metadata!) { BodyId = Session.BodyId };
            probe.Add(type, property, x, y, extra);
            _service.BuildChangeSet(probe.Document, Metadata!);
            Session.Add(type, property, x, y, extra);
        });
    }
    public void DiscardChanges() { Session?.DiscardChanges(); Changed("document"); }
    public string ChangesSummary()
    {
        if (Session == null || Metadata == null) return "尚未加载窗体。";
        return _service.BuildChangeSet(Session.Document, Metadata).Summary;
    }
    public async Task<bool> SaveAsync()
    {
        if (!CanSave || Session == null || Metadata == null) return !HasChanges;
        var saved = false;
        await RunAsync("保存修改", async () =>
        {
            var bodyId = Session.BodyId; var ids = Session.SelectedIds.ToArray();
            var result = await _service.SaveAsync(Session.Document.Clone(), Metadata, _lifetime.Token);
            Metadata = await _service.GetEditorMetadataAsync(_lifetime.Token);
            Session = new(result, Metadata);
            if (result.Bodies.Any(body => body.Id == bodyId)) Session.BodyId = bodyId;
            Session.Select(ids); Status = "修改已保存，并已重新读取服务器配置核对。"; saved = true;
            EditorChanged?.Invoke("document");
        });
        return saved;
    }

    private bool TryEdit(Action edit)
    {
        try { edit(); Error = ""; Changed("document"); return true; }
        catch (Exception ex)
        {
            Session?.CancelEdit(); Error = ex.Message;
            _ = _errors.LogErrorAsync("窗体配置修改-编辑", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            Changed("selection"); return false;
        }
    }
    private async Task RunAsync(string name, Func<Task> action)
    {
        if (IsBusy || _disposed) return;
        IsBusy = true; Error = "";
        try { await action(); }
        catch (OperationCanceledException) { }
        catch (Exception ex) { Error = ex.Message; await _errors.LogErrorAsync($"窗体配置修改-{name}", ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
        finally { IsBusy = false; NotifyState(); }
    }
    public async Task ReportErrorAsync(string area, Exception exception, bool showError = true)
    {
        if (showError) Error = exception.Message;
        await _errors.LogErrorAsync($"窗体配置修改-{area}", exception.Message, ErrorLog.LevelP1, exception.StackTrace ?? exception.InnerException?.StackTrace);
    }
    private void Changed(string kind) { NotifyState(); EditorChanged?.Invoke(kind); }
    private void NotifyState()
    {
        foreach (var name in new[] { nameof(HasChanges), nameof(PendingChangeCount), nameof(CanEdit), nameof(CanSave), nameof(CanUndo), nameof(CanRedo), nameof(CanAlign), nameof(CanDelete), nameof(SelectionSummary), nameof(SharedSummary), nameof(ConnectionMatches) }) OnPropertyChanged(name);
    }
    private void OnConnectionChanged()
    {
        _dispatcher?.TryEnqueue(() =>
        {
            if (_disposed) return;
            Status = ConnectionMatches ? "已恢复同一系统与账号的连接。草稿已保留，保存时将重新核对服务器配置。" : "Aras 连接已变更，原草稿已保留。返回原系统与账号后可重新核对并保存。";
            Changed("connection");
            if (ConnectionMatches) _ = RunAsync("刷新控件元数据", async () =>
            {
                Metadata = await _service.GetEditorMetadataAsync(_lifetime.Token);
                Session?.UpdateMetadata(Metadata);
            });
        });
    }
    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        _connection.ConnectionChanged -= OnConnectionChanged; _lifetime.Cancel(); _lifetime.Dispose();
    }
}
