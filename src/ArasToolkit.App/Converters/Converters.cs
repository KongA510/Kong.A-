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
        bool isEmpty;

        if (value is string s)
            isEmpty = string.IsNullOrEmpty(s);
        else
            isEmpty = value == null;

        bool invert = parameter?.ToString() == "Invert";
        return (isEmpty ^ invert) ? Visibility.Collapsed : Visibility.Visible;
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
/// 十六进制颜色字符串 → WPF Color 转换器
/// </summary>
public class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex && hex.StartsWith("#") && hex.Length == 7)
        {
            try
            {
                byte r = System.Convert.ToByte(hex.Substring(1, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(3, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(5, 2), 16);
                return Color.FromRgb(r, g, b);
            }
            catch { }
        }
        return Color.FromRgb(156, 163, 175); // 默认浅灰
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 十六进制颜色字符串 → SolidColorBrush（带 15% 不透明度）转换器
/// </summary>
public class StringToColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex && hex.StartsWith("#") && hex.Length == 7)
        {
            try
            {
                byte r = System.Convert.ToByte(hex.Substring(1, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(3, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(5, 2), 16);
                var color = Color.FromRgb(r, g, b);
                return new SolidColorBrush(Color.FromArgb(38, r, g, b)); // 15% opacity
            }
            catch { }
        }
        return new SolidColorBrush(Color.FromArgb(38, 156, 163, 175));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
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

/// <summary>
/// 根据 FileSystemItem 的 IsDirectory 返回对应 ToolTip 文本
/// （文件夹 →「在资源管理器中打开」/ 文件 →「在资源管理器中定位」）
/// </summary>
public class EnterTooltipConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ArasToolkit.Core.Models.FileSystemItem item)
            return "打开";
        return item.IsDirectory ? "在资源管理器中打开文件夹" : "在资源管理器中定位文件";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Null → false, 非Null → true 转换器（用于禁用控制）
/// </summary>
public class NotNullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && (value is not string s || !string.IsNullOrEmpty(s));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 多值字符串相等比较转换器 — 用于判断当前项是否正在加载（如 ConnectingId==ItemId）
/// 支持 values[0] 为 null 的场景（两个都为 null 时返回 false）
/// </summary>
public class MultiStringEqualityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2) return false;
        var a = values[0]?.ToString();
        var b = values[1]?.ToString();
        // 任意一方为空则不等
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}