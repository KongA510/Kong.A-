---
name: aras
description: Aras Innovator 开发工具箱技能 — 提供 Aras 登录认证、API 调用、项目架构规范及常用开发技巧。在打开 ArasToolkit 项目或进行 Aras 相关开发时自动调用。
keywords: Aras,ArasToolkit,Innovator,登录,HttpServerConnection,ScalcMD5,Item,AML,applyItem,工具箱
---

# Aras 开发工具箱 - 技能文件

---

## 一、Aras 登录认证方式（R37 标准实现）

``

### 11.4 编码前强制备份流程 ⚠️ 每次编码前必须执行

**目的**：每次编码前备份当前工作区，代码出错时可快速回滚到编码前状态。

#### 编码前（每次必须执行）

```bash
git add -A
git stash push -m "backup-before-TASK-XXX"
git stash pop   # 立即恢复，stash 中保留快照
```

> 注意：若编码出错，通过 `git stash list` 查看备份，用 `git stash apply stash@{N}` 恢复。

#### 编码后

```bash
git add -A
git commit -m "功能描述: 具体变更内容"
```

#### 审查通过后推送

```bash
# 备份 master → develop
git checkout develop
git merge master
git push origin develop

# 推送 master（force push）
git checkout master
git push origin master --force
```
`csharp
// ===== Aras 登录核心代码 =====
// 文件位置: src/ArasToolkit.Services/Services/LoginService.cs

// 1. 密码处理
//    loginInfo.Password.ToMd5() → 生成 32 位小写十六进制字符串（模拟 ScalcMD5）

// 2. 创建 HttpServerConnection（R37 API）
//    HttpServerConnection connection = IomFactory.CreateHttpServerConnection(
//        url, database, username, md5Password);

// 3. 创建 Innovator 实例
//    Innovator innovator = new Innovator(connection);
//    或: connection.Login().getInnovator()

// 4. 持久化到全局单例
//    _connectionService.SetConnection(connectionInfo, innovator, connection);
//    参数顺序: (ArasConnectionInfo, Innovator, HttpServerConnection)

// 5. 异常处理
//    catch (Exception ex)
//    {
//        throw new Exception($"CreateHttpServerConnection:{ex.Message}");
//    }

// 关键要点:
// ✓ R37 使用 IomFactory.CreateHttpServerConnection（非 UserPasswordLogin）
// ✓ 密码使用 MD5 哈希（32位小写十六进制）
// ✓ HttpServerConnection 和 Innovator 通过 ArasConnectionService 全局单例持久化
// ✓ Innovator 通过 _connectionService.InnovatorInstance 获取
// ✓ HttpConnection 通过 _connectionService.HttpConnection 获取
```

## 二、项目架构（三层 + MVVM）

```
ArasToolkit.Core       → Entities, Models, Interfaces, Extensions  (无UI依赖)
ArasToolkit.Services   → 业务逻辑实现, 配置管理, Data(DbContext)
ArasToolkit.App        → WPF Views, ViewModels, Styles

设计原则:
- 所有方法使用接口，高内聚低耦合
- 接口与实现同名: ILoginService ↔ LoginService
- EF 实体统一放在 Core/Entities/（命名空间 ArasToolkit.Core.Entities）
- 普通 POCO 模型放在 Core/Models/（命名空间 ArasToolkit.Core.Models）
- 数据库上下文放在 Services/Data/（命名空间 ArasToolkit.Services.Data）
- 扩展类统一在 Core/Extensions/
- 配置按功能细分: Config/AppSettings/, Config/Login/, Config/ErrorLogs/
```

## 三、常用命令

```bash
cd "d:\博威\项目\ICS\个人工具箱"
dotnet build ArasToolkit.slnx            # 编译
dotnet run --project src/ArasToolkit.App # 运行
dotnet add src/ArasToolkit.App package <包名>  # 添加NuGet
```

## 四、添加新功能步骤

