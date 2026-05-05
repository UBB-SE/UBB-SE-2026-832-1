using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IDailyLogService
{
    Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId);

    Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId);

    Task<UserDataDto?> GetNutritionTargetsAsync(int userId);

    Task<double> GetTodayBurnedCaloriesAsync(int userId);

    Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm);

    Task LogFoodItemAsync(int userId, LogMealRequestDto request);
}
