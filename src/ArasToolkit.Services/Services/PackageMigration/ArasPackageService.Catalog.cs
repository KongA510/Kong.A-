using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

public sealed partial class ArasPackageService : IArasPackageService
{
    public static readonly string[] MetadataTypes =
    [
        "ItemType", "RelationshipType", "Form", "Life Cycle Map", "Workflow Map", "Method", "List", "Permission", "Identity", "Sequence",
        "PresentationConfiguration", "CommandBarSection", "CommandBarMenu", "CommandBarButton", "CommandBarMenuButton", "CommandBarMenuSeparator",
        "CommandBarItem", "CommandBarEdit", "CommandBarDropDown", "CommandBarCheckbox", "Grid", "rb_TreeGridViewDefinition", "qry_QueryDefinition",
        "Action", "Report", "SQL", "Variable", "UserMessage", "EMail Message", "FileType", "xClassificationTree", "xPropertyDefinition",
        "mp_MacPolicy", "Relationship View", "View", "Item Action", "Item Report", "ItemType Life Cycle", "Allowed Workflow"
    ];
    public IReadOnlyList<string> SelectableTypes { get; } = MetadataTypes.TakeWhile(t => t != "mp_MacPolicy").Prepend("Property").ToList();
    private readonly IArasMigrationSessionFactory _sessions;
    private readonly IOfficialPackageEngine _engine;
    private readonly IErrorLogService _errors;
    private readonly IOperationLogService _operations;
    private readonly string _root;
    private readonly SemaphoreSlim _executionGate = new(1, 1);

    public ArasPackageService(IArasMigrationSessionFactory sessions, IOfficialPackageEngine engine, IErrorLogService errors,
        IOperationLogService operations) : this(sessions, engine, errors, operations, Path.Combine(AppContext.BaseDirectory, "Config", "PackageMigration")) { }

    public ArasPackageService(IArasMigrationSessionFactory sessions, IOfficialPackageEngine engine, IErrorLogService errors,
        IOperationLogService operations, string root)
    { _sessions = sessions; _engine = engine; _errors = errors; _operations = operations; _root = Path.GetFullPath(root); }