```
// 如果涉及数据库映射（EF Core）:
1. Entity:     Core/Entities/NewTable.cs          ← EF 实体，类名与表名一致(PascalCase)
2. DbContext:  Services/Data/ArasToolkitDbContext.cs → 添加 DbSet<NewTable>

// 如果是普通功能（不需数据库）:
1. Model:      Core/Models/NewFeature.cs           ← POCO 模型

// 通用步骤:
3. Interface:  Core/Interfaces/INewFeatureService.cs
4. Service:    Services/Services/NewFeatureService.cs
   → 若涉及数据库操作，注入 IDbContextFactory<ArasToolkitDbContext>
   → 所有写操作必须调用 IOperationLogService.LogAsync() 记录日志
5. ViewModel:  App/ViewModels/NewFeatureViewModel.cs
6. View:       App/Views/NewFeatureView.xaml + .cs
7. 注册DI:
   - Services/ServiceCollectionExtensions.cs → AddSingleton<IXxx, Xxx>()
   - App/App.xaml.cs → AddTransient<XxxVM>(), AddTransient<XxxView>()
8. 菜单:       ViewModels/MainViewModel.cs → MenuItems.Add()
9. 导航:       MainWindow.xaml.cs → NavigateToPage switch
```

## 五、密码/配置规范

```
- 密码存储: MD5（32位小写十六进制），不存明文
- 配置格式: JSON，位于 {BaseDir}/Config/
- 去重规则: Url+Database+Username 相同不重复
- 最后登录: Config/AppSettings/lastLogin.json
- 错误日志: Config/ErrorLogs/errors.json（SemaphoreSlim 线程安全）
```

## 六、当前主题配色

```
浅色简约风格
- 主背景: #FFFFFF   侧边栏: #F8F9FB
- 卡片:   #FFFFFF + #E5E7EB 边框
- 主文字: #111827   辅助文字: #6B7280
- 强调色: #6366F1   悬停: #4F46E5
- 输入框: #F9FAFB 圆角 8px
```

## 七、Aras API 常用参考

```csharp
// Innovator 常用方法
innovator.applyItem(item);                       // 创建/更新 Item
innovator.applyMethod("MethodName", "<body/>");  // 调用服务器方法
innovator.newItem("ItemType", "action");          // 新建 Item (action: get/add/update/delete)
innovator.getItemById("ItemType", "id");          // 按 ID 获取 Item
innovator.applySQL("SELECT ...");                 // 执行 SQL

// Item 常用操作
item.setProperty("property_name", value);         // 设置属性
item.getProperty("property_name");                // 获取属性
item.setAttribute("attribute", value);            // 设置特性
item.getRelatedItem();                            // 获取关联 Item
item.getItemsByXPath("//Item[@type='Part']");     // XPath 查询
item.setAction("add");                            // 设置操作类型

// 常用 ItemType: Part, Document, CAD, BOM, ECR, ECO, User, Identity
```

## 八、仪表盘与菜单规范 ⚠️ 重要

### 8.1 菜单层级结构

```
🏠 仪表盘          → DashboardView（总览 + 全部功能卡片入口）
📊 导入表格        → ExcelImportView
🌐 系统翻译 ▸      → 子仪表盘（卡片列表）
   ├─ 🔤 字段翻译
   ├─ 📝 表单翻译
   └─ 🪟 窗体翻译
⚙ 系统配置 ▸      → 子仪表盘（卡片列表）
   ├─ 🖼️ 窗体配置
   ├─ 📦 对象类配置
   ├─ 🔧 属性配置
   ├─ 📋 List配置
   └─ 🔒 权限配置
📋 系统日志 ▸      → 子仪表盘（卡片列表）
   ├─ 📜 更新日志
   ├─ 🐛 错误日志
   └─ 🔒 敏感操作日志
```

### 8.2 铁律（必须遵守）

