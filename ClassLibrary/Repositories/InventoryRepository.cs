using System.Collections.Generic;
using System.Threading;
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

    public async Task<IReadOnlyList<Inventory>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await this.databaseContext.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.Ingredient)
            .Where(inventory => EF.Property<int>(inventory, "UserId") == userId)
            .ToListAsync(cancellationToken);
    }

    private async Task<Inventory?> GetByUserIdAndIngredientIdAsync(int userId, int ingredientId, CancellationToken cancellationToken)
    {
        return await this.databaseContext.Inventories
            .FirstOrDefaultAsync(
                existingInventory =>
                    EF.Property<int>(existingInventory, "UserId") == userId &&
                    EF.Property<int>(existingInventory, "IngredientId") == ingredientId,
                cancellationToken);
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        var userId = inventory.User?.UserId ?? 0;
        var ingredientId = inventory.Ingredient?.IngredientId ?? 0;

        if (userId <= 0 || ingredientId <= 0)
        {
            throw new ArgumentException("Inventory must include User and Ingredient navigation stubs with valid key values.");
        }

        var existing = await this.GetByUserIdAndIngredientIdAsync(userId, ingredientId, cancellationToken);
        if (existing is not null)
        {
            existing.QuantityGrams += inventory.QuantityGrams;
        }
        else
        {
            this.databaseContext.Inventories.Add(inventory);
        }

        await this.databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        var existing = await this.databaseContext.Inventories.FindAsync([inventory.InventoryId], cancellationToken);
        if (existing is not null)
        {
            existing.QuantityGrams = inventory.QuantityGrams;
            await this.databaseContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(int inventoryId, CancellationToken cancellationToken = default)
    {
        var entity = await this.databaseContext.Inventories.FindAsync([inventoryId], cancellationToken);
        if (entity is not null)
        {
            this.databaseContext.Inventories.Remove(entity);
            await this.databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
