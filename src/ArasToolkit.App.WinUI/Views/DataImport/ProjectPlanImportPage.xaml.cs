using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>项目计划模板导出、上传预检与汇入页面。</summary>
public sealed partial class ProjectPlanImportPage : Page
{
    public ProjectPlanImportPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ProjectPlanImportViewModel>();
    }
}
