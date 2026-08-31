using System.ComponentModel;
using System.Text.RegularExpressions;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>XML / JSON 格式化、比对与实体转换共享页面。</summary>
public sealed partial class DataToolsPage : Page
{
    private static readonly Regex XmlMarkupRegex = new(
        @"<!--[\s\S]*?-->|<!\[CDATA\[[\s\S]*?\]\]>|<\?[\s\S]*?\?>|<!DOCTYPE[\s\S]*?>|<[^>]+>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex XmlTagNameRegex = new(
        @"^<\s*/?\s*(?<name>[A-Za-z_][\w:.-]*)",
        RegexOptions.Compiled);
    private static readonly Regex XmlAttributeRegex = new(
        @"(?<name>[A-Za-z_][\w:.-]*)(?<separator>\s*=\s*)(?<quoted>(?<quote>[""'])(?<value>.*?)(?:\k<quote>))",
        RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex XmlTextRegex = new(
        @">(?<text>[^<]+)<",
        RegexOptions.Compiled);

    private static readonly Windows.UI.Color DefaultCodeColor = Windows.UI.Color.FromArgb(255, 31, 41, 55);
    private static readonly Windows.UI.Color MarkupColor = Windows.UI.Color.FromArgb(255, 100, 116, 139);
    private static readonly Windows.UI.Color TagColor = Windows.UI.Color.FromArgb(255, 79, 70, 229);
    private static readonly Windows.UI.Color AttributeColor = Windows.UI.Color.FromArgb(255, 3, 105, 161);
    private static readonly Windows.UI.Color AttributeValueColor = Windows.UI.Color.FromArgb(255, 180, 83, 9);
    private static readonly Windows.UI.Color ElementTextColor = Windows.UI.Color.FromArgb(255, 15, 118, 110);
    private static readonly Windows.UI.Color CommentColor = Windows.UI.Color.FromArgb(255, 21, 128, 61);
    private static readonly Windows.UI.Color DeclarationColor = Windows.UI.Color.FromArgb(255, 124, 58, 237);

    private DataToolsViewModel Vm => (DataToolsViewModel)DataContext;

    public DataToolsPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<DataToolsViewModel>();
        Vm.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        Vm.Configure(e.Parameter?.ToString());
        RenderOutput();
    }

    private void CopyOutput_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Vm.OutputText)) return;
        var package = new DataPackage();
        package.SetText(Vm.OutputText);
        Clipboard.SetContent(package);
        Vm.NotifyOutputCopied();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        => ApplyResponsiveLayout(e.NewSize.Width < 1120);

    private void ApplyResponsiveLayout(bool compact)
    {
        TransformGrid.ColumnDefinitions[0].Width = new GridLength(compact ? 1 : 0.9, GridUnitType.Star);
        TransformGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(1.1, GridUnitType.Star);
        Grid.SetRow(TransformOutputCard, compact ? 1 : 0);
        Grid.SetColumn(TransformOutputCard, compact ? 0 : 1);

        CompareInputGrid.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        Grid.SetRow(CompareRightCard, compact ? 1 : 0);
        Grid.SetColumn(CompareRightCard, compact ? 0 : 1);

        var transformHeight = compact ? 380 : 520;
        TransformInputEditor.MinHeight = transformHeight;
        TransformOutputEditor.MinHeight = transformHeight;
        var compareHeight = compact ? 220 : 270;
        CompareLeftEditor.MinHeight = compareHeight;
        CompareRightEditor.MinHeight = compareHeight;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DataToolsViewModel.OutputText) or nameof(DataToolsViewModel.IsXmlFormatter))
            RenderOutput();
    }

    private void RenderOutput()
    {
        var output = Vm.OutputText ?? string.Empty;
        TransformOutputEditor.Document.SetText(TextSetOptions.None, output);
        TransformOutputEditor.Document.GetText(TextGetOptions.None, out var documentText);

        ApplyColor(0, documentText.Length, DefaultCodeColor);
        if (!Vm.IsXmlFormatter || string.IsNullOrWhiteSpace(output)) return;

        foreach (Match textMatch in XmlTextRegex.Matches(documentText))
        {
            var text = textMatch.Groups["text"];
            if (!string.IsNullOrWhiteSpace(text.Value))
                ApplyColor(text.Index, text.Length, ElementTextColor);
        }

        foreach (Match markup in XmlMarkupRegex.Matches(documentText))
            HighlightMarkup(markup);
    }

    private void HighlightMarkup(Match markup)
    {
        if (markup.Value.StartsWith("<!--", StringComparison.Ordinal) ||
            markup.Value.StartsWith("<![CDATA[", StringComparison.Ordinal))
        {
            ApplyColor(markup.Index, markup.Length, CommentColor);
            return;
        }

        if (markup.Value.StartsWith("<?", StringComparison.Ordinal) ||
            markup.Value.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            ApplyColor(markup.Index, markup.Length, DeclarationColor);
            return;
        }

        ApplyColor(markup.Index, markup.Length, MarkupColor);

        var tagName = XmlTagNameRegex.Match(markup.Value).Groups["name"];
        if (tagName.Success)
            ApplyColor(markup.Index + tagName.Index, tagName.Length, TagColor);

        foreach (Match attribute in XmlAttributeRegex.Matches(markup.Value))
        {
            var name = attribute.Groups["name"];
            var quotedValue = attribute.Groups["quoted"];
            ApplyColor(markup.Index + name.Index, name.Length, AttributeColor);
            ApplyColor(markup.Index + quotedValue.Index, quotedValue.Length, AttributeValueColor);
        }
    }

    private void ApplyColor(int start, int length, Windows.UI.Color color)
    {
        if (length <= 0) return;
        var range = TransformOutputEditor.Document.GetRange(start, start + length);
        range.CharacterFormat.ForegroundColor = color;
    }
}
