---
id: TASK-008
priority: P0
type: review
created: 2026-06-24
source: Claude Code
status: done
---

# [自动化监听] 扫描报告

## 检查结果

4个已修改 + 4个未跟踪文件（TASK-005 设置按钮功能），构建失败。

## 涉及文件

- `AGENTS.md` — 协作规范更新
- `MainWindow.xaml/.cs` — 底部设置按钮
- `App.xaml.cs` — DI注册
- 新建: SettingsViewModel.cs, SettingsWindow.xaml/.cs

## 构建状态

**fail** — 旧进程 ArasToolkit.App (PID 16452) 锁定输出DLL，非代码问题

## 待处理任务

- `tasks/TASK-005.md` — status: review_passed（等待编译验证）

## 操作建议

先结束旧进程：`taskkill /PID 16452 /F`，再重新编译验证
