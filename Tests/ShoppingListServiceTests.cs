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
    public async Task AddItemAsync_SetsNavigationPropertiesCorrectly()
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
        Assert.NotNull(captured.User);
        Assert.Equal(3, captured.User.UserId);
        Assert.NotNull(captured.Ingredient);
        Assert.Equal(15, captured.Ingredient.IngredientId);
        Assert.Equal(500, captured.QuantityGrams);
    }

    [Fact]
    public async Task AddItemAsync_ReturnsDto()
    {
        this.ingredientRepo
            .Setup(repo => repo.GetOrCreateIngredientIdByNameAsync("Eggs"))
            .ReturnsAsync(22);

        this.shoppingRepo
            .Setup(repo => repo.AddAsync(It.IsAny<ShoppingItem>()));

        var request = new AddShoppingItemRequest
        {
            ItemName = "Eggs",
            Quantity = 6,
        };

        var service = this.CreateService();
        var result = await service.AddItemAsync(1, request);

        Assert.NotNull(result);
        Assert.Equal("Eggs", result.IngredientName);
        Assert.Equal(6, result.QuantityGrams);
    }
}
