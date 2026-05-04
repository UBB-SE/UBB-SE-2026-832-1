using System.Net.Http;
using System.Threading.Tasks;

namespace WinUI.Services;

public interface IMealSearchService
{
    Task SearchMealsAsync();
}

public class MealSearchService : IMealSearchService
{
    private readonly HttpClient httpClient;
    private readonly string route = "api/mealsearch";

    public MealSearchService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task SearchMealsAsync()
    {
        var response = await httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
    }
}