1. **新增功能必须同时在以下位置注册入口：**
   - `MainViewModel.InitializeMenuItems()` — 侧边栏菜单项 + `CardIcon` emoji
   - `MainWindow.xaml.cs` → `NavigateToPage()` switch — 名称→View 映射
   - `DashboardViewModel.AllFeatures` — 主仪表盘"全部功能"卡片列表
   - 若功能属于某个父节点（系统翻译/系统配置），则加入父节点的 `Children`

2. **仪表盘卡片规范：**
   - 主仪表盘（DashboardView）和子仪表盘（PlaceholderView）**必须**使用相同样式：WrapPanel 布局 + emoji 图标 + CardStyle
   - 卡片宽度 260，3列排列
   - 图标使用 emoji（`CardIcon` 属性），字体大小 28
   - 卡片**必须可点击**，点击后跳转到对应 View

3. **子仪表盘行为：**
   - 点击侧边栏父节点 → 展开/折叠子菜单 + 右侧显示子仪表盘（子功能卡片列表）
   - 卡片使用 `MenuItemInfo.CardIcon` 作为图标
   - 卡片点击 → 通过 `NavigateToPage(name)` 导航

4. **菜单项 `MenuItemInfo` 模型字段：**
   - `Name` — 菜单名/导航名
   - `Icon` — 侧边栏图标标识
   - `CardIcon` — 仪表盘卡片 Emoji 图标
   - `Description` — 功能描述
   - `Children` — 子菜单项列表
   - `IsPlaceholder` — 是否为占位项（未实现功能）

5. **导航方法：**
   - `MainWindow.NavigateToPage(string name)` — 公开方法，供仪表盘/子仪表盘卡片调用
   - `MainWindow.NavigateToPage(MenuItemInfo)` — 内部方法，按名称 switch 分发

### 8.3 新增功能检查清单

```
□ MainViewModel.InitializeMenuItems() — 添加菜单项 + CardIcon
□ MainWindow.xaml.cs NavigateToPage switch — 添加名称→View 映射
□ DashboardViewModel.AllFeatures — 添加 QuickAction 卡片（含emoji图标）
□ 若为子功能，加入父节点 Children 列表
□ DI 注册（Services/ServiceCollectionExtensions + App/App.xaml.cs）
□ 创建 View + ViewModel + Service + Model
```

---

## 九、EF Core 数据库规范 ⚠️ 重要

### 9.1 目录与命名规范

```
Core/Entities/               ← 所有 EF 数据库映射实体
  ├── PersonalTask.cs        ← 对应 personal_task 表
  └── OperationLog.cs        ← 对应 operation_log 表

Services/Data/
  └── ArasToolkitDbContext.cs ← 唯一数据库上下文
```

**铁律：**
- EF 实体类名**必须**与数据库表名一致（snake_case → PascalCase）
  - `personal_task` → `PersonalTask`
  - `operation_log` → `OperationLog`
- EF 实体统一放在 `Core/Entities/`，命名空间 `ArasToolkit.Core.Entities`
- 普通 POCO 模型放在 `Core/Models/`，命名空间 `ArasToolkit.Core.Models`
- DbContext 放在 `Services/Data/`，命名空间 `ArasToolkit.Services.Data`
- 每个数据库表对应一个 Entity 文件，不允许多个实体混在一个文件中

### 9.2 实体映射规范

```csharp
[Table("table_name")]          // 指定数据库表名（必须与数据库一致）
public class TableName
{
    [Key]                       // 主键
    [Column("column_name")]     // 列名映射（snake_case）
    [MaxLength(100)]            // 字符串长度
    [Required]                  // 非空约束
    [NotMapped]                 // 不映射到数据库的字段
    public string PropertyName { get; set; }
}
```

- 列名使用 `[Column("snake_case")]` 映射数据库列名
- 不在数据库中的属性用 `[NotMapped]`
- 计算属性（仅 getter）统一标记 `[NotMapped]`

### 9.3 DbContext 注册

