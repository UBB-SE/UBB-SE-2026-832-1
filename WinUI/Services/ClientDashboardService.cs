using System.Net.Http.Json;
using ClassLibrary.DTOs.Analytics;

namespace WinUI.Services;

public sealed class ClientDashboardService : IClientDashboardService
{
    private const string ApiBaseAddress = "https://localhost:7197";
    private const string Route = "api/client";
    private readonly HttpClient httpClient;

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync(int clientId)
    {
        var summary = await this.httpClient.GetFromJsonAsync<DashboardSummary>($"{ApiBaseAddress}/{Route}/{clientId}/dashboard-summary");
        return summary ?? new DashboardSummary();
    }

    public async Task<IReadOnlyList<ConsistencyWeekBucket>> GetConsistencyLastFourWeeksAsync(int clientId)
    {
        var buckets = await this.httpClient.GetFromJsonAsync<List<ConsistencyWeekBucket>>($"{ApiBaseAddress}/{Route}/{clientId}/consistency-last-four-weeks");
        return buckets ?? new List<ConsistencyWeekBucket>();
    }

    public async Task<WorkoutHistoryPageResult> GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize)
    {
        var result = await this.httpClient.GetFromJsonAsync<WorkoutHistoryPageResult>($"{ApiBaseAddress}/{Route}/{clientId}/workout-history");
        return result ?? new WorkoutHistoryPageResult();
    }

    public async Task<IReadOnlyList<AchievementDataTransferObject>> GetRecentAchievementsAsync(int clientId)
    {
        var achievements = await this.httpClient.GetFromJsonAsync<List<AchievementDataTransferObject>>($"{ApiBaseAddress}/{Route}/{clientId}/achievements");
        return achievements ?? new List<AchievementDataTransferObject>();
    }
}
