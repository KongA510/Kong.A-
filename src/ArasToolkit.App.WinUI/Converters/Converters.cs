using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ArasToolkit.App.WinUI.Converters;

/// <summary>Bool → Visibility（支持 Invert 参数反转）。</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
        {
            bool invert = parameter?.ToString() == "Invert";
            return (b ^ invert) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility v) return v == Visibility.Visible;
        return false;
    }
}

/// <summary>字符串非空 → Visibility（支持 Invert）。</summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isEmpty = value is string s ? string.IsNullOrEmpty(s) : value == null;
        bool invert = parameter?.ToString() == "Invert";
        return (isEmpty ^ invert) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>Bool 取反。</summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is bool b ? !b : true;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is bool b ? !b : true;
}

/// <summary>十六进制颜色字符串 → Windows.UI.Color。</summary>
public class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string hex && hex.StartsWith("#") && hex.Length == 7)
        {
            try
            {
                byte r = System.Convert.ToByte(hex.Substring(1, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(3, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(5, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
            catch { }
        }
        return Color.FromArgb(255, 156, 163, 175);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>十六进制颜色字符串 → SolidColorBrush（15% 不透明度）。</summary>
public class StringToColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string hex && hex.StartsWith("#") && hex.Length == 7)
        {
            try
            {
                byte r = System.Convert.ToByte(hex.Substring(1, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(3, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(5, 2), 16);
                return new SolidColorBrush(Color.FromArgb(38, r, g, b));
            }
            catch { }
        }
        return new SolidColorBrush(Color.FromArgb(38, 156, 163, 175));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>任务状态 → SolidColorBrush。</summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var status = value?.ToString() ?? "";
        var color = status switch
        {
            "未开始" => Color.FromArgb(255, 107, 114, 128),
            "进行中" => Color.FromArgb(255, 99, 102, 241),
            "已延期" => Color.FromArgb(255, 239, 68, 68),
            "已完成" => Color.FromArgb(255, 16, 185, 129),
            _ => Color.FromArgb(255, 107, 114, 128)
        };
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>非 Null/非空字符串 → true。</summary>
public class NotNullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value != null && (value is not string s || !string.IsNullOrEmpty(s));

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}


/// <summary>更新日志类型（新增/修复/优化/移除）→ 画笔。参数 "bg" 返回 10% 透明背景，否则返回前景色。</summary>
public class LogTypeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = value?.ToString() ?? "";
        var color = type switch
        {
            "新增" => Color.FromArgb(255, 16, 185, 129),
            "修复" => Color.FromArgb(255, 245, 158, 11),
            "优化" => Color.FromArgb(255, 59, 130, 246),
            "移除" => Color.FromArgb(255, 239, 68, 68),
            _ => Color.FromArgb(255, 107, 114, 128)
        };
        if (parameter?.ToString() == "bg")
            return new SolidColorBrush(Color.FromArgb(26, color.R, color.G, color.B));
        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>Aras 连接状态文本 → 状态画笔（已连接=成功绿，其它=临界红）。</summary>
public class ConnectionStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var connected = value?.ToString() == "已连接";
        return connected
            ? new SolidColorBrush(Color.FromArgb(255, 16, 185, 129))
            : new SolidColorBrush(Color.FromArgb(255, 239, 68, 68));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}


/// <summary>通用字符串格式化转换器 — ConverterParameter 为复合格式串（如 'v{0}'），等价于 WPF 的 StringFormat。</summary>
public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string fmt && !string.IsNullOrEmpty(fmt))
        {
            try { return string.Format(fmt, value ?? ""); }
            catch { return value?.ToString() ?? ""; }
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
