using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

// NON-REPOSITORY LOGIC REMOVED:
// The original implementation contained SaveNutritionPlanForClient(NutritionPlan plan, int clientId)
// which orchestrated three sequential operations: InsertNutritionPlan, InsertMeal (for each meal),
// and AssignNutritionPlanToClient. This is pure orchestration and does not belong in a data-access
// layer. It must be reimplemented in a NutritionService that depends on IRepositoryNutrition.
public sealed class RepositoryNutrition(AppDbContext dbContext) : IRepositoryNutrition
{
    public async Task<int> InsertNutritionPlanAsync(NutritionPlan plan, CancellationToken cancellationToken = default)
    {
        await dbContext.NutritionPlans.AddAsync(plan, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return plan.PlanId;
    }

    public async Task InsertMealAsync(Meal meal, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        meal.NutritionPlanId = nutritionPlanId;
        await dbContext.Meals.AddAsync(meal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        bool exists = await dbContext.ClientNutritionPlans
            .AnyAsync(cnp => cnp.ClientId == clientId && cnp.NutritionPlanId == nutritionPlanId, cancellationToken);

        if (exists)
        {
            return;
        }

        dbContext.ClientNutritionPlans.Add(new ClientNutritionPlan
        {
            ClientId = clientId,
            NutritionPlanId = nutritionPlanId,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ClientNutritionPlans
            .AsNoTracking()
            .Where(cnp => cnp.ClientId == clientId)
            .Select(cnp => cnp.NutritionPlan)
            .Include(np => np.Meals)
            .OrderBy(np => np.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Meals
            .AsNoTracking()
            .Where(m => m.NutritionPlanId == nutritionPlanId)
            .OrderBy(m => m.MealId)
            .ToListAsync(cancellationToken);
    }
}
