using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public interface IShoppingListService
    {
        Task GenerateShoppingListFromMealPlanAsync(int userId);
        Task DeleteAsync(int id);
        Task<IEnumerable<ShoppingItem>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId);
        Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request);
        Task<bool> RemoveItemAsync(int itemId);
        Task<bool> MoveToPantryAsync(int itemId);
    }
}
