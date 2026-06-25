---
id: TASK-029
priority: P0
type: feat
created: 2026-06-26
source: Claude Code
status: pending_review
---

# Aras 登录信息管理 — 新增保存记录 + 连接后替换全局单例

## 问题描述

`ArasLoginWindow`（Aras 登录信息管理）当前只有「连接」和「删除」按钮：

```
当前:
  [连接] [删除]   ← 只能切换/删除已有记录，无法新增

期望:
  [＋ 新增] [连接] [删除]   ← 支持手动添加登录信息记录
```

用户无法在该窗口中直接添加新的 Aras 服务器登录信息记录。点击「连接」后应替换当前 `ArasConnectionService` 全局单例持有的连接对象，同时重新初始化 `ArasConnectionPool`。

## 根因分析

| 问题 | 根因 |
|------|------|
| 无法新增记录 | `ArasLoginWindow` 只有 ItemsControl 列表 + 连接/删除按钮，无新增入口 |
| 连接后连接池未更新 | `ArasLoginViewModel.ConnectAsync` 只调 `LoginService.LoginAsync` 更新 `ArasConnectionService`，未重建连接池 |

## 核心设计

```
新增记录:
  填写表单(URL/数据库/用户名/密码) → SaveCommand
    → LoginService.LoginAsync 测试连接可用性
    → ConfigService.SaveLoginInfoAsync 持久化到 JSON
    → 刷新列表

点击连接:
  ConnectCommand → LoginService.LoginAsync
    → ArasConnectionService.SetConnection(...)  替换全局单例
    → ArasConnectionPool.Reinitialize(10)       使用新连接信息重建池
    （旧连接由 Disconnect 或 SetConnection 内部清理）

连接池生命周期:
  - 池中连接: 创建后持续复用，导入完成后归还池中 → 不销毁
  - 全局单例 _connectionService.HttpConnection: 仅在 Disconnect() / SetConnection 替换时调 Logout()
  - 导入完成: 回收连接回池，不销毁任何连接
```

## 涉及文件

| 文件 | 改动 |
|------|------|
| `Core/Interfaces/IArasConnectionPool.cs` | **新建** — 连接池接口，`Reinitialize` 方法 |
| `Services/Services/ArasConnectionPool.cs` | 实现接口 + 注入 `IArasConnectionService` + `Reinitialize` |
| `Services/Services/ArasConnectionService.cs` | `SetConnection` 中先 Logout 旧连接再替换（若替换时旧连接存在） |
| `Services/ServiceCollectionExtensions.cs` | 注册 `IArasConnectionPool` 接口 |
| `App/ViewModels/ArasLoginViewModel.cs` | 新增 `SaveCommand` + 表单属性 + 连接后调用 `_connectionPool.Reinitialize(10)` |
| `App/Views/ArasLoginWindow.xaml` | 添加「＋ 新增」按钮 + 可折叠新增表单 |
| `App/Views/ArasLoginWindow.xaml.cs` | PasswordBox PasswordChanged 事件同步 |

## 详细实施方案

### 步骤 1：新建连接池接口 — `Core/Interfaces/IArasConnectionPool.cs`

```csharp
namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Aras 连接池接口 — 管理多个独立 Aras 连接，供多线程导入使用。
/// 池中连接在导入完成后归还复用，不自动销毁。
/// </summary>
public interface IArasConnectionPool
{
    /// <summary>当前池大小</summary>
    int PoolSize { get; }

    /// <summary>池中可用连接数</summary>
    int Available { get; }

    /// <summary>
    /// 使用当前 IArasConnectionService.CurrentConnection 的信息重新初始化连接池。
    /// 先清空旧池（Logout 旧连接），再创建新连接。
    /// poolSize=0 时仅清空。
    /// </summary>
    void Reinitialize(int poolSize);

    /// <summary>清空连接池，注销所有池中连接</summary>
    void Clear();
}
```

### 步骤 2：ArasConnectionPool 实现接口 — `Services/Services/ArasConnectionPool.cs`

