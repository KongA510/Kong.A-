using System.ComponentModel;
using System.Text.RegularExpressions;
using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;

namespace ArasToolkit.App.WinUI.Views;

/// <summary>XML / JSON 格式化、比对与实体转换共享页面。</summary>
public sealed partial class DataToolsPage : Page
{
    private const int MaximumHighlightedCharacters = 500_000;

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

    private bool _isNavigated;
    private bool _isRenderQueued;
    private bool _isRendering;

    private DataToolsViewModel Vm => (DataToolsViewModel)DataContext;

    public DataToolsPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<DataToolsViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _isNavigated = true;
        Vm.PropertyChanged -= ViewModel_PropertyChanged;
        Vm.Configure(e.Parameter?.ToString());
        Vm.PropertyChanged += ViewModel_PropertyChanged;
        _ = Vm.RefreshSavedXmlAsync();
        QueueXmlOutputRender();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _isNavigated = false;
        Vm.PropertyChanged -= ViewModel_PropertyChanged;
        base.OnNavigatedFrom(e);
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
        XmlOutputContainer.Height = transformHeight;
        PlainTransformOutputEditor.MinHeight = transformHeight;
        var compareHeight = compact ? 220 : 270;
        CompareLeftEditor.MinHeight = compareHeight;
        CompareRightEditor.MinHeight = compareHeight;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DataToolsViewModel.OutputText) or nameof(DataToolsViewModel.IsXmlFormatter))
            QueueXmlOutputRender();
    }

    /// <summary>
    /// 仅在 XML 格式化页面安排一次 UI 线程渲染。
    /// 比对模式没有 OutputText 呈现需求，因此不会创建任何高亮文本节点。
    /// </summary>
    private void QueueXmlOutputRender()
    {
        if (!_isNavigated || !Vm.IsXmlFormatter || _isRenderQueued)
            return;

        _isRenderQueued = true;
        if (DispatcherQueue.TryEnqueue(() =>
            {
                _isRenderQueued = false;
                if (!_isNavigated || !Vm.IsXmlFormatter) return;
                RenderXmlOutput();
            }))
            return;

        _isRenderQueued = false;
    }

    private void RenderXmlOutput()
    {
        if (_isRendering || !_isNavigated || !Vm.IsXmlFormatter) return;

        var output = Vm.OutputText ?? string.Empty;
        _isRendering = true;
        try
        {
            ShowHighlightedXml(output);
        }
        catch (Exception ex)
        {
            // 高亮失败时仍必须呈现完整格式化结果，不能再次出现“处理成功但右侧空白”。
            try
            {
                ShowPlainXml(output);
            }
            catch (Exception fallbackException)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[DataToolsPage] XML 纯文本兜底呈现失败: {fallbackException.Message}");
            }
            _ = Vm.NotifyOutputRenderFailedAsync(ex);
        }
        finally
        {
            _isRendering = false;
        }
    }

    private void ShowHighlightedXml(string output)
    {
        TransformOutputViewer.Blocks.Clear();
        var paragraph = new Paragraph();
        if (output.Length == 0)
        {
            TransformOutputViewer.Blocks.Add(paragraph);
            return;
        }

        // 超大 XML 使用单个 Run 呈现，避免数万个高亮节点造成界面卡顿。
        if (output.Length > MaximumHighlightedCharacters)
        {
            paragraph.Inlines.Add(CreateRun(output, DefaultCodeColor));
            TransformOutputViewer.Blocks.Add(paragraph);
            return;
        }

        var styles = BuildXmlStyles(output);
        var segmentStart = 0;
        for (var index = 1; index <= output.Length; index++)
        {
            if (index < output.Length && styles[index] == styles[segmentStart]) continue;
            paragraph.Inlines.Add(CreateRun(
                output.Substring(segmentStart, index - segmentStart),
                GetStyleColor((XmlTokenStyle)styles[segmentStart])));
            segmentStart = index;
        }
        TransformOutputViewer.Blocks.Add(paragraph);
    }

    private void ShowPlainXml(string output)
    {
        TransformOutputViewer.Blocks.Clear();
        var paragraph = new Paragraph();
        if (output.Length > 0)
            paragraph.Inlines.Add(CreateRun(output, DefaultCodeColor));
        TransformOutputViewer.Blocks.Add(paragraph);
    }

    private static byte[] BuildXmlStyles(string output)
    {
        var styles = new byte[output.Length];

        foreach (Match textMatch in XmlTextRegex.Matches(output))
        {
            var text = textMatch.Groups["text"];
            if (!string.IsNullOrWhiteSpace(text.Value))
                ApplyStyle(styles, text.Index, text.Length, XmlTokenStyle.ElementText);
        }

        foreach (Match markup in XmlMarkupRegex.Matches(output))
            ApplyMarkupStyles(styles, markup);
        return styles;
    }

    private static void ApplyMarkupStyles(byte[] styles, Match markup)
    {
        if (markup.Value.StartsWith("<!--", StringComparison.Ordinal) ||
            markup.Value.StartsWith("<![CDATA[", StringComparison.Ordinal))
        {
            ApplyStyle(styles, markup.Index, markup.Length, XmlTokenStyle.Comment);
            return;
        }

        if (markup.Value.StartsWith("<?", StringComparison.Ordinal) ||
            markup.Value.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            ApplyStyle(styles, markup.Index, markup.Length, XmlTokenStyle.Declaration);
            return;
        }

        ApplyStyle(styles, markup.Index, markup.Length, XmlTokenStyle.Markup);

        var tagName = XmlTagNameRegex.Match(markup.Value).Groups["name"];
        if (tagName.Success)
            ApplyStyle(styles, markup.Index + tagName.Index, tagName.Length, XmlTokenStyle.Tag);

        foreach (Match attribute in XmlAttributeRegex.Matches(markup.Value))
        {
            var name = attribute.Groups["name"];
            var quotedValue = attribute.Groups["quoted"];
            ApplyStyle(styles, markup.Index + name.Index, name.Length, XmlTokenStyle.Attribute);
            ApplyStyle(styles, markup.Index + quotedValue.Index, quotedValue.Length, XmlTokenStyle.AttributeValue);
        }
    }

    private static void ApplyStyle(byte[] styles, int start, int length, XmlTokenStyle style)
    {
        if (start < 0 || length <= 0 || start >= styles.Length) return;
        var end = (int)Math.Min((long)start + length, styles.Length);
        if (end <= start) return;
        Array.Fill(styles, (byte)style, start, end - start);
    }

    private static Run CreateRun(string text, Windows.UI.Color color) => new()
    {
        Text = text,
        Foreground = new SolidColorBrush(color)
    };

    private static Windows.UI.Color GetStyleColor(XmlTokenStyle style) => style switch
    {
        XmlTokenStyle.Markup => MarkupColor,
        XmlTokenStyle.Tag => TagColor,
        XmlTokenStyle.Attribute => AttributeColor,
        XmlTokenStyle.AttributeValue => AttributeValueColor,
        XmlTokenStyle.ElementText => ElementTextColor,
        XmlTokenStyle.Comment => CommentColor,
        XmlTokenStyle.Declaration => DeclarationColor,
        _ => DefaultCodeColor
    };

    private enum XmlTokenStyle : byte
    {
        Default,
        ElementText,
        Markup,
        Tag,
        Attribute,
        AttributeValue,
        Comment,
        Declaration
    }
}
