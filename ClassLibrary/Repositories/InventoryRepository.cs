using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Repositories;

public sealed class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext databaseContext;

    public InventoryRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId)
    {
        return await this.databaseContext.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.Ingredient)
            .Where(inventory => inventory.UserId == userId)
            .ToListAsync();
    }

    private async Task<Inventory?> GetByUserIdAndIngredientIdAsync(int userId, int ingredientId)
    {
        return await this.databaseContext.Inventories
            .FirstOrDefaultAsync(
                existingInventory =>
                    existingInventory.UserId == userId &&
                    existingInventory.IngredientId == ingredientId);
    }

    public async Task AddAsync(Inventory inventory)
    {
        if (inventory.UserId <= 0 || inventory.IngredientId <= 0)
        {
            throw new ArgumentException("Inventory must have valid UserId and IngredientId.");
        }

        var existing = await this.GetByUserIdAndIngredientIdAsync(inventory.UserId, inventory.IngredientId);
        if (existing is not null)
        {
            existing.QuantityGrams += inventory.QuantityGrams;
        }
        else
        {
            this.databaseContext.Inventories.Add(inventory);
        }

        await this.databaseContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Inventory inventory)
    {
        var existing = await this.databaseContext.Inventories.FindAsync([inventory.InventoryId]);
        if (existing is not null)
        {
            existing.QuantityGrams = inventory.QuantityGrams;
            await this.databaseContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int inventoryId)
    {
        var entity = await this.databaseContext.Inventories.FindAsync([inventoryId]);
        if (entity is not null)
        {
            this.databaseContext.Inventories.Remove(entity);
            await this.databaseContext.SaveChangesAsync();
        }
    }
}
