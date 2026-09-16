using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using ArasToolkit.Services.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

internal static class ForeignReferenceTests
{
    private const string Owner = "11111111111111111111111111111111";
    private const string Target = "22222222222222222222222222222222";
    private const string Source = "33333333333333333333333333333333";
    private const string External = "44444444444444444444444444444444";
    private static readonly string[] ItemRow = ["link", "Item", "Target", ""];
    private static readonly string[] ForeignRow = ["linked_name", "Foreign", "link", "name"];

    public static async Task<int> RunAsync(string directory)
    {
        var cases = 0;
        async Task Run(string name, Func<Task> test)
        {
            await test();
            Console.WriteLine("PASS Foreign: " + name);
            cases++;
        }
        string Template(params string[][] rows)
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("属性配置");
            string[] headers = ["名称", "数据类型", "数据源", "引用外部属性", "标签(简中)", "标签(繁中)", "标签(英文)"];
            for (var column = 0; column < headers.Length; column++) sheet.Cells[1, column + 1].Value = headers[column];
            for (var row = 0; row < rows.Length; row++)
            {
                for (var column = 0; column < rows[row].Length; column++) sheet.Cells[row + 2, column + 1].Value = rows[row][column];
                for (var column = 5; column <= 7; column++) sheet.Cells[row + 2, column].Value = rows[row][0];
            }
            var path = Path.Combine(directory, "foreign-" + Guid.NewGuid().ToString("N") + ".xlsx");
            package.SaveAs(new FileInfo(path));
            return path;
        }
        async Task Reject(Fixture fixture, string file, string error)
        {
            var preview = await fixture.Prepare(file);
            Check(!preview.CanImport && preview.Rows.Any(row => row.ValidationMessage.Contains(error)), error);
            Check(fixture.Writes.Count == 0 && fixture.Errors.Count > 0, "拒绝预检不得写入且必须记录错误");
        }

        foreach (var mode in new[] { "新增", "覆盖" })
        await Run("前置新增 Item、多个 Foreign / " + mode, async () =>
        {
            var fixture = new Fixture();
            var preview = await fixture.Prepare(Template(ItemRow, ForeignRow, ["linked_name2", "Foreign", "LINK", "name"]), mode);
            Check(preview.CanImport && fixture.Writes.Count == 0, "前置待汇入 Item 应通过只读预检");
            var id = Aml(preview.Rows[0]).Attribute("id")!.Value;
            Check(id.Length == 32 && preview.Rows[0].ExistingPropertyId == "", "新增 ID 不可误作覆盖 ID");
            foreach (var row in preview.Rows.Skip(1))
                Check(row.ResolvedDataSourceId == id && row.ResolvedForeignPropertyId == External &&
                      row.DependencyRowNumbers.SetEquals([2]), "多个 Foreign 必须复用源 ID 和正确依赖行");
        });

