using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IDailyLogRepository
{
    Task AddAsync(DailyLog log);
    Task<DailyLog?> GetNutritionTotalsForRangeAsync(int userId, DateTime startInclusive, DateTime endExclusive);
    Task<bool> HasAnyLogsAsync(int userId);
    Task<bool> HasFoodItemLoggedTodayAsync(int userId, int foodItemId);
}
