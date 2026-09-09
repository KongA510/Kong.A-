# 数据汇入回归验证

修复 WinUI 数据汇入在后台进度回调中更新绑定属性产生的跨线程异常，并保留异常或取消前的实际处理数量。暂停后继续使用同一批次，不从第一行重新导入。

## 服务层

```powershell
dotnet run --project tests/ArasToolkit.DataImport.RegressionTests/ArasToolkit.DataImport.RegressionTests.csproj
```

使用真实 R37 IOM 解析器及内存传输层，覆盖成功、空行、服务端拒绝、取消、空消息 COM 异常、日志失败、无连接和无效范围，不连接数据库。

## 原生 WinUI 页面

```powershell
dotnet build src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj -p:DataImportSmokeTest=true -p:OutputPath=C:/Temp/ArasDataImportSmoke/
Start-Process C:/Temp/ArasDataImportSmoke/ArasToolkit.App.WinUI.exe -WindowStyle Hidden -Wait
Get-Content C:/Temp/ArasDataImportSmoke/data-import-native.log
```

测试宿主只在显式开启 `DataImportSmokeTest` 时编译。加载实际 DataImportPage，验证绑定通知均在 UI 线程、并发进度、暂停继续不重复提交，以及部分中断和全部失败的状态提示。该测试使用内存服务，不连接数据库。测试后普通构建不带该参数即可生成正常应用。

## 2026-09-09 BLPLM 实测

使用用户提供的 `新建 Microsoft Excel 工作表.xlsx`，工作表 `Sheet1 (2)`，第 2～118 行，共 117 行，1 个线程，使用已有的“jp文档编码”AML 模板。

全部 117 行请求均执行并返回，成功 0、失败 117、跳过 0。服务端返回 `ItemClassificationNotFoundException`，因为源分类在 BLPLM 不存在；用户已确认这是预期业务结果。前后查询均未找到这些分类对应的编码设置。逐行失败明细已保存在本地日志，验证期间未修改目标分类。

原始错误堆栈位于 `DataImportViewModel.ImportProgress`，经 `ObservableObject.OnPropertyChanged` 进入 WinRT 的 `PropertyChangedEventArgsRuntimeClassFactory.CreateInstance`。原生测试验证修复后的通知均回到 UI 线程。
