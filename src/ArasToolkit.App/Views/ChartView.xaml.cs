using System.Windows.Controls;
using System;

namespace ArasToolkit.App.Views;

/// <summary>
/// ChartView.xaml 鐨勪氦浜掗€昏緫
/// </summary>
public partial class ChartView : UserControl
{
   public ChartView()
   {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ChartView] 初始化失败: {ex.Message}");
            throw;
        }
   }
}
