using System;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;

namespace WebAPI.Services;

public sealed class MealPlanService : IMealPlanService
{
    private const double DEFAULT_TOLERANCE = 0.10;

    private readonly IMealPlanRepository mealPlanRepository;

    public MealPlanService(IMealPlanRepository mealPlanRepository)
    {
        this.mealPlanRepository = mealPlanRepository;
    }

    public async Task<MealPlanDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mealPlan = await this.mealPlanRepository.GetByIdAsync(id, cancellationToken);

        if (mealPlan is null)
        {
            return null;
        }

        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(id, cancellationToken);

        return new MealPlanDto
        {
            MealPlanId = mealPlan.MealPlanId,
            UserId = mealPlan.UserId,
            CreatedAt = mealPlan.CreatedAt,
            GoalType = mealPlan.GoalType,
            FoodItems = foodItems.Select(foodItem => new FoodItemDto
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
            }).ToList(),
        };
    }

    public async Task<IReadOnlyList<MealPlanDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var mealPlans = await this.mealPlanRepository.GetByUserIdAsync(userId, cancellationToken);

        var results = new List<MealPlanDto>(mealPlans.Count);
        foreach (var mealPlan in mealPlans)
        {
            var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(mealPlan.MealPlanId, cancellationToken);

            results.Add(new MealPlanDto
            {
                MealPlanId = mealPlan.MealPlanId,
                UserId = mealPlan.UserId,
                CreatedAt = mealPlan.CreatedAt,
                GoalType = mealPlan.GoalType,
                FoodItems = foodItems.Select(foodItem => new FoodItemDto
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
                }).ToList(),
            });
        }

        return results;
    }

    public async Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default)
    {
        await this.mealPlanRepository.AddFoodItemToPlanAsync(mealPlanId, foodItemId, cancellationToken);
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default)
    {
        await this.mealPlanRepository.RemoveFoodItemFromPlanAsync(mealPlanId, foodItemId, cancellationToken);
    }

    public async Task<IReadOnlyList<FoodItemDto>> GetFoodItemsForPlanAsync(int mealPlanId, CancellationToken cancellationToken = default)
    {
        var foodItems = await this.mealPlanRepository.GetFoodItemsForPlanAsync(mealPlanId, cancellationToken);

        return foodItems.Select(foodItem => new FoodItemDto
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
        }).ToList();
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
}
