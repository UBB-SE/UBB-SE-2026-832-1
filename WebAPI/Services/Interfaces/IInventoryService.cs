using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.DTOs;

namespace WebAPI.Services.Interfaces;

public interface IInventoryService
{
    Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request);

    Task AddToPantryAsync(AddToPantryRequestDataTransferObject request);

    Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request);

    Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId);

    Task RemoveItemAsync(int inventoryId);

    Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync();
}
