---
id: TASK-028
priority: P0
type: feat
created: 2026-06-26
source: Claude Code
status: pending_review
---

# 数据汇入并发化 — 真实暂停/继续 + Aras连接池 + 多线程执行

## 问题描述

当前 `DataImportService.ExecuteImportAsync` 存在三个致命缺陷：

1. **暂停/继续是假的** — `_cts` 创建了但从未传入 Service，循环停不下来
2. **纯单线程串行** — `for` 循环逐行执行，`innovator.applyAML()` 是同步 HTTP 调用，每行等网络往返，完全无法利用等待时间
3. **共享单连接不安全** — `ArasConnectionService` 只持有一个 `Innovator`，多线程共享会导致竞态条件

## 根因分析

| 问题 | 根因 | 影响 |
|------|------|------|
| 暂停无效 | `_cts` 令牌未传入 `ExecuteImportAsync` 参数 | 暂停按钮只改 UI，循环继续跑 |
| 继续无效 | 循环从未停过 | 继续按钮无意义 |
| 单线程瓶颈 | `for` + `await` 是串行的，`applyAML` 内部是同步 HTTP | 导入 1000 行 ≈ 1000 次网络往返串行等待 |
| 连接非线程安全 | `HttpServerConnection` 内部有状态（Cookie/Session） | 多线程共享会导致响应错乱 |

## 架构设计

```
导入前:
  ArasConnectionPool.NewLogin(url, db, user, md5, count=10)
    → 创建 N 个独立 HttpServerConnection + Innovator
    → 入池 (ConcurrentQueue)

导入中:
  Parallel.ForEachAsync(rows, maxConcurrency: 10)
    → 每行从池借一个连接
    → Excel 读行 → 拼 AML → innovator.applyAML()
    → 还回连接池
    → 检查 CancellationToken → 支持暂停

暂停:
  _cts.Cancel() → Parallel.ForEachAsync 收到取消信号
    → 正在执行的请求完成后退出
    → 已处理行数/失败行数保留在 ImportResult 中
```

## 涉及文件

| 文件 | 改动 |
|------|------|
| `Core/Interfaces/IDataImportService.cs` | 新增 `maxConcurrency` 和 `cancellationToken` 参数 |
| `Core/Interfaces/IArasConnectionService.cs` | 保持 `object?` 属性不变（接口不暴露 IOM 类型） |
| `Services/Services/ArasConnectionPool.cs` | **新建** — 连接池，管理 N 个独立 Aras 连接 |
| `Services/Services/ArasConnectionService.cs` | 不变 — 保留当前单例单连接供其他功能使用 |
| `Services/Services/DataImportService.cs` | 注入连接池，并行循环 + 令牌检查 |
| `Services/ServiceCollectionExtensions.cs` | 注册 `ArasConnectionPool` 为 Singleton |
| `Core/Models/ImportResult.cs` | 新增 `ProcessedRows` 字段记录实际已处理行数 |
| `App/ViewModels/DataImportViewModel.cs` | 传入 `_cts.Token` + `maxConcurrency` 参数，暂停后保留结果 |

## 详细实施方案

### 步骤 1：ImportResult 新增字段 — `Core/Models/ImportResult.cs`

```csharp
/// <summary>实际已处理行数（暂停时小于 TotalRows）</summary>
public int ProcessedRows { get; set; }
```

### 步骤 2：新建 ArasConnectionPool — `Services/Services/ArasConnectionPool.cs`

