using System.ComponentModel;
using ArasToolkit.App.WinUI.ViewModels;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>相关代码主题与多语言代码段页面。</summary>
public sealed partial class RelatedCodeRecordPage : Page
{
    private const int MaximumHighlightedCharacters = 500_000;
    private readonly ICodeSyntaxHighlightService _highlightService;
    private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer _previewTimer;
    private bool _isNavigated;

    private RelatedCodeRecordViewModel Vm => (RelatedCodeRecordViewModel)DataContext;

    public RelatedCodeRecordPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<RelatedCodeRecordViewModel>();
        _highlightService = App.Services.GetRequiredService<ICodeSyntaxHighlightService>();
        _previewTimer = DispatcherQueue.CreateTimer();
        _previewTimer.Interval = TimeSpan.FromMilliseconds(200);
        _previewTimer.IsRepeating = false;
        _previewTimer.Tick += (_, _) => RenderEditorPreview();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _isNavigated = true;
        Vm.PropertyChanged -= ViewModel_PropertyChanged;
        Vm.PropertyChanged += ViewModel_PropertyChanged;
        QueueEditorPreview();
        RenderOpenedSegment();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _isNavigated = false;
        _previewTimer.Stop();
        Vm.PropertyChanged -= ViewModel_PropertyChanged;
        base.OnNavigatedFrom(e);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(RelatedCodeRecordViewModel.CodeContent)
            or nameof(RelatedCodeRecordViewModel.SelectedCodeType)
            or nameof(RelatedCodeRecordViewModel.IsSegmentEditorVisible))
        {
            QueueEditorPreview();
        }

        if (e.PropertyName == nameof(RelatedCodeRecordViewModel.OpenedSegment))
            RenderOpenedSegment();
    }

    private void QueueEditorPreview()
    {
        if (!_isNavigated)
            return;
        _previewTimer.Stop();
        _previewTimer.Start();
    }

    private void RenderEditorPreview()
    {
        if (!_isNavigated || !Vm.IsSegmentEditorVisible)
        {
            EditorPreview.Blocks.Clear();
            return;
        }
        RenderCode(EditorPreview, Vm.CodeContent, Vm.SelectedCodeType);
    }

    private void RenderOpenedSegment()
    {
        var segment = Vm.OpenedSegment;
        if (!_isNavigated || segment == null)
        {
            OpenedSegmentCode.Blocks.Clear();
            return;
        }

        RenderCode(OpenedSegmentCode, segment.CodeContent, segment.CodeType);
        CodeScrollViewer.ChangeView(null, 0, null, true);
    }

    private void RenderCode(RichTextBlock viewer, string code, string codeType)
    {
        viewer.Blocks.Clear();
        var paragraph = new Paragraph();
        if (string.IsNullOrEmpty(code))
        {
            viewer.Blocks.Add(paragraph);
            return;
        }

        if (code.Length > MaximumHighlightedCharacters)
        {
            paragraph.Inlines.Add(CreateRun(code, CodeHighlightStyle.Default));
            viewer.Blocks.Add(paragraph);
            return;
        }

        foreach (var span in _highlightService.Highlight(code, codeType))
        {
            if (span.Length <= 0 || span.Start < 0 || span.Start + span.Length > code.Length)
                continue;
            paragraph.Inlines.Add(CreateRun(code.Substring(span.Start, span.Length), span.Style));
        }
        viewer.Blocks.Add(paragraph);
    }

    private static Run CreateRun(string text, CodeHighlightStyle style) => new()
    {
        Text = text,
        Foreground = new SolidColorBrush(style switch
        {
            CodeHighlightStyle.Keyword => Windows.UI.Color.FromArgb(255, 79, 70, 229),
            CodeHighlightStyle.Type => Windows.UI.Color.FromArgb(255, 124, 58, 237),
            CodeHighlightStyle.String => Windows.UI.Color.FromArgb(255, 180, 83, 9),
            CodeHighlightStyle.Comment => Windows.UI.Color.FromArgb(255, 21, 128, 61),
            CodeHighlightStyle.Number => Windows.UI.Color.FromArgb(255, 3, 105, 161),
            CodeHighlightStyle.Operator => Windows.UI.Color.FromArgb(255, 100, 116, 139),
            CodeHighlightStyle.Markup => Windows.UI.Color.FromArgb(255, 100, 116, 139),
            CodeHighlightStyle.Attribute => Windows.UI.Color.FromArgb(255, 3, 105, 161),
            CodeHighlightStyle.AttributeValue => Windows.UI.Color.FromArgb(255, 180, 83, 9),
            _ => Windows.UI.Color.FromArgb(255, 31, 41, 55)
        })
    };

    private async void SearchButton_Click(object sender, RoutedEventArgs e) => await Vm.SearchAsync();

    private async void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter)
            return;
        e.Handled = true;
        await Vm.SearchAsync();
    }

    private void ThemeList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        e.Cancel = Vm.IsBusy || e.Items.OfType<RelatedCodeRecord>()
            .Any(item => item.UserId != CurrentUserContext.CurrentUserId);
    }

    private async void ThemeList_DragItemsCompleted(object sender, DragItemsCompletedEventArgs e)
    {
        if (e.DropResult == DataPackageOperation.Move)
            await Vm.PersistRecordOrderAsync();
    }

    private async void SegmentMenuList_DragItemsCompleted(object sender, DragItemsCompletedEventArgs e)
    {
        if (e.DropResult == DataPackageOperation.Move)
            await Vm.PersistSegmentOrderAsync();
    }

    private void SegmentMenuList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (OpenSelectedSegment())
            e.Handled = true;
    }

    private void SegmentMenuList_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && OpenSelectedSegment())
            e.Handled = true;
    }

    private bool OpenSelectedSegment()
    {
        if (SegmentMenuList.SelectedItem is not RelatedCodeSegment segment)
            return false;
        Vm.OpenSegment(segment);
        return true;
    }

    private void EditSegmentButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: RelatedCodeSegment segment })
            Vm.BeginEditSegment(segment);
    }

    private async void DeleteSegmentButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: RelatedCodeSegment segment })
            await Vm.DeleteSegmentAsync(segment);
    }

    private async void MoveUpButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: RelatedCodeSegment segment })
            await Vm.MoveSegmentAsync(segment, -1);
    }

    private async void MoveDownButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: RelatedCodeSegment segment })
            await Vm.MoveSegmentAsync(segment, 1);
    }

    private async void CopySegmentButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: RelatedCodeSegment segment })
            return;
        try
        {
            var package = new DataPackage();
            package.SetText(segment.CodeContent);
            Clipboard.SetContent(package);
            Clipboard.Flush();
            Vm.ReportCopied(segment);
        }
        catch (Exception ex)
        {
            await Vm.ReportClipboardErrorAsync(ex);
        }
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var width = e.NewSize.Width;
        if (width >= 920)
        {
            WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(width >= 1280 ? 270 : 220);
            WorkspaceGrid.ColumnDefinitions[1].Width = new GridLength(width >= 1280 ? 360 : 310);
            WorkspaceGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            WorkspaceGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            WorkspaceGrid.RowDefinitions[1].Height = new GridLength(0);
            WorkspaceGrid.RowDefinitions[2].Height = new GridLength(0);
            PlacePanel(ThemePanel, 0, 0);
            PlacePanel(SegmentPanel, 0, 1);
            PlacePanel(CodePanel, 0, 2);
            return;
        }

        if (width >= 700)
        {
            WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            WorkspaceGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            WorkspaceGrid.ColumnDefinitions[2].Width = new GridLength(0);
            WorkspaceGrid.RowDefinitions[0].Height = new GridLength(300);
            WorkspaceGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            WorkspaceGrid.RowDefinitions[2].Height = new GridLength(0);
            PlacePanel(ThemePanel, 0, 0);
            PlacePanel(SegmentPanel, 0, 1);
            PlacePanel(CodePanel, 1, 0, 2);
            return;
        }

        WorkspaceGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        WorkspaceGrid.ColumnDefinitions[1].Width = new GridLength(0);
        WorkspaceGrid.ColumnDefinitions[2].Width = new GridLength(0);
        WorkspaceGrid.RowDefinitions[0].Height = new GridLength(210);
        WorkspaceGrid.RowDefinitions[1].Height = new GridLength(250);
        WorkspaceGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
        PlacePanel(ThemePanel, 0, 0);
        PlacePanel(SegmentPanel, 1, 0);
        PlacePanel(CodePanel, 2, 0);
    }

    private static void PlacePanel(FrameworkElement panel, int row, int column, int columnSpan = 1)
    {
        Grid.SetRow(panel, row);
        Grid.SetColumn(panel, column);
        Grid.SetColumnSpan(panel, columnSpan);
    }
}
