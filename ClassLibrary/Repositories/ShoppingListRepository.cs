namespace ClassLibrary.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

public sealed class ShoppingListRepository : IShoppingListRepository
{
    private readonly AppDbContext _databaseContext;

    public ShoppingListRepository(AppDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task AddAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        await _databaseContext.ShoppingItems.AddAsync(item, cancellationToken);
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingItem?> GetByIdAsync(int shoppingItemId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .FirstOrDefaultAsync(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId, cancellationToken);
    }

    public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .FirstOrDefaultAsync(shoppingItem => shoppingItem.UserId == userId && shoppingItem.IngredientId == ingredientId, cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .Where(shoppingItem => shoppingItem.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        _databaseContext.ShoppingItems.Update(item);
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int shoppingItemId, CancellationToken cancellationToken = default)
    {
        await _databaseContext.ShoppingItems
            .Where(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}