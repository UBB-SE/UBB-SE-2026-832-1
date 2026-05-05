using System.Net.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views;

public sealed partial class WorkoutLogView : Page
{
    private readonly WorkoutLogViewModel viewModel;
    private readonly UserSession userSession;

    public WorkoutLogView()
    {
        this.InitializeComponent();

        this.userSession = new UserSession();
        this.viewModel = new WorkoutLogViewModel(
            new WorkoutLogService(new WorkoutLogServiceProxy(new HttpClient())));
        this.DataContext = this.viewModel;

        this.Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs args)
    {
        await this.viewModel.LoadClientWeightAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
        await this.viewModel.LoadWorkoutHistoryAsync(this.userSession.CurrentClientId).ConfigureAwait(true);
    }
}
