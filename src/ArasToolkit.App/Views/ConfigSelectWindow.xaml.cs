using System.Windows;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.Views;

/// <summary>
/// AML配置选择窗口 — 选择已保存的配置并返回
/// </summary>
public partial class ConfigSelectWindow : Window
{
    public ConfigSelectWindow(DataImportViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SelectConfig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DataImportConfig config)
        {
           if (DataContext is DataImportViewModel vm)
           {
                vm.AmlContent = config.AmlContent;
                vm.SelectedConfig = config;
            }
            DialogResult = true;
            Close();
        }
    }

    private void DeleteConfig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DataImportConfig config)
        {
            if (DataContext is DataImportViewModel vm)
            {
                vm.DeleteConfigCommand.Execute(config.Id);
            }
        }
    }
}
