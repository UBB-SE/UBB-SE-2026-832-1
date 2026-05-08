using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class DashboardService : IDashboardService
{
    private const int DaysPerWeek = 7;
    private const int ConsistencyWeeks = 4;

    private readonly IWorkoutAnalyticsRepository analyticsRepository;

    public DashboardService(IWorkoutAnalyticsRepository analyticsRepository)
    {
        this.analyticsRepository = analyticsRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(long clientId)
    {
        int clientIdInt = (int)clientId;
        int totalWorkouts = await this.analyticsRepository.GetTotalWorkoutsAsync(clientIdInt);

        var sevenDaysAgo = DateTime.Today.AddDays(-DaysPerWeek);
        var tomorrow = DateTime.Today.AddDays(1);
        var recentLogs = await this.analyticsRepository.GetWorkoutsInRangeAsync(clientIdInt, sevenDaysAgo, tomorrow);

        double totalActiveMinutes = 0;
        var nameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var log in recentLogs)
        {
            totalActiveMinutes += log.Duration.TotalMinutes;

            if (!string.IsNullOrWhiteSpace(log.WorkoutName))
            {
                nameCounts.TryGetValue(log.WorkoutName, out int count);
                nameCounts[log.WorkoutName] = count + 1;
            }
        }

        string preferred = nameCounts.Count > 0
            ? nameCounts.MaxBy(kvp => kvp.Value).Key
            : string.Empty;

        return new DashboardSummaryDto
        {
            TotalWorkouts = totalWorkouts,
            TotalActiveTimeLastSevenDays = totalActiveMinutes,
            PreferredWorkoutName = preferred,
        };
    }

    public async Task<WorkoutHistoryPageResultDto> GetHistoryAsync(long clientId, int page, int pageSize)
    {
        int skip = page * pageSize;
        var (logs, totalCount) = await this.analyticsRepository.GetWorkoutHistoryPageAsync((int)clientId, skip, pageSize);

        return new WorkoutHistoryPageResultDto
        {
            TotalCount = totalCount,
            Items = logs.Select(log => new WorkoutHistoryItemDto
            {
                WorkoutId = log.WorkoutLogId,
                Date = log.Date,
                ActivityName = log.WorkoutName ?? string.Empty,
                Duration = log.Duration.TotalMinutes,
            }).ToList(),
        };
    }

    public async Task<List<ConsistencyWeekBucketDto>> GetConsistencyAsync(long clientId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var mondayThisWeek = GetMondayOfWeek(today);
        var buckets = new List<ConsistencyWeekBucketDto>(ConsistencyWeeks);

        for (int i = 0; i < ConsistencyWeeks; i++)
        {
            var weekStart = mondayThisWeek.AddDays(-(ConsistencyWeeks - 1 - i) * DaysPerWeek);
            var weekEnd = weekStart.AddDays(DaysPerWeek);

            var logs = await this.analyticsRepository.GetWorkoutsInRangeAsync(
                (int)clientId,
                weekStart.ToDateTime(TimeOnly.MinValue),
                weekEnd.ToDateTime(TimeOnly.MinValue));

            buckets.Add(new ConsistencyWeekBucketDto
            {
                WeekStart = weekStart.ToDateTime(TimeOnly.MinValue),
                WorkoutCount = logs.Count,
            });
        }

        return buckets;
    }

    private static DateOnly GetMondayOfWeek(DateOnly date)
    {
        int offset = date.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)date.DayOfWeek - (int)DayOfWeek.Monday;
        return date.AddDays(-offset);
    }
}
