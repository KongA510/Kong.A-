using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using ArasToolkit.App.ViewModels;
using ArasToolkit.Core.Entities;

namespace ArasToolkit.App.Views;

/// <summary>
/// TodoView.xaml code-behind
/// </summary>
public partial class TodoView : UserControl
{
    public TodoView()
    {
        InitializeComponent();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is PersonalTask item)
        {
            if (DataContext is TodoViewModel vm)
            {
                vm.EditCommand.Execute(item);
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is PersonalTask item)
        {
            if (DataContext is TodoViewModel vm)
            {
                vm.DeleteCommand.Execute(item.Id);
            }
        }
    }

    private void SelectionCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is TodoViewModel vm)
        {
            vm.SelectedCount = vm.Entries.Count(item => item.IsSelected);
            vm.IsAllSelected = vm.SelectedCount == vm.Entries.Count && vm.Entries.Count > 0;
        }
    }

    private void CardCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        SelectionCheckBox_Changed(sender, e);
    }

    private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is PersonalTask item)
        {
            if (DataContext is TodoViewModel vm)
                vm.OpenDetailCommand.Execute(item);
        }
    }

    private void DetailEditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is PersonalTask item)
        {
            if (DataContext is TodoViewModel vm)
            {
                vm.CloseDetailCommand.Execute(null);
                vm.EditCommand.Execute(item);
            }
        }
    }

    private void DetailPopupBg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is TodoViewModel vm)
        {
            vm.CloseDetailCommand.Execute(null);
        }
    }
}