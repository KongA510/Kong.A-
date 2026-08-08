using ArasToolkit.App.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ArasToolkit.App.WinUI.Views;

public sealed partial class ObjectClassConfigurationPage : Page
{
    private const double CompactLayoutWidth = 760;
    private const double WideLayoutWidth = 1180;

    private ObjectClassConfigurationViewModel Vm => (ObjectClassConfigurationViewModel)DataContext;

    public ObjectClassConfigurationPage()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ObjectClassConfigurationViewModel>();
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
        => await Vm.InitializeAsync();

    private void Page_Unloaded(object sender, RoutedEventArgs e)
        => Vm.Dispose();

    private void SelectionCheckBox_Click(object sender, RoutedEventArgs e)
        => Vm.NotifySelectionChanged();

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width >= WideLayoutWidth)
        {
            ApplyWideLayout();
        }
        else if (e.NewSize.Width >= CompactLayoutWidth)
        {
            ApplyMediumLayout();
        }
        else
        {
            ApplyCompactLayout();
        }
    }

    private void ApplyWideLayout()
    {
        PageContent.Margin = new Thickness(32, 24, 32, 36);
        SetQueryColumns(new GridLength(1, GridUnitType.Star), GridLength.Auto,
            GridLength.Auto, GridLength.Auto, GridLength.Auto);
        Place(QueryTextBox, 0, 0);
        Place(QueryButton, 0, 1);
        Place(SelectAllButton, 0, 2);
        Place(ClearSelectionButton, 0, 3);
        Place(SelectionSummaryText, 0, 4);

        OptionStack.Orientation = Orientation.Horizontal;
        ApplyTwoColumnRoleLayout();
        ApplyInlineRoleHeader();
        ApplyInlineExecutionLayout();
    }

    private void ApplyMediumLayout()
    {
        PageContent.Margin = new Thickness(24, 20, 24, 32);
        SetQueryColumns(new GridLength(1, GridUnitType.Star), GridLength.Auto,
            GridLength.Auto, GridLength.Auto, new GridLength(0));
        Place(QueryTextBox, 0, 0, 4);
        Place(QueryButton, 1, 0);
        Place(SelectAllButton, 1, 1);
        Place(ClearSelectionButton, 1, 2);
        Place(SelectionSummaryText, 1, 3);

        OptionStack.Orientation = Orientation.Vertical;
        ApplyTwoColumnRoleLayout();
        ApplyInlineRoleHeader();
        ApplyInlineExecutionLayout();
    }

    private void ApplyCompactLayout()
    {
        PageContent.Margin = new Thickness(16, 16, 16, 28);
        SetQueryColumns(new GridLength(1, GridUnitType.Star), new GridLength(0),
            new GridLength(0), new GridLength(0), new GridLength(0));
        Place(QueryTextBox, 0, 0);
        Place(QueryButton, 1, 0);
        Place(SelectAllButton, 2, 0);
        Place(ClearSelectionButton, 3, 0);
        Place(SelectionSummaryText, 4, 0);

        OptionStack.Orientation = Orientation.Vertical;
        RoleSettingsGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        RoleSettingsGrid.ColumnDefinitions[1].Width = new GridLength(0);
        Place(CreatorRolesTextBox, 0, 0);
        Place(ReadOnlyRolesTextBox, 1, 0);
        Place(AdminRolesTextBox, 2, 0);
        Place(CanAddRolesTextBox, 3, 0);
        Place(TransitionRoleTextBox, 4, 0);
        Place(RoleHintText, 5, 0);

        Place(RoleHeaderTitle, 0, 0, 3);
        Place(ResetSettingsButton, 1, 1);
        Place(SaveSettingsButton, 1, 2);

        Place(ProgressPanel, 0, 0, 3);
        Place(CancelButton, 1, 1);
        Place(ExecuteButton, 1, 2);
    }

    private void ApplyTwoColumnRoleLayout()
    {
        RoleSettingsGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        RoleSettingsGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        Place(CreatorRolesTextBox, 0, 0);
        Place(ReadOnlyRolesTextBox, 0, 1);
        Place(AdminRolesTextBox, 1, 0);
        Place(CanAddRolesTextBox, 1, 1);
        Place(TransitionRoleTextBox, 2, 0);
        Place(RoleHintText, 2, 1);
    }

    private void ApplyInlineRoleHeader()
    {
        Place(RoleHeaderTitle, 0, 0);
        Place(ResetSettingsButton, 0, 1);
        Place(SaveSettingsButton, 0, 2);
    }

    private void ApplyInlineExecutionLayout()
    {
        Place(ProgressPanel, 0, 0);
        Place(CancelButton, 0, 1);
        Place(ExecuteButton, 0, 2);
    }

    private void SetQueryColumns(params GridLength[] widths)
    {
        for (var index = 0; index < widths.Length; index++)
        {
            QueryGrid.ColumnDefinitions[index].Width = widths[index];
        }
    }

    private static void Place(FrameworkElement element, int row, int column, int columnSpan = 1)
    {
        Grid.SetRow(element, row);
        Grid.SetColumn(element, column);
        Grid.SetColumnSpan(element, columnSpan);
    }
}
