using System.Collections.Generic;
using System.Linq;
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

    public async Task AddAsync(ShoppingItem item)
    {
        await databaseContext.ShoppingItems.AddAsync(item);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllAsync()
    {
        return await databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .ToListAsync();
    }

    public async Task<ShoppingItem?> GetByIdAsync(int shoppingItemId)
    {
        return await databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .Include(shoppingItem => shoppingItem.User)
            .FirstOrDefaultAsync(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId);
    }

    public async Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId)
    {
        return await databaseContext.ShoppingItems
            .Include(shoppingItem => shoppingItem.Ingredient)
            .FirstOrDefaultAsync(
                shoppingItem =>
                    shoppingItem.User.UserId == userId &&
                    shoppingItem.Ingredient.IngredientId == ingredientId);
    }

    public async Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId)
    {
        return await databaseContext.ShoppingItems
            .AsNoTracking()
            .Include(shoppingItem => shoppingItem.Ingredient)
            .Where(shoppingItem => shoppingItem.User.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateAsync(ShoppingItem item)
    {
        databaseContext.ShoppingItems.Update(item);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int shoppingItemId)
    {
        await databaseContext.ShoppingItems
            .Where(shoppingItem => shoppingItem.ShoppingItemId == shoppingItemId)
            .ExecuteDeleteAsync();
    }
}
