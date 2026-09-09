using System.Xml.Linq;
using ArasToolkit.Services.Services;

internal static class AmlRenderingTests
{
    public static void Run(DataImportService service, Action<bool, string> check)
    {
        const string value = "R&D <零件> \"双引号\" '单引号' ]]>\r\n第二行\t😀";
        const string template = "<AML><Item type='Part' action='add' keyed_name='@A'><name>@A</name><description><![CDATA[@A]]></description></Item></AML>";
        var aml = service.ReplaceAmlPlaceholders(template, new() { ["A"] = value });
        var parsed = XDocument.Parse(aml);
        var item = parsed.Root!.Element("Item")!;
        check((string?)item.Attribute("keyed_name") == value && item.Element("name")!.Value == value
            && item.Element("description")!.Value == value && aml.Contains("R&amp;D"),
            "XML serialization preserves special characters, attribute whitespace, Unicode and CDATA data");
        check(service.PreviewAml(template, new() { ["A"] = value }) == aml,
            "AML preview and import use identical escaping");

        const string entityText = "&amp; &#x20; &#32; &#x1F600; &unknown;";
        var entities = XDocument.Parse(service.ReplaceAmlPlaceholders(
            "<AML><Item><fixed>&amp;|&#x20;|&#32;|&#x1F600;|&apos;|&quot;|&lt;|&gt;</fixed><value>@A</value></Item></AML>",
            new() { ["A"] = entityText }));
        check(entities.Descendants("fixed").Single().Value == "&| | |😀|'|\"|<|>"
            && entities.Descendants("value").Single().Value == entityText,
            "template entities decode once while entity-looking Excel text remains literal");

        var columns = XDocument.Parse(service.ReplaceAmlPlaceholders(
            "<Item><a>@A</a><aa>@AA</aa><ab>@AB</ab></Item>",
            new() { ["A"] = "@AA & raw", ["AA"] = "second", ["AB"] = "third" }));
        check(columns.Root!.Element("a")!.Value == "@AA & raw" && columns.Root.Element("aa")!.Value == "second"
            && columns.Root.Element("ab")!.Value == "third", "column tokens are matched completely and cell contents are never expanded again");

        const string injected = "</name><Item type='User' action='delete' id='any'/><name>";
        var safe = XDocument.Parse(service.ReplaceAmlPlaceholders(template, new() { ["A"] = injected }));
        check(safe.Descendants("Item").Count() == 1 && safe.Descendants("name").Single().Value == injected,
            "cell markup remains text and cannot add AML actions");

        var paths = XDocument.Parse(service.ReplaceAmlPlaceholders(
            @"<AML>\n<Item path='C:\temp\new'><name>@A</name><fixed>C:\temp\new</fixed></Item>\n</AML>",
            new() { ["A"] = @"C:\new\test" }));
        check((string?)paths.Root!.Element("Item")!.Attribute("path") == @"C:\temp\new"
            && paths.Descendants("fixed").Single().Value == @"C:\temp\new"
            && paths.Descendants("name").Single().Value == @"C:\new\test",
            "legacy formatting escapes do not alter literal backslashes in values or paths");

        bool Rejects(string input, Dictionary<string, string> row)
        {
            try { service.ReplaceAmlPlaceholders(input, row); return false; }
            catch (Exception) { return true; }
        }
        check(Rejects(template, new() { ["A"] = "invalid\u0001character" })
            && Rejects(template, new() { ["A"] = "unpaired\ud800" }),
            "invalid XML controls and unpaired surrogates are rejected without deleting source text");
        check(Rejects("<Item><name>@AA</name></Item>", new() { ["A"] = "first" }),
            "missing columns cannot silently produce incorrect values");
        check(Rejects("<Item><name>R&D</name></Item>", new())
            && Rejects("<!DOCTYPE Item [<!ENTITY value 'hidden'>]><Item>&value;</Item>", new()),
            "malformed templates and DTD entities are rejected");
    }
}
