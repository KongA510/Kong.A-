# 导包工具回归

```powershell
dotnet run --project tests/ArasToolkit.PackageMigration.RegressionTests/ArasToolkit.PackageMigration.RegressionTests.csproj
```

30 项检查，使用内存服务器与引擎，不登录实库：多段 AML、窗体所属字段修补、关系 ItemType 去重、依赖循环、版本化 ID、多语言、命名引用、同名异 ID、重复导入、破损包、路径穿越、DTD、删除及条件操作限制、目标并发变化、备份失败、部分失败、取消和凭据脱敏等。

原生 WinUI 宿主只在显式开关下编译。验证实际 XBF、搜索、多选、依赖树、差异勾选、UI 线程进度、取消入口和执行历史，并保存页面截图。测试服务完全为内存实现。

```powershell
dotnet build src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj `
  -p:PackageMigrationSmokeTest=true -p:OutputPath=C:/Temp/ArasPackageNative/
Start-Process C:/Temp/ArasPackageNative/ArasToolkit.App.WinUI.exe -WindowStyle Hidden -Wait
Get-Content C:/Temp/ArasPackageNative/package-native.log
```

完成后用不带该开关的正常命令构建主程序。实库验收由单独的本地工具执行，不把连接配置、凭据、完整 AML、方法内容或生产数据库资料放入测试源码。
