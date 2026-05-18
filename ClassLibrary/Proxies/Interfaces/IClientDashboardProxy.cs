using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;

namespace ClassLibrary.Proxies.Interfaces;

public interface IClientDashboardProxy
{
    Task<DashboardSummary> GetDashboardSummaryAsync(int clientId);

    Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId);

    Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize);

    Task<WorkoutSessionDetail?> GetWorkoutSessionDetailAsync(int clientId, int workoutLogId);

    Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId);
}



