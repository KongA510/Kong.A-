# 对象类多语系与生命周期回归

运行：`dotnet run --project tests/ArasToolkit.ObjectClass.RegressionTests`

使用真实 R37 IOM，仅替换网络传输及日志持久化，不访问 Aras 或工具箱数据库。

- 对象类 `label`、`label_plural` 与关系类页签标签的新增/覆盖三语映射。
- XML 特殊字符、名称引号、空白译文保留已有值。
- 标准 Excel 模板经过公开 ImportAsync 入口，验证回读、缺失语言、失败统计及日志。
- 四状态五转换的新建路径；已有生命周期补齐三语、按现有坐标修复空白退回路径。
- 保留手工转折点、重复执行不创建重复转换、查询失败不提交写入。

多语系节点采用 `http://www.aras.com/I18N`（无尾部斜杠）及显式 `xml:lang`，参考 [Aras Configuring Internationalization — AML](https://www.aras.com/community/documentationlibrary/Innovator/23/Content/Innovator%2023%20Docs/Configuring%20Internationalization/AML.htm)。

已只读核对现场 R37 的 Language 配置为 `en/zc/zt`，Life Cycle Transition 的 `segments` 字段及现有数据格式为 `x,y|x,y`。默认退回从上方绕行（`320,40|100,40`）；已有节点按实际坐标计算，已有非空路径保留。

既有数据修复入口：对象类使用原模板的“覆盖”模式；生命周期重新勾选“生命周期与状态权限设定”执行即可。测试不会自动批量修改现场已有配置。
