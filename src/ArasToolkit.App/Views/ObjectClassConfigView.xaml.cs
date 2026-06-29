using System;
using System.Windows.Controls;

namespace ArasToolkit.App.Views;

/// <summary>
/// ObjectClassConfigView.xaml 的交互逻辑
/// </summary>
public partial class ObjectClassConfigView : UserControl
{
    public ObjectClassConfigView()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ObjectClassConfigView] 初始化失败: {ex.Message}");
            throw;
        }
    }
}
