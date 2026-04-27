using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IRepositoryNutrition
{
    Task<int> InsertNutritionPlanAsync(NutritionPlan plan, CancellationToken cancellationToken = default);

    Task InsertMealAsync(Meal meal, int nutritionPlanId, CancellationToken cancellationToken = default);

    Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId, CancellationToken cancellationToken = default);
}
