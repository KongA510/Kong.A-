using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 文本翻译服务接口 — AI 驱动的 Excel 批量翻译
/// </summary>
public interface ITextTranslationService
{
    /// <summary>获取已保存的 API Key</summary>
    Task<string?> GetApiKeyAsync();

    /// <summary>保存 API Key</summary>
    Task SaveApiKeyAsync(string apiKey);

    /// <summary>读取源文件指定列数据</summary>
    Task<List<Dictionary<int, string>>> ReadSourceRowsAsync(string filePath, int maxRows = 100);

    /// <summary>
    /// 执行翻译 — 分批串行调用 AI，合并结果写入输出文件
    /// </summary>
    Task<TextTranslationRecord> TranslateAsync(
        string filePath,
        string templateType,
        string? sourceLanguage = null,
        string? customPrompt = null,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>获取历史翻译记录列表（分页）</summary>
    Task<(List<TextTranslationRecord> Items, int TotalCount)> GetHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);

    /// <summary>按 ID 获取单条历史记录</summary>
    Task<TextTranslationRecord?> GetHistoryByIdAsync(string id);

    /// <summary>获取所有 AI 模型配置</summary>
    Task<List<AiModelConfig>> GetAiModelsAsync(string? userId = null);

    /// <summary>保存 AI 模型配置</summary>
    Task SaveAiModelAsync(AiModelConfig config);

    /// <summary>启用指定 AI 模型（禁用其他的）</summary>
    Task EnableAiModelAsync(string id, string userId);

    /// <summary>获取当前启用的 AI 模型</summary>
    Task<AiModelConfig?> GetEnabledAiModelAsync(string? userId = null);

    /// <summary>删除 AI 模型配置</summary>
    Task DeleteAiModelAsync(string id);
}
