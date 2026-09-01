namespace ArasToolkit.Core.Models;

/// <summary>相关代码记录支持的代码类型。</summary>
public static class RelatedCodeTypes
{
    public const string JavaScript = "JS";
    public const string Sql = "SQL";
    public const string CSharp = "C#";
    public const string Aml = "AML";
    public const string Xml = "XML";
    public const string Other = "其他";

    public static IReadOnlyList<string> All { get; } =
        [JavaScript, Sql, CSharp, Aml, Xml, Other];

    public static string Normalize(string? codeType)
    {
        var value = codeType?.Trim();
        if (string.IsNullOrEmpty(value))
            return Other;

        if (string.Equals(value, "JAVASCRIPT", StringComparison.OrdinalIgnoreCase))
            return JavaScript;
        if (string.Equals(value, "CSHARP", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "CS", StringComparison.OrdinalIgnoreCase))
            return CSharp;

        return All.FirstOrDefault(item =>
            string.Equals(item, value, StringComparison.OrdinalIgnoreCase)) ?? Other;
    }
}
