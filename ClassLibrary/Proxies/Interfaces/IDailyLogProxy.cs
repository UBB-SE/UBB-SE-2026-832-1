using ClassLibrary.DTOs;

namespace ClassLibrary.Proxies.Interfaces;

public interface IDailyLogProxy
{
    Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId);

    Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId);

    Task<UserDataDto?> GetNutritionTargetsAsync(int userId);

    Task<double> GetTodayBurnedCaloriesAsync(int userId);

    Task<double> GetWeekBurnedCaloriesAsync(int userId);

    Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm);

    Task LogFoodItemAsync(int userId, LogMealRequestDto request);
}



