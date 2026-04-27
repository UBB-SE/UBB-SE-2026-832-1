using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class InventoryRepository(AppDbContext dbContext) : IInventoryRepository
{
    public async Task<Inventory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Inventories
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.Inventories
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var ingredient = await dbContext.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(ing => ing.FoodId == item.IngredientId, cancellationToken);

            item.IngredientName = ingredient?.Name ?? string.Empty;
        }

        return items;
    }

    public async Task AddAsync(Inventory entity, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Inventories
            .FirstOrDefaultAsync(i => i.UserId == entity.UserId && i.IngredientId == entity.IngredientId, cancellationToken);

        if (existing is not null)
        {
            existing.QuantityGrams += entity.QuantityGrams;
        }
        else
        {
            dbContext.Inventories.Add(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Inventory entity, CancellationToken cancellationToken = default)
    {
        dbContext.Inventories.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Inventories.FindAsync([id], cancellationToken);
        if (entity is not null)
        {
            dbContext.Inventories.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
