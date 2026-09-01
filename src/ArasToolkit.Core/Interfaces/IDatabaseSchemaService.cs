using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>显式检查并同步应用数据库表结构。</summary>
public interface IDatabaseSchemaService
{
    Task<DatabaseSchemaCheckResult> CheckAndSynchronizeAsync();
}