```csharp
// 在现有类上实现接口 + 注入 IArasConnectionService + 新增 Reinitialize
public class ArasConnectionPool : IArasConnectionPool
{
    private readonly IArasConnectionService _connectionService;
    private readonly ConcurrentBag<PooledConnection> _pool = [];

    public ArasConnectionPool(IArasConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    /// <summary>
    /// 从全局单例 CurrentConnection 读取登录信息重建连接池。
    /// 切换全局连接后调用此方法同步池状态。
    /// </summary>
    public void Reinitialize(int poolSize)
    {
        Clear(); // 先清空旧池（Logout 旧连接）

        var info = _connectionService.CurrentConnection;
        if (info == null || poolSize <= 0)
        {
            PoolSize = 0;
            return;
        }

        poolSize = Math.Min(poolSize, 10); // 线程上限10
        PoolSize = poolSize;

        for (int i = 0; i < poolSize; i++)
        {
            // 使用标准 Aras IOM 登录链创建独立连接
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(
                info.Url, info.Database, info.Username, info.Md5Password);
            var innovator = new Innovator(conn);
            _pool.Add(new PooledConnection(conn, innovator, i));
        }
    }

    // ... 其余 Rent/Return/Clear 方法保持不变
}
```

### 步骤 3：DI 注册 — `Services/ServiceCollectionExtensions.cs`

```csharp
// 注册接口（指向具体类型单例）
services.AddSingleton<IArasConnectionPool>(sp => sp.GetRequiredService<ArasConnectionPool>());
services.AddSingleton<ArasConnectionPool>();  // 保留具体类型注册，供 DataImportService 直接注入
```

### 步骤 4：ArasConnectionService — 替换时先清理旧连接

```csharp
// SetConnection 方法中，先 Logout 旧连接再替换
public void SetConnection(ArasConnectionInfo connectionInfo, object innovator, object httpConnection)
{
    // 如果之前已有连接，先注销旧连接
    if (_httpConnection != null && _httpConnection != (httpConnection as HttpServerConnection))
    {
        try { _httpConnection.Logout(); }
        catch { /* 静默 */ }
    }

    _currentConnection = connectionInfo;
    _currentConnection.LoginTime = DateTime.Now;
    _innovator = innovator as Innovator;
    _httpConnection = httpConnection as HttpServerConnection;
}
```

### 步骤 5：ArasLoginViewModel — 新增保存 + 连接池同步

**5a. 构造函数注入 + 新增属性：**

```csharp
private readonly IArasConnectionPool _connectionPool; // ← 新增

// 新增表单绑定属性
private string _newUrl = string.Empty;
private string _newDatabase = string.Empty;
private string _newUsername = string.Empty;
private string _newPassword = string.Empty;
private bool _isAdding;

public string NewUrl { get => _newUrl; set => SetProperty(ref _newUrl, value); }
public string NewDatabase { get => _newDatabase; set => SetProperty(ref _newDatabase, value); }
public string NewUsername { get => _newUsername; set => SetProperty(ref _newUsername, value); }
public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
public bool IsAdding { get => _isAdding; set => SetProperty(ref _isAdding, value); }

public ArasLoginViewModel(
    IConfigService configService,
    ILoginService loginService,
    IArasConnectionService connectionService,
    IErrorLogService errorLogService,
    IArasConnectionPool connectionPool) // ← 新增
{
    // ... 原有赋值 ...
    _connectionPool = connectionPool;

    SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
    ShowAddFormCommand = new RelayCommand(_ => IsAdding = !IsAdding);
}

public ICommand SaveCommand { get; }
public ICommand ShowAddFormCommand { get; }
```

**5b. 保存逻辑：**

```csharp
private bool CanSave()
{
    return !IsProcessing
        && !string.IsNullOrWhiteSpace(NewUrl)
        && !string.IsNullOrWhiteSpace(NewDatabase)
        && !string.IsNullOrWhiteSpace(NewUsername)
        && !string.IsNullOrWhiteSpace(NewPassword);
}

/// <summary>
/// 新增登录信息记录 — 先测试连接 → 再持久化保存到 JSON
/// </summary>
private async Task SaveAsync()
{
    IsProcessing = true;
    ErrorMessage = string.Empty;
    StatusMessage = "正在验证连接...";

    try
    {
        var loginInfo = new LoginInfo
        {
            Url = NewUrl.Trim(),
            Database = NewDatabase.Trim(),
            Username = NewUsername.Trim(),
            Password = NewPassword,
            RememberMe = true,
            ConfigName = $"login_{DateTime.Now:yyyyMMddHHmmss}"
        };

        // 去重检查 — 相同 Url+Database+Username 不重复保存
        var isDuplicate = await _configService.IsDuplicateLoginInfoAsync(loginInfo);
        if (isDuplicate)
        {
            ErrorMessage = "该连接记录已存在（相同 URL + 数据库 + 用户名）";
            return;
        }

        // 测试连接是否可用
        await _loginService.LoginAsync(loginInfo);

        // 持久化保存 LoginInfo 到 Config/Login/*.json
        await _configService.SaveLoginInfoAsync(loginInfo);

        StatusMessage = $"已保存: {NewUsername}@{NewDatabase}";

        // 清空表单 + 关闭表单 + 刷新列表
        NewUrl = NewDatabase = NewUsername = NewPassword = string.Empty;
        IsAdding = false;
        await LoadAsync();
    }
    catch (Exception ex)
    {
        ErrorMessage = "保存失败: " + ex.Message;
        await _errorLogService.LogErrorAsync("Aras登录-新增", ex.Message,
            ErrorLog.LevelP1, ex.StackTrace);
    }
    finally { IsProcessing = false; }
}
```

