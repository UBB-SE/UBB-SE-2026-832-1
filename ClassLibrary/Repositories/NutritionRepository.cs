using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;


public sealed class NutritionRepository : INutritionRepository
{
    private readonly AppDbContext databaseContext;

    public NutritionRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<int> InsertNutritionPlanAsync(NutritionPlan plan)
    {
        await this.databaseContext.NutritionPlans.AddAsync(plan);
        await this.databaseContext.SaveChangesAsync();
        return plan.NutritionPlanId;
    }

    public async Task InsertMealAsync(Meal meal, int nutritionPlanId)
    {
        var plan = await this.databaseContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        meal.NutritionPlan = plan;
        await this.databaseContext.Meals.AddAsync(meal);
        await this.databaseContext.SaveChangesAsync();
    }

    public async Task AssignNutritionPlanToClientAsync(int clientId, int nutritionPlanId)
    {
        bool exists = await this.databaseContext.ClientNutritionPlans
            .AnyAsync(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId && clientNutritionPlan.NutritionPlan.NutritionPlanId == nutritionPlanId);

        if (exists)
        {
            return;
        }

        var client = await this.databaseContext.Clients
            .FirstOrDefaultAsync(client => client.ClientId == clientId)
            ?? throw new InvalidOperationException($"Client {clientId} not found.");

        var plan = await this.databaseContext.NutritionPlans
            .FirstOrDefaultAsync(nutritionPlan => nutritionPlan.NutritionPlanId == nutritionPlanId)
            ?? throw new InvalidOperationException($"NutritionPlan {nutritionPlanId} not found.");

        this.databaseContext.ClientNutritionPlans.Add(new ClientNutritionPlan
        {
            Client = client,
            NutritionPlan = plan,
        });

        await this.databaseContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<NutritionPlan>> GetNutritionPlansForClientAsync(int clientId)
    {
        return await this.databaseContext.ClientNutritionPlans
            .AsNoTracking()
            .Where(clientNutritionPlan => clientNutritionPlan.Client.ClientId == clientId)
            .Select(clientNutritionPlan => clientNutritionPlan.NutritionPlan)
            .Include(nutritionPlan => nutritionPlan.Meals)
            .OrderBy(nutritionPlan => nutritionPlan.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Meal>> GetMealsForPlanAsync(int nutritionPlanId)
    {
        return await this.databaseContext.Meals
            .AsNoTracking()
            .Where(meal => meal.NutritionPlan.NutritionPlanId == nutritionPlanId)
            .OrderBy(meal => meal.MealId)
            .ToListAsync();
    }
}