```csharp
// ServiceCollectionExtensions.cs
services.AddDbContextFactory<ArasToolkitDbContext>();
// 使用 IDbContextFactory 模式，每次操作创建短生命周期 DbContext（线程安全）
// 连接字符串在 ArasToolkitDbContext.OnConfiguring() 中从 DBSeeting.json 读取
```

### 9.4 DBSeeting.json 连接字符串

```json
// 文件位置: src/ArasToolkit.Core/DBSeeting.json
// 构建时自动复制到输出目录（App.csproj → CopyConfigFiles Target）
{
  "sql": "Data Source=192.168.0.58,1437;Initial Catalog=Kong.A;..."
}
```

### 9.5 操作日志规范

- **所有数据库写操作**（Create/Update/Delete/Import）**必须**通过 `IOperationLogService.LogAsync()` 记录日志
- 服务实现中注入 `IOperationLogService`，每次 SaveChanges 后调用
- 日志写入失败**不能**阻止主业务流程（try-catch 包裹，Debug 输出）
- 操作类型: `"Create"`, `"Update"`, `"Delete"`, `"Import"`
- 实体类型: 使用 Entity 类名，如 `"PersonalTask"`

```csharp
// 标准写法（在 Service 的每个写操作末尾调用）
await _operationLogService.LogAsync("Create", "PersonalTask", item.Id,
    $"创建任务: {item.TaskName}");
```

### 9.6 数据库初始化策略 ⚠️

```
EF Core 实体仅用于「映射」已有数据库表，不自动创建或修改表结构。
- ❌ 禁止在启动时调用 EnsureCreated() / Migrate()
- ❌ 禁止自动执行 DDL 操作
- ✅ 实体仅映射已有表，列名与数据库完全一致
- ✅ 仅在用户明确要求时（如"执行迁移"）才生成/修改表结构
```

### 9.7 creator_on 列规范 ⚠️ 必须遵守

```
所有数据库表必须包含 creator_on 列，作为记录创建时间：

铁律：
- ✅ 所有新表必须添加 creator_on DATETIME2 NOT NULL DEFAULT GETDATE()
- ✅ 所有实体必须添加 CreatorOn 属性（DateTime 类型，默认值 DateTime.Now）
- ✅ 所有列表查询必须以 CreatorOn DESC 倒序排序（最新记录在前）
- ✅ 新增记录时必须显式设置 CreatorOn = DateTime.Now
- ✅ DbContext OnModelCreating 中必须映射 CreatorOn → creator_on
- ✅ EnsureSchemaAsync 中必须包含 creator_on 列的同步逻辑
```

```csharp
// 实体中的标准写法
[Column("creator_on")]
public DateTime CreatorOn { get; set; } = DateTime.Now;

// Service 中新增记录时
item.CreatorOn = DateTime.Now;

// 所有列表查询排序
.OrderByDescending(x => x.CreatorOn)
```

### 9.8 新增数据库表步骤

```
1. Core/Entities/NewTable.cs         ← 创建实体，类名=表名(PascalCase)
2. Services/Data/ArasToolkitDbContext.cs → DbSet<NewTable> + OnModelCreating 配置
3. Services/Services/NewTableService.cs  → 注入 IDbContextFactory + IOperationLogService
4. 遵循操作日志规范（每条写操作记录日志）
5. Core/Interfaces/INewTableService.cs   → 服务接口
6. DI 注册: ServiceCollectionExtensions → AddSingleton<INewTableService, NewTableService>()
```

### 9.9 模块联动检查规则 ⚠️ 必须遵守

