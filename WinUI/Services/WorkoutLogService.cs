using System.Net.Http.Json;
using ClassLibrary.DTOs;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class WorkoutLogService : IWorkoutLogService
{
    private readonly HttpClient httpClient;

    public WorkoutLogService(HttpClient httpClient)
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
