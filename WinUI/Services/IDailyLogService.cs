using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IDailyLogService
{
    Task<UserDataDto?> GetNutritionTargetsAsync(int userId);
    Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId);
    Task<DailyLogTotalsDto> GetWeekTotalsAsync(int userId);
    Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForAutocompleteAsync();
    Task<double> GetTodayBurnedCaloriesAsync(int userId);
    Task<double> GetWeekBurnedCaloriesAsync(int userId);
    Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId);
    Task<bool> HasAnyLogsAsync(int userId);
    Task LogFoodItemAsync(int userId, LogMealRequestDto request);
    Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm);
}
