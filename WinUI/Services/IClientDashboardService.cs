using ClassLibrary.Models;
using ClassLibrary.Models.Analytics;

namespace WinUI.Services;

/// <summary>
/// UI service surface for ClientDashboardViewModel. Methods are intentionally
/// defined but not implemented — viewmodels will call into this service and
/// the implementations will be filled out as the UI and viewmodels are
/// refined.
///
/// Note: This is a UI layer service. Any repository logic must remain in
/// the ClassLibrary repositories (EF Core); this interface does not contain
/// direct data access responsibilities.
/// </summary>
public interface IClientDashboardService
{
    Task<DashboardSummary> GetDashboardSummaryAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId, CancellationToken cancellationToken = default);
    Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken = default);
}
