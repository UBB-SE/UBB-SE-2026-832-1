using ClassLibrary.Models;
using Moq;
using WinUI.Services;
using WinUI.ViewModels;

namespace Tests;

public sealed class ShoppingListViewModelTests
{
    private readonly Mock<IShoppingListProxy> mockShoppingListProxy = new();
    private readonly UserSession userSession = new();

    private ShoppingListViewModel CreateViewModel() =>
        new(this.mockShoppingListProxy.Object, this.userSession);

    [Fact]
    public async Task LoadItemsAsync_PopulatesItemsCollection()
    {
        var items = new List<ShoppingListItem>
        {
            new() { ShoppingListItemId = 1, IngredientName = "Milk", QuantityGrams = 1000 },
            new() { ShoppingListItemId = 2, IngredientName = "Bread", QuantityGrams = 500 },
        };

        this.mockShoppingListProxy
            .Setup(service => service.GetShoppingItemsAsync(It.IsAny<int>()))
            .ReturnsAsync(items);

        var viewModel = this.CreateViewModel();
        await viewModel.LoadItemsAsync();

        Assert.Equal(2, viewModel.Items.Count);
        Assert.Equal("Milk", viewModel.Items[0].IngredientName);
    }

    [Fact]
    public async Task AddItemCommand_NullIngredient_ShowsErrorStatus()
    {
        this.mockShoppingListProxy
            .Setup(service => service.AddItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>()))
            .ReturnsAsync((ShoppingListItem?)null);

        var viewModel = this.CreateViewModel();
        viewModel.PendingIngredientName = "Eggs";

        await viewModel.AddItemCommand.ExecuteAsync(null);

