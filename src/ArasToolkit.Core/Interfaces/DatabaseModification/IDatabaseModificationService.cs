using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Excel 驱动的参数化 SQL 批量修改服务。
/// </summary>
public interface IDatabaseModificationService
{
    Task<List<string>> GetSheetNamesAsync(string filePath);
    Task<DatabaseModificationPreview> PreviewExcelAsync(
        DatabaseModificationRequest request,
        int previewRowCount = 20,
        CancellationToken cancellationToken = default);
    Task<DatabaseModificationPreview> PreviewSqlAsync(
        DatabaseModificationRequest request,
        int previewRowCount = 20,
        CancellationToken cancellationToken = default);
    Task<DatabaseModificationResult> ExecuteAsync(
        DatabaseModificationRequest request,
        string connectionString,
        IProgress<DatabaseModificationProgress>? progress = null,
        CancellationToken cancellationToken = default);

    Task<List<SqlTemplate>> GetSavedTemplatesAsync(string? userId = null);
    Task SaveTemplateAsync(SqlTemplate template);
    Task DeleteTemplateAsync(string id);
}
