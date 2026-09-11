using System.Reflection;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using ArasToolkit.Services.Services;
using Microsoft.EntityFrameworkCore;

// 真实服务与 WinUI ViewModel，使用独立内存数据库；不访问工具箱或 Aras 数据库。
var options = new DbContextOptionsBuilder<ArasToolkitDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
var factory = new TestFactory(options);
var logCalls = new List<string>();
T LogProxy<T>() where T : class
{
    var proxy = DispatchProxy.Create<T, TestProxy>();
    ((TestProxy)(object)proxy).Handler = (method, args) =>
    {
        logCalls.Add(method.Name);
        return Task.CompletedTask;
    };
    return proxy;
}
var operations = LogProxy<IOperationLogService>();
var errors = LogProxy<IErrorLogService>();
var snippets = new CommonQuerySnippetService(factory, operations, errors);
var records = new RelatedCodeRecordService(factory, operations, errors);
var xml = new SavedXmlService(factory, operations, errors);
var previousUser = CurrentUserContext.Current;
int checks = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    checks++;
}
void Login(string id, bool admin = false) => CurrentUserContext.Current = new AppUserInfo
{
    Id = id, Username = id, IsAdmin = admin, Role = admin ? "Admin" : "User"
};
async Task Reject(Func<Task> action)
{
    var count = logCalls.Count;
    try { await action(); }
    catch (InvalidOperationException)
    {
        Check(logCalls.Count > count, "拒绝写入仍需记录错误日志");
        return;
    }
    throw new InvalidOperationException("他人记录写入应被拒绝");
}
async Task Ready(Func<bool> busy)
{
    for (var attempt = 0; busy() && attempt < 500; attempt++) await Task.Delay(10);
    Check(!busy(), "ViewModel 加载应完成");
}

