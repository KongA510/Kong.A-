using System.Security.Cryptography;
using System.Text;

namespace ArasToolkit.Core.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// MD5加密（模拟Aras.IOM.Innovator.ScalcMD5）
    /// </summary>
    public static string ToMd5(this string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
    
    /// <summary>
    /// 判断字符串是否为空
    /// </summary>
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
    
    /// <summary>
    /// 判断字符串是否不为空
    /// </summary>
    public static bool IsNotNullOrEmpty(this string? value) => !string.IsNullOrEmpty(value);
}
