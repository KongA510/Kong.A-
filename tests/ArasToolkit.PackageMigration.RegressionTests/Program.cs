using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Services;

await Suite.Run();

static class Suite
{
    const string A = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", B = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB", C = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";
    static int count;
    static readonly string Root = Path.Combine(Path.GetTempPath(), "ArasPackageTests", Guid.NewGuid().ToString("N"));
    static string Xml(string type, string id, string body) => $"<Item type='{type}' id='{id}' action='add'>{body}</Item>";
    static void Check(bool value, string name) { if (!value) throw new Exception("FAIL: " + name); Console.WriteLine("PASS " + ++count + " " + name); }
    static async Task Reject(Func<Task> action, string name) { try { await action(); } catch (Exception) { Check(true, name); return; } throw new Exception("FAIL did not reject: " + name); }
    static async Task<MigrationPackage> Package(string aml)
    {
        var dir = Path.Combine(Root, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(dir, "Import", "Items"));
        await File.WriteAllTextAsync(Path.Combine(dir, "Import", "Items", "items.xml"), "<AML>" + aml + "</AML>");
        await PackageFileStore.WriteManifestAsync(dir, "test.package", [], default);
        return await PackageFileStore.ReadAsync(Path.Combine(dir, "imports.mf"));
    }
    public static async Task Run()
    {
        Directory.CreateDirectory(Root);
        CurrentUserContext.Current = new AppUserInfo { Id = "package-tests" };
        var form = Xml("Form", A, "<name>sample</name><Relationships>" + Xml("Field", B, "<label>before</label>") + "</Relationships>");
        var package = await Package(form + Xml("Field", B, "<label>after</label>") + Xml("Form", A, "<width>800</width>"));
        var effective = PackageFileStore.EffectiveItems(package);
        Check(effective.Count == 1 && effective[0].Aml.Contains("after") && effective[0].Aml.Contains("800"), "official multipart Form and owned Field patches");
        var copied = await PackageFileStore.CopySelectionAsync(package, ["Form:" + A], Path.Combine(Root, "selected"), default);
        Check((await PackageFileStore.ReadAsync(copied)).Items.Count == 3, "selected root retains every original patch");
        var relation = await Package(Xml("RelationshipType", A, "<name>R</name><relationship_id>" + Xml("ItemType", B, "<name>R</name>") + "</relationship_id>") + Xml("ItemType", B, "<label>Relation</label>"));
        Check(PackageFileStore.EffectiveItems(relation).Single().Aml.Contains("Relation</label>"), "RelationshipType owns ItemType patches without duplication");
        await PackageFileStore.SealAsync(package, default);
        await File.AppendAllTextAsync(Path.Combine(package.RootDirectory, package.Items[0].File), " ");
        await Reject(() => PackageFileStore.ReadAsync(package.ManifestPath), "checksum detects modified AML");
        await Reject(() => Task.FromResult(PackageXml.Parse("<!DOCTYPE AML [<!ENTITY a SYSTEM 'file:///C:/secret'>]><AML>&a;</AML>")), "DTD is prohibited");
        await Reject(() => Task.FromResult(PackageXml.SafePath(Root, "../escape")), "directory traversal rejected");
        await Reject(() => Package($"<Item type='Method' id='{A}' action='delete'/>"), "delete action rejected");
        await Reject(() => Package($"<Item type='ItemType' id='{A}' action='edit' where='1=1'/>"), "unbounded where action rejected");
        var owned = await Package(Xml("ItemType", A, $"<Relationships><Item type='Property' action='edit' where=\"source_id='{A}' and name='created_by_id'\"><label>Creator</label></Item></Relationships>"));
        Check(owned.Items.Count == 1, "official scoped builtin Property edit accepted");
        var zip = Path.Combine(Root, "attack.zip");
        using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create)) { using var writer = new StreamWriter(archive.CreateEntry("../outside.xml").Open()); writer.Write("bad"); }
        await Reject(() => PackageFileStore.UnzipAsync(zip, Path.Combine(Root, "unzip"), default), "ZIP traversal rejected");
        var credential = new MigrationCredential { Md5Password = "test-secret" };
        Check(!JsonSerializer.Serialize(credential).Contains("test-secret") && !credential.ToString().Contains("test-secret") && PackageXml.Redact("test-secret", credential.Md5Password) == "[已隐藏]", "credentials absent from JSON, display and errors");
        Check(new PackageItemSelection { Id = A, ConfigId = B, IsVersionable = true }.ExportId == B, "versionable items use stable config_id");
        var translated = PackageXml.Parse(Xml("Form", A, "<label xml:lang='zh'>中文</label><label xml:lang='en'>English</label>")).Root!;
        Check(PackageXml.Differences(translated, PackageXml.Parse(Xml("Form", A, "<label xml:lang='en'>English</label>")).Root!).Count == 1, "missing translated value remains visible");
        var property = PackageXml.Parse(Xml("Property", A, "<name>x</name><data_type>string</data_type><stored_length>100</stored_length>")).Root!;
        var shorter = PackageXml.Parse(Xml("Property", A, "<name>x</name><data_type>integer</data_type><stored_length>50</stored_length>")).Root!;
        Check(PackageXml.DestructiveChanges(shorter, property).Count == 2, "type changes and length shrink blocked");
        var namedA = PackageXml.Parse(Xml("ItemType", A, "<method><Item type='Method' action='get'><name>first</name></Item></method>")).Root!;
        var namedB = PackageXml.Parse(Xml("ItemType", A, "<method><Item type='Method' action='get'><name>second</name></Item></method>")).Root!;
        Check(PackageXml.Differences(namedA, namedB).Count == 1, "named Method reference changes remain visible");
        Check(PackageXml.RelationshipKey(XElement.Parse("<Item type='Property' where=\"name='a'\"/>")) != PackageXml.RelationshipKey(XElement.Parse("<Item type='Property' where=\"name='b'\"/>")), "scoped Property patches cannot collapse onto each other");
        Check(PackageXml.Differences(PackageXml.Parse(Xml("List", A, "<name>L</name>")).Root!, PackageXml.Parse(Xml("List", A, "<name>L</name><Relationships>" + Xml("Value", B, "<value>target extra</value>") + "</Relationships>")).Root!).Count == 0, "target-only relationships are retained");

        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>M</name><method_code>return this;</method_code>"));
            f.Target["Method:" + B] = Xml("Method", B, "<name>M</name>");
            Check((await f.Service.PreflightAsync(p, "target", "fake")).Issues.Any(i => i.Message.Contains("同名不同 ID")), "same-name different-ID blocks preflight");
        });
        await WithFixture(async f => {
            var p = await Package(translated.ToString());
            Check((await f.Service.PreflightAsync(p, "target", "fake")).Issues.Any(i => i.Message.Contains("目标未安装包语言")), "missing target language blocks preflight");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>M</name><method_code>return this;</method_code>"));
            var plan = await f.Service.PreflightAsync(p, "target", "fake");
            f.Target["Method:" + A] = Xml("Method", A, "<name>M</name><method_code>changed concurrently</method_code>");
            var result = await f.Service.ExecuteAsync(plan, "fake");
            Check(result.Status == "失败" && f.Engine.ImportCalls == 0 && result.Message.Contains("已变化"), "target change prevents import");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>M</name><method_code>next</method_code>"));
            f.Target["Method:" + A] = Xml("Method", A, "<name>M</name><method_code>previous</method_code>");
            var plan = await f.Service.PreflightAsync(p, "target", "fake");
            f.Engine.FailBackup = true;
            var result = await f.Service.ExecuteAsync(plan, "fake");
            Check(result.Status == "失败" && f.Engine.ImportCalls == 0, "backup failure stops import");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>M</name><method_code>next</method_code>"));
            var result = await f.Service.ExecuteAsync(await f.Service.PreflightAsync(p, "target", "fake"), "fake");
            var repeated = await f.Service.PreflightAsync(p, "target", "fake");
            Check(result.Status == "成功" && repeated.Differences.All(x => x.Kind == "相同") && !repeated.CanExecute && f.Engine.ImportCalls == 1, "readback and duplicate import skip");
            Check(File.Exists(Path.Combine(result.Directory, "backup", "new-items.json")), "new-item inventory saved before import");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>A</name>") + Xml("Method", B, "<name>B</name>"));
            f.Engine.FailAfterOne = true;
            var result = await f.Service.ExecuteAsync(await f.Service.PreflightAsync(p, "target", "fake"), "fake");
            Check(result.Status == "失败" && result.Items.Count(x => x.Status == "已验证") == 1 && result.Items.Count(x => x.Status == "未写入") == 1, "partial failure stops and verifies actual target state");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>A</name>"));
            f.Engine.CancelImport = true;
            var result = await f.Service.ExecuteAsync(await f.Service.PreflightAsync(p, "target", "fake"), "fake");
            Check(result.Status == "已取消" && result.Items.All(x => x.Status == "未写入"), "official cancellation boundary and readback");
        });
        await WithFixture(async f => {
            f.Source["Method:" + A] = Xml("Method", A, $"<name>A</name><reference type='Method'>{B}</reference>");
            f.Source["Method:" + B] = Xml("Method", B, $"<name>B</name><reference type='Method'>{A}</reference>");
            var analysis = await f.Service.AnalyzeAsync("source", [new() { Type = "Method", Id = A, Name = "A" }], "fake");
            Check(analysis.Items.Count == 2 && analysis.CanExport, "dependency cycles terminate and deduplicate");
        });
        await WithFixture(async f => {
            f.Source["Method:" + A] = Xml("Method", A, "<name>A</name><reference><Item type='Method' action='get'><name>B</name></Item></reference>");
            f.Source["Method:" + B] = Xml("Method", B, "<name>B</name>");
            var analysis = await f.Service.AnalyzeAsync("source", [new() { Type = "Method", Id = A }], "fake");
            Check(analysis.Items.Count == 2 && analysis.CanExport, "name-based official references join the dependency closure");
            var p = await Package(f.Source["Method:" + A] + f.Source["Method:" + B]);
            Check((await f.Service.PreflightAsync(p, "target", "fake")).CanExecute, "new named reference resolves within the same package");
            var broken = await Package(f.Source["Method:" + A]);
            Check(!(await f.Service.PreflightAsync(broken, "target", "fake")).CanExecute, "unresolved named reference blocks import");
        });
        await WithFixture(async f => {
            var p = await Package(Xml("Method", A, "<name>A</name>"));
            p.Source = new MigrationEndpoint { Url = "http://test/target", Database = "target" };
            await PackageFileStore.WriteMetadataAsync(p.RootDirectory, new PackageFileStore.PackageMetadata { Source = p.Source }, default);
            p = await PackageFileStore.ReadAsync(p.ManifestPath);
            await Reject(() => f.Service.PreflightAsync(p, "target", "fake"), "same database is rejected");
        });
        Console.WriteLine($"{count} checks passed. Evidence: {Root}");
    }
    static Task WithFixture(Func<Fixture, Task> test) => test(new Fixture(Path.Combine(Root, Guid.NewGuid().ToString("N"))));
}

