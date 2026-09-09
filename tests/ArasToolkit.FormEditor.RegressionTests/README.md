# 窗体配置修改回归

不需要 Aras 服务器或工具箱数据库。服务回归使用真实 R37 IOM，替换 `IServerConnection` 传输层；模拟批量事务不是对真实服务器事务行为的验证。

```powershell
dotnet run --project tests/ArasToolkit.FormEditor.RegressionTests/ArasToolkit.FormEditor.RegressionTests.csproj
dotnet build src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj
Get-ChildItem -Recurse src/ArasToolkit.App.WinUI/obj -Filter FormConfigurationEditPage.xbf
```

画布测试要求 Playwright Node 包和安装的 Microsoft Edge。可使用已有 Playwright 安装，将 `NODE_PATH` 指向其 `node_modules`，然后执行：

```powershell
node tests/ArasToolkit.FormEditor.RegressionTests/canvas.test.cjs
```

浏览器测试启动本机临时 HTTP 服务和无头 Edge，结束后关闭。截图写入被忽略的 `.codex/screenshots/form-editor-canvas.png`。不访问真实 Aras 资源；资源请求由测试拦截。

真实 R37 验收：在专用测试库准备 Classic Form、带多语言和事件的 Field、HTML 边框、分组和多个 View；保存后通过官方编辑器核对 ID、XY、高度、控件类型、事件和关联。用另一账号锁定或修改相同窗体验证冲突；在包含新增与修改的事务中制造单条失败，确认整批回滚。测试库连接必须明确指定，不使用未确认环境运行写入测试。

2026-09-09 已在用户指定的 BLPLM 上用独立临时 Form/Method 完成 11 项实际读写检查并清理测试数据。结果及服务器版本边界见 `design/窗体配置修改实现与验收.md`。默认运行上述回归仍不访问真实数据库；连接密码、服务端快照和临时运行脚本不纳入源码。
