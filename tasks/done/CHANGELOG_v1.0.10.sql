-- =====================================================
-- Aras 开发工具箱 更新日志 SQL
-- 生成日期: 2026-07-01
-- 当前批次版本: v1.0.9 → v1.0.10
-- 包含提交: dd5f730 → 3a2d7d7 (2 commits)
-- =====================================================

-- ===== v1.0.10: 对象类配置导入优化 + List配置功能 =====

-- 1. 对象类配置导入优化 (dd5f730)
--    实时进度显示 + 模板减列 + CancellationToken + Mode=OneWay修复
INSERT INTO changelog (id, version, release_date, type, description, author, creator_on)
VALUES (NEWID(), '1.0.10', '2026-07-01', '优化',
'[对象类配置] 导入全面优化：(1) 新增 ImportProgressInfo 结构化进度模型，支持百分比+阶段+当前条目实时显示 (2) Task.Run 后台执行避免 UI 卡顿 (3) 新增取消导入按钮 + CancellationToken 支持随时终止 (4) 模板 Sheet2 移除"必须""自动搜索"默认值列（12→10列）(5) 完善全部 AML 字段中文注释 (6) 修复 WPF Run 元素不继承 DataContext 导致 SetPropertyInfo 报错 (7) 修复 ProgressBar.Value 只读属性 TwoWay 绑定异常（Mode=OneWay）',
'开发团队', '2026-07-01');

-- 2. List配置导入功能 (3a2d7d7)
--    完整 List 导入功能 + 模板淡橙色表头
INSERT INTO changelog (id, version, release_date, type, description, author, creator_on)
VALUES (NEWID(), '1.0.10', '2026-07-01', '新增',
'[List配置] 全新功能：(1) 3个Sheet模板批量导入 — List主档(2列)/菜单内容(6列)/菜单内容过滤(7列含过滤值) (2) 导入顺序串行执行: 主档→菜单→过滤 (3) 实时进度+取消+分页历史记录 与对象类配置一致 (4) list_import_log 表自动建表 (5) 仪表盘+菜单+导航全入口注册 (6) 所有模板表头统一淡橙色背景(#FDEBD0)区分数据区',
'开发团队', '2026-07-01');
