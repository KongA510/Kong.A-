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

    /// <summary>数据导入配置表</summary>
    public DbSet<DataImportConfig> DataImportConfigs => Set<DataImportConfig>();

    /// <summary>文本翻译记录表</summary>
    public DbSet<TextTranslationRecord> TextTranslationRecords => Set<TextTranslationRecord>();

    /// <summary>AI 模型配置表</summary>
    public DbSet<AiModelConfig> AiModelConfigs => Set<AiModelConfig>();

    /// <summary>对象类配置导入日志表</summary>
    public DbSet<ObjectClassImportLog> ObjectClassImportLogs => Set<ObjectClassImportLog>();

    /// <summary>List配置导入日志表</summary>
    public DbSet<ListImportLog> ListImportLogs => Set<ListImportLog>();

    /// <summary>属性配置导入日志表</summary>
    public DbSet<PropertyImportLog> PropertyImportLogs => Set<PropertyImportLog>();

    /// <summary>Aras登录配置表</summary>
    public DbSet<ArasLoginConfig> ArasLoginConfigs => Set<ArasLoginConfig>();

    /// <summary>权限配置导入日志表</summary>
    public DbSet<PermissionImportLog> PermissionImportLogs => Set<PermissionImportLog>();

    /// <summary>生命周期配置导入日志表</summary>
    public DbSet<LifecycleImportLog> LifecycleImportLogs => Set<LifecycleImportLog>();

    /// <summary>个人资料库笔记表</summary>
    public DbSet<KnowledgeEntry> KnowledgeEntries => Set<KnowledgeEntry>();

    /// <summary>SQL模板表</summary>
    public DbSet<SqlTemplate> SqlTemplates => Set<SqlTemplate>();

    /// <summary>数据库导出日志表</summary>
    public DbSet<DatabaseExportLog> DatabaseExportLogs => Set<DatabaseExportLog>();

    /// <summary>数据库导出配置表</summary>
    public DbSet<DatabaseExportConfig> DatabaseExportConfigs => Set<DatabaseExportConfig>();

    /// <summary>翻译任务表</summary>
    public DbSet<TranslationTask> TranslationTasks => Set<TranslationTask>();

    /// <summary>翻译记录表</summary>
    public DbSet<TranslationRecord> TranslationRecords => Set<TranslationRecord>();

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
            optionsBuilder.UseSqlServer(GetConnectionString(), o => o.EnableRetryOnFailure());
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
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(100);

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
        // DataImportConfig -> data_import_config table
        modelBuilder.Entity<DataImportConfig>(entity =>
        {
            entity.ToTable("data_import_config");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.ConfigName).HasColumnName("config_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.AmlContent).HasColumnName("aml_content").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
        });

        // TextTranslationRecord → text_translation_record 表
        modelBuilder.Entity<TextTranslationRecord>(entity =>
        {
            entity.ToTable("text_translation_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.SourceFileName).HasColumnName("source_file_name").IsRequired().HasMaxLength(500);
            entity.Property(e => e.OutputFileName).HasColumnName("output_file_name").HasMaxLength(500);
            entity.Property(e => e.OutputFilePath).HasColumnName("output_file_path").HasMaxLength(1000);
            entity.Property(e => e.TemplateType).HasColumnName("template_type").HasMaxLength(50);
            entity.Property(e => e.SourceLanguage).HasColumnName("source_language").HasMaxLength(50);
            entity.Property(e => e.SourceRowCount).HasColumnName("source_row_count");
            entity.Property(e => e.BatchCount).HasColumnName("batch_count");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(100);
            entity.Property(e => e.AiModelId).HasColumnName("ai_model_id").HasMaxLength(12);
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayCreatedAt);
        });

        // AiModelConfig → ai_model_config 表
        modelBuilder.Entity<AiModelConfig>(entity =>
        {
            entity.ToTable("ai_model_config");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModelName).HasColumnName("model_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApiKey).HasColumnName("api_key").HasMaxLength(500);
            entity.Property(e => e.ApiBaseUrl).HasColumnName("api_base_url").HasMaxLength(500);
            entity.Property(e => e.ModelIdentifier).HasColumnName("model_identifier").HasMaxLength(100);
            entity.Property(e => e.ExtraParams).HasColumnName("extra_params");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.StatusText);
        });

        // ObjectClassImportLog → object_class_import_log 表
        modelBuilder.Entity<ObjectClassImportLog>(entity =>
        {
            entity.ToTable("object_class_import_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImportTime).HasColumnName("import_time").IsRequired();
            entity.Property(e => e.ImportFile).HasColumnName("import_file").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorLog).HasColumnName("error_log");
            entity.Property(e => e.Sheet1Count).HasColumnName("sheet1_count");
            entity.Property(e => e.Sheet2Count).HasColumnName("sheet2_count");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayImportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.Summary);
        });

        // ===== List导入日志表 =====
        modelBuilder.Entity<ListImportLog>(entity =>
        {
            entity.ToTable("list_import_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImportTime).HasColumnName("import_time").IsRequired();
            entity.Property(e => e.ImportFile).HasColumnName("import_file").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorLog).HasColumnName("error_log");
            entity.Property(e => e.Sheet1Count).HasColumnName("sheet1_count");
            entity.Property(e => e.Sheet2Count).HasColumnName("sheet2_count");
            entity.Property(e => e.Sheet3Count).HasColumnName("sheet3_count");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayImportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.Summary);
        });

        // ===== 属性配置导入日志表 =====
        modelBuilder.Entity<PropertyImportLog>(entity =>
        {
            entity.ToTable("property_import_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImportTime).HasColumnName("import_time").IsRequired();
            entity.Property(e => e.ImportFile).HasColumnName("import_file").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorLog).HasColumnName("error_log");
            entity.Property(e => e.Sheet1Count).HasColumnName("sheet1_count");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayImportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.Summary);
        });

        // ===== ArasLoginConfig → aras_login_config 表 =====
        modelBuilder.Entity<ArasLoginConfig>(entity =>
        {
            entity.ToTable("aras_login_config");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.Url).HasColumnName("url").IsRequired().HasMaxLength(500);
            entity.Property(e => e.DatabaseName).HasColumnName("database_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Md5Password).HasColumnName("md5_password").HasMaxLength(32);
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);

            entity.Ignore(e => e.StatusText);
        });

        // ===== 权限配置导入日志表 =====
        modelBuilder.Entity<PermissionImportLog>(entity =>
        {
            entity.ToTable("permission_import_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImportTime).HasColumnName("import_time").IsRequired();
            entity.Property(e => e.ImportFile).HasColumnName("import_file").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorLog).HasColumnName("error_log");
            entity.Property(e => e.Sheet1Count).HasColumnName("sheet1_count");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayImportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.Summary);
        });

        // ===== 生命周期配置导入日志表 =====
        modelBuilder.Entity<LifecycleImportLog>(entity =>
        {
            entity.ToTable("lifecycle_import_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImportTime).HasColumnName("import_time").IsRequired();
            entity.Property(e => e.ImportFile).HasColumnName("import_file").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorLog).HasColumnName("error_log");
            entity.Property(e => e.Sheet1Count).HasColumnName("sheet1_count");
            entity.Property(e => e.Sheet2Count).HasColumnName("sheet2_count");
            entity.Property(e => e.Sheet3Count).HasColumnName("sheet3_count");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");

            entity.Ignore(e => e.DisplayImportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.Summary);
        });

        // ===== KnowledgeEntry → knowledge_entry 表 =====
        modelBuilder.Entity<KnowledgeEntry>(entity =>
        {
            entity.ToTable("knowledge_entry");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.PlainTextPreview).HasColumnName("plain_text_preview").HasMaxLength(500);
            entity.Property(e => e.Tags).HasColumnName("tags").HasMaxLength(500);
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(200);
            entity.Property(e => e.Pinned).HasColumnName("pinned");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(100);

            entity.Ignore(e => e.DisplayModifiedDate);
            entity.Ignore(e => e.DisplayCreatedDate);
            entity.Ignore(e => e.DisplayTags);
            entity.Ignore(e => e.HasTags);
            entity.Ignore(e => e.IsSelected);
        });

        // ===== SqlTemplate → sql_template 表 =====
        modelBuilder.Entity<SqlTemplate>(entity =>
        {
            entity.ToTable("sql_template");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.TemplateName).HasColumnName("template_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.SqlContent).HasColumnName("sql_content").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(100);
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
            entity.Ignore(e => e.DisplayCreatedAt);
        });

        // ===== DatabaseExportLog → database_export_log 表 =====
        modelBuilder.Entity<DatabaseExportLog>(entity =>
        {
            entity.ToTable("database_export_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.ConnectionString).HasColumnName("connection_string").HasMaxLength(1000);
            entity.Property(e => e.SqlQuery).HasColumnName("sql_query").IsRequired();
            entity.Property(e => e.ExportMode).HasColumnName("export_mode").IsRequired().HasMaxLength(20);
            entity.Property(e => e.BatchSize).HasColumnName("batch_size");
            entity.Property(e => e.TotalRows).HasColumnName("total_rows");
            entity.Property(e => e.ExportTime).HasColumnName("export_time").IsRequired();
            entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(1000);
            entity.Property(e => e.FileIndex).HasColumnName("file_index");
            entity.Property(e => e.FileCount).HasColumnName("file_count");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
            entity.Ignore(e => e.StatusText);
            entity.Ignore(e => e.DisplayExportTime);
            entity.Ignore(e => e.DisplayCreatedAt);
            entity.Ignore(e => e.Summary);
            entity.Ignore(e => e.FileName);
            entity.Ignore(e => e.FileDirectory);
        });

        // ===== DatabaseExportConfig → database_export_config 表 =====
        modelBuilder.Entity<DatabaseExportConfig>(entity =>
        {
            entity.ToTable("database_export_config");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(12).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ConfigName).HasColumnName("config_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.ConnectionString).HasColumnName("connection_string").IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.CreatorOn).HasColumnName("creator_on");
            entity.Ignore(e => e.StatusText);
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

                -- ===== personal_task 用户ID列同步 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='user_id')
                BEGIN
                    ALTER TABLE personal_task ADD user_id NVARCHAR(100) NULL;
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
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_id')
                BEGIN
                    ALTER TABLE error_log ADD user_id NVARCHAR(100) NULL;
                    -- 清理旧列
                    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_name')
                        ALTER TABLE error_log DROP COLUMN user_name;
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

                -- ===== data_import_config 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='data_import_config')
                BEGIN
                   CREATE TABLE data_import_config (
                       id NVARCHAR(12) NOT NULL PRIMARY KEY,
                       config_name NVARCHAR(200) NOT NULL,
                       aml_content NVARCHAR(MAX) NOT NULL,
                        user_id NVARCHAR(100) NOT NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE data_import_config ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END

                -- 清理旧列
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='sheet_name')
                    ALTER TABLE data_import_config DROP COLUMN sheet_name;
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='start_row')
                    ALTER TABLE data_import_config DROP COLUMN start_row;
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='end_row')
                    ALTER TABLE data_import_config DROP COLUMN end_row;
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='start_col')
                    ALTER TABLE data_import_config DROP COLUMN start_col;
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='data_import_config' AND COLUMN_NAME='end_col')
                    ALTER TABLE data_import_config DROP COLUMN end_col;

                -- ===== text_translation_record 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='text_translation_record')
                BEGIN
                    CREATE TABLE text_translation_record (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        source_file_name NVARCHAR(500) NOT NULL,
                        output_file_name NVARCHAR(500) NULL,
                        output_file_path NVARCHAR(1000) NULL,
                        template_type NVARCHAR(50) NULL,
                        source_language NVARCHAR(50) NULL,
                        source_row_count INT NOT NULL DEFAULT 0,
                        batch_count INT NOT NULL DEFAULT 0,
                        user_id NVARCHAR(100) NULL,
                        ai_model_id NVARCHAR(12) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='text_translation_record' AND COLUMN_NAME='source_language')
                        ALTER TABLE text_translation_record ADD source_language NVARCHAR(50) NULL;
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='text_translation_record' AND COLUMN_NAME='ai_model_id')
                        ALTER TABLE text_translation_record ADD ai_model_id NVARCHAR(12) NULL;
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='text_translation_record' AND COLUMN_NAME='user_id')
                        ALTER TABLE text_translation_record ADD user_id NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='text_translation_record' AND COLUMN_NAME='creator_on')
                        ALTER TABLE text_translation_record ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END

                -- ===== ai_model_config 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ai_model_config')
                BEGIN
                    CREATE TABLE ai_model_config (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        model_name NVARCHAR(200) NOT NULL,
                        api_key NVARCHAR(500) NULL,
                        api_base_url NVARCHAR(500) NULL DEFAULT 'https://apihub.agnes-ai.com/v1/chat/completions',
                        model_identifier NVARCHAR(100) NULL DEFAULT 'agnes-2.0-flash',
                        extra_params NVARCHAR(MAX) NULL,
                        is_enabled BIT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ai_model_config' AND COLUMN_NAME='creator_on')
                        ALTER TABLE ai_model_config ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ai_model_config' AND COLUMN_NAME='extra_params')
                        ALTER TABLE ai_model_config ADD extra_params NVARCHAR(MAX) NULL;
                END

                -- ===== object_class_import_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='object_class_import_log')
                BEGIN
                    CREATE TABLE object_class_import_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        import_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        import_file NVARCHAR(500) NOT NULL,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_log NVARCHAR(MAX) NULL,
                        sheet1_count INT NOT NULL DEFAULT 0,
                        sheet2_count INT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                -- ===== list_import_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='list_import_log')
                BEGIN
                    CREATE TABLE list_import_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        import_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        import_file NVARCHAR(500) NOT NULL,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_log NVARCHAR(MAX) NULL,
                        sheet1_count INT NOT NULL DEFAULT 0,
                        sheet2_count INT NOT NULL DEFAULT 0,
                        sheet3_count INT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                -- ===== aras_login_config 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='aras_login_config')
                BEGIN
                    CREATE TABLE aras_login_config (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        url NVARCHAR(500) NOT NULL,
                        database_name NVARCHAR(200) NOT NULL,
                        username NVARCHAR(100) NOT NULL,
                        md5_password NVARCHAR(32) NULL,
                        is_enabled BIT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE(),
                        user_id NVARCHAR(100) NOT NULL
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='aras_login_config' AND COLUMN_NAME='creator_on')
                        ALTER TABLE aras_login_config ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='aras_login_config' AND COLUMN_NAME='user_id')
                        ALTER TABLE aras_login_config ADD user_id NVARCHAR(100) NULL;
                END

                -- ===== property_import_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='property_import_log')
                BEGIN
                    CREATE TABLE property_import_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        import_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        import_file NVARCHAR(500) NOT NULL,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_log NVARCHAR(MAX) NULL,
                        sheet1_count INT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                -- ===== permission_import_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='permission_import_log')
                BEGIN
                    CREATE TABLE permission_import_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        import_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        import_file NVARCHAR(500) NOT NULL,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_log NVARCHAR(MAX) NULL,
                        sheet1_count INT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                -- ===== lifecycle_import_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='lifecycle_import_log')
                BEGIN
                    CREATE TABLE lifecycle_import_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        import_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        import_file NVARCHAR(500) NOT NULL,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_log NVARCHAR(MAX) NULL,
                        sheet1_count INT NOT NULL DEFAULT 0,
                        sheet2_count INT NOT NULL DEFAULT 0,
                        sheet3_count INT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='lifecycle_import_log' AND COLUMN_NAME='sheet2_count')
                        ALTER TABLE lifecycle_import_log ADD sheet2_count INT NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='lifecycle_import_log' AND COLUMN_NAME='sheet3_count')
                        ALTER TABLE lifecycle_import_log ADD sheet3_count INT NOT NULL DEFAULT 0;
                END

                -- ===== knowledge_entry 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='knowledge_entry')
                BEGIN
                    CREATE TABLE knowledge_entry (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        title NVARCHAR(500) NOT NULL,
                        content NVARCHAR(MAX) NULL,
                        plain_text_preview NVARCHAR(500) NULL,
                        tags NVARCHAR(500) NULL,
                        category NVARCHAR(200) NULL,
                        pinned BIT NOT NULL DEFAULT 0,
                        created_date DATETIME2 NOT NULL DEFAULT GETDATE(),
                        modified_date DATETIME2 NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE(),
                        user_id NVARCHAR(100) NULL
                    );
                END

                -- ===== sql_template 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='sql_template')
                BEGIN
                    CREATE TABLE sql_template (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        template_name NVARCHAR(200) NOT NULL,
                        sql_content NVARCHAR(MAX) NOT NULL,
                        description NVARCHAR(500) NULL,
                        user_id NVARCHAR(100) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='sql_template' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE sql_template ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END

                -- ===== database_export_log 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='database_export_log')
                BEGIN
                    CREATE TABLE database_export_log (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        connection_string NVARCHAR(1000) NULL,
                        sql_query NVARCHAR(MAX) NOT NULL,
                        export_mode NVARCHAR(20) NOT NULL DEFAULT N'一次导出',
                        batch_size INT NOT NULL DEFAULT 500,
                        total_rows INT NOT NULL DEFAULT 0,
                        export_time DATETIME2 NOT NULL DEFAULT GETDATE(),
                        file_path NVARCHAR(1000) NULL,
                        file_index INT NOT NULL DEFAULT 1,
                        file_count INT NOT NULL DEFAULT 1,
                        status NVARCHAR(20) NOT NULL DEFAULT 'Success',
                        error_message NVARCHAR(MAX) NULL,
                        user_id NVARCHAR(100) NOT NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='database_export_log' AND COLUMN_NAME='creator_on')
                        ALTER TABLE database_export_log ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='database_export_log' AND COLUMN_NAME='file_index')
                        ALTER TABLE database_export_log ADD file_index INT NOT NULL DEFAULT 1;
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='database_export_log' AND COLUMN_NAME='file_count')
                        ALTER TABLE database_export_log ADD file_count INT NOT NULL DEFAULT 1;
                END

                -- ===== database_export_config 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='database_export_config')
                BEGIN
                    CREATE TABLE database_export_config (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        user_id NVARCHAR(100) NOT NULL,
                        config_name NVARCHAR(200) NOT NULL,
                        connection_string NVARCHAR(1000) NOT NULL,
                        remark NVARCHAR(500) NULL,
                        is_enabled BIT NOT NULL DEFAULT 0,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='database_export_config' AND COLUMN_NAME='creator_on')
                BEGIN
                    ALTER TABLE database_export_config ADD creator_on DATETIME2 NOT NULL DEFAULT GETDATE();
                END
            
                -- ===== translation_task 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='translation_task')
                BEGIN
                    CREATE TABLE translation_task (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        task_name NVARCHAR(200) NOT NULL,
                        query_mode NVARCHAR(50) NULL,
                        query_condition NVARCHAR(1000) NULL,
                        source_language NVARCHAR(50) NULL,
                        target_languages NVARCHAR(200) NULL,
                        total_fields INT NOT NULL DEFAULT 0,
                        translated_fields INT NOT NULL DEFAULT 0,
                        progress_text NVARCHAR(100) NULL,
                        status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
                        output_file_path NVARCHAR(1000) NULL,
                        user_id NVARCHAR(100) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
                END

                -- ===== translation_record 表 =====
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='translation_record')
                BEGIN
                    CREATE TABLE translation_record (
                        id NVARCHAR(12) NOT NULL PRIMARY KEY,
                        task_id NVARCHAR(12) NOT NULL,
                        field_id NVARCHAR(50) NULL,
                        field_name NVARCHAR(200) NULL,
                        original_label NVARCHAR(500) NULL,
                        translated_label NVARCHAR(500) NULL,
                        target_language NVARCHAR(50) NULL,
                        creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
                    );
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


