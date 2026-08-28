using System;
using System.Collections.ObjectModel;
using ArasToolkit.Core.Extensions;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;

namespace ArasToolkit.App.WinUI.ViewModels;

/// <summary>
/// 主窗口 ViewModel — 管理导航菜单与全局状态（WinUI 版，菜单结构与 WPF 版完全一致）。
/// </summary>
public class MainViewModel : ObservableObject
{
    private readonly IChangelogService? _changelogService;
    private MenuItemInfo? _selectedMenuItem;
    private bool _isLoggedIn;
    private bool _isLoading;
    private string _versionText = "个人工具箱 v1.0";

    public MenuItemInfo? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>左下角版本号文本，从数据库读取最新版本。</summary>
    public string VersionText
    {
        get => _versionText;
        set => SetProperty(ref _versionText, value);
    }

    public ObservableCollection<MenuItemInfo> MenuItems { get; } = new();

    public MainViewModel(IChangelogService? changelogService = null)
    {
        _changelogService = changelogService;
        InitializeMenuItems();
        RefreshVersion();
    }

    /// <summary>从数据库获取最新版本号并刷新左下角显示。</summary>
    public void RefreshVersion()
    {
        try
        {
            if (_changelogService != null)
            {
                var ver = _changelogService.GetCurrentVersion();
                VersionText = $"个人工具箱 v{ver}";
            }
        }
        catch
        {
            VersionText = "个人工具箱 v1.0";
        }
    }

