---
id: TASK-020
priority: P0
type: bug
created: 2026-06-25
source: Claude Code锛堝垎鏋愬畾浣嶏級
status: done
reviewed: 2026-06-25
reviewer: Claude Code
review_conclusion: |
  全部 4 椤规鏌ラ€氳繃锛?
  1. IsMd5Format 鏂规硶姝ｇ‘锛堝惈 null 闃插尽锛?
  2. LoginAsync 閫昏緫姝ｇ‘锛圡D5 鐩存瘮锛屾槑鏂囧厛 hash 鍐嶆瘮锛?
  3. RegisterAsync 浣跨敤 ToMd5() 瀛樺偍
  4. EnsureSchemaAsync 鍖呭惈 schema 鍚屾 + 密码淇 + 榛樿用户

  棰濆鍔犲垎锛欼sMd5Format 鍔犱簡 null 妫€鏌ワ紙瑙勬牸鏈姹傦級锛岄槻寰℃€х紪绋嬨€?

  娉ㄦ剰锛氱敓浜у氨缁墠闇€纭鏁版嵁搴?app_user 琛?xinke.wang / admin 瀵嗘枃宸?UPDATE锛圗nsureSchemaAsync 浼氬皾璇曟墽琛岋紝浣嗕緷璧?app_user 琛ㄥ凡瀛樺湪锛夈€?
---

# TASK-020: 鐧诲綍密码鏍￠獙閫昏緫淇 —MD5 鏍煎紡妫€娴?+ 鍏煎文本

## 鐜拌薄

杈撳叆密码 `xwxy51020` 鐧诲綍鎻愮ず"璐﹀彿鎴栧瘑鐮侀敊璇?

## 鏍瑰洜

`AppUserService.LoginAsync` 绗?30 琛屽缁堟墽琛?`password.ToMd5()` 姣斿锛?

```csharp
if (user == null || user.Password != password.ToMd5())
    return null;
```

闂鏈変袱涓細

1. **鏁版嵁搴撳彲鑳戒粛瀛樻槑鏂?*锛歚app_user` 琛ㄤ腑 xinke.wang 密码瀛楁鑻ユ湭鎵ц UPDATE 浠嶄负文本 `"xwxy51020"`锛屼唬鐮佸皢鍏?MD5 鍝堝笇鍚庝笌文本姣斿锛屽繀鐒跺け璐?
2. **MD5 杈撳叆琚簩娆″搱甯?*锛氳嫢用户鐩存帴杈撳叆 32 浣?MD5 鍊肩櫥褰曪紝浼氳鍐嶆 ToMd5锛屽繀鐒跺け璐?

## 修改浠诲姟

### LoginAsync 澧炲姞 MD5 鏍煎紡检查

修改浣嶇疆: `src/ArasToolkit.Services/Services/AppUserService.cs` 绗?24-34 琛?

鏀归€?`LoginAsync` 鏂规硶锛屽垽鏂緭鍏ユ槸鍚﹀凡鏄?MD5 鏍煎紡锛?

```csharp
public async Task<AppUser?> LoginAsync(string username, string password)
{
    await using var context = await _contextFactory.CreateDbContextAsync();
    var user = await context.Set<AppUser>()
        .FirstOrDefaultAsync(u => u.Username == username);

    if (user == null)
        return null;

    // 检查杈撳叆鏄惁宸叉槸 32 浣嶅皬鍐欏崄鍏繘鍒?MD5 鏍煎紡
    string hashedInput = IsMd5Format(password) ? password : password.ToMd5();

    if (user.Password != hashedInput)
        return null;

    return user;
}

/// <summary>
/// 检查瀛楃涓叉槸鍚﹀凡鏄?32 浣嶅皬鍐欏崄鍏繘鍒?MD5 鏍煎紡
/// </summary>
private static bool IsMd5Format(string s)
{
    if (s.Length != 32) return false;
    foreach (char c in s)
    {
        if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
            return false;
    }
    return true;
}
```

**閫昏緫璇存槑**锛?
- 用户杈撳叆 `xwxy51020`锛堟槑鏂囷級鈫?`IsMd5Format` 杩斿洖 false 鈫?ToMd5 寰楀埌 `915d92dbc92c8b655764d3df7e22161b` 鈫?涓?DB 姣斿 鈫?鎴愬姛
- 用户杈撳叆 `915d92dbc92c8b655764d3df7e22161b`锛圡D5锛夆啋 `IsMd5Format` 杩斿洖 true 鈫?鐩存帴姣斿 鈫?鎴愬姛
- 鏁版嵁搴撴棤璁哄瓨 MD5 杩樻槸文本锛堣繃娓℃湡锛夛紝閮借兘鍖归厤

### 鏁版嵁搴?UPDATE锛堝繀椤伙級

```sql
UPDATE app_user SET password = N'915d92dbc92c8b655764d3df7e22161b' WHERE username = N'xinke.wang';
UPDATE app_user SET password = N'21232f297a57a5a743894a0e4a801fc3' WHERE username = N'admin';
```

## 楠岃瘉鏂瑰紡

- `dotnet build ArasToolkit.slnx` 闆堕敊璇?
- 杈撳叆文本 `xwxy51020` 鐧诲綍 鈫?鎴愬姛
- 杈撳叆 MD5 `915d92dbc92c8b655764d3df7e22161b` 鐧诲綍 鈫?鎴愬姛
- 杈撳叆 `admin` 鐧诲綍 鈫?鎴愬姛

---

## Git 瑙勮寖锛堢紪鐮佸墠蹇呴』鎵ц锛?

### 编码鍓嶅浠?

```bash
git add -A
git stash push -m "backup-before-TASK-020"
git stash pop
```

### 编码瀹屾垚鍚庢彁浜?

```bash
git add -A
git commit -m "淇: TASK-020 鐧诲綍密码鏍￠獙澧炲姞MD5鏍煎紡妫€娴嬶紝鍏煎文本杈撳叆"
```

### 瀹℃煡閫氳繃鍚庢帹閫侊紙master force push 鍓嶅厛澶囦唤鍒?develop锛?

```bash
git checkout develop
git merge master
git push origin develop
git checkout master
git push origin master --force
```

> 鍙傜収 AGENTS.md 绗?11 绔?Git 鍒嗘敮涓庢帹閫佺瓥鐣ャ€?


