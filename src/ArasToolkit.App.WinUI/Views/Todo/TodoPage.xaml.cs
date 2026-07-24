using System.Linq;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>
/// 个人任务页（WinUI 3）— 卡片列表 + 详情/编辑弹层 + 分页。
/// 卡片交互沿用 WPF 版逻辑：编辑/删除按钮、复选框多选、点击卡片打开详情。
/// </summary>
public sealed partial class TodoPage : Page
{
    public TodoPage()
    {
        this.InitializeComponent();
        DataContext = App.Services.GetRequiredService<TodoViewModel>();
    }

    private TodoViewModel? Vm => DataContext as TodoViewModel;

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: PersonalTask item })
            Vm?.EditCommand.Execute(item);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: PersonalTask item })
            Vm?.DeleteCommand.Execute(item.Id);
    }

    private void CardCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (Vm is { } vm)
        {
            vm.SelectedCount = vm.Entries.Count(item => item.IsSelected);
            vm.IsAllSelected = vm.SelectedCount == vm.Entries.Count && vm.Entries.Count > 0;
        }
    }

    private void Card_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // 点击卡片内的按钮/复选框时不触发详情
        if (IsInsideInteractiveControl(e.OriginalSource as DependencyObject)) return;
        if (sender is FrameworkElement { Tag: PersonalTask item })
            Vm?.OpenDetailCommand.Execute(item);
    }

    private void DetailEditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: PersonalTask item } && Vm is { } vm)
        {
            vm.CloseDetailCommand.Execute(null);
            vm.EditCommand.Execute(item);
        }
    }

    private void DetailPopupBg_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        Vm?.CloseDetailCommand.Execute(null);
    }

    private void SwallowPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private static bool IsInsideInteractiveControl(DependencyObject? obj)
    {
        while (obj != null)
        {
            if (obj is ButtonBase or CheckBox) return true;
            obj = VisualTreeHelper.GetParent(obj);
        }
        return false;
    }
}
