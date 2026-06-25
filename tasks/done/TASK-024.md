---
id: TASK-024
priority: P0
type: fix
created: 2026-06-25
source: Claude Code
status: done
---

# 数据汇入 — Excel 列头含括号时预览崩溃（PropertyPath 语法错误）

## 问题描述

数据汇入页面加载 Excel 预览时，如果 Excel 列头包含括号 `(` `)` 字符，WPF 抛出异常：

> 预览加载失败: PropertyPath 不匹配的括号 '审批流(括号内各部门并行，无先后顺序)' 中的语法错误

**触发条件**: Excel 表头列名包含 `(` 或 `)`（中文/英文括号均会触发）

**影响范围**: 任何列头含括号的 Excel 文件均无法预览，功能不可用

## 根因分析

### 完整调用链

```
Excel 列头: "审批流(括号内各部门并行，无先后顺序)"
    ↓
DataImportService.ReadSheetRangeAsync() [行125]
    → rawHeader = worksheet.Cells[1, c].Text?.Trim()
    → 原样保留 "(...)" → 未做任何清洗
    ↓
[行147] result.Data.Columns.Add(m.Header)
    → DataTable 列名 = "审批流(括号内各部门并行，无先后顺序)"  ← 问题列名
    ↓
DataImportView.xaml [行103]
    → DataGrid AutoGenerateColumns="True"
    → ItemsSource="{Binding PreviewData}"
    ↓
WPF 自动为每列生成 DataGridTextColumn
    → Binding.Path = "审批流(括号内各部门并行，无先后顺序)"
    ↓
PropertyPath 解析器将 () 视为索引器/附加属性语法
    → 💥 解析失败: "不匹配的括号"
```

### 为什么 `()` 会触发 WPF 绑定错误

WPF `PropertyPath` 语法中，`()` 有特殊含义：
- `(ownerType.property)` — 附加属性语法，如 `(Grid.Row)`
- `[index]` — 索引器语法

当列名包含 `(` 时，PropertyPath 解释器进入附加属性解析模式，但找不到匹配的 `)` 和类型名，于是报"不匹配的括号"。

### 同样受影响的字符

除了 `()` 外，以下字符也会破坏 WPF PropertyPath：
- `.` — 路径分隔符
- `/` — 多级路径
- `[` `]` — 索引器
- `(` `)` — 附加属性
- `,` `:` `+` `-` `#` — 语法分隔符

**但是**: 对于中文 Excel 表头，最常出现的是中文括号 `（）` 也是全角字符，视觉上相似但通常不会触发绑定错误（全角 `（）` 不被 WPF 视为特殊字符）。需要确认用户实际是半角 `(` 还是全角 `（`。

## 涉及文件

| 文件 | 说明 |
|---|---|
| `src/ArasToolkit.Services/Services/DataImportService.cs` 第 121-135 行 | `ReadSheetRangeAsync` — 列头读取处，需添加清洗逻辑 |
| `src/ArasToolkit.Services/Services/DataImportService.cs` 第 168-189 行 | `GetColumnMappingsAsync` — 同样读取列头，建议同步清洗保持一致 |

## 修复方案

### 方案：添加列头清洗方法，在生成 DataTable 列名前过滤非法字符

在 `DataImportService` 中添加一个私有静态方法：

```csharp
/// <summary>
/// 清洗列头文本，移除 WPF PropertyPath 不能解析的特殊字符
/// </summary>
private static string SanitizeHeader(string raw)
{
    if (string.IsNullOrEmpty(raw)) return raw;
    // 替换 WPF PropertyPath 非法字符为全角/下划线
    return raw
        .Replace('(', '（')   // 半角括号 → 全角
        .Replace(')', '）')
        .Replace('[', '【')
        .Replace(']', '】')
        .Replace('.', '．')   // 全角句点
        .Replace('/', '／')   // 全角斜线
        .Replace(',', '，')
        .Replace(':', '：');
}
```

然后在 `ReadSheetRangeAsync` 第 125 行调用：

```csharp
// 修改前:
var rawHeader = worksheet.Cells[1, c].Text?.Trim() ?? "";

// 修改后:
var rawHeader = SanitizeHeader(worksheet.Cells[1, c].Text?.Trim() ?? "");
```

同时 `GetColumnMappingsAsync` 第 184 行也需要同步清洗，保持列头展示一致性：

```csharp
// 修改前:
var header = worksheet.Cells[1, c].Text?.Trim() ?? "";

// 修改后:
var header = SanitizeHeader(worksheet.Cells[1, c].Text?.Trim() ?? "");
```

### 为什么不改用非 AutoGenerateColumns 方案

另一种方案是手动生成列并设置安全的 Binding 路径（如列索引绑定）。但此方案改动较大，需要替换整个 DataGrid 模板，且当前 `AutoGenerateColumns="True"` 配合 DataTable 是最简洁的实现。**推荐先用清洗方案**，改动最小、风险最低。

## 预期行为

修复后：
1. 列头 `审批流(括号内各部门并行，无先后顺序)` → 清洗为 `审批流（括号内各部门并行，无先后顺序）`（半角括号替换为全角）
2. DataGrid 正常渲染所有列，无 PropertyPath 异常
3. 列映射表 `@A = 审批流（括号内各部门并行，无先后顺序）` 中显示清洗后的列头
4. 所有其他含特殊字符的列头也能正常显示

**注意**: 列头清洗只影响**显示**，不改变 AML 模板中的占位符 `@A` `@B` 引用方式（仍通过列字母绑定，不受影响）。

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 无新增 Warning
- [ ] 使用含括号列头的 Excel 文件测试：浏览 → 加载预览 → 确认 DataGrid 正常渲染

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]
