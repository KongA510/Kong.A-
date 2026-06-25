using System.Windows;

namespace ArasToolkit.App.Views;

public partial class TextPromptWindow : Window
{
    public string? InputText { get; private set; }

    public TextPromptWindow(string title, string prompt)
    {
        InitializeComponent();
        Title = title;
        TemplateNameBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        InputText = TemplateNameBox.Text?.Trim();
        if (string.IsNullOrEmpty(InputText))
        {
            MessageBox.Show("请输入模板名称", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}