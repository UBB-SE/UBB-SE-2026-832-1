using ClassLibrary.DTOs;

namespace WinUI.Services;

public interface IMealPlanService
{
    Task<MealPlanDto?> GetByIdAsync(int id);

    Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId);

    Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId);
}
