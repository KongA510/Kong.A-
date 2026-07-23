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
        DescText.Text = $"{name} — 该功能页面正在迁移到 WinUI 3，敬请期待。";
    }
}
