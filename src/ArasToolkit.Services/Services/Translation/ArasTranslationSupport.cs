using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.Services.Services;

internal sealed record ArasTranslationTarget(string Name, string LanguageCode);

/// <summary>Aras 多语言翻译共享规则与安全 AML 构造。</summary>
internal static class ArasTranslationSupport
{
    private static readonly XNamespace I18n = "http://www.aras.com/I18N/";

    private static readonly Dictionary<string, string> LanguageCodes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["中文"] = "zc",
            ["简体中文"] = "zc",
            ["簡體中文"] = "zc",
            ["繁体中文"] = "zt",
            ["繁體中文"] = "zt",
            ["英文"] = "en",
            ["英语"] = "en",
            ["English"] = "en",
            ["越南"] = "vi",
            ["越南语"] = "vi",
            ["日文"] = "ja",
            ["日语"] = "ja",
            ["韩文"] = "ko",
            ["韩语"] = "ko",
            ["泰语"] = "th",
            ["印尼语"] = "id",
            ["法语"] = "fr",
            ["德语"] = "de",
            ["西班牙语"] = "es",
            ["柬埔寨语"] = "km"
        };

    public static Innovator GetConnectedInnovator(ArasConnectionService connectionService)
        => connectionService.TypedInnovator
           ?? throw new InvalidOperationException("尚未连接 Aras，请先连接当前用户的默认 Aras 配置。");

    public static List<ArasTranslationTarget> ParseTargets(string targetLanguages)
    {
        var targets = new List<ArasTranslationTarget>();
        foreach (var raw in targetLanguages.Split(
                     [',', '，', ';', '；'],
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var code = LanguageCodes.TryGetValue(raw, out var knownCode)
                ? knownCode
                : IsLanguageCode(raw)
                    ? raw.ToLowerInvariant()
                    : throw new InvalidOperationException($"无法识别目标语言“{raw}”，请使用语言名称或 Aras 语言代码。");

            if (targets.All(target => !string.Equals(
                    target.LanguageCode, code, StringComparison.OrdinalIgnoreCase)))
            {
                targets.Add(new ArasTranslationTarget(raw, code));
            }
        }

        if (targets.Count == 0)
            throw new InvalidOperationException("至少需要设置一种目标语言。");

        return targets;
    }

    public static List<ItemTypeItem> GetItemTypes(Innovator innovator)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "ItemType"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id,name,label")));
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        ThrowIfError(result, "读取对象类失败");

        var items = new List<ItemTypeItem>();
        for (var index = 0; index < result.getItemCount(); index++)
        {
            var item = result.getItemByIndex(index);
            items.Add(new ItemTypeItem
            {
                Id = item.getID(),
                Name = item.getProperty("name", string.Empty),
                Label = item.getProperty("label", string.Empty)
            });
        }

        return items
            .Where(item => !string.IsNullOrWhiteSpace(item.Id) && !string.IsNullOrWhiteSpace(item.Name))
            .OrderBy(item => item.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public static List<ArasFormItem> GetFormsByItemType(Innovator innovator, string itemTypeId)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "ItemType"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id,name"),
                new XElement("id", itemTypeId),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "View"),
                        new XAttribute("action", "get"),
                        new XAttribute("select", "id,related_id(name,label)")))));
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        ThrowIfError(result, "读取对象类窗体失败");
        if (result.getItemCount() == 0)
            return [];

        var itemType = result.getItemByIndex(0);
        var itemTypeName = itemType.getProperty("name", string.Empty);
        var relationships = itemType.getRelationships();
        var forms = new List<ArasFormItem>();
        for (var index = 0; index < relationships.getItemCount(); index++)
        {
            var related = relationships.getItemByIndex(index).getRelatedItem();
            if (related == null || string.IsNullOrWhiteSpace(related.getID()))
                continue;

            forms.Add(new ArasFormItem
            {
                Id = related.getID(),
                Name = related.getProperty("name", string.Empty),
                Label = related.getProperty("label", string.Empty),
                ItemTypeName = itemTypeName,
                IsSelected = true
            });
        }

        return forms
            .GroupBy(form => form.Id, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(form => form.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public static void ApplyLocalizedValues(
        Innovator innovator,
        string itemType,
        string itemId,
        ArasTranslationTarget target,
        IReadOnlyDictionary<string, string> values)
    {
        var item = new XElement("Item",
            new XAttribute("type", itemType),
            new XAttribute("action", "edit"),
            new XAttribute("id", itemId),
            new XAttribute(XNamespace.Xmlns + "i18n", I18n));

        foreach (var (propertyName, value) in values.Where(pair => !string.IsNullOrWhiteSpace(pair.Value)))
        {
            item.Add(new XElement(I18n + propertyName,
                new XAttribute(XNamespace.Xml + "lang", target.LanguageCode),
                value));
        }

        if (!item.Elements().Any())
            return;

        var result = innovator.applyAML(
            new XElement("AML", item).ToString(SaveOptions.DisableFormatting));
        ThrowIfError(result, $"写入 {itemType} 多语言标签失败");
    }

    public static async Task<string> TranslateTextAsync(
        IAiDispatcherService aiDispatcher,
        string sourceText,
        string sourceLanguage,
        ArasTranslationTarget target,
        string context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sourceText))
            return string.Empty;

        var prompt = $"你是 Aras Innovator 企业系统本地化专家。将下面的{context}从{sourceLanguage}翻译为{target.Name}。" +
                     "保留产品名、缩写、单位和占位符，只返回译文，不要解释，不要添加引号。\n" +
                     sourceText.Trim();
        var translated = await aiDispatcher.ChatAsync(prompt, cancellationToken: cancellationToken);
        var cleaned = translated.Trim().Trim('"', '\'', '“', '”');
        if (string.IsNullOrWhiteSpace(cleaned))
            throw new InvalidOperationException("AI 服务返回了空译文，已停止写回 Aras。");
        return cleaned;
    }

    public static TranslationRecord CreateRecord(
        TranslationTask task,
        string fieldId,
        string fieldName,
        string original,
        string translated,
        ArasTranslationTarget target)
        => new()
        {
            TaskId = task.Id,
            ItemType = task.TaskType switch
            {
                "字段翻译" => "Property",
                "表单翻译" => "Form",
                "窗体翻译" => "Field",
                _ => task.TaskType
            },
            FieldId = Truncate(fieldId, 50),
            ItemName = Truncate(RemovePropertySuffix(fieldName), 200),
            FieldName = Truncate(fieldName, 200),
            OriginalLabel = original,
            TranslatedLabel = translated,
            TargetLanguage = Truncate($"{target.Name} ({target.LanguageCode})", 50),
            Status = "Completed",
            CreatorOn = DateTime.Now
        };

    public static void ThrowIfError(Item result, string context)
    {
        if (result.isError())
            throw new InvalidOperationException($"{context}: {result.getErrorString()}");
    }

    private static bool IsLanguageCode(string value)
        => value.Length is >= 2 and <= 10 &&
           value.All(character => char.IsAsciiLetter(character) || character == '-');

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];

    private static string RemovePropertySuffix(string value)
        => value.EndsWith(".label", StringComparison.OrdinalIgnoreCase)
            ? value[..^6]
            : value.EndsWith(".legend", StringComparison.OrdinalIgnoreCase)
                ? value[..^7]
                : value;
}
