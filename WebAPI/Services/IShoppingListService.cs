using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WebApi.Services
{
    public interface IShoppingListService
    {
        Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId);
        Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request);
        Task<bool> RemoveItemAsync(int itemId);
        Task<bool> MoveToPantryAsync(int itemId);
    }
}
