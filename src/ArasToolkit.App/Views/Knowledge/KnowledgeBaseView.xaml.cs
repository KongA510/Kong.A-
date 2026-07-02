using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using ArasToolkit.App.ViewModels;
using Microsoft.Win32;

namespace ArasToolkit.App.Views;

/// <summary>
/// 个人资料库 View — 三态工作流：分类列表 → 查看(只读) → 编辑(RichTextBox)
/// </summary>
public partial class KnowledgeBaseView : UserControl
{
    private KnowledgeViewModel? _vm;

    public KnowledgeBaseView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 加载时注册 FlowDocument 回调 + 插入图片回调
    /// </summary>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        _vm = DataContext as KnowledgeViewModel;
        if (_vm == null) return;

        // 注册保存时获取 FlowDocument 的回调（编辑态使用）
        _vm.OnRequestDocument = () => EditorRichTextBox.Document;

        // 注册加载时设置 FlowDocument 的回调（编辑态使用）
        _vm.OnLoadDocument = doc =>
        {
            EditorRichTextBox.Document = doc;
            _vm.IsEditorDirty = false;
        };

        // 注册插入图片回调
        _vm.OnInsertImageRequested = InsertImage;

        // 注册内容变更回调
        _vm.OnEditorContentChanged = () => _vm.IsEditorDirty = true;

        // 监听 ViewDocument 变化以更新只读预览
        _vm.PropertyChanged += (s, args) =>
        {
            if (args.PropertyName == nameof(KnowledgeViewModel.ViewDocument) && _vm.ViewDocument != null)
            {
                ViewFlowDocumentScrollViewer.Document = _vm.ViewDocument;
            }
        };
    }

    /// <summary>
    /// 卸载时清理回调防止内存泄漏
    /// </summary>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_vm != null)
        {
            _vm.OnRequestDocument = null;
            _vm.OnLoadDocument = null;
            _vm.OnInsertImageRequested = null;
            _vm.OnEditorContentChanged = null;
            _vm = null;
        }
    }

    /// <summary>
    /// 分类列表选中变更 → 加载该分类笔记
    /// </summary>
    private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_vm == null || _vm.SelectedCategory == null) return;
        if (_vm.SelectCategoryCommand.CanExecute(_vm.SelectedCategory))
        {
            _vm.SelectCategoryCommand.Execute(_vm.SelectedCategory);
        }
    }

    /// <summary>
    /// 笔记卡片单击 — 选中高亮；双击 — 进入查看态
    /// </summary>
    private void NoteCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border border || border.DataContext is not Core.Entities.KnowledgeEntry entry)
            return;

        _vm!.SelectedNote = entry;

        if (e.ClickCount == 2)
        {
            if (_vm.ViewEntryCommand.CanExecute(entry))
            {
                _vm.ViewEntryCommand.Execute(entry);
            }
            e.Handled = true;
        }
    }

    /// <summary>
    /// RichTextBox 文本变更 — 标记为脏
    /// </summary>
    private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vm?.OnEditorContentChanged?.Invoke();
    }

    /// <summary>
    /// 快捷键支持：Ctrl+S 保存
    /// </summary>
    private void EditorRichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            _vm?.SaveEntryCommand.Execute(null);
        }
    }

    /// <summary>
    /// 清除搜索框
    /// </summary>
    private void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        if (_vm != null)
            _vm.SearchKeyword = "";
    }

    /// <summary>
    /// 插入图片 — 打开文件对话框，读取图片并插入到 RichTextBox
    /// </summary>
    private void InsertImage()
    {
        var dlg = new OpenFileDialog
        {
            Title = "选择图片",
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.gif;*.bmp|所有文件|*.*",
            Multiselect = false
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            var imageBytes = File.ReadAllBytes(dlg.FileName);

            // 创建 BitmapImage
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageBytes);
            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze(); // 跨线程安全

            // 限制显示最大宽度 600px
            if (bitmap.PixelWidth > 600)
            {
                var resized = new System.Windows.Media.Imaging.BitmapImage();
                resized.BeginInit();
                resized.StreamSource = new MemoryStream(imageBytes);
                resized.DecodePixelWidth = 600;
                resized.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                resized.EndInit();
                resized.Freeze();
                bitmap = resized;
            }

            // 创建 Image 控件并插入到 RichTextBox
            var image = new System.Windows.Controls.Image
            {
                Source = bitmap,
                Stretch = System.Windows.Media.Stretch.Uniform,
                MaxWidth = 600,
                Margin = new Thickness(0, 4, 0, 4)
            };

            var container = new InlineUIContainer(image, EditorRichTextBox.CaretPosition);
            EditorRichTextBox.CaretPosition = container.ElementEnd;

            _vm!.IsEditorDirty = true;

            // 如果当前 FlowDocument 没有任何 Block，先插入一个 Paragraph
            if (!EditorRichTextBox.Document.Blocks.Any())
            {
                EditorRichTextBox.Document.Blocks.Add(new Paragraph());
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"插入图片失败: {ex.Message}", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ===== 格式化工具栏事件 =====

    private void BoldButton_Click(object sender, RoutedEventArgs e)
    {
        EditorRichTextBox.Focus();
        EditingCommands.ToggleBold.Execute(null, EditorRichTextBox);
    }

    private void ItalicButton_Click(object sender, RoutedEventArgs e)
    {
        EditorRichTextBox.Focus();
        EditingCommands.ToggleItalic.Execute(null, EditorRichTextBox);
    }

    private void UnderlineButton_Click(object sender, RoutedEventArgs e)
    {
        EditorRichTextBox.Focus();
        EditingCommands.ToggleUnderline.Execute(null, EditorRichTextBox);
    }

    private void BulletListButton_Click(object sender, RoutedEventArgs e)
    {
        EditorRichTextBox.Focus();
        EditingCommands.ToggleBullets.Execute(null, EditorRichTextBox);
    }
}
