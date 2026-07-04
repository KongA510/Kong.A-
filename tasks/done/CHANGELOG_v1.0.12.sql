-- =====================================================
-- Aras 开发工具箱 更新日志 SQL
-- 生成日期: 2026-07-03
-- 当前批次版本: v1.0.11 → v1.0.12
-- 包含提交: 3项修复/优化
-- =====================================================

-- ===== v1.0.12: DbContext补充列映射 + List导入AML修正 + 覆盖逻辑缓存优化 =====

-- 1. DbContext extra_params 列映射补充
INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.12', '2026-07-03', '修复',
'[AI调度层] DbContext 补充 extra_params 列映射：(1) OnModelCreating 中新增 ai_model_config.extra_params → ExtraParams 映射 (2) EnsureSchemaAsync 同步增加 extra_params 列检测与 ALTER TABLE 逻辑 (3) 修复因缺少列映射导致 ExtraParams 属性读写数据库失败的问题',
'开发团队', '2026-07-03');

-- 2. List导入AML标签格式修正 + 列索引偏移
INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.12', '2026-07-03', '修复',
'[List配置导入] AML 标签格式修正 + 列索引起始偏移：(1) <label> 改为 xml:lang="en" 承载英文标签 (2) 新增 xml:lang="zc" 的 i18n:label 承载简体中文 (3) 保留 xml:lang="zt" 的 i18n:label 承载繁体中文 (4) 菜单内容/过滤菜单列索引从 0-based 修正为 1-based（父阶名称从 col0→col1），与 Excel 模板列号对齐',
'开发团队', '2026-07-03');

-- 3. List导入覆盖逻辑 source_id 精确匹配 + 缓存池优化
INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.12', '2026-07-03', '优化',
'[List配置导入] 覆盖模式 where 条件精确化 + 查询缓存池：(1) 菜单内容 merge 条件从 Value.value 单字段扩展为 value + source_id 双字段组合匹配，避免不同 List 下同名 value 覆盖到错误条目 (2) 新增 ResolveListId 方法通过 List.name 查询 id (3) 新增 Dictionary<string,string> 缓存池，同名父阶只查一次 Aras 服务器，免去重复 API 调用 (4) List主档 Sheet 跳过 source_id 查询，无需解析父阶关系',
'开发团队', '2026-07-03');
