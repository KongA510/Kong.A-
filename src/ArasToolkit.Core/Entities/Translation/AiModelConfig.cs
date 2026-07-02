using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

/// <summary>
/// AI 模型配置实体 — EF Core 映射 ai_model_config 表
/// </summary>
[Table("ai_model_config")]
public class AiModelConfig
{
    /// <summary>唯一标识（12位短GUID）</summary>
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    /// <summary>所属用户ID</summary>
    [Column("user_id")]
    [MaxLength(100)]
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>模型名称 如 "Anges AI"</summary>
    [Column("model_name")]
    [MaxLength(200)]
    [Required]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>API Key</summary>
    [Column("api_key")]
    [MaxLength(500)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>API 基础 URL</summary>
    [Column("api_base_url")]
    [MaxLength(500)]
    public string ApiBaseUrl { get; set; } = "https://apihub.agnes-ai.com/v1/chat/completions";

    /// <summary>模型标识符（如 "agnes-2.0-flash"）</summary>
    [Column("model_identifier")]
    [MaxLength(100)]
    public string ModelIdentifier { get; set; } = "agnes-2.0-flash";

    /// <summary>额外请求参数 JSON — 用户粘贴原始 JSON，合并到 AI 请求体根级（如 {"max_completion_tokens":1024,"temperature":1.0}）。为空则不追加</summary>
    [Column("extra_params")]
    public string? ExtraParams { get; set; }

    /// <summary>是否启用（同一用户只能有一个启用）</summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; }

    /// <summary>记录创建时间</summary>
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    // ===== UI 属性（不映射） =====

    /// <summary>启用状态文本</summary>
    [NotMapped]
    public string StatusText => IsEnabled ? "已启用" : "未启用";
}
