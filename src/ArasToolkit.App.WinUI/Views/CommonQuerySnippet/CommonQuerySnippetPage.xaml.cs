using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>常用 SQL/AML/XML 片段库。</summary>
public sealed partial class CommonQuerySnippetPage : Page
{
    public CommonQuerySnippetPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<CommonQuerySnippetViewModel>();
    }

    private CommonQuerySnippetViewModel? ViewModel => DataContext as CommonQuerySnippetViewModel;

    private async void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.SearchAsync();
    }

    private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter || ViewModel == null)
            return;
        e.Handled = true;
        await ViewModel.SearchAsync();
    }

    private async void CopyPreviewButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel == null || string.IsNullOrWhiteSpace(ViewModel.PreviewContent))
            return;

        try
        {
            var package = new DataPackage();
            package.SetText(ViewModel.PreviewContent);
            Clipboard.SetContent(package);
            Clipboard.Flush();
            ViewModel.ReportCopied();
        }
        catch (Exception ex)
        {
            await ViewModel.ReportClipboardErrorAsync(ex);
        }
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < 1050;

        WorkspaceGrid.ColumnDefinitions[0].Width = compact
            ? new GridLength(1, GridUnitType.Star)
            : new GridLength(330);
        WorkspaceGrid.ColumnDefinitions[1].Width = compact
            ? new GridLength(0)
            : new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.RowDefinitions[0].Height = compact ? new GridLength(300) : new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.RowDefinitions[1].Height = compact ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        Grid.SetRow(LibraryPanel, 0);
        Grid.SetColumn(LibraryPanel, 0);
        Grid.SetRow(EditorPanel, compact ? 1 : 0);
        Grid.SetColumn(EditorPanel, compact ? 0 : 1);

        var stackCodePanels = e.NewSize.Width < 760;
        CodeWorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        CodeWorkspaceGrid.ColumnDefinitions[1].Width = stackCodePanels
            ? new GridLength(0)
            : new GridLength(1, GridUnitType.Star);
        CodeWorkspaceGrid.RowDefinitions[0].Height = stackCodePanels ? new GridLength(250) : new GridLength(1, GridUnitType.Star);
        CodeWorkspaceGrid.RowDefinitions[1].Height = stackCodePanels ? new GridLength(250) : new GridLength(0);
        Grid.SetRow(SourcePanel, 0);
        Grid.SetColumn(SourcePanel, 0);
        Grid.SetRow(PreviewPanel, stackCodePanels ? 1 : 0);
        Grid.SetColumn(PreviewPanel, stackCodePanels ? 0 : 1);
    }
}
