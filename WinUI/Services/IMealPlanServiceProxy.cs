using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IMealPlanServiceProxy
{
    Task<MealPlanDto?> GetByIdAsync(int id);

    Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId);

    Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId);

    Task<MealPlanDto?> GetTodaysMealPlanAsync(int userId);

    Task<int> GenerateMealPlanAsync(int userId);

    Task<string> GetUserGoalAsync(int userId);

    Task SaveMealsToDailyLogAsync(int mealPlanId, int userId);

    Task SaveMealToDailyLogAsync(int foodItemId, int calories, int userId);
}
