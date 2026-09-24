using System.Net;
using System.Text.RegularExpressions;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

public sealed partial class ArasPackageService
{
    private static readonly HttpClient ResourceClient = new(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(12) };

    private async Task<List<PackageIssue>> CheckResourcesAsync(MigrationPackage package, MigrationEndpoint target, CancellationToken ct)
    {
        var resources = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in package.Items)
        foreach (var node in PackageXml.Parse(entry.Aml).Descendants().Where(x => !x.HasElements))
        {
            if (node.Name.LocalName is "image" or "open_icon" or "closed_icon" or "stylesheet" or "url")
            {
                if (!string.IsNullOrWhiteSpace(node.Value)) resources.TryAdd(node.Value.Trim(), entry.SelectionKey);
            }
            if (node.Name.LocalName is "html_code" or "css")
                foreach (Match match in Regex.Matches(node.Value, "(?:src\\s*=\\s*[\"']|url\\(\\s*[\"']?)([^\"')>\\s]+)", RegexOptions.IgnoreCase))
                    resources.TryAdd(match.Groups[1].Value, entry.SelectionKey);
        }
        var issues = new List<PackageIssue>();
        var endpoint = new Uri(target.Url.TrimEnd('/') + "/");
        if (endpoint.AbsolutePath.TrimEnd('/').EndsWith("/Server", StringComparison.OrdinalIgnoreCase)) endpoint = new Uri(endpoint, "../");
        var clientBase = new Uri(endpoint, "Client/scripts/");
        foreach (var (resource, key) in resources)
        {
            ct.ThrowIfCancellationRequested();
            if (resource.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) continue;
            if (!Uri.TryCreate(clientBase, resource, out var uri) || uri.Scheme is not ("http" or "https"))
            {
                issues.Add(new PackageIssue { ItemKey = key, IsBlocking = true, Message = "无法验证的文件资源，请先部署并使用可访问的路径：" + resource });
                continue;
            }
            if (uri.GetLeftPart(UriPartial.Authority) != endpoint.GetLeftPart(UriPartial.Authority))
            {
                issues.Add(new PackageIssue { ItemKey = key, Message = "外部资源需单独核对（未传递登录凭据）：" + resource });
                continue;
            }
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, uri);
                using var response = await ResourceClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
                if (!response.IsSuccessStatusCode)
                    issues.Add(new PackageIssue { ItemKey = key, IsBlocking = response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone,
                        Message = $"目标资源返回 HTTP {(int)response.StatusCode}，请检查部署：{resource}" });
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                await _errors.LogErrorAsync("导包-资源预检", ex.Message, ErrorLog.LevelP1);
                issues.Add(new PackageIssue { ItemKey = key, Message = "资源访问未确认：" + resource });
            }
        }
        return issues;
    }
}
