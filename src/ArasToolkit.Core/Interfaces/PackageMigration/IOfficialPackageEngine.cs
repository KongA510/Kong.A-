using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public interface IOfficialPackageEngine
{
    Task<PackageEngineInfo> ProbeAsync(string directory, CancellationToken cancellationToken = default);
    Task ExportAsync(PackageEngineExportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
    Task ImportAsync(PackageEngineImportRequest request, IProgress<PackageMigrationProgress>? progress = null, CancellationToken cancellationToken = default);
}
