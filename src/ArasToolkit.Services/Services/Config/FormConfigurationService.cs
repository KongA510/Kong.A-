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
    public const int DefaultTextAreaRows = 100;
    public const int DefaultTextAreaColumns = 340;

    // HTML 边框字段固定放在所有数据字段之后，并以负层级显示在控件背后。
    private const int BorderFieldX = 10;
    private const int BorderFieldY = 20;
    private const int BorderFieldZIndex = -1;
    private const int BorderHtmlWidth = 830;
    private const int BorderHeightOffset = 50;

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
                FieldType = ArasFormConfigurationOptions.GetDefaultFieldType(property.DataType),
                X = StartX + column * HorizontalSpacing,
                Y = StartY + row * VerticalSpacing,
                DisplayLength = isItem ? ItemDisplayLength : TextDisplayLength,
                Sequence = index + 1,
                TextAreaRows = DefaultTextAreaRows,
                TextAreaColumns = DefaultTextAreaColumns
            };
        }).ToList();
    }

    /// <summary>按当前集合顺序重新编号；保留用户编辑后的坐标与其它字段设置。</summary>
    public void NormalizeLayoutOrder(IList<ArasFormFieldLayout> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        for (var index = 0; index < fields.Count; index++)
            fields[index].Sequence = index + 1;
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
        var generatedFieldCount = request.Fields.Count + 1; // 包含末尾自动追加的 HTML 边框字段。

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
                // 第二步（覆盖）：保留 Form 身份，移除全部旧 Body 后整体重建主体并更新尺寸。
                formId = existing.getID();
                ReplaceFormBody(innovator, existing, request, cancellationToken);
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
                    $"{(wasUpdated ? "覆盖" : "创建")}窗体: {request.FormName}，对象类: {request.ItemTypeName}，字段: {generatedFieldCount}")
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
                FieldCount = generatedFieldCount,
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
    /// 只查询同名 Form，不附加 Body 关系筛选，确保没有 Body 的异常旧窗体也能被识别。
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
                new XElement("name", formName)));

        var result = innovator.applyAML(ToAml(aml));

        // 必须先判断 isEmpty。零条结果在 IOM 中可能也是 Error Item（错误码 0）。
        if (result.isEmpty())
            return null;

        // 只有非空的 Error Item 才代表权限、AML、连接或服务器等真实错误。
        ThrowIfError(result, "检查同名窗体失败");
        return result.getItemCount() > 0 ? result.getItemByIndex(0) : null;
    }

    /// <summary>
    /// 覆盖现有 Form 的主体：先删除该 Form 下全部旧 Body，再创建新的 Body 与 Field。
    /// 不复用旧 Body，也不逐个删除 Field，避免旧主体关系残留和大量 AML 节点带来的性能损耗。
    /// </summary>
    private static void ReplaceFormBody(
        Innovator innovator,
        Item existingForm,
        ArasFormConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var bodyIds = GetFormBodyIds(innovator, existingForm.getID());
        var replaceAml = new XElement("AML");

        // 异常数据中可能存在多个 Body；覆盖时全部移除，确保最终只保留一个新主体。
        foreach (var bodyId in bodyIds)
        {
            replaceAml.Add(new XElement("Item",
                new XAttribute("type", "Body"),
                new XAttribute("action", "delete"),
                new XAttribute("id", bodyId)));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 删除与重建放在同一次请求中，减少网络往返和无 Body 中间状态的暴露时间。
        replaceAml.Add(
            BuildFormDimensionsItem(request, existingForm.getID(),
                new XElement("Relationships", BuildBodyItem(request.Fields))));

        var replaceResult = innovator.applyAML(ToAml(replaceAml));
        ThrowIfError(replaceResult, "覆盖窗体主体失败");
    }

    /// <summary>
    /// 按 source_id 查询 Form 下全部 Body，只返回覆盖流程实际需要的 ID。
    /// 零条 Body 属于可修复的异常数据，返回空集合后仍会为 Form 创建新主体。
    /// </summary>
    private static IReadOnlyList<string> GetFormBodyIds(Innovator innovator, string formId)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "Body"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("source_id", formId)));

        var result = innovator.applyAML(ToAml(aml));
        if (result.isEmpty())
            return [];

        ThrowIfError(result, "读取窗体主体失败");
        var bodyIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < result.getItemCount(); index++)
        {
            var bodyId = result.getItemByIndex(index).getID();
            if (!string.IsNullOrWhiteSpace(bodyId))
                bodyIds.Add(bodyId);
        }

        return bodyIds.ToList();
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
                new XElement("width", CalculateFormWidth(request.Fields)),
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
            new XElement("width", CalculateFormWidth(request.Fields)),
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
    /// 将布局模型转换为 Aras Field 关系项，并在末尾追加一个 HTML 边框字段。
    /// propertytype_id 保持 Field 与原始 Property 的绑定关系。
    /// </summary>
    private static IEnumerable<XElement> BuildFieldItems(
        IReadOnlyList<ArasFormFieldLayout> fields)
    {
        foreach (var field in fields)
        {
            var fieldItem = new XElement("Item",
                    new XAttribute("type", "Field"),
                    new XAttribute("action", "add"),
                    new XElement("name", field.Name),
                    new XElement("label", field.Label),
                    new XElement("field_type", field.FieldType),
                    new XElement("propertytype_id", field.PropertyId),
                    new XElement("display_length_unit", "px"),
                    new XElement("is_visible", "1"),
                    new XElement("is_disabled", field.IsDisabled ? "1" : "0"),
                    new XElement("font_weight", "bold"),
                    new XElement("font_color", field.FontColor),
                    new XElement("label_position", "top"),
                    new XElement("font_family", "arial, helvetica, sans-serif"),
                    new XElement("font_size", "8pt"),
                    new XElement("x", field.X),
                    new XElement("y", field.Y),
                    new XElement("display_length", field.DisplayLength));

            if (field.IsTextAreaField)
            {
                fieldItem.Add(
                    new XElement("textarea_rows", field.TextAreaRows),
                    new XElement("textarea_cols", field.TextAreaColumns));
            }

            yield return fieldItem;
        }

        // yield return 位于普通字段循环之后，保证新建和覆盖时边框始终是最后一个 Field。
        yield return BuildBorderFieldItem(fields);
    }

    /// <summary>
    /// 构造不绑定 Property 的 HTML 边框字段。X/Y 固定为 10/20，
    /// HTML 高度使用当前 Form 计算高度加 50，宽度固定为 830px。
    /// </summary>
    private static XElement BuildBorderFieldItem(IReadOnlyList<ArasFormFieldLayout> fields)
    {
        var borderHeight = CalculateFormHeight(fields) + BorderHeightOffset;
        var htmlCode = FormattableString.Invariant(
            $"<div style=\"height:{borderHeight}px;width:830px;border:1px solid\"> </div>");

        return new XElement("Item",
            new XAttribute("type", "Field"),
            new XAttribute("action", "add"),
            new XElement("name", "width_HTML"),
            new XElement("label", "表单基础信息"),
            new XElement("field_type", "html"),
            new XElement("display_length_unit", "px"),
            new XElement("is_visible", "1"),
            new XElement("font_weight", "bold"),
            new XElement("label_position", "top"),
            new XElement("font_family", "arial, helvetica, sans-serif"),
            new XElement("font_size", "8pt"),
            new XElement("x", BorderFieldX),
            new XElement("y", BorderFieldY),
            new XElement("z_index", -1),
            new XElement("html_code", htmlCode),
            new XElement("display_length", BorderHtmlWidth));
    }

    /// <summary>根据用户编辑后的 X 与显示长度计算窗体宽度，并保留右侧边距。</summary>
    private static int CalculateFormWidth(IReadOnlyList<ArasFormFieldLayout> fields) =>
        fields.Count == 0
            ? StartX + (ColumnsPerRow - 1) * HorizontalSpacing + TextDisplayLength + 50
            : fields.Max(field => field.X + field.DisplayLength) + 50;

    /// <summary>按最后一行 Y 坐标计算窗体高度，并保留底部操作空间。</summary>
    private static int CalculateFormHeight(IReadOnlyList<ArasFormFieldLayout> fields) =>
        fields.Count == 0 ? 200 : fields.Max(field => field.Y) + 100;

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
        if (request.Fields.Any(field => !ArasFormConfigurationOptions.IsKnownFieldType(field.FieldType)))
            throw new InvalidOperationException("存在不受支持的控件类型，无法生成窗体。");
        if (request.Fields.Any(field => !ArasFormConfigurationOptions.IsKnownFontColor(field.FontColor)))
            throw new InvalidOperationException("存在不受支持的标题颜色，无法生成窗体。");
        if (request.Fields.Any(field => field.X < 0 || field.Y < 0 || field.DisplayLength <= 0))
            throw new InvalidOperationException("字段的 X、Y 必须大于等于 0，显示长度必须大于 0。");
        if (request.Fields.Any(field => field.IsTextAreaField &&
                                        (field.TextAreaRows <= 0 || field.TextAreaColumns <= 0)))
            throw new InvalidOperationException("Text Area 控件的行数和列数必须大于 0。");
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
