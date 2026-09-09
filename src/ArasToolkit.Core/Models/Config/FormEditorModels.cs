using System.Globalization;
using System.Text.Json;

namespace ArasToolkit.Core.Models;

/// <summary>Classic Form 编辑器的无 UI / IOM 依赖数据契约。原始 XML 用于并发检查与无损保留。</summary>
public sealed class FormEditorItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N").ToUpperInvariant();
    public string Type { get; set; } = "Field";
    public string OriginalXml { get; set; } = "";
    public bool IsNew { get; set; }
    public bool IsDeleted { get; set; }
    public Dictionary<string, string?> Original { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, string?> Changes { get; set; } = new(StringComparer.Ordinal);
    public List<FormEditorItem> Children { get; set; } = [];

    public string Get(string name, string fallback = "") =>
        (Changes.TryGetValue(name, out var changed) ? changed : Original.GetValueOrDefault(name)) ?? fallback;
    public int Number(string name, int fallback = 0) =>
        int.TryParse(Get(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) ? number : fallback;
    public string Name => Get("name", Id);
    public string DisplayName => $"{Get("label", Name)} · {Name} [{Get("field_type", Type)}]{(IsNew ? " · 新增" : "")}{(Get("is_visible", "1") == "0" ? " · 隐藏" : "")}";
    public bool HasChanges => IsNew || IsDeleted || Changes.Count > 0 || Children.Any(item => item.HasChanges);

    public void Set(string name, string? value)
    {
        if (!IsNew && Original.TryGetValue(name, out var original) && original == value)
            Changes.Remove(name);
        else if (!IsNew && !Original.ContainsKey(name) && value == null)
            Changes.Remove(name);
        else
            Changes[name] = value;
    }

    public FormEditorItem Clone() => new()
    {
        Id = Id, Type = Type, OriginalXml = OriginalXml, IsNew = IsNew, IsDeleted = IsDeleted,
        Original = new(Original, StringComparer.Ordinal), Changes = new(Changes, StringComparer.Ordinal),
        Children = Children.Select(child => child.Clone()).ToList()
    };
}

public sealed record FormEditorView(string Id, string ItemTypeId, string ItemTypeName, string Usage, string Classification, string Identity);

public sealed class FormEditorSummary
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string Classification { get; set; } = "";
    public List<FormEditorView> Views { get; set; } = [];
    public string DisplayName => $"{(string.IsNullOrWhiteSpace(Label) ? Name : Label)} ({Name}) · {string.Join(" / ", Views.Select(view => string.IsNullOrEmpty(view.Usage) ? "default" : view.Usage).Distinct())}";
}

public sealed class FormEditorProperty
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string DataType { get; set; } = "";
    public string DataSource { get; set; } = "";
    public string DataSourceName { get; set; } = "";
    public string DefaultValue { get; set; } = "";
    public List<ArasFormOption> Values { get; set; } = [];
    public string DisplayName => $"{(string.IsNullOrEmpty(Label) ? Name : Label)} ({Name}) · {DataType}";
}

public sealed class FormEditorDefinition
{
    public string ItemTypeId { get; set; } = "";
    public string ConnectionKey { get; set; } = "";
    public FormEditorItem Form { get; set; } = new() { Type = "Form" };
    public List<FormEditorProperty> Properties { get; set; } = [];
    public List<FormEditorView> SharedViews { get; set; } = [];
    public bool IsResponsive => Form.Get("classification").Contains("responsive", StringComparison.OrdinalIgnoreCase);
    public IEnumerable<FormEditorItem> Bodies => Form.Children.Where(item => item.Type == "Body" && !item.IsDeleted);
    public IEnumerable<FormEditorItem> Fields => Bodies.SelectMany(body => body.Children).Where(item => item.Type == "Field" && !item.IsDeleted);
    public FormEditorDefinition Clone() => new()
    {
        ItemTypeId = ItemTypeId, ConnectionKey = ConnectionKey, Form = Form.Clone(),
        Properties = Properties, SharedViews = SharedViews
    };
}

