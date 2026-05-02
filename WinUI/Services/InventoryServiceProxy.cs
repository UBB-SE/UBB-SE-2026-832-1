using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace WinUI.Services;

public sealed class InventoryServiceProxy : IInventoryServiceProxy
{
    private const string API_BASE_ADDRESS = "https://localhost:7197";
    private readonly HttpClient httpClient;

    public InventoryServiceProxy(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId)
    {
        var inventory = await this.httpClient.GetFromJsonAsync<List<InventoryDataTransferObject>>($"{API_BASE_ADDRESS}/api/inventory/{userId}");
        return inventory ?? [];
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
    {
        var ingredients = await this.httpClient.GetFromJsonAsync<List<IngredientDataTransferObject>>($"{API_BASE_ADDRESS}/api/inventory/ingredients");
        return ingredients ?? [];
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/inventory/add-to-pantry", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/inventory/add-ingredient-by-name", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request)
    {
        var response = await this.httpClient.PostAsJsonAsync($"{API_BASE_ADDRESS}/api/inventory/consume-meal", request);
        return response.IsSuccessStatusCode;
    }

    public async Task RemoveItemAsync(int inventoryId)
    {
        var response = await this.httpClient.DeleteAsync($"{API_BASE_ADDRESS}/api/inventory/{inventoryId}");
        response.EnsureSuccessStatusCode();
    }
}
