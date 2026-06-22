using System.Collections.ObjectModel;

namespace ArasToolkit.Core.Models;

/// <summary>
/// 侧边导航菜单项模型 — 支持二级菜单层级
/// </summary>
public class MenuItemInfo
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Type? ViewType { get; set; }
    public bool IsPlaceholder { get; set; }

    /// <summary>仪表盘卡片图标（Emoji）</summary>
    public string CardIcon { get; set; } = "📄";

    /// <summary>二级子菜单项</summary>
    public ObservableCollection<MenuItemInfo> Children { get; set; } = new();

    /// <summary>父节点是否展开</summary>
    public bool IsExpanded { get; set; }

    /// <summary>是否有子节点（用于模板判断）</summary>
    public bool HasChildren => Children.Count > 0;
}
