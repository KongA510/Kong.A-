# Foreign 属性汇入修复验证

日期：2026-09-16。更新日志：1.0.70（ID 20091，已通过 IChangelogService 写入并回读确认操作日志）。

## 修复规则

Foreign 的数据源是当前对象类的 Item 属性；引用外部属性是该 Item 属性指向的目标对象类字段，与 [Aras 官方说明](https://aras.com/en/blog/aras-fundamentals-advanced-properties) 一致。

1. 先查系统中对应 Property，校验其所属对象类。
2. 系统不存在时，使用本次模板前面已经通过校验、已完成 AML 组装的属性进行临时解析。依赖必须放在 Foreign 前面，不限于紧邻上一行。
3. 校验源类型为 Item 且 ItemType 数据源有效，再校验目标字段。目标是当前对象类时，也支持前面的待新增目标字段。
4. 给被引用的待新增属性预分配 GUID，将其 add AML 的 id 和 Foreign 引用写成同一值。预检不写入 Aras。
5. 导入重新预检并按 Excel 顺序提交。前置依赖失败时不发送该 Foreign 请求，记录行号及失败原因，独立行继续执行。
6. 前面安排覆盖的 Item 属性按本次覆盖后的数据源校验。权限错误、重复匹配、无效类型、错误对象类和缺失外部字段仍拦截。

模板填写示例：

| 顺序 | 名称 | 数据类型 | 数据源 | 引用外部属性 |
| --- | --- | --- | --- | --- |
| 1 | linked_part | Item | Part | |
| 2 | linked_number | Foreign | linked_part | item_number |

## 本地验证

执行 `dotnet run --project tests/ArasToolkit.PropertyImport.RegressionTests --no-restore`：53 项通过，包括新增的 19 项 Foreign 场景和原有 34 项属性、多语言、取消及保存场景。

执行 `dotnet build src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj --no-restore -v:minimal`：成功，0 警告、0 错误。已检查 PropertyConfigPage.xbf 存在。回归运行的 NuGet 漏洞源查询出现 NU1900（句柄无效），不影响编译和测试结果；漏洞审计未据此判为通过。

## BLPLM 实测

从工具箱保存的 BLPLM 连接配置创建并租用 ArasConnectionPool 连接，使用真实 R37 IOM 及正式 PropertyImportService。测试对象为独立临时 ItemType `z_foreign_verify_0916103435`，ID `C7CFFE32489E4D31A3FA6A487131AB5E`。

| 检查 | 结果 |
| --- | --- |
| 汇入前确认源 Item 属性尚不存在 | 通过 |
| 一条待新增 Item、两条 Foreign 完成预检 | 通过 |
| 预检后再次查询，确认没有提前创建属性 | 通过 |
| 两条 Foreign 共用源属性预分配 ID | 通过 |
| 三条属性真实新增及对象类保存 | 通过 |
| 回读 Foreign.data_source 等于已创建 Item 属性 ID | 两条均通过 |
| 回读 Foreign.foreign_property 等于 Part.item_number 的 Property ID | 两条均通过 |
| 单独 Foreign 使用系统已有源属性 | 通过 |
| 待新增源放在 Foreign 后面 | 正确拦截并提示调整行顺序 |
| 不存在的外部字段 | 正确拦截 |
| 自引用：源 Item 和目标 String 均在前面待新增 | 三条真实汇入及保存通过 |
| 原三条记录以覆盖模式再次汇入 | 三条覆盖及保存通过 |
| 再次普通保存对象类 | 通过 |
| 清理临时 Foreign、Item、String 属性及 ItemType | 完成并确认不存在 |

第一次验证的临时对象也已确认清理；最终运行正常退出。测试创建、导入和删除均记录操作日志。

本地原始证据保存在 `.codex/foreign-property-validation-20260916/evidence/`：`checks.json`、`preview.json`、`import.json`、`readback.xml` 及测试模板。凭据不进入报告和 Git 提交。

## 修改范围

- `PropertyImportService`：前置属性解析、临时引用 ID、依赖提交检查、模板说明。
- `PropertyImportPreviewRow`：新增属性预分配 ID 和依赖行号。
- 属性汇入回归项目及本验证记录。

WinUI 原有预览、进度和汇入入口继续使用该服务；未修改 WPF 页面、ViewModel 或数据库表映射。
