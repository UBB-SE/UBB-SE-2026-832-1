namespace WinUI.Services;

using ClassLibrary.DTOs;
using System.Net.Http.Json;

public sealed class ClientProfileService : IClientProfileService
{
    private const string ROUTE = "api/client-profile";
    private readonly HttpClient httpClient;

    public ClientProfileService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ClientProfileSnapshotDataTransferObject> GetClientProfileAsync(int clientId)
    {
        return await httpClient.GetFromJsonAsync<ClientProfileSnapshotDataTransferObject>($"{ROUTE}/{clientId}")
            ?? throw new InvalidOperationException("Empty response from client profile endpoint.");
    }

    public async Task<ClientProfileSnapshotDataTransferObject> SyncNutritionAsync(int clientId, NutritionSyncRequestDataTransferObject request)
    {
        var response = await httpClient.PostAsJsonAsync($"{ROUTE}/{clientId}/sync-nutrition", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ClientProfileSnapshotDataTransferObject>()
            ?? throw new InvalidOperationException("Empty response from nutrition sync endpoint.");
    }
}
