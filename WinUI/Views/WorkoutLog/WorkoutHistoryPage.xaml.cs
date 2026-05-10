using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI.Services;
using WinUI.ViewModels;

namespace WinUI.Views.WorkoutLog;

public sealed partial class WorkoutHistoryPage : Page
{
    public WorkoutLogViewModel ViewModel { get; }
    public int ClientId { get; private set; }

    public WorkoutHistoryPage()
    {
        var userSession = new UserSession();
        ClientId = userSession.CurrentClientId;
        ViewModel = new WorkoutLogViewModel(
            new WorkoutLogService(new System.Net.Http.HttpClient()));
        ViewModel.StartWorkoutRequested += clientId => Frame.Navigate(typeof(ActiveWorkoutView), clientId);
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.LoadLogsCommand.Execute(ClientId);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
    }
}
