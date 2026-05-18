using System.Net.Http;
using System.Threading.Tasks;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public class MealSearchProxy : IMealSearchProxy
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/mealsearch";

    public MealSearchProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task SearchMealsAsync()
    {
        var response = await httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}


