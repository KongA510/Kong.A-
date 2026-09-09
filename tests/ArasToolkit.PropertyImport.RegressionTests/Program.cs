using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Services;
using OfficeOpenXml;

// 使用真实 R37 IOM 解析响应；只替换传输层，不连接 Aras 或工具箱数据库。
const string itemTypeId = "11111111111111111111111111111111";
const string propertyId = "22222222222222222222222222222222";
const string itemTypeName = "RegressionItemType";
var workDir = Path.Combine(Path.GetTempPath(), "property-import-regression-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(workDir);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

string Fault(string code, string message) => $"""
    <SOAP-ENV:Fault><faultcode>{code}</faultcode><faultstring>{message}</faultstring></SOAP-ENV:Fault>
    """;
string Property(string id) => $"""
    <Item type="Property" id="{id}"><name>item_number</name><source_id>{itemTypeId}</source_id></Item>
    """;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

string CreateTemplate(string fileName, params (string Name, string Type, string Source)[] rows)
{
    using var package = new ExcelPackage();
    var sheet = package.Workbook.Worksheets.Add("属性配置");
    string[] headers = ["名称", "标签(简中)", "标签(繁中)", "标签(英文)", "数据类型", "数据源"];
    for (var column = 0; column < headers.Length; column++)
        sheet.Cells[1, column + 1].Value = headers[column];
    for (var index = 0; index < rows.Length; index++)
    {
        var row = rows[index];
        object[] values = [row.Name, "编号", "編號", "Number", row.Type, row.Source];
        for (var column = 0; column < values.Length; column++)
            sheet.Cells[index + 2, column + 1].Value = values[column];
    }
    var path = Path.Combine(workDir, fileName);
    package.SaveAs(new FileInfo(path));
    return path;
}

async Task<(PropertyImportPreview Preview, List<string> Errors)> Prepare(
    string file, string mode, Func<XElement, string> propertyResponse)
{
    var transport = DispatchProxy.Create<IServerConnection, TestProxy>();
    ((TestProxy)(object)transport).Handler = (method, args) =>
    {
        if (method.Name != "CallAction")
            throw new InvalidOperationException($"Unexpected IOM call: {method.Name}");
        var request = XDocument.Parse(((XmlDocument)args[1]!).OuterXml).Descendants("Item").Single();
        Check((string?)request.Attribute("action") == "get", "预览不允许写入 Aras");
        string body;
        if ((string?)request.Attribute("type") == "ItemType" &&
            (string?)request.Attribute("id") == itemTypeId)
        {
            body = $"<Result><Item type=\"ItemType\" id=\"{itemTypeId}\"><name>{itemTypeName}</name></Item></Result>";
        }
        else
        {
            if ((string?)request.Attribute("type") == "Property")
                Check((string?)request.Element("source_id") == itemTypeId, "属性查询必须限定目标对象类");
            body = propertyResponse(request);
        }
        ((XmlDocument)args[2]!).LoadXml($"""
            <SOAP-ENV:Envelope xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
              <SOAP-ENV:Body>{body}</SOAP-ENV:Body>
            </SOAP-ENV:Envelope>
            """);
        return null;
    };
    var connection = new ArasConnectionService();
    connection.SetConnection(new ArasConnectionInfo(), new Innovator(transport), null!);
    var errors = new List<string>();
    var errorService = DispatchProxy.Create<IErrorLogService, TestProxy>();
    ((TestProxy)(object)errorService).Handler = (method, args) =>
    {
        Check(method.Name == nameof(IErrorLogService.LogErrorAsync), "Unexpected error log call");
        errors.Add((string)args[1]!);
        return Task.CompletedTask;
    };
    // PrepareAsync 不使用数据库或操作日志；若意外触及这些依赖，测试立即失败。
    var service = new PropertyImportService(null!, connection, null!, errorService);
    return (await service.PrepareAsync(file, itemTypeId, itemTypeName, mode), errors);
}

void CheckAml(PropertyImportPreviewRow row, string action)
{
    Check(row.IsValid && row.HasAmlPreview, "有效属性必须提供 AML 预览");
    var item = XDocument.Parse(row.AmlPreview).Root!.Element("Item")!;
    Check((string?)item.Attribute("type") == "Property", "提交类型应为 Property");
    Check((string?)item.Attribute("action") == action, $"AML action 应为 {action}");
    Check((string?)item.Element("source_id") == itemTypeId, "AML 必须限定目标对象类");
    Check((string?)item.Element("name") == row.Name, "AML 名称应与模板一致");
    Check(action == "edit"
        ? (string?)item.Attribute("id") == propertyId && row.ExistingPropertyId == propertyId
        : item.Attribute("id") == null && row.ExistingPropertyId == "", "现有属性 ID 与操作不符");
    Check(row.PlannedAction == (action == "edit" ? "覆盖现有属性" : "新增属性"), "预览操作与 AML 不一致");
}

void CheckTranslations(XElement item, string propertyName, string english, string simplified, string traditional)
{
    XNamespace i18n = "http://www.aras.com/I18N";
    var nodes = item.Elements().Where(node => node.Name.LocalName == propertyName).ToList();
    Check(nodes.Count == 3, $"{propertyName} 必须有三个独立语言值");
    Check(nodes.All(node => node.Name.Namespace == i18n),
        $"{propertyName} 的所有语言（包括英文）必须使用 i18n 命名空间，避免写入当前会话语言");
    foreach (var (language, expected) in new[] { ("en", english), ("zc", simplified), ("zt", traditional) })
    {
        var node = nodes.Single(node => (string?)node.Attribute(XNamespace.Xml + "lang") == language);
        Check(node.Value == expected, $"{propertyName}/{language} 取值错误或列错位");
    }
}

async Task<int> CheckMultilingualTemplates()
{
    var passed = 0;
    // 不同文字与 XML 特殊字符用于识别三语串位及转义丢失。
    const string english = "Description & <English>";
    const string simplified = "项目描述 & <简中>";
    const string traditional = "專案說明 & <繁中>";
    var values = new Dictionary<string, string>
    {
        ["名称"] = "describe", ["数据类型"] = "Multilingual String",
        ["标签(英文)"] = english, ["标签(简中)"] = simplified, ["标签(繁中)"] = traditional,
        ["默认值(英文)"] = "Default EN", ["默认值(简中)"] = "简体默认值", ["默认值(繁中)"] = "繁體預設值",
        ["提示(英文)"] = "Help EN", ["提示(简中)"] = "简体提示", ["提示(繁中)"] = "繁體提示"
    };
    foreach (var reordered in new[] { false, true })
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("属性配置");
        var columns = reordered ? values.Reverse().ToArray() : values.ToArray();
        for (var index = 0; index < columns.Length; index++)
        {
            // 表头两边空白不应改变列映射。
            sheet.Cells[1, index + 1].Value = $" {columns[index].Key} ";
            sheet.Cells[2, index + 1].Value = columns[index].Value;
        }
        var file = Path.Combine(workDir, $"multilingual-{reordered}.xlsx");
        package.SaveAs(new FileInfo(file));
        foreach (var action in new[] { "add", "edit" })
        {
            var (preview, errors) = await Prepare(file, action == "add" ? "新增" : "覆盖", _ =>
                action == "add" ? Fault("0", "No items of type Property found.") : $"<Result>{Property(propertyId)}</Result>");
            Check(preview.CanImport && errors.Count == 0, "多语言模板应通过预检");
            var row = preview.Rows.Single();
            Check(row.LabelEn == english && row.LabelZhCn == simplified && row.LabelZhTw == traditional,
                "预览必须按表头读取对应三语列");
            Check(row.DefaultValueEn == "Default EN" && row.DefaultValueZhCn == "简体默认值" && row.DefaultValueZhTw == "繁體預設值",
                "默认值的三语列映射错误");
            Check(row.HelpTooltipEn == "Help EN" && row.HelpTooltipZhCn == "简体提示" && row.HelpTooltipZhTw == "繁體提示",
                "提示的三语列映射错误");
            CheckAml(row, action);
            var item = XDocument.Parse(row.AmlPreview).Root!.Element("Item")!;
            CheckTranslations(item, "label", english, simplified, traditional);
            CheckTranslations(item, "default_value", "Default EN", "简体默认值", "繁體預設值");
            CheckTranslations(item, "help_tooltip", "Help EN", "简体提示", "繁體提示");
            Console.WriteLine($"PASS 属性三语: {action}, 列顺序调整={reordered}");
            passed++;
        }
    }

    // 对象类汇入使用已由 Excel 读取层转义的列值；检查两个 AML 构建入口。
    foreach (var mode in new[] { "新增", "覆盖" })
    {
        var objectRow = new Dictionary<int, string>
        {
            [1] = "RegressionItemType", [2] = "项目", [3] = "專案", [4] = "Project",
            [5] = "项目列表", [6] = "專案列表", [7] = "Projects", [8] = "0"
        };
        var objectBuilder = typeof(ObjectClassImportService).GetMethod("BuildObjectClassAml", BindingFlags.NonPublic | BindingFlags.Static)!;
        var objectAml = (string)objectBuilder.Invoke(null, [objectRow, mode])!;
        var objectItem = XDocument.Parse(objectAml).Root!.Element("Item")!;
        CheckTranslations(objectItem, "label", "Project", "项目", "專案");
        CheckTranslations(objectItem, "label_plural", "Projects", "项目列表", "專案列表");
        Console.WriteLine($"PASS 对象类三语标签及 TOC: {mode}");
        passed++;

        var relationshipRow = new Dictionary<int, string>
        {
            [1] = "RegressionItemType", [2] = "RegressionRelationship", [3] = "100",
            [4] = "项目关系", [5] = "專案關係", [6] = "Project Links", [7] = "0", [8] = "0"
        };
        var relationshipBuilder = typeof(ObjectClassImportService).GetMethod("BuildRelationshipTypeAml", BindingFlags.NonPublic | BindingFlags.Static)!;
        var relationshipAml = (string)relationshipBuilder.Invoke(null, [relationshipRow, mode])!;
        CheckTranslations(XDocument.Parse(relationshipAml).Root!.Element("Item")!, "label", "Project Links", "项目关系", "專案關係");
        Console.WriteLine($"PASS 关系类三语页签: {mode}");
        passed++;
    }
    return passed;
}

