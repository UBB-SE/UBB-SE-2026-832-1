using System.Net.Http;
using System.Threading.Tasks;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public class MainProxy : IMainProxy
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/main";

    public MainProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task GetMainDataAsync()
    {
        var response = await httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}