/// <summary>只有服务器 Property 元数据实际存在的属性，才能出现在编辑器或新增 AML 中。</summary>
public sealed class FormEditorPropertyDescriptor
{
    public string ItemType { get; set; } = "Field";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string DataType { get; set; } = "";
    public string DataSource { get; set; } = "";
    public string DefaultValue { get; set; } = "";
    public bool Required { get; set; }
    public int StoredLength { get; set; }
    public List<ArasFormOption> Options { get; set; } = [];
}

public sealed class FormEditorMetadata
{
    public string ConnectionKey { get; set; } = "";
    public Dictionary<string, FormEditorPropertyDescriptor> Field { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, FormEditorPropertyDescriptor> Form { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, FormEditorPropertyDescriptor> Body { get; set; } = new(StringComparer.Ordinal);
    public List<FormEditorSummary> AvailableForms { get; set; } = [];
    public string ContainerProperty => FindField("container", "container_name", "group_name");
    public string NestedFormProperty => FindField("form", "form_id", "nested_form");
    public string ImageProperty => FindField("image", "image_src", "image_url");
    public string FindField(params string[] candidates) => candidates.FirstOrDefault(Field.ContainsKey) ?? "";
    public IEnumerable<ArasFormOption> FieldTypes => Field.TryGetValue("field_type", out var descriptor)
        ? descriptor.Options : [];
}

public sealed record FormEditorChange(string Action, string ItemType, string Id, string Name, IReadOnlyDictionary<string, string?> Properties);
public sealed class FormEditorChangeSet
{
    public string Aml { get; set; } = "";
    public List<FormEditorChange> Changes { get; set; } = [];
    public string Summary => string.Join(Environment.NewLine, Changes.Select(change =>
        $"{(change.Action == "add" ? "新增" : change.Action == "delete" ? "删除" : "修改")} {change.ItemType} · {change.Name}" +
        (change.Properties.Count == 0 ? "" : "：" + string.Join("、", change.Properties.Select(pair => $"{pair.Key}={pair.Value ?? "(清空)"}")))));
}

public sealed record FormEditorResource(byte[] Content, string ContentType);

/// <summary>单份草稿的撤销、选择顺序与布局操作。所有写入仅改变内存，网络提交由服务负责。</summary>
public sealed class FormEditorSession
{
    private readonly Stack<FormEditorItem> _undo = new();
    private readonly Stack<FormEditorItem> _redo = new();
    private FormEditorItem? _before;
    public FormEditorDefinition Document { get; private set; }
    public FormEditorMetadata Metadata { get; private set; }
    public string BodyId { get; set; }
    public List<string> SelectedIds { get; } = [];
    public bool IsDirty => Document.Form.HasChanges;
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;
    public FormEditorItem? Body => Document.Bodies.FirstOrDefault(body => body.Id == BodyId);
    public IEnumerable<FormEditorItem> Fields => Body?.Children.Where(field => !field.IsDeleted) ?? [];
    public IEnumerable<FormEditorItem> Selected => SelectedIds.Select(id => Fields.FirstOrDefault(field => field.Id == id)).OfType<FormEditorItem>();

    public FormEditorSession(FormEditorDefinition document, FormEditorMetadata metadata)
    {
        Document = document; Metadata = metadata;
        BodyId = document.Bodies.FirstOrDefault()?.Id ?? "";
    }

    public void BeginEdit() => _before ??= Document.Form.Clone();
    public void UpdateMetadata(FormEditorMetadata metadata) => Metadata = metadata;
    public void EndEdit()
    {
        if (_before != null && JsonSerializer.Serialize(_before) != JsonSerializer.Serialize(Document.Form))
        { _undo.Push(_before); _redo.Clear(); }
        _before = null;
    }
    public void CancelEdit()
    {
        if (_before != null) Document.Form = _before;
        _before = null;
        RepairSelection();
    }
    public void Edit(Action action)
    {
        BeginEdit();
        try { action(); EndEdit(); }
        finally { if (_before != null) CancelEdit(); }
    }
    public void Undo()
    {
        if (_undo.Count == 0) return;
        _redo.Push(Document.Form.Clone()); Document.Form = _undo.Pop(); RepairSelection();
    }
    public void DiscardChanges()
    {
        void Reset(FormEditorItem item)
        {
            item.Changes.Clear(); item.IsDeleted = false;
            item.Children.RemoveAll(child => child.IsNew);
            foreach (var child in item.Children) Reset(child);
        }
        Reset(Document.Form); _undo.Clear(); _redo.Clear(); _before = null; RepairSelection();
    }
    public void Redo()
    {
        if (_redo.Count == 0) return;
        _undo.Push(Document.Form.Clone()); Document.Form = _redo.Pop(); RepairSelection();
    }
    public void Select(IEnumerable<string> ids)
    {
        var valid = Fields.Select(field => field.Id).ToHashSet();
        SelectedIds.Clear(); SelectedIds.AddRange(ids.Where(valid.Contains).Distinct());
    }
    public string Container(FormEditorItem item) => string.IsNullOrEmpty(Metadata.ContainerProperty) ? "" : item.Get(Metadata.ContainerProperty);
    public bool CanMoveSelection => Selected.Any() && Selected.All(field =>
        field.Get("positioning", "absolute") is "absolute" or "") && Selected.Select(Container).Distinct().Count() == 1;
    public void Move(IReadOnlyDictionary<string, (int X, int Y)> coordinates)
    {
        if (!CanMoveSelection || !Metadata.Field.ContainsKey("x") || !Metadata.Field.ContainsKey("y")) return;
        foreach (var field in Selected)
            if (coordinates.TryGetValue(field.Id, out var point))
            { field.Set("x", Math.Max(0, point.X).ToString(CultureInfo.InvariantCulture)); field.Set("y", Math.Max(0, point.Y).ToString(CultureInfo.InvariantCulture)); }
    }
    public void AlignY()
    {
        if (!CanMoveSelection || SelectedIds.Count < 2 || !Metadata.Field.ContainsKey("y")) return;
        var y = Selected.First().Get("y", "0");
        Edit(() => { foreach (var field in Selected) field.Set("y", y); });
    }
    public IReadOnlyList<FormEditorItem> DeleteSelection(bool includeDescendants)
    {
        var deletion = Selected.ToList();
        var changed = true;
        while (changed)
        {
            var names = deletion.Select(field => field.Name).ToHashSet(StringComparer.Ordinal);
            var children = Fields.Where(field => !deletion.Contains(field) && names.Contains(Container(field))).ToList();
            if (children.Count > 0 && !includeDescendants)
                throw new InvalidOperationException("所选容器仍有子控件，请使用整组删除。");
            changed = children.Count > 0; deletion.AddRange(children);
        }
        Edit(() =>
        {
            foreach (var item in deletion)
                if (item.IsNew) Body?.Children.Remove(item); else item.IsDeleted = true;
            if (Body is { IsNew: true, Children.Count: 0 } emptyBody)
            { Document.Form.Children.Remove(emptyBody); BodyId = Document.Bodies.FirstOrDefault()?.Id ?? ""; }
        });
        SelectedIds.Clear(); return deletion;
    }
    public FormEditorItem Add(string fieldType, FormEditorProperty? property, int x, int y, IReadOnlyDictionary<string, string?>? extra = null)
    {
        if (!Metadata.FieldTypes.Any(option => option.Value == fieldType))
            throw new InvalidOperationException("目标系统未提供该控件类型，不能新增。");
        var names = Document.Fields.Select(field => field.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var basis = property?.Name ?? fieldType.Replace(' ', '_');
        var maxLength = Metadata.Field.GetValueOrDefault("name")?.StoredLength ?? 0;
        if (maxLength <= 0) maxLength = 32;
        basis = basis[..Math.Min(basis.Length, maxLength)];
        var name = basis; var suffix = 1;
        while (names.Contains(name))
        {
            var ending = $"_{++suffix}";
            if (ending.Length >= maxLength) throw new InvalidOperationException("无法在名称长度限制内生成唯一控件名称。");
            name = basis[..Math.Min(basis.Length, maxLength - ending.Length)] + ending;
        }
        var field = new FormEditorItem { IsNew = true };
        void Set(string key, string? value) { if (Metadata.Field.ContainsKey(key)) field.Set(key, value); }
        foreach (var descriptor in Metadata.Field.Values.Where(value => !string.IsNullOrEmpty(value.DefaultValue) && FormEditorRules.IsEditableField(value.Name, Metadata)))
            Set(descriptor.Name, descriptor.DefaultValue);
        Set("name", name); Set("label", string.IsNullOrEmpty(property?.Label) ? name : property.Label);
        Set("field_type", fieldType); Set("x", Math.Max(0, x).ToString()); Set("y", Math.Max(0, y).ToString());
        Set("positioning", "absolute"); Set("is_visible", "1"); Set("is_disabled", "0");
        Set("display_length", "150"); Set("display_length_unit", "px"); Set("label_position", "top");
        if (property != null) Set("propertytype_id", property.Id);
        if (fieldType == "textarea") { Set("textarea_rows", "100"); Set("textarea_cols", "340"); }
        if (extra != null) foreach (var pair in extra) Set(pair.Key, pair.Value);
        Edit(() =>
        {
            if (Body == null)
            {
                var body = new FormEditorItem { Type = "Body", IsNew = true };
                Document.Form.Children.Add(body); BodyId = body.Id;
            }
            Body!.Children.Add(field);
        });
        Select([field.Id]); return field;
    }
    private void RepairSelection()
    {
        if (!Document.Bodies.Any(body => body.Id == BodyId)) BodyId = Document.Bodies.FirstOrDefault()?.Id ?? "";
        Select(SelectedIds.ToArray());
    }
}

public static class FormEditorRules
{
    public static string ConnectionIdentity(string key) => key.LastIndexOf('|') is var index && index >= 0 ? key[..index] : key;
    public static readonly string[] CommonFieldProperties = ["name", "label", "field_type", "propertytype_id", "x", "y", "positioning", "display_length", "display_length_unit", "width", "height", "textarea_rows", "textarea_cols", "z_index", "tab_index", "tab_stop", "is_visible", "is_disabled", "label_position", "text_align", "font_family", "font_size", "font_color", "font_weight", "font_style", "legend", "border_width", "border_style", "border_color", "html_code", "background_color", "bg_color"];
    public static readonly string[] FormProperties = ["label", "width", "height"];
    public static bool IsEditableField(string name, FormEditorMetadata metadata) =>
        CommonFieldProperties.Contains(name) || name == metadata.ContainerProperty || name == metadata.NestedFormProperty || name == metadata.ImageProperty;
    public static IEnumerable<ArasFormOption> CompatibleTypes(FormEditorMetadata metadata, FormEditorProperty? property)
    {
        if (property == null) return metadata.FieldTypes;
        string[] types = property.DataType.ToLowerInvariant() switch
        {
            "item" => property.DataSourceName == "File" ? ["file item", "item", "nested form"] : ["item", "nested form"], "date" => ["date"], "boolean" => ["checkbox"], "image" => ["image"],
            "file item" => ["file item"], "formatted text" => ["formatted text"], "ml_string" => ["ml_string"],
            "list" or "filter list" => ["dropdown", "radio button list", "listbox single select", "color list"],
            "mv_list" => ["dropdown", "checkbox list", "listbox multi select"],
            "text" => ["textarea", "text"], "color" => ["color"], "color list" => ["color list"],
            _ => ["text", "password", "textarea", "label"]
        };
        return metadata.FieldTypes.Where(option => types.Contains(option.Value));
    }
}
