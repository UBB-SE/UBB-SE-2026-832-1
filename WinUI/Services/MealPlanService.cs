using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class MealPlanService : IMealPlanService
{
    private readonly HttpClient httpClient;

    public MealPlanService(HttpClient httpClient)
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

    public async Task<MealPlanDto?> GetTodaysMealPlanAsync(int userId)
    {
        var response = await this.httpClient.GetAsync($"{ApiBaseUrl.BASE_URL}/api/mealplans/user/{userId}/today");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MealPlanDto>();
    }

    public async Task<int> GenerateMealPlanAsync(int userId)
    {
        var response = await this.httpClient.PostAsync($"{ApiBaseUrl.BASE_URL}/api/mealplans/user/{userId}/generate", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<string> GetUserGoalAsync(int userId)
    {
        var goal = await this.httpClient.GetFromJsonAsync<string>($"{ApiBaseUrl.BASE_URL}/api/mealplans/user/{userId}/goal");
        return goal ?? "maintenance";
    }

    public async Task SaveMealsToDailyLogAsync(int mealPlanId, int userId)
    {
        var response = await this.httpClient.PostAsync($"{ApiBaseUrl.BASE_URL}/api/mealplans/{mealPlanId}/log/{userId}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task SaveMealToDailyLogAsync(int foodItemId, int calories, int userId)
    {
        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/log",
            new { MealId = foodItemId, Calories = calories });
        response.EnsureSuccessStatusCode();
    }
}
