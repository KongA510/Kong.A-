using System.Globalization;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Aras 经典窗体配置服务。查询“搜索中隐藏”未勾选（is_hidden2=0）的属性，
/// 生成四列绝对坐标布局，并通过安全构造的 AML 创建或覆盖窗体。
/// </summary>
public sealed class FormConfigurationService : IFormConfigurationService
{
    // ===== Aras 经典窗体布局规范 =====
    // 坐标与间距均使用 Aras Form/Field 的绝对坐标单位。
    public const int StartX = 50;
    public const int StartY = 50;
    public const int ColumnsPerRow = 4;
    public const int HorizontalSpacing = 200;
    public const int VerticalSpacing = 50;
    public const int ItemDisplayLength = 135;
    public const int TextDisplayLength = 150;

    // 这四个系统属性如果存在，必须固定占据第一行且保持下列顺序。
    private static readonly IReadOnlyDictionary<string, int> PreferredOrder =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["created_by_id"] = 0,
            ["created_on"] = 1,
            ["state"] = 2,
            ["item_number"] = 3
        };

    // 系统属性使用统一中文标签，避免不同对象类中的标签不一致。
    private static readonly IReadOnlyDictionary<string, string> PreferredLabels =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["created_by_id"] = "创建者",
            ["created_on"] = "创建时间",
            ["state"] = "状态",
            ["item_number"] = "编号"
        };

    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public FormConfigurationService(
        ArasConnectionService connectionService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _connectionService = connectionService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    /// <summary>
    /// 从当前已登录的 Aras 连接读取所有非关系对象类。
    /// 此方法只读取对象类基本信息，具体属性在用户选定一个对象类后再按需加载。
    /// </summary>
    public async Task<IReadOnlyList<ArasItemTypeInfo>> GetItemTypesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();

            // is_relationship=0：排除关系类，避免它们出现在窗体配置对象类列表中。
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "ItemType"),
                    new XAttribute("action", "get"),
                    new XAttribute("select", "id,name,label,is_relationship"),
                    new XAttribute("orderBy", "label"),
                    new XElement("is_relationship", "0")));

            var result = innovator.applyAML(ToAml(aml));
            ThrowIfError(result, "获取对象类失败");

            var items = new List<ArasItemTypeInfo>();
            for (var index = 0; index < result.getItemCount(); index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var item = result.getItemByIndex(index);
                var name = item.getProperty("name", string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                items.Add(new ArasItemTypeInfo
                {
                    Id = item.getID(),
                    Name = name,
                    Label = item.getProperty("label", name).Trim()
                });
            }

            return items;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("窗体配置-获取对象类", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>
    /// 读取指定对象类中未勾选“搜索中隐藏”的 Property。
    /// 同时读取数据类型、数据源、长度和排序号，供控件类型及布局预览使用。
    /// </summary>
    public async Task<IReadOnlyList<ArasFormProperty>> GetVisiblePropertiesAsync(
        string itemTypeId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemTypeId);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();

            // 通过 ItemType → Property 关系一次取回属性，避免逐个属性请求 Aras。
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "ItemType"),
                    new XAttribute("action", "get"),
                    new XAttribute("select", "id,name,label"),
                    new XElement("id", itemTypeId),
                    new XElement("Relationships",
                        new XElement("Item",
                            new XAttribute("type", "Property"),
                            new XAttribute("action", "get"),
                            new XAttribute("select", "id,name,label,data_type,data_source,stored_length,is_hidden2,sort_order"),
                            new XAttribute("orderBy", "sort_order"),
                            new XElement("is_hidden2", "0")))));

            var result = innovator.applyAML(ToAml(aml));
            ThrowIfError(result, "获取对象类属性失败");

            var itemType = result.getItemCount() > 0 ? result.getItemByIndex(0) : result;
            var relationships = itemType.getRelationships();
            var properties = new List<ArasFormProperty>();

            for (var index = 0; index < relationships.getItemCount(); index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var property = relationships.getItemByIndex(index);
                if (!property.getType().Equals("Property", StringComparison.OrdinalIgnoreCase))
                    continue;

                var name = property.getProperty("name", string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name) ||
                    property.getProperty("is_hidden2", "0") == "1")
                {
                    continue;
                }

                _ = int.TryParse(property.getProperty("sort_order", "0"),
                    NumberStyles.Integer, CultureInfo.InvariantCulture, out var sortOrder);

                properties.Add(new ArasFormProperty
                {
                    Id = property.getID(),
                    Name = name,
                    Label = property.getProperty("label", name).Trim(),
                    DataType = property.getProperty("data_type", "string").Trim(),
                    DataSourceId = property.getProperty("data_source", string.Empty).Trim(),
                    SortOrder = sortOrder,
                    IsHiddenInSearch = false,
                    IsSelected = true
                });
            }

            return properties;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("窗体配置-获取属性", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>
    /// 按项目规范生成四列绝对坐标布局。
    /// 系统优先字段先排序，其余字段再按 Aras sort_order 和名称稳定排序。
    /// </summary>
    public IReadOnlyList<ArasFormFieldLayout> BuildDefaultLayout(
        IEnumerable<ArasFormProperty> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var ordered = properties
            .Where(property => property.IsSelected && !property.IsHiddenInSearch)
            .OrderBy(property => PreferredOrder.TryGetValue(property.Name, out var priority)
                ? priority
                : int.MaxValue)
            .ThenBy(property => property.SortOrder)
            .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return ordered.Select((property, index) =>
        {
            // 每行四个控件：索引除以四得到行号，取模得到列号。
            var row = index / ColumnsPerRow;
            var column = index % ColumnsPerRow;
            var isItem = property.DataType.Equals("item", StringComparison.OrdinalIgnoreCase);
            var label = PreferredLabels.TryGetValue(property.Name, out var preferredLabel)
                ? preferredLabel
                : property.DisplayLabel;

            return new ArasFormFieldLayout
            {
                PropertyId = property.Id,
                Name = property.Name,
                Label = label,
                DataType = property.DataType,
                FieldType = MapFieldType(property.DataType),
                X = StartX + column * HorizontalSpacing,
                Y = StartY + row * VerticalSpacing,
                DisplayLength = isItem ? ItemDisplayLength : TextDisplayLength,
                Sequence = index + 1
            };
        }).ToList();
    }

    /// <summary>
    /// 将预览布局写入 Aras：创建新 Form，或在用户允许时覆盖同名 Form，
    /// 并可选择把该 Form 设为对象类默认视图。
    /// </summary>
    public async Task<ArasFormConfigurationResult> ApplyAsync(
        ArasFormConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();

            // 第一步：只检查同名 Form。没有找到属于正常新增链路，不应抛出异常。
            var existing = GetExistingForm(innovator, request.FormName);
            var wasUpdated = existing != null;

            if (wasUpdated && !request.ReplaceExisting)
                throw new InvalidOperationException($"窗体“{request.FormName}”已存在，请启用覆盖同名窗体后重试。");

            string formId;
            if (existing == null)
            {
                // 第二步（新增）：在一个 AML 中创建 Form、Body 及全部 Field。
                var addResult = innovator.applyAML(ToAml(BuildAddFormAml(request)));
                ThrowIfError(addResult, "创建窗体失败");
                formId = addResult.getItemCount() > 0
                    ? addResult.getItemByIndex(0).getID()
                    : addResult.getID();
            }
            else
            {
                // 第二步（覆盖）：保留 Form 身份，替换 Body 下的 Field 并更新尺寸。
                formId = existing.getID();
                ReplaceFormFields(innovator, existing, request, cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(formId))
                throw new InvalidOperationException("Aras 已返回成功结果，但未返回窗体 ID。");

            var viewAssigned = false;
            if (request.SetAsDefaultView)
            {
                // 第三步：确保对象类存在指向该 Form 的默认 View 关系。
                viewAssigned = EnsureDefaultView(innovator, request.ItemTypeId, formId);
            }

            try
            {
                // 第四步：记录敏感写操作；日志失败不能回滚已经成功的 Aras 写入。
                await _operationLogService.LogAsync(
                    wasUpdated ? "Update" : "Create",
                    "ArasFormConfiguration",
                    formId,
                    $"{(wasUpdated ? "覆盖" : "创建")}窗体: {request.FormName}，对象类: {request.ItemTypeName}，字段: {request.Fields.Count}")
                    .ConfigureAwait(false);
            }
            catch (Exception logException)
            {
                System.Diagnostics.Debug.WriteLine($"[FormConfiguration] 操作日志写入失败: {logException.Message}");
            }

            return new ArasFormConfigurationResult
            {
                FormId = formId,
                FormName = request.FormName,
                FieldCount = request.Fields.Count,
                WasUpdated = wasUpdated,
                ViewAssigned = viewAssigned
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("窗体配置-写入Aras", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>获取强类型 Innovator；未连接时立即给出明确错误。</summary>
    private Innovator GetInnovator() => _connectionService.TypedInnovator
        ?? throw new InvalidOperationException("未连接到 Aras 系统，请先在“Aras连接”页面登录。");

    /// <summary>
    /// 查询同名 Form，并同时加载 Body/Field，供覆盖流程复用。
    /// Aras IOM 对“合法查询但零条匹配”会同时返回 isError=true 与 isEmpty=true；
    /// 这种结果表示 Form 不存在，必须返回 null 进入新增链路，不能作为真实错误抛出。
    /// </summary>
    private static Item? GetExistingForm(Innovator innovator, string formName)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "Form"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id,name,label,width,height"),
                new XElement("name", formName),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "Body"),
                        new XAttribute("action", "get"),
                        new XAttribute("select", "id"),
                        new XElement("Relationships",
                            new XElement("Item",
                                new XAttribute("type", "Field"),
                                new XAttribute("action", "get"),
                                new XAttribute("select", "id")))))));

        var result = innovator.applyAML(ToAml(aml));

        // 必须先判断 isEmpty。零条结果在 IOM 中可能也是 Error Item（错误码 0）。
        if (result.isEmpty())
            return null;

        // 只有非空的 Error Item 才代表权限、AML、连接或服务器等真实错误。
        ThrowIfError(result, "检查同名窗体失败");
        return result.getItemCount() > 0 ? result.getItemByIndex(0) : null;
    }

    /// <summary>
    /// 覆盖现有 Form 的字段。已有 Body 时删除旧 Field 后批量新增；
    /// 老数据没有 Body 时则补建 Body，并在两条路径中同步更新窗体尺寸。
    /// </summary>
    private static void ReplaceFormFields(
        Innovator innovator,
        Item existingForm,
        ArasFormConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        Item? body = null;
        var formRelationships = existingForm.getRelationships();
        for (var index = 0; index < formRelationships.getItemCount(); index++)
        {
            var candidate = formRelationships.getItemByIndex(index);
            if (candidate.getType().Equals("Body", StringComparison.OrdinalIgnoreCase))
            {
                body = candidate;
                break;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (body != null)
        {
            // 删除旧字段、更新窗体尺寸并写入新字段放在同一 AML 请求中，
            // 尽量避免覆盖过程中出现“旧字段已删但新字段尚未写入”的中间状态。
            var updateAml = new XElement("AML",
                BuildFormDimensionsItem(request, existingForm.getID()));
            var fields = body.getRelationships();
            for (var index = 0; index < fields.getItemCount(); index++)
            {
                var field = fields.getItemByIndex(index);
                if (field.getType().Equals("Field", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(field.getID()))
                {
                    updateAml.Add(new XElement("Item",
                        new XAttribute("type", "Field"),
                        new XAttribute("action", "delete"),
                        new XAttribute("id", field.getID())));
                }
            }

            updateAml.Add(new XElement("Item",
                new XAttribute("type", "Body"),
                new XAttribute("action", "edit"),
                new XAttribute("id", body.getID()),
                new XElement("Relationships", BuildFieldItems(request.Fields))));
            var updateResult = innovator.applyAML(ToAml(updateAml));
            ThrowIfError(updateResult, "覆盖窗体字段失败");
            return;
        }

        var addBodyAml = new XElement("AML",
            BuildFormDimensionsItem(request, existingForm.getID(),
                new XElement("Relationships", BuildBodyItem(request.Fields))));
        var addBodyResult = innovator.applyAML(ToAml(addBodyAml));
        ThrowIfError(addBodyResult, "为现有窗体创建主体失败");
    }

    /// <summary>
    /// 确保 ItemType 与 Form 之间存在默认 View。
    /// 已有关联时更新其类型与优先级；没有关联时新增 View 关系。
    /// </summary>
    private static bool EnsureDefaultView(Innovator innovator, string itemTypeId, string formId)
    {
        var queryAml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "ItemType"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("id", itemTypeId),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "View"),
                        new XAttribute("action", "get"),
                        new XAttribute("select", "id,related_id,type,display_priority")))));

        var result = innovator.applyAML(ToAml(queryAml));
        ThrowIfError(result, "检查对象类视图失败");
        var itemType = result.getItemCount() > 0 ? result.getItemByIndex(0) : result;
        var views = itemType.getRelationships();
        for (var index = 0; index < views.getItemCount(); index++)
        {
            var view = views.getItemByIndex(index);
            if (view.getType().Equals("View", StringComparison.OrdinalIgnoreCase) &&
                view.getProperty("related_id", string.Empty).Equals(formId, StringComparison.OrdinalIgnoreCase))
            {
                var editViewAml = new XElement("AML",
                    new XElement("Item",
                        new XAttribute("type", "View"),
                        new XAttribute("action", "edit"),
                        new XAttribute("id", view.getID()),
                        new XElement("type", "default"),
                        new XElement("display_priority", "0")));
                var editResult = innovator.applyAML(ToAml(editViewAml));
                ThrowIfError(editResult, "更新默认视图失败");
                return true;
            }
        }

        var addViewAml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "ItemType"),
                new XAttribute("action", "edit"),
                new XAttribute("id", itemTypeId),
                new XElement("Relationships",
                    new XElement("Item",
                        new XAttribute("type", "View"),
                        new XAttribute("action", "add"),
                        new XElement("related_id", formId),
                        new XElement("type", "default"),
                        new XElement("display_priority", "0")))));
        var addResult = innovator.applyAML(ToAml(addViewAml));
        ThrowIfError(addResult, "关联对象类默认视图失败");
        return true;
    }

    /// <summary>构造新增 Form、Body 和 Field 的完整 AML。</summary>
    private static XElement BuildAddFormAml(ArasFormConfigurationRequest request) =>
        new("AML",
            new XElement("Item",
                new XAttribute("type", "Form"),
                new XAttribute("action", "add"),
                new XElement("name", request.FormName),
                new XElement("label", request.FormLabel),
                new XElement("width", CalculateFormWidth()),
                new XElement("height", CalculateFormHeight(request.Fields)),
                new XElement("Relationships", BuildBodyItem(request.Fields))));

    /// <summary>构造更新 Form 标签和尺寸的 Item，可选附带关系数据。</summary>
    private static XElement BuildFormDimensionsItem(
        ArasFormConfigurationRequest request,
        string formId,
        XElement? relationships = null)
    {
        var item = new XElement("Item",
            new XAttribute("type", "Form"),
            new XAttribute("action", "edit"),
            new XAttribute("id", formId),
            new XElement("label", request.FormLabel),
            new XElement("width", CalculateFormWidth()),
            new XElement("height", CalculateFormHeight(request.Fields)));
        if (relationships != null)
            item.Add(relationships);
        return item;
    }

    /// <summary>构造 Form 下的 Body 关系项。</summary>
    private static XElement BuildBodyItem(IReadOnlyList<ArasFormFieldLayout> fields) =>
        new("Item",
            new XAttribute("type", "Body"),
            new XAttribute("action", "add"),
            new XElement("Relationships", BuildFieldItems(fields)));

    /// <summary>
    /// 将布局模型转换为 Aras Field 关系项。
    /// propertytype_id 保持 Field 与原始 Property 的绑定关系。
    /// </summary>
    private static IEnumerable<XElement> BuildFieldItems(
        IReadOnlyList<ArasFormFieldLayout> fields) => fields.Select(field =>
            new XElement("Item",
                new XAttribute("type", "Field"),
                new XAttribute("action", "add"),
                new XElement("name", field.Name),
                new XElement("label", field.Label),
                new XElement("field_type", field.FieldType),
                new XElement("propertytype_id", field.PropertyId),
                new XElement("x", field.X),
                new XElement("y", field.Y),
                new XElement("display_length", field.DisplayLength)));

    /// <summary>按四列布局计算窗体宽度，并为最右侧控件保留额外边距。</summary>
    private static int CalculateFormWidth() =>
        StartX + (ColumnsPerRow - 1) * HorizontalSpacing + TextDisplayLength + 50;

    /// <summary>按最后一行 Y 坐标计算窗体高度，并保留底部操作空间。</summary>
    private static int CalculateFormHeight(IReadOnlyList<ArasFormFieldLayout> fields) =>
        fields.Count == 0 ? 200 : fields.Max(field => field.Y) + 100;

    /// <summary>把 Aras Property 数据类型映射为经典 Form Field 控件类型。</summary>
    private static string MapFieldType(string dataType) => dataType.Trim().ToLowerInvariant() switch
    {
        "item" => "item",
        "list" or "mv_list" or "filter list" => "dropdown",
        "boolean" => "checkbox",
        "date" => "date",
        _ => "text"
    };

    /// <summary>在生成 AML 前校验请求，防止写入无法绑定 Property 的无效 Field。</summary>
    private static void ValidateRequest(ArasFormConfigurationRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ItemTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ItemTypeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FormName);
        if (request.Fields.Count == 0)
            throw new InvalidOperationException("至少需要选择一个属性才能生成窗体。");
        if (request.Fields.Any(field => string.IsNullOrWhiteSpace(field.PropertyId)))
            throw new InvalidOperationException("存在未关联 Aras Property ID 的字段，无法生成窗体。");
    }

    /// <summary>
    /// 将真实 Aras Error Item 转换为带业务上下文的异常。
    /// 调用方若允许“零条结果”，必须先通过 Item.isEmpty() 单独处理。
    /// </summary>
    private static void ThrowIfError(Item result, string context)
    {
        if (result.isError())
            throw new InvalidOperationException($"{context}: {result.getErrorString()}");
    }

    /// <summary>以紧凑格式序列化 AML，XElement 会自动转义用户输入。</summary>
    private static string ToAml(XElement aml) => aml.ToString(SaveOptions.DisableFormatting);
}
