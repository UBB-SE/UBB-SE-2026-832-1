using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class MealPlanService : IMealPlanService
{
    private readonly IMealPlanServiceProxy serviceProxy;

    public MealPlanService(IMealPlanServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<MealPlanDto?> GetByIdAsync(int id)
    {
        return this.serviceProxy.GetByIdAsync(id);
    }

    public Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId)
    {
        return this.serviceProxy.GetByUserIdAsync(userId);
    }

    public Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId)
    {
        return this.serviceProxy.AddFoodItemToPlanAsync(mealPlanId, foodItemId);
    }

    public Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId)
    {
        return this.serviceProxy.RemoveFoodItemFromPlanAsync(mealPlanId, foodItemId);
    }

    public Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId)
    {
        return this.serviceProxy.GetFoodItemsForPlanAsync(mealPlanId);
    }
}
