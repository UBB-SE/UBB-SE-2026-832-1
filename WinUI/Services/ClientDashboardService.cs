namespace WinUI.Services;

public class ClientDashboardService : IClientDashboardService
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/client-dashboard";

    public ClientDashboardService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetDashboardSummary(int clientId)
    {
        var response = await httpClient.GetAsync($"{route}/summary/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetConsistencyLastFourWeeks(int clientId)
    {
        var response = await httpClient.GetAsync($"{route}/consistency/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetWorkoutHistoryPage(int clientId, int page, int pageSize)
    {
        var response = await httpClient.GetAsync($"{route}/history/{clientId}?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GetRecentAchievements(int clientId)
    {
        var response = await httpClient.GetAsync($"{route}/achievements/{clientId}");
        response.EnsureSuccessStatusCode();
    }
}
