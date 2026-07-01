using System;
using System.IO;

namespace ArasToolkit.Core.Models;

/// <summary>
/// 文件系统条目 — 表示文件或文件夹
/// </summary>
public class FileSystemItem
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }

    public string Icon => IsDirectory ? "📁" : GetFileIcon(Name);

    /// <summary>WPF 兼容颜色十六进制字符串（#RRGGBB），用于图标着色</summary>
    public string IconColor => IsDirectory ? "#D4A017" : GetFileColor(Name);

    public string SizeDisplay => IsDirectory ? "" : FormatSize(Size);

    public string LastModifiedDisplay => LastModified.ToString("yyyy-MM-dd HH:mm");

    private static string GetFileIcon(string fileName)
    {
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? "";

        return ext switch
        {
            // 图片
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".svg" or ".webp" or ".ico" => "🖼️",
            // 音视频
            ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".wma" => "🎵",
            ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" or ".webm" => "🎬",
            // 文档
            ".doc" or ".docx" => "📝",
            ".xls" or ".xlsx" or ".csv" => "📊",
            ".ppt" or ".pptx" => "📽️",
            ".pdf" => "📕",
            ".txt" or ".md" or ".log" => "📄",
            // 代码
            ".cs" or ".java" or ".py" or ".js" or ".ts" or ".go" or ".rs" or ".cpp" or ".c" or ".h" => "💻",
            ".xaml" or ".xml" or ".json" or ".yaml" or ".yml" or ".config" or ".csproj" => "⚙️",
            ".sql" => "🗄️",
            // 压缩包
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" or ".bz2" => "📦",
            // 可执行
            ".exe" or ".dll" or ".msi" or ".bat" or ".cmd" or ".ps1" => "⚡",
            // 默认
            _ => "📄"
        };
    }

    private static string GetFileColor(string fileName)
    {
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? "";

        return ext switch
        {
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".svg" or ".webp" or ".ico" => "#10B981",  // 绿
            ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".wma" => "#F59E0B",                       // 琥珀
            ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" or ".webm" => "#EF4444",             // 红
            ".doc" or ".docx" => "#3B82F6",                                                               // 蓝
            ".xls" or ".xlsx" or ".csv" => "#10B981",                                                      // 绿
            ".ppt" or ".pptx" => "#F97316",                                                                // 橙
            ".pdf" => "#EF4444",                                                                           // 红
            ".txt" or ".md" or ".log" => "#6B7280",                                                        // 灰
            ".cs" or ".java" or ".py" or ".js" or ".ts" or ".go" or ".rs" or ".cpp" or ".c" or ".h" => "#8B5CF6", // 紫
            ".xaml" or ".xml" or ".json" or ".yaml" or ".yml" or ".config" or ".csproj" => "#6366F1",     // 靛蓝
            ".sql" => "#06B6D4",                                                                           // 青
            ".zip" or ".rar" or ".7z" or ".tar" or ".gz" or ".bz2" => "#78716C",                          // 棕
            ".exe" or ".dll" or ".msi" or ".bat" or ".cmd" or ".ps1" => "#DC2626",                        // 深红
            _ => "#9CA3AF"                                                                                 // 浅灰
        };
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }
}
