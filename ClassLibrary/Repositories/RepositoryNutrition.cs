using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;


public sealed class RepositoryNutrition : IRepositoryNutrition
{
    private readonly AppDbContext databaseContext;

    public RepositoryNutrition(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<int> InsertNutritionPlanAsync(NutritionPlan plan, CancellationToken cancellationToken = default)
    {
        await this.databaseContext.NutritionPlans.AddAsync(plan, cancellationToken);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
        return plan.NutritionPlanId;
    }

    public async Task InsertMealAsync(Meal meal, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        var plan = await this.databaseContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        meal.NutritionPlan = plan;
        await this.databaseContext.Meals.AddAsync(meal, cancellationToken);
        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        bool exists = await this.databaseContext.ClientNutritionPlans
            .AnyAsync(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId && clientNutritionPlan.NutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken);

        if (exists)
        {
            return;
        }

        var client = await this.databaseContext.Clients
            .FirstOrDefaultAsync(client => client.ClientId == clientId, cancellationToken)
            ?? throw new InvalidOperationException($"Client {clientId} not found.");

        var plan = await this.databaseContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        this.databaseContext.ClientNutritionPlans.Add(new ClientNutritionPlan
        {
            Client = client,
            NutritionPlan = plan,
        });

        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.ClientNutritionPlans
            .AsNoTracking()
            .Where(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId)
            .Select(clientNutritionPlan => clientNutritionPlan.NutritionPlan)
            .Include(nutritionPlan => nutritionPlan.Meals)
            .OrderBy(nutritionPlan => nutritionPlan.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Meals
            .AsNoTracking()
            .Where(meal => meal.NutritionPlan.NutritionPlanId == nutritionPlanId)
            .OrderBy(meal => meal.MealId)
            .ToListAsync(cancellationToken);
    }
}
