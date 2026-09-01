using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

/// <summary>相关代码主题与有序代码段的数据库服务。</summary>
public sealed class RelatedCodeRecordService : IRelatedCodeRecordService
{
    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public RelatedCodeRecordService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _dbFactory = dbFactory;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<RelatedCodeRecord>> GetAllAsync(string? keyword = null)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var query = db.RelatedCodeRecords
                .AsNoTracking()
                .Where(item => item.UserId == userId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var value = keyword.Trim();
                query = query.Where(item =>
                    item.Title.Contains(value) ||
                    (item.Description != null && item.Description.Contains(value)) ||
                    item.Segments.Any(segment =>
                        segment.SegmentName.Contains(value) ||
                        segment.CodeType.Contains(value) ||
                        (segment.Description != null && segment.Description.Contains(value)) ||
                        segment.CodeContent.Contains(value)));
            }

            return await query
                .OrderByDescending(item => item.CreatorOn)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-加载列表", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<RelatedCodeRecord?> GetByIdAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var record = await db.RelatedCodeRecords
                .AsNoTracking()
                .Include(item => item.Segments)
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId)
                .ConfigureAwait(false);
            if (record != null)
                record.Segments = record.Segments.OrderBy(item => item.SortOrder).ToList();
            return record;
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-加载详情", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<RelatedCodeRecord> SaveRecordAsync(RelatedCodeRecord record)
    {
        try
        {
            ValidateRecord(record);
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var userId = CurrentUserContext.CurrentUserId;
            var existing = string.IsNullOrWhiteSpace(record.Id)
                ? null
                : await db.RelatedCodeRecords
                    .FirstOrDefaultAsync(item => item.Id == record.Id && item.UserId == userId)
                    .ConfigureAwait(false);

            if (existing == null)
            {
                record.Id = Guid.NewGuid().ToString("N")[..12];
                record.Title = record.Title.Trim();
                record.Description = NormalizeOptional(record.Description);
                record.UserId = userId;
                record.CreatorOn = DateTime.Now;
                record.Segments = [];
                db.RelatedCodeRecords.Add(record);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await LogOperationSafelyAsync("Create", nameof(RelatedCodeRecord), record.Id,
                    $"创建相关代码主题: {record.Title}").ConfigureAwait(false);
                return record;
            }

            existing.Title = record.Title.Trim();
            existing.Description = NormalizeOptional(record.Description);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Update", nameof(RelatedCodeRecord), existing.Id,
                $"更新相关代码主题: {existing.Title}").ConfigureAwait(false);
            record.Id = existing.Id;
            record.UserId = existing.UserId;
            record.CreatorOn = existing.CreatorOn;
            return record;
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-保存主题", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<RelatedCodeSegment> SaveSegmentAsync(string recordId, RelatedCodeSegment segment)
    {
        try
        {
            ValidateSegment(segment);
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var record = await GetOwnedRecordAsync(db, recordId).ConfigureAwait(false);
            var existing = string.IsNullOrWhiteSpace(segment.Id)
                ? null
                : await db.RelatedCodeSegments
                    .FirstOrDefaultAsync(item => item.Id == segment.Id && item.RecordId == record.Id)
                    .ConfigureAwait(false);

            if (existing == null)
            {
                var nextOrder = await db.RelatedCodeSegments
                    .Where(item => item.RecordId == record.Id)
                    .Select(item => (int?)item.SortOrder)
                    .MaxAsync()
                    .ConfigureAwait(false) ?? -1;

                segment.Id = Guid.NewGuid().ToString("N")[..12];
                segment.RecordId = record.Id;
                segment.SegmentName = segment.SegmentName.Trim();
                segment.CodeType = RelatedCodeTypes.Normalize(segment.CodeType);
                segment.Description = NormalizeOptional(segment.Description);
                segment.SortOrder = nextOrder + 1;
                segment.CreatorOn = DateTime.Now;
                segment.Record = null;
                db.RelatedCodeSegments.Add(segment);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await LogOperationSafelyAsync("Create", nameof(RelatedCodeSegment), segment.Id,
                    $"为“{record.Title}”新增 {segment.CodeType} 代码段: {segment.SegmentName}")
                    .ConfigureAwait(false);
                return segment;
            }

            existing.SegmentName = segment.SegmentName.Trim();
            existing.CodeType = RelatedCodeTypes.Normalize(segment.CodeType);
            existing.Description = NormalizeOptional(segment.Description);
            existing.CodeContent = segment.CodeContent;
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Update", nameof(RelatedCodeSegment), existing.Id,
                $"更新“{record.Title}”的 {existing.CodeType} 代码段: {existing.SegmentName}")
                .ConfigureAwait(false);

            segment.RecordId = existing.RecordId;
            segment.SortOrder = existing.SortOrder;
            segment.CreatorOn = existing.CreatorOn;
            segment.CodeType = existing.CodeType;
            return segment;
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-保存代码段", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task DeleteRecordAsync(string id)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var record = await GetOwnedRecordAsync(db, id).ConfigureAwait(false);
            var segmentCount = await db.RelatedCodeSegments.CountAsync(item => item.RecordId == record.Id)
                .ConfigureAwait(false);
            db.RelatedCodeRecords.Remove(record);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Delete", nameof(RelatedCodeRecord), record.Id,
                $"删除相关代码主题“{record.Title}”及 {segmentCount} 个代码段")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-删除主题", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task DeleteSegmentAsync(string recordId, string segmentId)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var record = await GetOwnedRecordAsync(db, recordId).ConfigureAwait(false);
            var segment = await db.RelatedCodeSegments
                .FirstOrDefaultAsync(item => item.Id == segmentId && item.RecordId == record.Id)
                .ConfigureAwait(false);
            if (segment == null)
                return;

            db.RelatedCodeSegments.Remove(segment);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await NormalizeOrderAsync(db, record.Id).ConfigureAwait(false);
            await LogOperationSafelyAsync("Delete", nameof(RelatedCodeSegment), segment.Id,
                $"删除“{record.Title}”的 {segment.CodeType} 代码段: {segment.SegmentName}")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-删除代码段", ex).ConfigureAwait(false);
            throw;
        }
    }

    public async Task ReorderSegmentsAsync(string recordId, IReadOnlyList<string> orderedSegmentIds)
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            var record = await GetOwnedRecordAsync(db, recordId).ConfigureAwait(false);
            var segments = await db.RelatedCodeSegments
                .Where(item => item.RecordId == record.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            if (orderedSegmentIds.Count != segments.Count ||
                orderedSegmentIds.Distinct(StringComparer.Ordinal).Count() != segments.Count ||
                segments.Any(item => !orderedSegmentIds.Contains(item.Id, StringComparer.Ordinal)))
            {
                throw new InvalidOperationException("代码段排序数据已变化，请刷新后重试。");
            }

            var orderMap = orderedSegmentIds
                .Select((id, index) => (id, index))
                .ToDictionary(item => item.id, item => item.index, StringComparer.Ordinal);
            foreach (var segment in segments)
                segment.SortOrder = orderMap[segment.Id];

            await db.SaveChangesAsync().ConfigureAwait(false);
            await LogOperationSafelyAsync("Update", nameof(RelatedCodeRecord), record.Id,
                $"调整相关代码主题“{record.Title}”的代码段顺序")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await LogErrorAsync("相关代码记录-调整顺序", ex).ConfigureAwait(false);
            throw;
        }
    }

    private static async Task<RelatedCodeRecord> GetOwnedRecordAsync(
        ArasToolkitDbContext db, string recordId)
    {
        var userId = CurrentUserContext.CurrentUserId;
        return await db.RelatedCodeRecords
            .FirstOrDefaultAsync(item => item.Id == recordId && item.UserId == userId)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("未找到相关代码主题，或当前用户无权访问。");
    }

    private static async Task NormalizeOrderAsync(ArasToolkitDbContext db, string recordId)
    {
        var segments = await db.RelatedCodeSegments
            .Where(item => item.RecordId == recordId)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.CreatorOn)
            .ToListAsync()
            .ConfigureAwait(false);
        for (var index = 0; index < segments.Count; index++)
            segments[index].SortOrder = index;
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private static void ValidateRecord(RelatedCodeRecord record)
    {
        if (string.IsNullOrWhiteSpace(record.Title))
            throw new InvalidOperationException("请输入主题标题。");
        if (record.Title.Trim().Length > 200)
            throw new InvalidOperationException("主题标题不能超过 200 个字符。");
        if (record.Description?.Trim().Length > 2000)
            throw new InvalidOperationException("主题说明不能超过 2000 个字符。");
    }

    private static void ValidateSegment(RelatedCodeSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.SegmentName))
            throw new InvalidOperationException("请输入代码段名称。");
        if (segment.SegmentName.Trim().Length > 200)
            throw new InvalidOperationException("代码段名称不能超过 200 个字符。");
        if (segment.Description?.Trim().Length > 1000)
            throw new InvalidOperationException("代码段说明不能超过 1000 个字符。");
        if (string.IsNullOrWhiteSpace(segment.CodeContent))
            throw new InvalidOperationException("请输入代码内容。");
    }

    private async Task LogOperationSafelyAsync(
        string operationType, string entityType, string id, string description)
    {
        try
        {
            await _operationLogService.LogAsync(operationType, entityType, id, description)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[相关代码记录] 操作日志写入失败: {ex.Message}");
        }
    }

    private Task LogErrorAsync(string functionName, Exception ex) =>
        _errorLogService.LogErrorAsync(functionName, ex.Message, ErrorLog.LevelP0, ex.StackTrace);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
