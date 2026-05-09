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

    public Task<MealPlanDto?> GetTodaysMealPlanAsync(int userId)
    {
        return this.serviceProxy.GetTodaysMealPlanAsync(userId);
    }

    public Task<int> GenerateMealPlanAsync(int userId)
    {
        return this.serviceProxy.GenerateMealPlanAsync(userId);
    }

    public Task<string> GetUserGoalAsync(int userId)
    {
        return this.serviceProxy.GetUserGoalAsync(userId);
    }

    public Task SaveMealsToDailyLogAsync(int mealPlanId, int userId)
    {
        return this.serviceProxy.SaveMealsToDailyLogAsync(mealPlanId, userId);
    }

    public Task SaveMealToDailyLogAsync(int foodItemId, int calories, int userId)
    {
        return this.serviceProxy.SaveMealToDailyLogAsync(foodItemId, calories, userId);
    }
}
