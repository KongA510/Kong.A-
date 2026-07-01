using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// 文件浏览器服务接口 — 文件系统操作
/// </summary>
public interface IFileExplorerService
{
    /// <summary>获取指定路径下单层目录内容（目录在前，文件在后）</summary>
    Task<List<FileSystemItem>> GetDirectoryContentsAsync(string path);

    /// <summary>递归模糊搜索当前目录及子目录中匹配的文件/文件夹</summary>
    Task<List<FileSystemItem>> SearchAsync(string rootPath, string term, CancellationToken ct = default);

    /// <summary>用系统默认程序打开文件</summary>
    void OpenFile(string filePath);

    /// <summary>用资源管理器打开文件夹或定位文件所在位置</summary>
    void OpenInExplorer(string path);

    /// <summary>将指定文件复制到目标目录</summary>
    Task CopyFileAsync(string sourceFilePath, string targetDirectory);

    /// <summary>检查路径是否为有效目录</summary>
    bool DirectoryExists(string path);

    /// <summary>获取父目录路径（根目录返回 null）</summary>
    string? GetParentPath(string path);
}
