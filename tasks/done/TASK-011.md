---
id: TASK-011
priority: P0
type: feature
created: 2026-06-24
source: Claude Code
status: done
---

# 新增应用独立登录系统

## 需求背景

当前登录界面直接对接 Aras Innovator 系统（HttpServerConnection），但实际使用中需要先进入工具箱再连接 Aras。需要新增一套独立的应用登录系统：

- **保留**现有 Aras 登录全部功能（Url/Database/Username/Password + 已保存记录列表）
- **新增**应用层登录作为入口，登录成功后才显示主界面
- 新增 `app_user` 数据库表存储用户信息

## 具体需求

### 1. 新建 app_user 表 + 实体

```
表名: app_user
列:
  - id          NVARCHAR(12) PRIMARY KEY     （12位短GUID）
  - username    NVARCHAR(100) NOT NULL UNIQUE （登录账户）
  - password    NVARCHAR(100) NOT NULL        （密码，明文即可—内部工具）
  - display_name NVARCHAR(100)               （显示名称/昵称）
  - is_admin    BIT NOT NULL DEFAULT 0        （是否为管理员）
  - avatar      NVARCHAR(500) NULL            （头像路径或base64，可选）
  - creator_on  DATETIME2 NOT NULL DEFAULT GETDATE()
```

### 2. 新建 Entity: Core/Entities/AppUser.cs

```csharp
[Table("app_user")]
public class AppUser
{
    [Key][Column("id")][MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];
    
    [Column("username")][MaxLength(100)][Required]
    public string Username { get; set; }
    
    [Column("password")][MaxLength(100)][Required]
    public string Password { get; set; }
    
    [Column("display_name")][MaxLength(100)]
    public string? DisplayName { get; set; }
    
    [Column("is_admin")]
    public bool IsAdmin { get; set; }
    
    [Column("avatar")][MaxLength(500)]
    public string? Avatar { get; set; }
    
    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;
}
```

### 3. 改造登录界面 LoginView

- 将当前登录界面（Aras连接登录）**整体保留**，但改为二级界面
- 新增 **一级登录界面**（应用登录）：账号 + 密码 + 登录/注册按钮
- 一级登录校验 `app_user` 表，成功后进入主界面
- 一级登录支持"注册"：填写 账号+密码+显示名称 → 写入 app_user 表（默认 is_admin=0）

### 4. 登录流程变更

```
应用启动 → 应用登录界面（验证 app_user 表）
  → 登录成功 → 主界面（侧边栏 + Dashboard）
  → 主界面内通过"Aras登录信息"按钮 → 查看/管理Aras连接（原有登录信息）
```

### 5. 登录状态管理

- 应用登录成功后，将当前 AppUser 信息（Id, Username, DisplayName, IsAdmin）保存到全局可访问的单例/静态属性中
- 后续所有操作日志/错误日志的 `user_name` 从此处读取

### 6. app_user 种子数据

首次运行时，EnsureSchemaAsync 中自动创建表并插入默认管理员：
- username: admin, password: admin, display_name: 系统管理员, is_admin: 1

## 涉及文件

- `src/ArasToolkit.Core/Entities/AppUser.cs` — 新建实体
- `src/ArasToolkit.Core/Interfaces/IAppUserService.cs` — 新建接口
- `src/ArasToolkit.Services/Services/AppUserService.cs` — 新建服务
- `src/ArasToolkit.Services/Data/ArasToolkitDbContext.cs` — 添加 DbSet<AppUser> + OnModelCreating + EnsureSchemaAsync
- `src/ArasToolkit.App/ViewModels/LoginViewModel.cs` — 改造为应用登录 + 保留Aras登录历史管理
- `src/ArasToolkit.App/Views/LoginView.xaml` — 重写登录界面布局
- `src/ArasToolkit.App/Views/LoginView.xaml.cs` — 适配新逻辑
- `src/ArasToolkit.App/App.xaml.cs` — DI注册
- `src/ArasToolkit.Services/ServiceCollectionExtensions.cs` — DI注册
- `src/ArasToolkit.Core/Models/LoginInfo.cs` — 保持不变（Aras连接信息用）

## 检查清单（Codex 完成）

### 新增文件
- [x] Core/Entities/AppUser.cs — 应用用户实体
- [x] Core/Interfaces/IAppUserService.cs — 应用用户服务接口
- [x] Services/Services/AppUserService.cs — 应用用户服务实现（登录/注册/Schema同步）
- [x] Core/Models/CurrentUserContext.cs — 全局当前用户上下文
- [x] App/ViewModels/AppLoginViewModel.cs — 应用登录 ViewModel
- [x] App/Views/AppLoginView.xaml — 应用登录界面
- [x] App/Views/AppLoginView.xaml.cs — 应用登录代码后端

### 修改文件
- [x] Services/Data/ArasToolkitDbContext.cs — DbSet<AppUser> + OnModelCreating + EnsureSchema
- [x] App/MainWindow.xaml.cs — 启动时显示 AppLoginView，替换旧 LoginView
- [x] App/App.xaml.cs — DI 注册 AppLoginViewModel + AppLoginView
- [x] Services/ServiceCollectionExtensions.cs — DI 注册 IAppUserService

### 编译验证
- [x] dotnet build ArasToolkit.slnx 通过（0 errors, 2 warnings，warnings 为预有依赖冲突）
- [ ] 无新增 Warning

## 编译结果（Codex 填写）
- 修复状态: success
- 编译结果: pass
- 备注: 应用登录系统完成，启动时显示登录界面，支持登录/注册，注册后自动登录

## 审查结论（Claude Code）

**结论: 通过** ✅

TASK-011 审查通过 -- AppUser 实体/表设计完整，登录/注册逻辑正确，DI注册完整，CurrentUserContext 设置正确。编译 0 errors。迁移到 done。

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]

