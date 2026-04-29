using ClassLibrary.DTOs;

namespace WebAPI.Services;

public interface IMealPlanService
{
    Task<MealPlanDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default);

    Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId, CancellationToken cancellationToken = default);
}