        await Run("已有源属性优先，支持名称和 GUID", async () =>
        {
            var fixture = new Fixture();
            fixture.Properties.Add(Property(Source, Owner, "link", "item", Target));
            var preview = await fixture.Prepare(Template(ForeignRow, ["by_id", "Foreign", Source, External]));
            Check(preview.CanImport && preview.Rows.All(row => row.ResolvedDataSourceId == Source &&
                row.ResolvedForeignPropertyId == External && row.DependencyRowNumbers.Count == 0), "现有源解析失败");
        });
        await Run("源在后面不能提前引用", () => Reject(new Fixture(), Template(ForeignRow, ItemRow), "放在本行之前"));
        await Run("源属性缺失", () => Reject(new Fixture(), Template(ForeignRow), "均不存在"));
        await Run("前置属性不是 Item", () => Reject(new Fixture(), Template(["link", "String", "", ""], ForeignRow), "Item 属性"));
        await Run("前置 Item 的 ItemType 不存在", () => Reject(new Fixture(), Template(["link", "Item", "Missing", ""], ForeignRow), "均不存在"));
        await Run("前置行本地校验失败", () => Reject(new Fixture(), Template(["link", "Item", "", ""], ForeignRow), "均不存在"));
        await Run("重复源属性不允许回退", () => Reject(new Fixture(), Template(ItemRow, ItemRow, ForeignRow), "均不存在"));
        await Run("不存在的目标字段不能从其它对象类的模板行借用", () => Reject(new Fixture(),
            Template(["missing", "String", "", ""], ItemRow, ["f", "Foreign", "link", "missing"]), "不存在外部属性"));
        await Run("自引用目标允许前置待新增字段", async () =>
        {
            var fixture = new Fixture();
            var preview = await fixture.Prepare(Template(["own_name", "String", "", ""], ["self", "Item", "Owner", ""],
                ["own_foreign", "Foreign", "self", "own_name"]));
            Check(preview.CanImport, "自引用应通过预检");
            var row = preview.Rows[2];
            Check(row.DependencyRowNumbers.SetEquals([2, 3]) && row.ResolvedForeignPropertyId == preview.Rows[0].NewPropertyId &&
                row.ResolvedDataSourceId == preview.Rows[1].NewPropertyId, "自引用源/目标的 ID 与依赖必须正确");
        });
        foreach (var failure in new[] { "Permission denied", "No items of type Property found." })
        await Run("真实查询错误不可回退模板 / " + failure, async () =>
        {
            var fixture = new Fixture { SourceQueryFault = failure };
            await Reject(fixture, Template(ItemRow, ForeignRow), failure);
        });
        await Run("GUID 必须属于对应对象类", async () =>
        {
            var fixture = new Fixture();
            fixture.Properties.Add(Property(Source, Target, "link", "item", Target));
            await Reject(fixture, Template(["f", "Foreign", Source, "name"]), "不存在于当前对象类");
        });
        await Run("系统源重复不得回退", async () =>
        {
            var fixture = new Fixture();
            fixture.Properties.Add(Property(Source, Owner, "link", "item", Target));
            fixture.Properties.Add(Property("55555555555555555555555555555555", Owner, "link", "item", Target));
            await Reject(fixture, Template(ForeignRow), "多个匹配属性");
        });
        await Run("前置覆盖采用本次 Item 数据源和已有 ID", async () =>
        {
            var fixture = new Fixture();
            fixture.Properties.Add(Property(Source, Owner, "link", "item", Owner));
            var preview = await fixture.Prepare(Template(ItemRow, ForeignRow));
            Check(preview.CanImport && preview.Rows[1].ResolvedDataSourceId == Source &&
                  preview.Rows[1].ResolvedForeignPropertyId == External && preview.Rows[1].DependencyRowNumbers.SetEquals([2]),
                "覆盖应按新目标校验，不得使用旧 ItemType");
        });
        await Run("覆盖将源改成非 Item 时拒绝 Foreign", async () =>
        {
            var fixture = new Fixture();
            fixture.Properties.Add(Property(Source, Owner, "link", "item", Target));
            await Reject(fixture, Template(["link", "String", "", ""], ForeignRow), "Item 属性");
        });
        foreach (var sourceFails in new[] { false, true })
        await Run("逐条提交与依赖失败阻断 / " + sourceFails, async () =>
        {
            var fixture = new Fixture { FailSourceWrite = sourceFails };
            var file = Template(ItemRow, ForeignRow, ["independent", "String", "", ""]);
            var result = await fixture.Service.ImportAsync(file, Owner, "Owner", "新增");
            var writes = fixture.Writes.Where(item => (string?)item.Attribute("type") == "Property").ToList();
            Check(result.ItemTypeSaved, "成功属性必须保存对象类");
            if (sourceFails)
            {
                Check(writes.Count == 2 && writes.All(item => (string?)item.Element("data_type") != "foreign") &&
                      result.Sheet1Count == 1 && result.FailedDetails.Any(error => error.Contains("前置属性未成功汇入")),
                    "源失败必须阻断 Foreign，独立行继续提交");
            }
            else
            {
                Check(result.IsSuccess && !result.HasFailures && result.AddedCount == 3, "依赖导入应该全部成功");
                Check((string?)writes[1].Element("data_source") == (string?)writes[0].Attribute("id"), "实际源提交和 Foreign 引用 ID 必须一致");
            }
        });
        return cases;
    }

    private static XElement Aml(PropertyImportPreviewRow row) => XDocument.Parse(row.AmlPreview).Root!.Element("Item")!;
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static XElement Property(string id, string owner, string name, string type, string source = "") =>
        new("Item", new XAttribute("type", "Property"), new XAttribute("id", id), new XElement("source_id", owner),
            new XElement("name", name), new XElement("data_type", type), new XElement("data_source", source));

    private sealed class Fixture
    {
        public List<XElement> Properties { get; } = [Property(External, Target, "name", "string")];
        public List<XElement> Writes { get; } = [];
        public List<string> Errors { get; } = [];
        public string? SourceQueryFault { get; init; }
        public bool FailSourceWrite { get; init; }
        public PropertyImportService Service { get; }
        public Fixture()
        {
            var transport = DispatchProxy.Create<IServerConnection, TestProxy>();
            ((TestProxy)(object)transport).Handler = (method, args) =>
            {
                Check(method.Name == "CallAction", "只替换 IOM 传输层");
                var request = XDocument.Parse(((XmlDocument)args[1]!).OuterXml).Descendants("Item").Single();
                var id = (string?)request.Attribute("id");
                var name = (string?)request.Element("name");
                var type = (string?)request.Attribute("type");
                string body;
                if ((string?)request.Attribute("action") != "get")
                {
                    Writes.Add(new XElement(request));
                    body = name == "link" && FailSourceWrite ? Fault("SOAP-ENV:Server", "source rejected") :
                        $"<Result><Item type='{type}' id='{id ?? Guid.NewGuid().ToString("N")}' /></Result>";
                }
                else if (type == "ItemType")
                    body = id == Owner || name == "Owner" ? $"<Result><Item type='ItemType' id='{Owner}'><name>Owner</name></Item></Result>" :
                        name == "Target" ? $"<Result><Item type='ItemType' id='{Target}'><name>Target</name></Item></Result>" : Fault("0", "Missing ItemType");
                else if (name == "link" && ((string?)request.Attribute("select"))!.Contains("source_id") && SourceQueryFault != null)
                    body = Fault("SOAP-ENV:Server", SourceQueryFault);
                else
                {
                    var matches = Properties.Where(item => id != null ? (string?)item.Attribute("id") == id :
                        string.Equals((string?)item.Element("name"), name, StringComparison.OrdinalIgnoreCase) &&
                        (string?)item.Element("source_id") == (string?)request.Element("source_id")).ToList();
                    body = matches.Count == 0 ? Fault("0", "No items of type Property found.") : new XElement("Result", matches).ToString();
                }
                ((XmlDocument)args[2]!).LoadXml("<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'><SOAP-ENV:Body>" + body + "</SOAP-ENV:Body></SOAP-ENV:Envelope>");
                return null;
            };
            var connection = new ArasConnectionService();
            connection.SetConnection(new ArasConnectionInfo(), new Innovator(transport), null!);
            var errors = DispatchProxy.Create<IErrorLogService, TestProxy>();
            ((TestProxy)(object)errors).Handler = (_, args) => { Errors.Add((string)args[1]!); return Task.CompletedTask; };
            var operations = DispatchProxy.Create<IOperationLogService, TestProxy>();
            ((TestProxy)(object)operations).Handler = (_, _) => Task.CompletedTask;
            Service = new PropertyImportService(new RecordingFactory(), connection, operations, errors);
        }
        public Task<PropertyImportPreview> Prepare(string file, string mode = "覆盖") => Service.PrepareAsync(file, Owner, "Owner", mode);
        private static string Fault(string code, string message) => $"<SOAP-ENV:Fault><faultcode>{code}</faultcode><faultstring>{message}</faultstring></SOAP-ENV:Fault>";
    }
    private sealed class RecordingFactory : IDbContextFactory<ArasToolkitDbContext>
    {
        public ArasToolkitDbContext CreateDbContext() => new RecordingContext();
        public Task<ArasToolkitDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) => Task.FromResult(CreateDbContext());
    }
    private sealed class RecordingContext() : ArasToolkitDbContext(new DbContextOptionsBuilder<ArasToolkitDbContext>()
        .UseSqlServer("Server=(local);Database=UnusedRegressionDatabase;Integrated Security=True").Options)
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
