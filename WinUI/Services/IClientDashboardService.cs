namespace WinUI.Services;

public interface IClientDashboardService
{
    Task GetDashboardSummary(int clientId);

    Task GetConsistencyLastFourWeeks(int clientId);

    Task GetWorkoutHistoryPage(int clientId, int page, int pageSize);

    Task GetRecentAchievements(int clientId);
}
