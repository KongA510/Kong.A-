using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class PackageMigrationPage : Page
{
    private PackageMigrationViewModel Vm => (PackageMigrationViewModel)DataContext;
    public PackageMigrationPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<PackageMigrationViewModel>();
        Loaded += async (_, _) => await Vm.InitializeAsync();
        Vm.DependenciesChanged += RenderDependencies;
        Unloaded += (_, _) => { Vm.DependenciesChanged -= RenderDependencies; Vm.Dispose(); };
    }

    private void RenderDependencies()
    {
        DependencyTree.RootNodes.Clear();
        var nodes = Vm.Dependencies.GroupBy(x => x.Item.Key).ToDictionary(g => g.Key,
            g => new TreeViewNode { Content = g.First().DisplayName + " · " + g.First().Reason, IsExpanded = string.IsNullOrEmpty(g.First().ParentKey) });
        var attached = new HashSet<string>();
        foreach (var entry in Vm.Dependencies)
        {
            if (!attached.Add(entry.Item.Key)) continue;
            var ancestors = new HashSet<string> { entry.Item.Key };
            var parentKey = entry.ParentKey;
            bool cyclic = false;
            while (!string.IsNullOrEmpty(parentKey))
            {
                if (!ancestors.Add(parentKey)) { cyclic = true; break; }
                parentKey = Vm.Dependencies.FirstOrDefault(x => x.Item.Key == parentKey)?.ParentKey ?? "";
            }
            if (!cyclic && nodes.TryGetValue(entry.ParentKey, out var parent)) parent.Children.Add(nodes[entry.Item.Key]);
            else DependencyTree.RootNodes.Add(nodes[entry.Item.Key]);
        }
    }

    private void RemoveSelection_Click(object sender, RoutedEventArgs e)
    { if (sender is Button { Tag: PackageItemSelection item }) Vm.RemoveSelection(item); }
    private async void OpenHistory_Click(object sender, RoutedEventArgs e)
    { if (sender is Button { Tag: PackageMigrationResult entry }) await Vm.OpenHistoryAsync(entry); }
}
