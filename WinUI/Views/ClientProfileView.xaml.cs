using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WinUI.Views;

public sealed partial class ClientProfileView : Page
{
    public object? ViewModel { get; }

    public ClientProfileView()
    {
        this.InitializeComponent();

        // Stub: viewmodel will be implemented later
        this.ViewModel = null;
        this.DataContext = this.ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
    {
        base.OnNavigatedTo(eventArgs);

        if (this.ViewModel == null)
        {
            return;
        }

        if (eventArgs.Parameter is int clientId)
        {
            // Call LoadClientData on viewmodel when available
        }
    }
}
