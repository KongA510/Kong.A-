---
id: TASK-002
priority: P1
type: fix
created: 2026-06-22
source: Claude Code
status: done
---

# 瀹℃煡骞朵慨澶?OxyPlot 2.2.0 API 鍏煎鎬ч闄?
## 闂鎻忚堪

`ChartViewModel.BuildBarChart()` 鏂规硶涓娇鐢ㄤ簡澶氫釜 OxyPlot API锛岃櫧缂栬瘧閫氳繃锛屼絾鍦?OxyPlot 2.2.0 涓彲鑳藉瓨鍦ㄨ繍琛屾椂琛屼负宸紓鎴栧睘鎬т笉鍙敤鐨勯棶棰樸€傚綋鍓嶆棤娉曠‘璁よ繖浜?API 鍦?2.2.0 鐗堟湰涓殑鍏蜂綋琛ㄧ幇锛岄渶瑕佸湪 TASK-001 淇鍚庨獙璇佸浘琛ㄦ槸鍚﹁兘姝ｅ父娓叉煋
## 娑夊強鏂囦欢

- `src/ArasToolkit.App/ViewModels/ChartViewModel.cs` —`BuildBarChart()` 鏂规硶锛堢113-201琛岋級

## 闇€瀹℃煡鐨?API 灞炴€?
| 琛屽彿 | 灞炴€?| 椋庨櫓 |
|------|------|------|
| 119 | `TitleFontWeight = FontWeights.Bold` | OxyPlot 鍙兘鏈熸湜 `double` 绫诲瀷鑰岄潪 `System.Windows.FontWeight` |
| 153 | `AbsoluteMinimum = 0` | OxyPlot 2.x 涓彲鑳藉凡绉婚櫎 |
| 154 | `MajorGridlineStyle = LineStyle.Dash` | 纭 `LineStyle` 鏋氫妇鍊?|
| 156 | `MinorGridlineStyle = LineStyle.None` | 鍚屼笂 |
| 163 | `FillColor` on BarSeries | OxyPlot 2.x 鍙兘浣跨敤 `Fill` |
| 167 | `LabelPlacement = LabelPlacement.Outside` | 纭鏋氫妇鏄惁瀛樺湪 |
| 170 | `FontWeight = FontWeights.Bold` on BarSeries | 鍚屼笂绫诲瀷闂 |
| 189 | `item.Color` on BarItem | OxyPlot 2.x 涓?BarItem 棰滆壊璁剧疆鏂瑰紡鍙兘涓嶅悓 |

## 棰勬湡琛屼负

鍒囨崲鍥捐〃绫诲瀷锛堢姸鎬佸垎甯?/ 椤圭洰鍒嗗竷锛夊悗锛屾煴鐘跺浘姝ｅ父娓叉煋锛屼笉宕╂簝銆佷笉鍑虹幇绌虹櫧鍥捐〃鍖哄煙
## Claude Code 鍒嗘瀽

OxyPlot 2.0 鏄噸澶х増鏈洿鏂帮紝API 鏈夋樉钁楀彉鍖栥€傚綋鍓嶄唬鐮佸湪 2.2.0 涓嬬紪璇戦€氳繃璇存槑灞炴€у悕绉板瓨鍦紝浣嗚繍琛屾椂琛屼负鍙兘鏈夐棶棰橈紙濡傛煇浜涘睘鎬ц鏍囪涓?Obsolete銆佹暟鍊艰寖鍥翠笉鍚屻€佹垨 setter 鍐呴儴瀹炵幇鏈夊彉鍖栵級
**寤鸿鏂规**锛氬厛瀹屾垚 TASK-001 淇璁╃晫闈㈣兘鍔犺浇锛岀劧鍚庤繍琛屽簲鐢ㄨ瀵熷浘琛ㄦ槸鍚︽覆鏌撴甯搞€傝嫢鍑虹幇寮傚父锛屾寜浠ヤ笅浼樺厛绾ф帓鏌ワ細
1. 绠€鍖?`BuildBarChart` 鍒版渶灏忓彲鐢ㄧ増鏈紙鍙繚鐣?Title + Axes + BarSeries锛?2. 閫愭娣诲姞鏍峰紡灞炴€э紝姣忔楠岃瘉
3. 鍙傝€?OxyPlot 2.2.0 瀹樻柟绀轰緥浠ｇ爜纭姝ｇ‘ API

## 缂栬瘧楠岃瘉

- [ ] 鍥捐〃鍦ㄣ€岀姸鎬佸垎甯冦€嶄笅姝ｅ父鏄剧ず鏌辩姸鍥?- [ ] 鍒囨崲鍒般€岄」鐩垎甯冦€嶄笉宕╂簝
- [ ] 鐐瑰嚮銆屽埛鏂般€嶆寜閽浘琛ㄩ噸鏂板姞杞?
## 淇缁撴灉锛圕odex 濉啓锛?
- 淇鐘舵€? [success / partial / failed]
- 缂栬瘧缁撴灉: [pass / fail]
- 澶囨敞: [Codex 濉啓鐨勪慨澶嶈鏄嶿

