# 数据汇入回归验证

修复 WinUI 数据汇入在后台进度回调中更新绑定属性产生的跨线程异常，并保留异常或取消前的实际处理数量。暂停后继续使用同一批次，不从第一行重新导入。

## 服务层

```powershell
dotnet run --project tests/ArasToolkit.DataImport.RegressionTests/ArasToolkit.DataImport.RegressionTests.csproj
```

使用真实 R37 IOM 解析器及内存传输层，覆盖成功、空行、服务端拒绝、取消、空消息 COM 异常、日志失败、无连接和无效范围，不连接数据库。

特殊字符回归覆盖文本/属性/CDATA 中的 `& < > ' "`、`]]>`、Unicode、换行和制表符，确认 Excel 原始文本经 IOM 传输后保持一致。XML 1.0 禁止的控制字符及缺失列占位符记录为行失败。`@A`、`@AA` 一次完整匹配，单元格中的占位符或 XML 片段不会再次展开。

模板中已有的 `&amp;`、`&#x20;`、十进制字符引用按 XML 规则解析一次；单元格中看起来像实体的字符串保留为原文，不自动解码。单元格的首尾空白也保留。模板中的占位符用于属性值或文本值，不用于生成标签名。

汇入日志明细采用制表符分隔的四列：`状态 / 信息 / Excel行号 / 失败行号`。最后一列仅在该行失败时填写，使用源工作表从 1 开始的真实行号，成功和跳过留空。错误详情中的换行、制表符使用可见转义，避免串列。日志结尾另外提供失败行号升序汇总；中断但未执行的行不会标成失败行。

规则依据：[Aras AML 官方说明](https://www.aras.com/community/documentationlibrary/Innovator/28/Content/Innovator%2024%20Docs/Programmer%27s%20Guide/The%20Aras%20Markup%20Language%20AML.htm)、[W3C XML 1.0 字符与实体引用](https://www.w3.org/TR/xml/#sec-references)、[Microsoft XmlConvert.VerifyXmlChars](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlconvert.verifyxmlchars)。

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

特殊字符处理与日志末列优化后，使用同一文件再次验证：117 条明细均为固定四列，失败行号依次为 2～118，结果中的失败行号集合与明细完全一致，未创建这些分类对应的编码设置。

原始错误堆栈位于 `DataImportViewModel.ImportProgress`，经 `ObservableObject.OnPropertyChanged` 进入 WinRT 的 `PropertyChangedEventArgsRuntimeClassFactory.CreateInstance`。原生测试验证修复后的通知均回到 UI 线程。
