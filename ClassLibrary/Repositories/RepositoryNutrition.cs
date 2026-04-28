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
        return plan.NutritionPlanId;
    }

    public async Task InsertMealAsync(Meal meal, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        meal.NutritionPlan = plan;
        await dbContext.Meals.AddAsync(meal, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        bool exists = await dbContext.ClientNutritionPlans
            .AnyAsync(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId && clientNutritionPlan.NutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken);

        if (exists)
        {
            return;
        }

        var client = await dbContext.Clients
            .FirstOrDefaultAsync(client => client.ClientId == clientId, cancellationToken)
            ?? throw new InvalidOperationException($"Client {clientId} not found.");

        var plan = await dbContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken)
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
            .Where(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId)
            .Select(clientNutritionPlan => clientNutritionPlan.NutritionPlan)
            .Include(nutritionPlan => nutritionPlan.Meals)
            .OrderBy(nutritionPlan => nutritionPlan.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Meals
            .AsNoTracking()
            .Where(meal => meal.NutritionPlan.NutritionPlanId == nutritionPlanId)
            .OrderBy(meal => meal.MealId)
            .ToListAsync(cancellationToken);
    }
}
