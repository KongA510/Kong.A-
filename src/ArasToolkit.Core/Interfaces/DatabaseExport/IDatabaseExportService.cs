using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public interface IDatabaseExportService
{
    Task<DatabaseExportResult> ExportAsync(
        string outputDir,
        string exportMode,
        int batchSize,
        string sqlQuery,
        string connectionString,
        IProgress<ImportProgressInfo>? progress = null,
        CancellationToken cancellationToken = default);

    Task<(List<DatabaseExportLog> Items, int TotalCount)> GetHistoryAsync(
        string? userId = null, int page = 1, int pageSize = 20);

    Task<DatabaseExportLog?> GetLogByIdAsync(string id);
}
