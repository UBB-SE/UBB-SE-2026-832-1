namespace WinUI.Services;

public interface IClientDashboardService
{
    Task GetDashboardSummaryAsync(int clientId);

    Task GetConsistencyLastFourWeeksAsync(int clientId);

    Task GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize);

    Task GetRecentAchievementsAsync(int clientId);
}
