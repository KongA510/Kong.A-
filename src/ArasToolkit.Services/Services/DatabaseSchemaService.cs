using System.Data;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ArasToolkit.Services.Services;

/// <summary>为设置页提供逐表结构同步结果。</summary>
public sealed class DatabaseSchemaService : IDatabaseSchemaService
{
    /// <summary>
    /// 数据库检查专用字段映射。实体新增映射字段时，必须在此处添加对应的幂等同步 SQL。
    /// </summary>
    private static readonly IReadOnlyList<DatabaseFieldMapping> FieldMappings =
    [
        new(
            "related_code_record",
            "sort_order",
            """
            IF OBJECT_ID(N'dbo.related_code_record', N'U') IS NOT NULL
               AND COL_LENGTH(N'dbo.related_code_record', N'sort_order') IS NULL
            BEGIN
                ALTER TABLE related_code_record
                    ADD sort_order INT NOT NULL DEFAULT 0 WITH VALUES;

                EXEC sys.sp_executesql N'
                    ;WITH ordered_records AS
                    (
                        SELECT id,
                               ROW_NUMBER() OVER
                                   (PARTITION BY user_id ORDER BY creator_on DESC, id) - 1 AS row_order
                        FROM related_code_record
                    )
                    UPDATE target
                    SET sort_order = ordered_records.row_order
                    FROM related_code_record AS target
                    INNER JOIN ordered_records ON ordered_records.id = target.id;';
            END
            """)
    ];

