# 属性汇入与多语言回归验证

在仓库根目录执行：

```powershell
dotnet run --project tests/ArasToolkit.PropertyImport.RegressionTests/ArasToolkit.PropertyImport.RegressionTests.csproj
```

使用项目自带的 R37 IOM 解析模拟 SOAP 响应，并调用实际 `PropertyImportService.PrepareAsync`。不连接 Aras 或工具箱数据库，不提交属性变更；临时模板在验证后清理。

覆盖模式按 `source_id + name` 查询：0 条生成 `add`，1 条生成带已有 ID 的 `edit`，多条阻止提交。新增模式遇到已有属性仍报告名称冲突。

24 个场景覆盖：

- 两种模式下的标准空结果、本地化空结果、空 Result、已有属性、真实错误、错误消息相同但错误码不同、重复属性，以及混合新增/覆盖和缺失数据源引用。验证预览状态、校验数量、AML action/ID/目标对象类和错误日志行为。
- 属性三语标签、默认值、提示从 Excel 表头到预览模型、AML 的完整映射，包含列顺序调整、表头空白、XML 特殊字符和新增/覆盖两条路径。
- 对象类的三语名称、TOC 显示文字，以及关系类三语页签在新增/覆盖模式中的语言对应与命名空间。

R37 的无匹配响应可同时满足 `isError() == true`、`getItemCount() == 0`。只在允许不存在的属性查询中先处理数量为 0 的情况；不修改共享错误处理或必需引用校验。

## 多语言写入规则

英文、简中、繁中的代码分别为 `en`、`zc`、`zt`。三语属性都必须使用 `http://www.aras.com/I18N` 命名空间：

```xml
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="en">Description</i18n:label>
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="zc">项目描述</i18n:label>
<i18n:label xmlns:i18n="http://www.aras.com/I18N" xml:lang="zt">專案說明</i18n:label>
```

无命名空间的 `<label xml:lang="en">` 仍会更新会话语言，不能用来固定写入英文。参考 [Aras 官方国际化文档 5.1.1.2，第 32 页](https://aras.com/wp-content/uploads/2024/04/Aras-Innovator-120-Configuring-Internationalization.pdf#page=32)。

验证属于离线的列映射和请求 AML 检查，不替代目标 Aras 的导入后回读。已错位的历史值需要使用源模板，在修复版程序中重新预览并覆盖汇入；仅更新程序不会自动修正已有记录。
