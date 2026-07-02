using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// AI 模型配置管理服务 — 独立于任何业务功能，纯 CRUD + 互斥启用。
/// </summary>
public interface IAiModelConfigService
{
    /// <summary>获取指定用户所有模型配置</summary>
    Task<List<AiModelConfig>> GetAllAsync(string? userId = null);

    /// <summary>获取当前启用的模型（同一用户只能启用一个）</summary>
    Task<AiModelConfig?> GetEnabledAsync(string? userId = null);

    /// <summary>新增或更新模型配置</summary>
    Task SaveAsync(AiModelConfig config);

    /// <summary>启用指定模型（同时禁用该用户其他模型）</summary>
    Task EnableAsync(string id, string userId);

    /// <summary>删除模型配置</summary>
    Task DeleteAsync(string id);
}
