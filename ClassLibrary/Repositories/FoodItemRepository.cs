using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.Filters;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class FoodItemRepository(AppDbContext dbContext) : IFoodItemRepository
{
    public async Task<FoodItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.FoodItems
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FoodItemId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FoodItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.FoodItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FoodItem entity, CancellationToken cancellationToken = default)
    {
        dbContext.FoodItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FoodItem entity, CancellationToken cancellationToken = default)
    {
        dbContext.FoodItems.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.FoodItems.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.FoodItems.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetByFilterAsync(FoodItemFilter filter, CancellationToken cancellationToken = default)
    {
        var query = dbContext.FoodItems.AsNoTracking().AsQueryable();

        if (filter.IsVegan)
        {
            query = query.Where(f => f.IsVegan);
        }

        if (filter.IsKeto)
        {
            query = query.Where(f => f.IsKeto);
        }

        if (filter.IsGlutenFree)
        {
            query = query.Where(f => f.IsGlutenFree);
        }

        if (filter.IsLactoseFree)
        {
            query = query.Where(f => f.IsLactoseFree);
        }

        if (filter.IsNutFree)
        {
            query = query.Where(f => f.IsNutFree);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(f => f.Name.ToLower().Contains(term));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task ToggleFavoriteAsync(int userId, int foodItemId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FoodItemId == foodItemId, cancellationToken);

        if (existing is not null)
        {
            dbContext.Favorites.Remove(existing);
        }
        else
        {
            dbContext.Favorites.Add(new Favorite { UserId = userId, FoodItemId = foodItemId });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FoodItem>> GetFavoritesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Join(dbContext.FoodItems, f => f.FoodItemId, fi => fi.FoodItemId, (f, fi) => fi)
            .ToListAsync(cancellationToken);
    }
}
