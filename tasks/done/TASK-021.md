---
id: TASK-021
priority: P0
type: bug
created: 2026-06-25
source: Claude Code锛堝垎鏋愬畾浣嶏級
status: done
reviewed: 2026-06-25
reviewer: Claude Code
review_conclusion: |
  鍏ㄩ儴妫€鏌ラ€氳繃锛?  1. DataImportView.xaml:133 InputBrush 鈫?SurfaceBrush 宸蹭慨澶?  2. 鍏ㄥ眬鏃犲叾浠?InputBrush 寮曠敤
  3. SurfaceBrush 棰滆壊 #F9FAFB 涓庡師鎰忓浘涓€鑷达紙杈撳叆妗嗘祬鐏拌儗鏅級
---

# TASK-021: DataImportView 璧勬簮寮曠敤閿欒 鈥?InputBrush 涓嶅瓨鍦?
## 鐜拌薄

鐐瑰嚮鑿滃崟"鏁版嵁姹囧叆"鍚庨棯閫€锛屾姤閿欎綅缃湪 `DataImportView.xaml` 鐨?`InitializeComponent()`銆?
## 鏍瑰洜

`DataImportView.xaml` 绗?133 琛屽紩鐢ㄤ簡 `{StaticResource InputBrush}`锛屼絾 Theme.xaml 涓湭瀹氫箟璇ヨ祫婧愰敭锛孹AML 瑙ｆ瀽鏃舵姏鍑?`XamlParseException`銆?
```xml
<!-- DataImportView.xaml:133 鈥?閿欒寮曠敤 -->
Background="{StaticResource InputBrush}"
```

Theme.xaml 涓疄闄呭瓨鍦ㄧ殑鍒峰瓙閿悕锛?`PrimaryBrush`, `SecondaryBrush`, `AccentBrush`, `AccentHoverBrush`, `SurfaceBrush`, `CardBrush`, `TextPrimaryBrush`, `TextSecondaryBrush`, `BorderBrush`, `SuccessBrush`, `ErrorBrush`, `WarningBrush`, `SidebarBgBrush`

**娌℃湁 `InputBrush`銆?*

## 淇敼浠诲姟

### 1. 淇璧勬簮寮曠敤

淇敼浣嶇疆: `src/ArasToolkit.App/Views/DataImportView.xaml` 绗?133 琛?
```diff
- Background="{StaticResource InputBrush}"
+ Background="{StaticResource SurfaceBrush}"
```

`SurfaceBrush` 棰滆壊涓?`#F9FAFB`锛屽氨鏄緭鍏ユ鑳屾櫙鑹诧紝璇箟鍜岃瑙夋晥鏋滀竴鑷淬€?
### 2. 鎺掓煡鍏ㄥ眬 InputBrush 寮曠敤

纭鏁翠釜椤圭洰涓彧鏈?DataImportView.xaml 涓€澶勫紩鐢ㄤ簡 `InputBrush`锛堝凡鐢?Claude 纭锛夈€?
## 楠岃瘉鏂瑰紡

- `dotnet build ArasToolkit.slnx` 闆堕敊璇?- 杩愯鍚庣偣鍑?鏁版嵁姹囧叆"鑿滃崟 鈫?椤甸潰姝ｅ父鏄剧ず锛屼笉鍐嶉棯閫€

---

## Git 瑙勮寖锛堢紪鐮佸墠蹇呴』鎵ц锛?
### 缂栫爜鍓嶅浠?
```bash
git add -A
git stash push -m "backup-before-TASK-021"
git stash pop
```

### 缂栫爜瀹屾垚鍚庢彁浜?
```bash
git add -A
git commit -m "淇: TASK-021 DataImportView涓璉nputBrush璧勬簮寮曠敤閿欒锛屾敼涓篠urfaceBrush"
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
