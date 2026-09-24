using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace ArasToolkit.PackageBridge;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.InputEncoding = new UTF8Encoding(false);
        Console.OutputEncoding = new UTF8Encoding(false);
        var output = Console.Out;
        var json = new JavaScriptSerializer { MaxJsonLength = 32 * 1024 * 1024 };
        BridgeRequest? request = null;
        void Emit(BridgeEvent message)
        {
            if (!string.IsNullOrEmpty(request?.Password)) message.Message = message.Message.Replace(request!.Password, "[已隐藏]");
            lock (output) { output.WriteLine(json.Serialize(message)); output.Flush(); }
        }
        try
        {
            // Only the installation path is on the command line. Credentials arrive over stdin.
            if (args.Length != 1) throw new InvalidOperationException("缺少官方引擎目录。");
            var directory = Path.GetFullPath(args[0]);
            var version = FileVersionInfo.GetVersionInfo(Path.Combine(directory, "Libs.dll")).FileVersion;
            if (version != "14.0.28.41847") throw new InvalidOperationException("官方引擎版本未适配：" + version);
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                var path = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };
            request = json.Deserialize<BridgeRequest>(Console.ReadLine() ?? throw new InvalidOperationException("缺少请求。"));
            // The official engine prints progress to Console. Our protocol uses the captured writer.
            Console.SetOut(TextWriter.Null);
            EngineRunner.Run(request, Emit);
            Emit(new BridgeEvent { Kind = "complete", Message = "完成" });
            return 0;
        }
        catch (OperationCanceledException)
        {
            Emit(new BridgeEvent { Kind = "cancelled", Message = "已在官方引擎操作边界取消；请回读目标确认已完成项目。" });
            return 2;
        }
        catch (Exception ex)
        {
            Emit(new BridgeEvent { Kind = "error", Message = ex.Message });
            return 1;
        }
        finally { if (request != null) request.Password = ""; }
    }
}

internal sealed class BridgeRequest
{
    public string Operation { get; set; } = "";
    public string Url { get; set; } = "";
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Directory { get; set; } = "";
    public string Manifest { get; set; } = "";
    public string PackageName { get; set; } = "";
    public string Release { get; set; } = "";
    public string Description { get; set; } = "";
    public string[] Languages { get; set; } = Array.Empty<string>();
    public List<BridgeItem> Items { get; set; } = new List<BridgeItem>();
}

internal sealed class BridgeItem
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
}

internal sealed class BridgeEvent
{
    public string Kind { get; set; } = "";
    public string Message { get; set; } = "";
    public int Completed { get; set; }
    public int Total { get; set; }
}
