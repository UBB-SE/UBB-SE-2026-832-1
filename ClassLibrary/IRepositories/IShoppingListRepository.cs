using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.IRepositories
{
    public interface IShoppingListRepository
    {
        Task AddAsync(ShoppingItem item);
        Task<IEnumerable<ShoppingItem>> GetAllAsync();
        Task<ShoppingItem?> GetByIdAsync(int id);
        Task<ShoppingItem?> GetByUserAndIngredientAsync(int userId, int ingredientId);
        Task<IEnumerable<ShoppingItem>> GetAllByUserIdAsync(int userId);
        Task UpdateAsync(ShoppingItem item);
        Task DeleteAsync(int id);
        Task<List<ShoppingItem>> GetIngredientsNeededFromMealPlanAsync(int userId);
    }
}
