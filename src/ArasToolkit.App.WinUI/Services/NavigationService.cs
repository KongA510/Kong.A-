using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using ArasToolkit.App.WinUI.Views;

namespace ArasToolkit.App.WinUI.Services;

/// <summary>
/// 导航服务 — 维护"菜单名 → 页面类型"路由表，驱动主 Frame 导航。
/// 未注册的菜单名回退到 PlaceholderPage（迁移期间占位）。
/// </summary>
public class NavigationService
{
    private readonly Dictionary<string, Type> _routes = new();
    private Frame? _frame;

    public Frame? Frame => _frame;

    public void SetFrame(Frame frame) => _frame = frame;

    /// <summary>注册菜单名到页面类型的映射。</summary>
    public void Register(string name, Type pageType) => _routes[name] = pageType;

    /// <summary>按菜单名导航；未注册则显示占位页。</summary>
    public bool Navigate(string name)
    {
        if (_frame == null) return false;
        var type = _routes.TryGetValue(name, out var t) ? t : typeof(PlaceholderPage);
        return _frame.Navigate(type, name);
    }
}
