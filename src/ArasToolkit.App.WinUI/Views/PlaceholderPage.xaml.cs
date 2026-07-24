using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 通用占位页 — 显示导航参数（菜单名），用于尚未迁移的功能页面。
/// </summary>
public sealed partial class PlaceholderPage : Page
{
    public PlaceholderPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var name = e.Parameter?.ToString() ?? "页面";
        TitleText.Text = name;

        // 个人资料库：经决策(C)保留为 WPF 版独有，WinUI 版不作迁移，显示明确说明。
        if (name == "个人资料库")
        {
            IconText.Text = "📚";
            DescText.Text = "个人资料库基于 WPF 富文本（FlowDocument）存储，与 WinUI 3 数据格式不兼容。经评估，该功能仅在 WPF 版（ArasToolkit.App）中提供，WinUI 版暂不支持。";
        }
        else
        {
            DescText.Text = $"{name} — 该功能页面正在迁移到 WinUI 3，敬请期待。";
        }
    }
}
