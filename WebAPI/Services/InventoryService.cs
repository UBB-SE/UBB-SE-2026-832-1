using System.Collections.Generic;
using System.Linq;
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

    private const int DEFAULT_INGREDIENTS_QUANTITY = 100;

    public InventoryService(
        IIngredientRepository ingredientRepository,
        IInventoryRepository inventoryRepository,
        IMealPlanRepository mealPlanRepository)
    {
        this.inventoryRepository = inventoryRepository;
        this.mealPlanRepository = mealPlanRepository;
        this.ingredientRepository = ingredientRepository;
    }

    public async Task<bool> ConsumeMealAsync(ConsumeMealRequestDataTransferObject request)
    {
        var requiredIngredientIds = await this.mealPlanRepository.GetIngredientIdsForMealPlanAsync(request.MealPlanId);
        var requiredQuantityByIngredientId = requiredIngredientIds
            .GroupBy(ingredientId => ingredientId)
            .ToDictionary(
                group => group.Key,
                group => group.Count() * DEFAULT_INGREDIENTS_QUANTITY);

        var inventoryByIngredientId = (await this.inventoryRepository.GetAllByUserIdAsync(request.UserId))
            .GroupBy(inventoryItem => inventoryItem.Ingredient.IngredientId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(inventoryItem => inventoryItem.InventoryId).ToList());

        foreach (var requiredIngredient in requiredQuantityByIngredientId)
        {
            if (!inventoryByIngredientId.TryGetValue(requiredIngredient.Key, out var stocks) ||
                stocks.Sum(stock => stock.QuantityGrams) < requiredIngredient.Value)
            {
                return false;
            }
        }

        foreach (var requiredIngredient in requiredQuantityByIngredientId)
        {
            int remainingQuantityToConsume = requiredIngredient.Value;

            foreach (var stock in inventoryByIngredientId[requiredIngredient.Key])
            {
                if (remainingQuantityToConsume <= 0)
                {
                    break;
                }

                int quantityToConsume = stock.QuantityGrams >= remainingQuantityToConsume
                    ? remainingQuantityToConsume
                    : stock.QuantityGrams;

                stock.QuantityGrams -= quantityToConsume;
                remainingQuantityToConsume -= quantityToConsume;

                if (stock.QuantityGrams <= 0)
                {
                    await this.inventoryRepository.DeleteAsync(stock.InventoryId);
                }
                else
                {
                    await this.inventoryRepository.UpdateAsync(stock);
                }
            }
        }

        return true;
    }

    public async Task AddToPantryAsync(AddToPantryRequestDataTransferObject request)
    {
        var newItem = new Inventory
        {
            User = new User { UserId = request.UserId },
            Ingredient = new Ingredient { IngredientId = request.IngredientId },
            QuantityGrams = request.QuantityGrams,
        };

        await this.inventoryRepository.AddAsync(newItem);
    }

    public async Task AddIngredientByNameToPantryAsync(AddIngredientByNameRequestDataTransferObject request)
    {
        int ingredientId = await this.ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.IngredientName);
        await this.AddToPantryAsync(new AddToPantryRequestDataTransferObject
        {
            UserId = request.UserId,
            IngredientId = ingredientId,
            QuantityGrams = DEFAULT_INGREDIENTS_QUANTITY,
        });
    }

    public async Task<IReadOnlyList<InventoryDataTransferObject>> GetUserInventoryAsync(int userId)
    {
        var inventoryItems = await this.inventoryRepository.GetAllByUserIdAsync(userId);
        return inventoryItems.Select(MapToInventoryDto).ToList();
    }

    public async Task RemoveItemAsync(int inventoryId)
    {
        await this.inventoryRepository.DeleteAsync(inventoryId);
    }

    public async Task<IReadOnlyList<IngredientDataTransferObject>> GetAllIngredientsAsync()
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
