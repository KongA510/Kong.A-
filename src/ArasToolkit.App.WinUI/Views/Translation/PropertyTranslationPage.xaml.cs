using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 属性翻译页（WinUI 3）— 查询字段/属性、AI 批量翻译、进度与任务历史。
/// </summary>
public sealed partial class PropertyTranslationPage : Page
{
    public PropertyTranslationPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<PropertyTranslationViewModel>();
    }
}
