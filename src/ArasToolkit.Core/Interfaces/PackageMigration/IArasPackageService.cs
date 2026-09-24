using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public interface IArasPackageService
{
    IReadOnlyList<string> SelectableTypes { get; }
    Task<List<PackageItemSelection>> SearchAsync(string connectionId, string type, string keyword, CancellationToken cancellationToken = default);
    Task<PackageAnalysis> AnalyzeAsync(string connectionId, IReadOnlyList<PackageItemSelection> selected, string engineDirectory, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<MigrationPackage> ExportAsync(PackageAnalysis analysis, string engineDirectory, string destination, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<MigrationPackage> ReadPackageAsync(string path, CancellationToken cancellationToken = default);
    Task<PackageMigrationPlan> PreflightAsync(MigrationPackage package, string targetConnectionId, string engineDirectory, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<PackageMigrationResult> ExecuteAsync(PackageMigrationPlan plan, string engineDirectory, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
    Task<List<PackageMigrationResult>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task<PackageMigrationSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(PackageMigrationSettings settings);
}
