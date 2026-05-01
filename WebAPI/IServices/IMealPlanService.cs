using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface IMealPlanService
{
    Task<MealPlanDto?> GetByIdAsync(int id);

    Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId);

    Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId);

    (int TotalCalories, int TotalProtein, int TotalCarbohydrates, int TotalFat) CalculateTotalNutrition(IReadOnlyList<FoodItemDto> foodItems);

    bool ValidateMealPlan(IReadOnlyList<FoodItemDto> foodItems, int targetCalories, int targetProtein, int targetCarbohydrates, int targetFat, double tolerance = 0.10);
}
