using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;
using ClassLibrary.Models;

namespace WinUI.Services;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(long clientId, CancellationToken token);
    Task<List<ConsistencyWeekBucket>> GetConsistencyAsync(long clientId, CancellationToken token);
    Task<WorkoutHistoryPageResult> GetHistoryAsync(long clientId, int page, int pageSize, CancellationToken token);
}
