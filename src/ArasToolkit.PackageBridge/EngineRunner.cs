using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Aras.IOM;
using Aras.Tools.SolutionUpgrade;

namespace ArasToolkit.PackageBridge;

internal static class EngineRunner
{
    public static void Run(BridgeRequest request, Action<BridgeEvent> emit)
    {
        var cancelled = 0;
        ImportExportManager? manager = null;
        var listener = new Thread(() =>
        {
            try
            {
                while (Console.ReadLine() is string command)
                    if (command == "cancel") { Interlocked.Exchange(ref cancelled, 1); manager?.Cancel(); }
            }
            catch (Exception ex) { emit(new BridgeEvent { Kind = "warning", Message = "取消通道关闭：" + ex.Message }); }
        }) { IsBackground = true };
        listener.Start();
        void CheckCancelled() { if (Volatile.Read(ref cancelled) != 0) throw new OperationCanceledException(); }
        var connection = IomFactory.CreateHttpServerConnection(request.Url, request.Database, request.Username, request.Password);
        try
        {
            connection.Timeout = 120000;
            var login = connection.Login();
            if (login.isError()) throw new InvalidOperationException(login.getErrorString());
            var helper = new CItemHelper(connection);
            helper.Login(); // Initializes the engine's server-version and compatibility metadata.
            CheckCancelled();
            if (request.Operation == "export")
            {
                Directory.CreateDirectory(request.Directory);
                helper.Folder = request.Directory;
                var exporter = new CExportItems(helper);
                var queries = new ConcurrentDictionary<string, Lazy<Item>>();
                var selected = request.Items.GroupBy(i => i.Type).ToDictionary(g => g.Key, g => new HashSet<string>(g.Select(i => i.Id), StringComparer.OrdinalIgnoreCase));
                var count = 0;
                foreach (var item in request.Items)
                {
                    CheckCancelled();
                    // Null excludedRefs means Don't Remove, not the engine's silent-removal default.
                    // Use ID filenames so duplicate/unsafe keyed names cannot overwrite another element.
                    exporter.Export(new ExportItem(item.Id, item.Id, item.Type), request.PackageName, "3", null,
                        ImportExport.ExportMetadataWithLanguageResources, request.Languages, queries, selected);
                    emit(new BridgeEvent { Kind = "progress", Message = item.Type + " · " + item.Name, Completed = ++count, Total = request.Items.Count });
                }
            }
            else if (request.Operation == "import")
            {
                var context = new SolutionUpgradeContext
                {
                    Action = ImportExport.Import, Url = request.Url, DataBase = request.Database, UserName = request.Username,
                    Password = "", ManifestFile = request.Manifest, WorkingDirectory = Path.GetDirectoryName(request.Manifest),
                    LogFilePath = Path.Combine(Path.GetDirectoryName(request.Manifest)!, "official-import.log"),
                    Languages = request.Languages, Timeout = 120000, Verbose = false
                };
                var messages = new Messages(emit, () => { if (Volatile.Read(ref cancelled) != 0) manager?.Cancel(); });
                manager = new ImportExportManager(context, messages, helper);
                CheckCancelled();
                var result = manager.ImportSolutions(manager.ManifestFile.GetAllPackageNames(), new SUImportContext
                { bMerge = true, bFast = false, bVault = false, Release = request.Release, Description = request.Description });
                CheckCancelled();
                if (result != 0 || messages.Failure != null) throw new InvalidOperationException(messages.Failure ?? "官方引擎返回失败，请查看执行日志。");
            }
            else throw new InvalidOperationException("不支持的引擎操作。");
            CheckCancelled();
        }
        finally
        {
            try { connection.Logout(); }
            catch (Exception ex) { emit(new BridgeEvent { Kind = "warning", Message = "关闭引擎连接：" + ex.Message }); }
        }
    }

    private sealed class CallbackMessage(Action<string> callback) : Message
    {
        public override bool? Execute() { callback(Text); return false; }
    }

    private sealed class Messages(Action<BridgeEvent> emit, Action check) : IMessagesFactory
    {
        public string? Failure { get; private set; }
        private Message Create(string kind) => new CallbackMessage(text =>
        {
            check();
            if (kind == "error") Failure = text;
            emit(new BridgeEvent { Kind = kind, Message = text ?? "" });
        });
        public Message GetErrorMessage() => Create("error");
        public Message GetErrorMessageQuestion() => Create("error");
        public Message GetWarningMessage() => Create("warning");
        public Message GetStatusMessage() => Create("progress");
        public Message GetCurrentPackageMessage() => Create("progress");
    }
}
