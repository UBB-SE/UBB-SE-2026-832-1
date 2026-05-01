using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface INutritionRepository
{
    Task<int> InsertNutritionPlanAsync(NutritionPlan plan);

    Task InsertMealAsync(Meal meal, int nutritionPlanId);

    Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId);

    Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId);

    Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId);
}
