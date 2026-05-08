using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(long clientId);

    Task<WorkoutHistoryPageResultDto> GetHistoryAsync(long clientId, int page, int pageSize);

    Task<List<ConsistencyWeekBucketDto>> GetConsistencyAsync(long clientId);
}
