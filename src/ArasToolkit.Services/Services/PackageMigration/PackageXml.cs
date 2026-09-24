using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

public static class PackageXml
{
    private static readonly HashSet<string> VolatileProperties = new(StringComparer.OrdinalIgnoreCase)
    { "created_on", "created_by_id", "modified_on", "modified_by_id", "locked_by_id", "generation", "is_current", "config_id", "keyed_name", "new_version", "not_lockable" };

    public static XDocument Parse(string xml)
    {
        using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings
        { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 32 * 1024 * 1024 });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    public static List<XElement> Items(string xml) => Parse(xml).Descendants("Item").Where(x => !x.Ancestors("Item").Any()).ToList();
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    public static string Redact(string value, string secret) => string.IsNullOrEmpty(secret) ? value : value.Replace(secret, "[已隐藏]", StringComparison.OrdinalIgnoreCase);
    public static bool IsId(string? value) => value != null && Regex.IsMatch(value, "\\A[0-9a-fA-F]{32}\\z");

    public static bool IsOwnedPropertyEdit(XElement item)
    {
        if ((string?)item.Attribute("type") != "Property" || (string?)item.Attribute("action") != "edit") return false;
        var match = Regex.Match((string?)item.Attribute("where") ?? "", "\\Asource_id='([0-9a-fA-F]{32})' and name='([a-zA-Z0-9_]+)'\\z");
        return match.Success && item.Ancestors("Item").Any(x => (string?)x.Attribute("type") == "ItemType"
            && string.Equals((string?)x.Attribute("id"), match.Groups[1].Value, StringComparison.OrdinalIgnoreCase))
            && (item.Element("name") == null || item.Element("name")!.Value == match.Groups[2].Value);
    }

    public static void ValidateReadOnly(string aml)
    {
        var document = Parse(aml);
        if (document.Root?.Name != "AML" || !document.Root.Elements("Item").Any()
            || document.Descendants("Item").Any(x => (string?)x.Attribute("action") != "get"))
            throw new InvalidDataException("迁移查询会话仅支持 get AML。");
    }

    public static string Query(string type, string? id = null, string? name = null, string? select = null)
    {
        var item = new XElement("Item", new XAttribute("type", type), new XAttribute("action", "get"));
        if (!string.IsNullOrEmpty(id)) item.SetAttributeValue("id", id);
        if (name != null) item.Add(new XElement("name", name));
        if (select != null) item.SetAttributeValue("select", select);
        return new XElement("AML", item).ToString(SaveOptions.DisableFormatting);
    }

