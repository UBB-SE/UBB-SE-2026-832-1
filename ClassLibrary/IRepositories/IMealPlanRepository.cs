using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IMealPlanRepository
{
    Task AddAsync(MealPlan entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GeneratePersonalizedDailyMealPlanAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MealPlan>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MealPlan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IngredientViewModel>> GetIngredientsForMealAsync(int mealId, CancellationToken cancellationToken = default);

    Task<MealPlan?> GetLatestMealPlanAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Meal>> GetMealsForMealPlanAsync(int mealPlanId, CancellationToken cancellationToken = default);

    Task<MealPlan?> GetTodaysMealPlanAsync(int userId, CancellationToken cancellationToken = default);

    Task SaveMealsToDailyLogAsync(int userId, IReadOnlyList<Meal> meals, CancellationToken cancellationToken = default);

    Task SaveMealToDailyLogAsync(int userId, int mealId, int calories, CancellationToken cancellationToken = default);

    Task UpdateAsync(MealPlan entity, CancellationToken cancellationToken = default);
}