sealed class Fixture : IArasMigrationSessionFactory, IErrorLogService, IOperationLogService
{
    public Dictionary<string, string> Source { get; } = [];
    public Dictionary<string, string> Target { get; } = [];
    public FakeEngine Engine { get; }
    public ArasPackageService Service { get; }
    public Fixture(string root) { Engine = new(this); Service = new(this, Engine, this, this, root); }
    public Task<List<MigrationEndpoint>> GetConnectionsAsync(CancellationToken ct = default) => Task.FromResult(new List<MigrationEndpoint>());
    public Task<MigrationCredential> GetCredentialAsync(string id, CancellationToken ct = default) => Task.FromResult(new MigrationCredential());
    public Task<IArasMigrationSession> OpenAsync(string id, CancellationToken ct = default) => Task.FromResult<IArasMigrationSession>(new FakeSession(this, id));
    public Task LogErrorAsync(string fn, string message, string? level = null, string? stack = null) => Task.CompletedTask;
    public Task<(List<ErrorLog> Items, int TotalCount)> GetPagedEntriesAsync(int page, int size, string? level = null, DateTime? from = null, DateTime? to = null) => Task.FromResult((new List<ErrorLog>(), 0));
    public Task<List<ErrorLog>> GetAllEntriesAsync() => Task.FromResult(new List<ErrorLog>());
    public Task ClearAllAsync() => Task.CompletedTask;
    public Task LogAsync(string op, string entity, string id, string? desc = null, string? user = null) => Task.CompletedTask;
    public Task<(List<OperationLog> Items, int TotalCount)> GetPagedAsync(int page, int size, string? type = null) => Task.FromResult((new List<OperationLog>(), 0));
    public string GetCurrentUserName() => "test";
}
sealed class FakeSession(Fixture fixture, string id) : IArasMigrationSession
{
    public MigrationEndpoint Endpoint => new() { Id = id, Database = id, Url = "http://test/" + id, Version = "14.0.9.36244" };
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public Task<string> QueryAsync(string aml, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        PackageXml.ValidateReadOnly(aml);
        var query = PackageXml.Parse(aml).Root!.Element("Item")!;
        var type = (string)query.Attribute("type")!;
        if (type == "Language") return Task.FromResult("<Result><Item type='Language'><code>en</code></Item></Result>");
        if (type == "ItemType" && query.Element("Relationships") != null)
            return Task.FromResult($"<Result><Item type='ItemType' id='11111111111111111111111111111111'><name>{query.Element("name")!.Value}</name><is_versionable>0</is_versionable><Relationships><Item type='Property'><name>name</name></Item></Relationships></Item></Result>");
        var store = id == "source" ? fixture.Source : fixture.Target;
        var items = store.Values.Select(x => PackageXml.Parse(x).Root!).Where(x => (string?)x.Attribute("type") == type);
        if (query.Attribute("id") != null) items = items.Where(x => (string?)x.Attribute("id") == (string?)query.Attribute("id"));
        if (query.Element("name") != null) items = items.Where(x => (string?)x.Element("name") == (string?)query.Element("name"));
        return Task.FromResult(new XElement("Result", items).ToString());
    }
}
sealed class FakeEngine(Fixture fixture) : IOfficialPackageEngine
{
    public bool FailBackup, FailAfterOne, CancelImport;
    public int ImportCalls;
    public Task<PackageEngineInfo> ProbeAsync(string dir, CancellationToken ct = default) => Task.FromResult(new PackageEngineInfo { IsSupported = true, Directory = dir, Version = OfficialPackageEngine.SupportedVersion });
    public async Task ExportAsync(PackageEngineExportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken ct = default)
    {
        if (FailBackup && Path.GetFileName(Path.GetDirectoryName(request.OutputDirectory)) == "backup") throw new IOException("backup write failure");
        var store = request.ConnectionId == "source" ? fixture.Source : fixture.Target;
        foreach (var item in request.Items)
        {
            var folder = Path.Combine(request.OutputDirectory, item.Type); Directory.CreateDirectory(folder);
            await File.WriteAllTextAsync(Path.Combine(folder, item.ExportId + ".xml"), "<AML>" + store[item.Key] + "</AML>", ct);
        }
    }
    public async Task ImportAsync(PackageEngineImportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken ct = default)
    {
        ImportCalls++;
        if (CancelImport) throw new OperationCanceledException();
        foreach (var entry in PackageFileStore.EffectiveItems(await PackageFileStore.ReadAsync(request.ManifestPath, ct)))
        {
            fixture.Target[entry.Key] = entry.Aml;
            if (FailAfterOne) throw new IOException("partial failure");
        }
    }
}
