using Microsoft.UI.Xaml.Controls;
using WinUI.Views;

namespace WinUI.Services;

public sealed class NavigationService : INavigationService
{
    private Frame? frame;
    private readonly IAnalyticsDashboardRefreshBus refreshBus;

    public NavigationService(IAnalyticsDashboardRefreshBus refreshBus)
    {
        this.refreshBus = refreshBus;
    }

    public void AttachFrame(Frame frame)
    {
        this.frame = frame;
    }

    public void NavigateToClientDashboard(bool requestRefresh)
    {
        frame?.Navigate(typeof(MainView));

        if (requestRefresh)
        {
            refreshBus.RequestRefresh();
        }
    }

    public void NavigateToCalendarIntegration()
    {
        frame?.Navigate(typeof(CalendarIntegrationPage));
    }

    public void NavigateToRankShowcase()
    {
        // TODO: Navigate to RankShowcasePage once it is implemented
    }

    public void NavigateToActiveWorkout(int clientId = 0)
    {
        // TODO: Navigate to ActiveWorkoutPage once it is implemented
    }

    public void NavigateToWorkoutLogs()
    {
        // TODO: Navigate to WorkoutLogsPage once it is implemented
    }

    public void GoBack()
    {
        if (frame?.CanGoBack == true)
        {
            frame.GoBack();
        }
    }

    public void NavigateToTrainerDashboard()
    {
        // TODO: Navigate to TrainerDashboardView once it is implemented
    }

    public void NavigateToClientProfile(int clientId)
    {
        // TODO: Navigate to ClientProfileView once it is implemented
    }
}
