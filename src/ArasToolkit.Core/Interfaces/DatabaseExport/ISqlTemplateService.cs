using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

public interface ISqlTemplateService
{
    Task<List<SqlTemplate>> GetAllAsync(string? userId = null);
    Task<SqlTemplate?> GetByIdAsync(string id);
    Task SaveAsync(SqlTemplate template);
    Task DeleteAsync(string id);
}
