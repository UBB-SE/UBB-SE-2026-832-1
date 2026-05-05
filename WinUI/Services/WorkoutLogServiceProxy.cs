using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class WorkoutLogServiceProxy : IWorkoutLogServiceProxy
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";
    private readonly HttpClient httpClient;

    public WorkoutLogServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        var history = await this.httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>(
            $"{API_BASE_ADDRESS}/api/client/{clientId}/workout-history");
        return history ?? [];
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        var userData = await this.httpClient.GetFromJsonAsync<UserDataDto>(
            $"{API_BASE_ADDRESS}/api/users/{clientId}/data");
        return userData?.Weight ?? 0;
    }
}
