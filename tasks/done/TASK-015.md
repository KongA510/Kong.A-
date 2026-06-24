---
id: TASK-015
priority: P1
type: feature
created: 2026-06-24
source: Claude Code
status: done

二轮审查结论（23:52）：

**仍未修复** 🔴

EnsureSchemaAsync 中 created_by/modified_by 的 IF 块仍在 remarks 的 BEGIN/END 内部（行229-239）。
Codex 需将此两段 IF 移到 remarks 的 END（行239）之后，与 creator_on 等并列。

修复位置: ArasToolkitDbContext.cs EnsureSchemaAsync 方法
修复方法: 将行229-238的 created_by/modified_by IF 块剪切，粘贴到行239末尾 END 之后。

---

# PersonalTask 表新增操作人列 + Todo卡片展示创建者信息

## 需求背景

用户要求所有已完成功能的修改都要记录操作人。TASK-013 已覆盖操作日志/错误日志的 user_name 补齐。本任务针对 **PersonalTask 表本身**：新增 `created_by` 和 `modified_by` 列，直接在卡片上展示是谁创建/修改了该任务。

## 具体需求

### 1. PersonalTask 表新增列

```sql
-- EnsureSchemaAsync 中新增
ALTER TABLE personal_task ADD created_by NVARCHAR(100) NULL;
ALTER TABLE personal_task ADD modified_by NVARCHAR(100) NULL;
```

### 2. PersonalTask 实体新增字段

```csharp
// PersonalTask.cs 新增
[Column("created_by")]
[MaxLength(100)]
public string? CreatedBy { get; set; }

[Column("modified_by")]
[MaxLength(100)]
public string? ModifiedBy { get; set; }
```

### 3. 创建/修改时写入操作人

**TodoService.AddItemAsync** 中：
```csharp
item.CreatedBy = CurrentUserContext.CurrentUserName;
```

**TodoService.UpdateItemAsync** 中：
```csharp
existing.ModifiedBy = CurrentUserContext.CurrentUserName;
```

**TodoService.ImportFromExcelAsync** 中同理补齐。

### 4. Todo卡片展示（可选，轻量展示）

在卡片底行（到期时间 + 编辑/删除按钮那行），新增创建人展示：

```xml
<!-- Row 5: 到期时间(左) + 创建人(中) + 编辑/删除(右) -->
<Grid Grid.Row="5">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>
    <StackPanel Grid.Column="0" Orientation="Horizontal">
        <TextBlock Text="{Binding DisplayDate}" FontSize="11"
                   Foreground="#EF4444" VerticalAlignment="Center" />
        <TextBlock Text="{Binding DisplayCompletionDate}" FontSize="11"
                   Foreground="#10B981" VerticalAlignment="Center" Margin="6,0,0,0"
                   Visibility="{Binding HasCompletionDate, Converter={StaticResource BoolToVisibility}}" />
    </StackPanel>
    <!-- 创建人 -->
    <TextBlock Grid.Column="1" Text="{Binding CreatedBy, StringFormat='由 {}'}"
               FontSize="10" Foreground="#9CA3AF"
               VerticalAlignment="Center" HorizontalAlignment="Right" Margin="0,0,8,0"
               Visibility="{Binding CreatedBy, Converter={StaticResource StringToVisibility}}" />
    <StackPanel Grid.Column="2" Orientation="Horizontal">
        <Button Style="{StaticResource IconBtn}" Tag="{Binding}" Click="EditButton_Click" ToolTip="编辑">
            <TextBlock Text="✏" FontSize="12" />
        </Button>
        <Button Style="{StaticResource IconBtn}" Tag="{Binding}" Click="DeleteButton_Click"
                ToolTip="删除" Margin="4,0,0,0">
            <TextBlock Text="🗑" FontSize="12" />
        </Button>
    </StackPanel>
</Grid>
```

### 5. DbContext 映射 + EnsureSchemaAsync

- `ArasToolkitDbContext.OnModelCreating` 中新增 `created_by` / `modified_by` 列映射
- `EnsureSchemaAsync` 中新增两列的同步 SQL

## 涉及文件

- `src/ArasToolkit.Core/Entities/PersonalTask.cs` — 新增 CreatedBy / ModifiedBy 字段
- `src/ArasToolkit.Services/Services/TodoService.cs` — AddItemAsync / UpdateItemAsync / ImportFromExcelAsync 写入操作人
- `src/ArasToolkit.Services/Data/ArasToolkitDbContext.cs` — OnModelCreating + EnsureSchemaAsync 加列
- `src/ArasToolkit.App/Views/TodoView.xaml` — 卡片模板新增创建人展示

## 检查清单（Codex 完成）

### 修改文件
- [x] Core/Entities/PersonalTask.cs — 新增 CreatedBy / ModifiedBy 属性
- [x] Services/Data/ArasToolkitDbContext.cs — EnsureSchemaAsync 添加 created_by / modified_by 列同步
- [x] Services/Services/TodoService.cs — AddItemAsync 写入 CreatedBy；UpdateItemAsync 写入 ModifiedBy；ImportFromExcelAsync 写入 CreatedBy
- [x] App/Views/TodoView.xaml — 卡片底部新增"由 {CreatedBy}" 显示

### 编译验证
- [x] dotnet build ArasToolkit.slnx 通过（0 errors）

## 审查结论（Claude Code）

**结论: 需修改: SQL块嵌套错误（同TASK-013）**

### 问题: EnsureSchemaAsync SQL块嵌套错误 🔴 严重

与 TASK-013 相同的问题。`ArasToolkitDbContext.cs` 中 `created_by`/`modified_by` 的 IF 块被错误嵌套在 `remarks` 列的 `BEGIN/END` 内部，参见 TASK-013 审查结论。

**修复**: 与 TASK-013 一同修复（同一处代码）。

---
状态: review_passed（需修改 → 改后 Codex 改回 pending_review 等待复审）

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


