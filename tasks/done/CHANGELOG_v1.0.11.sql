-- =====================================================
-- Aras 开发工具箱 更新日志 SQL
-- 生成日期: 2026-07-01
-- 当前批次版本: v1.0.10 → v1.0.11
-- 包含提交: 4项优化调整
-- =====================================================

-- ===== v1.0.11: Aras登录DB化 + 登录UI重构 + 翻译进度对齐 + UI微调 =====

-- 1. Aras登录信息 JSON → 数据库存储迁移
--    新增 aras_login_config 表 + IsEnabled 自动连接
INSERT INTO changelog (id, version, release_date, type, description, author, creator_on)
VALUES (NEWID(), '1.0.11', '2026-07-01', '新增',
'[Aras登录配置] JSON→数据库存储迁移：(1) 新增 aras_login_config 表替代本地JSON文件，与 AI 模型配置存储模式一致 (2) 新增 IsEnabled 启用字段 — 启用后应用启动自动完成 Innovator 依赖注入和连接池初始化 (3) 首次启动自动检测旧JSON文件并迁移到DB（明文密码→MD5 32位小写）(4) 切换连接时自动启用目标配置并禁用其他 (5) 新增 IArasLoginConfigService 服务 + 操作日志记录',
'开发团队', '2026-07-01');

-- 2. Aras登录UI重构
--    字段标签 + 编辑模式 + MD5密码加密 + 启用状态指示
INSERT INTO changelog (id, version, release_date, type, description, author, creator_on)
VALUES (NEWID(), '1.0.11', '2026-07-01', '优化',
'[Aras登录UI] 全面重做：(1) 表单区域增加 URL/数据库/用户名/密码 标签提升操作体验 (2) 密码保存时自动转为32位小写MD5加密存储，密码框显示*号掩码 (3) 编辑模式下密码框显示占位符••••••••，未修改则保留原哈希 (4) 卡片增加绿色"✓ 已启用"标记和"编辑"按钮 (5) LoginService 增加 IsPasswordHashed 判断避免DB加载时二次哈希 (6) 应用登录成功后自动连接已启用的 Aras 配置',
'开发团队', '2026-07-01');

-- 3. 翻译进度显示对齐对象配置UI
--    结构化进度 + 进度条 + 百分比 + 取消按钮
INSERT INTO changelog (id, version, release_date, type, description, author, creator_on)
VALUES (NEWID(), '1.0.11', '2026-07-01', '优化',
'[文本翻译] 进度显示全面升级对齐对象类配置UI：(1) 新增 TranslationProgressInfo 结构化进度模型 — 阶段/批次/百分比/状态文本 (2) 翻译卡片增加绿色进度卡片(#F0FDF4)+确定型ProgressBar(百分比)+✕取消按钮 (3) 支持 CancellationToken 随时终止翻译 (4) IProgress<string> 升级为 IProgress<TranslationProgressInfo> 报告结构化数据 (5) 源文件选择框缩窄+高度统一30px消除割裂感 (6) 开始翻译按钮移至浏览按钮右侧同色自然成组',
'开发团队', '2026-07-01');

-- ===== DB Schema 更新（仅首次执行） =====

-- 新增 aras_login_config 表
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
