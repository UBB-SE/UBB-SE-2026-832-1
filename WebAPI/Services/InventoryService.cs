using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly IInventoryRepository inventoryRepository;
    private readonly IMealPlanRepository mealPlanRepository;
    private readonly IIngredientRepository ingredientRepository;

    private const int DefaultIngredientsQuantity = 100;

    public InventoryService(
        IIngredientRepository ingredientRepository,
        IInventoryRepository inventoryRepository,
        IMealPlanRepository mealPlanRepository)
    {
        this.inventoryRepository = inventoryRepository;
        this.mealPlanRepository = mealPlanRepository;
        this.ingredientRepository = ingredientRepository;
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        var requiredIngredientIds = await this.mealPlanRepository.GetIngredientIdsForMealPlanAsync(request.MealPlanId, cancellationToken);
        var inventoryByIngredientId = (await this.inventoryRepository.GetAllByUserIdAsync(request.UserId, cancellationToken))
            .GroupBy(inventoryItem => inventoryItem.Ingredient.IngredientId)
            .ToDictionary(group => group.Key, group => group.First());

        foreach (int ingredientId in requiredIngredientIds)
        {
            if (!inventoryByIngredientId.TryGetValue(ingredientId, out var stock) || stock.QuantityGrams < DefaultIngredientsQuantity)
            {
                return false;
            }
        }

        foreach (int ingredientId in requiredIngredientIds)
        {
            var stock = inventoryByIngredientId[ingredientId];
            stock.QuantityGrams -= DefaultIngredientsQuantity;

            if (stock.QuantityGrams <= 0)
            {
                await this.inventoryRepository.DeleteAsync(stock.InventoryId, cancellationToken);
            }
            else
            {
                await this.inventoryRepository.UpdateAsync(stock, cancellationToken);
            }
        }

        return true;
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        var newItem = new Inventory
        {
            User = new User { UserId = request.UserId },
            Ingredient = new Ingredient { IngredientId = request.IngredientId },
            QuantityGrams = request.QuantityGrams,
        };

        await this.inventoryRepository.AddAsync(newItem, cancellationToken);
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request, CancellationToken cancellationToken = default)
    {
        int ingredientId = await this.ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.IngredientName);
        await this.AddToPantryAsync(new AddToPantryRequestDataTransferObject
        {
            UserId = request.UserId,
            IngredientId = ingredientId,
            QuantityGrams = DefaultIngredientsQuantity,
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var inventoryItems = await this.inventoryRepository.GetAllByUserIdAsync(userId, cancellationToken);
        return inventoryItems.Select(MapToInventoryDto).ToList();
    }

    public async Task RemoveItemAsync(int inventoryId, CancellationToken cancellationToken = default)
    {
        await this.inventoryRepository.DeleteAsync(inventoryId, cancellationToken);
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync(CancellationToken cancellationToken = default)
    {
        var ingredients = await this.ingredientRepository.GetAllAsync();
        return ingredients.Select(MapToIngredientDto).ToList();
    }

    private static InventoryDataTransferObject MapToInventoryDto(Inventory inventory)
    {
        return new InventoryDataTransferObject
        {
            InventoryId = inventory.InventoryId,
            IngredientId = inventory.Ingredient.IngredientId,
            IngredientName = inventory.Ingredient.Name,
            QuantityGrams = inventory.QuantityGrams,
        };
    }

    private static IngredientDataTransferObject MapToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDataTransferObject
        {
            IngredientId = ingredient.IngredientId,
            Name = ingredient.Name,
            CaloriesPer100g = ingredient.CaloriesPer100g,
            ProteinPer100g = ingredient.ProteinPer100g,
            CarbohydratesPer100g = ingredient.CarbohydratesPer100g,
            FatPer100g = ingredient.FatPer100g,
        };
    }
}
