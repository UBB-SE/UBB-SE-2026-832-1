using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.Services;

public sealed class ClientDashboardService : IClientDashboardService
{
    private const string ROUTE = "api/client";
    private readonly HttpClient httpClient;

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<DashboardSummary>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/dashboard-summary");
            return result ?? new DashboardSummary();
        }
        catch
        {
            // Return safe empty data on error
            return new DashboardSummary();
        }
    }

    public async Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<ConsistencyWeekBucket>>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/consistency-four-weeks");
            return result ?? new List<ConsistencyWeekBucket>();
        }
        catch
        {
            // Return safe empty data on error
            return new List<ConsistencyWeekBucket>();
        }
    }

    public async Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<WorkoutHistoryPageResult>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/workout-history?page={page}&pageSize={pageSize}");
            return result ?? new WorkoutHistoryPageResult { TotalCount = 0, Items = new List<WorkoutHistoryRow>() };
        }
        catch
        {
            // Return safe empty data on error
            return new WorkoutHistoryPageResult { TotalCount = 0, Items = new List<WorkoutHistoryRow>() };
        }
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>(
                $"{ApiBaseUrl.BASE_URL}/{ROUTE}/{clientId}/achievements");
            return result ?? new List<AchievementDataTransferObject>();
        }
        catch
        {
            // Return safe empty data on error
            return new List<AchievementDataTransferObject>();
        }
    }
}
