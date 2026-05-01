using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.Filters;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class FoodItemRepository(AppDbContext dbContext) : IFoodItemRepository
{
    public async Task<FoodItem?> GetByIdAsync(int id)
    {
        return await dbContext.FoodItems
            .AsNoTracking()
            .FirstOrDefaultAsync(foodItem => foodItem.FoodItemId == id);
    }

    public async Task<IReadOnlyList<FoodItem>> GetAllAsync()
    {
        return await dbContext.FoodItems
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(FoodItem entity)
    {
        dbContext.FoodItems.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(FoodItem entity)
    {
        dbContext.FoodItems.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await dbContext.FoodItems.FindAsync([id]);
        if (entity is not null)
        {
            dbContext.FoodItems.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetByFilterAsync(FoodItemFilter filter)
    {
        var query = dbContext.FoodItems.AsNoTracking().AsQueryable();

        if (filter.IsVegan)
        {
            query = query.Where(foodItem => foodItem.IsVegan);
        }

        if (filter.IsKeto)
        {
            query = query.Where(foodItem => foodItem.IsKeto);
        }

        if (filter.IsGlutenFree)
        {
            query = query.Where(foodItem => foodItem.IsGlutenFree);
        }

        if (filter.IsLactoseFree)
        {
            query = query.Where(foodItem => foodItem.IsLactoseFree);
        }

        if (filter.IsNutFree)
        {
            query = query.Where(foodItem => foodItem.IsNutFree);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(foodItem => foodItem.Name.ToLower().Contains(term));
        }

        return await query.ToListAsync();
    }

    public async Task ToggleFavoriteAsync(int userId, int foodItemId)
    {
        var existing = await dbContext.Favorites
            .FirstOrDefaultAsync(
                favorite => EF.Property<int>(favorite, "UserId") == userId && EF.Property<int>(favorite, "FoodItemId") == foodItemId);

        if (existing is not null)
        {
            dbContext.Favorites.Remove(existing);
        }
        else
        {
            dbContext.Favorites.Add(new Favorite
            {
                User = new User { UserId = userId },
                FoodItem = new FoodItem { FoodItemId = foodItemId },
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<FoodItem>> GetFavoritesByUserIdAsync(int userId)
    {
        return await dbContext.Favorites
            .AsNoTracking()
            .Where(favorite => EF.Property<int>(favorite, "UserId") == userId)
            .Select(favorite => favorite.FoodItem)
            .ToListAsync();
    }
}
