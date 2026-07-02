# AI 统一调度层重构方案 · 工作流设计

> **状态**: 设计阶段，仅文档，本次不做代码修改  
> **设计日期**: 2026-07-02

---

## 一、目标

将 AI 配置管理、HTTP 调用、响应解析从 `TextTranslationService` 中拆出，形成独立的 **AI 调度层**。
后续任何新 AI 功能（翻译、摘要、分类、生成等）只需提供 `prompt` 即可调用，调度层自动处理配置读取、请求构造、HTTP 通信、错误重试、流式响应。

---

## 二、三层架构设计

```
┌─────────────────────────────────────────────────────────┐
│  所有 AI 调用方 (TextTranslationService, 未来新功能...)    │
│  └─ 只关心: "给 prompt → 拿 response 文本"               │
├─────────────────────────────────────────────────────────┤
│  IAiDispatcherService                  ← 新增接口        │
│  ├─ ChatAsync(prompt, options?) → string                 │
│  ├─ ChatStreamAsync(prompt, options?) → IAsyncEnumerable │
│  └─ (未来) EmbedAsync, ClassifyAsync...                  │
├─────────────────────────────────────────────────────────┤
│  AiDispatcherService                   ← 新增实现        │
│  ├─ 自动读取当前启用的 AiModelConfig                      │
│  ├─ 构造 OpenAI 兼容请求体                                │
│  ├─ 管理 HttpClient 生命周期                             │
│  ├─ 错误重试 / 熔断 / 超时                               │
│  └─ 统一日志记录 (IErrorLogService)                       │
├─────────────────────────────────────────────────────────┤
│  IAiModelConfigService                 ← 新增接口 (拆出) │
│  ├─ GetEnabledAsync() → AiModelConfig                    │
│  ├─ GetAllAsync() → List<AiModelConfig>                  │
│  ├─ SaveAsync(config)                                    │
│  ├─ EnableAsync(id)                                      │
│  ├─ DeleteAsync(id)                                      │
│  └─ AI 配置 CRUD，独立于任何业务功能                       │
├─────────────────────────────────────────────────────────┤
│  AiModelConfigService                  ← 新增实现 (拆出) │
│  └─ 从 ITextTranslationService 中搬出 AI 配置方法         │
├─────────────────────────────────────────────────────────┤
│  ITextTranslationService               ← 保留 (精简)    │
│  └─ 只保留翻译特有逻辑: BuildPrompt / ParseResult / Excel IO │
│     └─ 内部注入 IAiDispatcherService，翻译时只调 ChatAsync  │
└─────────────────────────────────────────────────────────┘
```

---

## 三、核心接口设计

### 3.1 IAiDispatcherService（AI 调度器）

```csharp
namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// AI 统一调度层 — 对所有 OpenAI 兼容服务提供统一调用入口。
/// 调用方只需给 prompt，调度器自动处理配置、HTTP、重试、日志。
/// </summary>
public interface IAiDispatcherService
{
    /// <summary>
    /// 发送聊天请求（非流式），返回完整响应文本。
    /// </summary>
    /// <param name="prompt">用户提示词</param>
    /// <param name="options">可选参数（temperature/max_tokens/system_message，均可为 null）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<string> ChatAsync(
        string prompt,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送聊天请求（流式），逐块返回响应文本。
    /// </summary>
    Task ChatStreamAsync(
        string prompt,
        Action<string> onChunk,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前启用的模型配置（用于 UI 展示"正在使用: xxx 模型"）
    /// </summary>
    Task<AiModelConfig?> GetCurrentModelAsync();
}

/// <summary>
/// 可选聊天参数 — 所有字段均可 null，null 时使用 AI 服务默认值
/// </summary>
public class ChatOptions
{
    public string? SystemMessage { get; set; }  // 系统角色提示
    public double? Temperature { get; set; }    // 0~2，默认由服务决定
    public int? MaxTokens { get; set; }         // 最大输出 token
    public double? TopP { get; set; }           // 核采样
}
```

**`ChatOptions` 说明**：这是**可选的扩展参数**，ALL 字段均可为 null。调用方可以不传（三参数模式，走默认），也可以按需微调。不影响「三参数即刻」的核心理念。三参数仍是主流程：

```csharp
// 翻译调用（三参数，无需 ChatOptions） — 是最常见的用法
var result = await _aiDispatcher.ChatAsync(translationPrompt);
```

### 3.2 IAiModelConfigService（AI 模型配置管理）

```csharp
namespace ArasToolkit.Core.Interfaces;

public interface IAiModelConfigService
{
    Task<List<AiModelConfig>> GetAllAsync(string? userId = null);
    Task<AiModelConfig?> GetEnabledAsync(string? userId = null);
    Task SaveAsync(AiModelConfig config);
    Task EnableAsync(string id, string userId);
    Task DeleteAsync(string id);
}
```

这个就是从 `ITextTranslationService` 中搬出 5 个 AI 配置方法，一字不改，只是搬家。

### 3.3 ITextTranslationService（精简后）

