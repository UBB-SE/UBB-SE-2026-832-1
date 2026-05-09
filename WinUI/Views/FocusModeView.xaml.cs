using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI.Views;

public sealed partial class FocusModeView : Page
{
    private readonly ContentDialog? hostDialog;

    public object? ViewModel { get; }

    public FocusModeView()
        : this(null, null)
    {
    }

    public FocusModeView(object? viewModel, ContentDialog? hostDialog)
    {
        this.InitializeComponent();
        this.ViewModel = viewModel;
        this.DataContext = viewModel;
        this.hostDialog = hostDialog;
    }

    private void ExitButton_Click(object sender, RoutedEventArgs eventArgs)
    {
        if (this.hostDialog is not null)
        {
            this.hostDialog.Hide();
            return;
        }

        if (this.Frame is not null && this.Frame.CanGoBack)
        {
            this.Frame.GoBack();
        }
    }
}