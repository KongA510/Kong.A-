using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

/// <summary>XML、JSON 格式化、比对与 .NET 实体转换服务。</summary>
public interface IDataToolService
{
    Task<string> FormatXmlAsync(string input);
    Task<DataComparisonResult> CompareXmlAsync(string left, string right);
    Task<string> FormatJsonAsync(string input);
    Task<DataComparisonResult> CompareJsonAsync(string left, string right);
    Task<string> JsonToEntityAsync(string json, string rootClassName);
    Task<string> EntityToJsonAsync(string entityCode);
}