try
{
    var template = CreateTemplate("single.xlsx", ("item_number", "String", ""));
    var cases = new (string Name, string Response, bool Exists, string? Error)[]
    {
        ("No items found", Fault("0", "No items of type Property found."), false, null),
        ("本地化空结果", Fault("0", "未找到 Property 数据。"), false, null),
        ("空 Result", "<Result />", false, null),
        ("已有属性", $"<Result>{Property(propertyId)}</Result>", true, null),
        ("真实错误", Fault("SOAP-ENV:Server", "Permission denied"), false, "Permission denied"),
        ("错误消息不能代替错误码", Fault("SOAP-ENV:Server", "No items of type Property found."), false, "查询属性"),
        ("重复属性", $"<Result>{Property(propertyId)}{Property("33333333333333333333333333333333")}</Result>", false, "多个同名属性")
    };
    var passed = 0;
    foreach (var mode in new[] { "覆盖", "新增" })
    foreach (var test in cases)
    {
        var (preview, errors) = await Prepare(template, mode, _ => test.Response);
        var row = preview.Rows.Single();
        var nameConflict = test.Exists && mode == "新增";
        if (test.Error != null || nameConflict)
        {
            Check(!preview.CanImport && preview.InvalidCount == 1 && !row.HasAmlPreview, "错误行必须阻止提交");
            Check(row.ValidationMessage.Contains(test.Error ?? "已存在属性"), "应保留准确错误原因");
            Check(errors.Count == (nameConflict ? 0 : 1), "真实查询异常应写入错误日志");
        }
        else
        {
            Check(preview.CanImport && preview.ValidCount == 1 && preview.InvalidCount == 0, "空结果或唯一命中应允许提交");
            Check(errors.Count == 0, "空结果不应写错误日志");
            CheckAml(row, test.Exists ? "edit" : "add");
        }
        Console.WriteLine($"PASS {mode}: {test.Name}");
        passed++;
    }

    var mixedTemplate = CreateTemplate("mixed.xlsx",
        ("item_number", "String", ""), ("created_on", "Date", ""), ("name", "String", ""));
    var (mixed, mixedErrors) = await Prepare(mixedTemplate, "覆盖", request =>
        (string?)request.Element("name") == "created_on"
            ? $"<Result>{Property(propertyId)}</Result>"
            : Fault("0", "No items of type Property found."));
    Check(mixed.CanImport && mixed.ValidCount == 3 && mixedErrors.Count == 0, "混合新增/覆盖必须全部可提交");
    CheckAml(mixed.Rows[0], "add");
    CheckAml(mixed.Rows[1], "edit");
    CheckAml(mixed.Rows[2], "add");
    Console.WriteLine("PASS 覆盖: 混合新增与已有属性");
    passed++;

    var referenceTemplate = CreateTemplate("reference.xlsx", ("ic_project", "Item", "MissingItemType"));
    var (reference, referenceErrors) = await Prepare(referenceTemplate, "覆盖", _ => Fault("0", "No items of type ItemType found."));
    Check(!reference.CanImport && reference.InvalidCount == 1 && !reference.Rows[0].HasAmlPreview,
        "缺失数据源不能当成待新增属性放行");
    Check(referenceErrors.Count == 1, "缺失引用应保留错误日志");
    Console.WriteLine("PASS 覆盖: 缺失引用仍阻止提交");
    passed++;
    passed += await CheckMultilingualTemplates();
    var lifecycleTemplate = CreateTemplate("lifecycle.xlsx",
        ("first_property", "String", ""), ("second_property", "String", ""), ("third_property", "String", ""));
    passed += await ImportLifecycleTests.RunAsync(lifecycleTemplate);
    Console.WriteLine($"All {passed} regression cases passed.");
}
finally
{
    foreach (var file in Directory.EnumerateFiles(workDir))
        File.Delete(file);
    Directory.Delete(workDir);
}

public class TestProxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Handler(targetMethod!, args!);
}
