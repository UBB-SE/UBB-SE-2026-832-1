using ClassLibrary.DTOs;
using ClassLibrary.IRepositories;
using ClassLibrary.Models;
using Moq;
using WebAPI.Services;

namespace Tests;

public sealed class InventoryServiceTests
{
    private readonly Mock<IIngredientRepository> ingredientRepo = new();
    private readonly Mock<IInventoryRepository> inventoryRepo = new();
    private readonly Mock<IMealPlanRepository> mealPlanRepo = new();

    private InventoryService CreateService() => new(
        this.ingredientRepo.Object,
        this.inventoryRepo.Object,
        this.mealPlanRepo.Object);

    [Fact]
    public async Task AddToPantryAsync_SetsUserIdDirectly()
    {
        var request = new AddToPantryRequestDataTransferObject
        {
            UserId = 7,
            IngredientId = 42,
            QuantityGrams = 250,
        };

        Inventory? captured = null;
        this.inventoryRepo
            .Setup(repo => repo.AddAsync(It.IsAny<Inventory>()))
            .Callback<Inventory>(inv => captured = inv);

        var service = this.CreateService();
        await service.AddToPantryAsync(request);

        Assert.NotNull(captured);
        Assert.Equal(7, captured.UserId);
        Assert.Equal(42, captured.IngredientId);
        Assert.Equal(250, captured.QuantityGrams);
        Assert.Null(captured.User);
        Assert.Null(captured.Ingredient);
    }

    [Fact]
    public async Task AddToPantryAsync_DoesNotCreateStubNavigationEntities()
    {
        var request = new AddToPantryRequestDataTransferObject
        {
            UserId = 1,
            IngredientId = 10,
            QuantityGrams = 100,
        };

        Inventory? captured = null;
        this.inventoryRepo
            .Setup(repo => repo.AddAsync(It.IsAny<Inventory>()))
            .Callback<Inventory>(inv => captured = inv);

        var service = this.CreateService();
        await service.AddToPantryAsync(request);

        Assert.NotNull(captured);
        Assert.Null(captured.User);
        Assert.Null(captured.Ingredient);
    }

    [Fact]
    public async Task AddIngredientByNameToPantryAsync_ResolvesIngredientIdAndDelegates()
    {
        this.ingredientRepo
            .Setup(repo => repo.GetOrCreateIngredientIdByNameAsync("Chicken"))
            .ReturnsAsync(55);

        Inventory? captured = null;
        this.inventoryRepo
            .Setup(repo => repo.AddAsync(It.IsAny<Inventory>()))
            .Callback<Inventory>(inv => captured = inv);

        var request = new AddIngredientByNameRequestDataTransferObject
        {
            UserId = 3,
            IngredientName = "Chicken",
        };

        var service = this.CreateService();
        await service.AddIngredientByNameToPantryAsync(request);

        Assert.NotNull(captured);
        Assert.Equal(3, captured.UserId);
        Assert.Equal(55, captured.IngredientId);
        Assert.Equal(100, captured.QuantityGrams);
    }
}
