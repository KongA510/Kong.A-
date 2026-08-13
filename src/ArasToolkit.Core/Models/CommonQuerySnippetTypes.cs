namespace ArasToolkit.Core.Models;

/// <summary>常用查询片段支持的内容类型。</summary>
public static class CommonQuerySnippetTypes
{
    public const string Sql = "SQL";
    public const string Aml = "AML";
    public const string Xml = "XML";

    public static IReadOnlyList<string> All { get; } = [Sql, Aml, Xml];

    public static bool IsXmlBased(string? contentType) =>
        string.Equals(contentType, Aml, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(contentType, Xml, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string? contentType)
    {
        var normalized = contentType?.Trim().ToUpperInvariant();
        return All.Contains(normalized) ? normalized! : Sql;
    }
}
