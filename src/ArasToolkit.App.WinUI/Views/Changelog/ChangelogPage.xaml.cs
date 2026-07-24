using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 更新日志页（WinUI 3）— 版本/统计卡片 + 类型筛选 + 时间线列表。
/// </summary>
public sealed partial class ChangelogPage : Page
{
    public ChangelogPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<ChangelogViewModel>();
    }
}
