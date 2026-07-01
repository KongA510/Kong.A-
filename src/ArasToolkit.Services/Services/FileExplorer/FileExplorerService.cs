using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 文件浏览器服务 — 纯文件系统操作，无数据库依赖
/// </summary>
public class FileExplorerService : IFileExplorerService
{
    public Task<List<FileSystemItem>> GetDirectoryContentsAsync(string path)
    {
        return Task.Run(() =>
        {
            var items = new List<FileSystemItem>();
            try
            {
                var dirInfo = new DirectoryInfo(path);

                // 子目录在前
                foreach (var dir in dirInfo.EnumerateDirectories().OrderBy(d => d.Name))
                {
                    items.Add(new FileSystemItem
                    {
                        Name = dir.Name,
                        FullPath = dir.FullName,
                        IsDirectory = true,
                        Size = 0,
                        LastModified = dir.LastWriteTime
                    });
                }

                // 文件在后
                foreach (var file in dirInfo.EnumerateFiles().OrderBy(f => f.Name))
                {
                    items.Add(new FileSystemItem
                    {
                        Name = file.Name,
                        FullPath = file.FullName,
                        IsDirectory = false,
                        Size = file.Length,
                        LastModified = file.LastWriteTime
                    });
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }

            return items;
        });
    }

    public Task<List<FileSystemItem>> SearchAsync(string rootPath, string term, CancellationToken ct = default)
    {
        return Task.Run(() =>
        {
            var results = new List<FileSystemItem>();
            var searchTerm = term.ToLowerInvariant();
            SearchRecursive(rootPath, searchTerm, results, ct);

            return results
                .OrderBy(i => i.IsDirectory ? 0 : 1)
                .ThenBy(i => i.Name)
                .ToList();
        }, ct);
    }

    private static void SearchRecursive(string path, string term, List<FileSystemItem> results, CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return;

        try
        {
            // 匹配目录
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                if (ct.IsCancellationRequested) return;

                var dirName = Path.GetFileName(dir);
                if (dirName.ToLowerInvariant().Contains(term))
                {
                    try
                    {
                        var di = new DirectoryInfo(dir);
                        results.Add(new FileSystemItem
                        {
                            Name = dirName,
                            FullPath = dir,
                            IsDirectory = true,
                            Size = 0,
                            LastModified = di.LastWriteTime
                        });
                    }
                    catch { }
                }

                SearchRecursive(dir, term, results, ct);
            }

            // 匹配文件
            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (ct.IsCancellationRequested) return;

                var fileName = Path.GetFileName(file);
                if (fileName.ToLowerInvariant().Contains(term))
                {
                    try
                    {
                        var fi = new FileInfo(file);
                        results.Add(new FileSystemItem
                        {
                            Name = fileName,
                            FullPath = file,
                            IsDirectory = false,
                            Size = fi.Length,
                            LastModified = fi.LastWriteTime
                        });
                    }
                    catch { }
                }
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (DirectoryNotFoundException) { }
    }

    public void OpenFile(string filePath)
    {
        try
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FileExplorerService.OpenFile failed: {ex.Message}");
        }
    }

    public void OpenInExplorer(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = false });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FileExplorerService.OpenInExplorer failed: {ex.Message}");
        }
    }

    public Task CopyFileAsync(string sourceFilePath, string targetDirectory)
    {
        return Task.Run(() =>
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var destPath = Path.Combine(targetDirectory, fileName);

            // 如果目标已存在同名文件，添加序号
            if (File.Exists(destPath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                int counter = 1;
                do
                {
                    destPath = Path.Combine(targetDirectory, $"{nameWithoutExt} ({counter}){ext}");
                    counter++;
                } while (File.Exists(destPath));
            }

            File.Copy(sourceFilePath, destPath);
        });
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public string? GetParentPath(string path)
    {
        return Directory.GetParent(path)?.FullName;
    }
}
