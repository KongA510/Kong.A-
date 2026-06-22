using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.Views.Placeholder;

/// <summary>
/// PlaceholderView.xaml 的交互逻辑 — 支持子仪表盘卡片展示
/// </summary>
public partial class PlaceholderView : UserControl
{
    private Action<string>? _navigateCallback;

    public PlaceholderView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 设置占位页面标题
    /// </summary>
    public void SetTitle(string title, string? description = null)
    {
        TitleText.Text = title;
        if (!string.IsNullOrEmpty(description))
            DescriptionText.Text = description;
    }

    /// <summary>
    /// 动态添加子功能卡片列表（供父节点仪表盘使用）
    /// </summary>
    /// <param name="children">子功能列表</param>
    /// <param name="navigateCallback">点击卡片时的导航回调</param>
    public void AddChildItems(ObservableCollection<MenuItemInfo> children, Action<string>? navigateCallback = null)
    {
        _navigateCallback = navigateCallback;
        ChildItemsControl.ItemsSource = children;
        ChildItemsControl.Visibility = Visibility.Visible;
        EmptyPlaceholder.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// 子仪表盘功能卡片点击 → 导航到对应页面
    /// </summary>
    private void ChildCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is MenuItemInfo item)
        {
            _navigateCallback?.Invoke(item.Name);
        }
    }
}