```csharp
using System.Collections.Concurrent;
using Aras.IOM;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Aras 连接池 — 管理多个独立 HttpServerConnection + Innovator，
/// 供数据汇入多线程并发执行 AML 时使用
/// </summary>
public class ArasConnectionPool
{
    private readonly ConcurrentBag<PooledConnection> _pool = [];

    /// <summary>当前池大小（线程数上限）</summary>
    public int PoolSize { get; private set; }

    /// <summary>池中可用连接数</summary>
    public int Available => _pool.Count;

    /// <summary>
    /// 登录 Aras 并创建 N 个独立连接放入池中
    /// </summary>
    /// <param name="url">Aras 服务器 URL</param>
    /// <param name="database">数据库名</param>
    /// <param name="username">用户名</param>
    /// <param name="md5Password">MD5 小写十六进制密码</param>
    /// <param name="poolSize">连接池大小（=线程数上限，最大10）</param>
    public void Initialize(string url, string database, string username, string md5Password, int poolSize)
    {
        // 先清空旧连接（如果还有残留）
        Clear();
        poolSize = Math.Min(poolSize, 10); // 线程上限10
        PoolSize = poolSize;

        // 逐个创建独立连接（IOM 的 IomFactory 不支持并发创建）
        for (int i = 0; i < poolSize; i++)
        {
            // 每个连接都是独立的 HttpServerConnection + Innovator 实例
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, database, username, md5Password);
            var innovator = new Innovator(conn);
            _pool.Add(new PooledConnection(conn, innovator, i));
        }
    }

    /// <summary>
    /// 从池中取一个连接（阻塞等待直到有可用连接）
    /// </summary>
    public PooledConnection Rent()
    {
        // ConcurrentBag 不支持 TryTake 阻塞，使用自旋+短暂休眠
        PooledConnection? item;
        while (!_pool.TryTake(out item))
            Thread.Sleep(10);
        return item;
    }

    /// <summary>
    /// 归还连接到池中
    /// </summary>
    public void Return(PooledConnection item)
    {
        if (item != null)
            _pool.Add(item);
    }

    /// <summary>
    /// 清空连接池（注销所有连接）
    /// </summary>
    public void Clear()
    {
        while (_pool.TryTake(out var item))
        {
            try { item.Connection.Logout(); }
            catch { /* 静默 */ }
        }
        PoolSize = 0;
    }
}

/// <summary>
/// 池化连接 — 封装一个独立连接及其标识
/// </summary>
public class PooledConnection
{
    public int Index { get; }
    public HttpServerConnection Connection { get; }
    public Innovator Innovator { get; }

    public PooledConnection(HttpServerConnection connection, Innovator innovator, int index)
    {
        Connection = connection;
        Innovator = innovator;
        Index = index;
    }
}
```

**线程安全分析：**
- `ConcurrentBag` 保证多线程 `TryTake`/`Add` 无锁安全
- 每个 `PooledConnection` 是独立的 `HttpServerConnection`，线程间不共享 Cookie/Session
- `Rent()`/`Return()` 期间连接只被一个线程持有，`applyAML` 调用完全安全

### 步骤 3：DataImportService 注入连接池 + 并发执行 — `Services/Services/DataImportService.cs`

**3a. 构造函数新增 `ArasConnectionPool` 参数：**

```csharp
private readonly ArasConnectionService _connectionService;
private readonly ArasConnectionPool _connectionPool; // ← 新增

public DataImportService(
    IDbContextFactory<ArasToolkitDbContext> contextFactory,
    IErrorLogService errorLogService,
    IOperationLogService operationLogService,
    ArasConnectionService connectionService,
    ArasConnectionPool connectionPool) // ← 新增
{
    // ... 原有赋值 ...
    _connectionService = connectionService;
    _connectionPool = connectionPool; // ← 新增
}
```

**3b. 接口签名新增参数：**

```csharp
// IDataImportService.ExecuteImportAsync 签名改为:
Task<ImportResult> ExecuteImportAsync(
    string filePath, string? sheetName,
    int startRow, int endRow, int startCol, int endCol,
    string amlContent,
    int maxConcurrency = 1,                              // ← 新增：并发线程数（默认1=串行）
    CancellationToken cancellationToken = default,       // ← 新增：取消令牌
    Func<int, int, Task>? progressCallback = null);
```

**3c. 核心循环改为并行 + 令牌检查：**

