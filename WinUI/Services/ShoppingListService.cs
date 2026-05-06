using ClassLibrary.DTOs;
using ClassLibrary.Models;
using WinUI.Services.Interfaces;

namespace WinUI.Services;

public sealed class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListServiceProxy serviceProxy;

    public ShoppingListService(IShoppingListServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public async Task<IReadOnlyList<ShoppingListItem>> GetShoppingItemsAsync(int userId)
    {
        var dtos = await this.serviceProxy.GetShoppingItemsAsync(userId);
        return dtos.Select(MapShoppingListItem).ToList();
    }

    public async Task<ShoppingListItem?> AddItemAsync(string itemName, int userId, double quantityGrams)
    {
        bool success = await this.serviceProxy.AddItemAsync(itemName, userId, quantityGrams);
        if (!success)
        {
            return null;
        }

        IReadOnlyList<ShoppingListItem> latestItems = await this.GetShoppingItemsAsync(userId);
        return latestItems
            .Where(item => item.IngredientName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.ShoppingListItemId)
            .FirstOrDefault();
    }

    public Task<bool> RemoveItemAsync(ShoppingListItem item)
    {
        return this.serviceProxy.RemoveItemAsync(item.ShoppingListItemId);
    }

    public Task<bool> MoveToPantryAsync(ShoppingListItem item)
    {
        return this.serviceProxy.MoveToPantryAsync(item.ShoppingListItemId);
    }

    public Task<int> GenerateListAsync(int userId)
    {
        return this.serviceProxy.GenerateListAsync(userId);
    }

    public async Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string query)
    {
        var allIngredients = await this.serviceProxy.GetAllIngredientsAsync();

        return allIngredients
            .Where(ingredient => ingredient.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(ingredient => new KeyValuePair<int, string>(ingredient.IngredientId, ingredient.Name))
            .ToList();
    }

    private static ShoppingListItem MapShoppingListItem(ShoppingItemDto shoppingItemDataTransferObject)
    {
        return new ShoppingListItem
        {
            ShoppingListItemId = shoppingItemDataTransferObject.Id,
            IngredientName = shoppingItemDataTransferObject.IngredientName,
            QuantityGrams = shoppingItemDataTransferObject.QuantityGrams,
            IsChecked = shoppingItemDataTransferObject.IsChecked,
        };
    }
}
