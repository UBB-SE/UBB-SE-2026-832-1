using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.Services;

public sealed class ClientDashboardService : IClientDashboardService
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";
    private const string ROUTE = "api/client";
    private readonly HttpClient httpClient;

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public Task<DashboardSummary> GetDashboardSummaryAsync(int clientId)
    {
        return Task.FromResult(new DashboardSummary());
    }

    public Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId)
    {
        return Task.FromResult<IReadOnlyList<ConsistencyWeekBucket>>([]);
    }

    public async Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize)
    {
        var items = await httpClient.GetFromJsonAsync<List<WorkoutHistoryRow>>(
            $"{API_BASE_ADDRESS}/{ROUTE}/{clientId}/workout-history");
        return new WorkoutHistoryPageResult { TotalCount = items?.Count ?? 0, Items = items ?? [] };
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId)
    {
        var achievements = await httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>(
            $"{API_BASE_ADDRESS}/{ROUTE}/{clientId}/achievements");
        return achievements ?? [];
    }
}
