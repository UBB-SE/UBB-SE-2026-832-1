using ClassLibrary.DTOs;
using Moq;
using WinUI.Services;
using WinUI.ViewModels;

namespace Tests;

public sealed class PantryViewModelTests
{
    private const int TEST_USER_ID = 42;

    private readonly Mock<IInventoryService> mockInventoryService = new();

    private PantryViewModel CreateViewModel() =>
        new(this.mockInventoryService.Object, TEST_USER_ID);

    [Fact]
    public async Task LoadInventoryAsync_PopulatesItems()
    {
        var inventoryItems = new List<InventoryDataTransferObject>
        {
            new() { InventoryId = 1, IngredientName = "Chicken", QuantityGrams = 500 },
            new() { InventoryId = 2, IngredientName = "Rice", QuantityGrams = 300 },
        };

        this.mockInventoryService
            .Setup(service => service.GetUserInventoryAsync(TEST_USER_ID))
            .ReturnsAsync(inventoryItems);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadInventoryAsync();

        Assert.Equal(2, viewModel.Items.Count);
        Assert.Equal("Chicken", viewModel.Items[0].IngredientName);
        Assert.Equal("Rice", viewModel.Items[1].IngredientName);
    }

    [Fact]
    public async Task LoadInventoryAsync_WhenServiceThrows_SetsStatusMessage()
    {
        this.mockInventoryService
            .Setup(service => service.GetUserInventoryAsync(TEST_USER_ID))
            .ThrowsAsync(new Exception("Network error"));

        var viewModel = this.CreateViewModel();
        await viewModel.LoadInventoryAsync();

        Assert.Contains("Network error", viewModel.StatusMessage);
        Assert.Empty(viewModel.Items);
    }

    [Fact]
    public async Task LoadInventoryAsync_EmptyList_IsListEmptyReturnsTrue()
    {
        this.mockInventoryService
            .Setup(service => service.GetUserInventoryAsync(TEST_USER_ID))
            .ReturnsAsync(new List<InventoryDataTransferObject>());

        var viewModel = this.CreateViewModel();
        await viewModel.LoadInventoryAsync();

        Assert.True(viewModel.IsListEmpty);
    }

    [Fact]
    public async Task LoadInventoryAsync_WithItems_IsListEmptyReturnsFalse()
    {
        var inventoryItems = new List<InventoryDataTransferObject>
        {
            new() { InventoryId = 1, IngredientName = "Eggs", QuantityGrams = 200 },
        };

        this.mockInventoryService
            .Setup(service => service.GetUserInventoryAsync(TEST_USER_ID))
            .ReturnsAsync(inventoryItems);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadInventoryAsync();

        Assert.False(viewModel.IsListEmpty);
    }

    [Fact]
    public async Task LoadIngredientsAsync_PopulatesAvailableAndFiltered()
    {
        var ingredients = new List<IngredientDataTransferObject>
        {
            new() { IngredientId = 1, Name = "Flour" },
            new() { IngredientId = 2, Name = "Sugar" },
        };

        this.mockInventoryService
            .Setup(service => service.GetAllIngredientsAsync())
            .ReturnsAsync(ingredients);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadIngredientsAsync();

        Assert.Equal(2, viewModel.AvailableIngredients.Count);
        Assert.Equal(2, viewModel.FilteredIngredients.Count);
    }

