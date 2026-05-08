using ClassLibrary.Models;

namespace WinUI.Services;

public interface IShoppingListService
{
    Task<IReadOnlyList<ShoppingListItem>> GetShoppingItemsAsync(int userId);

    Task<ShoppingListItem?> AddItemAsync(string itemName, int userId, double quantityGrams);

    Task<bool> RemoveItemAsync(ShoppingListItem item);

    Task<bool> MoveToPantryAsync(ShoppingListItem item);

    Task<int> GenerateListAsync(int userId);

    Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string query);
}
