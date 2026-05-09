using System;
using System.Linq;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebApi.IServices;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class MealPlanService : IMealPlanService
{
    private const double DEFAULT_TOLERANCE = 0.10;

    private readonly IMealPlanRepository mealPlanRepository;
    private readonly IFoodItemRepository foodItemRepository;
    private readonly IUserService userService;
    private readonly IDailyLogService dailyLogService;

    public MealPlanService(
        IMealPlanRepository mealPlanRepository,
        IFoodItemRepository foodItemRepository,
        IUserService userService,
        IDailyLogService dailyLogService)
    {
        this.mealPlanRepository = mealPlanRepository;
        this.foodItemRepository = foodItemRepository;
        this.userService = userService;
        this.dailyLogService = dailyLogService;
    }

    public async Task<MealPlanDto?> GetByIdAsync(int id)
    {
        var mealPlan = await this.mealPlanRepository.GetByIdAsync(id);

        if (mealPlan is null)
        {
            return null;
        }

        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(id);

        return MapToMealPlanDto(mealPlan, foodItems);
    }

    public async Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId)
    {
        var mealPlans = await this.mealPlanRepository.GetByUserIdAsync(userId);

        var results = new List<MealPlanDto>(mealPlans.Count);
        foreach (var mealPlan in mealPlans)
        {
            var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(mealPlan.MealPlanId);
            results.Add(MapToMealPlanDto(mealPlan, foodItems));
        }

        return results;
    }

    public async Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId)
    {
        await this.mealPlanRepository.AddFoodItemToPlanAsync(mealPlanId, foodItemId);
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId)
    {
        await this.mealPlanRepository.RemoveFoodItemFromPlanAsync(mealPlanId, foodItemId);
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId)
    {
        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(mealPlanId);
        return MapToFoodItemDtos(foodItems);
    }

    public (int TotalCalories, int TotalProtein, int TotalCarbohydrates, int TotalFat) CalculateTotalNutrition(IReadOnlyList<FoodItemDto> foodItems)
    {
        if (foodItems is null || foodItems.Count == 0)
        {
            return (0, 0, 0, 0);
        }

        int calories = 0, protein = 0, carbohydrates = 0, fat = 0;

        foreach (var foodItem in foodItems)
        {
            calories += foodItem.Calories;
            protein += foodItem.Protein;
            carbohydrates += foodItem.Carbohydrates;
            fat += foodItem.Fat;
        }

        return (calories, protein, carbohydrates, fat);
    }

    public bool ValidateMealPlan(IReadOnlyList<FoodItemDto> foodItems, int targetCalories, int targetProtein, int targetCarbohydrates, int targetFat, double tolerance = DEFAULT_TOLERANCE)
    {
        var (totalCalories, totalProtein, totalCarbohydrates, totalFat) = CalculateTotalNutrition(foodItems);

        return
            Math.Abs(totalCalories - targetCalories) <= targetCalories * tolerance &&
            Math.Abs(totalProtein - targetProtein) <= targetProtein * tolerance &&
            Math.Abs(totalCarbohydrates - targetCarbohydrates) <= targetCarbohydrates * tolerance &&
            Math.Abs(totalFat - targetFat) <= targetFat * tolerance;
    }

    private static FoodItemDto MapToFoodItemDto(FoodItem foodItem)
    {
        return new FoodItemDto
        {
            FoodItemId = foodItem.FoodItemId,
            Name = foodItem.Name,
            Calories = foodItem.Calories,
            Carbohydrates = foodItem.Carbohydrates,
            Fat = foodItem.Fat,
            Protein = foodItem.Protein,
            IsVegan = foodItem.IsVegan,
            IsKeto = foodItem.IsKeto,
            IsGlutenFree = foodItem.IsGlutenFree,
            IsLactoseFree = foodItem.IsLactoseFree,
            IsNutFree = foodItem.IsNutFree,
            Description = foodItem.Description,
            ImageUrl = foodItem.ImageUrl,
        };
    }

    public async Task<MealPlanDto?> GetTodaysMealPlanAsync(int userId)
    {
        var todaysPlan = await this.mealPlanRepository.GetTodaysMealPlanAsync(userId);
        if (todaysPlan is null)
        {
            return null;
        }

        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(todaysPlan.MealPlanId);
        return new MealPlanDto
        {
            MealPlanId = todaysPlan.MealPlanId,
            UserId = userId,
            CreatedAt = todaysPlan.CreatedAt,
            GoalType = todaysPlan.GoalType,
            FoodItems = MapToFoodItemDtos(foodItems),
        };
    }

    public async Task<int> GenerateMealPlanAsync(int userId)
    {
        var userData = await this.userService.GetUserDataAsync(userId);
        string goalType = userData?.Goal ?? "maintenance";

        var allFoodItems = await this.foodItemRepository.GetAllAsync();
        var selected = allFoodItems.Take(3).ToList();

        int planId = await this.mealPlanRepository.CreateMealPlanAsync(userId, goalType);

        foreach (var item in selected)
        {
            await this.mealPlanRepository.AddFoodItemToPlanAsync(planId, item.FoodItemId);
        }

        return planId;
    }

    public async Task<string> GetUserGoalAsync(int userId)
    {
        var userData = await this.userService.GetUserDataAsync(userId);
        return userData?.Goal ?? "maintenance";
    }

    public async Task SaveMealsToDailyLogAsync(int mealPlanId, int userId)
    {
        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(mealPlanId);
        foreach (var item in foodItems)
        {
            await this.dailyLogService.LogFoodItemAsync(userId, new LogMealRequestDto { MealId = item.FoodItemId, Calories = item.Calories });
        }
    }

    private static List<FoodItemDto> MapToFoodItemDtos(IReadOnlyList<FoodItem> foodItems)
    {
        return foodItems.Select(MapToFoodItemDto).ToList();
    }

    private static MealPlanDto MapToMealPlanDto(MealPlan mealPlan, IReadOnlyList<FoodItem> foodItems)
    {
        return new MealPlanDto
        {
            MealPlanId = mealPlan.MealPlanId,
            UserId = mealPlan.User.UserId,
            CreatedAt = mealPlan.CreatedAt,
            GoalType = mealPlan.GoalType,
            FoodItems = MapToFoodItemDtos(foodItems),
        };
    }
}
