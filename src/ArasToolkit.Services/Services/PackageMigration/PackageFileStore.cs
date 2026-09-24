using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>Filesystem format shared with the official utilities. No server access.</summary>
public static class PackageFileStore
{
    public static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private const long MaxBytes = 256L * 1024 * 1024;

    public static async Task<string> UnzipAsync(string zip, string destination, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destination);
        using var archive = ZipFile.OpenRead(zip);
        if (archive.Entries.Count > 10000 || archive.Entries.Sum(e => e.Length) > MaxBytes) throw new InvalidDataException("包超过 256 MB 或 10000 个文件的限制。");
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = PackageXml.SafePath(destination, entry.FullName.Replace('/', Path.DirectorySeparatorChar));
            if (!names.Add(path)) throw new InvalidDataException("ZIP 中存在重复路径。");
            if (entry.FullName.EndsWith('/')) { Directory.CreateDirectory(path); continue; }
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var from = entry.Open();
            await using var to = new FileStream(path, FileMode.CreateNew);
            await from.CopyToAsync(to, cancellationToken);
        }
        var manifests = Directory.GetFiles(destination, "*.mf", SearchOption.AllDirectories);
        if (manifests.Length != 1) throw new InvalidDataException("ZIP 必须包含唯一的 .mf 清单，请分开选择多个包。");
        return manifests[0];
    }

    public static async Task<MigrationPackage> ReadAsync(string manifest, CancellationToken cancellationToken = default)
    {
        manifest = Path.GetFullPath(manifest);
        var root = Path.GetDirectoryName(manifest)!;
        var doc = PackageXml.Parse(await File.ReadAllTextAsync(manifest, cancellationToken));
        if (doc.Root?.Name != "imports") throw new InvalidDataException("清单根节点必须为 imports。");
        var result = new MigrationPackage { RootDirectory = root, ManifestPath = manifest };
        var packages = doc.Root.Elements("package").ToList();
        var names = packages.Select(p => (string?)p.Attribute("name") ?? "").ToList();
        if (names.Any(string.IsNullOrWhiteSpace) || names.Distinct(StringComparer.Ordinal).Count() != names.Count || names.Count == 0)
            throw new InvalidDataException("清单包名为空或重复。");
        var seenFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var package in packages)
        {
            var name = (string)package.Attribute("name")!;
            var relative = (string?)package.Attribute("path") ?? throw new InvalidDataException("清单缺少 path。");
            // The official core-package convention expands these package names into folders.
            if (name is "com.aras.innovator.core" or "com.aras.innovator.admin" or "com.aras.innovator.dashboards" or "com.aras.innovator.preferences")
                relative = Path.Combine(relative, name.Replace('.', Path.DirectorySeparatorChar));
            var directory = PackageXml.SafePath(root, Path.Combine(relative, ".package-directory-marker"));
            directory = Path.GetDirectoryName(directory)!;
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("缺少包目录：" + relative);
            result.RequiredPackages.AddRange(package.Elements("dependson").Select(x => (string?)x.Attribute("name") ?? "").Where(x => !names.Contains(x)));
            foreach (var file in Directory.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal))
            {
                cancellationToken.ThrowIfCancellationRequested();
                PackageXml.SafePath(root, Path.GetRelativePath(root, file));
                if (!seenFiles.Add(file)) throw new InvalidDataException("多个 package 指向相同 AML 文件。");
                if (seenFiles.Count > 10000 || new FileInfo(file).Length > 32 * 1024 * 1024) throw new InvalidDataException("AML 文件数量或大小超限。");
                var xml = PackageXml.Parse(await File.ReadAllTextAsync(file, cancellationToken));
                if (xml.Root?.Name != "AML") throw new InvalidDataException("AML 文件根节点错误：" + file);
                foreach (var item in xml.Root.Elements("Item"))
                {
                    var type = (string?)item.Attribute("type") ?? "";
                    var id = (string?)item.Attribute("id") ?? "";
                    if ((string?)item.Attribute("action") is "delete" or "purge") throw new InvalidDataException("首版禁止官方包中的删除/清理动作：" + type + "；请单独核对该操作。");
                    if (!PackageXml.IsId(id) || string.IsNullOrWhiteSpace(type)) throw new InvalidDataException("包元素必须有 type 和稳定 ID：" + file);
                    foreach (var child in item.DescendantsAndSelf("Item"))
                    {
                        var action = (string?)child.Attribute("action") ?? "add";
                        if (action is not ("add" or "edit" or "merge" or "get"))
                            throw new InvalidDataException($"首版禁止动作 {action}：{type} / {id}");
                        if ((child.Attribute("where") != null && !PackageXml.IsOwnedPropertyEdit(child)) || child.Attribute("idlist") != null)
                            throw new InvalidDataException("包包含批量条件操作，需改为显式 ID 后迁入。");
                    }
                    if ((string?)item.Attribute("action") == "get") throw new InvalidDataException("包顶层只能包含待迁入的元数据。");
                    // Owned relationships (e.g. Member) can be additional roots in an official file.
                    // Their metadata owner is validated against the target schema during preflight.
                    result.Items.Add(new PackageFileItem
                    {
                        PackageName = name, File = Path.GetRelativePath(root, file), Aml = item.ToString(SaveOptions.DisableFormatting),
                        Item = new PackageItemSelection { Type = type, Id = id, Name = (string?)item.Element("name") ?? (string?)item.Attribute("keyed_name") ?? id }
                    });
                }
                result.Languages.AddRange(xml.Descendants().Attributes(XNamespace.Xml + "lang").Select(a => a.Value));
            }
        }
        if (result.Items.Count == 0) throw new InvalidDataException("包中没有可迁入元素。");
        AssignOwners(result.Items);
        result.Languages = result.Languages.Append("en").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        result.RequiredPackages = result.RequiredPackages.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        var info = Path.Combine(root, "package-info.json");
        if (File.Exists(info))
        {
            var metadata = JsonSerializer.Deserialize<PackageMetadata>(await File.ReadAllTextAsync(info, cancellationToken), JsonOptions)
                ?? throw new InvalidDataException("包说明无效。");
            result.Source = metadata.Source; result.EngineVersion = metadata.EngineVersion;
            result.Dependencies = metadata.Dependencies; result.Issues.AddRange(metadata.Issues);
        }
        var hashes = await FileHashesAsync(result, cancellationToken);
        var checksums = Path.Combine(root, "checksums.json");
        if (File.Exists(checksums))
        {
            var expected = JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(checksums, cancellationToken))!;
            if (expected.Count != hashes.Count || hashes.Any(x => !expected.TryGetValue(x.Key, out var hash) || hash != x.Value))
                throw new InvalidDataException("包校验失败：清单或 AML 已被修改。请重新导出或重新生成校验文件。");
        }
        result.Hash = PackageXml.Hash(string.Join("\n", hashes.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Key + "=" + x.Value)));
        return result;
    }

    public static void AssignOwners(List<PackageFileItem> items)
    {
        var definitions = items.Select(i => (Entry: i, Xml: PackageXml.Parse(i.Aml).Root!)).ToList();
        foreach (var item in items)
        {
            var owner = definitions.FirstOrDefault(x => x.Entry.Key != item.Key && x.Xml.Descendants("Item").Any(n =>
                PackageXml.ItemKey(n) == item.Key && (string?)n.Attribute("action") != "get" && n.HasElements));
            if (owner.Entry != null) item.OwnerKey = owner.Entry.Key;
        }
        foreach (var item in items)
        {
            var visited = new HashSet<string> { item.Key };
            while (item.OwnerKey.Length > 0)
            {
                if (!visited.Add(item.OwnerKey)) throw new InvalidDataException("AML 所属元素形成循环：" + item.Key);
                var parent = items.First(x => x.Key == item.OwnerKey);
                if (parent.OwnerKey.Length == 0) break;
                item.OwnerKey = parent.OwnerKey;
            }
        }
    }

    public static List<PackageFileItem> EffectiveItems(MigrationPackage package) => package.Items.GroupBy(i => i.SelectionKey).Select(g =>
    {
        var root = g.First(i => i.Key == g.Key);
        var ordered = g.Where(i => i.Key == g.Key).Concat(g.Where(i => i.Key != g.Key));
        return new PackageFileItem { Item = root.Item, File = root.File, PackageName = root.PackageName,
            Aml = PackageXml.MergeSegments(ordered.Select(i => PackageXml.Parse(i.Aml).Root!)).ToString(SaveOptions.DisableFormatting) };
    }).ToList();

    public static async Task<Dictionary<string, string>> FileHashesAsync(MigrationPackage package, CancellationToken cancellationToken)
    {
        var files = package.Items.Select(i => i.File).Append(Path.GetRelativePath(package.RootDirectory, package.ManifestPath));
        if (File.Exists(Path.Combine(package.RootDirectory, "package-info.json"))) files = files.Append("package-info.json");
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in files.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await using var stream = File.OpenRead(PackageXml.SafePath(package.RootDirectory, file));
            result[file] = Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken));
        }
        return result;
    }

    public static async Task WriteManifestAsync(string root, string name, IEnumerable<string> dependencies, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(root);
        var manifest = new XDocument(new XElement("imports", new XElement("package", new XAttribute("name", name),
            new XAttribute("path", "Import"), dependencies.Distinct().Order().Select(d => new XElement("dependson", new XAttribute("name", d))))));
        await File.WriteAllTextAsync(Path.Combine(root, "imports.mf"), manifest.ToString(), cancellationToken);
    }

    public static async Task WriteMetadataAsync(string root, PackageMetadata metadata, CancellationToken cancellationToken)
        => await File.WriteAllTextAsync(Path.Combine(root, "package-info.json"), JsonSerializer.Serialize(metadata, JsonOptions), cancellationToken);

    public static async Task SealAsync(MigrationPackage package, CancellationToken cancellationToken)
        => await File.WriteAllTextAsync(Path.Combine(package.RootDirectory, "checksums.json"), JsonSerializer.Serialize(await FileHashesAsync(package, cancellationToken), JsonOptions), cancellationToken);

    public static async Task<string> CopySelectionAsync(MigrationPackage package, HashSet<string> selected, string destination, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destination);
        // Retain directory names (including Fixes), package names and manifest order.
        var manifest = PackageXml.Parse(await File.ReadAllTextAsync(package.ManifestPath, cancellationToken));
        var includedPackages = package.Items.Where(i => selected.Contains(i.SelectionKey)).Select(i => i.PackageName).ToHashSet();
        foreach (var node in manifest.Root!.Elements("package").ToList())
            if (!includedPackages.Contains((string)node.Attribute("name")!)) node.Remove();
        foreach (var group in package.Items.Where(i => selected.Contains(i.SelectionKey)).GroupBy(i => i.File))
        {
            var path = PackageXml.SafePath(destination, group.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, new XElement("AML", group.Select(i => PackageXml.Parse(i.Aml).Root!)).ToString(), cancellationToken);
        }
        var manifestPath = Path.Combine(destination, Path.GetFileName(package.ManifestPath));
        await File.WriteAllTextAsync(manifestPath, manifest.ToString(), cancellationToken);
        return manifestPath;
    }

    public sealed class PackageMetadata
    {
        public MigrationEndpoint? Source { get; set; }
        public string EngineVersion { get; set; } = "";
        public List<PackageDependency> Dependencies { get; set; } = [];
        public List<PackageIssue> Issues { get; set; } = [];
    }
}
