using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class WorkoutLogServiceProxy : IWorkoutLogServiceProxy
{
    private readonly HttpClient httpClient;

    public WorkoutLogServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<WorkoutLogDataTransferObject>> GetWorkoutHistoryAsync(int clientId)
    {
        var history = await this.httpClient.GetFromJsonAsync<List<WorkoutLogDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/client/{clientId}/workout-history");
        return history ?? [];
    }

    public async Task<double> GetClientWeightAsync(int clientId)
    {
        var userData = await this.httpClient.GetFromJsonAsync<UserDataDto>(
            $"{ApiBaseUrl.BASE_URL}/api/users/{clientId}/data");
        return userData?.Weight ?? 0;
    }
}
