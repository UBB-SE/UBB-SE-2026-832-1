namespace WinUI.Services;

public class ClientProfileService : IClientProfileService
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/client-profile";

    public ClientProfileService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetClientProfile(int clientId)
    {
        var response = await httpClient.GetAsync($"{route}/{clientId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task SyncNutrition(int clientId)
    {
        var response = await httpClient.PostAsync($"{route}/{clientId}/sync-nutrition", content: null);
        response.EnsureSuccessStatusCode();
    }
}
