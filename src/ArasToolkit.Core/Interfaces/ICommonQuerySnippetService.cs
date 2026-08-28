using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>常用 SQL/AML/XML 片段存储服务。</summary>
public interface ICommonQuerySnippetService
{
    Task<List<CommonQuerySnippet>> GetAllAsync(string? contentType = null, string? keyword = null);
    Task SaveAsync(CommonQuerySnippet snippet);
    Task DeleteAsync(string id);
}
