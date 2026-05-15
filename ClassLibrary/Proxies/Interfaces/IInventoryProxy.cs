using ClassLibrary.DTOs;

namespace ClassLibrary.Proxies.Interfaces;

public interface IInventoryProxy
{
    Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId);

    Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync();

    Task AddToPantryAsync(AddToPantryRequestDataTransferObject request);

    Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request);

    Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request);

    Task RemoveItemAsync(int inventoryId);
}