**5c. 连接后同步连接池：**

```csharp
private async Task ConnectAsync(object? parameter)
{
    if (parameter is not LoginInfo info) return;

    // ... 原有 IsProcessing/StatusMessage 设置 ...

    try
    {
        await _loginService.LoginAsync(info);
        // LoginAsync 内部已调用 _connectionService.SetConnection(...) 替换全局单例

        // 同步重建连接池 — 池中连接使用新的全局连接信息
        _connectionPool.Reinitialize(poolSize: 10);

        StatusMessage = "已切换到 " + info.Username + "@" + info.Database;
    }
    // ... catch/finally 保持不变 ...
}
```

### 步骤 6：ArasLoginWindow.xaml — 添加新增按钮 + 折叠表单

**6a. 标题栏添加「＋ 新增」按钮：**

```xml
<Grid Grid.Row="0">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>
    <StackPanel Grid.Column="0">
        <TextBlock Text="Aras 登录信息管理" FontSize="18" FontWeight="Bold"
                   Foreground="{StaticResource TextPrimaryBrush}" />
        <TextBlock Text="查看和管理已保存的 Aras 连接配置" FontSize="12"
                   Foreground="{StaticResource TextSecondaryBrush}" Margin="0,4,0,16" />
    </StackPanel>
    <!-- 新增按钮 -->
    <Button Grid.Column="1" Content="＋ 新增" Style="{StaticResource PrimaryButton}"
            Height="28" FontSize="12" Padding="10,0" Margin="0,0,8,0"
            Command="{Binding ShowAddFormCommand}" VerticalAlignment="Top"
            Visibility="{Binding IsAdding, Converter={StaticResource BoolToVisibility}, ConverterParameter=Invert}" />
    <Button Grid.Column="2" Content="✕" Style="{StaticResource WindowButton}"
            Click="CloseButton_Click" VerticalAlignment="Top" />
</Grid>
```

**6b. 窗口高度从 480 → 600 以容纳新增表单**

**6c. Grid.RowDefinitions 调整：**

```xml
<Grid.RowDefinitions>
    <RowDefinition Height="Auto" />   <!-- 0: 标题栏 -->
    <RowDefinition Height="*" />      <!-- 1: 连接列表 -->
    <RowDefinition Height="Auto" />   <!-- 2: 新增表单（可折叠） -->
    <RowDefinition Height="Auto" />   <!-- 3: 状态栏 -->
</Grid.RowDefinitions>
```

**6d. 新增表单（Grid.Row="2"）：**

