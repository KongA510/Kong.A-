-- ===== Changelog v1.0.8 =====

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'新增', N'数据汇入并发化: ArasConnectionPool连接池(ConcurrentBag+Rent/Return), Parallel.ForEachAsync多线程执行, CancellationToken真实暂停/继续 — TASK-028', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'新增', N'Aras登录信息管理: ＋新增按钮+可折叠表单保存登录记录, IArasConnectionPool接口, SetConnection预Logout, 连接后Reinitialize(10)重建池 — TASK-029', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'新增', N'DataImportService内部集成Aras API: innovator.applyAML()真实执行AML, 回调仅汇报进度, 解锁进度条逐行更新 — TASK-027', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'修复', N'ReplaceAmlPlaceholders反转义AML模板中的\\n\\t\\r文字反斜杠序列', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'修复', N'ChangelogViewModel补齐6个绑定属性(CurrentVersion/NewCount/FixCount/OptimizeCount/FilterTypes/FilterType) — TASK-025', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'修复', N'StringToVisibilityConverter支持ConverterParameter=Invert和非字符串null检查 — TASK-026', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'修复', N'全局UTF-8编码修复: 剥离49个文件BOM, 移除3个幽灵文件索引', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'优化', N'bin目录整理: R37lib DLL(188个)移至R37lib子目录, App.xaml.cs添加AssemblyResolve探测, 根目录从220减至135个文件', N'开发团队', GETDATE());

INSERT INTO changelog (version, release_date, type, description, author, creator_on)
VALUES ('1.0.8', '2026-06-26', N'修复', N'ArasConnectionService.HttpConnection显式接口实现+SemaphoreSlim异步锁+LoginService使用Login().getInnovator()标准登录链', N'开发团队', GETDATE());
