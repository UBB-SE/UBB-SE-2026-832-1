using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdAsync(int id);

    Task<IReadOnlyList<MealPlan>> GetByUserIdAsync(int userId);

    Task AddAsync(MealPlan entity);

    Task UpdateAsync(MealPlan entity);

    Task DeleteAsync(int id);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId);

    Task<IReadOnlyList<FoodItem>> GetFoodItemsForPlanAsync(int mealPlanId);

    Task AddIngredientToFoodItemAsync(int foodItemId, int ingredientId);

    Task RemoveIngredientFromFoodItemAsync(int foodItemId, int ingredientId);

    Task<IReadOnlyList<int>> GetIngredientIdsForMealPlanAsync(int mealPlanId);
}