    private string UserDirectory => PackageXml.SafePath(_root, PackageXml.Hash(CurrentUserContext.CurrentUserId)[..16]);
    private string NewDirectory(string prefix)
    {
        var directory = Path.Combine(UserDirectory, prefix + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(directory);
        return directory;
    }

    private async Task<T> Guard<T>(string action, Func<Task<T>> run)
    {
        try { return await run().ConfigureAwait(false); }
        catch (Exception ex) { await _errors.LogErrorAsync("导包-" + action, ex.Message, ErrorLog.LevelP1); throw; }
    }

    public Task<PackageMigrationSettings> LoadSettingsAsync() => Guard("读取设置", async () =>
    {
        var file = Path.Combine(UserDirectory, "settings.json");
        if (File.Exists(file)) return JsonSerializer.Deserialize<PackageMigrationSettings>(await File.ReadAllTextAsync(file)) ?? new();
        var candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "PackageImportExportUtilities"));
        // For installations outside a source checkout the directory is chosen explicitly in the page.
        return new PackageMigrationSettings { EngineDirectory = Directory.Exists(candidate) ? candidate : "" };
    });

    public async Task SaveSettingsAsync(PackageMigrationSettings settings) => await Guard("保存设置", async () =>
    {
        Directory.CreateDirectory(UserDirectory);
        await WriteJsonAsync(Path.Combine(UserDirectory, "settings.json"), settings, CancellationToken.None);
        return true;
    });

    public Task<List<PackageItemSelection>> SearchAsync(string connectionId, string type, string keyword, CancellationToken cancellationToken = default)
        => Guard("检索配置", async () =>
        {
            if (!SelectableTypes.Contains(type)) throw new ArgumentException("不支持该配置类型。");
            await using var session = await _sessions.OpenAsync(connectionId, cancellationToken);
            var metadata = new MetadataReader(session);
            var schema = await metadata.Schema(type, cancellationToken);
            if (schema == null) return [];
            var query = new XElement("Item", new XAttribute("type", type), new XAttribute("action", "get"),
                new XAttribute("select", "id,name,config_id,keyed_name" + (metadata.HasProperty(schema, "label") ? ",label" : "")),
                new XAttribute("maxRecords", "500"), new XAttribute("orderBy", "name"));
            if (!string.IsNullOrWhiteSpace(keyword)) query.Add(new XElement("name", new XAttribute("condition", "like"), "%" + keyword.Trim() + "%"));
            if (type == "ItemType") query.Add(new XElement("is_relationship", "0"));
            var rows = PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), cancellationToken));
            return rows.Select(x => Selection(type, x, (string?)schema.Element("is_versionable") == "1")).ToList();
        });

    public Task<PackageAnalysis> AnalyzeAsync(string connectionId, IReadOnlyList<PackageItemSelection> selected, string engineDirectory,
        IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default) => Guard("分析依赖", async () =>
    {
        if (selected.Count == 0) throw new InvalidOperationException("请先勾选配置。");
        var probe = await _engine.ProbeAsync(engineDirectory, cancellationToken);
        if (!probe.IsSupported) throw new InvalidOperationException(probe.Message);
        await using var session = await _sessions.OpenAsync(connectionId, cancellationToken);
        var metadata = new MetadataReader(session);
        var analysis = new PackageAnalysis
        {
            Source = session.Endpoint, WorkingDirectory = NewDirectory("analysis"), EngineDirectory = probe.Directory,
            Languages = await LanguagesAsync(session, cancellationToken)
        };
        var ownership = await metadata.PackageOwnership(cancellationToken);
        var queue = new Queue<PackageDependency>();
        var visited = new Dictionary<string, PackageDependency>(StringComparer.OrdinalIgnoreCase);
        var explicitIds = selected.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        async Task Enqueue(string type, string id, string parent, string reason, bool explicitItem = false)
        {
            if (!PackageXml.IsId(id)) return;
            var key = type + ":" + id.ToUpperInvariant();
            if (visited.ContainsKey(key)) return;
            if (visited.Count >= 1500) throw new InvalidOperationException("依赖超过 1500 项，请拆分模块迁移。");
            var item = await metadata.Find(type, id, cancellationToken);
            if (item == null)
            {
                analysis.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = key, Message = "源系统缺少引用：" + key });
                return;
            }
            if (visited.TryGetValue(item.Value.Selection.Key, out var already)) { visited[key] = already; return; }
            if (type == "Property")
            {
                await Enqueue("ItemType", item.Value.Value.Element("source_id")?.Value ?? "", parent, "属性随所属对象类打包", explicitItem);
                return;
            }
            var node = new PackageDependency { Item = item.Value.Selection, ParentKey = parent, Reason = reason, Status = explicitItem ? "主动选择" : "自动补入" };
            node.Item.Package = ownership.GetValueOrDefault(node.Item.ExportId, "");
            var builtInIdentity = node.Item.Type == "Identity" && new[] { "World", "Administrators", "All Employees", "Super User", "Innovator Admin", "Creator", "Owner", "Manager", "Aras PLM" }.Contains(node.Item.Name);
            var builtInMetadata = node.Item.Type == "ItemType" && (MetadataTypes.Contains(node.Item.Name)
                || new[] { "Property", "Body", "Field", "Life Cycle State", "Life Cycle Transition", "Activity Template", "Workflow Process", "Activity" }.Contains(node.Item.Name));
            if (builtInIdentity || builtInMetadata) node.Item.Package = ""; // Verify baseline IDs, not incidental custom package membership.
            node.IsExternal = !explicitItem && !explicitIds.Contains(node.Item.Key) &&
                (node.Item.Package.StartsWith("com.aras.", StringComparison.OrdinalIgnoreCase) || !MetadataTypes.Contains(node.Item.Type)
                 || builtInMetadata || node.Item.Type == "Identity" && ((string?)item.Value.Value.Element("is_alias") == "1" || builtInIdentity));
            if (node.IsExternal) node.Status = "目标前置依赖";
            visited[key] = node;
            visited[node.Item.Key] = node;
            analysis.Items.Add(node);
            if (!node.IsExternal) queue.Enqueue(node);
        }
        foreach (var item in selected) await Enqueue(item.Type, item.ExportId, "", "用户勾选", true);
        var exportedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ownedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var batch = new List<PackageDependency>();
            while (queue.Count > 0) { var next = queue.Dequeue(); if (exportedIds.Add(next.Item.Key)) batch.Add(next); }
            if (batch.Count == 0) continue;
            await _engine.ExportAsync(new PackageEngineExportRequest
            {
                ConnectionId = connectionId, EngineDirectory = probe.Directory, OutputDirectory = Path.Combine(analysis.WorkingDirectory, "Import"),
                PackageName = "toolkit.analysis", Items = batch.Select(x => x.Item).ToList(), Languages = analysis.Languages
            }, progress, cancellationToken);
            foreach (var node in batch)
            {
                var file = Path.Combine(analysis.WorkingDirectory, "Import", node.Item.Type, node.Item.ExportId + ".xml");
                var aml = PackageXml.Parse(await File.ReadAllTextAsync(file, cancellationToken));
                foreach (var unsafeAction in aml.Descendants("Item").Where(x => (string?)x.Attribute("action") is "delete" or "purge"))
                    analysis.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = node.Item.Key,
                        Message = "官方包包含删除/清理动作，首版禁止自动删除目标配置：" + (string?)unsafeAction.Attribute("type") + " / " + ((string?)unsafeAction.Attribute("where") ?? (string?)unsafeAction.Attribute("id")) });
                foreach (var owned in aml.Descendants("Item").Where(x => x.Attribute("id") != null)) ownedIds.Add(PackageXml.ItemKey(owned));
                foreach (var reference in await metadata.References(aml, cancellationToken))
                    if (string.IsNullOrEmpty(reference.Type) || !PackageXml.IsId(reference.Id))
                        analysis.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = node.Item.Key, Message = "未能解析引用：" + reference.Reason + " / " + reference.Id });
                    else if (!ownedIds.Contains(reference.Type + ":" + reference.Id.ToUpperInvariant()))
                        await Enqueue(reference.Type, reference.Id, node.Item.Key, reference.Reason);
                if (node.Item.Type == "ItemType")
                {
                    var query = new XElement("Item", new XAttribute("type", "RelationshipType"), new XAttribute("action", "get"),
                        new XAttribute("select", "id,name"), new XElement("source_id", node.Item.Id));
                    foreach (var relation in PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), cancellationToken)))
                        await Enqueue("RelationshipType", (string)relation.Attribute("id")!, node.Item.Key, "对象类关系页签");
                }
                analysis.Issues.AddRange(ResourceIssues(node.Item.Key, aml));
                progress?.Report(new PackageMigrationProgress { Stage = "分析依赖", Message = node.Item.DisplayName, Completed = exportedIds.Count, Total = exportedIds.Count + queue.Count });
            }
        }
        // RelationshipType exports own their relationship ItemType. Exporting it separately creates duplicate properties.
        var included = Directory.EnumerateFiles(Path.Combine(analysis.WorkingDirectory, "Import"), "*.xml", SearchOption.AllDirectories)
            .SelectMany(f => PackageXml.Parse(File.ReadAllText(f)).Descendants("Item").Where(i => i.Ancestors("Item").Any() && (string?)i.Attribute("action") != "get" && i.HasElements)
                .Select(PackageXml.ItemKey)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        analysis.Items.RemoveAll(n => !n.IsExternal && included.Contains(n.Item.Key));
        analysis.Issues = analysis.Issues.DistinctBy(x => (x.ItemKey, x.Message)).ToList();
        await WriteJsonAsync(Path.Combine(analysis.WorkingDirectory, "analysis.json"), analysis, cancellationToken);
        return analysis;
    });

    public Task<MigrationPackage> ExportAsync(PackageAnalysis analysis, string engineDirectory, string destination,
        IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default) => Guard("生成包", async () =>
    {
        if (!analysis.CanExport) throw new InvalidOperationException("依赖分析存在阻断问题，请处理后重新分析。");
        var directory = NewDirectory("package");
        var name = "toolkit.migration." + PackageXml.Hash(analysis.Source.Identity + "|" + string.Join("|", analysis.Items.Where(i => !i.IsExternal).Select(i => i.Item.Key).Order()))[..16].ToLowerInvariant();
        var dependencies = analysis.Items.Where(i => i.IsExternal && !string.IsNullOrEmpty(i.Item.Package)).Select(i => i.Item.Package);
        await _engine.ExportAsync(new PackageEngineExportRequest
        {
            ConnectionId = analysis.Source.Id, EngineDirectory = engineDirectory, OutputDirectory = Path.Combine(directory, "Import"),
            PackageName = name, Items = analysis.Items.Where(x => !x.IsExternal).Select(x => x.Item).ToList(), Languages = analysis.Languages
        }, progress, cancellationToken);
        await PackageFileStore.WriteManifestAsync(directory, name, dependencies, cancellationToken);
        await PackageFileStore.WriteMetadataAsync(directory, new PackageFileStore.PackageMetadata
        { Source = analysis.Source, EngineVersion = (await _engine.ProbeAsync(engineDirectory, cancellationToken)).Version, Dependencies = analysis.Items, Issues = analysis.Issues }, cancellationToken);
        var package = await PackageFileStore.ReadAsync(Path.Combine(directory, "imports.mf"), cancellationToken);
        await PackageFileStore.SealAsync(package, cancellationToken);
        package = await PackageFileStore.ReadAsync(package.ManifestPath, cancellationToken);
        if (!string.IsNullOrWhiteSpace(destination))
        {
            if (destination.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(destination)) throw new IOException("目标 ZIP 已存在，请选择新文件名。");
                ZipFile.CreateFromDirectory(directory, destination);
            }
            else
            {
                if (Directory.Exists(destination) && Directory.EnumerateFileSystemEntries(destination).Any()) throw new IOException("请选择空的导出目录。");
                Directory.CreateDirectory(destination);
                foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    var to = PackageXml.SafePath(destination, Path.GetRelativePath(directory, file));
                    Directory.CreateDirectory(Path.GetDirectoryName(to)!); File.Copy(file, to, false);
                }
            }
        }
        await AuditAsync("Create", name, "导出迁移包：" + analysis.Source.Database + "，" + package.Items.Count + " 项配置");
        return package;
    });

    public Task<MigrationPackage> ReadPackageAsync(string path, CancellationToken cancellationToken = default) => Guard("读取包", async () =>
    {
        if (path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) path = await PackageFileStore.UnzipAsync(path, NewDirectory("opened"), cancellationToken);
        else if (Directory.Exists(path)) path = Path.Combine(path, "imports.mf");
        return await PackageFileStore.ReadAsync(path, cancellationToken);
    });

    private static PackageItemSelection Selection(string type, XElement item, bool versionable) => new()
    {
        Type = type, Id = (string?)item.Attribute("id") ?? "", ConfigId = (string?)item.Element("config_id") ?? "",
        Name = (string?)item.Element("name") ?? (string?)item.Element("keyed_name") ?? (string?)item.Attribute("id") ?? "",
        Label = (string?)item.Element("label") ?? "", IsVersionable = versionable
    };

    private static async Task<List<string>> LanguagesAsync(IArasMigrationSession session, CancellationToken ct)
        => PackageXml.Items(await session.QueryAsync(PackageXml.Query("Language", select: "code"), ct))
            .Select(x => (string?)x.Element("code") ?? "").Where(x => x.Length > 0).Append("en").Distinct(StringComparer.OrdinalIgnoreCase).ToList();

    private static IEnumerable<PackageIssue> ResourceIssues(string key, XDocument document)
    {
        foreach (var method in document.Descendants("Item").Where(x => (string?)x.Attribute("type") == "Method" && !string.IsNullOrWhiteSpace((string?)x.Element("method_code"))))
            yield return new PackageIssue { ItemKey = key, Message = "方法 " + ((string?)method.Element("name") ?? key) + " 的动态调用、硬编码 ID 和外部组件需核对；可在配置列表补选相关方法。" };
        foreach (var resource in document.Descendants().Where(x => !x.HasElements && (x.Name.LocalName.Contains("icon") || x.Name.LocalName is "html_code" or "stylesheet" or "url")))
            if (resource.Value.Contains("vault:", StringComparison.OrdinalIgnoreCase) || resource.Value.Contains("vault://", StringComparison.OrdinalIgnoreCase))
                yield return new PackageIssue { ItemKey = key, IsBlocking = true, Message = "包含 Vault 资源，首版不自动复制文件：" + key + "/" + resource.Name.LocalName };
            else if (!string.IsNullOrEmpty(resource.Value) && resource.Name.LocalName is "url" or "stylesheet")
                yield return new PackageIssue { ItemKey = key, Message = "请核对目标系统外部资源：" + resource.Value };
    }

    private static async Task WriteJsonAsync<T>(string path, T value, CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(value, PackageFileStore.JsonOptions), ct);
        File.Move(temporary, path, true);
    }

    private async Task AuditAsync(string action, string id, string message)
    {
        try { await _operations.LogAsync(action, "PackageMigration", id, message); }
        catch (Exception ex) { await _errors.LogErrorAsync("导包-操作日志", ex.Message, ErrorLog.LevelP0); }
    }

    private sealed class MetadataReader(IArasMigrationSession session)
    {
        private readonly Dictionary<string, XElement?> _schemas = new(StringComparer.Ordinal);
        private readonly Dictionary<string, (PackageItemSelection Selection, XElement Value)?> _items = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _typeNames = new(StringComparer.OrdinalIgnoreCase);
        public bool HasProperty(XElement schema, string name) => schema.Descendants("Item").Any(x => (string?)x.Element("name") == name);

        public async Task<XElement?> Schema(string type, CancellationToken ct)
        {
            if (_schemas.TryGetValue(type, out var cached)) return cached;
            var query = new XElement("Item", new XAttribute("type", "ItemType"), new XAttribute("action", "get"),
                new XAttribute("select", "id,name,is_versionable,is_relationship"), new XElement("name", type), new XElement("Relationships",
                    new XElement("Item", new XAttribute("type", "Property"), new XAttribute("action", "get"), new XAttribute("select", "name,data_type,data_source"))));
            var item = PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), ct)).SingleOrDefault();
            _schemas[type] = item;
            if (item != null) _typeNames[(string)item.Attribute("id")!] = type;
            return item;
        }

        public async Task<(PackageItemSelection Selection, XElement Value)?> Find(string type, string id, CancellationToken ct)
        {
            var key = type + ":" + id;
            if (_items.TryGetValue(key, out var cached)) return cached;
            var schema = await Schema(type, ct);
            if (schema == null) return _items[key] = null;
            var versionable = (string?)schema.Element("is_versionable") == "1";
            var query = new XElement("Item", new XAttribute("type", type), new XAttribute("action", "get"));
            if (versionable) query.Add(new XElement("or", new XElement("id", id), new XElement("config_id", id)), new XElement("is_current", "1"));
            else query.SetAttributeValue("id", id);
            var item = PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), ct)).SingleOrDefault();
            var actualType = (string?)item?.Attribute("type") ?? type;
            if (actualType != type) versionable = (string?)(await Schema(actualType, ct))?.Element("is_versionable") == "1";
            return _items[key] = item == null ? null : (Selection(actualType, item, versionable), item);
        }

        public async Task<Dictionary<string, string>> PackageOwnership(CancellationToken ct)
        {
            const string query = "<AML><Item type=\"PackageDefinition\" action=\"get\" select=\"name\"><Relationships><Item type=\"PackageGroup\" action=\"get\" select=\"name\"><Relationships><Item type=\"PackageElement\" action=\"get\" select=\"element_id\" /></Relationships></Item></Relationships></Item></AML>";
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in PackageXml.Items(await session.QueryAsync(query, ct)))
                foreach (var element in package.Descendants("Item").Where(x => (string?)x.Attribute("type") == "PackageElement"))
                {
                    var id = (string?)element.Element("element_id");
                    if (id != null) result.TryAdd(id, (string?)package.Element("name") ?? "");
                }
            return result;
        }

        public async Task<List<(string Type, string Id, string Reason)>> References(XDocument doc, CancellationToken ct, IEnumerable<XElement>? packageDefinitions = null)
        {
            var references = new List<(string Type, string Id, string Reason)>();
            var definitions = (packageDefinitions ?? doc.Descendants("Item")).Where(x => (string?)x.Attribute("action") != "get").ToList();
            foreach (var item in doc.Descendants("Item"))
            {
                var type = (string?)item.Attribute("type") ?? "";
                var schema = await Schema(type, ct);
                if (schema == null) throw new InvalidDataException("无法识别配置元素类型，请先安装所需的元数据定义：" + type);
                if ((string?)item.Attribute("action") == "get")
                {
                    var included = definitions.FirstOrDefault(x => (string?)x.Attribute("type") == type && item.Element("name") != null && (string?)x.Element("name") == (string?)item.Element("name"));
                    var query = new XElement(item);
                    query.SetAttributeValue("select", "id,config_id");
                    var resolved = included ?? PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), ct)).SingleOrDefault();
                    var id = resolved == null ? "" : (string?)resolved.Attribute("id") ?? "";
                    if ((string?)schema.Element("is_versionable") == "1" && PackageXml.IsId((string?)resolved?.Element("config_id"))) id = resolved!.Element("config_id")!.Value;
                    references.Add((type, id, "命名引用 " + type + "/" + ((string?)item.Element("name") ?? "(条件查询)")));
                    continue;
                }
                foreach (var property in item.Elements().Where(p => p.Name != "Relationships" && p.Name != "id"
                    && (p.Name != "source_id" || !item.Ancestors("Item").Any())))
                {
                    if (property.HasElements || !PackageXml.IsId(property.Value)) continue;
                    var targetType = (string?)property.Attribute("type");
                    if (type == "Property" && property.Name == "data_source")
                        targetType = ((string?)item.Element("data_type"))?.ToLowerInvariant() switch
                        { "item" => "ItemType", "list" or "filter list" => "List", "sequence" => "Sequence", "foreign" => "Property", _ => targetType };
                    if (string.IsNullOrEmpty(targetType))
                    {
                        var definition = schema.Descendants("Item").FirstOrDefault(p => (string?)p.Element("name") == property.Name.LocalName);
                        if ((string?)definition?.Element("data_type") != "item") continue;
                        var dataSource = (string?)definition.Element("data_source") ?? "";
                        if (!PackageXml.IsId(dataSource))
                        {
                            var localTypes = definitions.Where(x => string.Equals((string?)x.Attribute("id"), property.Value, StringComparison.OrdinalIgnoreCase))
                                .Select(x => (string?)x.Attribute("type") ?? "").Distinct().ToList();
                            references.Add((localTypes.Count == 1 ? localTypes[0] : "", property.Value, type + "/" + property.Name.LocalName));
                            continue;
                        }
                        if (!_typeNames.TryGetValue(dataSource, out targetType))
                        {
                            var typeItem = PackageXml.Items(await session.QueryAsync(PackageXml.Query("ItemType", dataSource, select: "name"), ct)).FirstOrDefault();
                            targetType = (string?)typeItem?.Element("name");
                            if (targetType != null) _typeNames[dataSource] = targetType;
                        }
                    }
                    if (!string.IsNullOrEmpty(targetType)) references.Add((targetType, property.Value, type + "/" + property.Name.LocalName));
                }
            }
            return references.Distinct().ToList();
        }
    }
}