```
每当修改某个功能的以下任一层面时，必须同步检查并更新该模块的所有相关部分：
- 实体（Entity）字段变更 → 检查 DbContext映射、Service逻辑、ViewModel、View(XAML列/表单)、导出模板、导入逻辑
- 列顺序/列名变更 → 检查 XAML DataGrid、编辑表单、导出模板表头、导入列映射
- 服务接口变更 → 检查 Service实现、ViewModel调用、DI注册

典型联动清单（以Todo模块为例）：
□ PersonalTask.cs          → 实体字段
□ ArasToolkitDbContext.cs  → OnModelCreating 列映射
□ ITodoService.cs          → 接口方法签名
□ TodoService.cs           → CRUD + Excel导出模板表头 + 导入列映射
□ TodoViewModel.cs         → 属性/命令/编辑拷贝
□ TodoView.xaml            → DataGrid列 + 编辑表单字段
□ TodoView.xaml.cs         → Code-behind事件处理
```

### 9.10 错误日志规范 ⚠️ 所有异常必须记录

**错误级别定义：**
| 级别 | 含义 | 适用场景 |
|------|------|---------|
| `P0-致命` | 致命错误 | 数据库连接失败、服务崩溃、无法恢复的异常 |
| `P1-普通` | 普通错误 | 操作失败、导出/导入失败、业务逻辑异常 |

**铁律：**
- Service 层所有 catch 块**必须**调用 `_errorLogService.LogErrorAsync()` 写入错误日志
- ViewModel 层所有 catch 块**必须**调用 `_errorLogService.LogErrorAsync()` 写入错误日志
- P0 用于 DB 连接/操作失败，P1 用于文件操作/业务异常
- 错误日志写入失败不能阻止主流程（ErrorLogService 自身异常静默处理）

```csharp
// Service 层示例
catch (Exception ex)
{
    await _errorLogService.LogErrorAsync("Todo-新增", ex.Message,
        ErrorLog.LevelP0, ex.StackTrace);
    throw;
}

// ViewModel 层示例
catch (Exception ex)
{
    StatusMessage = $"加载失败: {ex.Message}";
    await _errorLogService.LogErrorAsync("Todo-加载列表", ex.Message,
        ErrorLog.LevelP1, ex.StackTrace);
}
```

---

## 十、更新日志写入规则 ⚠️ 必须遵守

### 10.1 写入时机

每次完成一个完整的大功能后，**必须**通过 `IChangelogService.AddEntryAsync()` 写入更新日志。

### 10.2 版本号规则

```
- 若用户未指定大版本号（如 v1.1、v2.0），则小版本号 +1（如 1.0.0 → 1.0.1）
- 当前版本号通过 IChangelogService.GetCurrentVersion() 获取最新版本
- 大版本号变更需用户明确指示（如"发布 v2.0"）
```

### 10.3 写入示例

```csharp
// 在完成大功能后，通过 IChangelogService 写入
var changelogService = App.Services.GetRequiredService<IChangelogService>();
var currentVersion = changelogService.GetCurrentVersion();

// 小版本递增（如 1.0.0 → 1.0.1）
var parts = currentVersion.Split('.');
parts[2] = (int.Parse(parts[2]) + 1).ToString();
var newVersion = string.Join('.', parts);

await changelogService.AddEntryAsync(new Changelog
{
    Version = newVersion,
    ReleaseDate = DateTime.Now,
    Type = "新增",       // 新增/修复/优化/移除
    Description = "功能描述（简洁明确）",
    Author = "开发团队"
});
```

### 10.4 日志类型说明

| 类型 | 适用场景 |
|------|---------|
| `新增` | 新功能、新模块、新页面 |
| `修复` | Bug 修复、异常处理 |
| `优化` | 性能优化、代码重构、UI 改进 |
| `移除` | 删除废弃功能 |

### 10.5 铁律

- ✅ 每个完整大功能完成后，必须写入一条更新日志
- ✅ 版本号自动递增（小版本 +1），除非用户指定大版本
- ✅ 描述简洁明确，说明做了什么
- ✅ Author 统一填写 "开发团队"
- ❌ 不得跳过写入
- ❌ 不得在没有用户指示时自行升级大版本号

---

