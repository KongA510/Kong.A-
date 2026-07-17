import os
base = r'D:\博威\项目\ICS\个人工具箱\src'

# TranslationTask.cs
p1 = os.path.join(base, r'ArasToolkit.Core\Entities\Translation\TranslationTask.cs')
with open(p1, 'w', encoding='utf-8') as f:
    f.write("""using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

[Table("translation_task")]
public class TranslationTask
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("task_name")]
    [MaxLength(200)]
    [Required]
    public string TaskName { get; set; } = string.Empty;

    [Column("query_mode")]
    [MaxLength(50)]
    public string QueryMode { get; set; } = string.Empty;

    [Column("query_condition")]
    [MaxLength(1000)]
    public string? QueryCondition { get; set; }

    [Column("source_language")]
    [MaxLength(50)]
    public string? SourceLanguage { get; set; }

    [Column("target_languages")]
    [MaxLength(200)]
    public string? TargetLanguages { get; set; }

    [Column("total_fields")]
    public int TotalFields { get; set; }

    [Column("translated_fields")]
    public int TranslatedFields { get; set; }

    [Column("progress_text")]
    [MaxLength(100)]
    public string? ProgressText { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [Column("output_file_path")]
    [MaxLength(1000)]
    public string? OutputFilePath { get; set; }

    [Column("user_id")]
    [MaxLength(100)]
    public string? UserId { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
""")
print('TranslationTask.cs done')

# TranslationRecord.cs
p2 = os.path.join(base, r'ArasToolkit.Core\Entities\Translation\TranslationRecord.cs')
with open(p2, 'w', encoding='utf-8') as f:
    f.write("""using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArasToolkit.Core.Entities;

[Table("translation_record")]
public class TranslationRecord
{
    [Key]
    [Column("id")]
    [MaxLength(12)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];

    [Column("task_id")]
    [MaxLength(12)]
    [Required]
    public string TaskId { get; set; } = string.Empty;

    [Column("field_id")]
    [MaxLength(50)]
    public string? FieldId { get; set; }

    [Column("field_name")]
    [MaxLength(200)]
    public string? FieldName { get; set; }

    [Column("original_label")]
    [MaxLength(500)]
    public string? OriginalLabel { get; set; }

    [Column("translated_label")]
    [MaxLength(500)]
    public string? TranslatedLabel { get; set; }

    [Column("target_language")]
    [MaxLength(50)]
    public string? TargetLanguage { get; set; }

    [Column("creator_on")]
    public DateTime CreatorOn { get; set; } = DateTime.Now;

    [NotMapped]
    public string DisplayCreatedAt => CreatorOn.ToString("yyyy-MM-dd HH:mm:ss");
}
""")
print('TranslationRecord.cs done')

# IFieldTranslationService.cs
p3 = os.path.join(base, r'ArasToolkit.Core\Interfaces\Translation\IFieldTranslationService.cs')
with open(p3, 'w', encoding='utf-8') as f:
    f.write("""using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public class ArasFormItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class FieldItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Legend { get; set; } = string.Empty;
    public string FormName { get; set; } = string.Empty;
}

public interface IFieldTranslationService
{
    Task<List<ArasFormItem>> GetFormListAsync();
    Task<List<FieldItem>> GetFieldsByFormIdAsync(string formId);
    Task<List<FieldItem>> QueryFieldsByAmlAsync(string aml);
    Task<List<FieldItem>> QueryFieldsBySqlAsync(string sql);
    Task<TranslationTask> CreateTaskAsync(string taskName, string queryMode, string queryCondition, string sourceLanguage, string targetLanguages, int totalFields);
    Task TranslateAsync(TranslationTask task, List<FieldItem> fields, string sourceLanguage, string targetLanguages, IProgress<TranslationProgressInfo>? progress = null, CancellationToken cancellationToken = default);
    Task<string> ExportToExcelAsync(TranslationTask task, List<FieldItem> fields);
    Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);
}
""")
print('IFieldTranslationService.cs done')

# IPropertyTranslationService.cs
p4 = os.path.join(base, r'ArasToolkit.Core\Interfaces\Translation\IPropertyTranslationService.cs')
with open(p4, 'w', encoding='utf-8') as f:
    f.write("""using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Core.Interfaces;

public class ItemTypeItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class PropertyItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
}

public interface IPropertyTranslationService
{
    Task<List<ItemTypeItem>> GetItemTypeListAsync();
    Task<List<PropertyItem>> GetPropertiesByItemTypeIdAsync(string itemTypeId);
    Task<List<PropertyItem>> QueryPropertiesByAmlAsync(string aml);
    Task<List<PropertyItem>> QueryPropertiesBySqlAsync(string sql);
    Task<TranslationTask> CreateTaskAsync(string taskName, string queryMode, string queryCondition, string sourceLanguage, string targetLanguages, int totalFields);
    Task TranslateAsync(TranslationTask task, List<PropertyItem> properties, string sourceLanguage, string targetLanguages, IProgress<TranslationProgressInfo>? progress = null, CancellationToken cancellationToken = default);
    Task<string> ExportToExcelAsync(TranslationTask task, List<PropertyItem> properties);
    Task<(List<TranslationTask> Items, int TotalCount)> GetTaskHistoryAsync(string? userId = null, int page = 1, int pageSize = 20);
}
""")
print('IPropertyTranslationService.cs done')

print('All files created')
