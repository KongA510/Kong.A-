using System.Text.RegularExpressions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>无需第三方编辑器的轻量代码词法高亮器。</summary>
public sealed class CodeSyntaxHighlightService : ICodeSyntaxHighlightService
{
    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "async", "await", "base", "bool", "break", "byte", "case",
        "catch", "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int",
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
        "operator", "out", "override", "params", "private", "protected", "public", "readonly",
        "record", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
        "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "var", "virtual", "void", "volatile", "while",
        "with", "yield"
    };

    private static readonly HashSet<string> CSharpTypes = new(StringComparer.Ordinal)
    {
        "DateTime", "DateTimeOffset", "Guid", "Task", "List", "Dictionary", "HashSet", "IEnumerable",
        "IReadOnlyList", "ICollection", "CancellationToken", "StringBuilder", "Exception"
    };

    private static readonly HashSet<string> JavaScriptKeywords = new(StringComparer.Ordinal)
    {
        "async", "await", "break", "case", "catch", "class", "const", "continue", "debugger",
        "default", "delete", "do", "else", "export", "extends", "false", "finally", "for", "from",
        "function", "get", "if", "import", "in", "instanceof", "let", "new", "null", "of", "return",
        "set", "static", "super", "switch", "this", "throw", "true", "try", "typeof", "undefined",
        "var", "void", "while", "with", "yield"
    };

    private static readonly HashSet<string> SqlKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "ADD", "ALTER", "AND", "AS", "ASC", "BEGIN", "BETWEEN", "BY", "CASE", "COMMIT",
        "CREATE", "CROSS", "DELETE", "DESC", "DISTINCT", "DROP", "ELSE", "END", "EXISTS", "FROM",
        "FULL", "GROUP", "HAVING", "IF", "IN", "INNER", "INSERT", "INTO", "IS", "JOIN", "LEFT",
        "LIKE", "MERGE", "NOT", "NULL", "ON", "OR", "ORDER", "OUTER", "PRIMARY", "REFERENCES",
        "RIGHT", "ROLLBACK", "SELECT", "SET", "TABLE", "THEN", "TOP", "TRANSACTION", "UNION",
        "UNIQUE", "UPDATE", "VALUES", "WHEN", "WHERE", "WITH"
    };

    private static readonly Regex XmlMarkupRegex = new(
        @"<!--[\s\S]*?-->|<!\[CDATA\[[\s\S]*?\]\]>|<\?[\s\S]*?\?>|<!DOCTYPE[\s\S]*?>|<[^>]+>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex XmlTagNameRegex = new(
        @"^<\s*/?\s*(?<name>[A-Za-z_][\w:.-]*)", RegexOptions.Compiled);
    private static readonly Regex XmlAttributeRegex = new(
        @"(?<name>[A-Za-z_][\w:.-]*)(?<separator>\s*=\s*)(?<quoted>(?<quote>[\""'])(?<value>.*?)(?:\k<quote>))",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public IReadOnlyList<CodeHighlightSpan> Highlight(string code, string? codeType)
    {
        if (string.IsNullOrEmpty(code))
            return [];

        var normalized = RelatedCodeTypes.Normalize(codeType);
        return normalized switch
        {
            RelatedCodeTypes.CSharp => HighlightCode(code, CSharpKeywords, CSharpTypes, false, true),
            RelatedCodeTypes.JavaScript => HighlightCode(code, JavaScriptKeywords, null, false, true),
            RelatedCodeTypes.Sql => HighlightCode(code, SqlKeywords, null, true, false),
            RelatedCodeTypes.Aml or RelatedCodeTypes.Xml => HighlightXml(code),
            _ => [new CodeHighlightSpan(0, code.Length, CodeHighlightStyle.Default)]
        };
    }

    private static IReadOnlyList<CodeHighlightSpan> HighlightCode(
        string code,
        HashSet<string> keywords,
        HashSet<string>? types,
        bool sqlMode,
        bool slashLineComments)
    {
        var styles = new CodeHighlightStyle[code.Length];
        var index = 0;
        while (index < code.Length)
        {
            if (slashLineComments && Matches(code, index, "//"))
            {
                index = ApplyUntilLineEnd(styles, code, index, CodeHighlightStyle.Comment);
                continue;
            }
            if (sqlMode && Matches(code, index, "--"))
            {
                index = ApplyUntilLineEnd(styles, code, index, CodeHighlightStyle.Comment);
                continue;
            }
            if (Matches(code, index, "/*"))
            {
                var end = code.IndexOf("*/", index + 2, StringComparison.Ordinal);
                var length = (end < 0 ? code.Length : end + 2) - index;
                ApplyStyle(styles, index, length, CodeHighlightStyle.Comment);
                index += length;
                continue;
            }

            var current = code[index];
            var verbatim = !sqlMode && current == '@' && index + 1 < code.Length && code[index + 1] == '"';
            if (current is '\'' or '"' or '`' || verbatim)
            {
                var start = index;
                var quote = verbatim ? '"' : current;
                index += verbatim ? 2 : 1;
                while (index < code.Length)
                {
                    if (verbatim && code[index] == '"' && index + 1 < code.Length && code[index + 1] == '"')
                    {
                        index += 2;
                        continue;
                    }
                    if (!verbatim && code[index] == '\\')
                    {
                        index = Math.Min(index + 2, code.Length);
                        continue;
                    }
                    if (code[index++] == quote)
                        break;
                }
                ApplyStyle(styles, start, index - start, CodeHighlightStyle.String);
                continue;
            }

            if (char.IsDigit(current))
            {
                var start = index++;
                while (index < code.Length &&
                       (char.IsLetterOrDigit(code[index]) || code[index] is '.' or '_' or 'x' or 'X'))
                    index++;
                ApplyStyle(styles, start, index - start, CodeHighlightStyle.Number);
                continue;
            }

            if (IsIdentifierStart(current))
            {
                var start = index++;
                while (index < code.Length && IsIdentifierPart(code[index]))
                    index++;
                var word = code[start..index];
                var style = keywords.Contains(word)
                    ? CodeHighlightStyle.Keyword
                    : types?.Contains(word) == true
                        ? CodeHighlightStyle.Type
                        : CodeHighlightStyle.Default;
                ApplyStyle(styles, start, index - start, style);
                continue;
            }

            if ("+-*/%=!<>?:&|^~.,;()[]{}".Contains(current))
                styles[index] = CodeHighlightStyle.Operator;
            index++;
        }

        return Compress(styles);
    }

    private static IReadOnlyList<CodeHighlightSpan> HighlightXml(string code)
    {
        var styles = new CodeHighlightStyle[code.Length];
        foreach (Match markup in XmlMarkupRegex.Matches(code))
        {
            if (markup.Value.StartsWith("<!--", StringComparison.Ordinal) ||
                markup.Value.StartsWith("<![CDATA[", StringComparison.Ordinal))
            {
                ApplyStyle(styles, markup.Index, markup.Length, CodeHighlightStyle.Comment);
                continue;
            }

            ApplyStyle(styles, markup.Index, markup.Length, CodeHighlightStyle.Markup);
            var tagName = XmlTagNameRegex.Match(markup.Value).Groups["name"];
            if (tagName.Success)
                ApplyStyle(styles, markup.Index + tagName.Index, tagName.Length, CodeHighlightStyle.Keyword);

            foreach (Match attribute in XmlAttributeRegex.Matches(markup.Value))
            {
                var name = attribute.Groups["name"];
                var value = attribute.Groups["quoted"];
                ApplyStyle(styles, markup.Index + name.Index, name.Length, CodeHighlightStyle.Attribute);
                ApplyStyle(styles, markup.Index + value.Index, value.Length, CodeHighlightStyle.AttributeValue);
            }
        }
        return Compress(styles);
    }

    private static int ApplyUntilLineEnd(
        CodeHighlightStyle[] styles, string code, int start, CodeHighlightStyle style)
    {
        var end = code.IndexOf('\n', start);
        if (end < 0)
            end = code.Length;
        ApplyStyle(styles, start, end - start, style);
        return end;
    }

    private static void ApplyStyle(
        CodeHighlightStyle[] styles, int start, int length, CodeHighlightStyle style)
    {
        if (length <= 0 || start < 0 || start >= styles.Length)
            return;
        Array.Fill(styles, style, start, Math.Min(length, styles.Length - start));
    }

    private static IReadOnlyList<CodeHighlightSpan> Compress(CodeHighlightStyle[] styles)
    {
        if (styles.Length == 0)
            return [];

        var spans = new List<CodeHighlightSpan>();
        var start = 0;
        for (var index = 1; index <= styles.Length; index++)
        {
            if (index < styles.Length && styles[index] == styles[start])
                continue;
            spans.Add(new CodeHighlightSpan(start, index - start, styles[start]));
            start = index;
        }
        return spans;
    }

    private static bool Matches(string source, int index, string value) =>
        index + value.Length <= source.Length &&
        source.AsSpan(index, value.Length).SequenceEqual(value.AsSpan());

    private static bool IsIdentifierStart(char value) => char.IsLetter(value) || value is '_' or '$';
    private static bool IsIdentifierPart(char value) => char.IsLetterOrDigit(value) || value is '_' or '$';
}
