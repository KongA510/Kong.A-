using System.Text.Json;
using ArasToolkit.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Data;

/// <summary>
/// Aras 工具箱 EF Core 数据库上下文
/// 管理 personal_task, operation_log, error_log, changelog 表的 ORM 映射
/// </summary>
public class ArasToolkitDbContext : DbContext
{
    /// <summary>个人任务表</summary>
    public DbSet<PersonalTask> PersonalTasks => Set<PersonalTask>();

    /// <summary>操作日志表</summary>
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();

    /// <summary>错误日志表</summary>
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();

   /// <summary>更新日志表</summary>
   public DbSet<Changelog> Changelogs => Set<Changelog>();
 
     /// <summary>应用用户表</summary>
     public DbSet<AppUser> AppUsers => Set<AppUser>();

    /// <summary>缓存的连接字符串（避免重复读取文件）</summary>
    private static string? _cachedConnectionString;

    public ArasToolkitDbContext() { }

    public ArasToolkitDbContext(DbContextOptions<ArasToolkitDbContext> options)
        : base(options) { }

    /// <summary>
    /// 配置数据库连接 — 从 DBSeeting.json 读取 SQL Server 连接字符串
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    /// <summary>
    /// 配置实体映射 — 指定表名和列名
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PersonalTask → personal_task 表
        modelBuilder.Entity<PersonalTask>(entity =>
        {
            entity.ToTable("personal_task");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.TaskName).HasColumnName("task_name").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("task_state").HasMaxLength(50);
            entity.Property(e => e.ExpectedDate).HasColumnName("estinmated_time");
            entity.Property(e => e.CompletionDate).HasColumnName("completing_time");
            entity.Property(e => e.ProjectName).HasColumnName("project_name").HasMaxLength(200);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(1000);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(2000);
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.CompletionPercent).HasColumnName("completion_percent");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.IsSelected);
            entity.Ignore(e => e.IsDueToday);
            entity.Ignore(e => e.DisplayDate);
            entity.Ignore(e => e.DisplayStartDate);
            entity.Ignore(e => e.DisplayCompletionDate);
            entity.Ignore(e => e.DisplayCreatedDate);
            entity.Ignore(e => e.DisplayModifiedDate);
            entity.Ignore(e => e.DisplayCreatorOn);
        });

        // OperationLog → operation_log 表
        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.ToTable("operation_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.OperationType).HasColumnName("operation_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasColumnName("entity_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.OperateTime).HasColumnName("operate_time");
            entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
        });

        // ErrorLog → error_log 表
        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.ToTable("error_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.FunctionName).HasColumnName("function_name").HasMaxLength(200);
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.RecordDate).HasColumnName("record_date");
            entity.Property(e => e.Level).HasColumnName("level").HasMaxLength(20);
            entity.Property(e => e.StackTrace).HasColumnName("stack_trace");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayDate);
        });

        // Changelog → changelog 表
        modelBuilder.Entity<Changelog>(entity =>
        {
            entity.ToTable("changelog");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Version).HasColumnName("version").HasMaxLength(20);
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Author).HasColumnName("author").HasMaxLength(100);
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

           entity.Ignore(e => e.DisplayDate);
       });
 
         // AppUser → app_user 表
         modelBuilder.Entity<AppUser>(entity =>
         {
             entity.ToTable("app_user");
             entity.HasKey(e => e.Id);
             entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
             entity.Property(e => e.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
             entity.Property(e => e.Password).HasColumnName("password").IsRequired().HasMaxLength(100);
             entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(100);
             entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
             entity.Property(e => e.Avatar).HasColumnName("avatar").HasMaxLength(500);
             entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
         });
   }

   /// <summary>
    /// 从 DBSeeting.json 读取 SQL Server 连接字符串（带缓存）
    /// </summary>
    private static string GetConnectionString()
    {
        if (!string.IsNullOrEmpty(_cachedConnectionString))
            return _cachedConnectionString;

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(baseDir, "DBSeeting.json");

        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(baseDir, "..", "..", "..", "..",
                "ArasToolkit.Core", "DBSeeting.json");
        }

        if (!File.Exists(configPath))
            throw new FileNotFoundException(
                $"找不到数据库配置文件 DBSeeting.json，已搜索路径: {baseDir}");

        var json = File.ReadAllText(configPath);
        using var doc = JsonDocument.Parse(json);
        _cachedConnectionString = doc.RootElement.GetProperty("sql").GetString() ?? "";

        if (string.IsNullOrEmpty(_cachedConnectionString))
            throw new InvalidOperationException("DBSeeting.json 中未找到 'sql' 连接字符串");

        return _cachedConnectionString;
    }

    /// <summary>
    /// 刷新缓存的连接字符串（配置变更后调用）
    /// </summary>
    public static void RefreshConnectionString()
    {
        _cachedConnectionString = null;
    }

    /// <summary>
    /// 同步数据库表结构（仅当用户明确要求时调用）
    /// — 检查并添加缺失的列/表
    /// </summary>
    public async Task EnsureSchemaAsync()
    {
        try
        {
            var sql = @"
                -- ===== personal_task 表列同步 =====

                -- 任务说明
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='description')
                BEGIN
                    ALTER TABLE personal_task ADD description NVARCHAR(2000) NULL;
                    UPDATE personal_task SET description = '' WHERE description IS NULL;
                END

                -- 开始时间
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='start_date')
                    ALTER TABLE personal_task ADD start_date DATETIME2 NULL;

                -- 完成度
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='completion_percent')
                    ALTER TABLE personal_task ADD completion_percent INT NOT NULL DEFAULT 0;

                -- 创建日期
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='created_date')
                    ALTER TABLE personal_task ADD created_date DATETIME2 NOT NULL DEFAULT GETDATE();

                -- 修改日期
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='modified_date')
                    ALTER TABLE personal_task ADD modified_date DATETIME2 NULL;

                -- 记录创建时间（全部表通用列）
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE personal_task ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    UPDATE personal_task SET creator_on = ISNULL(created_date, GETDATE());
                END

                -- 备注
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='remarks')
                BEGIN
                    ALTER TABLE personal_task ADD remarks NVARCHAR(1000) NULL;
                    UPDATE personal_task SET remarks = '' WHERE remarks IS NULL;
                END

                -- created_by / modified_by
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='created_by')
                BEGIN
                    ALTER TABLE personal_task ADD created_by NVARCHAR(100) NULL;
                END

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='modified_by')
                BEGIN
                    ALTER TABLE personal_task ADD modified_by NVARCHAR(100) NULL;
                END

                -- ===== operation_log 表列同步 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='operation_log' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE operation_log ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    UPDATE operation_log SET creator_on = ISNULL(operate_time, GETDATE());
                END

                -- ===== error_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='error_log')
                BEGIN
                    CREATE TABLE error_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        function_name NVARCHAR(200) NOT NULL,
                        error_message NVARCHAR(MAX) NOT NULL,
                        record_date DATETIME2 NOT NULL DEFAULT GETDATE(),
                        level NVARCHAR(20) NOT NULL DEFAULT 'P1-普通',
                        stack_trace NVARCHAR(MAX) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_name')
                BEGIN
                    ALTER TABLE error_log ADD user_name NVARCHAR(100) NULL;
                END

                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE error_log ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    UPDATE error_log SET creator_on = ISNULL(record_date, GETDATE());
                END

                -- ===== changelog 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='changelog')
                BEGIN
                    CREATE TABLE changelog (
                        id BIGINT IDENTITY(1,1) PRIMARY KEY,
                        version NVARCHAR(20) NOT NULL,
                        release_date DATETIME2 NOT NULL DEFAULT GETDATE(),
                        type NVARCHAR(20) NOT NULL DEFAULT N'新增',
                        description NVARCHAR(MAX) NOT NULL,
                        author NVARCHAR(100) NOT NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='changelog' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE changelog ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    UPDATE changelog SET creator_on = ISNULL(release_date, GETDATE());
               END
 
                 -- ===== app_user 表 =====
                 IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='app_user')
                 BEGIN
                     CREATE TABLE app_user (
                         id NVARCHAR(12) NOT NULL PRIMARY KEY,
                         username NVARCHAR(100) NOT NULL,
                         password NVARCHAR(100) NOT NULL,
                         display_name NVARCHAR(100) NULL,
                         is_admin BIT NOT NULL DEFAULT 0,
                         avatar NVARCHAR(500) NULL,
                         creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                     );
                 END
                 ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='app_user' AND COLUMN_NAME='creator_on')
                 BEGIN
                     ALTER TABLE app_user ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                 END
            ";
            await Database.ExecuteSqlRawAsync(sql);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Schema] 表结构同步失败: {ex.Message}");
        }
    }
}
