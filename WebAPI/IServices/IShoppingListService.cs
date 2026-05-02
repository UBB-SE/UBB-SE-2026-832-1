using ClassLibrary.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.IServices;

public interface IShoppingListService
{
    Task GenerateShoppingListFromMealPlanAsync(int userId);
    Task DeleteAsync(int id);
    Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId);
    Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> MoveToPantryAsync(int itemId);
}
