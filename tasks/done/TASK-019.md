---
id: TASK-019
priority: P0
type: bug
created: 2026-06-25
source: Claude Code锛堝垎鏋愬畾浣嶏級
status: done_review
---

# TASK-019: App.xaml 缁撴瀯错误 + 错误日志鏌ヨ淇 + 密码 MD5

## 闂涓€锛歺inke.wang 榛樿密码浠嶆槸文本

### 浣嶇疆

`src/ArasToolkit.Services/Services/AppUserService.cs` 绗?19琛?
### 褰撳墠浠ｇ爜

```csharp
Password = "xwxy51020",
```

### 搴旀敼涓?
```csharp
Password = "915d92dbc92c8b655764d3df7e22161b",
```

涓嶄慨鏀瑰垯鏂扮敤鎴?xinke.wang 密码瀛樻槑鏂囷紝LoginAsync 鐢?MD5 姣斿姘歌繙澶辫触
---

## 闂浜岋細GetPagedEntriesAsync 涓?total 鍦ㄧ敤鎴疯繃婊ゅ墠璁＄畻

### 浣嶇疆

`src/ArasToolkit.Services/Services/ErrorLogService.cs` 绗?8琛?
### 褰撳墠浠ｇ爜

```csharp
var total = await query.CountAsync();        // line 68 —用户杩囨护涔嬪墠灏?count 浜?
if (!CurrentUserContext.IsAdmin)              // line 71
{
    query = query.Where(e => e.UserName == CurrentUserContext.CurrentUserName);
}

var items = await query...ToListAsync();     // line 76 —items 鏄繃婊ゅ悗鐨?return (items, total);                        // line 82 —total 鏄繃婊ゅ墠鐨勶紝涓嶅尮閰?```

### 搴旀敼涓?
灏?`var total = await query.CountAsync();` 绉诲埌 `if (!CurrentUserContext.IsAdmin)` 鍧椾箣鍚庯細

```csharp
if (!CurrentUserContext.IsAdmin)
{
    query = query.Where(e => e.UserName == CurrentUserContext.CurrentUserName);
}

var total = await query.CountAsync();        // 绉诲埌杩囨护鍚?
var items = await query...ToListAsync();
return (items, total);
```

---

## 闂涓夛細OnModelCreating 涓?ErrorLog 缂哄皯 UserName 鏄犲皠

### 浣嶇疆

`src/ArasToolkit.Services/Data/ArasToolkitDbContext.cs` OnModelCreating 鏂规硶锛孍rrorLog 閰嶇疆鍧?
### 搴旀敼涓?
鍦?`entity.Property(e => e.CreatorOn).HasColumnName("creator_on");` 涔嬪悗娣诲姞涓€琛岋細

```csharp
entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(100);
```

---

## 闂鍥涳細AppLoginViewModel 鏈皟鐢?DbContext.EnsureSchemaAsync()

### 璇存槑

error_log 琛ㄧ殑鍒涘缓閫昏緫鍦?`ArasToolkitDbContext.EnsureSchemaAsync()` 涓紝浣嗗惎鍔ㄦ祦绋嬩粠鏈皟鐢ㄥ畠銆傞渶鍦ㄥ簲鐢ㄧ櫥褰曟垚鍔熷悗鑷姩瑙﹀彂鎵€鏈変笟鍔¤〃鐨勫悓姝ャ€?
### 修改

`src/ArasToolkit.App/ViewModels/AppLoginViewModel.cs`锛?
1. 娣诲姞 using锛?```csharp
using ArasToolkit.Services.Data;
```

2. 鏋勯€犲嚱鏁版敞鍏?`IDbContextFactory<ArasToolkitDbContext>`锛?```csharp
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

3. `InitializeAsync()` 鏂规硶涓坊鍔狅細
```csharp
private async Task InitializeAsync()
{
    await _appUserService.EnsureSchemaAsync();

    // 鍚屾鎵€鏈変笟鍔¤〃锛坋rror_log銆乨ata_import_config 绛夛級
    await using var dbContext = await _contextFactory.CreateDbContextAsync();
    await dbContext.EnsureSchemaAsync();

    ...
}
```

---

## 闂浜旓細ConfigSelectWindow 宸插垱寤轰絾鏈娇鐢?
### 浣嶇疆

`src/ArasToolkit.App/ViewModels/DataImportViewModel.cs` 绗?09-228琛?
### 褰撳墠浠ｇ爜

```csharp
var pick = configs.FirstOrDefault();   // 浠呭彇绗竴涓紝鏃犲脊绐楅€夋嫨
```

### 搴旀敼涓?
```csharp
var window = new ConfigSelectWindow(this);
window.Owner = Application.Current.MainWindow;
window.ShowDialog();
```

---

## 鏁版嵁搴?UPDATE锛堟墜鍔ㄦ墽琛岋級

### error_log 鏃ф暟鎹ˉ榻?user_name
```sql
UPDATE error_log SET user_name = N'xinke.wang' WHERE user_name IS NULL;
```

### app_user 密码鏀?MD5
```sql
UPDATE app_user SET password = N'915d92dbc92c8b655764d3df7e22161b' WHERE username = N'xinke.wang';
UPDATE app_user SET password = N'21232f297a57a5a743894a0e4a801fc3' WHERE username = N'admin';
```

MD5 瀵圭収锛?- `xwxy51020` 鈫?`915d92dbc92c8b655764d3df7e22161b`
- `admin` 鈫?`21232f297a57a5a743894a0e4a801fc3`

---

## 楠岃瘉鏂瑰紡

1. `dotnet build ArasToolkit.slnx` 闆堕敊璇?2. 鍒犻櫎 Config/AppSettings/appLogin.json
3. xinke.wang / xwxy51020 鐧诲綍鎴愬姛
4. admin / admin 鐧诲綍鎴愬姛
5. ErrorLogView 鑳界湅鍒板巻鍙查敊璇褰?6. ArasLoginWindow 鐐瑰嚮鍚庢甯稿脊鍑?
---

## Claude Code 瀹℃煡缁撹 (绗簩杞?—2026-06-25)

**缁撹: 闇€修改**

缂栬瘧: 0 错误
| # | 修改鐐?| 鐘舵€?|
|---|--------|------|
| 1 | xinke.wang 密码鏀逛负 MD5 | 宸蹭慨澶?|
| 3 | ErrorLog UserName Fluent API 鏄犲皠 | 宸蹭慨澶?|
| 2 | total 绉诲埌用户杩囨护涔嬪悗 | 寰呬慨鏀?|
| 4 | AppLoginViewModel 璋冪敤 EnsureSchemaAsync | 寰呬慨鏀?|
| 5 | ConfigSelectWindow 寮圭獥璋冪敤 | 寰呬慨鏀?|

3椤逛粛鏈慨鏀癸紝涓庣涓€杞竴鑷淬€?

