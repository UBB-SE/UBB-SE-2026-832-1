namespace WinUI.Services;

public sealed class ClientProfileService : IClientProfileService
{
    private const string ROUTE = "api/client-profile";
    private readonly HttpClient httpClient;

    public ClientProfileService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetClientProfileAsync(int clientId)
    {
        var response = await httpClient.GetAsync($"{ROUTE}/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task SyncNutritionAsync(int clientId)
    {
        var response = await httpClient.PostAsync($"{ROUTE}/{clientId}/sync-nutrition", content: null);
        response.EnsureSuccessStatusCode();
    }
}
