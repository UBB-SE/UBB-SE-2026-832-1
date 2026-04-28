using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;


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
        var plan = await dbContext.NutritionPlans
            .FirstOrDefaultAsync(np => np.PlanId == nutritionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        meal.NutritionPlan = plan;
        await dbContext.Meals.AddAsync(meal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        bool exists = await dbContext.ClientNutritionPlans
            .AnyAsync(cnp => cnp.Client.Id == clientId && cnp.NutritionPlan.PlanId == nutritionPlanId, cancellationToken);

        if (exists)
        {
            return;
        }

        var client = await dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken)
            ?? throw new InvalidOperationException($"Client {clientId} not found.");

        var plan = await dbContext.NutritionPlans
            .FirstOrDefaultAsync(np => np.PlanId == nutritionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        dbContext.ClientNutritionPlans.Add(new ClientNutritionPlan
        {
            Client = client,
            NutritionPlan = plan,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ClientNutritionPlans
            .AsNoTracking()
            .Where(cnp => cnp.Client.Id == clientId)
            .Select(cnp => cnp.NutritionPlan)
            .Include(np => np.Meals)
            .OrderBy(np => np.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Meals
            .AsNoTracking()
            .Where(m => m.NutritionPlan.PlanId == nutritionPlanId)
            .OrderBy(m => m.MealId)
            .ToListAsync(cancellationToken);
    }
}
