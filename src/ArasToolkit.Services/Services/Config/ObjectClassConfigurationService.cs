using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Globalization;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>对象类权限页签、可创建者与标准四状态生命周期的一键设定。</summary>
public sealed class ObjectClassConfigurationService : IObjectClassConfigurationService
{
    private const string SettingsRelativePath = "Config/AppSettings/objectClassConfiguration.json";
    private const string LabelLanguages = MultilingualAml.Languages;
    private const string LifecycleStatePermissionProperty = "state_permission_id";
    private static readonly XNamespace I18n = MultilingualAml.Namespace;

    private static readonly LifecycleStateDefinition[] LifecycleStates =
    [
        new("Preliminary", "新建立", "新建立", 100, 120, false, false),
        new("In Review", "审核中", "審核中", 320, 120, false, false),
        new("Released", "已发行", "已發行", 540, 120, true, true),
        new("Cancel", "取消", "取消", 320, 300, false, true)
    ];

    private static readonly (string From, string To)[] LifecycleTransitions =
    [
        ("Preliminary", "In Review"),
        ("In Review", "Preliminary"),
        ("In Review", "Released"),
        ("In Review", "Cancel"),
        ("Preliminary", "Cancel")
    ];

    private readonly ArasConnectionService _connectionService;
    private readonly IOperationLogService _operationLogService;
    private readonly IErrorLogService _errorLogService;

    public ObjectClassConfigurationService(
        ArasConnectionService connectionService,
        IOperationLogService operationLogService,
        IErrorLogService errorLogService)
    {
        _connectionService = connectionService;
        _operationLogService = operationLogService;
        _errorLogService = errorLogService;
    }

