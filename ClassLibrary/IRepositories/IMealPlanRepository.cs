using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MealPlan>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task AddAsync(MealPlan entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(MealPlan entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItem>> GetFoodItemsForPlanAsync(int mealPlanId, CancellationToken cancellationToken = default);

    Task AddIngredientToFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default);

    Task RemoveIngredientFromFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetIngredientIdsForMealPlanAsync(int mealPlanId, CancellationToken cancellationToken = default);
}