```csharp
// 循环前：收集所有行数据到 List（Excel 读取在非 UI 线程，串行读比较安全）
var rows = new List<(int rowNum, Dictionary<string, string> rowData)>();
for (int r = startRow; r <= maxRow; r++)
{
    var rowData = new Dictionary<string, string>();
    foreach (var kv in colMap)
        rowData[kv.Key] = worksheet.Cells[r, kv.Value].Text?.Trim() ?? "";
    rows.Add((r, rowData));
}

// 验证连接池已初始化
if (maxConcurrency > 1 && _connectionPool.PoolSize < maxConcurrency)
{
    await writer.WriteLineAsync("[错误] 连接池未初始化或池大小不足，回退为单线程执行");
    maxConcurrency = 1;
}

var parallelOptions = new ParallelOptions
{
    MaxDegreeOfParallelism = maxConcurrency,
    CancellationToken = cancellationToken
};

int processed = 0;
// 使用 Interlocked 保证多线程计数安全
int success = 0, failure = 0;

await Parallel.ForEachAsync(rows, parallelOptions, async (item, ct) =>
{
    // 检查取消令牌（暂停时快速退出）
    ct.ThrowIfCancellationRequested();

    // 从连接池取一个连接
    PooledConnection? pooledConn = null;
    if (maxConcurrency > 1)
        pooledConn = _connectionPool.Rent();

    try
    {
        var innovator = pooledConn?.Innovator ?? _connectionService.TypedInnovator;
        if (innovator == null)
        {
            Interlocked.Increment(ref failure);
            await writer.WriteLineAsync("[错误] 行" + item.rowNum + ": 无可用 Aras 连接");
            return;
        }

        var replacedAml = ReplaceAmlPlaceholders(amlContent, item.rowData);
        var resultItem = innovator.applyAML(replacedAml); // 同步 HTTP 调用

        if (!resultItem.isError())
        {
            Interlocked.Increment(ref success);
            await writer.WriteLineAsync("[成功] 行" + item.rowNum + ": AML执行成功");
        }
        else
        {
            Interlocked.Increment(ref failure);
            var errMsg = resultItem.getErrorString();
            await writer.WriteLineAsync("[失败] 行" + item.rowNum + ": " + errMsg);
        }
    }
    catch (OperationCanceledException)
    {
        throw; // 重新抛出让 Parallel.ForEachAsync 处理暂停
    }
    catch (Exception ex)
    {
        Interlocked.Increment(ref failure);
        await writer.WriteLineAsync("[失败] 行" + item.rowNum + ": " + ex.Message);
    }
    finally
    {
        // 还回连接池
        if (pooledConn != null)
            _connectionPool.Return(pooledConn);

        // 更新进度（线程安全递减计数）
        var current = Interlocked.Increment(ref processed);
        if (progressCallback != null)
            await progressCallback(current, result.TotalRows);
    }
});

result.SuccessCount = success;
result.FailureCount = failure;
result.ProcessedRows = processed;
```

**3d. 暂停/恢复机制：**
- ViewModel 暂停时调用 `_cts.Cancel()` → `Parallel.ForEachAsync` 收到取消信号
- 正在执行的请求（`applyAML` 是同步调用）**无法被中断**，但完成后不再启动新请求
- `result.ProcessedRows` 记录实际处理数，`FailureCount + SuccessCount = ProcessedRows`
- 继续时重新调用 `ExecuteImportAsync`，`startRow = 上次 startRow + ProcessedRows`

### 步骤 4：ViewModel 真正的暂停/继续 — `App/ViewModels/DataImportViewModel.cs`

**4a. 导入执行改为传入令牌和线程数：**

```csharp
private int _maxConcurrency = 1; // 线程数（1=串行）
public int MaxConcurrency
{
    get => _maxConcurrency;
    set => SetProperty(ref _maxConcurrency, Math.Clamp(value, 1, 10));
}

private async Task ExecuteImportAsync()
{
    IsImporting = true;
    IsPaused = false;
    _cts = new CancellationTokenSource();
    ErrorMessage = string.Empty;

    try
    {
        ImportProgress = 0;
        ProgressText = "导入中...";

        // 记录暂停恢复的偏移量
        int startOffset = _pauseOffset; // 上次暂停的行号偏移
        _pauseOffset = 0;

        LastResult = await _dataImportService.ExecuteImportAsync(
            SelectedFilePath, SelectedSheetName,
            StartRow + startOffset, EndRow, StartCol, EndCol,
            AmlContent,
            MaxConcurrency,                    // ← 传入线程数
            _cts.Token,                        // ← 传入取消令牌
            async (current, total) =>
            {
                // current 是已完成行数，计算百分比
                ImportProgress = total > 0 ? (double)current / total * 100 : 0;
                ProgressText = current + "/" + total;
                await Task.Delay(1);
            });

        // 导入完成后的状态更新
        ImportProgress = 100;
        ProgressText = "完成: " + (LastResult?.TotalRows ?? 0) + " 行";
        StatusMessage = "导入完成: 总计" + (LastResult?.TotalRows ?? 0)
            + " 成功" + (LastResult?.SuccessCount ?? 0)
            + " 失败" + (LastResult?.FailureCount ?? 0);
    }
    catch (OperationCanceledException)
    {
        // 暂停：保留 _pauseOffset 供继续使用
        _pauseOffset = (LastResult?.ProcessedRows ?? 0);
        ProgressText = "已暂停: " + _pauseOffset + "/" + (LastResult?.TotalRows ?? 0);
        StatusMessage = "导入已暂停，可点击继续";
    }
    catch (Exception ex)
    {
        ErrorMessage = "导入失败: " + ex.Message;
    }
    finally
    {
        if (!IsPaused)
        {
            IsImporting = false;
            _pauseOffset = 0;
        }
        IsPaused = false;
    }
}
```

