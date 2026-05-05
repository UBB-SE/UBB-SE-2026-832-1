using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class DailyLogServiceProxy : IDailyLogServiceProxy
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";
    private readonly HttpClient httpClient;

    public DailyLogServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId)
    {
        var totals = await this.httpClient.GetFromJsonAsync<DailyLogTotalsDto>(
            $"{API_BASE_ADDRESS}/api/dailylog/user/{userId}/today");
        return totals ?? new DailyLogTotalsDto();
    }

    public async Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId)
    {
        var totals = await this.httpClient.GetFromJsonAsync<DailyLogTotalsDto>(
            $"{API_BASE_ADDRESS}/api/dailylog/user/{userId}/week");
        return totals ?? new DailyLogTotalsDto();
    }

    public async Task<UserDataDto?> GetNutritionTargetsAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<UserDataDto>(
            $"{API_BASE_ADDRESS}/api/dailylog/user/{userId}/targets");
    }

    public async Task<double> GetTodayBurnedCaloriesAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<double>(
            $"{API_BASE_ADDRESS}/api/dailylog/user/{userId}/burned-calories");
    }

    public async Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm)
    {
        var queryString = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : $"?searchTerm={Uri.EscapeDataString(searchTerm)}";
        var foodItems = await this.httpClient.GetFromJsonAsync<List<FoodItemDto>>(
            $"{API_BASE_ADDRESS}/api/dailylog/fooditems/search{queryString}");
        return foodItems ?? [];
    }

    public async Task LogFoodItemAsync(int userId, LogMealRequestDto request)
    {
        var response = await this.httpClient.PostAsJsonAsync(
            $"{API_BASE_ADDRESS}/api/dailylog/user/{userId}/log", request);
        response.EnsureSuccessStatusCode();
    }
}
