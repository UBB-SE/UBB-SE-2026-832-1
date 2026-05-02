using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.Services;

public interface IClientDashboardService
{
    Task<DashboardSummary> GetDashboardSummaryAsync(int clientId);

    Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId);

    Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize);

    Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId);
}
