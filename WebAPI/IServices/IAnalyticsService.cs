using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Models;

namespace WebAPI.IServices;

public interface IAnalyticsService
{
    void ComputeCaloriesForWorkout(WorkoutLog log, double clientWeightKg);

    string DetermineIntensityTag(float averageMetabolicEquivalent);

    Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId);

    Task<DashboardSummary> GetDashboardSummaryAsync(int clientId);
}
