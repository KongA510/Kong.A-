---
id: TASK-013
priority: P0
type: feature
created: 2026-06-24
source: Claude Code
status: pending
---

# 操作日志/错误日志补齐操作人列 + 按用户隔离查询

## 需求背景

当前所有已完成功能（Todo、Excel导入等）写入 operation_log / error_log 时，`user_name` / `function_name` 等字段未关联到具体的应用登录用户。需要在所有日志写入处补齐操作人信息，并且查询时按用户隔离（管理员可见全部，普通用户仅见自己的记录）。

## 具体需求

### 1. 日志表列检查（无需修改）

- `operation_log.user_name` — 已有列，当前可能为空字符串或未填写
- `error_log.function_name` — 已有列（记录调用模块名，无需改动）
- `error_log` 表无 `user_name` 列 → **需要新增**

### 2. error_log 表新增 user_name 列

```sql
-- EnsureSchemaAsync 中新增
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME='error_log' AND COLUMN_NAME='user_name')
BEGIN
    ALTER TABLE error_log ADD user_name NVARCHAR(100) NULL;
END
```

### 3. ErrorLog 实体新增字段

```csharp
// ErrorLog.cs 新增
[Column("user_name")]
[MaxLength(100)]
public string? UserName { get; set; }
```

### 4. 全局当前用户上下文

在 Core 层新建 `CurrentUserContext` 静态类：

```csharp
// Core/Models/CurrentUserContext.cs
namespace ArasToolkit.Core.Models;

public class AppUserInfo
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string? DisplayName { get; set; }
    public bool IsAdmin { get; set; }
}

public static class CurrentUserContext
{
    public static AppUserInfo? Current { get; set; }
    
    public static string CurrentUserName => Current?.Username ?? "未知用户";
    public static bool IsAdmin => Current?.IsAdmin ?? false;
}
```

### 5. 所有日志写入处补齐 user_name

**TodoService.cs**（所有 `_operationLogService.LogAsync` 调用）：
```csharp
// 当前写法（user_name 为空或硬编码）
await _operationLogService.LogAsync("Create", "PersonalTask", item.Id,
    $"创建任务: {item.TaskName}");

// 修改为
await _operationLogService.LogAsync("Create", "PersonalTask", item.Id,
    $"创建任务: {item.TaskName}", CurrentUserContext.CurrentUserName);
```

**注意**：需要检查 `IOperationLogService.LogAsync` 的签名是否支持 userName 参数。如果不支持，需要新增重载。

**ErrorLogService** 中写入 error_log 时同理补充 `user_name`：
```csharp
await _errorLogService.LogErrorAsync("Todo-新增", ex.Message,
    ErrorLog.LevelP0, ex.StackTrace);
// → LogErrorAsync 内部写 error_log 时补充 UserName = CurrentUserContext.CurrentUserName
```

**SettingsViewModel.cs** 中写 error_log 时也要补齐：
```csharp
await _errorLogService.LogErrorAsync("设置-连接字符串保存", ex.Message,
    ErrorLog.LevelP0, ex.StackTrace);
// 内部同一处写入 UserName
```

### 6. 查询日志时按用户隔离

**OperationLogViewModel / ErrorLogViewModel** 加载数据时：

```csharp
// 加载操作日志/错误日志时
if (CurrentUserContext.IsAdmin)
{
    // 管理员：加载全部
    var items = await context.OperationLogs
        .OrderByDescending(x => x.OperateTime)
        .ToListAsync();
}
else
{
    // 普通用户：仅加载自己的记录
    var items = await context.OperationLogs
        .Where(x => x.UserName == CurrentUserContext.CurrentUserName)
        .OrderByDescending(x => x.OperateTime)
        .ToListAsync();
}
```

### 7. 登录成功后设置 CurrentUserContext

TASK-011 中的应用登录成功后：
```csharp
CurrentUserContext.Current = new AppUserInfo
{
    Id = user.Id,
    Username = user.Username,
    DisplayName = user.DisplayName,
    IsAdmin = user.IsAdmin
};
```

## 涉及文件

- `src/ArasToolkit.Core/Entities/ErrorLog.cs` — 新增 UserName 字段
- `src/ArasToolkit.Core/Models/CurrentUserContext.cs` — 新建全局用户上下文
- `src/ArasToolkit.Core/Interfaces/IOperationLogService.cs` — 确认 LogAsync 签名
- `src/ArasToolkit.Core/Interfaces/IErrorLogService.cs` — 确认 LogErrorAsync 签名
- `src/ArasToolkit.Services/Services/OperationLogService.cs` — 补齐 user_name 写入
- `src/ArasToolkit.Services/Services/ErrorLogService.cs` — 补齐 user_name 写入
- `src/ArasToolkit.Services/Services/TodoService.cs` — 所有 LogAsync 补齐 userName
- `src/ArasToolkit.Services/Data/ArasToolkitDbContext.cs` — OnModelCreating + EnsureSchemaAsync 加 user_name 列
- `src/ArasToolkit.App/ViewModels/OperationLogViewModel.cs` — 按用户隔离查询
- `src/ArasToolkit.App/ViewModels/ErrorLogViewModel.cs` — 按用户隔离查询
- `src/ArasToolkit.App/ViewModels/LoginViewModel.cs` — 登录成功后设置 CurrentUserContext

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 无新增 Warning

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]
