using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 数据汇入页（WinUI 3）— Excel 导入 Aras：文件/范围、列映射、数据预览、AML 模板与执行。
/// </summary>
public sealed partial class DataImportPage : Page
{
    public DataImportPage()
    {
        this.InitializeComponent();
        var vm = App.Services.GetRequiredService<DataImportViewModel>();
        vm.RequestConfigSelectionAsync = ShowConfigSelectorAsync;
        vm.PropertyChanged += OnViewModelPropertyChanged;
        DataContext = vm;
    }

    private DataImportViewModel? Vm => DataContext as DataImportViewModel;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DataImportViewModel.PreviewData) && sender is DataImportViewModel vm)
            RenderPreview(vm.PreviewData);
    }

    /// <summary>原生渲染数据预览表格（WinUI 无 AutoGenerateColumns DataGrid，改为代码后台构建）。</summary>
    private void RenderPreview(DataTable? dt)
    {
        PreviewContainer.Children.Clear();
        if (dt == null || dt.Columns.Count == 0)
        {
            PreviewPlaceholder.Visibility = Visibility.Visible;
            return;
        }
        PreviewPlaceholder.Visibility = Visibility.Collapsed;

        const double cellWidth = 120;
        var textBrush = (Brush)Application.Current.Resources["TextPrimaryBrush"];
        var headerBg = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];

        var header = new StackPanel { Orientation = Orientation.Horizontal, Background = headerBg };
        foreach (DataColumn col in dt.Columns)
        {
            header.Children.Add(new TextBlock
            {
                Text = col.ColumnName,
                Width = cellWidth,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = textBrush,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Padding = new Thickness(6, 7, 6, 7),
            });
        }
        PreviewContainer.Children.Add(header);

        foreach (DataRow row in dt.Rows)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };
            foreach (DataColumn col in dt.Columns)
            {
                var val = row[col];
                rowPanel.Children.Add(new TextBlock
                {
                    Text = val == DBNull.Value ? "" : val?.ToString() ?? "",
                    Width = cellWidth,
                    FontSize = 12,
                    Foreground = textBrush,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Padding = new Thickness(6, 5, 6, 5),
                });
            }
            PreviewContainer.Children.Add(rowPanel);
        }
    }

    /// <summary>配置选择对话框（替代 WPF ConfigSelectWindow）。返回是否选择了配置。</summary>
    private async Task<bool> ShowConfigSelectorAsync()
    {
        var vm = Vm;
        if (vm == null) return false;

        bool selected = false;
        var listPanel = new StackPanel { Spacing = 6 };

        var dialog = new ContentDialog
        {
            Title = "选择 AML 配置",
            XamlRoot = this.XamlRoot,
            CloseButtonText = "关闭",
        };

        void Rebuild()
        {
            listPanel.Children.Clear();
            foreach (var config in vm.SavedConfigs)
            {
                var row = new Grid { ColumnSpacing = 8, Padding = new Thickness(4, 2, 4, 2) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var name = new TextBlock
                {
                    Text = config.ConfigName,
                    FontSize = 13,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(name, 0);

                var selectButton = new Button
                {
                    Content = "选择",
                    Height = 28,
                    Style = (Style)Application.Current.Resources["AccentButton"],
                };
                selectButton.Click += (s, e) =>
                {
                    vm.AmlContent = config.AmlContent;
                    vm.SelectedConfig = config;
                    selected = true;
                    dialog.Hide();
                };
                Grid.SetColumn(selectButton, 1);

                var deleteButton = new Button
                {
                    Content = "删除",
                    Height = 28,
                    Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0xEF, 0x44, 0x44)),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                };
                deleteButton.Click += (s, e) => vm.DeleteConfigCommand.Execute(config.Id);
                Grid.SetColumn(deleteButton, 2);

                row.Children.Add(name);
                row.Children.Add(selectButton);
                row.Children.Add(deleteButton);
                listPanel.Children.Add(row);
            }

            if (vm.SavedConfigs.Count == 0)
            {
                listPanel.Children.Add(new TextBlock
                {
                    Text = "暂无已保存的配置",
                    FontSize = 12,
                    Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                });
            }
        }

        Rebuild();
        PropertyChangedEventHandler handler = (s, e) =>
        {
            if (e.PropertyName == nameof(vm.SavedConfigs)) Rebuild();
        };
        vm.PropertyChanged += handler;

        dialog.Content = new ScrollViewer { Content = listPanel, MaxHeight = 420 };
        try
        {
            await dialog.ShowAsync();
        }
        finally
        {
            vm.PropertyChanged -= handler;
        }
        return selected;
    }
}
