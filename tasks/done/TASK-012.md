---
id: TASK-012
priority: P1
type: feature
created: 2026-06-24
source: Claude Code
status: done
---

## Claude Code 审查结论 (23:50)

**结论: 通过** ✅

- 侧边栏底部布局正确：设置 + Aras登录信息 紧靠左侧
- ArasLoginWindow 弹窗完整：连接/删除功能正常
- ConnectAsync 内部调用 LoginAsync 覆盖全局连接
- DI 注册完整，编译 0 errors

## 需求背景

用户截图反馈两个问题：
1. 侧边栏底部"设置"按钮位置不对，需要紧靠左下角
2. 需要在"设置"右侧新增"Aras登录信息"按钮，点击后查看/管理Aras连接配置

## 具体需求

### 1. 侧边栏底部布局修正

当前 MainWindow.xaml 底部设置按钮：
```xml
<!-- 底部设置按钮 -->
<Border Grid.Row="2"
        BorderBrush="{StaticResource BorderBrush}"
        BorderThickness="0,1,0,0"
        Padding="16,12">
    <Button Content="⚙ 设置" ... HorizontalAlignment="Center">
```

**修改为**：两个按钮水平并排，紧靠左下角：
```xml
<Border Grid.Row="2" BorderBrush="..." BorderThickness="0,1,0,0" Padding="12,12">
    <StackPanel Orientation="Horizontal" HorizontalAlignment="Left">
        <Button Content="⚙️ 设置" ... />
        <Button Content="🔗 Aras登录信息" ... Margin="6,0,0,0" />
    </StackPanel>
</Border>
```

关键变化：
- `HorizontalAlignment="Left"` （紧靠左侧，不再居中）
- 两个按钮水平排列，间距 6px

### 2. "Aras登录信息"按钮功能

点击后打开一个新的弹窗 `ArasLoginWindow`（仿 SettingsWindow 结构）：

```
弹窗标题: "Aras 登录信息管理"
内容:
  ┌─────────────────────────────────────┐
  │ 已保存的 Aras 连接配置              │
  │                                     │
  │ [列表: Url / Database / Username]  │
  │ 每行右侧: [连接] [删除] 按钮       │
  │                                     │
  │ 点击"连接"→ 将选中的连接信息写入   │
  │ 全局 ArasConnectionService，        │
  │ 覆盖当前 conn 对象                  │
  └─────────────────────────────────────┘
```

### 3. ArasLoginWindow 实现

- 新建 `ArasLoginWindow.xaml` + `.cs`（Window，不是 UserControl）
- 新建 `ArasLoginViewModel.cs`
- ViewModel 加载已保存的 Aras 登录配置列表（复用 IConfigService.LoadAllLoginInfosAsync()）
- 每行三个操作：
  - **连接**：调用 ILoginService.LoginAsync() 重新建立 HttpServerConnection，覆盖全局 ArasConnectionService 中的连接
  - **删除**：删除该配置
  - **查看密码**：可选，显示/隐藏密码

### 4. 连接切换逻辑

```csharp
// ArasLoginViewModel 中
private async Task ConnectToAras(LoginInfo info)
{
    // 1. 重新执行登录流程，获取新的 Innovator + HttpServerConnection
    await _loginService.LoginAsync(info);
    // 2. LoginService 内部会调用 _connectionService.SetConnection() 覆盖全局连接
    // 3. 提示用户连接已切换
    StatusMessage = $"已切换到 {info.Username}@{info.Database}";
}
```

注意：需要确认 ILoginService.LoginAsync() 内部调用了 `_connectionService.SetConnection()`。如果未调用，需增加该步骤。

## 涉及文件

- `src/ArasToolkit.App/MainWindow.xaml` — 底部按钮布局修正
- `src/ArasToolkit.App/MainWindow.xaml.cs` — ArasLoginButton_Click 事件
- `src/ArasToolkit.App/Views/ArasLoginWindow.xaml` — 新建弹窗
- `src/ArasToolkit.App/Views/ArasLoginWindow.xaml.cs` — 新建 code-behind
- `src/ArasToolkit.App/ViewModels/ArasLoginViewModel.cs` — 新建 ViewModel
- `src/ArasToolkit.App/App.xaml.cs` — DI 注册 ArasLoginViewModel
- `src/ArasToolkit.Services/Services/LoginService.cs` — 确认 SetConnection 逻辑


## 检查清单（Codex 完成）

### 修改文件
- [x] App/MainWindow.xaml — 侧边栏底部：两个按钮水平并排（设置 + Aras登录信息），紧靠左下角
- [x] App/MainWindow.xaml.cs — 新增 ArasLoginButton_Click 事件处理
- [x] App/ViewModels/ArasLoginViewModel.cs — 新建 ViewModel（加载列表/连接/删除）
- [x] App/Views/ArasLoginWindow.xaml — 新建弹窗界面（列表 + 连接/删除按钮）
- [x] App/Views/ArasLoginWindow.xaml.cs — 新建 code-behind
- [x] App/App.xaml.cs — DI 注册 ArasLoginViewModel + ArasLoginWindow

### 功能验证
- [x] 点击"Aras登录信息"弹出管理窗口
- [x] 窗口显示已保存的 Aras 连接列表（Url/Database/Username）
- [x] 每行支持"连接"（切换全局连接）和"删除"操作
- [x] LoginService.LoginAsync 内部已调用 _connectionService.SetConnection()

### 编译验证
- [x] dotnet build ArasToolkit.slnx 通过（0 errors）

## 编译结果（Codex 填写）
- 修复状态: success
- 编译结果: pass
- 备注: 侧边栏底部两按钮 + ArasLoginWindow 弹窗管理连接