**4b. 暂停/继续属性修改：**

```csharp
private int _pauseOffset; // 暂停时的已处理行数偏移

private void PauseAsync()
{
    IsPaused = true;
    _cts?.Cancel(); // 发送取消信号 → 并行循环收到后停止
}

private void ResumeAsync()
{
    IsPaused = false;
    // 重新执行导入，从上次暂停位置继续
    _ = ExecuteImportAsync();
}
```

### 步骤 5：DI 注册 — `Services/ServiceCollectionExtensions.cs`

```csharp
// 注册 Aras 连接池（单例，供并发导入使用）
services.AddSingleton<ArasConnectionPool>();
```

### 步骤 6：ArasConnectionService 保持不变

当前 `ArasConnectionService` 的 `TypedInnovator` 属性已存在，不需要修改。单连接供登录后的会话检测和单线程场景使用；连接池是多线程导入的独立通道。

### 步骤 7：UI 添加线程数输入控件 — `App/Views/DataImportView.xaml`

在执行按钮行（Grid.Row="4"）的 StackPanel 中添加：

```xml
<!-- 在执行按钮前面添加线程数输入 -->
<TextBlock Text="线程数:" FontSize="13" Foreground="{StaticResource TextSecondaryBrush}"
           VerticalAlignment="Center" Margin="0,0,4,0" />
<TextBox Text="{Binding MaxConcurrency}" Width="40" Margin="0,0,12,0"
         FontSize="13" VerticalContentAlignment="Center" />
```

## 联动检查清单

- [ ] Core/Models/ImportResult.cs — 新增 `ProcessedRows` 字段
- [ ] Core/Interfaces/IDataImportService.cs — 新增 `maxConcurrency` + `cancellationToken` 参数
- [ ] Services/Services/ArasConnectionPool.cs — 新建连接池，ConcurrentBag 管理 N 个独立连接
- [ ] Services/Services/DataImportService.cs — 注入连接池，Parallel.ForEachAsync + 令牌检查
- [ ] Services/ServiceCollectionExtensions.cs — 注册 `ArasConnectionPool` Singleton
- [ ] App/ViewModels/DataImportViewModel.cs — 传入令牌/线程数，暂停保留偏移量
- [ ] App/ViewModels/DataImportViewModel.cs — `MaxConcurrency` 属性，范围 1~10
- [ ] App/Views/DataImportView.xaml — 添加线程数输入框
- [ ] Services/Services/ArasConnectionService.cs — 不变
- [ ] DI 注册 — 新增 ArasConnectionPool

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 0 errors
- [ ] 启动后登录 Aras → 数据汇入 → 设置线程数=3 → 执行导入，验证多连接并发
- [ ] 点击暂停 → 验证取消效果（已处理的计数正确，未开始的不执行）
- [ ] 点击继续 → 验证从偏移位置恢复导入

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


## Git 提交规范

```
修复完成后执行:
1. git add -A && git status
2. git commit -m "feat: TASK-028 数据汇入并发化 — Aras连接池+暂停继续+多线程10上限"
3. git push origin master --force
```


---

## Codex 实现检查清单

| # | 检查项 | 状态 |
|---|--------|------|
| 1 | ImportResult — 新增 ProcessedRows 字段 | ✅ |
| 2 | ArasConnectionPool — 新建连接池（ConcurrentBag + Rent/Return） | ✅ |
| 3 | IDataImportService — 新增 maxConcurrency + CancellationToken 参数 | ✅ |
| 4 | DataImportService — 注入 ArasConnectionPool，Parallel.ForEachAsync 并发执行 | ✅ |
| 5 | DataImportService — 多线程用连接池租借，单线程用全局连接 | ✅ |
| 6 | DataImportService — Interlocked 保证线程安全计数 + CancellationToken 支持暂停 | ✅ |
| 7 | DataImportViewModel — 传入 MaxConcurrency + _cts.Token + 捕获 OperationCanceledException | ✅ |
| 8 | DataImportViewModel — 暂停保留 _pauseOffset，继续从上次位置恢复 | ✅ |
| 9 | ServiceCollectionExtensions — 注册 ArasConnectionPool 单例 | ✅ |
| 10 | 编译验证 | ✅ 通过 (0 errors) |
