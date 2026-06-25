-- ===== Fix: user_id migration for error_log and personal_task =====
-- Execute this SQL against Kong.A database

-- 1. error_log: add user_id, fill with known user ID, drop user_name
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_id')
BEGIN
    ALTER TABLE error_log ADD user_id NVARCHAR(100) NULL;
    PRINT 'Added user_id column to error_log';
END

-- Fill all NULL user_id with known user
UPDATE error_log SET user_id = 'ae7cd0e8f4e8' WHERE user_id IS NULL;

-- Drop old user_name column
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_name')
BEGIN
    ALTER TABLE error_log DROP COLUMN user_name;
    PRINT 'Dropped user_name column from error_log';
END

-- 2. personal_task: add user_id, fill with known user ID
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='personal_task' AND COLUMN_NAME='user_id')
BEGIN
    ALTER TABLE personal_task ADD user_id NVARCHAR(100) NULL;
    PRINT 'Added user_id column to personal_task';
END

-- Fill all NULL user_id with known user
UPDATE personal_task SET user_id = 'ae7cd0e8f4e8' WHERE user_id IS NULL;

-- Also update created_by to use user_id pattern for consistency
UPDATE personal_task SET created_by = 'ae7cd0e8f4e8' WHERE created_by IS NOT NULL AND created_by <> '';

PRINT 'Migration complete: error_log + personal_task user_id columns added and populated.';
