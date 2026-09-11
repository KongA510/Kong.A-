using System.Xml.Linq;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>
/// 已保存 XML 的数据库服务。可显式开放管理员全量查询，写操作仍按当前用户隔离。
/// </summary>
public sealed class SavedXmlService : ISavedXmlService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public SavedXmlService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<SavedXml>> GetAllAsync(bool includeAllForAdmin = false)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var canReadAll = includeAllForAdmin && CurrentUserContext.IsAdmin;
            return await db.SavedXmlItems
                .AsNoTracking()
                .Where(item => canReadAll || item.UserId == userId)
                .OrderByDescending(item => item.CreatorOn)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("XML数据库-加载", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<SavedXml> SaveAsync(string name, string xmlContent)
    {
        try
        {
            var normalizedName = ValidateAndNormalize(name, xmlContent);
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var existing = await db.SavedXmlItems
                .FirstOrDefaultAsync(item => item.UserId == userId && item.Name == normalizedName)
                .ConfigureAwait(false);

            if (existing == null)
            {
                var created = new SavedXml
                {
                    Id = Guid.NewGuid().ToString("N")[..12],
                    Name = normalizedName,
                    XmlContent = xmlContent,
                    UserId = userId,
                    CreatorOn = DateTime.Now
                };
                db.SavedXmlItems.Add(created);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await LogOperationSafelyAsync("Create", created.Id,
                    $"保存 XML: {created.Name}").ConfigureAwait(false);
                return created;
            }

            // 同一用户下名称唯一；再次保存相同名称即覆盖内容并更新排序时间。
            existing.XmlContent = xmlContent;
            existing.CreatorOn = DateTime.Now;
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Update", existing.Id,
                $"覆盖 XML: {existing.Name}").ConfigureAwait(false);
            return existing;
        }
        catch (Exception ex)
        {
            var level = ex is FormatException or InvalidOperationException
                ? ErrorLog.LevelP1
                : ErrorLog.LevelP0;
            await _errorLogService.LogErrorAsync("XML数据库-保存", ex.Message,
                level, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var existing = await db.SavedXmlItems
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId)
                .ConfigureAwait(false);
            if (existing == null) return;

            db.SavedXmlItems.Remove(existing);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Delete", existing.Id,
                $"删除 XML: {existing.Name}").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("XML数据库-删除", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    private static string ValidateAndNormalize(string name, string xmlContent)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("请输入 XML 名称。");
        var normalizedName = name.Trim();
        if (normalizedName.Length > 200)
            throw new InvalidOperationException("XML 名称不能超过 200 个字符。");
        if (string.IsNullOrWhiteSpace(xmlContent))
            throw new InvalidOperationException("没有可保存的 XML 内容。");

        // 数据库存储前再次校验，避免其他调用方绕过格式化页面写入无效 XML。
        try
        {
            _ = XDocument.Parse(xmlContent, LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex) when (ex is System.Xml.XmlException or ArgumentException)
        {
            throw new FormatException($"XML 结构无效：{ex.Message}", ex);
        }

        return normalizedName;
    }

    private async Task LogOperationSafelyAsync(string operationType, string id, string description)
    {
        try
        {
            await _operationLogService.LogAsync(operationType, nameof(SavedXml), id, description)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // 操作日志失败不能阻断 XML 主业务。
            System.Diagnostics.Debug.WriteLine($"[XML数据库] 操作日志写入失败: {ex.Message}");
        }
    }
}
