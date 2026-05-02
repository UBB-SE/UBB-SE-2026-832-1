namespace WinUI.Services;

public sealed class ClientDashboardService : IClientDashboardService
{
    private const string ROUTE = "api/client-dashboard";
    private readonly HttpClient httpClient;

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetDashboardSummaryAsync(int clientId)
    {
        var response = await httpClient.GetAsync($"{ROUTE}/summary/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetConsistencyLastFourWeeksAsync(int clientId)
    {
        var response = await httpClient.GetAsync($"{ROUTE}/consistency/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetWorkoutHistoryPageAsync(int clientId, int page, int pageSize)
    {
        var response = await httpClient.GetAsync($"{ROUTE}/history/{clientId}?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetRecentAchievementsAsync(int clientId)
    {
        var response = await httpClient.GetAsync($"{ROUTE}/achievements/{clientId}");
        response.EnsureSuccessStatusCode();
    }
}
