using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;

namespace ArasToolkit.Services.Services;

/// <summary>
/// AI 统一调度器 — 对所有 OpenAI 兼容服务提供统一调用入口。
/// 自动读取当前启用的 AiModelConfig（3 参数 + ExtraParams JSON），
/// 构造最小协议请求体，支持自定义参数合并、错误重试、流式输出。
/// </summary>
public class AiDispatcherService : IAiDispatcherService
{
    private readonly IAiModelConfigService _modelConfigService;
    private readonly IErrorLogService _errorLogService;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
    private const int MaxRetries = 3;

    public AiDispatcherService(
        IAiModelConfigService modelConfigService,
        IErrorLogService errorLogService)
    {
        _modelConfigService = modelConfigService;
        _errorLogService = errorLogService;
    }

    /// <inheritdoc />
    public async Task<string> ChatAsync(
        string prompt,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var (apiUrl, apiKey, body) = await PrepareRequestAsync(prompt, options);
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        Exception? lastEx = null;
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var respText = await SendSingleAsync(apiUrl, apiKey, json, cancellationToken);
                return ExtractContent(respText);
            }
            catch (HttpRequestException ex) when (IsRetryable(ex))
            {
                lastEx = ex;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
        }

        await _errorLogService.LogErrorAsync("AI调度-重试耗尽",
            $"重试 {MaxRetries} 次后仍失败: {lastEx?.Message}",
            ErrorLog.LevelP0, lastEx?.StackTrace);

        throw new InvalidOperationException(
            $"AI 调用失败（已重试 {MaxRetries} 次）: {lastEx?.Message}", lastEx);
    }

    /// <inheritdoc />
    public async Task ChatStreamAsync(
        string prompt,
        Action<string> onChunk,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var opts = options ?? new ChatOptions();
        opts.Stream = true;

        var (apiUrl, apiKey, body) = await PrepareRequestAsync(prompt, opts);
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        using var response = await _httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
       using var reader = new StreamReader(stream, Encoding.UTF8);
       string? line;
       while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
       {
           if (!line.StartsWith("data: ") || line.Length < 7) continue;
           var data = line[6..];
           if (data == "[DONE]") break;
           try
           {
               var chunk = JsonSerializer.Deserialize<StreamChunk>(data);
               var delta = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
               if (!string.IsNullOrEmpty(delta))
                   onChunk(delta);
           }
           catch (Exception ex) { await _errorLogService.LogErrorAsync("AI流式-SSE解析失败", "原始数据: " + data + ", 错误: " + ex.Message, ErrorLog.LevelP1, ex.StackTrace); }
       }
    }

    /// <inheritdoc />
    public async Task<AiModelConfig?> GetCurrentModelAsync()
        => await _modelConfigService.GetEnabledAsync();

    // ===== 私有方法 =====

    /// <summary>
    /// 准备完整请求：解析配置 + 构造请求体（含 ExtraParams 合并）。
    /// </summary>
    private async Task<(string url, string key, Dictionary<string, object?> body)> PrepareRequestAsync(
        string prompt, ChatOptions? options)
    {
        var aiModel = await _modelConfigService.GetEnabledAsync();
        var apiUrl = !string.IsNullOrWhiteSpace(aiModel?.ApiBaseUrl)
            ? aiModel.ApiBaseUrl
            : "https://apihub.agnes-ai.com/v1/chat/completions";
        var apiKey = !string.IsNullOrWhiteSpace(aiModel?.ApiKey)
            ? aiModel.ApiKey
            : throw new InvalidOperationException("请先在设置中配置 AI API Key");
        var modelId = !string.IsNullOrWhiteSpace(aiModel?.ModelIdentifier)
            ? aiModel.ModelIdentifier
            : "agnes-2.0-flash";

        var body = new Dictionary<string, object?>
        {
            ["model"] = modelId,
            ["messages"] = BuildMessages(prompt, options?.SystemMessage)
        };

        // ChatOptions → 标准字段
        if (options != null)
        {
            if (options.Temperature.HasValue) body["temperature"] = options.Temperature.Value;
            if (options.MaxTokens.HasValue) body["max_tokens"] = options.MaxTokens.Value;
            if (options.TopP.HasValue) body["top_p"] = options.TopP.Value;
            if (options.MaxCompletionTokens.HasValue) body["max_completion_tokens"] = options.MaxCompletionTokens.Value;
            if (options.Stream.HasValue) body["stream"] = options.Stream.Value;
            if (options.FrequencyPenalty.HasValue) body["frequency_penalty"] = options.FrequencyPenalty.Value;
            if (options.PresencePenalty.HasValue) body["presence_penalty"] = options.PresencePenalty.Value;
        }

        // ExtraParams 用户自定义 JSON 合并（优先级最高，覆盖上面所有字段）
        MergeExtraParams(body, aiModel?.ExtraParams);

        return (apiUrl, apiKey, body);
    }

    /// <summary>合并 ExtraParams JSON 到请求体根级（覆盖同名 key）。</summary>
    private static void MergeExtraParams(Dictionary<string, object?> body, string? extraParamsJson)
    {
        if (string.IsNullOrWhiteSpace(extraParamsJson)) return;
        try
        {
            using var doc = JsonDocument.Parse(extraParamsJson.Trim());
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                body[prop.Name] = prop.Value.ValueKind switch
                {
                    JsonValueKind.String => prop.Value.GetString(),
                    JsonValueKind.Number when prop.Value.TryGetInt64(out var i) => i,
                    JsonValueKind.Number => prop.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => prop.Value.GetRawText()
                };
            }
        }
        catch (JsonException)
        {
            // ExtraParams 格式有误时静默忽略，不中断主流程
        }
    }

    private static List<object> BuildMessages(string prompt, string? systemMessage)
    {
        var msgs = new List<object>();
        if (!string.IsNullOrWhiteSpace(systemMessage))
            msgs.Add(new { role = "system", content = systemMessage });
        msgs.Add(new { role = "user", content = prompt });
        return msgs;
    }

    private static async Task<string> SendSingleAsync(
        string url, string apiKey, string json, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static string ExtractContent(string responseBody)
    {
        var result = JsonSerializer.Deserialize<AgnesResponse>(responseBody);
        if (result?.Choices == null || result.Choices.Length == 0)
            throw new InvalidOperationException("AI 返回结果为空");
        return result.Choices[0].Message?.Content?.Trim()
               ?? throw new InvalidOperationException("AI 返回内容为空");
    }

    private static bool IsRetryable(HttpRequestException ex)
        => ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests
        || (ex.StatusCode.HasValue && (int)ex.StatusCode.Value >= 500);

    // ===== 响应模型 =====

    private class AgnesResponse
    {
        [JsonPropertyName("choices")]
        public AgnesChoice[]? Choices { get; set; }
    }

    private class AgnesChoice
    {
        [JsonPropertyName("message")]
        public AgnesMessage? Message { get; set; }

        [JsonPropertyName("delta")]
        public AgnesMessage? Delta { get; set; }
    }

    private class AgnesMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class StreamChunk
    {
        [JsonPropertyName("choices")]
        public AgnesChoice[]? Choices { get; set; }
    }
}
