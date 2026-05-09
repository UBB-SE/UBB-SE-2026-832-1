using System.Net.Http;
using System.Threading.Tasks;

namespace WinUI.Services;

public interface IWorkoutLogsService
{
    Task GetWorkoutLogsAsync();
}

public class WorkoutLogsService : IWorkoutLogsService
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/workoutlogs";

    public WorkoutLogsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetWorkoutLogsAsync()
    {
        var response = await httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}
