using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.IRepositories;

public interface IShoppingListRepository
{
    Task AddAsync(ShoppingItem item);
    Task DeleteAsync(int shoppingItemId);
    Task<IReadOnlyList<ShoppingItem>> GetAllAsync();
    Task<IReadOnlyList<ShoppingItem>> GetAllByUserIdAsync(int userId);
    Task<ShoppingItem?> GetByIdAsync(int shoppingItemId);
    Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId);
    Task UpdateAsync(ShoppingItem item);
}
