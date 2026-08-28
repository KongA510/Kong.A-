using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>结构化数据格式化、语义归一化比对与 .NET 实体样例转换。</summary>
public sealed class DataToolService : IDataToolService
{
    private const int MaximumInputLength = 2_000_000;
    private const int MaximumComparisonLines = 10_000;
    private static readonly Regex XmlDeclarationRegex = new(
        "^<\\?xml\\s+(?<body>.*?)\\?>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex XmlDeclarationValueRegex = new(
        "(?<name>version|encoding|standalone)\\s*=\\s*(['\"])(?<value>.*?)\\2",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ClassRegex = new(
        @"\bclass\s+(?<name>@?[\p{L}_][\p{L}\p{N}_]*)", RegexOptions.Compiled);
    private static readonly Regex PropertyRegex = new(
        "(?:\\[\\s*(?:System\\.Text\\.Json\\.Serialization\\.)?JsonPropertyName\\s*\\(\\s*\\\"(?<json>(?:\\\\.|[^\\\"])*)\\\"\\s*\\)\\s*\\]\\s*)?" +
        "public\\s+(?:required\\s+)?(?<type>[\\p{L}\\p{N}_@.<>\\[\\],?\\s]+?)\\s+(?<name>@?[\\p{L}_][\\p{L}\\p{N}_]*)\\s*\\{\\s*get\\s*;\\s*(?:set|init)\\s*;\\s*\\}",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private readonly IErrorLogService _errorLogService;

    public DataToolService(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    public Task<string> FormatXmlAsync(string input)
        => ExecuteAsync("其他功能-XML格式化", () => FormatXml(input));

    public Task<DataComparisonResult> CompareXmlAsync(string left, string right)
        => ExecuteAsync("其他功能-XML比对", () =>
        {
            var leftFormatted = FormatXml(left);
            var rightFormatted = FormatXml(right);
            return CompareFormattedText(leftFormatted, rightFormatted);
        });

    public Task<string> FormatJsonAsync(string input)
        => ExecuteAsync("其他功能-JSON格式化", () => FormatJson(input, sortProperties: false));

    public Task<DataComparisonResult> CompareJsonAsync(string left, string right)
        => ExecuteAsync("其他功能-JSON比对", () =>
        {
            // 对象属性按名称排序，避免仅属性顺序不同造成伪差异；数组顺序仍保留并参与比对。
            var leftFormatted = FormatJson(left, sortProperties: true);
            var rightFormatted = FormatJson(right, sortProperties: true);
            return CompareFormattedText(leftFormatted, rightFormatted);
        });

    public Task<string> JsonToEntityAsync(string json, string rootClassName)
        => ExecuteAsync("其他功能-JSON转实体类", () => GenerateEntityClasses(json, rootClassName));

    public Task<string> EntityToJsonAsync(string entityCode)
        => ExecuteAsync("其他功能-实体类转JSON", () => GenerateJsonSample(entityCode));

    private async Task<T> ExecuteAsync<T>(string functionName, Func<T> action)
    {
        try
        {
            return await Task.Run(action);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync(functionName, ex.Message, ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    private static string FormatXml(string input)
    {
        GuardInput(input, "XML");
        var document = ExtractXmlDocument(input);
        if (document.Root == null)
            throw new FormatException("未找到有效的 XML 根节点。");

        SortXmlAttributes(document.Root);
        var builder = new StringBuilder();
        if (document.Declaration != null)
        {
            builder.Append("<?xml version=\"")
                .Append(string.IsNullOrWhiteSpace(document.Declaration.Version) ? "1.0" : document.Declaration.Version)
                .Append('"');
            if (!string.IsNullOrWhiteSpace(document.Declaration.Encoding))
                builder.Append(" encoding=\"").Append(document.Declaration.Encoding).Append('"');
            if (!string.IsNullOrWhiteSpace(document.Declaration.Standalone))
                builder.Append(" standalone=\"").Append(document.Declaration.Standalone).Append('"');
            builder.AppendLine("?>");
        }

        builder.Append(document.Root.ToString(SaveOptions.None));
        return builder.ToString();
    }

    private static XDocument ExtractXmlDocument(string input)
    {
        var text = input.Trim().TrimStart('\uFEFF');
        var candidates = new List<int>();
        var declarationIndex = text.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
        if (declarationIndex >= 0)
            candidates.Add(declarationIndex);

        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] != '<' || candidates.Contains(index))
                continue;
            if (index + 1 >= text.Length || text[index + 1] is '/' or '!' or '?')
                continue;
            candidates.Add(index);
        }

        foreach (var start in candidates)
        {
            try
            {
                var fragment = text[start..];
                var declaration = ReadXmlDeclaration(fragment, out var contentOffset);
                var settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    IgnoreWhitespace = true,
                    IgnoreComments = false,
                    IgnoreProcessingInstructions = false
                };
                using var stringReader = new StringReader(fragment[contentOffset..]);
                using var reader = XmlReader.Create(stringReader, settings);
                reader.MoveToContent();
                if (XNode.ReadFrom(reader) is XElement root)
                    return new XDocument(declaration, root);
            }
            catch (XmlException)
            {
                // 当前“<”不是有效根节点，继续尝试后续候选节点。
            }
            catch (InvalidOperationException)
            {
                // 当前候选节点无法形成完整文档，继续查找。
            }
        }

        throw new FormatException("未提取到完整、有效的 XML。请检查根节点、结束标签和属性引号。");
    }

    private static XDeclaration? ReadXmlDeclaration(string fragment, out int contentOffset)
    {
        contentOffset = 0;
        var match = XmlDeclarationRegex.Match(fragment);
        if (!match.Success)
            return null;

        contentOffset = match.Length;
        string version = "1.0";
        string? encoding = null;
        string? standalone = null;
        foreach (Match valueMatch in XmlDeclarationValueRegex.Matches(match.Groups["body"].Value))
        {
            var name = valueMatch.Groups["name"].Value.ToLowerInvariant();
            var value = valueMatch.Groups["value"].Value;
            if (name == "version") version = value;
            else if (name == "encoding") encoding = value;
            else if (name == "standalone") standalone = value;
        }
        return new XDeclaration(version, encoding, standalone);
    }

    private static void SortXmlAttributes(XElement root)
    {
        foreach (var element in root.DescendantsAndSelf())
        {
            var attributes = element.Attributes()
                .OrderBy(attribute => attribute.IsNamespaceDeclaration ? 0 : 1)
                .ThenBy(attribute => attribute.Name.NamespaceName, StringComparer.Ordinal)
                .ThenBy(attribute => attribute.Name.LocalName, StringComparer.Ordinal)
                .Select(attribute => new XAttribute(attribute))
                .ToList();
            element.RemoveAttributes();
            element.Add(attributes);
        }
    }

    private static string FormatJson(string input, bool sortProperties)
    {
        GuardInput(input, "JSON");
        using var document = ExtractJsonDocument(input);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            WriteJsonElement(document.RootElement, writer, sortProperties);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static JsonDocument ExtractJsonDocument(string input)
    {
        var options = new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
            MaxDepth = 128
        };
        var text = input.Trim().TrimStart('\uFEFF');
        try
        {
            return JsonDocument.Parse(text, options);
        }
        catch (JsonException)
        {
            // 支持从日志或说明文本中提取第一个完整的对象/数组。
        }

        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] is not ('{' or '['))
                continue;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(text[index..]);
                var reader = new Utf8JsonReader(bytes, new JsonReaderOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                    MaxDepth = 128
                });
                if (!reader.Read())
                    continue;
                return JsonDocument.ParseValue(ref reader);
            }
            catch (JsonException)
            {
                // 继续尝试下一个对象或数组起点。
            }
        }

        throw new FormatException("未提取到完整、有效的 JSON。请检查括号、逗号和字符串引号。");
    }

    private static void WriteJsonElement(JsonElement element, Utf8JsonWriter writer, bool sortProperties)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                IEnumerable<JsonProperty> properties = element.EnumerateObject();
                if (sortProperties)
                    properties = properties.OrderBy(property => property.Name, StringComparer.Ordinal);
                foreach (var property in properties)
                {
                    writer.WritePropertyName(property.Name);
                    WriteJsonElement(property.Value, writer, sortProperties);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                    WriteJsonElement(item, writer, sortProperties);
                writer.WriteEndArray();
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static DataComparisonResult CompareFormattedText(string left, string right)
    {
        var leftLines = SplitLines(left);
        var rightLines = SplitLines(right);
        if (leftLines.Length + rightLines.Length > MaximumComparisonLines)
            throw new FormatException($"格式化后的总行数不能超过 {MaximumComparisonLines:N0} 行。");

        var atoms = BuildMyersDiff(leftLines, rightLines);
        var lines = CoalesceChangedBlocks(atoms);
        return new DataComparisonResult
        {
            LeftFormatted = left,
            RightFormatted = right,
            Lines = lines,
            RemovedCount = lines.Count(line => line.Kind == DataDiffKind.Removed),
            ModifiedCount = lines.Count(line => line.Kind == DataDiffKind.Modified),
            AddedCount = lines.Count(line => line.Kind == DataDiffKind.Added)
        };
    }

    private static string[] SplitLines(string text)
        => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n');

    private static List<DiffAtom> BuildMyersDiff(IReadOnlyList<string> left, IReadOnlyList<string> right)
    {
        var maximum = left.Count + right.Count;
        var vector = new Dictionary<int, int> { [1] = 0 };
        var trace = new List<Dictionary<int, int>>();

        for (var distance = 0; distance <= maximum; distance++)
        {
            trace.Add(new Dictionary<int, int>(vector));
            for (var diagonal = -distance; diagonal <= distance; diagonal += 2)
            {
                int x;
                if (diagonal == -distance ||
                    (diagonal != distance && GetVectorValue(vector, diagonal - 1) < GetVectorValue(vector, diagonal + 1)))
                {
                    x = GetVectorValue(vector, diagonal + 1);
                }
                else
                {
                    x = GetVectorValue(vector, diagonal - 1) + 1;
                }

                var y = x - diagonal;
                while (x < left.Count && y < right.Count &&
                       string.Equals(left[x], right[y], StringComparison.Ordinal))
                {
                    x++;
                    y++;
                }
                vector[diagonal] = x;
                if (x >= left.Count && y >= right.Count)
                    return BacktrackDiff(trace, left, right, distance);
            }
        }

        return [];
    }

    private static List<DiffAtom> BacktrackDiff(
        IReadOnlyList<Dictionary<int, int>> trace,
        IReadOnlyList<string> left,
        IReadOnlyList<string> right,
        int finalDistance)
    {
        var result = new List<DiffAtom>();
        var x = left.Count;
        var y = right.Count;

        for (var distance = finalDistance; distance >= 0; distance--)
        {
            var vector = trace[distance];
            var diagonal = x - y;
            var previousDiagonal = diagonal == -distance ||
                                   (diagonal != distance &&
                                    GetVectorValue(vector, diagonal - 1) < GetVectorValue(vector, diagonal + 1))
                ? diagonal + 1
                : diagonal - 1;
            var previousX = GetVectorValue(vector, previousDiagonal);
            var previousY = previousX - previousDiagonal;

            while (x > previousX && y > previousY)
            {
                result.Add(new DiffAtom(DataDiffKind.Equal, x, y, left[x - 1], right[y - 1]));
                x--;
                y--;
            }

            if (distance == 0)
                break;
            if (x == previousX)
            {
                result.Add(new DiffAtom(DataDiffKind.Added, null, y, string.Empty, right[y - 1]));
                y--;
            }
            else
            {
                result.Add(new DiffAtom(DataDiffKind.Removed, x, null, left[x - 1], string.Empty));
                x--;
            }
        }

        result.Reverse();
        return result;
    }

    private static int GetVectorValue(IReadOnlyDictionary<int, int> vector, int key)
        => vector.TryGetValue(key, out var value) ? value : 0;

    private static List<DataDiffLine> CoalesceChangedBlocks(IReadOnlyList<DiffAtom> atoms)
    {
        var result = new List<DataDiffLine>();
        for (var index = 0; index < atoms.Count;)
        {
            if (atoms[index].Kind == DataDiffKind.Equal)
            {
                result.Add(ToDiffLine(atoms[index]));
                index++;
                continue;
            }

            var removed = new List<DiffAtom>();
            var added = new List<DiffAtom>();
            while (index < atoms.Count && atoms[index].Kind != DataDiffKind.Equal)
            {
                if (atoms[index].Kind == DataDiffKind.Removed) removed.Add(atoms[index]);
                else added.Add(atoms[index]);
                index++;
            }

            var pairedCount = Math.Min(removed.Count, added.Count);
            for (var pair = 0; pair < pairedCount; pair++)
            {
                result.Add(new DataDiffLine
                {
                    LeftLineNumber = removed[pair].LeftLineNumber,
                    RightLineNumber = added[pair].RightLineNumber,
                    LeftText = removed[pair].LeftText,
                    RightText = added[pair].RightText,
                    Kind = DataDiffKind.Modified
                });
            }
            for (var item = pairedCount; item < removed.Count; item++) result.Add(ToDiffLine(removed[item]));
            for (var item = pairedCount; item < added.Count; item++) result.Add(ToDiffLine(added[item]));
        }
        return result;
    }

    private static DataDiffLine ToDiffLine(DiffAtom atom) => new()
    {
        LeftLineNumber = atom.LeftLineNumber,
        RightLineNumber = atom.RightLineNumber,
        LeftText = atom.LeftText,
        RightText = atom.RightText,
        Kind = atom.Kind
    };

    private static string GenerateEntityClasses(string json, string rootClassName)
    {
        GuardInput(json, "JSON");
        using var document = ExtractJsonDocument(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new FormatException("JSON 转实体类要求根节点为对象（{ ... }）。");

        var safeRootName = ToIdentifier(rootClassName, "RootEntity");
        var context = new EntityGenerationContext();
        BuildClassDefinition(context, safeRootName, [document.RootElement]);

        var builder = new StringBuilder();
        builder.AppendLine("using System;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Text.Json.Serialization;")
            .AppendLine();
        for (var index = 0; index < context.Definitions.Count; index++)
        {
            WriteClassDefinition(builder, context.Definitions[index]);
            if (index < context.Definitions.Count - 1) builder.AppendLine();
        }
        return builder.ToString().TrimEnd();
    }

    private static string BuildClassDefinition(
        EntityGenerationContext context,
        string requestedName,
        IReadOnlyList<JsonElement> objects)
    {
        var className = context.ReserveClassName(requestedName);
        var definition = new EntityClassDefinition(className);
        context.Definitions.Add(definition);

        var propertyOrder = new List<string>();
        var propertyValues = new Dictionary<string, List<JsonElement>>(StringComparer.Ordinal);
        foreach (var objectElement in objects)
        {
            foreach (var property in objectElement.EnumerateObject())
            {
                if (!propertyValues.TryGetValue(property.Name, out var values))
                {
                    values = [];
                    propertyValues[property.Name] = values;
                    propertyOrder.Add(property.Name);
                }
                values.Add(property.Value);
            }
        }

        foreach (var jsonName in propertyOrder)
        {
            var values = propertyValues[jsonName];
            var isOptional = values.Count < objects.Count || values.Any(value => value.ValueKind == JsonValueKind.Null);
            var propertyName = ToIdentifier(jsonName, "Value");
            var type = ResolveEntityPropertyType(context, propertyName, values, isOptional);
            definition.Properties.Add(new EntityPropertyDefinition(jsonName, propertyName, type.TypeName, type.Initializer));
        }
        return className;
    }

    private static EntityTypeDefinition ResolveEntityPropertyType(
        EntityGenerationContext context,
        string propertyName,
        IReadOnlyList<JsonElement> values,
        bool isOptional)
    {
        var nonNullValues = values.Where(value => value.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined)).ToList();
        if (nonNullValues.Count == 0)
            return new EntityTypeDefinition("object?", null);

        var kinds = nonNullValues.Select(value => value.ValueKind).Distinct().ToList();
        if (kinds.All(kind => kind == JsonValueKind.Number))
        {
            var numericType = InferNumericType(nonNullValues);
            return new EntityTypeDefinition(isOptional ? numericType + "?" : numericType, null);
        }
        if (kinds.Count != 1)
            return new EntityTypeDefinition("object?", null);

        switch (kinds[0])
        {
            case JsonValueKind.Object:
            {
                var childName = BuildClassDefinition(context, propertyName, nonNullValues);
                return new EntityTypeDefinition(isOptional ? childName + "?" : childName,
                    isOptional ? null : "new()");
            }
            case JsonValueKind.Array:
            {
                var elements = nonNullValues.SelectMany(value => value.EnumerateArray()).ToList();
                var itemType = ResolveArrayItemType(context, Singularize(propertyName), elements);
                var listType = $"List<{itemType}>";
                return new EntityTypeDefinition(isOptional ? listType + "?" : listType,
                    isOptional ? null : "new()");
            }
            case JsonValueKind.String:
            {
                var strings = nonNullValues.Select(value => value.GetString() ?? string.Empty).ToList();
                if (strings.All(value => Guid.TryParse(value, out _)))
                    return new EntityTypeDefinition(isOptional ? "Guid?" : "Guid", null);
                if (strings.All(value => DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind, out _)))
                    return new EntityTypeDefinition(isOptional ? "DateTime?" : "DateTime", null);
                return new EntityTypeDefinition(isOptional ? "string?" : "string",
                    isOptional ? null : "string.Empty");
            }
            case JsonValueKind.True:
            case JsonValueKind.False:
                return new EntityTypeDefinition(isOptional ? "bool?" : "bool", null);
            default:
                return new EntityTypeDefinition("object?", null);
        }
    }

    private static string ResolveArrayItemType(
        EntityGenerationContext context,
        string requestedClassName,
        IReadOnlyList<JsonElement> elements)
    {
        var nonNull = elements.Where(value => value.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined)).ToList();
        if (nonNull.Count == 0) return "object";
        var kinds = nonNull.Select(value => value.ValueKind).Distinct().ToList();
        if (kinds.All(kind => kind == JsonValueKind.Number)) return InferNumericType(nonNull);
        if (kinds.Count != 1) return "object";
        return kinds[0] switch
        {
            JsonValueKind.Object => BuildClassDefinition(context, requestedClassName, nonNull),
            JsonValueKind.String when nonNull.All(value => Guid.TryParse(value.GetString(), out _)) => "Guid",
            JsonValueKind.String when nonNull.All(value => DateTimeOffset.TryParse(value.GetString(),
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _)) => "DateTime",
            JsonValueKind.String => "string",
            JsonValueKind.True or JsonValueKind.False => "bool",
            JsonValueKind.Array => "List<object>",
            _ => "object"
        };
    }

    private static string InferNumericType(IReadOnlyList<JsonElement> values)
    {
        if (values.All(value => value.TryGetInt32(out _))) return "int";
        if (values.All(value => value.TryGetInt64(out _))) return "long";
        if (values.All(value => value.TryGetDecimal(out _))) return "decimal";
        return "double";
    }

    private static void WriteClassDefinition(StringBuilder builder, EntityClassDefinition definition)
    {
        builder.Append("public sealed class ").AppendLine(definition.Name)
            .AppendLine("{");
        foreach (var property in definition.Properties)
        {
            builder.Append("    [JsonPropertyName(")
                .Append(JsonSerializer.Serialize(property.JsonName))
                .AppendLine(")]" );
            builder.Append("    public ").Append(property.TypeName).Append(' ').Append(property.PropertyName)
                .Append(" { get; set; }");
            if (!string.IsNullOrWhiteSpace(property.Initializer))
                builder.Append(" = ").Append(property.Initializer).Append(';');
            builder.AppendLine().AppendLine();
        }
        if (definition.Properties.Count > 0)
            builder.Length -= Environment.NewLine.Length;
        builder.AppendLine("}");
    }

    private static string GenerateJsonSample(string entityCode)
    {
        GuardInput(entityCode, ".NET 实体类");
        var classes = ParseEntityClasses(entityCode);
        if (classes.Count == 0)
            throw new FormatException("未识别到包含 public 属性的 .NET 实体类。");

        var root = classes[0];
        var classLookup = classes.ToDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase);
        var node = BuildJsonObject(root, classLookup, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        return node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

    private static List<ParsedEntityClass> ParseEntityClasses(string code)
    {
        var result = new List<ParsedEntityClass>();
        foreach (Match classMatch in ClassRegex.Matches(code))
        {
            var openingBrace = code.IndexOf('{', classMatch.Index + classMatch.Length);
            if (openingBrace < 0) continue;
            var closingBrace = FindMatchingBrace(code, openingBrace);
            if (closingBrace < 0)
                throw new FormatException($"类 {classMatch.Groups["name"].Value} 缺少结束大括号。");

            var body = code[(openingBrace + 1)..closingBrace];
            var parsedClass = new ParsedEntityClass(classMatch.Groups["name"].Value.TrimStart('@'));
            foreach (Match propertyMatch in PropertyRegex.Matches(body))
            {
                var jsonName = propertyMatch.Groups["json"].Success
                    ? DecodeJsonPropertyName(propertyMatch.Groups["json"].Value)
                    : propertyMatch.Groups["name"].Value.TrimStart('@');
                parsedClass.Properties.Add(new ParsedEntityProperty(
                    jsonName,
                    Regex.Replace(propertyMatch.Groups["type"].Value, @"\s+", string.Empty),
                    propertyMatch.Groups["name"].Value.TrimStart('@')));
            }
            if (parsedClass.Properties.Count > 0)
                result.Add(parsedClass);
        }
        return result;
    }

    private static int FindMatchingBrace(string text, int openingBrace)
    {
        var depth = 0;
        var inString = false;
        var inCharacter = false;
        var escaped = false;
        for (var index = openingBrace; index < text.Length; index++)
        {
            var character = text[index];
            if (escaped)
            {
                escaped = false;
                continue;
            }
            if ((inString || inCharacter) && character == '\\')
            {
                escaped = true;
                continue;
            }
            if (!inCharacter && character == '"') inString = !inString;
            else if (!inString && character == '\'') inCharacter = !inCharacter;
            else if (!inString && !inCharacter)
            {
                if (character == '{') depth++;
                else if (character == '}' && --depth == 0) return index;
            }
        }
        return -1;
    }

    private static string DecodeJsonPropertyName(string encoded)
    {
        try
        {
            return JsonSerializer.Deserialize<string>($"\"{encoded}\"") ?? encoded;
        }
        catch (JsonException)
        {
            return encoded;
        }
    }

    private static JsonObject BuildJsonObject(
        ParsedEntityClass entityClass,
        IReadOnlyDictionary<string, ParsedEntityClass> classLookup,
        ISet<string> recursionPath)
    {
        var result = new JsonObject();
        if (!recursionPath.Add(entityClass.Name))
            return result;

        foreach (var property in entityClass.Properties)
            result[property.JsonName] = BuildJsonValue(property.TypeName, classLookup, recursionPath);
        recursionPath.Remove(entityClass.Name);
        return result;
    }

    private static JsonNode? BuildJsonValue(
        string rawType,
        IReadOnlyDictionary<string, ParsedEntityClass> classLookup,
        ISet<string> recursionPath)
    {
        var type = rawType.Replace("global::", string.Empty, StringComparison.Ordinal).TrimEnd('?');
        if (TryGetCollectionElementType(type, out var itemType))
        {
            var array = new JsonArray();
            var simpleItemType = GetSimpleTypeName(itemType.TrimEnd('?'));
            if (classLookup.TryGetValue(simpleItemType, out var itemClass) && !recursionPath.Contains(itemClass.Name))
                array.Add(BuildJsonObject(itemClass, classLookup, recursionPath));
            return array;
        }

        var simpleType = GetSimpleTypeName(type);
        if (classLookup.TryGetValue(simpleType, out var nestedClass))
        {
            return recursionPath.Contains(nestedClass.Name)
                ? null
                : BuildJsonObject(nestedClass, classLookup, recursionPath);
        }

        return simpleType.ToLowerInvariant() switch
        {
            "string" or "char" => JsonValue.Create(string.Empty),
            "bool" or "boolean" => JsonValue.Create(false),
            "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "int16" or "int32" => JsonValue.Create(0),
            "long" or "ulong" or "int64" => JsonValue.Create(0L),
            "float" or "single" or "double" => JsonValue.Create(0d),
            "decimal" => JsonValue.Create(0m),
            "datetime" or "datetimeoffset" => JsonValue.Create("0001-01-01T00:00:00Z"),
            "dateonly" => JsonValue.Create("0001-01-01"),
            "timeonly" or "timespan" => JsonValue.Create("00:00:00"),
            "guid" => JsonValue.Create("00000000-0000-0000-0000-000000000000"),
            "jsonobject" => new JsonObject(),
            "jsonarray" => new JsonArray(),
            _ => null
        };
    }

    private static bool TryGetCollectionElementType(string type, out string itemType)
    {
        if (type.EndsWith("[]", StringComparison.Ordinal))
        {
            itemType = type[..^2];
            return true;
        }
        var opening = type.IndexOf('<');
        var closing = type.LastIndexOf('>');
        if (opening > 0 && closing > opening)
        {
            var genericName = GetSimpleTypeName(type[..opening]);
            if (genericName is "List" or "IList" or "ICollection" or "IEnumerable" or "IReadOnlyList" or "HashSet")
            {
                itemType = type[(opening + 1)..closing];
                return true;
            }
        }
        itemType = string.Empty;
        return false;
    }

    private static string GetSimpleTypeName(string type)
    {
        var alias = type.LastIndexOf('.');
        return (alias >= 0 ? type[(alias + 1)..] : type).TrimStart('@');
    }

    private static string ToIdentifier(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value)) return fallback;
        var words = Regex.Split(value.Trim(), @"[^\p{L}\p{N}_]+")
            .Where(word => !string.IsNullOrWhiteSpace(word));
        var builder = new StringBuilder();
        foreach (var word in words)
        {
            var segment = word.Trim('_');
            if (segment.Length == 0) continue;
            builder.Append(char.ToUpperInvariant(segment[0]));
            if (segment.Length > 1) builder.Append(segment[1..]);
        }
        if (builder.Length == 0) builder.Append(fallback);
        if (char.IsDigit(builder[0])) builder.Insert(0, '_');
        var identifier = builder.ToString();
        return CSharpKeywords.Contains(identifier) ? identifier + "Value" : identifier;
    }

    private static string Singularize(string value)
    {
        if (value.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
            return value[..^3] + "y";
        if (value.EndsWith("ses", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
            return value[..^2];
        if (value.EndsWith('s') && !value.EndsWith("ss", StringComparison.OrdinalIgnoreCase) && value.Length > 1)
            return value[..^1];
        return value + "Item";
    }

    private static void GuardInput(string input, string name)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException($"请输入{name}内容。");
        if (input.Length > MaximumInputLength)
            throw new FormatException($"{name}内容不能超过 {MaximumInputLength:N0} 个字符。");
    }

    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
        "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
        "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
        "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
        "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly",
        "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
        "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
        "ushort", "using", "virtual", "void", "volatile", "while", "record", "required", "init", "file"
    };

    private sealed record DiffAtom(
        DataDiffKind Kind,
        int? LeftLineNumber,
        int? RightLineNumber,
        string LeftText,
        string RightText);

    private sealed class EntityGenerationContext
    {
        private readonly HashSet<string> _classNames = new(StringComparer.OrdinalIgnoreCase);
        public List<EntityClassDefinition> Definitions { get; } = [];

        public string ReserveClassName(string requestedName)
        {
            var baseName = ToIdentifier(requestedName, "GeneratedEntity");
            var candidate = baseName;
            var suffix = 2;
            while (!_classNames.Add(candidate))
                candidate = baseName + suffix++;
            return candidate;
        }
    }

    private sealed record EntityTypeDefinition(string TypeName, string? Initializer);
    private sealed record EntityPropertyDefinition(string JsonName, string PropertyName, string TypeName, string? Initializer);
    private sealed class EntityClassDefinition(string name)
    {
        public string Name { get; } = name;
        public List<EntityPropertyDefinition> Properties { get; } = [];
    }
    private sealed record ParsedEntityProperty(string JsonName, string TypeName, string PropertyName);
    private sealed class ParsedEntityClass(string name)
    {
        public string Name { get; } = name;
        public List<ParsedEntityProperty> Properties { get; } = [];
    }
}
