using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class DailyLogService : IDailyLogService
{
    private readonly HttpClient httpClient;

    public DailyLogService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> HasAnyLogsAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<bool>($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/has-logs");
    }

    public async Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId)
    {
        var totals = await this.httpClient.GetFromJsonAsync<DailyLogTotalsDto>($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/today");
        return totals ?? new DailyLogTotalsDto();
    }

    public async Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId)
    {
        var totals = await this.httpClient.GetFromJsonAsync<DailyLogTotalsDto>($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/week");
        return totals ?? new DailyLogTotalsDto();
    }

    public async Task<UserDataDto?> GetNutritionTargetsAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<UserDataDto>($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/targets");
    }

    public async Task<double> GetTodayBurnedCaloriesAsync(int userId)
    {
        return await this.httpClient.GetFromJsonAsync<double>($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/burned-calories");
    }

    public async Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm)
    {
        var term = searchTerm ?? string.Empty;
        var items = await this.httpClient.GetFromJsonAsync<List<FoodItemDto>>($"{ApiBaseUrl.BASE_URL}/api/dailylog/fooditems/search?searchTerm={term}");

        return items ?? [];
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForAutocompleteAsync()
    {
        var items = await this.httpClient.GetFromJsonAsync<List<FoodItemDto>>($"{ApiBaseUrl.BASE_URL}/api/dailylog/fooditems/autocomplete");
        return items ?? [];
    }

    public async Task LogFoodItemAsync(int userId, LogMealRequestDto request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{ApiBaseUrl.BASE_URL}/api/dailylog/user/{userId}/log", request);

        response.EnsureSuccessStatusCode();
    }
}