try
{
    await using (var db = factory.CreateDbContext())
    {
        foreach (var (id, index) in new[] { ("admin-a", 0), ("admin-b", 1), ("user-c", 2) })
        {
            var created = new DateTime(2026, 9, 1).AddDays(index);
            db.CommonQuerySnippets.Add(new CommonQuerySnippet
            {
                Id = "s-" + id, UserId = id, Title = id, ContentType = index == 1 ? "AML" : "SQL",
                Content = index == 1 ? "<AML><Item /></AML>" : "select 1", CreatorOn = created
            });
            db.RelatedCodeRecords.Add(new RelatedCodeRecord
            {
                Id = "r-" + id, UserId = id, Title = id, CreatorOn = created,
                Segments = [new RelatedCodeSegment
                {
                    Id = "c-" + id, RecordId = "r-" + id, SegmentName = "示例",
                    CodeContent = "needle-" + id, CreatorOn = created
                }]
            });
            db.SavedXmlItems.Add(new SavedXml
            {
                Id = "x-" + id, UserId = id, Name = id, XmlContent = "<root />", CreatorOn = created
            });
        }
        await db.SaveChangesAsync();
    }

    foreach (var admin in new[] { "admin-a", "admin-b" })
    {
        Login(admin, true);
        Check((await snippets.GetAllAsync()).Count == 3, "每个管理员应看到全部片段");
        Check((await snippets.GetAllAsync("AML", "Item")).Single().Id == "s-admin-b", "共享查询仍支持类型与内容筛选");
        Check((await snippets.GetAllAsync()).First().UserId == "user-c", "片段仍按创建时间倒序");
        Check((await records.GetAllAsync()).Count == 3, "每个管理员应看到全部主题");
        Check((await records.GetAllAsync("needle-user-c")).Single().Id == "r-user-c", "可搜索其他账号代码段内容");
        Check((await records.GetByIdAsync("r-user-c"))!.Segments.Single().CodeContent == "needle-user-c", "共享主题可加载完整代码段");
        Check((await xml.GetAllAsync(true)).Count == 3, "XML 格式化管理员可查询全部");
        Check((await xml.GetAllAsync()).Single().UserId == admin, "XML 默认查询保持本人范围");
    }
    Login("user-c");
    Check((await snippets.GetAllAsync()).Single().UserId == "user-c", "普通用户片段仍隔离");
    Check((await records.GetAllAsync()).Single().UserId == "user-c", "普通用户主题仍隔离");
    Check(await records.GetByIdAsync("r-admin-a") == null, "普通用户不能按 ID 读取他人详情");
    Check((await xml.GetAllAsync(true)).Single().UserId == "user-c", "普通用户即使请求全部 XML 也仍隔离");
    CurrentUserContext.Current = null;
    Check((await snippets.GetAllAsync()).Count == 0 && (await records.GetAllAsync()).Count == 0
        && (await xml.GetAllAsync(true)).Count == 0, "未登录不会获得管理员查询范围");

    Login("admin-a", true);
    await Reject(() => snippets.SaveAsync(new CommonQuerySnippet { Id = "s-admin-b", Title = "改名", Content = "select 2" }));
    await Reject(() => records.SaveRecordAsync(new RelatedCodeRecord { Id = "r-admin-b", Title = "改名" }));
    await Reject(() => records.SaveSegmentAsync("r-admin-b", new RelatedCodeSegment { SegmentName = "新段", CodeContent = "x" }));
    await Reject(() => records.DeleteRecordAsync("r-admin-b"));
    await Reject(() => records.DeleteSegmentAsync("r-admin-b", "c-admin-b"));
    await Reject(() => records.ReorderRecordsAsync(["r-admin-b", "r-admin-a"]));
    await Reject(() => records.ReorderSegmentsAsync("r-admin-b", ["c-admin-b"]));
    await snippets.DeleteAsync("s-admin-b");
    await xml.DeleteAsync("x-admin-b");
    await using (var db = factory.CreateDbContext())
    {
        Check(await db.CommonQuerySnippets.CountAsync() == 3 && await db.RelatedCodeRecords.CountAsync() == 3,
            "保存共享记录不能误建副本");
        Check((await db.CommonQuerySnippets.FindAsync("s-admin-b"))!.Title == "admin-b", "他人片段未修改或删除");
        Check((await db.RelatedCodeRecords.FindAsync("r-admin-b"))!.Title == "admin-b", "他人主题未修改或删除");
        Check(await db.SavedXmlItems.FindAsync("x-admin-b") != null, "他人 XML 未删除");
    }

    var dialogs = new TestDialogs();
    var snippetVm = new CommonQuerySnippetViewModel(snippets, dialogs, errors);
    await Ready(() => snippetVm.IsBusy);
    snippetVm.SelectedSnippet = snippetVm.Snippets.Single(item => item.Id == "s-admin-b");
    Check(!snippetVm.SaveCommand.CanExecute(null) && !snippetVm.DeleteCommand.CanExecute(null), "他人片段禁用保存和删除");
    Check(snippetVm.PreviewContent.Contains("AML"), "他人片段仍可预览复制");
    snippetVm.NewCommand.Execute(null);
    snippetVm.Title = "本人新片段";
    snippetVm.Content = "select 2";
    Check(snippetVm.SaveCommand.CanExecute(null), "本人仍可新建片段");
    snippetVm.SelectedSnippet = snippetVm.Snippets.Single(item => item.Id == "s-admin-a");
    Check(snippetVm.SaveCommand.CanExecute(null) && snippetVm.DeleteCommand.CanExecute(null), "本人片段仍可编辑删除");

    var recordVm = new RelatedCodeRecordViewModel(records, dialogs, errors);
    await Ready(() => recordVm.IsBusy);
    recordVm.SelectedRecord = recordVm.Records.Single(item => item.Id == "r-admin-b");
    await Ready(() => recordVm.IsBusy);
    Check(!recordVm.CanModifyRecord && !recordVm.EditRecordCommand.CanExecute(null)
        && !recordVm.DeleteRecordCommand.CanExecute(null) && !recordVm.NewSegmentCommand.CanExecute(null),
        "他人主题不开放编辑删除或新增代码段");
    recordVm.OpenSegment(recordVm.Segments.Single());
    Check(recordVm.OpenedSegment!.CodeContent == "needle-admin-b", "他人代码可打开复用");
    recordVm.BeginEditSegment(recordVm.Segments.Single());
    Check(!recordVm.IsSegmentEditorVisible, "代码段事件入口不能编辑他人代码");
    recordVm.SelectedRecord = recordVm.Records.Single(item => item.Id == "r-admin-a");
    await Ready(() => recordVm.IsBusy);
    Check(recordVm.CanModifyRecord && recordVm.EditRecordCommand.CanExecute(null), "本人主题仍可编辑");

    var xmlVm = new DataToolsViewModel(new DataToolService(errors), xml, errors, null!, dialogs);
    await xmlVm.RefreshSavedXmlAsync();
    Check(xmlVm.SavedXmlItems.Count == 3, "格式化页传入管理员共享查询选项");
    xmlVm.SelectedSavedXml = xmlVm.SavedXmlItems.Single(item => item.Id == "x-admin-b");
    Check(xmlVm.LoadSavedXmlToACommand.CanExecute(null) && !xmlVm.DeleteSavedXmlCommand.CanExecute(null),
        "他人 XML 可加载，不可删除");
    xmlVm.LoadSavedXmlToACommand.Execute(null);
    await Ready(() => xmlVm.IsBusy);
    dialogs.NextName = "admin-b";
    xmlVm.SaveXmlCommand.Execute(null);
    await Ready(() => xmlVm.IsBusy);
    Check(xmlVm.StatusMessage.Contains("新的名称"), "共享 XML 同名保存说明权限，不误建重复内容");
    Check((await xml.GetAllAsync(true)).Count == 3, "共享 XML 同名保存未新增副本");
    xmlVm.Configure("XML比对");
    await xmlVm.RefreshSavedXmlAsync();
    Check(xmlVm.SavedXmlItems.Single().UserId == "admin-a", "XML 比对页面保持本人记录");

    // 本人 CRUD 以及共享列表中的本人排序仍正常，其他账号的顺序不变。
    var ownSnippet = new CommonQuerySnippet { Id = "", Title = "新增", Content = "select 3" };
    await snippets.SaveAsync(ownSnippet);
    ownSnippet.Content = "select 4";
    await snippets.SaveAsync(ownSnippet);
    await snippets.DeleteAsync(ownSnippet.Id);
    var ownRecord = await records.SaveRecordAsync(new RelatedCodeRecord { Id = "", Title = "新增" });
    var ownSegment = await records.SaveSegmentAsync(ownRecord.Id, new RelatedCodeSegment { Id = "", SegmentName = "新增", CodeContent = "ok" });
    await recordVm.SearchAsync();
    recordVm.Records.Move(recordVm.Records.IndexOf(recordVm.Records.Single(item => item.Id == ownRecord.Id)), 0);
    await recordVm.PersistRecordOrderAsync();
    Check(!recordVm.HasError, "混合列表排序仅保存本人主题，不因他人记录而失败");
    Check((await records.GetByIdAsync(ownRecord.Id))!.SortOrder == 0
        && (await records.GetByIdAsync("r-admin-b"))!.SortOrder == 0, "本人排序生效，他人顺序不变");
    await records.DeleteSegmentAsync(ownRecord.Id, ownSegment.Id);
    await records.DeleteRecordAsync(ownRecord.Id);
    var savedXml = await xml.SaveAsync("本人 XML", "<old />");
    var updatedXml = await xml.SaveAsync("本人 XML", "<new />");
    Check(savedXml.Id == updatedXml.Id && updatedXml.XmlContent == "<new />", "本人 XML 同名覆盖仍正常");
    await xml.DeleteAsync(savedXml.Id);
    Check((await snippets.GetAllAsync()).Count == 3 && (await records.GetAllAsync()).Count == 3
        && (await xml.GetAllAsync(true)).Count == 3, "本人增改删后原始数据保留");
    Console.WriteLine($"PASS: {checks} checks; admin sharing, owner-only writes, duplicate prevention, XML compare isolation.");
}
finally { CurrentUserContext.Current = previousUser; }

sealed class TestFactory(DbContextOptions<ArasToolkitDbContext> options) : IDbContextFactory<ArasToolkitDbContext>
{
    public ArasToolkitDbContext CreateDbContext() => new(options);
}
public class TestProxy : DispatchProxy
{
    public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args ?? []);
}
sealed class TestDialogs : IDialogService
{
    public string? NextName { get; set; }
    public Task AlertAsync(string title, string message) => Task.CompletedTask;
    public Task<bool> ConfirmAsync(string title, string message, string confirmText = "确定", string cancelText = "取消") => Task.FromResult(true);
    public Task<string?> PromptAsync(string title, string placeholder = "", string defaultValue = "") => Task.FromResult(NextName);
}
