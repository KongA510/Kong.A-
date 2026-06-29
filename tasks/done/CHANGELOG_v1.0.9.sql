-- =====================================================
-- Aras 开发工具箱 更新日志 SQL
-- 生成日期: 2026-06-30
-- 当前批次版本: v1.0.8 → v1.0.9
-- 包含提交: 8575d9b → d6e0130 (5 commits)
-- =====================================================

-- ===== v1.0.9: 对象类配置功能 + 文件资源管理器 + 报表修复 =====

-- 1. 文件资源管理器 (8575d9b)
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'新增', N'我的资料文件资源管理器: 懒加载树形导航+模糊搜索+文件上传+系统资源管理器定位', N'开发团队', GETDATE());

-- 2. 对象类配置导入 (15211b4)
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'新增', N'对象类配置功能: Excel模板下载(对象类+关系类双Sheet)+批量导入Aras+EF实体映射+敏感操作日志+新增/覆盖模式切换', N'开发团队', GETDATE());

-- 3. 对象类配置 AML 多语系汇入
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'新增', N'对象类配置: Aras AML批量汇入(itemType action=add/merge+where动态匹配), 导入日志按日期分目录(Config/ObjectClassImports/yyyy_M_d/logs+uploads隔离)', N'开发团队', GETDATE());

-- 4. 对象类配置 UI 完善 (d6e0130)
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'新增', N'对象类配置UI: 导入进度实时显示+失败日志详情+查看日志按钮+历史记录打开文件按钮+DataGrid选中行浅灰背景', N'开发团队', GETDATE());

-- 5. 文本翻译+对象类配置 历史分页 (c4c08ff)
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'优化', N'文本翻译+对象类配置: 历史记录分页功能，修复分页控件居中问题', N'开发团队', GETDATE());

-- 6. 数据报表修复 (c357c38)
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'修复', N'数据报表竖条图切换: BarSeries→LinearBarSeries修复OxyPlot渲染异常', N'开发团队', GETDATE());

-- 7. 版本号动态读取
INSERT INTO changelog (version, release_date, type, description, author, creator_on) VALUES
('1.0.9', '2026-06-30', N'修复', N'左下角版本号动态读取: MainViewModel注入IChangelogService, VersionText从数据库获取最新版本而非写死v1.0', N'开发团队', GETDATE());
