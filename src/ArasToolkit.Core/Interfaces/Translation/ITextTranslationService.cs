using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;
using ArasToolkit.Core.Models.Translation;

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

    /// <summary>获取 Excel 文件所有 Sheet 名称（用于「源文本自定义翻译」模式）</summary>
    Task<List<string>> GetSheetNamesAsync(string filePath);

    /// <summary>获取指定 Sheet 的列信息（表头+列字母），用于列选择下拉</summary>
    Task<List<ExcelColumnInfo>> GetSheetColumnsAsync(string filePath, string sheetName);

    /// <summary>读取指定 Sheet 指定列的全部文本（用于源文本预览/复制）</summary>
    Task<List<string>> ReadColumnTextAsync(string filePath, string sheetName, int columnIndex);

    /// <summary>
    /// 「源文本自定义翻译」模式 — 读取指定 Sheet 的数据列，AI 翻译后写入翻译列，
    /// 复制原 Sheet 全部内容到输出文件并在翻译列填入译文。
    /// </summary>
    Task<TextTranslationRecord> TranslateSourceCustomAsync(
        string filePath,
        string sheetName,
        int sourceColumnIndex,
        int targetColumnIndex,
        string targetLanguage,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

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
}
