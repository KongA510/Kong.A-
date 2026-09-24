using System.Text.Json.Serialization;
using ArasToolkit.Core.Extensions;

namespace ArasToolkit.Core.Models;

public sealed class MigrationEndpoint
{
    public string Id { get; set; } = "";
    public string Url { get; set; } = "";
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Version { get; set; } = "";
    public string DisplayName => $"{Database} · {Username} · {Url}";
    public string Identity => Url.Trim().TrimEnd('/').ToUpperInvariant() + "|" + Database.Trim().ToUpperInvariant();
}

/// <summary>Only used in memory when opening a connection. Never persist this object.</summary>
public sealed class MigrationCredential
{
    public MigrationEndpoint Endpoint { get; set; } = new();
    [JsonIgnore] public string Md5Password { get; set; } = "";
    public override string ToString() => Endpoint.DisplayName;
}

public sealed class PackageItemSelection : ObservableObject
{
    private bool _isSelected;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    public string Type { get; set; } = "";
    public string Id { get; set; } = "";
    public string ConfigId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string Package { get; set; } = "";
    public bool IsVersionable { get; set; }
    public string ExportId => IsVersionable && !string.IsNullOrEmpty(ConfigId) ? ConfigId : Id;
    public string Key => Type + ":" + ExportId.ToUpperInvariant();
    public string DisplayName => string.IsNullOrEmpty(Label) ? $"{Type} · {Name}" : $"{Type} · {Name} — {Label}";
}

public sealed class PackageDependency
{
    public PackageItemSelection Item { get; set; } = new();
    public string ParentKey { get; set; } = "";
    public string Reason { get; set; } = "";
    public bool IsExternal { get; set; }
    public string Status { get; set; } = "自动补入";
    public string DisplayName => $"{Status}  {Item.DisplayName}";
}

public sealed class PackageIssue
{
    public string ItemKey { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsBlocking { get; set; }
    public override string ToString() => (IsBlocking ? "阻断 · " : "提示 · ") + Message;
}

public sealed class PackageAnalysis
{
    public string WorkingDirectory { get; set; } = "";
    public string EngineDirectory { get; set; } = "";
    public MigrationEndpoint Source { get; set; } = new();
    public List<PackageDependency> Items { get; set; } = [];
    public List<PackageIssue> Issues { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public bool CanExport => Items.Any(x => !x.IsExternal) && !Issues.Any(x => x.IsBlocking);
}

public sealed class PackageEngineInfo
{
    public string Directory { get; set; } = "";
    public string Version { get; set; } = "";
    public bool IsSupported { get; set; }
    public string Message { get; set; } = "";
}

public sealed class PackageFileItem
{
    public PackageItemSelection Item { get; set; } = new();
    public string PackageName { get; set; } = "";
    public string File { get; set; } = "";
    public string Aml { get; set; } = "";
    public string Key => Item.Key;
    /// <summary>Official follow-up patches belong to the root that defines their owned item.</summary>
    public string OwnerKey { get; set; } = "";
    public string SelectionKey => string.IsNullOrEmpty(OwnerKey) ? Key : OwnerKey;
}

public sealed class MigrationPackage
{
    public string RootDirectory { get; set; } = "";
    public string ManifestPath { get; set; } = "";
    public string Hash { get; set; } = "";
    public string EngineVersion { get; set; } = "";
    public MigrationEndpoint? Source { get; set; }
    public List<PackageFileItem> Items { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public List<string> RequiredPackages { get; set; } = [];
    public List<PackageIssue> Issues { get; set; } = [];
    public List<PackageDependency> Dependencies { get; set; } = [];
}

public sealed class PackageDifference : ObservableObject
{
    private bool _include = true;
    public bool Include { get => _include; set => SetProperty(ref _include, value); }
    public string ItemKey { get; set; } = "";
    public string Name { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Details { get; set; } = "";
    public string BeforeAml { get; set; } = "";
    public string TargetHash { get; set; } = "";
    public bool IsBlocking { get; set; }
    public string DisplayName => $"{Kind} · {Name}";
}

public sealed class PackageMigrationPlan
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public MigrationEndpoint Target { get; set; } = new();
    public MigrationPackage Package { get; set; } = new();
    public List<PackageDifference> Differences { get; set; } = [];
    public List<PackageIssue> Issues { get; set; } = [];
    public string TargetRelease { get; set; } = "";
    public DateTime CreatorOn { get; set; } = DateTime.Now;
    public bool CanExecute => !Issues.Any(x => x.IsBlocking) && !Differences.Any(x => x.IsBlocking)
        && Differences.Any(x => x.Include && x.Kind is "新增" or "合并");
}

public sealed class PackageMigrationProgress
{
    public string Stage { get; set; } = "";
    public string Message { get; set; } = "";
    public int Completed { get; set; }
    public int Total { get; set; }
    public double Percent => Total > 0 ? Math.Min(100, Completed * 100.0 / Total) : 0;
}

public sealed class PackageMigrationResult
{
    public string Id { get; set; } = "";
    public string Target { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Directory { get; set; } = "";
    public string Status { get; set; } = "准备";
    public string Message { get; set; } = "";
    public DateTime CreatorOn { get; set; } = DateTime.Now;
    public List<PackageItemOutcome> Items { get; set; } = [];
    public string DisplayName => $"{CreatorOn:yyyy-MM-dd HH:mm} · {Target} · {Status}";
}

public sealed class PackageItemOutcome
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "未执行";
    public string Message { get; set; } = "";
}

public sealed class PackageEngineExportRequest
{
    public string EngineDirectory { get; set; } = "";
    public string ConnectionId { get; set; } = "";
    public string OutputDirectory { get; set; } = "";
    public string PackageName { get; set; } = "";
    public List<PackageItemSelection> Items { get; set; } = [];
    public List<string> Languages { get; set; } = [];
}

public sealed class PackageEngineImportRequest
{
    public string EngineDirectory { get; set; } = "";
    public string ConnectionId { get; set; } = "";
    public string ManifestPath { get; set; } = "";
    public string TargetRelease { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Languages { get; set; } = [];
}

public sealed class PackageMigrationSettings
{
    public string EngineDirectory { get; set; } = "";
    public string SourceConnectionId { get; set; } = "";
    public string TargetConnectionId { get; set; } = "";
}
