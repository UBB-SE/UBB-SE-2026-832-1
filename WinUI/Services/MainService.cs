using System.Net.Http;
using System.Threading.Tasks;

namespace WinUI.Services;

public interface IMainService
{
    Task GetMainDataAsync();
}

public class MainService : IMainService
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/main";

    public MainService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetMainDataAsync()
    {
        var response = await httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}