using ClassLibrary.DTOs;

namespace WinUI.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryServiceProxy serviceProxy;

    public InventoryService(IInventoryServiceProxy serviceProxy)
    {
        this.serviceProxy = serviceProxy;
    }

    public Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId)
    {
        return this.serviceProxy.GetUserInventoryAsync(userId);
    }

    public Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
    {
        return this.serviceProxy.GetAllIngredientsAsync();
    }

    public Task AddToPantryAsync(AddToPantryRequestDataTransferObject request)
    {
        return this.serviceProxy.AddToPantryAsync(request);
    }

    public Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request)
    {
        return this.serviceProxy.AddIngredientByNameToPantryAsync(request);
    }

    public Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request)
    {
        return this.serviceProxy.ConsumeMealAsync(request);
    }

    public Task RemoveItemAsync(int inventoryId)
    {
        return this.serviceProxy.RemoveItemAsync(inventoryId);
    }
}
