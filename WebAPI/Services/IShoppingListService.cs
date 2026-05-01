using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public interface IShoppingListService
    {
        Task GenerateShoppingListFromMealPlanAsync(int userId);
        Task DeleteAsync(int id);
        Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId);
        Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request);
        Task<bool> RemoveItemAsync(int itemId);
        Task<bool> MoveToPantryAsync(int itemId);
    }
}
