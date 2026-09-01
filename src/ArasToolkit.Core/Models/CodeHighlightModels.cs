namespace ArasToolkit.Core.Models;

/// <summary>代码高亮片段的语义样式。</summary>
public enum CodeHighlightStyle
{
    Default,
    Keyword,
    Type,
    String,
    Comment,
    Number,
    Operator,
    Markup,
    Attribute,
    AttributeValue
}

/// <summary>源代码中一个连续的高亮区间。</summary>
public readonly record struct CodeHighlightSpan(
    int Start,
    int Length,
    CodeHighlightStyle Style);
