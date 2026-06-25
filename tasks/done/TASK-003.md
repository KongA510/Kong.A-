---
id: TASK-003
priority: P1
type: fix
created: 2026-06-22
source: Claude Code
status: done
---

# 鏁版嵁鎶ヨ〃妯″潡闃插尽鎬х紪绋嬪姞鍥?
## 闂鎻忚堪

TASK-001 鏆撮湶浜嗕竴涓灦鏋勭骇闂锛歏iew 灞傜殑 XAML 寮傚父锛堝 StaticResource 缂哄け锛夊彂鐢熷湪 `InitializeComponent()` 闃舵锛屾鏃?ViewModel 鏋勯€犲嚱鏁板皻鏈畬鎴愶紝错误日志鏈嶅姟鏃犳硶璁板綍銆傞渶瑕佸妯″潡杩涜闃插尽鎬у姞鍥猴紝纭繚 View 鏋勯€犲け璐ユ椂涔熻兘鐣欎笅璇婃柇淇℃伅
## 娑夊強鏂囦欢

- `src/ArasToolkit.App/Views/ChartView.xaml.cs` —娣诲姞 try-catch
- `src/ArasToolkit.App/MainWindow.xaml.cs` —NavigateToPage 娣诲姞 try-catch
- `src/ArasToolkit.App/ViewModels/ChartViewModel.cs` —鏋勯€犲嚱鏁板姞鍥?
## 鍏蜂綋鍔犲浐鐐?
### 1. ChartView.xaml.cs —鏋勯€犲嚱鏁板紓甯告崟鑾?
```csharp
public partial class ChartView : UserControl
{
    public ChartView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            // XAML 瑙ｆ瀽澶辫触鏃惰嚦灏戣緭鍑?Debug 璇婃柇
            System.Diagnostics.Debug.WriteLine($"[ChartView] 鍒濆鍖栧け璐? {ex.Message}");
            throw; // 閲嶆柊鎶涘嚭锛屼絾纭繚璇婃柇淇℃伅宸茶緭鍑?        }
    }
}
```

### 2. MainWindow.xaml.cs —NavigateToPage 寮傚父淇濇姢

鍦?`NavigateToPage(MenuItemInfo)` 鏂规硶鐨?switch 鍒嗘敮澶栧眰鍖呰９ try-catch锛屾崟鑾?View 鏋勯€犲紓甯稿苟璁板綍
### 3. ChartViewModel —鏋勯€犲嚱鏁板畨鍏ㄥ垵濮嬪寲

褰撳墠 `_ = LoadChartAsync()` 涓?fire-and-forget锛屽悓姝ュ紓甯稿彲鑳介€冮€搞€傚缓璁細
- 灏嗗垵濮嬫暟鎹姞杞界Щ鍒?View 鐨?`Loaded` 浜嬩欢涓Е鍙?- 鎴栦娇鐢?`Task.Run` 鍖呰９浠ョ‘淇濆紓甯镐笉浼氫紶鎾埌鏋勯€犲嚱鏁?
## 棰勬湡琛屼负

View 鏋勯€犲け璐ユ椂锛?- Debug 杈撳嚭涓寘鍚?`[ChartView]` 鍓嶇紑鐨勮瘖鏂俊鎭?- 错误日志涓褰?`Main-瀵艰埅` 绾у埆鐨勫紓甯镐俊鎭?- 搴旂敤涓嶉棯閫€锛堥檷绾т负鏄剧ず错误鎻愮ず锛?
## Claude Code 鍒嗘瀽

杩欐槸浠庢湰娆￠棯閫€闂涓彁鐐肩殑鏋舵瀯鏀硅繘銆俉PF 鐨?`InitializeComponent()` 鎵ц鐨?XAML 瑙ｆ瀽鏄悓姝ョ殑锛屼换浣?XAML 灞傞潰鐨勯敊璇兘浼氬鑷?`Window`/`UserControl` 鏋勯€犲け璐ャ€傚湪 MVVM 鏋舵瀯涓紝ViewModel 鏃犳硶鎰熺煡 View 鐨勬瀯閫犲け璐ャ€傚洜姝?*蹇呴』鍦?View 灞傚仛闃插尽**
瀵逛簬 `MainWindow.NavigateToPage`锛屽缓璁湪 switch 鐨勬瘡涓?View 鏋勯€犲灞傚姞 try-catch锛?
```csharp
private void NavigateToPage(MenuItemInfo menuItem)
{
    try
    {
        object? view = menuItem.Name switch
        {
            "浠〃鐩? => new DashboardView { ... },
            // ...
            _ => null
        };
        if (view != null) MainContentControl.Content = view;
    }
    catch (Exception ex)
    {
        // 璁板綍瀵艰埅寮傚父浣嗕笉宕╂簝
        MainContentControl.Content = new TextBlock 
        { 
            Text = $"鉀?椤甸潰鍔犺浇澶辫触: {ex.Message}",
            FontSize = 16, Foreground = ... 
        };
    }
}
```

## 缂栬瘧楠岃瘉

- [ ] `dotnet build ArasToolkit.slnx` 閫氳繃
- [ ] 妯℃嫙 XAML 错误鏃?Debug 杈撳嚭鍖呭惈璇婃柇淇℃伅
- [ ] 搴旂敤涓嶉棯閫€

## 淇缁撴灉锛圕odex 濉啓锛?
- 淇鐘舵€? [success / partial / failed]
- 缂栬瘧缁撴灉: [pass / fail]
- 澶囨敞: [Codex 濉啓鐨勪慨澶嶈鏄嶿

