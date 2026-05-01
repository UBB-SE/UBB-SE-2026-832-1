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
            .FirstOrDefaultAsync(mealPlan => mealPlan.MealPlanId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MealPlan>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .Where(mealPlan => mealPlan.User.UserId == userId)
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
            .AnyAsync(
                mealPlanFoodItem =>
                    EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId &&
                    EF.Property<int>(mealPlanFoodItem, "FoodItemId") == foodItemId,
                cancellationToken);

        if (!exists)
        {
            dbContext.MealPlanFoodItems.Add(new MealPlanFoodItem
            {
                MealPlan = new MealPlan { MealPlanId = mealPlanId },
                FoodItem = new FoodItem { FoodItemId = foodItemId },
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.MealPlanFoodItems
            .FirstOrDefaultAsync(
                mealPlanFoodItem =>
                    EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId &&
                    EF.Property<int>(mealPlanFoodItem, "FoodItemId") == foodItemId,
                cancellationToken);

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
            .Where(mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId)
            .Select(mealPlanFoodItem => mealPlanFoodItem.FoodItem)
            .ToListAsync(cancellationToken);
    }

    public async Task AddIngredientToFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.FoodItemIngredients
            .AnyAsync(
                foodItemIngredient =>
                    EF.Property<int>(foodItemIngredient, "FoodItemId") == foodItemId &&
                    EF.Property<int>(foodItemIngredient, "IngredientId") == ingredientId,
                cancellationToken);

        if (!exists)
        {
            dbContext.FoodItemIngredients.Add(new FoodItemIngredient
            {
                FoodItem = new FoodItem { FoodItemId = foodItemId },
                Ingredient = new Ingredient { IngredientId = ingredientId },
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveIngredientFromFoodItemAsync(int foodItemId, int ingredientId, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.FoodItemIngredients
            .FirstOrDefaultAsync(
                foodItemIngredient =>
                    EF.Property<int>(foodItemIngredient, "FoodItemId") == foodItemId &&
                    EF.Property<int>(foodItemIngredient, "IngredientId") == ingredientId,
                cancellationToken);

        if (entry is not null)
        {
            dbContext.FoodItemIngredients.Remove(entry);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<int>> GetIngredientIdsForMealPlanAsync(int mealPlanId, CancellationToken cancellationToken = default)
    {
        var foodItemIds = await dbContext.MealPlanFoodItems
            .AsNoTracking()
            .Where(mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId)
            .Select(mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "FoodItemId"))
            .ToListAsync(cancellationToken);

        return await dbContext.FoodItemIngredients
            .AsNoTracking()
            .Where(foodItemIngredient => foodItemIds.Contains(EF.Property<int>(foodItemIngredient, "FoodItemId")))
            .Select(foodItemIngredient => EF.Property<int>(foodItemIngredient, "IngredientId"))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

}
