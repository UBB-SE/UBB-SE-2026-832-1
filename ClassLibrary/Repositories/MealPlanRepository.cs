using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class MealPlanRepository(AppDbContext dbContext) : IMealPlanRepository
{
    public async Task<MealPlan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(mp => mp.MealPlanId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MealPlan>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .Where(mp => mp.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MealPlan entity, CancellationToken cancellationToken = default)
    {
        dbContext.MealPlans.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MealPlan entity, CancellationToken cancellationToken = default)
    {
        dbContext.MealPlans.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.MealPlans.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.MealPlans.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.MealPlanFoodItems
            .AnyAsync(mpf => mpf.MealPlanId == mealPlanId && mpf.FoodItemId == foodItemId, cancellationToken);

        if (!exists)
        {
            dbContext.MealPlanFoodItems.Add(new MealPlanFoodItem { MealPlanId = mealPlanId, FoodItemId = foodItemId });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.MealPlanFoodItems
            .FirstOrDefaultAsync(mpf => mpf.MealPlanId == mealPlanId && mpf.FoodItemId == foodItemId, cancellationToken);

        if (entry is not null)
        {
            dbContext.MealPlanFoodItems.Remove(entry);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetFoodItemsForPlanAsync(int mealPlanId, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlanFoodItems
            .AsNoTracking()
            .Where(mpf => mpf.MealPlanId == mealPlanId)
            .Join(dbContext.FoodItems, mpf => mpf.FoodItemId, fi => fi.FoodItemId, (mpf, fi) => fi)
            .ToListAsync(cancellationToken);
    }

    public async Task AddIngredientToFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.FoodItemIngredients
            .AnyAsync(fi => fi.FoodItemId == foodItemId && fi.IngredientId == ingredientId, cancellationToken);

        if (!exists)
        {
            dbContext.FoodItemIngredients.Add(new FoodItemIngredient { FoodItemId = foodItemId, IngredientId = ingredientId });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveIngredientFromFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.FoodItemIngredients
            .FirstOrDefaultAsync(fi => fi.FoodItemId == foodItemId && fi.IngredientId == ingredientId, cancellationToken);

        if (entry is not null)
        {
            dbContext.FoodItemIngredients.Remove(entry);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