> 此技能在打开 ArasToolkit 项目时自动加载。可根据实际开发经验持续更新。



## 十一、Git 分支与推送策略 ⚠️ 必须遵守

### 11.1 分支结构

```
master   → 最新版本（唯一推送目标，云端源码托管主分支）
develop  → 历史版本归档（保留上一轮推送的完整提交历史，用于回溯/还原）
```

### 11.2 推送规则（铁律）

1. **每次推送前必须先备份当前 master 到 develop：**
   - 推送前：`git checkout develop && git merge master && git push origin develop`
   - 推送时：`git checkout master && git push origin master --force`
   - 目的：保证 `develop` 始终保留上一轮完整历史，可随时恢复

2. **master 使用 force push：**
   - 本地 master 是唯一代码真相来源
   - 远程 master 通过 `git push origin master --force` 覆盖，保持与本地完全一致

3. **develop 保留完整线性历史：**
   - 存放所有历史版本提交记录
   - 每次 master 推送前，将 master 合并到 develop，确保历史不丢失
   - develop 包含完整的 Todo 模块 v1.0.1 ~ v1.0.6 开发历史

### 11.3 恢复操作

```bash
# 从 develop 恢复上一版本到 master
git checkout master
git reset --hard develop~1   # develop~1 = 上一轮推送前的 master
git push origin master --force
```

---

> 此技能在打开 ArasToolkit 项目时自动加载。可根据实际开发经验持续更新。


---

---

## 十二、Codex 与 Claude Code 协作规范 ⚠️ 必须遵守

### 12.1 角色分工

| AI 助手 | 职责 |
|---------|------|
| **Codex**（当前对话） | 代码编写、自动化管理、功能实装 |
| **Claude Code** | 任务分析、方案输出到公共文件、代码审查、项目监听与状态监控、排版与文档格式化 |

### 12.2 协作流程（迭代闭环）

```
Claude Code ──方案输出──→ tasks/TASK-NNN.md (status: pending)
                              ↓
                         Codex 编写代码
                              ↓
                         tasks/ 写入检查清单 (status: pending_review)
                              ↓
                     ┌── Claude Code 审查 ─────────┐
                     │                              │
                     │  审查结论:「需修改: ...」       │  审查结论:「通过」
                     │                              │
                     ↓                              ↓
                  Codex 修正                      review_passed
                  (status 改回 pending_review)       │
                     │                              ↓
                     └──→ 返回审查 ←──┘        Codex 读取结论:
                                                  │ 「通过」→ 编译 → done/
                                                  │ 「需修改」→ 继续修正循环
                                                  ↓
                                            Codex 编译验证
                                                  │
                                                  ↓
                                            status: done
                                                  │
                                                  ↓
                                           移入 tasks/done/
```

**各步骤说明：**

| 步骤 | 执行者 | 输出 | status 变更 |
|------|--------|------|-------------|
| 1. 方案输出 | Claude Code | 分析/技术方案 → `tasks/TASK-NNN.md` | `pending` |
| 2. 代码编写 | Codex | 编码、修改项目文件 | 不变 |
| 3. 输出检查清单 | Codex | 检查清单写入同文件 | `pending` → `pending_review` |
| 4. 代码审查 | Claude Code | 审查意见、通过/修改 | `pending_review` → `review_passed` |
| 5. 代码修正 | Codex | 按审查意见修改代码，status 改回 `pending_review` | 等待再次审查 |
| 6. 再次审查 | Claude Code | 确认修改已修复，写出审查结论 | 同步骤 4 |
| 7. 编译验证 | Codex | `dotnet build` | `review_passed` → `done` → 移入 `done/` |
| 8. 自动化监听 | 自动化任务 | 扫描任务 + 状态检查 | 持续运行 |

### 12.3 任务文件生命周期（状态流转）

