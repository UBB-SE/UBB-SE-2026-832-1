using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class DailyLogService : IDailyLogService
{
    private readonly IDailyLogServiceProxy serviceProxy;

    public DailyLogService(IDailyLogServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId)
    {
        return this.serviceProxy.GetTodayTotalsAsync(userId);
    }

    public Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId)
    {
        return this.serviceProxy.GetCurrentWeekTotalsAsync(userId);
    }

    public Task<UserDataDto?> GetNutritionTargetsAsync(int userId)
    {
        return this.serviceProxy.GetNutritionTargetsAsync(userId);
    }

    public Task<double> GetTodayBurnedCaloriesAsync(int userId)
    {
        return this.serviceProxy.GetTodayBurnedCaloriesAsync(userId);
    }

    public Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm)
    {
        return this.serviceProxy.SearchFoodItemsAsync(searchTerm);
    }

    public Task LogFoodItemAsync(int userId, LogMealRequestDto request)
    {
        return this.serviceProxy.LogFoodItemAsync(userId, request);
    }
}
