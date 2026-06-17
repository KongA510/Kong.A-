using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ArasToolkit.App.Converters;

/// <summary>
/// Bool to Visibility converter
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            bool invert = parameter?.ToString() == "Invert";
            return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
            return visibility == Visibility.Visible;
        return false;
    }
}

/// <summary>
/// String non-empty to Visibility converter
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverse Bool converter
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        return true;
    }
}

/// <summary>
/// Status to Color converter (for status label background)
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? "";
        var color = status switch
        {
            "未开始" => System.Windows.Media.Color.FromRgb(107, 114, 128),
            "进行中" => System.Windows.Media.Color.FromRgb(99, 102, 241),
            "已延期" => System.Windows.Media.Color.FromRgb(239, 68, 68),
            "已完成" => System.Windows.Media.Color.FromRgb(16, 185, 129),
            _ => System.Windows.Media.Color.FromRgb(107, 114, 128)
        };
        return new System.Windows.Media.SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Completion percent to ProgressBar color converter (0-30 red, 31-60 yellow, 61-99 orange, 100 green)
/// </summary>
public class ProgressToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int percent;
        if (value is double d) { percent = (int)d; }
        else if (value is int i) { percent = i; }
        else { percent = 0; }

        if (percent >= 100) return new SolidColorBrush(Color.FromRgb(16, 185, 129));
        if (percent >= 61) return new SolidColorBrush(Color.FromRgb(249, 115, 22));
        if (percent >= 31) return new SolidColorBrush(Color.FromRgb(245, 158, 11));
        return new SolidColorBrush(Color.FromRgb(239, 68, 68));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}