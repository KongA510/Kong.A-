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
using ArasToolkit.Core.Models;
using ArasToolkit.Core.Models.Translation;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 文本翻译服务 — 通过 AI 实现 Excel 批量翻译
/// 数据持久化: EF Core (text_translation_record, ai_model_config)
/// </summary>
public class TextTranslationService : ITextTranslationService
{
    private readonly IConfigService _configService;
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
    private const string HistoryDir = "Config/TextTranslations";

    /// <summary>
    /// 获取按日期隔离的输出目录路径（格式: Config/TextTranslations/2026_7_1/）
    /// </summary>
    private static string GetOutputDir()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dateFolder = DateTime.Now.ToString("yyyy_M_d");
        var dir = Path.Combine(baseDir, HistoryDir, dateFolder);
        Directory.CreateDirectory(dir);
        return dir;
    }

    public TextTranslationService(IConfigService configService,
        IDbContextFactory<ArasToolkitDbContext> dbFactory)
    {
        _configService = configService;
        _dbFactory = dbFactory;
    }

    // ========== API Key 管理（兼容旧逻辑，从配置读取） ==========

    public async Task<string?> GetApiKeyAsync()
    {
        // 优先从启用的 AI 模型配置读取
        var enabledModel = await GetEnabledAiModelAsync();
        if (enabledModel != null && !string.IsNullOrWhiteSpace(enabledModel.ApiKey))
            return enabledModel.ApiKey;

        // 回退到旧版 AppSettings 中的 ApiKey
        return await _configService.LoadAppSettingAsync<string>("ApiKey");
    }

    public async Task SaveApiKeyAsync(string apiKey)
        => await _configService.SaveAppSettingAsync("ApiKey", apiKey);

    // ========== Excel 读取 ==========

    public Task<List<Dictionary<int, string>>> ReadSourceRowsAsync(string filePath, int maxRows = 100)
    {
        return Task.Run(() =>
        {
            var rows = new List<Dictionary<int, string>>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[0];

            int row = 2;
            while (row <= ws.Dimension.End.Row && rows.Count < maxRows)
            {
                var dict = new Dictionary<int, string>();
                for (int col = 1; col <= 5; col++)
                {
                    var val = ws.Cells[row, col].Text?.Trim();
                    if (!string.IsNullOrWhiteSpace(val))
                        dict[col] = val;
                }
                if (dict.Count > 0)
                    rows.Add(dict);
                row++;
            }
            return rows;
        });
    }

    // ========== 「源文本自定义翻译」模式：Sheet / 列 / 预览 ==========

    /// <summary>1-based 列序号 → Excel 列字母（1→A, 27→AA）</summary>
    private static string GetColumnLetter(int column)
    {
        var sb = new StringBuilder();
        while (column > 0)
        {
            column--;
            sb.Insert(0, (char)('A' + column % 26));
            column /= 26;
        }
        return sb.ToString();
    }

    public Task<List<string>> GetSheetNamesAsync(string filePath)
    {
        return Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            return package.Workbook.Worksheets
                .Where(w => !string.IsNullOrWhiteSpace(w.Name))
                .Select(w => w.Name)
                .ToList();
        });
    }

    public Task<List<ExcelColumnInfo>> GetSheetColumnsAsync(string filePath, string sheetName)
    {
        return Task.Run(() =>
        {
            var list = new List<ExcelColumnInfo>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[sheetName]
                     ?? throw new InvalidOperationException($"Sheet \"{sheetName}\" 不存在");

            int endCol = ws.Dimension?.End.Column ?? 0;
            if (endCol < 50) endCol = 50; // 至少列出 50 列，方便用户选无表头列
            for (int col = 1; col <= endCol; col++)
            {
                var header = ws.Cells[1, col].Text?.Trim();
                var letter = GetColumnLetter(col);
                list.Add(new ExcelColumnInfo
                {
                    Index = col,
                    Label = string.IsNullOrWhiteSpace(header) ? letter : $"{letter} - {header}"
                });
            }
            return list;
        });
    }

    public Task<List<string>> ReadColumnTextAsync(string filePath, string sheetName, int columnIndex)
    {
        return Task.Run(() =>
        {
            var texts = new List<string>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[sheetName]
                     ?? throw new InvalidOperationException($"Sheet \"{sheetName}\" 不存在");

            int endRow = ws.Dimension?.End.Row ?? 1;
            for (int row = 2; row <= endRow; row++)
            {
                var val = ws.Cells[row, columnIndex].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(val))
                    texts.Add(val);
            }
            return texts;
        });
    }

    /// <summary>
    /// 「源文本自定义翻译」模式 — 读取指定 Sheet 数据列文本，AI 翻译后写入翻译列，
    /// 复制原 Sheet 全部内容到输出文件并在翻译列填入译文（原文件不动）。
    /// </summary>
    public async Task<TextTranslationRecord> TranslateSourceCustomAsync(
        string filePath,
        string sheetName,
        int sourceColumnIndex,
        int targetColumnIndex,
        string targetLanguage,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var sourceFileName = Path.GetFileNameWithoutExtension(filePath);
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputFileName = $"{sourceFileName}_{timestamp}_translated.xlsx";
        var outputDir = GetOutputDir();
        var outputPath = Path.Combine(outputDir, outputFileName);

        // 1. 读取数据列（保留行号映射，跳过空单元格）
        progress?.Report(new TranslationProgressInfo
        {
            Phase = "读取源文件",
            ItemName = $"{sheetName} → {GetColumnLetter(sourceColumnIndex)} 列"
        });

        var sourceItems = await Task.Run(() =>
        {
            var items = new List<(int Row, string Text)>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[sheetName]
                     ?? throw new InvalidOperationException($"Sheet \"{sheetName}\" 不存在");
            int endRow = ws.Dimension?.End.Row ?? 1;
            for (int row = 2; row <= endRow; row++)
            {
                var val = ws.Cells[row, sourceColumnIndex].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(val))
                    items.Add((row, val));
            }
            return items;
        });

        if (sourceItems.Count == 0)
            throw new InvalidOperationException("所选数据列中没有可翻译的文本");

        // 2. 获取启用的 AI 模型
        var aiModel = await GetEnabledAiModelAsync();
        var apiKey = aiModel?.ApiKey ?? await GetApiKeyAsync();
        var apiUrl = aiModel?.ApiBaseUrl ?? "https://apihub.agnes-ai.com/v1/chat/completions";
        var modelId = aiModel?.ModelIdentifier ?? "agnes-2.0-flash";
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("请先在设置中配置 AI API Key");

        // 3. 分批翻译（序号化输入/输出，按序号回填，避免错位）
        const int batchSize = 100;
        var translations = new Dictionary<int, string>(); // rowIndex → 译文
        int totalBatches = (sourceItems.Count + batchSize - 1) / batchSize;
        int processed = 0;

        for (int b = 0; b < totalBatches; b++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var batch = sourceItems.Skip(b * batchSize).Take(batchSize).ToList();
            progress?.Report(new TranslationProgressInfo
            {
                Phase = "翻译中",
                Current = b + 1,
                PhaseTotal = totalBatches,
                OverallCurrent = processed,
                OverallTotal = sourceItems.Count,
                ItemName = $"第 {b + 1}/{totalBatches} 批 ({batch.Count} 条)"
            });

            var prompt = BuildSourceCustomPrompt(batch, targetLanguage);
            var responseText = await CallAIAsync(apiUrl, apiKey, modelId, prompt);
            var parsed = ParseNumberedTranslations(responseText);

            for (int i = 0; i < batch.Count; i++)
            {
                var (row, _) = batch[i];
                translations[row] = i < parsed.Count ? parsed[i] : "";
            }
            processed += batch.Count;
        }

        // 4. 复制原 Sheet 到输出文件，把译文写入翻译列
        progress?.Report(new TranslationProgressInfo
        {
            Phase = "生成输出文件",
            OverallCurrent = sourceItems.Count,
            OverallTotal = sourceItems.Count,
            ItemName = outputFileName
        });

        await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets[sheetName]
                     ?? throw new InvalidOperationException($"Sheet \"{sheetName}\" 不存在");

            // 翻译列表头：若为空则补一个标题
            if (string.IsNullOrWhiteSpace(ws.Cells[1, targetColumnIndex].Text))
                ws.Cells[1, targetColumnIndex].Value = $"翻译({targetLanguage})";
            ws.Cells[1, targetColumnIndex].Style.Font.Bold = true;

            foreach (var (row, translated) in translations)
                ws.Cells[row, targetColumnIndex].Value = translated;

            ws.Cells[ws.Dimension.Start.Row, ws.Dimension.Start.Column,
                     ws.Dimension.End.Row, ws.Dimension.End.Column].AutoFitColumns(8, 50);
            package.SaveAs(new FileInfo(outputPath));
        });

        // 5. 保存历史记录
        var record = new TextTranslationRecord
        {
            SourceFileName = Path.GetFileName(filePath),
            OutputFileName = outputFileName,
            OutputFilePath = outputPath,
            TemplateType = "源文本自定义翻译",
            SourceLanguage = $"{sheetName}: {GetColumnLetter(sourceColumnIndex)}→{GetColumnLetter(targetColumnIndex)}",
            SourceRowCount = sourceItems.Count,
            BatchCount = totalBatches,
            UserId = CurrentUserContext.CurrentUserId,
            AiModelId = aiModel?.Id,
            CreatorOn = DateTime.Now
        };
        await SaveRecordAsync(record);

        progress?.Report(new TranslationProgressInfo
        {
            Phase = "完成",
            OverallCurrent = sourceItems.Count,
            OverallTotal = sourceItems.Count,
            ItemName = outputFileName
        });
        return record;
    }

    /// <summary>构造「源文本自定义翻译」序号化 prompt</summary>
    private static string BuildSourceCustomPrompt(List<(int Row, string Text)> batch, string targetLanguage)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"请将以下文本翻译为{targetLanguage}。");
        sb.AppendLine("严格要求输出格式：每行一条，格式为 \"序号. 译文\"，序号与输入一致。");
        sb.AppendLine("不要输出任何解释、标题或额外文字，只输出翻译结果。");
        sb.AppendLine("原文：");
        for (int i = 0; i < batch.Count; i++)
            sb.AppendLine($"{i + 1}. {batch[i].Text}");
        return sb.ToString();
    }

    /// <summary>解析序号化翻译结果，按序号升序返回译文列表（1-based 序号 → 索引）</summary>
    private static List<string> ParseNumberedTranslations(string responseText)
    {
        var dict = new Dictionary<int, string>();
        var lines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var dotIndex = line.IndexOf(". ");
            if (dotIndex <= 0 || dotIndex > 6) continue;
            var numPart = line[..dotIndex];
            if (!int.TryParse(numPart, out var num)) continue;
            var text = line[(dotIndex + 2)..].Trim();
            dict[num] = text;
        }
        return dict.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
    }

    // ========== AI 翻译 ==========

    public async Task<TextTranslationRecord> TranslateAsync(
        string filePath,
        string templateType,
        string? sourceLanguage = null,
        string? customPrompt = null,
        IProgress<TranslationProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var sourceFileName = Path.GetFileNameWithoutExtension(filePath);
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputFileName = $"{sourceFileName}_{timestamp}_translated.xlsx";

        var outputDir = GetOutputDir();
        var outputPath = Path.Combine(outputDir, outputFileName);

        // 1. 读取源数据
        progress?.Report(new TranslationProgressInfo
        {
            Phase = "读取源文件",
            ItemName = Path.GetFileName(filePath)
        });
        var allRows = await ReadSourceRowsAsync(filePath, maxRows: int.MaxValue);
        if (allRows.Count == 0)
            throw new InvalidOperationException("源文件中没有可翻译的数据");

        const int batchSize = 100;
        var batches = SplitIntoBatches(allRows, batchSize);
        progress?.Report(new TranslationProgressInfo
        {
            Phase = "准备翻译",
            OverallTotal = allRows.Count,
            PhaseTotal = batches.Count,
            ItemName = $"共 {allRows.Count} 条，分 {batches.Count} 批"
        });

        // 2. 获取启用的 AI 模型
        var aiModel = await GetEnabledAiModelAsync();
        var apiKey = aiModel?.ApiKey ?? await GetApiKeyAsync();
        var apiUrl = aiModel?.ApiBaseUrl ?? "https://apihub.agnes-ai.com/v1/chat/completions";
        var modelId = aiModel?.ModelIdentifier ?? "agnes-2.0-flash";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("请先在设置中配置 AI API Key");

        // 3. 分批串行翻译
        var allResults = new List<string[]>();
        int processedCount = 0;
        for (int i = 0; i < batches.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = batches[i];
            var batchLabel = $"第 {i + 1}/{batches.Count} 批 ({batch.Count} 条)";
            progress?.Report(new TranslationProgressInfo
            {
                Phase = "翻译中",
                Current = i + 1,
                PhaseTotal = batches.Count,
                OverallCurrent = processedCount,
                OverallTotal = allRows.Count,
                ItemName = batchLabel
            });

            var prompt = BuildPrompt(batch, templateType, sourceLanguage, customPrompt);
            var responseText = await CallAIAsync(apiUrl, apiKey, modelId, prompt);
            var parsedRows = ParseTranslationResult(responseText, batch.Count, templateType);
            allResults.AddRange(parsedRows);
            processedCount += batch.Count;
        }

        // 4. 写入输出 Excel
        progress?.Report(new TranslationProgressInfo
        {
            Phase = "生成输出文件",
            OverallCurrent = allRows.Count,
            OverallTotal = allRows.Count,
            ItemName = outputFileName
        });
        await WriteOutputExcelAsync(outputPath, allRows, allResults, templateType, sourceLanguage);

        // 5. 保存到数据库
        var record = new TextTranslationRecord
        {
            SourceFileName = Path.GetFileName(filePath),
            OutputFileName = outputFileName,
            OutputFilePath = outputPath,
            TemplateType = templateType,
            SourceLanguage = sourceLanguage,
            SourceRowCount = allRows.Count,
            BatchCount = batches.Count,
            UserId = CurrentUserContext.CurrentUserId,
            AiModelId = aiModel?.Id,
            CreatorOn = DateTime.Now
        };
        await SaveRecordAsync(record);

        progress?.Report(new TranslationProgressInfo
        {
            Phase = "完成",
            OverallCurrent = allRows.Count,
            OverallTotal = allRows.Count,
            ItemName = outputFileName
        });
        return record;
    }

    // ========== 历史记录（EF Core + 分页） ==========

    public async Task<(List<TextTranslationRecord> Items, int TotalCount)> GetHistoryAsync(string? userId = null, int page = 1, int pageSize = 20)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.TextTranslationRecords.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(r => r.UserId == userId);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatorOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<TextTranslationRecord?> GetHistoryByIdAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TextTranslationRecords.FindAsync(id);
    }

    // ========== AI 模型配置管理（EF Core） ==========

    public async Task<List<AiModelConfig>> GetAiModelsAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.AiModelConfigs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(m => m.UserId == userId);
        return await query.OrderByDescending(m => m.CreatorOn).ToListAsync();
    }

    public async Task SaveAiModelAsync(AiModelConfig config)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.AiModelConfigs.FindAsync(config.Id);
        if (existing != null)
        {
            existing.ModelName = config.ModelName;
            existing.ApiKey = config.ApiKey;
            existing.ApiBaseUrl = config.ApiBaseUrl;
            existing.ModelIdentifier = config.ModelIdentifier;
            existing.IsEnabled = config.IsEnabled;
        }
        else
        {
            db.AiModelConfigs.Add(config);
        }
        await db.SaveChangesAsync();
    }

    public async Task EnableAiModelAsync(string id, string userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        // 禁用该用户所有模型
        var all = await db.AiModelConfigs.Where(m => m.UserId == userId).ToListAsync();
        foreach (var m in all)
            m.IsEnabled = (m.Id == id);
        await db.SaveChangesAsync();
    }

    public async Task<AiModelConfig?> GetEnabledAiModelAsync(string? userId = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.AiModelConfigs
            .Where(m => m.IsEnabled)
            .OrderByDescending(m => m.CreatorOn)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAiModelAsync(string id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var model = await db.AiModelConfigs.FindAsync(id);
        if (model != null)
        {
            db.AiModelConfigs.Remove(model);
            await db.SaveChangesAsync();
        }
    }

    // ========== 私有方法 ==========

    private async Task SaveRecordAsync(TextTranslationRecord record)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.TextTranslationRecords.Add(record);
        await db.SaveChangesAsync();
    }

    private static List<List<Dictionary<int, string>>> SplitIntoBatches(List<Dictionary<int, string>> rows, int batchSize)
    {
        var batches = new List<List<Dictionary<int, string>>>();
        for (int i = 0; i < rows.Count; i += batchSize)
            batches.Add(rows.Skip(i).Take(batchSize).ToList());
        return batches;
    }

    private static string BuildPrompt(List<Dictionary<int, string>> rows, string templateType,
        string? sourceLanguage, string? customPrompt)
    {
        if (templateType == "Aras翻译")
        {
            var texts = rows.Select(r => r.TryGetValue(3, out var v) ? v : "").ToList();
            var sb = new StringBuilder();
            sb.AppendLine("请将以下简体中文翻译为繁体中文和英文。");
            sb.AppendLine("严格要求输出格式：每一行用 | 分隔：简体中文 | 繁體中文 | English");
            sb.AppendLine("不要输出任何解释、标题或额外文字，只输出翻译结果。");
            sb.AppendLine("原文：");
            for (int i = 0; i < texts.Count; i++)
                sb.AppendLine($"{i + 1}. {texts[i]}");
            return sb.ToString();
        }

        if (!string.IsNullOrWhiteSpace(customPrompt))
        {
            var sb2 = new StringBuilder();
            sb2.AppendLine(customPrompt);
            sb2.AppendLine("原文：");
            for (int i = 0; i < rows.Count; i++)
                sb2.AppendLine($"{i + 1}. {(rows[i].TryGetValue(3, out var v2) ? v2 : "")}");
            return sb2.ToString();
        }

        var lang = sourceLanguage ?? "中文";
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine($"请将以下 {lang} 文本翻译为对应的多语言版本。");
        promptBuilder.AppendLine("严格要求输出格式：每一行用 | 分隔：原文 | 翻译后");
        promptBuilder.AppendLine("不要输出任何解释、标题或额外文字，只输出翻译结果。");
        promptBuilder.AppendLine("原文：");
        for (int i = 0; i < rows.Count; i++)
            promptBuilder.AppendLine($"{i + 1}. {(rows[i].TryGetValue(3, out var v3) ? v3 : "")}");
        return promptBuilder.ToString();
    }

    private static async Task<string> CallAIAsync(string apiUrl, string apiKey, string model, string prompt)
    {
        var requestBody = new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } }
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AgnesResponse>(responseBody);

        if (result?.Choices == null || result.Choices.Length == 0)
            throw new InvalidOperationException("AI 返回结果为空");

        return result.Choices[0].Message.Content?.Trim()
               ?? throw new InvalidOperationException("AI 返回内容为空");
    }

    private static List<string[]> ParseTranslationResult(string responseText, int expectedCount, string templateType)
    {
        var lines = responseText
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Contains('|'))
            .ToList();

        var colCount = templateType == "Aras翻译" ? 3 : 2;
        var results = new List<string[]>();
        foreach (var line in lines)
        {
            var parts = line.Split('|', colCount);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
                var dotIndex = parts[i].IndexOf(". ");
                if (dotIndex > 0 && dotIndex < 5 && char.IsDigit(parts[i][0]))
                    parts[i] = parts[i][(dotIndex + 2)..];
            }
            var row = new string[colCount];
            for (int i = 0; i < colCount; i++)
                row[i] = i < parts.Length ? parts[i] : "";
            results.Add(row);
        }

        while (results.Count < expectedCount)
        {
            var empty = new string[colCount];
            Array.Fill(empty, "");
            results.Add(empty);
        }
        return results;
    }

    private static async Task WriteOutputExcelAsync(string outputPath,
        List<Dictionary<int, string>> sourceRows, List<string[]> translatedRows,
        string templateType, string? sourceLanguage)
    {
        await Task.Run(() =>
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("翻译结果");

            if (templateType == "Aras翻译")
            {
                ws.Cells[1, 1].Value = "属性名称";
                ws.Cells[1, 2].Value = "ID";
                ws.Cells[1, 3].Value = "简体中文";
                ws.Cells[1, 4].Value = "繁体中文";
                ws.Cells[1, 5].Value = "英文";
                ws.Cells[1, 1, 1, 5].Style.Font.Bold = true;

                for (int r = 0; r < sourceRows.Count; r++)
                {
                    ws.Cells[r + 2, 1].Value = sourceRows[r].TryGetValue(1, out var col1) ? col1 : "";
                    ws.Cells[r + 2, 2].Value = sourceRows[r].TryGetValue(2, out var col2) ? col2 : "";
                    ws.Cells[r + 2, 3].Value = sourceRows[r].TryGetValue(3, out var col3) ? col3 : "";
                    if (r < translatedRows.Count && translatedRows[r].Length >= 3)
                    {
                        ws.Cells[r + 2, 4].Value = translatedRows[r][1];
                        ws.Cells[r + 2, 5].Value = translatedRows[r][2];
                    }
                }
            }
            else
            {
                ws.Cells[1, 1].Value = "属性名称";
                ws.Cells[1, 2].Value = "ID";
                ws.Cells[1, 3].Value = "原标签";
                ws.Cells[1, 4].Value = "翻译后标签";
                ws.Cells[1, 1, 1, 4].Style.Font.Bold = true;

                for (int r = 0; r < sourceRows.Count; r++)
                {
                    ws.Cells[r + 2, 1].Value = sourceRows[r].TryGetValue(1, out var col1) ? col1 : "";
                    ws.Cells[r + 2, 2].Value = sourceRows[r].TryGetValue(2, out var col2) ? col2 : "";
                    ws.Cells[r + 2, 3].Value = sourceRows[r].TryGetValue(3, out var col3) ? col3 : "";
                    if (r < translatedRows.Count && translatedRows[r].Length >= 2)
                        ws.Cells[r + 2, 4].Value = translatedRows[r][1];
                }
            }

            ws.Cells[1, 1, 1, 5].AutoFitColumns(8, 30);
            package.SaveAs(new FileInfo(outputPath));
        });
    }

    // ========== API 响应模型 ==========

    private class AgnesResponse
    {
        [JsonPropertyName("choices")]
        public AgnesChoice[]? Choices { get; set; }
    }

    private class AgnesChoice
    {
        [JsonPropertyName("message")]
        public AgnesMessage? Message { get; set; }
    }

    private class AgnesMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
