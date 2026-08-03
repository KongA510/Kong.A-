/*
    用途：修复 dbo.app_user 缺失或失效的 role / is_active 列。
    特性：幂等、保留现有用户数据、失败自动回滚。
    执行：在 SSMS 中选中工具箱所用数据库后，整段执行。
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.app_user', N'U') IS NULL
        THROW 51000, N'修复中止：dbo.app_user 表不存在。', 1;

    IF COL_LENGTH(N'dbo.app_user', N'is_admin') IS NULL
        THROW 51001, N'修复中止：dbo.app_user.is_admin 基础列不存在。', 1;

    -- 1. role：先以可空列加入，根据旧 is_admin 数据回填，再收紧为 NOT NULL。
    IF COL_LENGTH(N'dbo.app_user', N'role') IS NULL
        ALTER TABLE dbo.app_user ADD role NVARCHAR(50) NULL;

    EXEC sys.sp_executesql N'
        UPDATE dbo.app_user
        SET role = CASE WHEN is_admin = 1 THEN N''Admin'' ELSE N''User'' END
        WHERE role IS NULL OR LTRIM(RTRIM(role)) = N'''';

        UPDATE dbo.app_user
        SET role = N''Admin''
        WHERE is_admin = 1 AND role <> N''Admin'';

        ALTER TABLE dbo.app_user ALTER COLUMN role NVARCHAR(50) NOT NULL;';

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.object_id = dc.parent_object_id
           AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.app_user')
          AND c.name = N'role'
    )
        EXEC sys.sp_executesql N'
            ALTER TABLE dbo.app_user
                ADD CONSTRAINT DF_app_user_role DEFAULT N''User'' FOR role;';

    -- 2. is_active：旧用户默认为启用，避免修复后全部无法登录。
    IF COL_LENGTH(N'dbo.app_user', N'is_active') IS NULL
        ALTER TABLE dbo.app_user ADD is_active BIT NULL;

    EXEC sys.sp_executesql N'
        UPDATE dbo.app_user
        SET is_active = 1
        WHERE is_active IS NULL;

        ALTER TABLE dbo.app_user ALTER COLUMN is_active BIT NOT NULL;';

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.object_id = dc.parent_object_id
           AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.app_user')
          AND c.name = N'is_active'
    )
        EXEC sys.sp_executesql N'
            ALTER TABLE dbo.app_user
                ADD CONSTRAINT DF_app_user_is_active DEFAULT 1 FOR is_active;';

    -- 3. 保持新 role 与旧 is_admin 标识一致。
    EXEC sys.sp_executesql N'
        UPDATE dbo.app_user
        SET is_admin = 1
        WHERE role = N''Admin'' AND is_admin = 0;';

    COMMIT TRANSACTION;

    -- 验证结果：不输出密码等敏感数据。
    SELECT
        c.name AS column_name,
        t.name AS data_type,
        c.max_length,
        c.is_nullable,
        dc.definition AS default_value
    FROM sys.columns c
    INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
    LEFT JOIN sys.default_constraints dc
        ON dc.parent_object_id = c.object_id
       AND dc.parent_column_id = c.column_id
    WHERE c.object_id = OBJECT_ID(N'dbo.app_user')
      AND c.name IN (N'role', N'is_active')
    ORDER BY c.column_id;

    EXEC sys.sp_executesql N'
        SELECT role, is_active, COUNT_BIG(*) AS user_count
        FROM dbo.app_user
        GROUP BY role, is_active
        ORDER BY role, is_active;';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
