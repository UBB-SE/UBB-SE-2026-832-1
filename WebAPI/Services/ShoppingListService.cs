using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using WebAPI.IServices;

namespace WebAPI.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListRepository shoppingRepository;
        private readonly IIngredientRepository ingredientRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IMealPlanRepository mealPlanRepository;

        public ShoppingListService(
            IShoppingListRepository shoppingRepository,
            IIngredientRepository ingredientRepository,
            IInventoryRepository inventoryRepository,
            IMealPlanRepository mealPlanRepository)
        {
            this.shoppingRepository = shoppingRepository;
            this.ingredientRepository = ingredientRepository;
            this.inventoryRepository = inventoryRepository;
            this.mealPlanRepository = mealPlanRepository;
        }

        public async Task GenerateShoppingListFromMealPlanAsync(int userId)
        {
            var latestMealPlan =
                (await mealPlanRepository.GetByUserIdAsync(userId))
                .OrderByDescending(
                    mealPlan => mealPlan.MealPlanId)
                .FirstOrDefault();

            if (latestMealPlan == null)
            {
                return;
            }

            var foodItemIngredients =
                await mealPlanRepository
                    .GetFoodItemIngredientsForMealPlanAsync(
                        latestMealPlan.MealPlanId);

            var inventoryItems =
                await inventoryRepository.GetAllByUserIdAsync(userId);

            var existingShoppingItems =
                await shoppingRepository.GetAllByUserIdAsync(userId);

            var inventoryByIngredientId = inventoryItems
                .GroupBy(
                    inventoryItem =>
                        inventoryItem.Ingredient.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(
                        inventoryItem =>
                            (double)inventoryItem.QuantityGrams));

            var shoppingByIngredientId = existingShoppingItems
                .GroupBy(
                    shoppingItem =>
                        shoppingItem.Ingredient.IngredientId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(
                        shoppingItem =>
                            shoppingItem.QuantityGrams));

            var requiredIngredients = foodItemIngredients
                .GroupBy(
                    foodItemIngredient =>
                        foodItemIngredient.Ingredient.IngredientId)
                .Select(group => new
                {
                    IngredientId = group.Key,

                    TotalRequiredGrams = group.Sum(
                        foodItemIngredient =>
                            foodItemIngredient.QuantityGrams)
                });

            foreach (var requiredIngredient in requiredIngredients)
            {
                var existingInventoryQty =
                    inventoryByIngredientId.GetValueOrDefault(
                        requiredIngredient.IngredientId,
                        0);

                var existingShoppingQty =
                    shoppingByIngredientId.GetValueOrDefault(
                        requiredIngredient.IngredientId,
                        0);

                var totalNeeded =
                    requiredIngredient.TotalRequiredGrams
                    - (existingInventoryQty + existingShoppingQty);

                if (totalNeeded <= 0)
                {
                    continue;
                }

                var item = new ShoppingItem
                {
                    User = new User
                    {
                        UserId = userId
                    },

                    Ingredient = new Ingredient
                    {
                        IngredientId =
                            requiredIngredient.IngredientId
                    },

                    QuantityGrams = totalNeeded,
                    IsChecked = false
                };

                await shoppingRepository.AddAsync(item);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await shoppingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsAsync(int userId)
        {
            var items = await shoppingRepository.GetAllByUserIdAsync(userId);
            return items.Select(i => new ShoppingItemDto
            {
                Id = i.ShoppingItemId,
                IngredientName = i.Ingredient?.Name ?? string.Empty,
                QuantityGrams = i.QuantityGrams,
                IsChecked = i.IsChecked
            });
        }

        public async Task<ShoppingItemDto?> AddItemAsync(int userId, AddShoppingItemRequest request)
        {
            int ingredientId = await ingredientRepository.GetOrCreateIngredientIdByNameAsync(request.ItemName);

            var item = new ShoppingItem
            {
                User = new User { UserId = userId },
                Ingredient = new Ingredient { IngredientId = ingredientId },
                QuantityGrams = request.Quantity
            };

            await shoppingRepository.AddAsync(item);

            return new ShoppingItemDto
            {
                Id = item.ShoppingItemId,
                IngredientName = request.ItemName,
                QuantityGrams = item.QuantityGrams
            };
        }

        public async Task<bool> MoveToPantryAsync(int itemId)
        {
            var item = await shoppingRepository.GetByIdAsync(itemId);
            if (item == null)
            {
                return false;
            }

            var userId = item.User?.UserId ?? 0;
            if (userId <= 0)
            {
                return false;
            }

            await inventoryRepository.AddAsync(new Inventory
            {
                UserId = userId,
                IngredientId = item.Ingredient.IngredientId,
                QuantityGrams = item.QuantityGrams > 0 ? (int)item.QuantityGrams : 100
            });

            await shoppingRepository.DeleteAsync(item.ShoppingItemId);
            return true;
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            try
            {
                await shoppingRepository.DeleteAsync(itemId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
