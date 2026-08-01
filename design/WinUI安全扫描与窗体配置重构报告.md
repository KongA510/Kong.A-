# WinUI 安全扫描与窗体配置重构报告

日期：2026-08-01
范围：`ArasToolkit.App.WinUI`、共享 `Core` / `Services` 层，以及为保证双端兼容而受影响的 WPF 构建链路。

## 已处理问题

| 级别 | 问题 | 处理结果 |
|---|---|---|
| 高 | `System.Text.Json 8.0.0` 命中两个高危拒绝服务漏洞 | 在 Services 层固定到 `8.0.6`，恢复所有项目的 NuGet Audit；复扫结果为 0 个已知漏洞。 |
| 严重 | 数据库连接串及口令被提交到 Git | 跟踪文件改为无密钥模板；原本机值迁移到被 `.gitignore` 排除的 `DBSeeting.local.json`；运行时优先读取环境变量 `ARAS_TOOLKIT_DB_CONNECTION`。 |
| 严重 | `EnsureSchemaAsync` 会把两个账户口令反复重置为代码内置值 | 删除固定口令与固定人员账户逻辑；仅在显式设置 `ARAS_TOOLKIT_BOOTSTRAP_ADMIN_PASSWORD` 时允许首次引导管理员。 |
| 高 | AI 模型启用配置查询未按 `userId` 过滤 | 增加用户隔离条件，避免读取到其他用户启用的 API 配置。 |
| 高 | “数据库导出”实际可执行 UPDATE/DELETE/DDL 等任意 SQL | 增加只读词法校验，仅允许 `SELECT` / `WITH` 查询，并阻止 DML、DDL、`SELECT INTO`、外部查询等关键字。 |
| 高 | 用户输入直接拼接 AML 可能产生 XML/AML 注入 | 新窗体配置模块统一使用 `XElement` 构造 AML，字段名、标签、对象类及窗体名称均经过 XML 编码。 |
| 中 | WinUI 未处理异常只输出到 Debug | 接入 `IErrorLogService`，按 P0 写入错误日志，同时保留原异常行为。 |
| 中 | 部分页面初始化时自动执行数据库 DDL | 移除数据库导出页和配置页启动时的 `EnsureSchemaAsync`；结构检查保留在用户主动触发的设置功能中。 |

## Aras 窗体配置设计

- 对象类属性通过 `Property.is_hidden2 = 0` 抓取，即“搜索中隐藏”未勾选的字段。
- 默认全部选中，允许在写入前全选、清空或逐项调整。
- 固定优先顺序：`created_by_id`（创建者）、`created_on`（创建时间）、`state`（状态）、`item_number`（编号）；存在时严格位于最前。
- 坐标：起点 `(50, 50)`，每行四项，X 步长 200，Y 步长 50。
- `item` 类型显示长度 135；文本及其他默认控件显示长度 150。
- 根据数据类型映射 `item`、`dropdown`、`checkbox`、`date`、`text` 控件。
- 默认禁止无提示覆盖同名窗体；覆盖、默认 View 关联及写入前确认均由用户显式控制。
- 覆盖窗体时将尺寸更新、旧字段删除和新字段写入合并到同一 AML 请求，降低中间状态风险。

## UI 重构范围

- 主窗口采用 Mica、透明内容层、自适应 NavigationView 和 Fluent 状态栏。
- 全局色板改为 Light / Dark 主题字典，继续使用 Windows 系统强调色。
- 统一页面标题、副标题、分区标题、说明文字、按钮、卡片、圆角和表格行样式。
- 仪表盘改为自适应连接卡片、用户卡片和三列功能卡片。
- “系统翻译 / 系统配置 / 系统日志”父节点改为可点击子仪表盘，不再显示通用迁移占位提示。
- 新增窗体配置原生 WinUI 页面，包含规则提示、对象类选择、属性选择、坐标预览、进度和确认流程。

## 后续建议

1. 立即轮换曾进入 Git 历史的数据库账号口令，并使用 `git filter-repo` 清理远端历史；本次仅能保证新提交不再包含密钥。
2. 应用用户密码目前受项目兼容规范约束仍为 MD5。建议设计 PBKDF2/Argon2 的渐进迁移：登录时兼容旧哈希，校验成功后升级哈希格式。
3. `ConfigService` 的旧登录 JSON 使用 Base64，属于编码而非加密。建议改用 Windows Credential Locker 或 DPAPI，并保留一次性旧格式迁移。
4. AI API Key 当前仍保存在共享数据库明文字段中。建议引入每用户/每部署密钥保护，并让列表接口只返回掩码值。
5. 现有对象类、属性、List、权限和生命周期导入服务仍有字符串拼接 AML。建议分模块迁移为 `XElement` 或 IOM Item API，每次迁移后用真实 Aras 测试库回归。
6. 数据库导出应继续使用只读 SQL 账号；应用层只读校验是纵深防御，不能替代数据库最小权限。
7. 为窗体 AML 的实际字段属性名、View 类型和覆盖行为增加 R37 测试库集成测试；当前本地验证覆盖编译、XBF 生成和纯规则检查，不应直接在生产 Aras 上首次试用。

## 验证记录

- WinUI 项目：编译成功，0 警告、0 错误。
- WPF 项目：编译成功，0 错误；保留 4 个既有 `CS1998` 警告。
- NuGet 漏洞复扫：WinUI、WPF、Services 均为 0 个已知漏洞。
- XAML：`MainWindow.xbf`、`DashboardPage.xbf`、`FormConfigurationPage.xbf`、`PlaceholderPage.xbf` 均成功生成。