    private static readonly IReadOnlyDictionary<string, string> TableDisplayNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["related_code_record"] = "相关代码主题",
            ["related_code_segment"] = "相关代码段",
            ["common_query_snippet"] = "常用查询片段",
            ["personal_task"] = "个人任务",
            ["operation_log"] = "敏感操作日志",
            ["error_log"] = "错误日志",
            ["changelog"] = "更新日志",
            ["app_user"] = "应用用户"
        };

    private readonly IDbContextFactory<ArasToolkitDbContext> _dbFactory;
    private readonly IErrorLogService _errorLogService;
    private readonly IOperationLogService _operationLogService;

    public DatabaseSchemaService(
        IDbContextFactory<ArasToolkitDbContext> dbFactory,
        IErrorLogService errorLogService,
        IOperationLogService operationLogService)
    {
        _dbFactory = dbFactory;
        _errorLogService = errorLogService;
        _operationLogService = operationLogService;
    }

    public async Task<DatabaseSchemaCheckResult> CheckAndSynchronizeAsync()
    {
        IReadOnlyList<ManagedTable> expectedTables = [];
        SchemaSnapshot before = SchemaSnapshot.Empty;
        Exception? failure = null;

        try
        {
            await using var context = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            expectedTables = GetManagedTables(context);
            before = await ReadSnapshotAsync(context).ConfigureAwait(false);
            await ApplyFieldMappingsAsync(context).ConfigureAwait(false);
            await context.EnsureSchemaAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            failure = ex;
            await _errorLogService.LogErrorAsync("设置-数据库结构同步", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
        }

        SchemaSnapshot after;
        try
        {
            await using var verifyContext = await _dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            if (expectedTables.Count == 0)
                expectedTables = GetManagedTables(verifyContext);
            after = await ReadSnapshotAsync(verifyContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            failure ??= ex;
            after = SchemaSnapshot.Empty;
            await _errorLogService.LogErrorAsync("设置-数据库结构复检", ex.Message,
                ErrorLog.LevelP0, ex.StackTrace).ConfigureAwait(false);
        }

        var tables = BuildResults(expectedTables, before, after, failure);
        var success = failure == null && tables.All(item => item.Status != DatabaseSchemaTableStatus.Failed);
        if (success)
            await LogOperationSafelyAsync(tables).ConfigureAwait(false);

        return new DatabaseSchemaCheckResult
        {
            IsSuccess = success,
            ErrorMessage = failure?.Message,
            Tables = tables
        };
    }

    private static async Task ApplyFieldMappingsAsync(ArasToolkitDbContext context)
    {
        foreach (var mapping in FieldMappings)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(mapping.SynchronizeSql)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"同步 {mapping.TableName}.{mapping.ColumnName} 字段失败：{ex.Message}", ex);
            }
        }
    }

    private static IReadOnlyList<ManagedTable> GetManagedTables(ArasToolkitDbContext context)
    {
        var tables = new Dictionary<string, ManagedTable>(StringComparer.OrdinalIgnoreCase);
        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                continue;
            var schema = string.IsNullOrWhiteSpace(entityType.GetSchema()) ? "dbo" : entityType.GetSchema()!;
            var key = $"{schema}.{tableName}";
            if (!tables.TryGetValue(key, out var table))
            {
                table = new ManagedTable(
                    schema,
                    tableName,
                    TableDisplayNames.TryGetValue(tableName, out var displayName)
                        ? displayName
                        : entityType.ClrType.Name,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                tables.Add(key, table);
            }

            var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(storeObject);
                if (!string.IsNullOrWhiteSpace(columnName))
                    table.Columns.Add(columnName);
            }
        }

        return tables.Values.OrderBy(item => item.TableName, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static async Task<SchemaSnapshot> ReadSnapshotAsync(ArasToolkitDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await context.Database.OpenConnectionAsync().ConfigureAwait(false);

        try
        {
            var tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var databaseObjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS;
                    """;
                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var table = $"{reader.GetString(0)}.{reader.GetString(1)}";
                    tables.Add(table);
                    columns.Add($"{table}.{reader.GetString(2)}");
                }
            }

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    SELECT name FROM sys.indexes WHERE name IS NOT NULL
                    UNION ALL
                    SELECT name FROM sys.foreign_keys;
                    """;
                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                    databaseObjects.Add(reader.GetString(0));
            }

            return new SchemaSnapshot(tables, columns, databaseObjects);
        }
        finally
        {
            if (shouldClose)
                await context.Database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }

    private static IReadOnlyList<DatabaseSchemaTableResult> BuildResults(
        IReadOnlyList<ManagedTable> expectedTables,
        SchemaSnapshot before,
        SchemaSnapshot after,
        Exception? failure)
    {
        var results = new List<DatabaseSchemaTableResult>(expectedTables.Count);
        foreach (var table in expectedTables)
        {
            var tableKey = $"{table.Schema}.{table.TableName}";
            var missingAfter = table.Columns
                .Where(column => !after.Columns.Contains($"{tableKey}.{column}"))
                .OrderBy(column => column, StringComparer.OrdinalIgnoreCase)
                .ToList();
            var missingObjectsAfter = GetRequiredObjects(table.TableName)
                .Where(item => !after.DatabaseObjects.Contains(item))
                .ToList();

            DatabaseSchemaTableStatus status;
            string details;
            if (!after.Tables.Contains(tableKey))
            {
                status = DatabaseSchemaTableStatus.Failed;
                details = failure == null ? "数据表不存在。" : $"同步未完成：{failure.Message}";
            }
            else if (missingAfter.Count > 0 || missingObjectsAfter.Count > 0)
            {
                status = DatabaseSchemaTableStatus.Failed;
                var parts = new List<string>();
                if (missingAfter.Count > 0)
                    parts.Add($"缺少列：{string.Join("、", missingAfter)}");
                if (missingObjectsAfter.Count > 0)
                    parts.Add($"缺少约束/索引：{string.Join("、", missingObjectsAfter)}");
                details = string.Join("；", parts);
            }
            else if (!before.Tables.Contains(tableKey))
            {
                status = DatabaseSchemaTableStatus.Created;
                details = $"已创建并验证 {table.Columns.Count} 个映射列。";
            }
            else
            {
                var missingBeforeCount = table.Columns.Count(column =>
                    !before.Columns.Contains($"{tableKey}.{column}"));
                var missingObjectsBefore = GetRequiredObjects(table.TableName)
                    .Count(item => !before.DatabaseObjects.Contains(item));
                if (missingBeforeCount + missingObjectsBefore > 0)
                {
                    status = DatabaseSchemaTableStatus.Updated;
                    details = $"已补全 {missingBeforeCount} 个列、{missingObjectsBefore} 个约束或索引。";
                }
                else
                {
                    status = DatabaseSchemaTableStatus.Ready;
                    details = $"{table.Columns.Count} 个映射列均完整。";
                }
            }

            results.Add(new DatabaseSchemaTableResult
            {
                TableName = table.TableName,
                DisplayName = table.DisplayName,
                Status = status,
                Details = details
            });
        }

        return results;
    }

    private static IReadOnlyList<string> GetRequiredObjects(string tableName) => tableName switch
    {
        "related_code_record" => ["IX_related_code_record_user_sort"],
        "related_code_segment" =>
            ["FK_related_code_segment_record", "IX_related_code_segment_record_sort"],
        _ => []
    };

    private async Task LogOperationSafelyAsync(IReadOnlyList<DatabaseSchemaTableResult> tables)
    {
        try
        {
            var changed = tables.Count(item => item.Status is
                DatabaseSchemaTableStatus.Created or DatabaseSchemaTableStatus.Updated);
            await _operationLogService.LogAsync("Update", "DatabaseSchema", "all",
                $"数据库结构检查完成，共 {tables.Count} 张受管表，变更 {changed} 张。")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Schema] 操作日志写入失败: {ex.Message}");
        }
    }

    private sealed record ManagedTable(
        string Schema,
        string TableName,
        string DisplayName,
        HashSet<string> Columns);

    private sealed record DatabaseFieldMapping(
        string TableName,
        string ColumnName,
        string SynchronizeSql);

    private sealed record SchemaSnapshot(
        HashSet<string> Tables,
        HashSet<string> Columns,
        HashSet<string> DatabaseObjects)
    {
        public static SchemaSnapshot Empty { get; } = new(
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            new HashSet<string>(StringComparer.OrdinalIgnoreCase));
    }
}
