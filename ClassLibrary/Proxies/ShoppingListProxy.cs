using System.Net.Http.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Proxies.Interfaces;

namespace ClassLibrary.Proxies;

public sealed class ShoppingListProxy : IShoppingListProxy
{
    private readonly HttpClient httpClient;

    public ShoppingListProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ShoppingListItem>> GetShoppingItemsAsync(int userId)
    {
        var dtos = await this.httpClient.GetFromJsonAsync<List<ShoppingItemDto>>(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/user/{userId}");

        return (dtos ?? []).Select(MapShoppingListItem).ToList();
    }

    public async Task<ShoppingListItem?> AddItemAsync(string itemName, int userId, double quantityGrams)
    {
        var request = new AddShoppingItemRequest
        {
            ItemName = itemName,
            Quantity = quantityGrams,
        };

        var response = await this.httpClient.PostAsJsonAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/user/{userId}",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        IReadOnlyList<ShoppingListItem> latestItems = await this.GetShoppingItemsAsync(userId);
        return latestItems
            .Where(item => item.IngredientName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.ShoppingListItemId)
            .FirstOrDefault();
    }

    public async Task<bool> RemoveItemAsync(ShoppingListItem item)
    {
        var response = await this.httpClient.DeleteAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/{item.ShoppingListItemId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MoveToPantryAsync(ShoppingListItem item)
    {
        var response = await this.httpClient.PostAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/{item.ShoppingListItemId}/move-to-pantry",
            null);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> GenerateListAsync(int userId)
    {
        var previousItems = await this.GetShoppingItemsAsync(userId);

        var response = await this.httpClient.PostAsync(
            $"{ApiBaseUrl.BASE_URL}/api/ShoppingList/generate/{userId}",
            null);

        if (!response.IsSuccessStatusCode)
        {
            return -1;
        }

        var currentItems = await this.GetShoppingItemsAsync(userId);
        return Math.Max(0, currentItems.Count - previousItems.Count);
    }

    public async Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string query)
    {
        var allIngredients = await this.httpClient.GetFromJsonAsync<List<IngredientDataTransferObject>>(
            $"{ApiBaseUrl.BASE_URL}/api/inventory/ingredients");

        return (allIngredients ?? [])
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


