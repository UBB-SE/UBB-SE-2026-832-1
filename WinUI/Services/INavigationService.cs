namespace WinUI.Services;

public interface INavigationService
{
    void AttachFrame(Microsoft.UI.Xaml.Controls.Frame frame);

    void NavigateToClientDashboard(bool requestRefresh);

    void NavigateToCalendarIntegration();

    void NavigateToRankShowcase();

    void NavigateToActiveWorkout(int clientId = 0);

    void NavigateToWorkoutLogs();

    void GoBack();

    void NavigateToClientProfile(int clientId);

    void NavigateToTrainerDashboard();
}
