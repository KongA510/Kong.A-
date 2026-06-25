---
id: TASK-018
priority: P0
type: bug
created: 2026-06-25
source: Claude Code锛堝垎鏋愬畾浣嶏級
status: pending_review
---

# TASK-018: 错误日志鏃犱换浣曟暟鎹?—琛ㄦ湭鍒涘缓 + OnModelCreating 缂烘槧灏?
## 鐜拌薄

错误日志椤甸潰锛圗rrorLogView锛夊缁堜负绌猴紝鏌ヨ涓嶅埌浠讳綍错误璁板綍
## 鏍瑰洜鍒嗘瀽

### 鍘熷洜涓€锛歟rror_log 琛ㄦ湭鍒涘缓锛堜富瑕佸師鍥狅級

`error_log` 琛ㄧ殑 CREATE 閫昏緫鍦?`ArasToolkitDbContext.EnsureSchemaAsync()` 涓紝浣?*鍚姩娴佺▼浠庢湭璋冪敤瀹?*锛?
```
鍚姩娴佺▼:
  App.xaml.cs 鈫?ConfigureServices() 鈫?Application_Startup() 鈫?MainWindow
  AppLoginViewModel.InitializeAsync() 鈫?AppUserService.EnsureSchemaAsync()
      鈫?浠呭垱寤?app_user 琛紝涓嶅垱寤?error_log锛?
ArasToolkitDbContext.EnsureSchemaAsync() 鐨勮皟鐢ㄧ偣:
  - TodoService.cs:201 鈫?用户杩?Todo 妯″潡鏃舵墠璋冪敤
  - SettingsViewModel.cs:85 鈫?淇濆瓨杩炴帴璁剧疆鏃舵墠璋冪敤

鈫?用户濡傛灉娌¤繘鍏?Todo銆佹病淇濆瓨杩炴帴璁剧疆锛宔rror_log 琛ㄤ粠鏈垱寤?鈫?ErrorLogService.LogErrorAsync() 鎵ц INSERT 澶辫触
鈫?catch 鍧楅潤榛樹涪寮?鈫?错误日志姘歌繙涓虹┖
```

### 鍘熷洜浜岋細OnModelCreating 涓?ErrorLog 缂哄皯 UserName 鏄犲皠

`ArasToolkitDbContext.OnModelCreating()` 涓?ErrorLog 閰嶇疆缂哄皯锛?```csharp
entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100);
```

褰撳墠浠呬緷璧栧疄浣撲笂鐨?`[Column("user_name")]` 娉ㄨВ鍏滃簳锛屼絾鍏朵粬瀹炰綋閮芥槸 Fluent API + 娉ㄨВ鍙岄噸瑕嗙洊锛屽簲琛ラ綈
## 修改浠诲姟

### 1. AppLoginViewModel 涓紝搴旂敤鐧诲綍鎴愬姛鍚庤皟鐢?DbContext.EnsureSchemaAsync()

修改浣嶇疆: `src/ArasToolkit.App/ViewModels/AppLoginViewModel.cs`

a) 娣诲姞 using锛?```csharp
using ArasToolkit.Services.Data;
```

b) 娣诲姞鏋勯€犲嚱鏁板弬鏁板拰瀛楁锛?```csharp
private readonly IDbContextFactory<ArasToolkitDbContext> _contextFactory;

public AppLoginViewModel(IAppUserService appUserService, IConfigService configService,
    IDbContextFactory<ArasToolkitDbContext> contextFactory)
{
    _appUserService = appUserService;
    _configService = configService;
    _contextFactory = contextFactory;
    ...
}
```

c) 鍦?`InitializeAsync()` 鏂规硶涓紝`_appUserService.EnsureSchemaAsync()` 涔嬪悗锛屾柊澧烇細
```csharp
await using var dbContext = await _contextFactory.CreateDbContextAsync();
await dbContext.EnsureSchemaAsync();
```

### 2. OnModelCreating 涓ˉ榻?ErrorLog.UserName 鏄犲皠

修改浣嶇疆: `src/ArasToolkit.Services/Data/ArasToolkitDbContext.cs`

鍦?ErrorLog 閰嶇疆鍧椾腑锛宍entity.Property(e => e.CreatorOn).HasColumnName("creator_on");` 涔嬪悗娣诲姞锛?```csharp
entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100);
```

### 鏁版嵁搴?UPDATE

```sql
UPDATE error_log SET user_name = N'xinke.wang' WHERE user_name IS NULL;
```

## 楠岃瘉鏂瑰紡

- `dotnet build ArasToolkit.slnx` 闆堕敊璇?- 鍚姩搴旂敤 鈫?鐧诲綍 鈫?瑙﹀彂涓€涓敊璇紙濡傞敊璇瘑鐮佺櫥褰旳ras锛夆啋 杩涘叆错误日志椤甸潰 鈫?鑳界湅鍒拌褰?- 鐩存帴鏌ユ暟鎹簱 `SELECT * FROM error_log` 纭琛ㄥ凡鍒涘缓涓旀湁鏁版嵁

---

## Claude Code 瀹℃煡缁撹 (2026-06-25)

**缁撹: 闇€修改**

缂栬瘧: 0 错误
| # | 修改鐐?| 鐘舵€?|
|---|--------|------|
| 1 | AppLoginViewModel 璋冪敤 DbContext.EnsureSchemaAsync | 寰呬慨鏀?|
| 2 | OnModelCreating ErrorLog.UserName 鏄犲皠 | 宸蹭慨澶?|

绗?椤逛粛鏈慨鏀癸細AppLoginViewModel 鏈敞鍏?IDbContextFactory锛孖nitializeAsync 涓湭璋冪敤 EnsureSchemaAsync

