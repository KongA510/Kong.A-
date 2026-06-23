---
id: TASK-010
priority: P0
type: spec
created: 2026-06-24
source: Claude Code
status: done
---

# Git 本地备份与推送流程 — 每次编码强制遵守

## 问题描述

Codex 每次编码前缺少本地 Git 备份，代码出错时无法快速回滚。需建立强制 Git 流程：编码前 stash 备份，审核通过后按 CLAUDE.md 第十一章分支策略推送。

## 涉及文件

- 无需修改项目文件（纯流程规范）
- 与 `CLAUDE.md` 第十一章联动

## 预期行为

1. 每次编码前工作区已被 `git stash` 备份，可随时恢复
2. 任务审核通过后，代码已推送至远程仓库
3. master 为最新版本，develop 保留完整历史

## 强制流程

### 编码前备份（每次必须执行）

```bash
cd "D:/git仓库/个人工具箱"
git add -A
git stash push -m "backup-before-TASK-XXX"
```

目的：将当前所有变更（含未跟踪文件）暂存到 stash。若编码出错，可通过 `git stash pop` 恢复到编码前状态。

### 编码完成后提交

```bash
git add -A
git commit -m "描述: 具体变更内容"
```

### 审核通过后推送（仅审核通过后执行）

```bash
# 备份 master → develop
git checkout develop
git merge master
git push origin develop

# 推送 master（force push）
git checkout master
git push origin master --force
```

## Claude Code 分析

此流程是项目级强制约束，所有 TASK 编码工作均须遵守。与 CLAUDE.md 第十一章 Git 分支与推送策略完全一致。不遵守此流程的编码操作视为不合规。

## 编译验证

- 不涉及编译

## 检查清单（Codex 填写）

- [ ] 编码前已执行 `git stash push -m "backup-before-TASK-XXX"`
- [ ] 编码后已 `git commit`
- [ ] 审核通过后已执行推送（develop 备份 + master force push）


