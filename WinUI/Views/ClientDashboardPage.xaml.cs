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
    private readonly AnalyticsDashboardRefreshBus refreshBus = AnalyticsDashboardRefreshBus.Shared;

    public ClientDashboardViewModel ViewModel => this.viewModel;

    public ClientDashboardPage()
    {
        this.InitializeComponent();

        try
        {
            this.userSession = new UserSession();
            this.viewModel = new ClientDashboardViewModel(
                new ClientDashboardService(new System.Net.Http.HttpClient()),
                this.userSession);
            this.DataContext = this.viewModel;

            this.Loaded += this.Page_Loaded;
            this.Unloaded += this.Page_Unloaded;
            this.refreshBus.RefreshRequested += this.RefreshBus_RefreshRequested;
        }
        catch (Exception ex)
        {
            // Fail gracefully if initialization fails
            System.Diagnostics.Debug.WriteLine($"ClientDashboardPage initialization error: {ex.Message}");
        }
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            // Log but don't crash
            System.Diagnostics.Debug.WriteLine($"ClientDashboardPage_Loaded error: {ex.Message}");
        }
    }

    private async void RefreshBus_RefreshRequested(object? sender, EventArgs e)
    {
        await this.viewModel.LoadInitialAsync();
    }

    private async void WorkoutItemExpander_Expanded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not Expander expander || expander.DataContext is not WorkoutHistoryItemViewModel item)
            {
                return;
            }

            await this.viewModel.LoadWorkoutDetailAsync(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WorkoutItemExpander_Expanded error: {ex.Message}");
        }
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        this.refreshBus.RefreshRequested -= this.RefreshBus_RefreshRequested;
    }
}
