using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class ClientDashboardPage : Page
{
    private readonly ClientDashboardViewModel viewModel;
    private readonly UserSession userSession;

    public ClientDashboardViewModel ViewModel => this.viewModel;

    public ClientDashboardPage()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new ClientDashboardViewModel(
            new ClientDashboardService(new System.Net.Http.HttpClient()),
            this.userSession);
        this.DataContext = this.viewModel;

        this.Loaded += this.Page_Loaded;
        this.Unloaded += this.Page_Unloaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await this.viewModel.LoadInitialAsync();

        var workoutState = new WorkoutUiState();
        var note = workoutState.ProgressionHeadsUp;
        if (!string.IsNullOrWhiteSpace(note))
        {
            ProgressionInfoBar.Message = note;
            ProgressionInfoBar.IsOpen = true;
            workoutState.ProgressionHeadsUp = null;
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        // Cleanup if needed
    }
}
