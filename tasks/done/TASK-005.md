---
id: TASK-005
priority: P0
type: feature
created: 2026-06-24
source: Claude Code
status: done
---

# 侧边栏底部改为设置按钮 + 数据库连接字符串设置

## 问题描述

侧边栏底部当前有 3 个元素（"已登录"文本 + "数据库检查"按钮 + "退出"按钮），需要整合为一个"⚙ 设置"按钮，点击弹出设置对话框。同时新增"数据库连接字符串设置"功能，允许在 UI 中修改连接字符串，无需手动编辑底层配置文件。

## 涉及文件

| 操作 | 文件 |
|------|------|
| **新建** | `src/ArasToolkit.App/ViewModels/SettingsViewModel.cs` |
| **新建** | `src/ArasToolkit.App/Views/SettingsWindow.xaml` |
| **新建** | `src/ArasToolkit.App/Views/SettingsWindow.xaml.cs` |
| **修改** | `src/ArasToolkit.App/MainWindow.xaml` — 替换底部栏 |
| **修改** | `src/ArasToolkit.App/MainWindow.xaml.cs` — 删除旧方法，新增 SettingsButton_Click |
| **修改** | `src/ArasToolkit.App/App.xaml.cs` — 添加 DI 注册 |

## 预期行为

1. 侧边栏底部只显示一个**紧靠左下角**的"⚙ 设置"按钮
2. 点击弹出设置小窗口（~480×450），居中于主窗口
3. 窗口包含三个功能区：
   - **🔍 数据库检查** — 迁移现有 `RunDatabaseCheckAsync()` 逻辑
   - **🚪 退出** — 触发登出，回到登录界面
   - **🗄 数据库连接字符串设置** — 显示/编辑/保存连接字符串，保存后刷新 DbContext 缓存

## Claude Code 分析

方案已实施，代码结构正确，但 UI 细节需修正。

## 检查清单（Codex 填写）

- [x] SettingsViewModel.cs 已创建
- [x] SettingsWindow.xaml + .cs 已创建
- [x] MainWindow.xaml 底部栏已替换为设置按钮
- [x] MainWindow.xaml.cs 已删除旧方法和残留碎片
- [x] App.xaml.cs 已注册 DI
- [ ] **按钮位置修正**（见审查意见）
- [ ] dotnet build 通过
- [ ] 遵循 TASK-010 Git 备份流程

## 审查意见（Claude Code 填写）

- 审查结论: **需修改: 设置按钮 `HorizontalAlignment` 当前为 `Center`，应改为 `Left`，使按钮紧靠左下角而非居中**
- 具体修改: MainWindow.xaml 第 213 行 `HorizontalAlignment="Center"` → `HorizontalAlignment="Left"`；同时将 Border 的 `Padding="16,12"` 改为 `Padding="16,8"` 减少底部间距，使按钮紧贴左下角边缘
- 代码逻辑: 通过，SettingsViewModel / SettingsWindow / MainWindow.xaml.cs 均无问题
- 构建结果: 待 Codex 修复后编译验证
