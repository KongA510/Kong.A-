using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.Services.Services;

/// <summary>对象类默认权限、可创建者与标准四状态生命周期的一键设定。</summary>
public sealed class ObjectClassConfigurationService : IObjectClassConfigurationService
{
    private const string SettingsRelativePath = "Config/AppSettings/objectClassConfiguration.json";
    private const string LabelLanguages = "en,zc,zt";
    private static readonly XNamespace I18n = "http://www.aras.com/I18N/";

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
                    var messages = new List<string>();
                    if (options.ConfigureDefaultPermission)
                    {
                        messages.Add(EnsureDefaultPermission(
                            innovator, itemType, normalized, identityIds));
                    }

                    if (options.ConfigureCanAdd)
                    {
                        messages.Add(EnsureCanAdd(
                            innovator, itemType, normalized, identityIds));
                    }

                    if (options.ConfigureLifecycle)
                    {
                        messages.Add(EnsureLifecycle(
                            innovator, itemType, normalized, identityIds));
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

    private string EnsureDefaultPermission(
        Innovator innovator,
        ObjectClassConfigurationItem itemType,
        ObjectClassConfigurationSettings settings,
        IReadOnlyDictionary<string, string> identityIds)
    {
        if (itemType.HasDefaultPermission)
            return $"默认权限已存在({itemType.DefaultPermissionName})，跳过";

        var permissionId = EnsurePermission(
            innovator,
            itemType.Name,
            BuildPermissionRules(settings, false),
            identityIds);
        var edit = new XElement("AML",
            new XElement("Item",
                new XAttribute("type", "ItemType"),
                new XAttribute("action", "edit"),
                new XAttribute("id", itemType.Id),
                new XElement("default_permission", permissionId)));
        Apply(innovator, edit, $"设定 {itemType.Name} 默认权限失败");
        itemType.DefaultPermissionName = itemType.Name;
        return $"默认权限已设为 {itemType.Name}";
    }

    private string EnsureCanAdd(
        Innovator innovator,
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

            var add = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "Can Add"),
                    new XAttribute("action", "add"),
                    new XElement("source_id", itemType.Id),
                    new XElement("related_id", identityId)));
            Apply(innovator, add, $"为 {itemType.Name} 添加可创建者 {role} 失败");
            added++;
        }

        return added == 0
            ? "可创建者均已存在，跳过"
            : $"已添加 {added} 个可创建者";
    }

    private string EnsureLifecycle(
        Innovator innovator,
        ObjectClassConfigurationItem itemType,
        ObjectClassConfigurationSettings settings,
        IReadOnlyDictionary<string, string> identityIds)
    {
        if (FindItemIdByName(innovator, "Life Cycle Map", itemType.Name) != null)
            return $"同名生命周期 {itemType.Name} 已存在，跳过";

        var statePermissionIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var state in LifecycleStates)
        {
            var isReadOnlyState = !string.Equals(
                state.Name, "Preliminary", StringComparison.OrdinalIgnoreCase);
            var permissionName = $"{itemType.Name} - {state.Name}";
            statePermissionIds[state.Name] = EnsurePermission(
                innovator,
                permissionName,
                BuildPermissionRules(settings, isReadOnlyState),
                identityIds);
        }

        var mapId = innovator.getNewID();
        var stateIds = LifecycleStates.ToDictionary(
            state => state.Name,
            _ => innovator.getNewID(),
            StringComparer.OrdinalIgnoreCase);
        var transitionRoleId = identityIds[ParseRoles(settings.TransitionRole).Single()];
        var aml = BuildLifecycleAml(
            itemType, mapId, stateIds, statePermissionIds, transitionRoleId);
        Apply(innovator, aml, $"创建并挂载 {itemType.Name} 生命周期失败");
        return "已创建四状态生命周期、四套状态权限与五条转换";
    }

    private static XElement BuildLifecycleAml(
        ObjectClassConfigurationItem itemType,
        string mapId,
        IReadOnlyDictionary<string, string> stateIds,
        IReadOnlyDictionary<string, string> statePermissionIds,
        string transitionRoleId)
    {
        var aml = new XElement("AML",
            new XAttribute(XNamespace.Xmlns + "i18n", I18n),
            new XElement("Item",
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
                new XElement("label", new XAttribute(XNamespace.Xml + "lang", "en"), state.Name),
                new XElement(I18n + "label", new XAttribute(XNamespace.Xml + "lang", "zc"), state.LabelZc),
                new XElement(I18n + "label", new XAttribute(XNamespace.Xml + "lang", "zt"), state.LabelZt),
                new XElement("x", state.X),
                new XElement("y", state.Y),
                new XElement("released", state.IsReleased ? "1" : "0"),
                new XElement("not_lockable", state.IsNotLockable ? "1" : "0"),
                new XElement("image", "../images/LifeCycleState.svg"),
                new XElement("permission_id", statePermissionIds[state.Name])));
        }

        aml.Add(new XElement("Item",
            new XAttribute("type", "Life Cycle Map"),
            new XAttribute("action", "edit"),
            new XAttribute("id", mapId),
            new XElement("start_state", stateIds["Preliminary"])));

        foreach (var (from, to) in LifecycleTransitions)
        {
            aml.Add(new XElement("Item",
                new XAttribute("type", "Life Cycle Transition"),
                new XAttribute("action", "add"),
                new XElement("source_id", mapId),
                new XElement("from_state", stateIds[from]),
                new XElement("to_state", stateIds[to]),
                new XElement("role", transitionRoleId)));
        }

        aml.Add(new XElement("Item",
            new XAttribute("type", "ItemType Life Cycle"),
            new XAttribute("action", "add"),
            new XElement("source_id", itemType.Id),
            new XElement("related_id", mapId)));
        return aml;
    }

    private string EnsurePermission(
        Innovator innovator,
        string permissionName,
        IReadOnlyDictionary<string, bool> roleRules,
        IReadOnlyDictionary<string, string> identityIds)
    {
        var permissionId = FindItemIdByName(innovator, "Permission", permissionName);
        if (permissionId == null)
        {
            var add = new XElement("AML",
                new XElement("Item",
                    new XAttribute("type", "Permission"),
                    new XAttribute("action", "add"),
                    new XElement("name", permissionName)));
            var added = Apply(innovator, add, $"创建权限 {permissionName} 失败");
            permissionId = added.getID();
            if (string.IsNullOrWhiteSpace(permissionId))
                throw new InvalidOperationException($"创建权限 {permissionName} 后未返回 ID。");
        }

        foreach (var (role, fullControl) in roleRules)
        {
            EnsureAccess(
                innovator,
                permissionId,
                identityIds[role],
                fullControl,
                $"{permissionName}/{role}");
        }

        return permissionId;
    }

    private static void EnsureAccess(
        Innovator innovator,
        string permissionId,
        string identityId,
        bool fullControl,
        string context)
    {
        var accessId = FindRelationshipId(innovator, "Access", permissionId, identityId);
        var access = new XElement("Item",
            new XAttribute("type", "Access"),
            new XAttribute("action", accessId == null ? "add" : "edit"),
            accessId == null ? null : new XAttribute("id", accessId),
            accessId == null ? new XElement("source_id", permissionId) : null,
            accessId == null ? new XElement("related_id", identityId) : null,
            new XElement("can_discover", "1"),
            new XElement("can_get", "1"),
            new XElement("can_update", fullControl ? "1" : "0"),
            new XElement("can_delete", fullControl ? "1" : "0"));
        Apply(innovator, new XElement("AML", access), $"设定权限明细 {context} 失败");
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
        var result = Apply(innovator, aml, $"查询 {itemType} {name} 失败");
        return result.getItemCount() > 0 ? result.getItemByIndex(0).getID() : null;
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
        var result = Apply(innovator, aml, $"查询 {relationshipType} 关系失败");
        return result.getItemCount() > 0 ? result.getItemByIndex(0).getID() : null;
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
