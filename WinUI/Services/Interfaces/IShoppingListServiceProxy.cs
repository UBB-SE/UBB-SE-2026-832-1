using ClassLibrary.DTOs;
using ClassLibrary.Models;

namespace WinUI.Services.Interfaces;

public interface IShoppingListServiceProxy
{
    Task<IReadOnlyList<ShoppingItemDto>> GetShoppingItemsAsync(int userId);

    Task<bool> AddItemAsync(string itemName, int userId, double quantityGrams);

    Task<bool> RemoveItemAsync(int itemId);

    Task<bool> MoveToPantryAsync(int itemId);

    Task<int> GenerateListAsync(int userId);

    Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync();
}
