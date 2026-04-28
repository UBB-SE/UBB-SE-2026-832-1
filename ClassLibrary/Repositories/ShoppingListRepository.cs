namespace ClassLibrary.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.IRepositories;

public sealed class ShoppingListRepository(AppDbContext dbContext) : IShoppingListRepository
{
    public async Task AddAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        await dbContext.ShoppingItems.AddAsync(item, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ShoppingItems
            .AsNoTracking()
            .Include(s => s.Ingredient)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ShoppingItems
            .Include(s => s.Ingredient)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ShoppingItems
            .Include(s => s.Ingredient)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IngredientId == ingredientId, cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ShoppingItems
            .AsNoTracking()
            .Include(s => s.Ingredient)
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default)
    {
        dbContext.ShoppingItems.Update(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dbContext.ShoppingItems
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}