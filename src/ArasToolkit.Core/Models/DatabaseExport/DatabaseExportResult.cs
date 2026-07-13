namespace ArasToolkit.Core.Models;

public class DatabaseExportResult
{
    public int TotalRows { get; set; }
    public int BatchCount { get; set; }
    public int ExportedRows { get; set; }
    public List<string> ExportedFiles { get; set; } = new();
    public List<string> FailedDetails { get; set; } = new();
    public string LogFilePath { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public bool HasFailures => FailedDetails.Count > 0;
}
