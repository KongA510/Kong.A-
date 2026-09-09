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

## WinUI 原生画布回归

仅浏览器通过或生成 XBF 不能证明 WinUI 能加载 WebView2 原生组件。`NativeSmoke.cs` 通过显式构建开关启动真实生产页面、ViewModel 和 WebView2，服务替换为内存数据，保存方法直接拒绝写入。正常构建不包含此测试入口。

在项目根目录执行（需要 Windows App Runtime、WebView2 Runtime 和桌面会话）：

```powershell
dotnet build src/ArasToolkit.App.WinUI/ArasToolkit.App.WinUI.csproj -p:FormEditorSmokeTest=true -p:OutputPath=../../.codex/form-editor-native/bin/
$env:FORM_EDITOR_SMOKE_OUTPUT = Join-Path (Get-Location) '.codex/form-editor-native/result'
$env:FORM_EDITOR_SMOKE_ASSET_FAULTS = '1'
$testProcess = Start-Process -FilePath (Join-Path (Get-Location) '.codex/form-editor-native/bin/ArasToolkit.App.WinUI.exe') -WindowStyle Hidden -PassThru
if (-not $testProcess.WaitForExit(60000)) { throw '原生测试超时' }
Get-Content .codex/form-editor-native/result/native.log
if ($testProcess.ExitCode -ne 0) { throw '原生测试失败' }
```

原生测试覆盖 DLL 与进程架构一致、实际控件渲染、脚本异常提示、数据刷新不覆盖画布错误、重试保留草稿及撤销记录、资源文件缺失、启动握手超时和窗口关闭。结果记录在 `native.log`，画布截图为 `canvas.png`。故障注入只允许 `.codex` 内的隔离输出目录，临时修改其中的脚本文件并在 finally 恢复；不要同时运行两个测试实例。测试使用独立输出目录下的浏览器缓存。

可选设置 `FORM_EDITOR_SMOKE_SNAPSHOT` 指向包含 `definition.json` 和 `metadata.json` 的本地目录，以只读快照替换内存窗体。快照通过 `IFormConfigurationEditService` 读取后序列化；不把服务端配置或凭据提交到 Git。宿主仍禁止保存和外部资源读取。

2026-09-09 白板回归定位：不指定 RID 的 AnyCPU 构建选中了 x86 `Microsoft.Web.WebView2.Core.dll`，实际进程为 x64。WinUI 项目现在默认明确使用 x64，并按显式 x86/ARM64 平台或 RID 选择对应原生组件。必须用**未指定 RID 的普通构建**验证此问题，不能仅用 `-r win-x64` 掩盖默认构建错误。交付前重新执行正常 WinUI 构建并核验 XBF。

真实 R37 验收：在专用测试库准备 Classic Form、带多语言和事件的 Field、HTML 边框、分组和多个 View；保存后通过官方编辑器核对 ID、XY、高度、控件类型、事件和关联。用另一账号锁定或修改相同窗体验证冲突；在包含新增与修改的事务中制造单条失败，确认整批回滚。测试库连接必须明确指定，不使用未确认环境运行写入测试。

2026-09-09 已在用户指定的 BLPLM 上用独立临时 Form/Method 完成 11 项实际读写检查并清理测试数据。结果及服务器版本边界见 `design/窗体配置修改实现与验收.md`。默认运行上述回归仍不访问真实数据库；连接密码、服务端快照和临时运行脚本不纳入源码。
