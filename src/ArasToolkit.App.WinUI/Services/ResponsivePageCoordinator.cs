using System;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Services;

/// <summary>
/// Applies the common responsive contract to every navigated page. Page-specific
/// layouts can still handle their own column/row reflow while this coordinator
/// keeps the page viewport stretched and prevents page-level horizontal scrolling.
/// </summary>
public static class ResponsivePageCoordinator
{
    private const double CompactWidth = 760;
    private const double WideWidth = 1180;
    private static readonly ConditionalWeakTable<Page, object> AttachedPages = new();

    public static void Attach(Page page)
    {
        if (AttachedPages.TryGetValue(page, out _))
        {
            return;
        }

        AttachedPages.Add(page, new object());
        var outerScrollViewer = FindOuterScrollViewer(page.Content);
        if (outerScrollViewer is not null)
        {
            outerScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            outerScrollViewer.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        var marginTarget = outerScrollViewer?.Content as FrameworkElement
            ?? page.Content as FrameworkElement;
        if (marginTarget is null)
        {
            return;
        }

        var originalMargin = marginTarget.Margin;
        var originalPadding = outerScrollViewer?.Padding ?? default;

        void Apply(double width)
        {
            marginTarget.HorizontalAlignment = HorizontalAlignment.Stretch;
            marginTarget.Margin = GetResponsiveThickness(originalMargin, width);

            if (outerScrollViewer is not null)
            {
                outerScrollViewer.Padding = GetResponsiveThickness(originalPadding, width);
            }
        }

        page.SizeChanged += (_, args) => Apply(args.NewSize.Width);
        page.Loaded += (_, _) => Apply(page.ActualWidth);
    }

    private static ScrollViewer? FindOuterScrollViewer(object? content)
    {
        if (content is ScrollViewer scrollViewer)
        {
            return scrollViewer;
        }

        if (content is Panel { Children.Count: 1 } panel && panel.Children[0] is ScrollViewer child)
        {
            return child;
        }

        return null;
    }

    private static Thickness GetResponsiveThickness(Thickness original, double width)
    {
        if (original.Left < 20 && original.Right < 20 && original.Top < 20 && original.Bottom < 20)
        {
            return original;
        }

        if (width < CompactWidth)
        {
            return new Thickness(
                Math.Min(original.Left, 16),
                Math.Min(original.Top, 16),
                Math.Min(original.Right, 16),
                Math.Min(original.Bottom, 20));
        }

        if (width < WideWidth)
        {
            return new Thickness(
                Math.Min(original.Left, 24),
                Math.Min(original.Top, 20),
                Math.Min(original.Right, 24),
                Math.Min(original.Bottom, 28));
        }

        return original;
    }
}
