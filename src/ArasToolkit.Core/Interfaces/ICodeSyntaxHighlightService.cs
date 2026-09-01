using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>将源代码解析为 UI 可渲染的语法高亮区间。</summary>
public interface ICodeSyntaxHighlightService
{
    IReadOnlyList<CodeHighlightSpan> Highlight(string code, string? codeType);
}
