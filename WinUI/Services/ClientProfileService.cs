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

    public async Task SyncNutritionAsync(NutritionSyncRequestDataTransferObject request)
    {
        var response = await System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync(
            httpClient,
            "api/client/sync-nutrition",
            request);
        response.EnsureSuccessStatusCode();
    }
}
