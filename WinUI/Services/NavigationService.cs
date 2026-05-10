using Microsoft.UI.Xaml.Controls;
using WinUI.Views.CalendarIntegration;
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
        frame?.Navigate(typeof(RankShowcaseView));
    }

    public void NavigateToActiveWorkout(int clientId = 0)
    {
    
    }

    public void NavigateToWorkoutLogs()
    {
    
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
    
    }

    public void NavigateToClientProfile(int clientId)
    {
    
    }
}
