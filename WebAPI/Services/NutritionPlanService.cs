using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services;

public sealed class NutritionPlanService : INutritionPlanService
{
    private const int DEFAULT_PLAN_DURATION_DAYS = 30;

    private readonly INutritionRepository nutritionRepository;

    public NutritionPlanService(INutritionRepository nutritionRepository)
    {
        this.nutritionRepository = nutritionRepository;
    }

    public async Task<NutritionPlanDataTransferObject> CreatePlanAsync(int clientId)
    {
        var plan = new NutritionPlan
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(DEFAULT_PLAN_DURATION_DAYS),
        };

        int planId = await this.nutritionRepository.InsertNutritionPlanAsync(plan);
        await this.nutritionRepository.AssignNutritionPlanToClientAsync(clientId, planId);

        plan.NutritionPlanId = planId;
        return MapToDto(plan);
    }

    public async Task<IReadOnlyList<NutritionPlanDataTransferObject>> GetPlansForClientAsync(int clientId)
    {
        var plans = await this.nutritionRepository.GetNutritionPlansForClientAsync(clientId);
        var results = new List<NutritionPlanDataTransferObject>(plans.Count);

        foreach (var plan in plans)
        {
            var meals = plan.Meals;
            results.Add(MapToDto(plan, meals));
        }

        return results;
    }

    public async Task AddMealToPlanAsync(int planId, MealDataTransferObject meal)
    {
        var newMeal = new Meal
        {
            Name = meal.Name,
            Ingredients = new List<string>(meal.Ingredients),
            Instructions = meal.Instructions,
        };

        await this.nutritionRepository.InsertMealAsync(newMeal, planId);
    }

    private static NutritionPlanDataTransferObject MapToDto(NutritionPlan plan, IReadOnlyList<Meal>? meals = null)
    {
        return new NutritionPlanDataTransferObject
        {
            NutritionPlanId = plan.NutritionPlanId,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            Meals = (meals ?? plan.Meals ?? new List<Meal>()).Select(MapMealToDto).ToList(),
        };
    }

    private static MealDataTransferObject MapMealToDto(Meal meal)
    {
        return new MealDataTransferObject
        {
            MealId = meal.MealId,
            Name = meal.Name,
            Ingredients = new List<string>(meal.Ingredients),
            Instructions = meal.Instructions,
        };
    }
}