        Assert.True(viewModel.IsStatusVisible);
        Assert.Contains("Could not add item", viewModel.StatusMessage);
    }

    [Fact]
    public async Task AddItemCommand_Success_ClearsInputAndReloads()
    {
        var addedItem = new ShoppingListItem
        {
            ShoppingListItemId = 1,
            IngredientName = "Sugar",
            QuantityGrams = 500,
        };

        this.mockShoppingListProxy
            .Setup(service => service.AddItemAsync("Sugar", It.IsAny<int>(), 200))
            .ReturnsAsync(addedItem);

        this.mockShoppingListProxy
            .Setup(service => service.GetShoppingItemsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ShoppingListItem> { addedItem });

        var viewModel = this.CreateViewModel();
        viewModel.PendingIngredientName = "Sugar";
        viewModel.PendingQuantity = 200;

        await viewModel.AddItemCommand.ExecuteAsync(null);

        Assert.Equal(string.Empty, viewModel.PendingIngredientName);
        Assert.Contains("Sugar", viewModel.StatusMessage);
    }

    [Fact]
    public async Task AddItemCommand_EmptyName_DoesNotCallService()
    {
        var viewModel = this.CreateViewModel();
        viewModel.PendingIngredientName = "   ";

        await viewModel.AddItemCommand.ExecuteAsync(null);

        this.mockShoppingListProxy.Verify(
            service => service.AddItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<double>()),
            Times.Never);
    }

    [Fact]
    public async Task MoveToPantryCommand_Success_RemovesItemAndShowsStatus()
    {
        var item = new ShoppingListItem
        {
            ShoppingListItemId = 1,
            IngredientName = "Butter",
            QuantityGrams = 250,
        };

        this.mockShoppingListProxy
            .Setup(service => service.MoveToPantryAsync(item))
            .ReturnsAsync(true);

        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.MoveToPantryCommand.ExecuteAsync(item);

        Assert.Empty(viewModel.Items);
        Assert.Contains("Butter", viewModel.StatusMessage);
        Assert.Contains("Pantry", viewModel.StatusMessage);
    }

    [Fact]
    public async Task MoveToPantryCommand_Failure_ShowsErrorStatus()
    {
        var item = new ShoppingListItem
        {
            ShoppingListItemId = 1,
            IngredientName = "Cheese",
            QuantityGrams = 300,
        };

        this.mockShoppingListProxy
            .Setup(service => service.MoveToPantryAsync(item))
            .ReturnsAsync(false);

        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.MoveToPantryCommand.ExecuteAsync(item);

        Assert.Single(viewModel.Items);
        Assert.Contains("Failed", viewModel.StatusMessage);
    }

    [Fact]
    public async Task MoveToPantryCommand_NullItem_DoesNotCallService()
    {
        var viewModel = this.CreateViewModel();

        await viewModel.MoveToPantryCommand.ExecuteAsync(null);

        this.mockShoppingListProxy.Verify(
            service => service.MoveToPantryAsync(It.IsAny<ShoppingListItem>()),
            Times.Never);
    }

    [Fact]
    public async Task RemoveItemCommand_Success_RemovesItemFromCollection()
    {
        var item = new ShoppingListItem
        {
            ShoppingListItemId = 5,
            IngredientName = "Flour",
            QuantityGrams = 1000,
        };

        this.mockShoppingListProxy
            .Setup(service => service.RemoveItemAsync(item))
            .ReturnsAsync(true);

        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.RemoveItemCommand.ExecuteAsync(item);

        Assert.Empty(viewModel.Items);
        Assert.Contains("removed", viewModel.StatusMessage);
    }

    [Fact]
    public async Task RemoveItemCommand_Failure_ShowsError()
    {
        var item = new ShoppingListItem
        {
            ShoppingListItemId = 5,
            IngredientName = "Oil",
            QuantityGrams = 500,
        };

        this.mockShoppingListProxy
            .Setup(service => service.RemoveItemAsync(item))
            .ReturnsAsync(false);

        var viewModel = this.CreateViewModel();
        viewModel.Items.Add(item);

        await viewModel.RemoveItemCommand.ExecuteAsync(item);

        Assert.Single(viewModel.Items);
        Assert.Contains("Failed to delete", viewModel.StatusMessage);
    }

    [Fact]
    public async Task GenerateListCommand_ItemsAdded_ReloadsAndShowsSuccess()
    {
        this.mockShoppingListProxy
            .Setup(service => service.GenerateListAsync(It.IsAny<int>()))
            .ReturnsAsync(3);

        this.mockShoppingListProxy
            .Setup(service => service.GetShoppingItemsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ShoppingListItem>());

        var viewModel = this.CreateViewModel();

        await viewModel.GenerateListCommand.ExecuteAsync(null);

        Assert.Contains("3", viewModel.StatusMessage);
    }

    [Fact]
    public async Task GenerateListCommand_ZeroItems_ShowsAlreadyComplete()
    {
        this.mockShoppingListProxy
            .Setup(service => service.GenerateListAsync(It.IsAny<int>()))
            .ReturnsAsync(0);

        var viewModel = this.CreateViewModel();

        await viewModel.GenerateListCommand.ExecuteAsync(null);

        Assert.Contains("already have everything", viewModel.StatusMessage);
    }

    [Fact]
    public async Task GenerateListCommand_NegativeResult_ShowsError()
    {
        this.mockShoppingListProxy
            .Setup(service => service.GenerateListAsync(It.IsAny<int>()))
            .ReturnsAsync(-1);

        var viewModel = this.CreateViewModel();

        await viewModel.GenerateListCommand.ExecuteAsync(null);

        Assert.Contains("Error", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SearchIngredientsAsync_DelegatesToService()
    {
        var expected = new List<KeyValuePair<int, string>>
        {
            new(1, "Apple"),
            new(2, "Apricot"),
        };

        this.mockShoppingListProxy
            .Setup(service => service.SearchIngredientsAsync("Ap"))
            .ReturnsAsync(expected);

        var viewModel = this.CreateViewModel();
        var results = await viewModel.SearchIngredientsAsync("Ap");

        Assert.Equal(2, results.Count);
        Assert.Equal("Apple", results[0].Value);
    }
}
