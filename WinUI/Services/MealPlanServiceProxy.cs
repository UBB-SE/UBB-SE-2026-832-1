using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class MealPlanServiceProxy : IMealPlanServiceProxy
{
    private readonly HttpClient httpClient;

    public MealPlanServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<MealPlanDto?> GetByIdAsync(int id)
    {
        return await this.httpClient.GetFromJsonAsync<MealPlanDto>($"{ApiBaseUrl.BASE_URL}/api/mealplans/{id}");
    }

    public async Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId)
    {
        var mealPlans = await this.httpClient.GetFromJsonAsync<List<MealPlanDto>>($"{ApiBaseUrl.BASE_URL}/api/mealplans/user/{userId}");
        return mealPlans ?? [];
    }

    public async Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId)
    {
        var response = await this.httpClient.PostAsync($"{ApiBaseUrl.BASE_URL}/api/mealplans/{mealPlanId}/fooditems/{foodItemId}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId)
    {
        var response = await this.httpClient.DeleteAsync($"{ApiBaseUrl.BASE_URL}/api/mealplans/{mealPlanId}/fooditems/{foodItemId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId)
    {
        var foodItems = await this.httpClient.GetFromJsonAsync<List<FoodItemDto>>($"{ApiBaseUrl.BASE_URL}/api/mealplans/{mealPlanId}/fooditems");
        return foodItems ?? [];
    }
}
