-- ===== Changelog v1.0.7 =====

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.7', '2026-06-25', N'新增', N'数据汇入模块: AML模板配置弹窗保存+选择配置功能, 新增TextPromptWindow输入弹窗', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.7', '2026-06-25', N'优化', N'数据汇入: 精简data_import_config表至5列(id/config_name/aml_content/user_id/creator_on), 移除多余的Sheet/行列范围字段', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.7', '2026-06-25', N'修复', N'数据汇入: 修复5个关联Bug(保存配置无异常处理/删除配置参数忽略/选择配置后SelectedConfig未回写/数据源不一致/GetConfigsAsync错误吞没) — TASK-023', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.7', '2026-06-25', N'修复', N'数据汇入: 修复Excel列头含括号时WPF PropertyPath解析崩溃(SanitizeHeader方法) — TASK-024', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.7', '2026-06-25', N'优化', N'数据汇入: user_name改为user_id确保用户标识稳定性, SavedConfigs改为完整属性支持实时刷新', N'开发团队', GETDATE());

