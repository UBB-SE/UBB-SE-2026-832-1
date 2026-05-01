using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

namespace ClassLibrary.Repositories;

public sealed class ShoppingListRepository : IShoppingListRepository
{
    private readonly AppDbContext databaseContext;

    public ShoppingListRepository(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task AddAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        await databaseContext.ShoppingItems.AddAsync(item, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingItem?> GetByIdAsync(int shoppingItemId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .FirstOrDefaultAsync(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId, cancellationToken);
    }

    public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .FirstOrDefaultAsync(
                shoppingItem =>
                    shoppingItem.User.UserId == userId &&
                    shoppingItem.Ingredient.IngredientId == ingredientId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .Where(shoppingItem => shoppingItem.User.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        databaseContext.ShoppingItems.Update(item);
        await databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int shoppingItemId, CancellationToken cancellationToken = default)
    {
        await databaseContext.ShoppingItems
            .Where(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