    public async Task<List<ObjectClassConfigurationItem>> QueryItemTypesAsync(
        string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var innovator = GetInnovator();
            var aml = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "ItemType"),
                    new XAttribute("action", "get"),
                    new XAttribute("language", LabelLanguages),
                    new XAttribute("select", "id,name,label,default_permission")));
            var result = Apply(innovator, aml, "读取对象类失败");
            cancellationToken.ThrowIfCancellationRequested();

            var terms = (keyword ?? string.Empty).Split(
                [' ', '\t', '\r', '\n'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var items = new List<ObjectClassConfigurationItem>();
            for (var index = 0; index < result.getItemCount(); index++)
            {
                var item = result.getItemByIndex(index);
                var defaultPermissionId = item.getProperty("default_permission", string.Empty);
                var defaultPermissionName = item.getPropertyAttribute(
                    "default_permission", "keyed_name", string.Empty);
                if (!string.IsNullOrWhiteSpace(defaultPermissionId) &&
                    string.IsNullOrWhiteSpace(defaultPermissionName))
                {
                    defaultPermissionName = "已设定";
                }

                var candidate = new ObjectClassConfigurationItem
                {
                    Id = item.getID(),
                    Name = item.getProperty("name", string.Empty),
                    LabelEn = item.getProperty("label", string.Empty, "en"),
                    LabelZc = item.getProperty("label", string.Empty, "zc"),
                    LabelZt = item.getProperty("label", string.Empty, "zt"),
                    DefaultPermissionName = defaultPermissionName
                };

                if (string.IsNullOrWhiteSpace(candidate.Id) || string.IsNullOrWhiteSpace(candidate.Name))
                    continue;
                if (terms.Length > 0 && !terms.All(term => Matches(candidate, term)))
                    continue;

                items.Add(candidate);
            }

            return items
                .OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类配置-查询", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<ObjectClassConfigurationSettings> LoadSettingsAsync()
    {
        var path = GetSettingsPath();
        if (!File.Exists(path))
            return ObjectClassConfigurationSettings.CreateDefault();

        try
        {
            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);
            var settings = JsonSerializer.Deserialize<ObjectClassConfigurationSettings>(json);
            return NormalizeSettings(settings ?? ObjectClassConfigurationSettings.CreateDefault());
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类配置-读取角色模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            return ObjectClassConfigurationSettings.CreateDefault();
        }
    }

    public async Task SaveSettingsAsync(ObjectClassConfigurationSettings settings)
    {
        try
        {
            var normalized = NormalizeSettings(settings);
            var path = GetSettingsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var json = JsonSerializer.Serialize(normalized, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(path, json, new UTF8Encoding(false));
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类配置-保存角色模板", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    public async Task<ObjectClassConfigurationBatchResult> ConfigureAsync(
        IReadOnlyList<ObjectClassConfigurationItem> itemTypes,
        ObjectClassConfigurationOptions options,
        ObjectClassConfigurationSettings settings,
        IProgress<ObjectClassConfigurationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (itemTypes.Count == 0)
            throw new InvalidOperationException("至少需要选择一个对象类。");
        if (!options.HasAnySelection)
            throw new InvalidOperationException("至少需要勾选一项基础设定。");

        try
        {
            var normalized = NormalizeSettings(settings);
            await SaveSettingsAsync(normalized);
            var innovator = GetInnovator();
            var identityNames = GetRequiredIdentityNames(options, normalized);
            var identityIds = ResolveIdentityIds(innovator, identityNames);
            var batchResult = new ObjectClassConfigurationBatchResult { TotalCount = itemTypes.Count };

            for (var index = 0; index < itemTypes.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var itemType = itemTypes[index];
                progress?.Report(new ObjectClassConfigurationProgress
                {
                    Current = index,
                    Total = itemTypes.Count,
                    ItemTypeName = itemType.Name,
                    Phase = "正在设定"
                });

                try
                {
                    var aml = new XElement("AML",
                        new XAttribute(XNamespace.Xmlns + "i18n", I18n));
                    var messages = new List<string>();
                    if (options.ConfigureDefaultPermission)
                    {
                        messages.Add(AppendDefaultPermission(
                            innovator, aml, itemType, normalized, identityIds));
                    }

                    if (options.ConfigureCanAdd)
                    {
                        messages.Add(AppendCanAdd(
                            innovator, aml, itemType, normalized, identityIds));
                    }

                    if (options.ConfigureLifecycle)
                    {
                        messages.Add(AppendLifecycle(
                            innovator, aml, itemType, normalized, identityIds));
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    if (aml.Elements("Item").Any())
                    {
                        // Aras 将同一 <AML> 中的所有写入作为一次服务端事务；
                        // 任一 Item 失败时，Permission/Access/生命周期/挂载关系整体回滚。
                        Apply(innovator, aml,
                            $"配置 {itemType.Name} 的权限、可创建者与生命周期事务失败");
                    }

                    itemType.OperationSummary = string.Join("；", messages);
                    batchResult.SuccessCount++;
                    await TryLogOperationAsync(itemType, messages);
                }
                catch (Exception ex)
                {
                    itemType.OperationSummary = $"失败：{ex.Message}";
                    batchResult.FailedCount++;
                    batchResult.FailedDetails.Add($"{itemType.Name}: {ex.Message}");
                    await _errorLogService.LogErrorAsync(
                        $"对象类配置-{itemType.Name}", ex.Message,
                        ErrorLog.LevelP1, ex.StackTrace);
                }

                progress?.Report(new ObjectClassConfigurationProgress
                {
                    Current = index + 1,
                    Total = itemTypes.Count,
                    ItemTypeName = itemType.Name,
                    Phase = "基础设定"
                });
            }

            return batchResult;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类配置-执行", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
            throw;
        }
    }

    private string AppendDefaultPermission(
        Innovator innovator,
        XElement aml,
        ObjectClassConfigurationItem itemType,
        ObjectClassConfigurationSettings settings,
        IReadOnlyDictionary<string, string> identityIds)
    {
        var permissionId = AppendPermissionConfiguration(
            innovator,
            aml,
            itemType.Name,
            BuildPermissionRules(settings, false),
            identityIds);
        var allowedPermissionId = FindRelationshipId(
            innovator, "Allowed Permission", itemType.Id, permissionId);
        if (allowedPermissionId != null)
        {
            aml.Add(new XElement("Item",
                new XAttribute("type", "Allowed Permission"),
                new XAttribute("action", "edit"),
                new XAttribute("id", allowedPermissionId),
                new XElement("is_default", "1")));
            return $"权限页签已包含 {itemType.Name}，已更新详细权限并标记为默认";
        }

        aml.Add(new XElement("Item",
            new XAttribute("type", "Allowed Permission"),
            new XAttribute("action", "add"),
            new XAttribute("id", innovator.getNewID()),
            new XElement("source_id", itemType.Id),
            new XElement("is_default", "1"),
            new XElement("related_id", permissionId)));
        return $"已将 {itemType.Name} 添加到权限页签并标记为默认";
    }

    private string AppendCanAdd(
        Innovator innovator,
        XElement aml,
        ObjectClassConfigurationItem itemType,
        ObjectClassConfigurationSettings settings,
        IReadOnlyDictionary<string, string> identityIds)
    {
        var roles = ParseRoles(settings.CanAddRoles);
        var added = 0;
        foreach (var role in roles)
        {
            var identityId = identityIds[role];
            if (FindRelationshipId(innovator, "Can Add", itemType.Id, identityId) != null)
                continue;

            aml.Add(new XElement("Item",
                new XAttribute("type", "Can Add"),
                new XAttribute("action", "add"),
                new XAttribute("id", innovator.getNewID()),
                new XElement("source_id", itemType.Id),
                new XElement("related_id", identityId)));
            added++;
        }

        return added == 0
            ? "可创建者均已存在，跳过"
            : $"已添加 {added} 个可创建者";
    }

    private string AppendLifecycle(
        Innovator innovator,
        XElement aml,
        ObjectClassConfigurationItem itemType,
        ObjectClassConfigurationSettings settings,
        IReadOnlyDictionary<string, string> identityIds)
    {
        var mapId = FindItemIdByName(innovator, "Life Cycle Map", itemType.Name);
        var statePermissionIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var state in LifecycleStates)
        {
            var isReadOnlyState = !string.Equals(
                state.Name, "Preliminary", StringComparison.OrdinalIgnoreCase);
            var permissionName = $"{itemType.Name} - {state.Name}";
            statePermissionIds[state.Name] = AppendPermissionConfiguration(
                innovator,
                aml,
                permissionName,
                BuildPermissionRules(settings, isReadOnlyState),
                identityIds);
        }

        if (mapId != null)
        {
            var existingStates = LifecycleStates.ToDictionary(
                state => state.Name,
                state => FindLifecycleState(innovator, mapId, state.Name)
                         ?? throw new InvalidOperationException(
                             $"生命周期 {itemType.Name} 缺少状态 {state.Name}，无法安全补挂状态权限"),
                StringComparer.OrdinalIgnoreCase);
            AppendLifecycleStatePermissionLinks(aml,
                existingStates.ToDictionary(pair => pair.Key, pair => pair.Value.getID()), statePermissionIds);
            AppendReturnTransitionRepair(innovator, aml, mapId, existingStates);
            return $"已补齐同名生命周期 {itemType.Name} 的状态权限及简/繁/英标签，并检查退回路径";
        }

        mapId = innovator.getNewID();
        var stateIds = LifecycleStates.ToDictionary(
            state => state.Name,
            _ => innovator.getNewID(),
            StringComparer.OrdinalIgnoreCase);
        var transitionRoleId = identityIds[ParseRoles(settings.TransitionRole).Single()];
        AppendLifecycleItems(
            aml, itemType, mapId, stateIds, statePermissionIds, transitionRoleId);
        return "已创建四状态生命周期、四套状态权限与五条转换";
    }

    private static void AppendLifecycleItems(
        XElement aml,
        ObjectClassConfigurationItem itemType,
        string mapId,
        IReadOnlyDictionary<string, string> stateIds,
        IReadOnlyDictionary<string, string> statePermissionIds,
        string transitionRoleId)
    {
        aml.Add(new XElement("Item",
            new XAttribute("type", "Life Cycle Map"),
            new XAttribute("action", "add"),
            new XAttribute("id", mapId),
            new XElement("name", itemType.Name),
            new XElement("description", $"{itemType.Name} 标准四状态生命周期")));

        for (var index = 0; index < LifecycleStates.Length; index++)
        {
            var state = LifecycleStates[index];
            aml.Add(new XElement("Item",
                new XAttribute("type", "Life Cycle State"),
                new XAttribute("action", "add"),
                new XAttribute("id", stateIds[state.Name]),
                new XElement("source_id", mapId),
                new XElement("name", state.Name),
                MultilingualAml.Values("label", state.Name, state.LabelZc, state.LabelZt),
                new XElement("x", state.X),
                new XElement("y", state.Y),
                new XElement("released", state.IsReleased ? "1" : "0"),
                new XElement("not_lockable", state.IsNotLockable ? "1" : "0"),
                new XElement("image", "../images/LifeCycleState.svg"),
                new XElement(LifecycleStatePermissionProperty, statePermissionIds[state.Name])));
        }

        aml.Add(new XElement("Item",
            new XAttribute("type", "Life Cycle Map"),
            new XAttribute("action", "edit"),
            new XAttribute("id", mapId),
            new XElement("start_state", stateIds["Preliminary"])));

        foreach (var (from, to) in LifecycleTransitions)
        {
            var transition = new XElement("Item",
                new XAttribute("type", "Life Cycle Transition"),
                new XAttribute("action", "add"),
                new XElement("source_id", mapId),
                new XElement("from_state", stateIds[from]),
                new XElement("to_state", stateIds[to]),
                new XElement("role", transitionRoleId));
            if (from == "In Review" && to == "Preliminary")
            {
                var fromState = LifecycleStates.Single(state => state.Name == from);
                var toState = LifecycleStates.Single(state => state.Name == to);
                transition.Add(new XElement("segments",
                    BuildReturnSegments(fromState.X, fromState.Y, toState.X, toState.Y)));
            }
            aml.Add(transition);
        }

        aml.Add(new XElement("Item",
            new XAttribute("type", "ItemType Life Cycle"),
            new XAttribute("action", "add"),
            new XElement("source_id", itemType.Id),
            new XElement("related_id", mapId)));
    }

    private static void AppendLifecycleStatePermissionLinks(
        XElement aml,
        IReadOnlyDictionary<string, string> stateIds,
        IReadOnlyDictionary<string, string> statePermissionIds)
    {
        foreach (var state in LifecycleStates)
        {
            aml.Add(new XElement("Item",
                new XAttribute("type", "Life Cycle State"),
                new XAttribute("action", "edit"),
                new XAttribute("id", stateIds[state.Name]),
                MultilingualAml.Values("label", state.Name, state.LabelZc, state.LabelZt),
                new XElement(LifecycleStatePermissionProperty, statePermissionIds[state.Name])));
        }
    }

    private static string BuildReturnSegments(int fromX, int fromY, int toX, int toY)
    {
        // R37 segments 为绝对坐标 x,y|x,y；默认退回线从上方绕行，避开正向线。
        // 已有节点若被纵向排列，改从左侧绕行，避免转折点落到同一根竖线上。
        if (fromX == toX)
        {
            var routeX = fromX >= 80 ? fromX - 80 : fromX + 80;
            return FormattableString.Invariant($"{routeX},{fromY}|{routeX},{toY}");
        }
        var topY = Math.Min(fromY, toY);
        var routeY = topY >= 80 ? topY - 80 : Math.Max(fromY, toY) + 80;
        return FormattableString.Invariant($"{fromX},{routeY}|{toX},{routeY}");
    }

    private static void AppendReturnTransitionRepair(
        Innovator innovator, XElement aml, string mapId, IReadOnlyDictionary<string, Item> states)
    {
        var from = states["In Review"];
        var to = states["Preliminary"];
        var query = new XElement("AML", new XElement("Item",
            new XAttribute("type", "Life Cycle Transition"), new XAttribute("action", "get"),
            new XAttribute("select", "id,segments"),
            new XElement("source_id", mapId), new XElement("from_state", from.getID()),
            new XElement("to_state", to.getID())));
        var result = innovator.applyAML(query.ToString(SaveOptions.DisableFormatting));
        if (result.isError() && result.getErrorCode() != "0")
            throw new InvalidOperationException($"读取生命周期退回路径失败：{result.getErrorString()}");
        for (var index = 0; index < result.getItemCount(); index++)
        {
            var transition = result.getItemByIndex(index);
            // 已经调整过的路径保留；仅修复早期版本生成的空转折点。
            if (!string.IsNullOrWhiteSpace(transition.getProperty("segments", string.Empty))) continue;
            if (!int.TryParse(from.getProperty("x"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var fromX) ||
                !int.TryParse(from.getProperty("y"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var fromY) ||
                !int.TryParse(to.getProperty("x"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var toX) ||
                !int.TryParse(to.getProperty("y"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var toY))
                throw new InvalidOperationException("生命周期状态缺少有效坐标，无法修复退回路径。");
            aml.Add(new XElement("Item", new XAttribute("type", "Life Cycle Transition"),
                new XAttribute("action", "edit"), new XAttribute("id", transition.getID()),
                new XElement("segments", BuildReturnSegments(fromX, fromY, toX, toY))));
        }
    }

    private string AppendPermissionConfiguration(
        Innovator innovator,
        XElement aml,
        string permissionName,
        IReadOnlyDictionary<string, bool> roleRules,
        IReadOnlyDictionary<string, string> identityIds)
    {
        var existingPermissionId = FindItemIdByName(innovator, "Permission", permissionName);
        var isNewPermission = existingPermissionId == null;
        var permissionId = existingPermissionId ?? innovator.getNewID();
        if (isNewPermission)
        {
            aml.Add(new XElement("Item",
                new XAttribute("type", "Permission"),
                new XAttribute("action", "add"),
                new XAttribute("id", permissionId),
                new XElement("name", permissionName)));
        }

        foreach (var (role, fullControl) in roleRules)
        {
            var identityId = identityIds[role];
            var accessId = isNewPermission
                ? null
                : FindRelationshipId(innovator, "Access", permissionId, identityId);
            aml.Add(BuildAccessItem(
                innovator, permissionId, identityId, accessId, fullControl));
        }

        return permissionId;
    }

    private static XElement BuildAccessItem(
        Innovator innovator,
        string permissionId,
        string identityId,
        string? accessId,
        bool fullControl)
    {
        return new XElement("Item",
            new XAttribute("type", "Access"),
            new XAttribute("action", accessId == null ? "add" : "edit"),
            new XAttribute("id", accessId ?? innovator.getNewID()),
            accessId == null ? new XElement("source_id", permissionId) : null,
            accessId == null ? new XElement("related_id", identityId) : null,
            new XElement("can_discover", "1"),
            new XElement("can_get", "1"),
            new XElement("can_update", fullControl ? "1" : "0"),
            new XElement("can_delete", fullControl ? "1" : "0"));
    }

    private static Dictionary<string, bool> BuildPermissionRules(
        ObjectClassConfigurationSettings settings,
        bool readOnlyState)
    {
        var rules = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in ParseRoles(settings.CreatorFullControlRoles))
            rules[role] = !readOnlyState;
        foreach (var role in ParseRoles(settings.ReadOnlyRoles))
            rules.TryAdd(role, false);
        foreach (var role in ParseRoles(settings.AdministratorFullControlRoles))
            rules[role] = true;
        return rules;
    }

    private static IReadOnlyDictionary<string, string> ResolveIdentityIds(
        Innovator innovator,
        IReadOnlyCollection<string> identityNames)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var missing = new List<string>();
        foreach (var name in identityNames)
        {
            var id = FindItemIdByName(innovator, "Identity", name);
            if (id == null && string.Equals(name, "Administrator", StringComparison.OrdinalIgnoreCase))
                id = FindItemIdByName(innovator, "Identity", "Administrators");
            if (id == null)
                missing.Add(name);
            else
                result[name] = id;
        }

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Aras 中未找到以下 Identity：{string.Join("、", missing)}。请在角色模板中改为实际名称后重试。");
        }

        return result;
    }

    private static HashSet<string> GetRequiredIdentityNames(
        ObjectClassConfigurationOptions options,
        ObjectClassConfigurationSettings settings)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (options.ConfigureDefaultPermission || options.ConfigureLifecycle)
        {
            names.UnionWith(ParseRoles(settings.CreatorFullControlRoles));
            names.UnionWith(ParseRoles(settings.ReadOnlyRoles));
            names.UnionWith(ParseRoles(settings.AdministratorFullControlRoles));
        }
        if (options.ConfigureCanAdd)
            names.UnionWith(ParseRoles(settings.CanAddRoles));
        if (options.ConfigureLifecycle)
            names.UnionWith(ParseRoles(settings.TransitionRole));
        return names;
    }

    private static List<string> ParseRoles(string value)
        => value.Split(
                [',', '，', ';', '；', '\r', '\n'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private static ObjectClassConfigurationSettings NormalizeSettings(
        ObjectClassConfigurationSettings settings)
    {
        var normalized = new ObjectClassConfigurationSettings
        {
            CreatorFullControlRoles = NormalizeRoles(settings.CreatorFullControlRoles, "Creator"),
            ReadOnlyRoles = NormalizeRoles(settings.ReadOnlyRoles, "World,Owner,Manager"),
            AdministratorFullControlRoles = NormalizeRoles(
                settings.AdministratorFullControlRoles,
                "Aras PLM,Administrator,Innovator Admin"),
            CanAddRoles = NormalizeRoles(settings.CanAddRoles, "World"),
            TransitionRole = NormalizeRoles(settings.TransitionRole, "Aras PLM")
        };

        var transitionRoles = ParseRoles(normalized.TransitionRole);
        if (transitionRoles.Count != 1)
            throw new InvalidOperationException("生命周期转换角色必须且只能填写一个 Identity。");
        return normalized;
    }

    private static string NormalizeRoles(string value, string defaultValue)
    {
        var roles = ParseRoles(value);
        return roles.Count == 0 ? defaultValue : string.Join(',', roles);
    }

    private static string? FindItemIdByName(Innovator innovator, string itemType, string name)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", itemType),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("name", name)));
        return FindFirstItemIdOrNull(innovator, aml);
    }

    private static string? FindRelationshipId(
        Innovator innovator,
        string relationshipType,
        string sourceId,
        string relatedId)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", relationshipType),
                new XAttribute("action", "get"),
                new XAttribute("select", "id"),
                new XElement("source_id", sourceId),
                new XElement("related_id", relatedId)));
        return FindFirstItemIdOrNull(innovator, aml);
    }

    private static Item? FindLifecycleState(
        Innovator innovator,
        string mapId,
        string stateName)
    {
        var aml = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "Life Cycle State"),
                new XAttribute("action", "get"),
                new XAttribute("select", "id,x,y"),
                new XElement("source_id", mapId),
                new XElement("name", stateName)));
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        if (result.isError() && result.getErrorCode() != "0")
            throw new InvalidOperationException($"读取生命周期状态 {stateName} 失败：{result.getErrorString()}");
        return result.getItemCount() == 1 ? result.getItemByIndex(0) : null;
    }

    /// <summary>
    /// Aras 在 get 无匹配项时可能返回 Error Item。存在性预检将这种结果统一视为“不存在”，
    /// 不让正常的新增分支在查询阶段中断。后续写入失败仍由 Apply 抛出并回滚整个 AML。
    /// </summary>
    private static string? FindFirstItemIdOrNull(Innovator innovator, XElement aml)
    {
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        if (result.isError() || result.getItemCount() <= 0)
            return null;

        var id = result.getItemByIndex(0).getID();
        return string.IsNullOrWhiteSpace(id) ? null : id;
    }

    private static Item Apply(Innovator innovator, XElement aml, string context)
    {
        var result = innovator.applyAML(aml.ToString(SaveOptions.DisableFormatting));
        if (result.isError())
            throw new InvalidOperationException($"{context}: {result.getErrorString()}");
        return result;
    }

    private Innovator GetInnovator()
        => _connectionService.TypedInnovator
           ?? throw new InvalidOperationException("尚未连接 Aras，请先连接当前用户的默认 Aras 配置。");

    private async Task TryLogOperationAsync(
        ObjectClassConfigurationItem itemType,
        IReadOnlyCollection<string> messages)
    {
        try
        {
            await _operationLogService.LogAsync(
                "Update",
                "ItemType",
                itemType.Id,
                $"对象类基础设定 {itemType.Name}: {string.Join("；", messages)}");
        }
        catch (Exception ex)
        {
            await _errorLogService.LogErrorAsync("对象类配置-操作日志", ex.Message,
                ErrorLog.LevelP1, ex.StackTrace);
        }
    }

    private static bool Matches(ObjectClassConfigurationItem item, string term)
        => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
           item.LabelEn.Contains(term, StringComparison.OrdinalIgnoreCase) ||
           item.LabelZc.Contains(term, StringComparison.OrdinalIgnoreCase) ||
           item.LabelZt.Contains(term, StringComparison.OrdinalIgnoreCase);

    private static string GetSettingsPath()
        => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsRelativePath);

    private sealed record LifecycleStateDefinition(
        string Name,
        string LabelZc,
        string LabelZt,
        int X,
        int Y,
        bool IsReleased,
        bool IsNotLockable);
}
