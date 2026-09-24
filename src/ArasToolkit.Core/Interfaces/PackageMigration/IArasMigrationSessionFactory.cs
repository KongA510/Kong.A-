using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public interface IArasMigrationSessionFactory
{
    Task<List<MigrationEndpoint>> GetConnectionsAsync(CancellationToken cancellationToken = default);
    Task<MigrationCredential> GetCredentialAsync(string connectionId, CancellationToken cancellationToken = default);
    Task<IArasMigrationSession> OpenAsync(string connectionId, CancellationToken cancellationToken = default);
}

/// <summary>Read-only metadata session; separate from the application's active connection.</summary>
public interface IArasMigrationSession : IAsyncDisposable
{
    MigrationEndpoint Endpoint { get; }
    Task<string> QueryAsync(string aml, CancellationToken cancellationToken = default);
}
