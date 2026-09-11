using System.Xml.Linq;

namespace ArasToolkit.Services.Services;

/// <summary>Aras R37 多语系属性：命名空间不能包含尾部斜杠，所有语言均显式指定。</summary>
internal static class MultilingualAml
{
    internal static readonly XNamespace Namespace = "http://www.aras.com/I18N";
    internal const string Languages = "en,zc,zt";

    internal static IEnumerable<XElement> Values(
        string property, string english, string simplifiedChinese, string traditionalChinese)
    {
        foreach (var (language, value) in new[]
                 { ("en", english), ("zc", simplifiedChinese), ("zt", traditionalChinese) })
        {
            // 模板空白表示未提供译文；覆盖时不能清空已有语言值。
            if (!string.IsNullOrWhiteSpace(value))
                yield return new XElement(Namespace + property,
                    new XAttribute(XNamespace.Xml + "lang", language), value);
        }
    }
}