    [Fact]
    public async Task IngredientSearchText_FiltersIngredients()
    {
        var ingredients = new List<IngredientDataTransferObject>
        {
            new() { IngredientId = 1, Name = "Flour" },
            new() { IngredientId = 2, Name = "Sugar" },
            new() { IngredientId = 3, Name = "Flower Seeds" },
        };

        this.mockInventoryService
            .Setup(service => service.GetAllIngredientsAsync())
            .ReturnsAsync(ingredients);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadIngredientsAsync();

        viewModel.IngredientSearchText = "Flo";

        Assert.Equal(2, viewModel.FilteredIngredients.Count);
        Assert.All(viewModel.FilteredIngredients, ingredient =>
            Assert.Contains("Flo", ingredient.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task IngredientSearchText_EmptyString_ShowsAllIngredients()
    {
        var ingredients = new List<IngredientDataTransferObject>
        {
            new() { IngredientId = 1, Name = "Flour" },
            new() { IngredientId = 2, Name = "Sugar" },
        };

        this.mockInventoryService
            .Setup(service => service.GetAllIngredientsAsync())
            .ReturnsAsync(ingredients);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadIngredientsAsync();

        viewModel.IngredientSearchText = "X";
        Assert.Empty(viewModel.FilteredIngredients);

        viewModel.IngredientSearchText = string.Empty;
        Assert.Equal(2, viewModel.FilteredIngredients.Count);
    }

    [Fact]
    public async Task AddNewIngredientAsync_NullSelectedIngredient_SetsStatusMessage()
    {
        var viewModel = this.CreateViewModel();
        viewModel.SelectedIngredient = null;

        await viewModel.AddNewIngredientAsync();

        Assert.Contains("choose an ingredient", viewModel.StatusMessage);
        this.mockInventoryService.Verify(
            service => service.AddToPantryAsync(It.IsAny<AddToPantryRequestDataTransferObject>()),
            Times.Never);
    }

    [Fact]
    public async Task AddNewIngredientAsync_ZeroQuantity_SetsStatusMessage()
    {
        var viewModel = this.CreateViewModel();
        viewModel.SelectedIngredient = new IngredientDataTransferObject { IngredientId = 1, Name = "Eggs" };
        viewModel.QuantityToAdd = 0;

        await viewModel.AddNewIngredientAsync();

        Assert.Contains("greater than 0", viewModel.StatusMessage);
        this.mockInventoryService.Verify(
            service => service.AddToPantryAsync(It.IsAny<AddToPantryRequestDataTransferObject>()),
            Times.Never);
    }

    [Fact]
    public async Task AddNewIngredientAsync_ValidInput_CallsServiceAndReloads()
    {
        this.mockInventoryService
            .Setup(service => service.GetUserInventoryAsync(TEST_USER_ID))
            .ReturnsAsync(new List<InventoryDataTransferObject>());

        var viewModel = this.CreateViewModel();
        viewModel.SelectedIngredient = new IngredientDataTransferObject { IngredientId = 5, Name = "Oats" };
        viewModel.QuantityToAdd = 250;

        await viewModel.AddNewIngredientAsync();

        this.mockInventoryService.Verify(
            service => service.AddToPantryAsync(
                It.Is<AddToPantryRequestDataTransferObject>(request =>
                    request.UserId == TEST_USER_ID
                    && request.IngredientId == 5
                    && request.QuantityGrams == 250)),
            Times.Once);

        Assert.Contains("Oats", viewModel.StatusMessage);
        Assert.Null(viewModel.SelectedIngredient);
        Assert.Equal(string.Empty, viewModel.IngredientSearchText);
    }

    [Fact]
    public async Task AddNewIngredientAsync_WhenServiceThrows_SetsErrorStatus()
    {
        this.mockInventoryService
            .Setup(service => service.AddToPantryAsync(It.IsAny<AddToPantryRequestDataTransferObject>()))
            .ThrowsAsync(new Exception("Server error"));

        var viewModel = this.CreateViewModel();
        viewModel.SelectedIngredient = new IngredientDataTransferObject { IngredientId = 1, Name = "Milk" };
        viewModel.QuantityToAdd = 500;

        await viewModel.AddNewIngredientAsync();

        Assert.Contains("Server error", viewModel.StatusMessage);
    }

    [Fact]
    public async Task RemoveItemAsync_NullItem_DoesNothing()
    {
        var viewModel = this.CreateViewModel();

        await viewModel.RemoveItemAsync(null);

        this.mockInventoryService.Verify(
            service => service.RemoveItemAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task RemoveItemAsync_ValidItem_RemovesFromCollection()
    {
        var item = new InventoryDataTransferObject
        {
            InventoryId = 10,
            IngredientName = "Butter",
            QuantityGrams = 200,
        };

        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.RemoveItemAsync(item);

        Assert.Empty(viewModel.Items);
        this.mockInventoryService.Verify(
            service => service.RemoveItemAsync(10),
            Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenServiceThrows_SetsErrorStatus()
    {
        this.mockInventoryService
            .Setup(service => service.RemoveItemAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Delete failed"));

        var item = new InventoryDataTransferObject { InventoryId = 1, IngredientName = "Salt", QuantityGrams = 50 };
        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.RemoveItemAsync(item);

        Assert.Contains("Delete failed", viewModel.StatusMessage);
    }
}
