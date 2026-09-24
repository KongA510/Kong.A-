# 属性汇入与多语言回归验证

在仓库根目录执行：

```powershell
dotnet run --project tests/ArasToolkit.PropertyImport.RegressionTests/ArasToolkit.PropertyImport.RegressionTests.csproj
```

使用项目自带的 R37 IOM 解析模拟 SOAP 响应，并调用实际 `PropertyImportService.PrepareAsync` 和 `ImportAsync`。IOM 传输与 EF `SaveChangesAsync` 均由测试替身接管，不连接 Aras 或工具箱数据库，不提交真实属性变更；临时模板在验证后清理，导入归档与日志仅保留在测试输出目录。

覆盖模式按 `source_id + name` 查询：0 条生成 `add`，1 条生成带已有 ID 的 `edit`，多条阻止提交。新增模式遇到已有属性仍报告名称冲突。

53 个场景覆盖：

- 两种模式下的标准空结果、本地化空结果、空 Result、已有属性、真实错误、错误消息相同但错误码不同、重复属性，以及混合新增/覆盖和缺失数据源引用。验证预览状态、校验数量、AML action/ID/目标对象类和错误日志行为。
- 属性三语标签、默认值、提示从 Excel 表头到预览模型、AML 的完整映射，包含列顺序调整、表头空白、XML 特殊字符和新增/覆盖两条路径。
- 对象类的三语名称、TOC 显示文字，以及关系类三语页签在新增/覆盖模式中的语言对应与命名空间。
- 导入后的对象类空 edit：全部新增、全部覆盖、部分行失败、全部行失败、提交前取消、提交一行后取消、保存失败、取消且保存失败、保存期间收到取消、预检失败。检查请求顺序、目标 ID、无字段负载、执行次数、逐行进度、取消后的未提交状态以及历史和操作日志。
- Foreign 的 19 个场景：两种模式的前置待新增 Item、多个 Foreign 共用源 ID、系统已有源（名称/GUID）、源在后面、源缺失、非 Item 源、无效或重复前置行、外部字段缺失、自引用前置目标字段、真实查询错误、GUID 所属对象类错误、系统重复属性、前置覆盖后的数据源，以及实际提交时依赖成功/失败。

## Foreign 校验与汇入顺序

Foreign 的“数据源”填写当前对象类的 Item 属性，“引用外部属性”填写该 Item 属性所指对象类的字段。此关系与 [Aras 官方属性说明](https://aras.com/en/blog/aras-fundamentals-advanced-properties) 一致。

先查询系统属性；不存在时，只从本模板前面已通过校验和 AML 组装的行解析。必须把依赖属性放在 Foreign 之前；不接受后面的待新增行，也不把权限错误或重复匹配当成不存在。若源属性在前面被覆盖，按覆盖后的 ItemType 数据源校验目标字段。

例如先定义 `linked_part / Item / Part`，再定义 `linked_number / Foreign / linked_part / item_number`。预检给待新增的 `linked_part` 分配 GUID，同时更新该行的 add AML 和 Foreign 的 data_source，预检阶段不写入 Aras。正式汇入重新预检，按 Excel 行顺序提交；依赖行失败时不发送对应 Foreign 请求，独立行仍可继续。自引用当前对象类时，前面待新增的目标字段也按相同规则处理。

2026-09-16 在连接池 BLPLM 上使用独立临时 ItemType 实测：待新增源及多个 Foreign 汇入、引用 ID 回读、已有源预检、错误顺序与缺失目标拦截、自引用待新增目标、覆盖汇入和再次保存。临时属性及对象类验证后删除。详见 `output/property-foreign-validation-20260916/report.md`。

## 对象类保存与界面进度

只要本次有属性成功提交，结束时就在同一 Aras 连接上执行一次 `<Item type="ItemType" action="edit" id="目标ID" />`。此步骤不携带字段和关系；部分失败或取消后也执行，确保已提交属性完成对象类保存。预检失败或零条成功时不执行。对象类保存失败会明确显示错误，不会报告整体成功。

总进度为“属性行数 + 一次对象类保存”，开始请求不会提前增加完成数，保存结束前不会达到 100%。逐行结果独立记录，取消后尚未执行的行显示“未提交”。

WinUI 的模板生成、读取、对象类查询、AML 预检和导入均通过 `Task.Run` 在后台执行。进度在 UI 上创建的 `Progress<T>` 回送，包含加载环、行内加载状态、已用时间与保存阶段。运行时禁用目标和模式修改；停止后禁用重复停止，保存阶段等待收尾完成。

界面检查：打开属性配置，选择较多属性的模板，确认组装和提交期间加载环持续转动、窗口可响应操作、行状态逐条更新；停止后确认未提交行保留“未提交”，最后显示对象类保存结果。原生界面的动画和输入响应仍需在目标 Windows/Aras 环境中实际运行核对。

R37 的无匹配响应可同时满足 `isError() == true`、`getItemCount() == 0`。只在允许不存在的属性查询中先处理数量为 0 的情况；不修改共享错误处理或必需引用校验。

## 多语言写入规则

英文、简中、繁中的代码分别为 `en`、`zc`、`zt`。三语属性都必须使用 `http://www.aras.com/I18N` 命名空间：

```xml
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="en">Description</i18n:label>
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="zc">项目描述</i18n:label>
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="zt">專案說明</i18n:label>
```

无命名空间的 `<label xml:lang="en">` 仍会更新会话语言，不能用来固定写入英文。参考 [Aras 官方国际化文档 5.1.1.2，第 32 页](https://aras.com/wp-content/uploads/2024/04/Aras-Innovator-120-Configuring-Internationalization.pdf#page=32)。

以上多语言验证属于离线的列映射和请求 AML 检查，不替代目标 Aras 的导入后回读。已错位的历史值需要使用源模板，在修复版程序中重新预览并覆盖汇入；仅更新程序不会自动修正已有记录。