```
pending                 ← Claude Code 写完方案到 tasks/
  ↓
pending_review          ← Codex 写完代码 + 检查清单，等待审查
  ↓
review_passed           ← Claude Code 第一次审查通过
  ↑↓ (可循环多次)
pending_review          ← Codex 按审查意见修正后，再次等待审查
  ↓
done                    ← Codex 编译通过，任务完成
                          （文件移入 tasks/done/）
```

**铁律：**
- ✅ 新任务放 `tasks/` 根目录，`status: pending`
- ✅ Codex 编码后将 status 改为 `pending_review`，写入检查清单
- ✅ Claude Code 审查**必须**写出明确的审查结论，供 Codex 判断下一步
- ✅ 审查结论为「通过」→ `review_passed`，Codex 执行编译归档
- ✅ 审查结论为「需修改: ...」→ 保持 `review_passed`，Codex 按意见修改后 status 改回 `pending_review` 等待再次审查
- ✅ 可以多次循环，直到审查通过
- ✅ 审查通过后 Codex 编译，通过后 status 改为 `done`，移入 `tasks/done/`
- ❌ 新任务不要直接放 `tasks/done/`
- ❌ Codex 不要在审查通过前执行构建
- ✅ 每次编码前必须执行 `git stash push -m "backup-before-TASK-NNN"` 备份工作区
- ❌ 已完成审查报告可直接放 `tasks/done/`（如 TASK-006、TASK-007）
- ❌ Claude Code 不要自行将文件移入 `tasks/done/`（归档由 Codex 统一执行）

`
tasks/                                   ← Claude Code 放置新任务（status: pending）
├── TASK-NNN.md  status: pending         ← Codex 读取并编码
├── TASK-NNN.md  status: pending_review  ← Codex 写入检查清单，等审查
├── TASK-NNN.md  status: review_passed   ← Claude Code 审查通过
└── done/                                ← 编译验证通过后移入
    └── TASK-NNN.md  status: done        ← Codex 完成编译
`

**生命周期状态流转：**
`
pending → (Claude Code写完方案)
       → pending_review → (Codex写完代码+检查清单，等待审查)
                       → review_passed → (Claude Code审查通过)
                                       → done → (Codex编译通过，移入done/)
`

**铁律：**
- ✅ 新任务文件**必须**放在 tasks/ 根目录，status 设为 pending
- ✅ Codex 编码后**必须**将 status 改为 pending_review，写入检查清单
- ✅ Claude Code 审查通过后**必须**将 status 改为 review_passed
- ✅ Codex 收到 review_passed 后执行编译，通过后改为 done 并移入 tasks/done/
- ❌ 新任务不要直接放入 tasks/done/（否则自动化无法识别）
- ❌ 已经完成的审查报告可直接放 tasks/done/（如 TASK-005）
- ❌ Codex 不要在审查通过前执行构建（避免提前编译导致审查流于形式）

### 12.4 核心原则

- 各自发挥所长，各司其职，不强求对方做不擅长的事
- Claude Code 专注于「想清楚怎么说/怎么组织」，输出高质量的分析/方案/审查意见到公共文件
- Codex 专注于「写代码把它做出来」，把方案变成可运行的实现
- Codex 编码完成后必须输出检查任务清单，触发 Claude Code 审查环节
- 不越界：Codex 不做深度分析与排版，Claude Code 不直接写代码

### 12.5 自动化监听模式

- 自动化任务以 5 分钟为间隔进入监听模式（通过 Codex App 的 heartbeat 机制）
- 每次唤醒执行：
  1. git status + git log 检查文件变更和提交历史
  2. dotnet build --no-restore 检查编译状态
  3. 扫描 tasks/ 根目录下 status: pending 的 TASK-*.md 文件
  4. 输出格式化报告（分支/提交/待处理任务/变更/构建状态/下一步建议）
- 自动化 ID（用于 Claude Code 感知）：automation（Codex App 心跳自动化）

---

> 此技能在打开 ArasToolkit 项目时自动加载。可根据实际开发经验持续更新。