    public static string SafePath(string root, string relative)
    {
        root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (Path.IsPathRooted(relative) || relative.Contains(':')) throw new InvalidDataException("包路径必须在包目录内。");
        var path = Path.GetFullPath(Path.Combine(root, relative));
        if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("包路径超出包目录。");
        var cursor = new DirectoryInfo(Path.GetDirectoryName(path)!);
        while (cursor != null && cursor.FullName.Length >= root.TrimEnd(Path.DirectorySeparatorChar).Length)
        {
            if (cursor.Exists && (cursor.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("包不能通过链接访问其他目录。");
            cursor = cursor.Parent;
        }
        if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("包文件不能是链接。");
        return path;
    }

    public static string Canonical(XElement element)
    {
        XElement Normalize(XElement node)
        {
            if (node.Name == "Item")
            {
                return new XElement("Item", node.Attributes().Where(a => a.Name.LocalName is "id" or "type")
                        .OrderBy(a => a.Name.ToString()).Select(a => new XAttribute(a.Name, a.Name == "id" ? a.Value.ToUpperInvariant() : a.Value)),
                    node.Elements().Where(e => !VolatileProperties.Contains(e.Name.LocalName) && e.Name != "id")
                        .OrderBy(e => e.Name.ToString(), StringComparer.Ordinal).ThenBy(e => (string?)e.Attribute(XNamespace.Xml + "lang"), StringComparer.Ordinal)
                        .Select(Normalize));
            }
            if (node.Name == "Relationships")
                return new XElement(node.Name, node.Elements("Item").OrderBy(RelationshipKey, StringComparer.Ordinal).ThenBy(x => x.ToString(), StringComparer.Ordinal).Select(Normalize));
            var result = new XElement(node.Name, node.Attributes().Where(a => a.Name == XNamespace.Xml + "lang" || a.Name == "is_null")
                .OrderBy(a => a.Name.ToString()).Select(a => new XAttribute(a)));
            if (node.Elements("Item").Any())
            {
                var reference = node.Element("Item")!;
                if (reference.Attribute("id") is { } id) result.Value = id.Value.ToUpperInvariant();
                else result.Add(Normalize(reference)); // Official name-based get references must participate in comparisons.
            }
            else if (node.HasElements) result.Add(node.Elements().Select(Normalize));
            else result.Value = node.Value.Replace("\r\n", "\n");
            return result;
        }
        return Normalize(element).ToString(SaveOptions.DisableFormatting);
    }

    public static string ItemKey(XElement item) => ((string?)item.Attribute("type") ?? "") + ":" + ((string?)item.Attribute("id") ?? "").ToUpperInvariant();

    public static string RelationshipKey(XElement item)
    {
        if (item.Attribute("id") != null) return ItemKey(item);
        if ((string?)item.Attribute("type") == "Property" && item.Attribute("where") is { } condition)
            return "Property:where:" + condition.Value;
        return ((string?)item.Attribute("type") ?? "") + ":" + string.Join("|", new[] { "name", "related_id", "client_event", "server_event", "event" }
            .Select(p => p + "=" + (item.Element(p) is { } value ? Canonical(value) : "")));
    }

    /// <summary>Build a comparison view of official ordered add/edit segments. Original AML is never rewritten for execution.</summary>
    public static XElement MergeSegments(IEnumerable<XElement> segments)
    {
        var list = segments.Select(x => new XElement(x)).ToList();
        if (list.Count == 0) throw new InvalidDataException("缺少 AML 段。");
        var root = list[0];
        void Merge(XElement target, XElement patch)
        {
            foreach (var property in patch.Elements())
            {
                if (property.Name == "Relationships")
                {
                    var relationships = target.Element("Relationships");
                    if (relationships == null) { target.Add(new XElement(property)); continue; }
                    foreach (var child in property.Elements("Item"))
                    {
                        var old = relationships.Elements("Item").FirstOrDefault(x => RelationshipKey(x) == RelationshipKey(child));
                        if (old == null) relationships.Add(new XElement(child)); else Merge(old, child);
                    }
                }
                else
                {
                    var old = target.Elements(property.Name).FirstOrDefault(x => (string?)x.Attribute(XNamespace.Xml + "lang") == (string?)property.Attribute(XNamespace.Xml + "lang"));
                    if (old == null) target.Add(new XElement(property));
                    else if (old.Element("Item") is { } nested && property.Element("Item") is { } next && ItemKey(nested) == ItemKey(next)) Merge(nested, next);
                    else old.ReplaceWith(new XElement(property));
                }
            }
        }
        foreach (var patch in list.Skip(1))
        {
            var target = root.DescendantsAndSelf("Item").FirstOrDefault(x => ItemKey(x) == ItemKey(patch));
            if (target == null) throw new InvalidDataException("修补段找不到所属元素：" + ItemKey(patch));
            Merge(target, patch);
        }
        return root;
    }

    /// <summary>Compare only values carried by the package; never remove target-only relationships.</summary>
    public static List<string> Differences(XElement expected, XElement? actual)
    {
        var differences = new List<string>();
        void Compare(XElement source, XElement? target, string path)
        {
            if (target == null) { differences.Add(path + "：目标缺少"); return; }
            foreach (var property in source.Elements().Where(x => x.Name != "Relationships" && x.Name != "id" && !VolatileProperties.Contains(x.Name.LocalName)))
            {
                var language = (string?)property.Attribute(XNamespace.Xml + "lang");
                var other = target.Elements(property.Name).FirstOrDefault(x => (string?)x.Attribute(XNamespace.Xml + "lang") == language);
                // Server responses often put default-language labels in the non-i18n namespace.
                other ??= target.Elements().FirstOrDefault(x => x.Name.LocalName == property.Name.LocalName && (string?)x.Attribute(XNamespace.Xml + "lang") == language);
                if (Canonical(property) != Canonical(other ?? new XElement(property.Name)))
                {
                    string Short(string text) => text.Length > 160 ? text[..160] + "…" : text;
                    differences.Add($"{path}/{property.Name.LocalName}{(language == null ? "" : "[" + language + "]")}：{Short(other?.Value ?? "(空)")} → {Short(property.Value)}");
                }
                if (property.Element("Item") is { } definition && (string?)definition.Attribute("action") != "get")
                    Compare(definition, other?.Element("Item"), path + "/" + property.Name.LocalName);
            }
            foreach (var relation in source.Element("Relationships")?.Elements("Item") ?? [])
                Compare(relation, target.Element("Relationships")?.Elements("Item").FirstOrDefault(x => RelationshipKey(x) == RelationshipKey(relation)), path + "/" + RelationshipKey(relation));
        }
        Compare(expected, actual, ItemKey(expected));
        return differences;
    }

    public static List<string> DestructiveChanges(XElement source, XElement? target)
    {
        if (target == null) return [];
        var issues = new List<string>();
        var existing = target.DescendantsAndSelf("Item").Where(x => (string?)x.Attribute("type") == "Property")
            .GroupBy(RelationshipKey).ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        foreach (var property in source.DescendantsAndSelf("Item").Where(x => (string?)x.Attribute("type") == "Property"))
        {
            if (!existing.TryGetValue(RelationshipKey(property), out var old)) continue;
            var name = (string?)property.Element("name") ?? "Property";
            foreach (var field in new[] { "data_type", "data_source", "foreign_property", "is_multi_valued" })
                if (property.Element(field) is { } value && value.Value != (string?)old.Element(field)) issues.Add($"{name} 的 {field} 变更需要专门的数据迁移。");
            if (int.TryParse((string?)property.Element("stored_length"), out var next) && int.TryParse((string?)old.Element("stored_length"), out var previous) && next < previous)
                issues.Add($"{name} 的字段长度由 {previous} 缩减为 {next}。");
        }
        return issues;
    }
}
