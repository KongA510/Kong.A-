using System.Text.Json;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

public sealed partial class ArasPackageService
{
    public Task<PackageMigrationPlan> PreflightAsync(MigrationPackage package, string targetConnectionId, string engineDirectory,
        IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default) => Guard("目标预检", async () =>
    {
        var fresh = await PackageFileStore.ReadAsync(package.ManifestPath, cancellationToken);
        if (fresh.Hash != package.Hash) throw new InvalidOperationException("包已变更，请重新载入并比较。");
        var probe = await _engine.ProbeAsync(engineDirectory, cancellationToken);
        if (!probe.IsSupported) throw new InvalidOperationException(probe.Message);
        await using var session = await _sessions.OpenAsync(targetConnectionId, cancellationToken);
        if (fresh.Source?.Identity == session.Endpoint.Identity) throw new InvalidOperationException("源系统与目标系统相同，不能执行迁移。");
        var plan = new PackageMigrationPlan { Target = session.Endpoint, Package = fresh, TargetRelease = session.Endpoint.Version };
        plan.Issues.AddRange(fresh.Issues);
        if (fresh.Source is { Version.Length: > 0 } && fresh.Source.Version != session.Endpoint.Version)
            plan.Issues.Add(new PackageIssue { IsBlocking = true, Message = $"源 {fresh.Source.Version} 与目标 {session.Endpoint.Version} 版本不同，首版不执行跨版本升级或降级。" });
        if (!Version.TryParse(session.Endpoint.Version, out var version) || version.Major != 14 || version.Minor != 0 || version.Build > 28)
            plan.Issues.Add(new PackageIssue { IsBlocking = true, Message = "目标服务器超出当前适配器的版本范围；R37 需要独立验收。" });
        var languages = await LanguagesAsync(session, cancellationToken);
        foreach (var language in fresh.Languages.Except(languages, StringComparer.OrdinalIgnoreCase))
            plan.Issues.Add(new PackageIssue { IsBlocking = true, Message = "目标未安装包语言：" + language });
        foreach (var dependency in fresh.RequiredPackages)
            if (PackageXml.Items(await session.QueryAsync(PackageXml.Query("PackageDefinition", name: dependency, select: "id"), cancellationToken)).Count == 0)
                plan.Issues.Add(new PackageIssue { IsBlocking = true, Message = "目标缺少前置包：" + dependency });

        var metadata = new MetadataReader(session);
        var existing = new Dictionary<string, PackageItemSelection>();
        foreach (var entry in PackageFileStore.EffectiveItems(fresh))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var schema = await metadata.Schema(entry.Item.Type, cancellationToken);
            if (schema == null)
            {
                plan.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = entry.Key, Message = "目标缺少元数据类型：" + entry.Item.Type });
                continue;
            }
            if (!MetadataTypes.Contains(entry.Item.Type))
            {
                var sourceDefinition = schema.Descendants("Item").FirstOrDefault(x => (string?)x.Element("name") == "source_id");
                var ownerId = (string?)sourceDefinition?.Element("data_source");
                var owner = PackageXml.IsId(ownerId) ? PackageXml.Items(await session.QueryAsync(PackageXml.Query("ItemType", ownerId, select: "name"), cancellationToken)).FirstOrDefault() : null;
                if ((string?)schema.Element("is_relationship") != "1" || !MetadataTypes.Contains((string?)owner?.Element("name") ?? ""))
                    plan.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = entry.Key, Message = "首版仅迁移配置及其所属关系，不支持：" + entry.Item.Type });
            }
            var target = await metadata.Find(entry.Item.Type, entry.Item.Id, cancellationToken);
            if (target != null) existing[entry.Key] = target.Value.Selection;
            var source = PackageXml.Parse(entry.Aml).Root!;
            if (source.Element("name") is { } name && metadata.HasProperty(schema, "name"))
            {
                var sameNames = PackageXml.Items(await session.QueryAsync(PackageXml.Query(entry.Item.Type, name: name.Value, select: "id,config_id"), cancellationToken));
                if (sameNames.Any(x => !string.Equals((string?)x.Attribute("id"), target?.Selection.Id ?? entry.Item.Id, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals((string?)x.Element("config_id"), entry.Item.Id, StringComparison.OrdinalIgnoreCase)))
                    plan.Issues.Add(new PackageIssue { IsBlocking = true, ItemKey = entry.Key, Message = "同名不同 ID：" + entry.Item.Type + " / " + name.Value });
            }
        }
        var snapshots = await ExportSnapshotAsync(existing.Values.ToList(), session.Endpoint, fresh.Languages, engineDirectory,
            NewDirectory("preflight"), progress, cancellationToken);
        foreach (var entry in PackageFileStore.EffectiveItems(fresh))
        {
            var expected = PackageXml.Parse(entry.Aml).Root!;
            var before = snapshots.GetValueOrDefault(entry.Key);
            var target = before == null ? null : PackageXml.Parse(before).Root!;
            var changes = PackageXml.Differences(expected, target);
            var blockers = PackageXml.DestructiveChanges(expected, target);
            blockers.AddRange(NestedIdentityConflicts(expected, target));
            if (entry.Item.Type == "SQL" && target != null && changes.Count > 0)
                blockers.Add("官方工具会删除并重建 SQL 定义；首版阻止覆盖已有 SQL。");
            var difference = new PackageDifference
            {
                ItemKey = entry.Key, Name = entry.Item.DisplayName, BeforeAml = before ?? "",
                TargetHash = before == null ? "ABSENT" : PackageXml.Hash(PackageXml.Canonical(target!)),
                Kind = blockers.Count > 0 ? "冲突" : before == null ? "新增" : changes.Count == 0 ? "相同" : "合并",
                IsBlocking = blockers.Count > 0, Details = string.Join(Environment.NewLine, blockers.Concat(changes)),
                Include = blockers.Count == 0 && changes.Count > 0
            };
            plan.Differences.Add(difference);
        }
        plan.Issues.AddRange(await CheckReferencesAsync(fresh, metadata, cancellationToken));
        plan.Issues.AddRange(await CheckResourcesAsync(fresh, session.Endpoint, cancellationToken));
        plan.Issues = plan.Issues.DistinctBy(x => (x.ItemKey, x.Message)).ToList();
        await WriteJsonAsync(Path.Combine(NewDirectory("comparison"), "plan.json"), plan, cancellationToken);
        return plan;
    });

    private async Task<Dictionary<string, string>> ExportSnapshotAsync(List<PackageItemSelection> selections, MigrationEndpoint endpoint,
        List<string> languages, string engineDirectory, string directory, IProgress<PackageMigrationProgress>? progress, CancellationToken ct)
    {
        var snapshots = new Dictionary<string, string>();
        if (selections.Count == 0) return snapshots;
        await _engine.ExportAsync(new PackageEngineExportRequest
        {
            ConnectionId = endpoint.Id, EngineDirectory = engineDirectory, Items = selections, Languages = languages,
            PackageName = "toolkit.backup", OutputDirectory = Path.Combine(directory, "Import")
        }, progress, ct);
        await PackageFileStore.WriteManifestAsync(directory, "toolkit.backup", [], ct);
        var exported = PackageFileStore.EffectiveItems(await PackageFileStore.ReadAsync(Path.Combine(directory, "imports.mf"), ct));
        foreach (var item in selections.DistinctBy(i => i.Key))
        {
            var root = exported.Select(i => PackageXml.Parse(i.Aml).Root!).SelectMany(x => x.DescendantsAndSelf("Item")).FirstOrDefault(x => PackageXml.ItemKey(x) == item.Key)
                ?? throw new InvalidDataException("官方快照缺少元素：" + item.DisplayName);
            snapshots[item.Key] = root.ToString(SaveOptions.DisableFormatting);
        }
        return snapshots;
    }

    private static IEnumerable<string> NestedIdentityConflicts(XElement source, XElement? target)
    {
        if (target == null) yield break;
        foreach (var property in source.Descendants("Item").Where(x => (string?)x.Attribute("type") == "Property"))
        {
            var name = (string?)property.Element("name");
            var old = target.Descendants("Item").FirstOrDefault(x => (string?)x.Attribute("type") == "Property" && (string?)x.Element("name") == name);
            if (old != null && property.Attribute("id") != null && old.Attribute("id") != null && !string.Equals((string?)property.Attribute("id"), (string?)old.Attribute("id"), StringComparison.OrdinalIgnoreCase))
                yield return "属性同名不同 ID：" + name;
        }
    }

    private static async Task<List<PackageIssue>> CheckReferencesAsync(MigrationPackage package, MetadataReader metadata, CancellationToken ct)
    {
        var documents = package.Items.Select(i => PackageXml.Parse(i.Aml)).ToList();
        var contained = documents.SelectMany(x => x.Descendants("Item").Select(PackageXml.ItemKey)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var definitions = documents.SelectMany(x => x.Descendants("Item")).ToList();
        var issues = new List<PackageIssue>();
        foreach (var doc in documents)
        {
            foreach (var reference in await metadata.References(doc, ct, definitions))
            {
                if (string.IsNullOrEmpty(reference.Type) || !PackageXml.IsId(reference.Id))
                {
                    issues.Add(new PackageIssue { IsBlocking = true, Message = "包和目标未能解析引用：" + reference.Reason + " / " + reference.Id });
                    continue;
                }
                var key = reference.Type + ":" + reference.Id.ToUpperInvariant();
                if (contained.Contains(key) || (reference.Type == "CommandBarItem" && contained.Any(k =>
                    k.StartsWith("CommandBar", StringComparison.Ordinal) && k.EndsWith(":" + reference.Id, StringComparison.OrdinalIgnoreCase)))) continue;
                if (await metadata.Find(reference.Type, reference.Id, ct) == null)
                    issues.Add(new PackageIssue { IsBlocking = true, ItemKey = key, Message = "包和目标均缺少依赖：" + key + "（" + reference.Reason + "）" });
            }
            issues.AddRange(ResourceIssues(PackageXml.ItemKey(doc.Root!), doc));
        }
        foreach (var dependency in package.Dependencies.Where(x => x.IsExternal))
            if (!contained.Contains(dependency.Item.Key) && await metadata.Find(dependency.Item.Type, dependency.Item.ExportId, ct) == null)
                issues.Add(new PackageIssue { IsBlocking = true, ItemKey = dependency.Item.Key, Message = "目标缺少前置配置：" + dependency.Item.DisplayName });
        return issues;
    }

    public async Task<PackageMigrationResult> ExecuteAsync(PackageMigrationPlan plan, string engineDirectory,
        IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        if (!plan.CanExecute) throw new InvalidOperationException("比较结果没有可执行项目，或仍有阻断问题。");
        await _executionGate.WaitAsync(cancellationToken);
        var result = new PackageMigrationResult
        {
            Id = plan.Id, Target = plan.Target.DisplayName, UserId = CurrentUserContext.CurrentUserId, Directory = NewDirectory("run"),
            Items = plan.Differences.Where(x => x.Include && x.Kind is "新增" or "合并").Select(x => new PackageItemOutcome { Key = x.ItemKey, Name = x.Name }).ToList()
        };
        bool importStarted = false;
        bool engineCompleted = false;
        var selected = result.Items.Select(x => x.Key).ToHashSet();
        try
        {
            await WriteJsonAsync(Path.Combine(result.Directory, "result.json"), result, cancellationToken);
            progress?.Report(new PackageMigrationProgress { Stage = "执行前复查", Message = "重新读取目标，检查预览之后的修改。" });
            var fresh = await PreflightAsync(plan.Package, plan.Target.Id, engineDirectory, progress, cancellationToken);
            if (fresh.Target.Identity != plan.Target.Identity || fresh.Target.Version != plan.Target.Version)
                throw new InvalidOperationException("目标连接或版本已改变，请重新比较。");
            if (fresh.Issues.Any(x => x.IsBlocking) || fresh.Differences.Any(x => x.IsBlocking)) throw new InvalidOperationException("执行前复查出现阻断问题，请重新比较。");
            foreach (var previous in plan.Differences)
            {
                var current = fresh.Differences.SingleOrDefault(x => x.ItemKey == previous.ItemKey);
                if (current == null || current.TargetHash != previous.TargetHash) throw new InvalidOperationException("目标配置已变化：" + previous.Name + "，请重新比较。");
            }
            // A deselected new dependency must not silently become a missing reference.
            var selectionPackage = new MigrationPackage { Items = fresh.Package.Items.Where(i => selected.Contains(i.SelectionKey)).ToList(), Dependencies = fresh.Package.Dependencies };
            await using (var session = await _sessions.OpenAsync(plan.Target.Id, cancellationToken))
            {
                var metadata = new MetadataReader(session);
                var missing = await CheckReferencesAsync(selectionPackage, metadata, cancellationToken);
                if (missing.Any(x => x.IsBlocking)) throw new InvalidOperationException(string.Join(Environment.NewLine, missing.Where(x => x.IsBlocking).Select(x => x.Message)));
                var backupItems = new List<PackageItemSelection>();
                foreach (var item in PackageFileStore.EffectiveItems(selectionPackage))
                {
                    var existing = await metadata.Find(item.Item.Type, item.Item.Id, cancellationToken);
                    if (existing != null) backupItems.Add(existing.Value.Selection);
                }
                progress?.Report(new PackageMigrationProgress { Stage = "目标备份", Message = "保存目标原始 AML 和新增项清单。" });
                var backupDirectory = Path.Combine(result.Directory, "backup");
                Directory.CreateDirectory(backupDirectory);
                var packageNames = fresh.Package.Items.Where(i => selected.Contains(i.SelectionKey)).Select(i => i.PackageName).Distinct().ToList();
                var packageRecords = new XElement("AML");
                var newPackages = new List<string>();
                foreach (var name in packageNames)
                {
                    var query = new XElement("Item", new XAttribute("type", "PackageDefinition"), new XAttribute("action", "get"), new XElement("name", name),
                        new XElement("Relationships", new XElement("Item", new XAttribute("type", "PackageGroup"), new XAttribute("action", "get"),
                            new XElement("Relationships", new XElement("Item", new XAttribute("type", "PackageElement"), new XAttribute("action", "get"))))));
                    var rows = PackageXml.Items(await session.QueryAsync(new XElement("AML", query).ToString(), cancellationToken));
                    packageRecords.Add(rows);
                    if (rows.Count == 0) newPackages.Add(name);
                }
                await File.WriteAllTextAsync(Path.Combine(backupDirectory, "package-records-before.xml"), packageRecords.ToString(), cancellationToken);
                await WriteJsonAsync(Path.Combine(backupDirectory, "new-package-names.json"), newPackages, cancellationToken);
                var before = await ExportSnapshotAsync(backupItems, session.Endpoint, fresh.Package.Languages, engineDirectory, backupDirectory, progress, cancellationToken);
                foreach (var difference in fresh.Differences.Where(x => selected.Contains(x.ItemKey) && x.BeforeAml.Length > 0))
                    if (!before.TryGetValue(difference.ItemKey, out var xml) || PackageXml.Hash(PackageXml.Canonical(PackageXml.Parse(xml).Root!)) != difference.TargetHash)
                        throw new InvalidOperationException("备份过程中目标配置发生变化，请重新比较。");
                await WriteJsonAsync(Path.Combine(backupDirectory, "new-items.json"), fresh.Differences.Where(x => selected.Contains(x.ItemKey) && x.Kind == "新增").Select(x => x.ItemKey).ToList(), cancellationToken);
                var previousIds = before.Values.SelectMany(xml => PackageXml.Parse(xml).Descendants("Item").Select(PackageXml.ItemKey)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var owned = PackageFileStore.EffectiveItems(selectionPackage).SelectMany(entry => PackageXml.Parse(entry.Aml).Descendants("Item")
                    .Where(x => (string?)x.Attribute("action") != "get" && PackageXml.IsId((string?)x.Attribute("id")) && !previousIds.Contains(PackageXml.ItemKey(x)))
                    .Select(x => new { Root = entry.Key, Item = PackageXml.ItemKey(x), Name = (string?)x.Element("name") ?? "" })).Distinct().ToList();
                await WriteJsonAsync(Path.Combine(backupDirectory, "new-owned-ids.json"), owned, cancellationToken);
                await WriteJsonAsync(Path.Combine(result.Directory, "reviewed-plan.json"), plan, cancellationToken);
            }
            var manifest = await PackageFileStore.CopySelectionAsync(fresh.Package, selected, Path.Combine(result.Directory, "payload"), cancellationToken);
            // Re-read the exact payload that will be handed to the official engine.
            var payload = await PackageFileStore.ReadAsync(manifest, cancellationToken);
            await PackageFileStore.SealAsync(payload, cancellationToken);
            await using (var session = await _sessions.OpenAsync(plan.Target.Id, cancellationToken))
                foreach (var dependency in payload.RequiredPackages)
                    if (PackageXml.Items(await session.QueryAsync(PackageXml.Query("PackageDefinition", name: dependency, select: "id"), cancellationToken)).Count == 0)
                        throw new InvalidOperationException("取消勾选后，目标仍缺少所选包的前置包：" + dependency);
            cancellationToken.ThrowIfCancellationRequested();
            result.Status = "迁入中";
            await WriteJsonAsync(Path.Combine(result.Directory, "result.json"), result, cancellationToken);
            importStarted = true;
            await _engine.ImportAsync(new PackageEngineImportRequest
            {
                ConnectionId = plan.Target.Id, EngineDirectory = engineDirectory, ManifestPath = manifest,
                TargetRelease = fresh.TargetRelease, Description = "工具箱迁移 " + result.Id, Languages = fresh.Package.Languages
            }, progress, cancellationToken);
            engineCompleted = true;
            result.Status = "回读中";
        }
        catch (OperationCanceledException ex)
        {
            result.Status = "已取消"; result.Message = "已取消，正在核对已写入项目。";
            await _errors.LogErrorAsync("导包-取消迁入", ex.Message, ErrorLog.LevelP1);
        }
        catch (Exception ex)
        {
            result.Status = "失败"; result.Message = ex.Message;
            await _errors.LogErrorAsync("导包-执行迁入", ex.Message, ErrorLog.LevelP1);
        }
        finally
        {
            try
            {
                if (importStarted)
                {
                    try
                    {
                        await VerifyAsync(plan, selected, result, engineDirectory, progress);
                        if (engineCompleted) result.Status = result.Items.All(i => i.Status == "已验证") ? "成功" : "验证失败";
                    }
                    catch (Exception ex)
                    {
                        foreach (var item in result.Items.Where(x => x.Status != "已验证")) item.Status = "结果不确定";
                        result.Message += Environment.NewLine + "回读失败：" + ex.Message;
                        if (engineCompleted) result.Status = "结果不确定";
                        await _errors.LogErrorAsync("导包-回读验证", ex.Message, ErrorLog.LevelP0);
                    }
                    await AuditAsync("Import", result.Id, $"目标 {plan.Target.Database}；{result.Status}；已验证 {result.Items.Count(i => i.Status == "已验证")}/{result.Items.Count}；记录 {result.Directory}");
                }
                await WriteJsonAsync(Path.Combine(result.Directory, "result.json"), result, CancellationToken.None);
            }
            finally { _executionGate.Release(); }
        }
        return result;
    }

    private async Task VerifyAsync(PackageMigrationPlan plan, HashSet<string> selected, PackageMigrationResult result,
        string engineDirectory, IProgress<PackageMigrationProgress>? progress)
    {
        await using var session = await _sessions.OpenAsync(plan.Target.Id, CancellationToken.None);
        var metadata = new MetadataReader(session);
        var items = new List<PackageItemSelection>();
        foreach (var entry in PackageFileStore.EffectiveItems(plan.Package).Where(i => selected.Contains(i.Key)))
        {
            var current = await metadata.Find(entry.Item.Type, entry.Item.Id, CancellationToken.None);
            if (current != null) items.Add(current.Value.Selection);
        }
        var snapshots = await ExportSnapshotAsync(items, session.Endpoint, plan.Package.Languages, engineDirectory,
            Path.Combine(result.Directory, "after"), progress, CancellationToken.None);
        foreach (var outcome in result.Items)
        {
            if (!snapshots.TryGetValue(outcome.Key, out var xml)) { outcome.Status = "未写入"; continue; }
            var expected = PackageFileStore.EffectiveItems(plan.Package).Single(i => i.Key == outcome.Key);
            var differences = PackageXml.Differences(PackageXml.Parse(expected.Aml).Root!, PackageXml.Parse(xml).Root!);
            outcome.Status = differences.Count == 0 ? "已验证" : "内容不一致";
            outcome.Message = string.Join(Environment.NewLine, differences);
        }
    }

    public Task<List<PackageMigrationResult>> GetHistoryAsync(CancellationToken cancellationToken = default) => Guard("执行历史", async () =>
    {
        if (!Directory.Exists(UserDirectory)) return [];
        var results = new List<PackageMigrationResult>();
        foreach (var file in Directory.GetDirectories(UserDirectory, "run-*").Select(d => Path.Combine(d, "result.json")).Where(File.Exists))
        {
            var entry = JsonSerializer.Deserialize<PackageMigrationResult>(await File.ReadAllTextAsync(file, cancellationToken));
            if (entry != null && entry.UserId == CurrentUserContext.CurrentUserId)
            {
                if (entry.Status is "准备" or "迁入中" or "回读中") entry.Status = "未确认完成，请重新比较目标";
                results.Add(entry);
            }
        }
        return results.OrderByDescending(x => x.CreatorOn).ToList();
    });
}
