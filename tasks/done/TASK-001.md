---
id: TASK-001
priority: P0
type: fix
created: 2026-06-22
source: Claude Code
status: done
---

# 淇鏁版嵁鎶ヨ〃鐣岄潰闂€€ —XAML StaticResource 涓嶅瓨鍦?
## 闂鎻忚堪

鐐瑰嚮渚ц竟鏍忋€岎煋?鏁版嵁鎶ヨ〃銆嶅悗搴旂敤绔嬪嵆闂€€锛岄敊璇棩蹇椾腑鏃犱换浣曡褰曘€?
**鏍瑰洜**锛歚ChartView.xaml` 涓娇鐢ㄤ簡涓嶅瓨鍦ㄧ殑 StaticResource 閿悕 `MaterialDesignOutlinedComboBox` 鍜?`MaterialDesignFlatButton`銆傞」鐩殑 `App.xaml` 浠呭悎骞朵簡 `Styles/Theme.xaml`锛岃€?`Theme.xaml` 涓畾涔夌殑瀵瑰簲鏍峰紡閿悕涓?`DarkComboBox` 鍜?`PrimaryButton`銆侻aterialDesignThemes 鐨勮祫婧愬瓧鍏告湭琚悎骞跺埌搴旂敤绾у埆锛屽洜姝よ繖涓や釜閿悕鍦ㄨ繍琛屾椂鏃犳硶瑙ｆ瀽
WPF 鍦?`InitializeComponent()` 闃舵瑙ｆ瀽 XAML 鏃舵姏鍑?`XamlParseException`锛屽穿婧冨彂鐢熷湪 ViewModel 浠ｇ爜鎵ц涔嬪墠锛屽鑷撮敊璇棩蹇楁湇鍔℃棤娉曡褰曘€?
## 娑夊強鏂囦欢

- `src/ArasToolkit.App/Views/ChartView.xaml:65` —`{StaticResource MaterialDesignOutlinedComboBox}` 搴旀敼涓?`{StaticResource DarkComboBox}`
- `src/ArasToolkit.App/Views/ChartView.xaml:92` —`{StaticResource MaterialDesignFlatButton}` 搴旀敼涓?`{StaticResource PrimaryButton}`
- `src/ArasToolkit.App/Views/ChartView.xaml:64` —`materialDesign:HintAssist.Hint` 鍦ㄦ棤 MD 瀛楀吀鍚堝苟鏃跺彲鑳戒笉宸ヤ綔锛屽缓璁Щ闄?
## 棰勬湡琛屼负

鐐瑰嚮銆屾暟鎹姤琛ㄣ€嶈彍鍗曢」鍚庢甯告樉绀烘煴鐘跺浘鐣岄潰锛屽寘鎷細
- 鏍囬鏍?+ 宸ュ叿鏍忥紙鍥捐〃绫诲瀷涓嬫媺 + 鍒锋柊鎸夐挳锛?- OxyPlot 鏌辩姸鍥?- 鍥捐〃绫诲瀷鍒囨崲锛堢姸鎬佸垎甯?/ 椤圭洰鍒嗗竷锛?
## Claude Code 鍒嗘瀽

### 璇佹嵁

1. 椤圭洰鎵€鏈夊叾浠?View 浣跨敤 `DarkComboBox`锛圗rrorLogView銆丒xcelImportView锛夊拰 `PrimaryButton`锛圕hangelogView銆丩oginView銆丱perationLogView锛夛紝鏃犱竴澶勪娇鐢?`MaterialDesignOutlinedComboBox` 鎴?`MaterialDesignFlatButton`
2. `App.xaml` 浠呭悎骞?`<ResourceDictionary Source="pack://application:,,,/Styles/Theme.xaml" />`锛屾湭鍚堝苟浠讳綍 MaterialDesignThemes 鍐呯疆瀛楀吀
3. `Styles/Theme.xaml` 瀹氫箟锛歚x:Key="DarkComboBox"`锛堢139琛岋級銆乣x:Key="PrimaryButton"`锛堢63琛岋級

### 淇寤鸿

```xml
<!-- ChartView.xaml 绗?9-65琛岋細ComboBox -->
<ComboBox ItemsSource="{Binding ChartTypeOptions}"
          SelectedItem="{Binding SelectedChartType}"
          Width="140" Height="32" FontSize="13"
          Style="{StaticResource DarkComboBox}" />

<!-- ChartView.xaml 绗?2-93琛岋細Button -->
<Button Grid.Column="3"
        Command="{Binding RefreshCommand}"
        Content="馃攧 鍒锋柊"
        Width="90" Height="32" FontSize="12"
        Style="{StaticResource PrimaryButton}" />
```

鍚屾椂绉婚櫎绗?琛屼笉蹇呰鐨?`xmlns:materialDesign` 鍛藉悕绌洪棿澹版槑鍜岀64琛?`materialDesign:HintAssist.Hint` 灞炴€с€?
## 缂栬瘧楠岃瘉

- [ ] `dotnet build ArasToolkit.slnx` 閫氳繃
- [ ] 鏃犳柊澧?Warning

## 淇缁撴灉锛圕odex 濉啓锛?
- 淇鐘舵€? [success / partial / failed]
- 缂栬瘧缁撴灉: [pass / fail]
- 澶囨敞: [Codex 濉啓鐨勪慨澶嶈鏄嶿

