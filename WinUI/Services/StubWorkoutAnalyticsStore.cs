using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Models.Analytics;

namespace WinUI.Services;

/// <summary>
/// Stub implementation of IWorkoutAnalyticsStore that returns safe empty data.
/// This prevents crashes and allows the UI to display gracefully.
/// Can be replaced with a full SQLite or HTTP-based implementation later.
/// </summary>
public sealed class StubWorkoutAnalyticsStore : IWorkoutAnalyticsStore
{
    public Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<int> SaveWorkoutAsync(long clientId, ClassLibrary.Models.WorkoutLog log, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }

    public Task<DashboardSummary> GetDashboardSummaryAsync(long clientId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DashboardSummary
        {
            TotalWorkouts = 0,
            TotalActiveTimeLastSevenDays = TimeSpan.Zero,
            PreferredWorkoutName = null
        });
    }

    public Task<TimeSpan> GetTotalActiveTimeAsync(long clientId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(TimeSpan.Zero);
    }

    public Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(long clientId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var buckets = new List<ConsistencyWeekBucket>();
        
        for (int i = 0; i < 4; i++)
        {
            buckets.Add(new ConsistencyWeekBucket
            {
                WeekStart = today.AddDays(-21 + i * 7),
                WorkoutCount = 0
            });
        }
        
        return Task.FromResult<IReadOnlyList<ConsistencyWeekBucket>>(buckets);
    }

    public Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(long clientId, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new WorkoutHistoryPageResult
        {
            TotalCount = 0,
            Items = new List<WorkoutHistoryRow>()
        });
    }

    public Task<WorkoutSessionDetail?> GetWorkoutSessionDetailAsync(long clientId, int workoutLogId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<WorkoutSessionDetail?>(null);
    }
}
