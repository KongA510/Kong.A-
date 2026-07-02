using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// AI 统一调度层 — 对所有 OpenAI 兼容服务提供统一调用入口。
/// 调用方只需提供 prompt，调度器自动处理配置读取、请求构造、HTTP 通信、重试、日志。
/// </summary>
public interface IAiDispatcherService
{
    /// <summary>
    /// 发送聊天请求（非流式），返回完整响应文本。
    /// </summary>
    /// <param name="prompt">用户提示词</param>
    /// <param name="options">可选聊天参数（均可为 null，不传时走默认最小协议）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<string> ChatAsync(
        string prompt,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送聊天请求（流式），逐块回调响应文本。
    /// </summary>
    /// <param name="prompt">用户提示词</param>
    /// <param name="onChunk">每收到一块文本即回调</param>
    /// <param name="options">可选聊天参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ChatStreamAsync(
        string prompt,
        Action<string> onChunk,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前启用的模型配置（用于 UI 展示）
    /// </summary>
    Task<AiModelConfig?> GetCurrentModelAsync();
}

/// <summary>
/// 可选聊天参数 — 所有字段均可 null，null 时使用 AI 服务默认值。
/// </summary>
public class ChatOptions
{
    /// <summary>系统角色提示词</summary>
    public string? SystemMessage { get; set; }

    /// <summary>温度 0~2，控制随机性</summary>
    public double? Temperature { get; set; }

    /// <summary>最大输出 token 数</summary>
    public int? MaxTokens { get; set; }

    /// <summary>核采样 0~1</summary>
    public double? TopP { get; set; }

    /// <summary>最大完成 token 数（小米/部分服务用此参数名）</summary>
    public int? MaxCompletionTokens { get; set; }

    /// <summary>是否流式输出</summary>
    public bool? Stream { get; set; }

    /// <summary>频率惩罚 -2.0~2.0</summary>
    public double? FrequencyPenalty { get; set; }

    /// <summary>存在惩罚 -2.0~2.0</summary>
    public double? PresencePenalty { get; set; }
}
