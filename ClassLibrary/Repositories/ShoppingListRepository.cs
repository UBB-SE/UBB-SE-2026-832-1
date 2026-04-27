using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class ShoppingListRepository(AppDbContext dbContext) : IShoppingListRepository
{
    public async Task<ShoppingItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.ShoppingItems
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (item is not null)
        {
            var ingredient = await dbContext.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.FoodId == item.IngredientId, cancellationToken);

            item.IngredientName = ingredient?.Name ?? string.Empty;
        }

        return item;
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await dbContext.ShoppingItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var ingredient = await dbContext.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.FoodId == item.IngredientId, cancellationToken);

            item.IngredientName = ingredient?.Name ?? string.Empty;
        }

        return items;
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.ShoppingItems
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var ingredient = await dbContext.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.FoodId == item.IngredientId, cancellationToken);

            item.IngredientName = ingredient?.Name ?? string.Empty;
        }

        return items;
    }

    public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.ShoppingItems
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IngredientId == ingredientId, cancellationToken);

        if (item is not null)
        {
            var ingredient = await dbContext.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.FoodId == item.IngredientId, cancellationToken);

            item.IngredientName = ingredient?.Name ?? string.Empty;
        }

        return item;
    }

    public async Task AddAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        dbContext.ShoppingItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        dbContext.ShoppingItems.Update(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.ShoppingItems.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.ShoppingItems.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetIngredientsNeededFromMealPlanAsync(int userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var todaysPlan = await dbContext.MealPlans
            .AsNoTracking()
            .Where(mp => mp.UserId == userId && mp.CreatedAt >= today && mp.CreatedAt < tomorrow)
            .OrderByDescending(mp => mp.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (todaysPlan is null)
        {
            return new List<ShoppingItem>();
        }

        var mealIds = await dbContext.MealPlanMeals
            .AsNoTracking()
            .Where(mpm => mpm.MealPlanId == todaysPlan.Id)
            .Select(mpm => mpm.MealId)
            .ToListAsync(cancellationToken);

        var neededIngredients = await dbContext.MealsIngredients
            .AsNoTracking()
            .Where(mi => mealIds.Contains(mi.MealId))
            .GroupBy(mi => mi.FoodId)
            .Select(g => new
            {
                FoodId = g.Key,
                TotalQuantity = g.Sum(mi => mi.Quantity)
            })
            .ToListAsync(cancellationToken);

        var items = new List<ShoppingItem>();

        foreach (var needed in neededIngredients)
        {
            var inventory = await dbContext.Inventories
                .AsNoTracking()
                .FirstOrDefaultAsync(inv => inv.UserId == userId && inv.IngredientId == needed.FoodId, cancellationToken);

            double inventoryQty = inventory?.QuantityGrams ?? 0;
            double quantityNeeded = needed.TotalQuantity - inventoryQty;

            if (quantityNeeded > 0)
            {
                var ingredient = await dbContext.Ingredients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.FoodId == needed.FoodId, cancellationToken);

                items.Add(new ShoppingItem
                {
                    UserId = userId,
                    IngredientId = needed.FoodId,
                    IngredientName = ingredient?.Name ?? string.Empty,
                    QuantityGrams = quantityNeeded,
                    IsChecked = false
                });
            }
        }

        return items;
    }
}
