using System.Text;
using System.Text.RegularExpressions;

namespace ArasToolkit.Services.Services;

/// <summary>阻止数据库导出功能执行写入、DDL 或高风险外部查询语句。</summary>
internal static partial class SqlReadOnlyQueryValidator
{
    private static readonly HashSet<string> ForbiddenTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "ALTER", "BACKUP", "BULK", "CREATE", "DBCC", "DELETE", "DENY", "DROP",
        "EXEC", "EXECUTE", "GRANT", "INSERT", "INTO", "MERGE", "OPENQUERY",
        "OPENROWSET", "OPENDATASOURCE", "RECONFIGURE", "RESTORE", "REVOKE",
        "TRUNCATE", "UPDATE", "USE", "WRITETEXT"
    };

    public static void EnsureReadOnly(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            throw new InvalidOperationException("SQL 查询不能为空。");

        var normalized = StripCommentsAndLiterals(sql);
        var tokens = TokenRegex().Matches(normalized)
            .Select(match => match.Value)
            .ToList();

        if (tokens.Count == 0 ||
            (!tokens[0].Equals("SELECT", StringComparison.OrdinalIgnoreCase) &&
             !tokens[0].Equals("WITH", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("数据库导出仅允许 SELECT 或以 WITH 开头的只读查询。");
        }

        var forbidden = tokens.FirstOrDefault(token => ForbiddenTokens.Contains(token));
        if (forbidden != null)
            throw new InvalidOperationException($"数据库导出检测到非只读关键字“{forbidden}”，已拒绝执行。");
    }

    private static string StripCommentsAndLiterals(string sql)
    {
        var output = new StringBuilder(sql.Length);
        var state = SqlLexicalState.Normal;

        for (var index = 0; index < sql.Length; index++)
        {
            var current = sql[index];
            var next = index + 1 < sql.Length ? sql[index + 1] : '\0';

            switch (state)
            {
                case SqlLexicalState.Normal when current == '-' && next == '-':
                    output.Append("  ");
                    index++;
                    state = SqlLexicalState.LineComment;
                    break;
                case SqlLexicalState.Normal when current == '/' && next == '*':
                    output.Append("  ");
                    index++;
                    state = SqlLexicalState.BlockComment;
                    break;
                case SqlLexicalState.Normal when current == '\'':
                    output.Append(' ');
                    state = SqlLexicalState.StringLiteral;
                    break;
                case SqlLexicalState.Normal when current == '[':
                    output.Append(' ');
                    state = SqlLexicalState.BracketIdentifier;
                    break;
                case SqlLexicalState.Normal when current == '"':
                    output.Append(' ');
                    state = SqlLexicalState.QuotedIdentifier;
                    break;
                case SqlLexicalState.LineComment:
                    output.Append(char.IsWhiteSpace(current) ? current : ' ');
                    if (current is '\r' or '\n')
                        state = SqlLexicalState.Normal;
                    break;
                case SqlLexicalState.BlockComment when current == '*' && next == '/':
                    output.Append("  ");
                    index++;
                    state = SqlLexicalState.Normal;
                    break;
                case SqlLexicalState.BlockComment:
                    output.Append(char.IsWhiteSpace(current) ? current : ' ');
                    break;
                case SqlLexicalState.StringLiteral when current == '\'' && next == '\'':
                    output.Append("  ");
                    index++;
                    break;
                case SqlLexicalState.StringLiteral when current == '\'':
                    output.Append(' ');
                    state = SqlLexicalState.Normal;
                    break;
                case SqlLexicalState.BracketIdentifier when current == ']' && next == ']':
                    output.Append("  ");
                    index++;
                    break;
                case SqlLexicalState.BracketIdentifier when current == ']':
                    output.Append(' ');
                    state = SqlLexicalState.Normal;
                    break;
                case SqlLexicalState.QuotedIdentifier when current == '"' && next == '"':
                    output.Append("  ");
                    index++;
                    break;
                case SqlLexicalState.QuotedIdentifier when current == '"':
                    output.Append(' ');
                    state = SqlLexicalState.Normal;
                    break;
                case SqlLexicalState.StringLiteral:
                case SqlLexicalState.BracketIdentifier:
                case SqlLexicalState.QuotedIdentifier:
                    output.Append(char.IsWhiteSpace(current) ? current : ' ');
                    break;
                default:
                    output.Append(current);
                    break;
            }
        }

        return output.ToString();
    }

    [GeneratedRegex(@"\b[A-Za-z_]+\b", RegexOptions.CultureInvariant)]
    private static partial Regex TokenRegex();

    private enum SqlLexicalState
    {
        Normal,
        LineComment,
        BlockComment,
        StringLiteral,
        BracketIdentifier,
        QuotedIdentifier
    }
}
