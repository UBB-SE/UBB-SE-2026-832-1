using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class MealPlanRepository(AppDbContext dbContext) : IMealPlanRepository
{
    public async Task<MealPlan?> GetByIdAsync(int id)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(mealPlan => mealPlan.MealPlanId == id);
    }

    public async Task<IReadOnlyList<MealPlan>> GetByUserIdAsync(int userId)
    {
        return await dbContext.MealPlans
            .AsNoTracking()
            .Where(mealPlan => mealPlan.User.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(MealPlan entity)
    {
        dbContext.MealPlans.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(MealPlan entity)
    {
        dbContext.MealPlans.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await dbContext.MealPlans.FindAsync([id]);
        if (entity is not null)
        {
            dbContext.MealPlans.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task AddFoodItemToPlanAsync(int mealPlanId, int foodItemId)
    {
        var exists = await dbContext.MealPlanFoodItems
            .AnyAsync(
                mealPlanFoodItem =>
                    EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId &&
                    EF.Property<int>(mealPlanFoodItem, "FoodItemId") == foodItemId);

        if (!exists)
        {
            dbContext.MealPlanFoodItems.Add(new MealPlanFoodItem
            {
                MealPlan = new MealPlan { MealPlanId = mealPlanId },
                FoodItem = new FoodItem { FoodItemId = foodItemId },
            });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveFoodItemFromPlanAsync(int mealPlanId, int foodItemId)
    {
        var entry = await dbContext.MealPlanFoodItems
            .FirstOrDefaultAsync(
                mealPlanFoodItem =>
                    EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId &&
                    EF.Property<int>(mealPlanFoodItem, "FoodItemId") == foodItemId);

        if (entry is not null)
        {
            dbContext.MealPlanFoodItems.Remove(entry);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetFoodItemsForPlanAsync(int mealPlanId)
    {
        return await dbContext.MealPlanFoodItems
            .AsNoTracking()
            .Where(mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId)
            .Select(mealPlanFoodItem => mealPlanFoodItem.FoodItem)
            .ToListAsync();
    }

    public async Task AddIngredientToFoodItemAsync(int foodItemId, int ingredientId)
    {
        var exists = await dbContext.FoodItemIngredients
            .AnyAsync(
                foodItemIngredient =>
                    EF.Property<int>(foodItemIngredient, "FoodItemId") == foodItemId &&
                    EF.Property<int>(foodItemIngredient, "IngredientId") == ingredientId);

        if (!exists)
        {
            dbContext.FoodItemIngredients.Add(new FoodItemIngredient
            {
                FoodItem = new FoodItem { FoodItemId = foodItemId },
                Ingredient = new Ingredient { IngredientId = ingredientId },
            });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveIngredientFromFoodItemAsync(int foodItemId, int ingredientId)
    {
        var entry = await dbContext.FoodItemIngredients
            .FirstOrDefaultAsync(
                foodItemIngredient =>
                    EF.Property<int>(foodItemIngredient, "FoodItemId") == foodItemId &&
                    EF.Property<int>(foodItemIngredient, "IngredientId") == ingredientId);

        if (entry is not null)
        {
            dbContext.FoodItemIngredients.Remove(entry);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<int>> GetIngredientIdsForMealPlanAsync(int mealPlanId)
    {
        return await dbContext.MealPlanFoodItems
            .AsNoTracking()
            .Where(mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "MealPlanId") == mealPlanId)
            .Join(
                dbContext.FoodItemIngredients.AsNoTracking(),
                mealPlanFoodItem => EF.Property<int>(mealPlanFoodItem, "FoodItemId"),
                foodItemIngredient => EF.Property<int>(foodItemIngredient, "FoodItemId"),
                (_, foodItemIngredient) => EF.Property<int>(foodItemIngredient, "IngredientId"))
            .ToListAsync();
    }

}
