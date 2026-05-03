using ClassLibrary.DTOs;

namespace WebApi.IServices
{
    public interface IDailyLogService
    {
        Task<UserDataDto?> GetCurrentUserNutritionTargetsAsync(int userId);
        Task<DailyLogTotalsDto> GetCurrentWeekTotalsAsync(int userId);
        Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForAutocompleteAsync();
        Task<double> GetTodayBurnedCaloriesAsync(int userId);
        Task<DailyLogTotalsDto> GetTodayTotalsAsync(int userId);
        Task<bool> HasAnyLogsAsync(int userId);
        Task LogFoodItemAsync(int userId, LogMealRequestDto request);
        Task<IReadOnlyList<FoodItemDto>> SearchFoodItemsAsync(string? searchTerm);
    }
}