using ClassLibrary.IRepositories;
using ClassLibrary.Models.Analytics;
using ClassLibrary.Models;

namespace WinUI.Services;

/// <summary>
/// UI service implementation placeholder for ClientDashboardViewModel. The
/// implementation should call into repository interfaces (EF Core) to
/// perform operations. Currently methods are declared as stubs; they will
/// be implemented later as viewmodels are refined.
///
/// Any non-repository logic from the original source must not be placed
/// here — only UI-related orchestration belongs in this project-specific
/// service. Data access must go through ClassLibrary repositories.
/// </summary>
public sealed class ClientDashboardService : IClientDashboardService
{
    private readonly IWorkoutAnalyticsRepository analyticsRepository;

    public ClientDashboardService(IWorkoutAnalyticsRepository analyticsRepository)
    {
        this.analyticsRepository = analyticsRepository;
    }

    public Task<DashboardSummary> GetDashboardSummaryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new NotImplementedException();
    }

    public Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // TODO: Implement using EF Core repositories as viewmodels are refined.
        throw new NotImplementedException();
    }
}
