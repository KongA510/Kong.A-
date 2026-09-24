using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

public sealed class OfficialPackageEngine(IArasMigrationSessionFactory sessions, IErrorLogService errors) : IOfficialPackageEngine
{
    public const string SupportedVersion = "14.0.28.41847";
    public static string NormalizeDirectory(string directory)
    {
        directory = Path.GetFullPath(directory);
        return File.Exists(Path.Combine(directory, "ConsoleUpgrade", "Libs.dll")) ? Path.Combine(directory, "ConsoleUpgrade") : directory;
    }

    public Task<PackageEngineInfo> ProbeAsync(string directory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(directory)) return Task.FromResult(new PackageEngineInfo { Message = "请选择官方 PackageImportExportUtilities 目录。" });
        var result = new PackageEngineInfo { Directory = NormalizeDirectory(directory) };
        var files = new[] { "Libs.dll", "IOM.dll", "Aras.Net.dll", "Aras.Cryptography.dll" };
        var missing = files.Where(f => !File.Exists(Path.Combine(result.Directory, f))).ToList();
        if (missing.Count > 0) result.Message = "缺少官方文件：" + string.Join("、", missing);
        else
        {
            result.Version = FileVersionInfo.GetVersionInfo(Path.Combine(result.Directory, "Libs.dll")).FileVersion ?? "未知";
            result.IsSupported = result.Version == SupportedVersion && FileVersionInfo.GetVersionInfo(Path.Combine(result.Directory, "IOM.dll")).FileVersion == SupportedVersion;
            result.Message = result.IsSupported ? $"已适配官方引擎 {result.Version}；R37 尚未验收" : $"检测到 {result.Version}，此版本尚未适配，禁止迁入。";
        }
        return Task.FromResult(result);
    }

    public Task ExportAsync(PackageEngineExportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default)
        => RunAsync(request.EngineDirectory, request.ConnectionId, new Dictionary<string, object?>
        {
            ["Operation"] = "export", ["Directory"] = request.OutputDirectory, ["PackageName"] = request.PackageName,
            ["Languages"] = request.Languages, ["Items"] = request.Items.Select(i => new { Id = i.ExportId, i.Type, i.Name }).ToList()
        }, progress, cancellationToken);

    public Task ImportAsync(PackageEngineImportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default)
        => RunAsync(request.EngineDirectory, request.ConnectionId, new Dictionary<string, object?>
        {
            ["Operation"] = "import", ["Manifest"] = request.ManifestPath, ["Release"] = request.TargetRelease,
            ["Description"] = request.Description, ["Languages"] = request.Languages
        }, progress, cancellationToken);

    private async Task RunAsync(string engineDirectory, string connectionId, Dictionary<string, object?> request,
        IProgress<PackageMigrationProgress>? progress, CancellationToken cancellationToken)
    {
        var probe = await ProbeAsync(engineDirectory, cancellationToken);
        if (!probe.IsSupported) throw new InvalidOperationException(probe.Message);
        var credential = await sessions.GetCredentialAsync(connectionId, cancellationToken);
        try
        {
            request["Url"] = credential.Endpoint.Url; request["Database"] = credential.Endpoint.Database;
            request["Username"] = credential.Endpoint.Username; request["Password"] = credential.Md5Password;
            var bridge = Path.Combine(AppContext.BaseDirectory, "PackageBridge", "ArasToolkit.PackageBridge.exe");
            if (!File.Exists(bridge)) throw new FileNotFoundException("缺少导包桥接程序，请重新构建或完整安装工具箱。", bridge);
            var start = new ProcessStartInfo(bridge)
            {
                UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true,
                StandardInputEncoding = new UTF8Encoding(false), StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = probe.Directory
            };
            start.ArgumentList.Add(probe.Directory);
            using var process = Process.Start(start) ?? throw new InvalidOperationException("无法启动官方引擎桥接程序。");
            var stderrTask = process.StandardError.ReadToEndAsync();
            await process.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request));
            await process.StandardInput.FlushAsync();
            request.Remove("Password");
            // Never kill an active import: the server may still commit its current request.
            using var registration = cancellationToken.Register(() =>
            {
                try { process.StandardInput.WriteLine("cancel"); process.StandardInput.Flush(); }
                catch (Exception ex) { _ = errors.LogErrorAsync("导包-取消通知", PackageXml.Redact(ex.Message, credential.Md5Password), ErrorLog.LevelP1); }
            });
            string? failure = null;
            bool completed = false, cancelled = false;
            while (await process.StandardOutput.ReadLineAsync() is { } line)
            {
                using var message = JsonDocument.Parse(line);
                var root = message.RootElement;
                var kind = root.GetProperty("Kind").GetString();
                var text = PackageXml.Redact(root.GetProperty("Message").GetString() ?? "", credential.Md5Password);
                if (kind == "error") failure = text;
                if (kind == "cancelled") cancelled = true;
                if (kind == "complete") completed = true;
                progress?.Report(new PackageMigrationProgress
                {
                    Stage = (string)request["Operation"]! == "export" ? "官方导出" : "官方迁入", Message = text,
                    Completed = root.GetProperty("Completed").GetInt32(), Total = root.GetProperty("Total").GetInt32()
                });
            }
            await process.WaitForExitAsync();
            var stderr = PackageXml.Redact(await stderrTask, credential.Md5Password);
            if (cancelled || process.ExitCode == 2) throw new OperationCanceledException(cancellationToken);
            if (!completed || process.ExitCode != 0 || failure != null)
                throw new InvalidOperationException(failure ?? (string.IsNullOrWhiteSpace(stderr) ? "官方引擎异常结束，目标状态需要回读确认。" : stderr));
        }
        catch (Exception ex)
        {
            var sanitized = PackageXml.Redact(ex.Message, credential.Md5Password);
            await errors.LogErrorAsync("导包-官方引擎", sanitized, ErrorLog.LevelP1);
            if (ex is OperationCanceledException) throw;
            throw new InvalidOperationException(sanitized);
        }
        finally { request.Remove("Password"); credential.Md5Password = ""; }
    }
}