```xml
<Border Grid.Row="2" Margin="0,10,0,0" Padding="14,12"
        Background="{StaticResource CardBrush}"
        BorderBrush="{StaticResource BorderBrush}" BorderThickness="1"
        CornerRadius="8"
        Visibility="{Binding IsAdding, Converter={StaticResource BoolToVisibility}}">
    <StackPanel>
        <TextBlock Text="新增 Aras 连接记录" FontSize="14" FontWeight="SemiBold"
                   Foreground="{StaticResource TextPrimaryBrush}" Margin="0,0,0,10" />
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
            </Grid.RowDefinitions>

            <TextBlock Grid.Row="0" Grid.Column="0" Text="URL:" VerticalAlignment="Center"
                       Foreground="{StaticResource TextSecondaryBrush}" Margin="0,4,8,4" />
            <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding NewUrl, UpdateSourceTrigger=PropertyChanged}"
                     Margin="0,2" />

            <TextBlock Grid.Row="1" Grid.Column="0" Text="数据库:" VerticalAlignment="Center"
                       Foreground="{StaticResource TextSecondaryBrush}" Margin="0,4,8,4" />
            <TextBox Grid.Row="1" Grid.Column="1" Text="{Binding NewDatabase, UpdateSourceTrigger=PropertyChanged}"
                     Margin="0,2" />

            <TextBlock Grid.Row="2" Grid.Column="0" Text="用户名:" VerticalAlignment="Center"
                       Foreground="{StaticResource TextSecondaryBrush}" Margin="0,4,8,4" />
            <TextBox Grid.Row="2" Grid.Column="1" Text="{Binding NewUsername, UpdateSourceTrigger=PropertyChanged}"
                     Margin="0,2" />

            <TextBlock Grid.Row="3" Grid.Column="0" Text="密码:" VerticalAlignment="Center"
                       Foreground="{StaticResource TextSecondaryBrush}" Margin="0,4,8,4" />
            <PasswordBox Grid.Row="3" Grid.Column="1" x:Name="NewPasswordBox"
                         PasswordChanged="NewPasswordBox_PasswordChanged" Margin="0,2" />

            <StackPanel Grid.Row="4" Grid.Column="1" Orientation="Horizontal"
                        HorizontalAlignment="Right" Margin="0,8,0,0">
                <Button Content="取消" Height="28" FontSize="12" Padding="12,0"
                        Command="{Binding ShowAddFormCommand}" Margin="0,0,8,0" />
                <Button Content="保存并验证" Height="28" FontSize="12" Padding="12,0"
                        Style="{StaticResource PrimaryButton}"
                        Command="{Binding SaveCommand}" />
            </StackPanel>
        </Grid>
    </StackPanel>
</Border>
```

### 步骤 7：ArasLoginWindow.xaml.cs — PasswordBox 同步

```csharp
private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
{
    if (DataContext is ArasLoginViewModel vm)
        vm.NewPassword = ((PasswordBox)sender).Password;
}
```

## 联动检查清单

- [ ] Core/Interfaces/IArasConnectionPool.cs — 新建接口
- [ ] Services/Services/ArasConnectionPool.cs — 实现 IArasConnectionPool + 构造函数注入 IArasConnectionService + Reinitialize
- [ ] Services/Services/ArasConnectionService.cs — SetConnection 中先 Logout 旧连接
- [ ] Services/ServiceCollectionExtensions.cs — 注册 IArasConnectionPool 接口
- [ ] App/ViewModels/ArasLoginViewModel.cs — 注入 IArasConnectionPool + SaveCommand + 表单属性 + 连接后 Reinitialize(10)
- [ ] App/Views/ArasLoginWindow.xaml — 新增按钮 + 折叠表单 + Grid.Row 调整
- [ ] App/Views/ArasLoginWindow.xaml.cs — PasswordChanged 事件同步
- [ ] App/App.xaml.cs — DI 无需改动

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 0 errors
- [ ] 启动 → 登录 → 点击「🔆 Aras登录信息」→ 点击「＋ 新增」→ 填写表单 → 保存 → 列表刷新
- [ ] 点击列表中某条记录的「连接」→ 全局单例 + 连接池同步更新
- [ ] 进入数据汇入设线程数=3 → 执行导入 → 验证使用新连接

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


## Git 提交规范

```
修复完成后执行:
1. git add -A && git status
2. git commit -m "feat: TASK-029 Aras登录信息管理 — 新增保存记录+连接后替换全局单例+连接池同步"
3. git push origin master --force
```


---

## Codex 实现检查清单

| # | 检查项 | 状态 |
|---|--------|------|
| 1 | IArasConnectionPool 接口 — PoolSize/Available/Reinitialize/Clear | ✅ |
| 2 | ArasConnectionPool 实现接口 + 注入 IArasConnectionService + Reinitialize | ✅ |
| 3 | ArasConnectionService.SetConnection 替换前先 Logout 旧连接 | ✅ |
| 4 | ServiceCollectionExtensions 注册 IArasConnectionPool | ✅ |
| 5 | ArasLoginViewModel SaveCommand + 表单属性 + 连接后 Reinitialize(10) | ✅ |
| 6 | ArasLoginWindow.xaml ＋新增按钮 + 折叠表单(URL/DB/User/Password) | ✅ |
| 7 | ArasLoginWindow.xaml.cs PasswordBox 事件 + 表单切换逻辑 | ✅ |
| 8 | 编译验证 | ✅ 通过 (0 errors) |