    private void InitializeMenuItems()
    {
        // ===== 一级菜单：仪表盘 =====
        MenuItems.Add(new MenuItemInfo { Name = "仪表盘", Icon = "Home", CardIcon = "\uE80F", Description = "系统概览仪表盘" });

        // ===== 二级菜单：系统翻译 =====
        var sysTranslation = new MenuItemInfo { Name = "系统翻译", Icon = "Translate", CardIcon = "\uE774", Description = "Aras翻译工具集" };
        sysTranslation.Children.Add(new MenuItemInfo { Name = "文本翻译", Icon = "Translate", CardIcon = "\uE70F", Description = "AI 驱动的 Excel 批量翻译工具" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "翻译历史", Icon = "History", CardIcon = "\uE81C", Description = "历史翻译记录查询" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "字段翻译", Icon = "FormTextbox", CardIcon = "\uE8D2", Description = "按对象类翻译 Aras Property 标签" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "表单翻译", Icon = "FormTextbox", CardIcon = "\uE70F", Description = "按对象类翻译关联 Form 标签" });
        sysTranslation.Children.Add(new MenuItemInfo { Name = "窗体翻译", Icon = "WindowMaximize", CardIcon = "\uE8A7", Description = "翻译 Form Field 标签与说明" });
        MenuItems.Add(sysTranslation);

        // ===== 二级菜单：系统配置 =====
        var sysConfig = new MenuItemInfo { Name = "系统配置", Icon = "Cog", CardIcon = "\uE713", Description = "Aras系统配置工具集" };
        sysConfig.Children.Add(new MenuItemInfo { Name = "窗体配置", Icon = "Cog", CardIcon = "\uE8B9", Description = "按规范生成 Aras 经典窗体字段布局" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "对象类汇入", Icon = "DatabaseImport", CardIcon = "\uE8B5", Description = "通过 Excel 批量创建 Aras 对象类和关系类" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "对象类配置", Icon = "CubeOutline", CardIcon = "\uE7B8", Description = "批量配置权限页签、可创建者和标准生命周期" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "类结构汇入", Icon = "TreeFolderFolder", CardIcon = "\uE8F1", Description = "从 Excel 多级路径全量覆盖 ItemType 类结构" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "List配置", Icon = "FormatListBulleted", CardIcon = "\uE8FD", Description = "批量创建Aras List主档和菜单项" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "属性配置", Icon = "Tune", CardIcon = "🧩", Description = "选择现有对象类，预检 AML 后逐条写入属性" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "生命周期配置", Icon = "Refresh", CardIcon = "\uE895", Description = "批量创建Aras对象类的生命周期状态定义" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "工作流程设定", Icon = "BranchFork", CardIcon = "\uE8F0", Description = "从 Excel 预览、调整并一次性汇入 Aras Workflow Map" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "权限配置", Icon = "ShieldAccount", CardIcon = "\uE72E", Description = "Aras权限配置工具" });
        sysConfig.Children.Add(new MenuItemInfo { Name = "用户管理", Icon = "AccountGroup", CardIcon = "\uE716", Description = "管理员创建用户与角色分配" });
        MenuItems.Add(sysConfig);

        // ===== 二级菜单：数据相关 =====
        var dataRelated = new MenuItemInfo { Name = "数据相关", Icon = "Database", CardIcon = "\uE8B7", Description = "数据库导入、导出与批量修改工具集" };
        dataRelated.Children.Add(new MenuItemInfo { Name = "常用SQLorAML", Icon = "Code", CardIcon = "🧾", Description = "保存、预览并快速复用 SQL、AML 与 XML 片段" });
        dataRelated.Children.Add(new MenuItemInfo { Name = "数据库导出", Icon = "DatabaseExport", CardIcon = "\uE898", Description = "执行只读 SQL 查询并导出为 Excel 文件" });
        dataRelated.Children.Add(new MenuItemInfo { Name = "数据库修改模式", Icon = "Edit", CardIcon = "\uE70F", Description = "解析 Excel 占位符模板并安全批量执行 SQL" });
        dataRelated.Children.Add(new MenuItemInfo { Name = "数据汇入", Icon = "DatabaseImport", CardIcon = "\uE8B5", Description = "Excel 数据导入 Aras 系统" });
        dataRelated.Children.Add(new MenuItemInfo { Name = "汇入项目计划模板", Icon = "Calendar", CardIcon = "📅", Description = "导出标准 Excel，预检并汇入 Aras Project Template 与完整 WBS" });
        MenuItems.Add(dataRelated);

        // ===== 二级菜单：其他功能 =====
        var otherTools = new MenuItemInfo { Name = "其他功能", Icon = "DeveloperTools", CardIcon = "\uEC7A", Description = "XML、JSON 格式处理与 .NET 实体转换工具集" };
        otherTools.Children.Add(new MenuItemInfo { Name = "XML格式化", Icon = "Code", CardIcon = "\uE943", Description = "自动提取并格式化完整 XML 结构" });
        otherTools.Children.Add(new MenuItemInfo { Name = "XML比对", Icon = "Compare", CardIcon = "\uE8AB", Description = "归一化 XML 并以颜色标记缺少与内容差异" });
        otherTools.Children.Add(new MenuItemInfo { Name = "JSON格式化", Icon = "Code", CardIcon = "\uE943", Description = "校验并格式化 JSON 对象或数组" });
        otherTools.Children.Add(new MenuItemInfo { Name = "JSON比对", Icon = "Compare", CardIcon = "\uE8AB", Description = "归一化 JSON 并以颜色标记结构差异" });
        otherTools.Children.Add(new MenuItemInfo { Name = "JSON转实体类", Icon = "Class", CardIcon = "\uE7BA", Description = "按 .NET 规范生成实体类与序列化映射" });
        otherTools.Children.Add(new MenuItemInfo { Name = "实体类转JSON", Icon = "Code", CardIcon = "\uE713", Description = "从 .NET 实体类生成 JSON 结构样例" });
        MenuItems.Add(otherTools);

        // ===== 二级菜单：系统日志 =====
        var sysLog = new MenuItemInfo { Name = "系统日志", Icon = "History", CardIcon = "\uE8FD", Description = "系统日志与审计记录" };
        sysLog.Children.Add(new MenuItemInfo { Name = "更新日志", Icon = "History", CardIcon = "\uE81C", Description = "系统更新日志与版本记录" });
        sysLog.Children.Add(new MenuItemInfo { Name = "错误日志", Icon = "Bug", CardIcon = "\uEBE8", Description = "系统错误记录与排查" });
        sysLog.Children.Add(new MenuItemInfo { Name = "敏感操作日志", Icon = "ShieldAccount", CardIcon = "\uE72E", Description = "业务操作审计轨迹记录" });
        sysLog.Children.Add(new MenuItemInfo { Name = "Aras翻译日志", Icon = "Translate", CardIcon = "\uE81C", Description = "字段、表单、窗体翻译任务与明细" });
        MenuItems.Add(sysLog);

        // ===== 顶层叶节点 =====
        MenuItems.Add(new MenuItemInfo { Name = "个人任务记录", Icon = "ClipboardCheck", CardIcon = "\uE8FD", Description = "个人待办任务管理" });
        MenuItems.Add(new MenuItemInfo { Name = "我的资料", Icon = "Folder", CardIcon = "\uE8B7", Description = "文件资源管理器" });
    }
}