```csharp
// 移除所有 AI 配置管理方法（搬去 IAiModelConfigService）
// 移除 CallAIAsync（搬去 AiDispatcherService）
// TranslateAsync 内部改为调用 _aiDispatcher.ChatAsync(prompt)
public interface ITextTranslationService
{
    // ===== 翻译业务（保留） =====
    Task<TextTranslationRecord> TranslateAsync(...);           // 内部调用 IAiDispatcher
    Task<TextTranslationRecord> TranslateSourceCustomAsync(...);
    Task<List<Dictionary<int, string>>> ReadSourceRowsAsync(...);
    Task<List<string>> GetSheetNamesAsync(...);
    Task<List<ExcelColumnInfo>> GetSheetColumnsAsync(...);
    Task<List<string>> ReadColumnTextAsync(...);
    Task<(List<TextTranslationRecord>, int)> GetHistoryAsync(...);
    Task<TextTranslationRecord?> GetHistoryByIdAsync(string id);

    // ===== API Key 兼容（保留，作为简单读写） =====
    Task<string?> GetApiKeyAsync();
    Task SaveApiKeyAsync(string apiKey);
}
```

---

## 四、CallAIAsync → ChatAsync 请求体演进

**现状（极简）**：
```json
{
  "model": "xxx",
  "messages": [{ "role": "user", "content": "prompt" }]
}
```

**新版 ChatAsync 构造的请求体**：
```json
{
  "model": "xxx",
  "messages": [
    { "role": "system", "content": "你是一个翻译助手" },  // 仅当 ChatOptions.SystemMessage 非空时
    { "role": "user", "content": "prompt" }
  ],
  "temperature": 0.3,       // 仅当 ChatOptions.Temperature 非空时
  "max_tokens": 4096,       // 仅当 ChatOptions.MaxTokens 非空时
  "top_p": 1.0              // 仅当 ChatOptions.TopP 非空时
}
```

**关键是**：当 `ChatOptions` 为 null 或不传时，生成的 JSON 与现在**完全一致**（只有 model + messages），保证三参数兼容。任何 OpenAI 兼容服务都接受无 extra 字段的请求。

---

## 五、HTTP 调用增强点（低频但重要的改进）

| 改进项 | 说明 |
|--------|------|
| **配置缓存** | `GetEnabledAsync()` 结果缓存 60s，避免每次翻译重复查 DB |
| **重试机制** | HTTP 429/5xx 自动重试最多 3 次，间隔 1s/2s/4s 指数退避 |
| **超时可配** | 从硬编码 5 分钟改为 `ChatOptions.TimeoutSeconds`（默认 300） |
| **Response 反序列化** | `AgnesResponse` 私有类保持不动，完全兼容 OpenAI 响应格式 |
| **错误日志** | 所有 HTTP 异常通过 `IErrorLogService` 记录（当前缺失） |

---

## 六、改动文件清单（预估）

| 文件 | 操作 | 说明 |
|------|------|------|
| `Core/Interfaces/Ai/IAiDispatcherService.cs` | **新建** | 调度器接口 + ChatOptions 类 |
| `Core/Interfaces/Ai/IAiModelConfigService.cs` | **新建** | AI 配置 CRUD 接口 |
| `Services/Services/Ai/AiDispatcherService.cs` | **新建** | 调度器实现 (~120行) |
| `Services/Services/Ai/AiModelConfigService.cs` | **新建** | AI 配置实现 (~60行，从 TextTranslationService 搬) |
| `Core/Interfaces/Translation/ITextTranslationService.cs` | **修改** | 移除 5 个 AI 配置方法签名 |
| `Services/Services/Translation/TextTranslationService.cs` | **修改** | 注入 IAiDispatcherService，替换 CallAIAsync 调用；移除 AI 配置方法 |
| `Services/ServiceCollectionExtensions.cs` | **修改** | 注册 3 个新服务 |
| `App/App.xaml.cs` | **修改** | 若有 AI 配置 ViewModel 使用 ITextTranslationService 做配置管理，需改为注入 IAiModelConfigService |
| `App/ViewModels/Translation/TranslationApiKeyViewModel.cs` | **修改** | AI 配置管理 VM 改为依赖 IAiModelConfigService |
| `App/Views/Translation/TranslationApiKeyWindow.xaml` | **不修改** | 无需动 View |

**总计：6 新建 + 6 修改，0 删除。**

---

## 七、未来扩展蓝图

重构完成后，新增任何 AI 功能只需：

```csharp
// 例：未来要加一个"AI 代码审查"功能
public class CodeReviewService : ICodeReviewService
{
    private readonly IAiDispatcherService _ai;

    public async Task<string> ReviewAsync(string code)
    {
        var prompt = $"请审查以下代码：\n{code}";
        return await _ai.ChatAsync(prompt);  // 一行搞定，三参数兼容
    }
}
```

无需重复写：获取配置 → 构造 JSON → HttpClient → 解析响应 → 错误处理 → 日志记录。

---

## 八、不变量（确保三参数核心不被破坏）

1. **`AiModelConfig` 实体字段不动** — ApiKey + ApiBaseUrl + ModelIdentifier 三参数结构不变
2. **`CallAIAsync` 的最小请求体格式不动** — model + messages 就是全部必需字段
3. **`ChatOptions` 全部 nullable** — 不传时光骨文与现在一模一样
4. **兼容所有现有翻译功能** — 三种翻译模式（Aras/自定义/源文本自定义）零逻辑变化，只是内部调用路径从 `CallAIAsync` 变为 `_aiDispatcher.ChatAsync`
