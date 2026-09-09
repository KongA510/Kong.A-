using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>读取 Classic Form，并在同一 AML 事务内按 ID 增量更新。不会删除或重建已有 Body。</summary>
public sealed class FormConfigurationEditService : IFormConfigurationEditService
{
    private readonly ArasConnectionService _connection;
    private readonly IErrorLogService _errors;
    private readonly IOperationLogService _operations;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private FormEditorMetadata? _metadata;
    private readonly HashSet<string> _submitted = [];
    private readonly HashSet<string> _audited = [];

    public FormConfigurationEditService(ArasConnectionService connection, IErrorLogService errors, IOperationLogService operations)
    { _connection = connection; _errors = errors; _operations = operations; }

    private string ConnectionKey
    {
        get
        {
            var info = _connection.CurrentConnection;
            return info == null ? "" : $"{info.Url.TrimEnd('/')}|{info.Database}|{info.Username}|{info.LoginTime:O}";
        }
    }
    private Innovator Innovator => _connection.TypedInnovator ?? throw new InvalidOperationException("请先连接 Aras。");
    private void CheckConnection(string key)
    {
        if (string.IsNullOrEmpty(key) || key != ConnectionKey || _connection.TypedInnovator == null)
            throw new InvalidOperationException("Aras 连接已变更或断开。草稿已保留，请返回原连接重新加载并核对。");
    }
    private async Task<T> ReadAsync<T>(string operation, Func<Innovator, string, T> read, CancellationToken token)
    {
        await _gate.WaitAsync(token);
        try
        {
            var key = ConnectionKey; var inn = Innovator;
            var result = await Task.Run(() => { token.ThrowIfCancellationRequested(); return read(inn, key); }, token);
            CheckConnection(key); token.ThrowIfCancellationRequested(); return result;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        { await _errors.LogErrorAsync($"窗体配置修改-{operation}", ex.Message, ErrorLog.LevelP1, ex.StackTrace); throw; }
        finally { _gate.Release(); }
    }

    public Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(CancellationToken cancellationToken = default) =>
        ReadAsync<IReadOnlyList<ArasItemTypeInfo>>("查询对象类", (inn, _) => Get(inn, Query("ItemType", "id,name,label"))
            .Select(item => new ArasItemTypeInfo { Id = Id(item), Name = Value(item, "name"), Label = Value(item, "label") })
            .OrderBy(item => item.DisplayName, StringComparer.CurrentCultureIgnoreCase).ToList(), cancellationToken);

    public Task<IReadOnlyList<FormEditorSummary>> GetFormsAsync(string itemTypeId, CancellationToken cancellationToken = default) =>
        ReadAsync<IReadOnlyList<FormEditorSummary>>("查询窗体", (inn, _) =>
        {
            RequireId(itemTypeId);
            var query = Query("ItemType", "id,name", itemTypeId);
            query.Add(new XElement("Relationships", Query("View", "id,related_id(name,label,classification),type,classification,role")));
            var itemType = Get(inn, query).SingleOrDefault();
            if (itemType == null) return [];
            return Relationships(itemType, "View").Where(view => view.Element("related_id")?.Element("Item") != null)
                .GroupBy(view => Id(view.Element("related_id")!.Element("Item")!))
                .Select(group =>
                {
                    var form = group.First().Element("related_id")!.Element("Item")!;
                    return new FormEditorSummary
                    {
                        Id = Id(form), Name = Value(form, "name"), Label = Value(form, "label"), Classification = Value(form, "classification"),
                        Views = group.Select(view => ReadView(view, itemTypeId, Value(itemType, "name"))).ToList()
                    };
                }).OrderBy(form => form.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
        }, cancellationToken);

    public Task<FormEditorDefinition> GetDefinitionAsync(string itemTypeId, string formId, CancellationToken cancellationToken = default) =>
        ReadAsync("读取配置", (inn, key) => ReadDefinition(inn, key, itemTypeId, formId), cancellationToken);

    private static FormEditorDefinition ReadDefinition(Innovator inn, string key, string itemTypeId, string formId)
    {
        RequireId(formId);
        var query = Query("Form", "*", formId);
        var bodyQuery = Query("Body", "*");
        var fieldQuery = Query("Field", "*");
        // Include i18n properties in the conflict baseline, not only the login language.
        foreach (var definitionQuery in new[] { query, bodyQuery, fieldQuery }) definitionQuery.SetAttributeValue("language", "*");
        fieldQuery.Add(new XElement("Relationships", Query("Field Event", "*")));
        bodyQuery.Add(new XElement("Relationships", fieldQuery));
        query.Add(new XElement("Relationships", bodyQuery, Query("Form Event", "*")));
        var form = Get(inn, query).SingleOrDefault() ?? throw new InvalidOperationException("窗体不存在或没有读取权限。");
        var result = new FormEditorDefinition { ConnectionKey = key, ItemTypeId = itemTypeId, Form = Snapshot(form) };
        var viewQuery = Query("View", "id,source_id(name),related_id,type,classification,role");
        viewQuery.Add(new XElement("related_id", formId));
        result.SharedViews = Get(inn, viewQuery).Select(view => ReadView(view, ItemId(view, "source_id"),
            Value(view.Element("source_id")?.Element("Item"), "name", (string?)view.Element("source_id")?.Attribute("keyed_name") ?? ""))).ToList();
        if (!string.IsNullOrEmpty(itemTypeId))
        {
            RequireId(itemTypeId);
            var propertyQuery = Query("Property", "id,name,label,data_type,data_source,default_value");
            propertyQuery.Add(new XElement("source_id", itemTypeId));
            result.Properties = Get(inn, propertyQuery).Select(property => new FormEditorProperty
            {
                Id = Id(property), Name = Value(property, "name"), Label = Value(property, "label"),
                DataType = Value(property, "data_type"), DataSource = ItemId(property, "data_source"),
                DataSourceName = (string?)property.Element("data_source")?.Attribute("keyed_name") ?? "",
                DefaultValue = Value(property, "default_value")
            }).OrderBy(property => property.Name).ToList();
            foreach (var group in result.Properties.Where(property => property.DataType is "list" or "mv_list" or "filter list" or "color list")
                         .Where(property => IsId(property.DataSource)).GroupBy(property => property.DataSource))
            {
                var values = ReadList(inn, group.Key);
                foreach (var property in group) property.Values = values;
            }
        }
        return result;
    }

    public Task<FormEditorMetadata> GetEditorMetadataAsync(CancellationToken cancellationToken = default) =>
        ReadAsync("读取控件元数据", (inn, key) =>
        {
            if (_metadata?.ConnectionKey == key) return _metadata;
            var result = new FormEditorMetadata { ConnectionKey = key };
            var lists = new Dictionary<string, List<ArasFormOption>>();
            foreach (var (type, descriptors) in new[] { ("Field", result.Field), ("Form", result.Form), ("Body", result.Body) })
            {
                var query = Query("ItemType", "id,name"); query.Add(new XElement("name", type));
                query.Add(new XElement("Relationships", Query("Property", "name,label,data_type,data_source,default_value,is_required,stored_length")));
                var item = Get(inn, query).SingleOrDefault() ?? throw new InvalidOperationException($"无法读取 {type} 元数据。");
                foreach (var property in Relationships(item, "Property"))
                {
                    var descriptor = new FormEditorPropertyDescriptor
                    {
                        ItemType = type, Name = Value(property, "name"), Label = Value(property, "label"),
                        DataType = Value(property, "data_type"), DataSource = ItemId(property, "data_source"),
                        DefaultValue = Value(property, "default_value"), Required = Value(property, "is_required") == "1",
                        StoredLength = int.TryParse(Value(property, "stored_length"), out var length) ? length : 0
                    };
                    if (descriptor.DataType is "list" or "filter list" && IsId(descriptor.DataSource))
                    {
                        if (!lists.TryGetValue(descriptor.DataSource, out var options))
                            lists[descriptor.DataSource] = options = ReadList(inn, descriptor.DataSource);
                        descriptor.Options = options;
                    }
                    descriptors[descriptor.Name] = descriptor;
                }
            }
            if (!result.Field.ContainsKey("field_type") || !result.FieldTypes.Any())
                throw new InvalidOperationException("无法读取 Field.field_type 的官方控件列表。为避免猜测配置，编辑器暂不可用。");
            result.AvailableForms = Get(inn, Query("Form", "id,name,label,classification"))
                .Select(form => new FormEditorSummary { Id = Id(form), Name = Value(form, "name"), Label = Value(form, "label"), Classification = Value(form, "classification") })
                .OrderBy(form => form.Name).ToList();
            return _metadata = result;
        }, cancellationToken);

    public FormEditorChangeSet BuildChangeSet(FormEditorDefinition document, FormEditorMetadata metadata)
    {
        if (document.Form.Type != "Form" || document.Form.IsNew || document.Form.IsDeleted ||
            document.Form.Children.Any(body => body.Type != "Body" || body.Children.Any(field => field.Type != "Field" || field.Children.Count > 0)))
            throw new InvalidOperationException("编辑器只允许修改已有 Form 及其 Body/Field，不允许创建或删除窗体。");
        if (document.IsResponsive) throw new InvalidOperationException("首版仅编辑 Classic Form，响应式窗体保持原样。");
        if (FormEditorRules.ConnectionIdentity(document.ConnectionKey) != FormEditorRules.ConnectionIdentity(metadata.ConnectionKey))
            throw new InvalidOperationException("配置与控件元数据来自不同系统或账号。");
        var result = new FormEditorChangeSet();
        var root = new XElement("AML");
        var form = BuildItem(document.Form, metadata, result.Changes);
        if (form != null) root.Add(form);
        ValidateDocument(document, metadata);
        result.Aml = root.ToString(SaveOptions.DisableFormatting);
        return result;
    }

    private static XElement? BuildItem(FormEditorItem item, FormEditorMetadata metadata, List<FormEditorChange> changes)
    {
        if (!item.HasChanges) return null;
        if (item.Type == "Body" && item.IsDeleted) throw new InvalidOperationException("不允许删除已有 Body。");
        // Aras edit performs lock/update/unlock. The Form lock is managed by SaveAsync,
        // so update keeps a lock that the user already owned before entering this editor.
        var action = item.IsDeleted ? "delete" : item.IsNew ? "add" : item.Type == "Form" ? "update" : "edit";
        var xml = new XElement("Item", new XAttribute("type", item.Type), new XAttribute("id", item.Id), new XAttribute("action", action));
        RequireId(item.Id);
        if (item.IsDeleted)
        {
            changes.Add(new(action, item.Type, item.Id, item.Name, new Dictionary<string, string?>())); return xml;
        }
        var descriptors = item.Type == "Form" ? metadata.Form : item.Type == "Body" ? metadata.Body : metadata.Field;
        foreach (var (name, value) in item.Changes)
        {
            if (!descriptors.TryGetValue(name, out var descriptor) ||
                (item.Type == "Form" && !FormEditorRules.FormProperties.Contains(name)) ||
                (item.Type == "Field" && !FormEditorRules.IsEditableField(name, metadata)) || item.Type == "Body")
                throw new InvalidOperationException($"不允许写入未验证的配置：{item.Type}.{name}");
            if (!item.IsNew && name is "name" or "propertytype_id")
                throw new InvalidOperationException("已有控件的名称与属性绑定不可修改。");
            ValidateValue(item, descriptor, value);
            xml.Add(value == null ? new XElement(name, new XAttribute("is_null", "1")) : new XElement(name, value));
        }
        if (item.IsNew || item.Changes.Count > 0)
            changes.Add(new(action, item.Type, item.Id, item.Name, new Dictionary<string, string?>(item.Changes)));
        var children = item.Children.Select(child => BuildItem(child, metadata, changes)).OfType<XElement>().ToList();
        if (children.Count > 0) xml.Add(new XElement("Relationships", children));
        return xml;
    }

    private static void ValidateValue(FormEditorItem item, FormEditorPropertyDescriptor descriptor, string? value)
    {
        if (descriptor.Required && string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{descriptor.Name} 不能为空。");
        if (string.IsNullOrEmpty(value)) return;
        if (descriptor.StoredLength > 0 && descriptor.DataType is "string" or "ml_string" && value.Length > descriptor.StoredLength)
            throw new InvalidOperationException($"{descriptor.Name} 不能超过 {descriptor.StoredLength} 个字符。");
        if (descriptor.DataType == "integer" && !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            throw new InvalidOperationException($"{descriptor.Name} 必须为整数。");
        if (descriptor.DataType is "decimal" or "float" && !double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            throw new InvalidOperationException($"{descriptor.Name} 必须为数字。");
        if (descriptor.DataType == "boolean" && value is not "0" and not "1") throw new InvalidOperationException($"{descriptor.Name} 必须为 0 或 1。");
        if (descriptor.Options.Count > 0 && !descriptor.Options.Any(option => option.Value == value))
            throw new InvalidOperationException($"{descriptor.Name} 的值不在目标系统允许的选项中。");
        if (descriptor.DataType == "item" && !IsId(value)) throw new InvalidOperationException($"{descriptor.Name} 必须引用有效的 Aras ID。");
        if (descriptor.Name is "width" or "height" or "display_length" or "textarea_rows" or "textarea_cols" &&
            (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var size) || !double.IsFinite(size) || size <= 0))
            throw new InvalidOperationException($"{descriptor.Name} 必须大于 0。");
    }

    private static void ValidateDocument(FormEditorDefinition document, FormEditorMetadata metadata)
    {
        var fields = document.Fields.ToList();
        foreach (var field in fields.Where(field => field.IsNew))
        {
            if (string.IsNullOrWhiteSpace(field.Get("name")) || fields.Count(other => other.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase)) > 1)
                throw new InvalidOperationException($"新增控件名称为空或重复：{field.Name}");
            var propertyId = field.Get("propertytype_id");
            var property = document.Properties.FirstOrDefault(property => property.Id == propertyId);
            if (propertyId != "" && property == null) throw new InvalidOperationException("新增控件绑定的属性不属于当前对象类。");
            if (!FormEditorRules.CompatibleTypes(metadata, property).Any(option => option.Value == field.Get("field_type")))
                throw new InvalidOperationException($"控件类型与属性数据类型不兼容：{field.Name}");
            if (field.Get("field_type") == "nested form" && (metadata.NestedFormProperty == ""
                    ? property?.DataType != "item" || !IsId(property.DataSource)
                    : !IsId(field.Get(metadata.NestedFormProperty))))
                throw new InvalidOperationException("嵌套窗体必须绑定 Item 属性；有独立来源字段的系统须选择来源窗体。");
            if (field.Get("field_type") == "image" && property == null && (metadata.ImageProperty == "" || string.IsNullOrWhiteSpace(field.Get(metadata.ImageProperty))))
                throw new InvalidOperationException("图片控件必须填写图片来源。");
        }
        foreach (var field in fields.Where(field => !field.IsNew && field.Changes.ContainsKey("field_type")))
        {
            var property = document.Properties.FirstOrDefault(property => property.Id == field.Get("propertytype_id"));
            if (!FormEditorRules.CompatibleTypes(metadata, property).Any(option => option.Value == field.Get("field_type")))
                throw new InvalidOperationException($"控件类型与属性不兼容：{field.Name}");
        }
        if (metadata.ContainerProperty == "") return;
        foreach (var body in document.Bodies)
        {
            var active = body.Children.Where(field => !field.IsDeleted).ToList();
            foreach (var field in active)
            {
                var parentName = field.Get(metadata.ContainerProperty);
                if (parentName == "") continue;
                var ancestors = new HashSet<string> { field.Name };
                while (parentName != "")
                {
                    if (!ancestors.Add(parentName)) throw new InvalidOperationException($"分组存在循环引用：{field.Name}");
                    var parent = active.FirstOrDefault(item => item.Name == parentName && item.Get("field_type") == "groupbox");
                    if (parent == null)
                    {
                        var deletedParent = body.Children.Any(item => item.IsDeleted && item.Name == parentName);
                        if (field.IsNew || field.Changes.ContainsKey(metadata.ContainerProperty) || deletedParent)
                            throw new InvalidOperationException($"容器不存在或已删除：{parentName}");
                        break; // 不修改原系统遗留配置。
                    }
                    parentName = parent.Get(metadata.ContainerProperty);
                }
            }
        }
    }

    public async Task<FormEditorDefinition> SaveAsync(FormEditorDefinition document, FormEditorMetadata metadata, CancellationToken cancellationToken = default)
    {
        if (document.ConnectionKey != ConnectionKey &&
            FormEditorRules.ConnectionIdentity(document.ConnectionKey) == FormEditorRules.ConnectionIdentity(ConnectionKey))
        {
            // Same server/database/user, new login: use fresh metadata, then the normal baseline check.
            // The caller's draft remains untouched until a verified save succeeds.
            metadata = await GetEditorMetadataAsync(cancellationToken);
            document = document.Clone(); document.ConnectionKey = metadata.ConnectionKey;
        }
        await _gate.WaitAsync(cancellationToken);
        Innovator? inn = null; bool acquired = false; bool sent = false;
        FormEditorChangeSet? attempted = null;
        string auditKey = "";
        try
        {
            CheckConnection(document.ConnectionKey); inn = Innovator;
            var changes = BuildChangeSet(document, metadata);
            attempted = changes;
            auditKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
                FormEditorRules.ConnectionIdentity(document.ConnectionKey) + document.Form.OriginalXml + changes.Aml)));
            if (changes.Changes.Count == 0) return document;
            var current = await Task.Run(() => ReadDefinition(inn, document.ConnectionKey, document.ItemTypeId, document.Form.Id), cancellationToken);
            if (MatchesApplied(document.Form, current.Form))
            {
                if (_submitted.Contains(auditKey)) await AuditAsync(document, changes, auditKey);
                return current; // 处理上次响应丢失：固定 ID 已全部提交。
            }
            VerifyUnchanged(document.Form, current.Form);
            var owner = current.Form.Get("locked_by_id");
            if (owner != "" && owner != inn.getUserID()) throw new InvalidOperationException("窗体已被其他用户锁定，不能保存。");
            if (owner == "")
            {
                await Task.Run(() => Apply(inn, new XElement("AML", ActionItem("Form", "lock", document.Form.Id))));
                acquired = true;
            }
            cancellationToken.ThrowIfCancellationRequested(); CheckConnection(document.ConnectionKey);
            current = await Task.Run(() => ReadDefinition(inn, document.ConnectionKey, document.ItemTypeId, document.Form.Id));
            VerifyUnchanged(document.Form, current.Form);
            cancellationToken.ThrowIfCancellationRequested(); CheckConnection(document.ConnectionKey);
            sent = true;
            _submitted.Add(auditKey);
            await Task.Run(() => Apply(inn, XElement.Parse(changes.Aml)));
            var saved = await Task.Run(() => ReadDefinition(inn, document.ConnectionKey, document.ItemTypeId, document.Form.Id));
            if (!MatchesApplied(document.Form, saved.Form)) throw new InvalidOperationException("服务器返回的配置与提交内容不一致，请保留草稿并重新核对。");
            await AuditAsync(document, changes, auditKey);
            CheckConnection(document.ConnectionKey); return saved;
        }
        catch (OperationCanceledException) when (!sent) { throw; }
        catch (Exception ex)
        {
            await _errors.LogErrorAsync("窗体配置修改-保存", ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            if (sent && inn != null)
            {
                try
                {
                    CheckConnection(document.ConnectionKey);
                    var actual = await Task.Run(() => ReadDefinition(inn, document.ConnectionKey, document.ItemTypeId, document.Form.Id));
                    if (MatchesApplied(document.Form, actual.Form))
                    { if (attempted != null) await AuditAsync(document, attempted, auditKey); return actual; }
                }
                catch (Exception verifyError) { await _errors.LogErrorAsync("窗体配置修改-核对提交结果", verifyError.Message, ErrorLog.LevelP1, verifyError.StackTrace); }
                throw new InvalidOperationException("保存结果尚未确认，草稿和新增控件 ID 已保留。恢复连接后再次保存会先核对服务器，避免重复新增。" + ex.Message, ex);
            }
            throw;
        }
        finally
        {
            if (acquired && inn != null)
            {
                try { await Task.Run(() => Apply(inn, new XElement("AML", ActionItem("Form", "unlock", document.Form.Id)))); }
                catch (Exception ex) { await _errors.LogErrorAsync("窗体配置修改-释放本次锁", ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
            }
            _gate.Release();
        }
    }

    private async Task AuditAsync(FormEditorDefinition document, FormEditorChangeSet changes, string transaction)
    {
        foreach (var change in changes.Changes)
        {
            var key = transaction + change.Id + change.Action;
            if (_audited.Contains(key)) continue;
            try
            {
                await _operations.LogAsync(change.Action == "add" ? "Create" : change.Action == "delete" ? "Delete" : "Update",
                    change.ItemType, change.Id, $"窗体配置修改 · {document.Form.Name} · {change.Name} · {string.Join(",", change.Properties.Keys)}");
                _audited.Add(key);
            }
            catch (Exception ex) { await _errors.LogErrorAsync("窗体配置修改-操作日志", ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
        }
    }

    private static void VerifyUnchanged(FormEditorItem original, FormEditorItem current)
    {
        if (Canonical(original.OriginalXml) != Canonical(current.OriginalXml))
            throw new InvalidOperationException("服务器窗体已被修改。草稿已保留，请查看变更摘要后重新加载核对，不能覆盖他人的修改。");
    }
    private static string Canonical(string xml)
    {
        if (string.IsNullOrEmpty(xml)) return "";
        var element = XElement.Parse(xml);
        foreach (var node in element.Descendants().Where(node => node.Name.LocalName is "locked_by_id").ToArray()) node.Remove();
        foreach (var attribute in element.DescendantsAndSelf().Attributes().Where(attribute => attribute.Name.LocalName is "action" or "isNew" or "isTemp" or "keyed_name").ToArray()) attribute.Remove();
        return CanonicalElement(element);
    }
    private static string CanonicalElement(XElement element) =>
        new XElement(element.Name, element.Attributes().OrderBy(attribute => attribute.Name.ToString()),
            element.HasElements ? element.Elements().OrderBy(child => child.Name.ToString()).ThenBy(child => (string?)child.Attribute("id"))
                .Select(child => (object)XElement.Parse(CanonicalElement(child))) : new object[] { element.Value }).ToString(SaveOptions.DisableFormatting);

    private static bool MatchesApplied(FormEditorItem intended, FormEditorItem actual)
    {
        foreach (var pair in intended.Changes)
            if (actual.Get(pair.Key) != (pair.Value ?? "")) return false;
        foreach (var child in intended.Children)
        {
            var found = actual.Children.FirstOrDefault(item => item.Id == child.Id);
            if (child.IsDeleted) { if (found != null) return false; }
            else if (found == null || !MatchesApplied(child, found)) return false;
        }
        return true;
    }

    public async Task<FormEditorResource?> GetPreviewResourceAsync(string relativeOrAbsoluteUrl, string connectionKey, CancellationToken cancellationToken = default)
    {
        try
        {
            if (FormEditorRules.ConnectionIdentity(connectionKey) == FormEditorRules.ConnectionIdentity(ConnectionKey)) connectionKey = ConnectionKey;
            CheckConnection(connectionKey);
            var baseUri = new Uri(_connection.CurrentConnection!.Url.TrimEnd('/') + "/");
            var uri = new Uri(new Uri(baseUri, "Client/"), relativeOrAbsoluteUrl);
            if (uri.Scheme is not "https" and not "http" || uri.Authority != baseUri.Authority ||
                !uri.AbsolutePath.StartsWith(baseUri.AbsolutePath + "Client/", StringComparison.OrdinalIgnoreCase) ||
                uri.Query != "" || uri.UserInfo != "") return null;
            var extension = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".css" => "text/css", ".png" => "image/png", ".jpg" or ".jpeg" => "image/jpeg", ".gif" => "image/gif",
                ".svg" => "image/svg+xml", ".webp" => "image/webp", ".woff" => "font/woff", ".woff2" => "font/woff2", _ => ""
            };
            if (contentType == "") return null;
            using var handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false };
            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };
            using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode || response.Content.Headers.ContentLength > 5 * 1024 * 1024) return null;
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var buffer = new MemoryStream(); var bytes = new byte[8192]; int length;
            while ((length = await stream.ReadAsync(bytes, cancellationToken)) > 0)
            { if (buffer.Length + length > 5 * 1024 * 1024) return null; buffer.Write(bytes, 0, length); }
            CheckConnection(connectionKey);
            return new(buffer.ToArray(), contentType);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex) { await _errors.LogErrorAsync("窗体配置修改-预览资源", ex.Message, ErrorLog.LevelP1, ex.StackTrace); return null; }
    }

    private static FormEditorItem Snapshot(XElement item) => new()
    {
        Id = Id(item), Type = (string?)item.Attribute("type") ?? "", OriginalXml = item.ToString(SaveOptions.DisableFormatting),
        Original = item.Elements().Where(property => property.Name.NamespaceName == "" && property.Name.LocalName != "Relationships")
            .GroupBy(property => property.Name.LocalName).ToDictionary(group => group.Key,
                group => (string?)group.First().Attribute("is_null") == "1" ? null : group.First().Element("Item") is { } related ? Id(related) : group.First().Value),
        Children = item.Element("Relationships")?.Elements("Item").Where(child => (string?)child.Attribute("type") is "Body" or "Field").Select(Snapshot).ToList() ?? []
    };
    private static FormEditorView ReadView(XElement view, string itemTypeId, string itemTypeName) =>
        new(Id(view), itemTypeId, itemTypeName, Value(view, "type"), Value(view, "classification"), Value(view, "role"));
    private static List<ArasFormOption> ReadList(Innovator inn, string id)
    {
        var query = Query("Value", "value,label,sort_order"); query.Add(new XElement("source_id", id));
        return Get(inn, query).OrderBy(item => int.TryParse(Value(item, "sort_order"), out var value) ? value : 0)
            .Select(item => new ArasFormOption(Value(item, "label", Value(item, "value")), Value(item, "value"))).ToList();
    }
    private static XElement Query(string type, string select, string? id = null)
    {
        var query = new XElement("Item", new XAttribute("type", type), new XAttribute("action", "get"), new XAttribute("select", select));
        if (id != null) query.SetAttributeValue("id", id); return query;
    }
    private static XElement ActionItem(string type, string action, string id) => new("Item", new XAttribute("type", type), new XAttribute("action", action), new XAttribute("id", id));
    private static List<XElement> Get(Innovator inn, XElement query)
    {
        var result = inn.applyAML(new XElement("AML", query).ToString(SaveOptions.DisableFormatting));
        if (result.isEmpty()) return [];
        if (result.isError()) throw new InvalidOperationException(result.getErrorString());
        // R37 Item.ToString() serializes the owning SOAP document, not just this Item.
        return Enumerable.Range(0, result.getItemCount()).Select(index => XElement.Parse(result.getItemByIndex(index).node.OuterXml)).ToList();
    }
    private static void Apply(Innovator inn, XElement aml)
    {
        var result = inn.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        if (result.isError()) throw new InvalidOperationException(result.getErrorString());
    }
    private static IEnumerable<XElement> Relationships(XElement item, string type) => item.Element("Relationships")?.Elements("Item").Where(child => (string?)child.Attribute("type") == type) ?? [];
    private static string Id(XElement item) => (string?)item.Attribute("id") ?? Value(item, "id");
    private static string ItemId(XElement item, string property) => item.Element(property)?.Element("Item") is { } related ? Id(related) : Value(item, property);
    private static string Value(XElement? item, string property, string fallback = "") => item?.Element(property)?.Value is { Length: > 0 } value ? value : fallback;
    private static bool IsId(string value) => value.Length == 32 && value.All(Uri.IsHexDigit);
    private static void RequireId(string id) { if (!IsId(id)) throw new InvalidOperationException("Aras ID 无效。"); }
}
