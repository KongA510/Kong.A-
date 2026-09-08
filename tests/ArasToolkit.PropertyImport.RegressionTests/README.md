# 属性汇入空结果回归验证

在仓库根目录执行：

```powershell
dotnet run --project tests/ArasToolkit.PropertyImport.RegressionTests/ArasToolkit.PropertyImport.RegressionTests.csproj
```

使用项目自带的 R37 IOM 解析模拟 SOAP 响应，并调用实际 `PropertyImportService.PrepareAsync`。不连接 Aras 或工具箱数据库，不提交属性变更；临时模板在验证后清理。

覆盖模式按 `source_id + name` 查询：0 条生成 `add`，1 条生成带已有 ID 的 `edit`，多条阻止提交。新增模式遇到已有属性仍报告名称冲突。

16 个场景覆盖两种模式下的标准空结果、本地化空结果、空 Result、已有属性、真实错误、错误消息相同但错误码不同、重复属性，以及混合新增/覆盖和缺失数据源引用。验证预览状态、校验数量、AML action/ID/目标对象类和错误日志行为。

R37 的无匹配响应可同时满足 `isError() == true`、`getItemCount() == 0`。只在允许不存在的属性查询中先处理数量为 0 的情况；不修改共享错误处理或必需引用校验。
