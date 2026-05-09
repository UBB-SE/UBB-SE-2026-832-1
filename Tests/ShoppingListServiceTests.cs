using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class ShoppingListServiceTests
{
    private readonly Mock<IShoppingListRepository> shoppingRepo = new();
    private readonly Mock<IIngredientRepository> ingredientRepo = new();
    private readonly Mock<IInventoryRepository> inventoryRepo = new();
    private readonly Mock<IMealPlanRepository> mealPlanRepo = new();

    private ShoppingListService CreateService() => new(
        this.shoppingRepo.Object,
        this.ingredientRepo.Object,
        this.inventoryRepo.Object,
        this.mealPlanRepo.Object);

    [Fact]
    public async Task AddItemAsync_SetsUserIdAndIngredientIdDirectly()
    {
        this.ingredientRepo
            .Setup(repo => repo.GetOrCreateIngredientIdByNameAsync("Milk"))
            .ReturnsAsync(15);

        ShoppingItem? captured = null;
        this.shoppingRepo
            .Setup(repo => repo.AddAsync(It.IsAny<ShoppingItem>()))
            .Callback<ShoppingItem>(item => captured = item);

        var request = new AddShoppingItemRequest
        {
            ItemName = "Milk",
            Quantity = 500,
        };

        var service = this.CreateService();
        await service.AddItemAsync(3, request);

        Assert.NotNull(captured);
        Assert.Equal(3, captured.UserId);
        Assert.Equal(15, captured.IngredientId);
        Assert.Equal(500, captured.QuantityGrams);
        Assert.Null(captured.User);
        Assert.Null(captured.Ingredient);
    }

    [Fact]
    public async Task AddItemAsync_DoesNotCreateStubNavigationEntities()
    {
        this.ingredientRepo
            .Setup(repo => repo.GetOrCreateIngredientIdByNameAsync("Eggs"))
            .ReturnsAsync(22);

        ShoppingItem? captured = null;
        this.shoppingRepo
            .Setup(repo => repo.AddAsync(It.IsAny<ShoppingItem>()))
            .Callback<ShoppingItem>(item => captured = item);

        var request = new AddShoppingItemRequest
        {
            ItemName = "Eggs",
            Quantity = 6,
        };

        var service = this.CreateService();
        await service.AddItemAsync(1, request);

        Assert.NotNull(captured);
        Assert.Null(captured.User);
        Assert.Null(captured.Ingredient);
    }

    [Fact]
    public async Task GenerateShoppingListFromMealPlanAsync_SetsUserIdAndIngredientIdDirectly()
    {
        var mealPlan = new MealPlan { MealPlanId = 10 };
        this.mealPlanRepo
            .Setup(repo => repo.GetByUserIdAsync(5))
            .ReturnsAsync(new List<MealPlan> { mealPlan });

        this.mealPlanRepo
            .Setup(repo => repo.GetIngredientIdsForMealPlanAsync(10))
            .ReturnsAsync(new List<int> { 99 });

        this.inventoryRepo
            .Setup(repo => repo.GetAllByUserIdAsync(5))
            .ReturnsAsync(new List<Inventory>());

        this.shoppingRepo
            .Setup(repo => repo.GetAllByUserIdAsync(5))
            .ReturnsAsync(new List<ShoppingItem>());

        ShoppingItem? captured = null;
        this.shoppingRepo
            .Setup(repo => repo.AddAsync(It.IsAny<ShoppingItem>()))
            .Callback<ShoppingItem>(item => captured = item);

        var service = this.CreateService();
        await service.GenerateShoppingListFromMealPlanAsync(5);

        Assert.NotNull(captured);
        Assert.Equal(5, captured.UserId);
        Assert.Equal(99, captured.IngredientId);
        Assert.Null(captured.User);
        Assert.Null(captured.Ingredient);
    }
}
