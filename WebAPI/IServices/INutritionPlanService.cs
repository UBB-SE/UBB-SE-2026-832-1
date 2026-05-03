using ClassLibrary.DTOs;

namespace WebAPI.IServices;

public interface INutritionPlanService
{
    Task<NutritionPlanDataTransferObject> CreatePlanAsync(int clientId);

    Task<IReadOnlyList<NutritionPlanDataTransferObject>> GetPlansForClientAsync(int clientId);

    Task AddMealToPlanAsync(int planId, MealDataTransferObject meal);
}
