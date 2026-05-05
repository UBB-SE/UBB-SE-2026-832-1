using System.Collections.ObjectModel;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class ShoppingListViewModel : ObservableObject
{
    private const double defaultPendingQuantity = 100;
    private const int statusDisplayDurationMs = 3000;
    private const string statusAddSuccessFormat = "Updated '{0}' successfully!";
    private const string statusMoveToPantryFormat = "Moved '{0}' to Pantry.";
    private const string statusItemRemoved = "Item removed from list.";
    private const string statusAlreadyComplete = "You already have everything you need";
    private const string statusGenerateSuccessFormat = "Successfully generated {0} new items from your Meal Plan!";
    private const string errorAddItem = "Database error: Could not add item.";
    private const string errorMoveToPantry = "Failed to move item to Pantry.";
    private const string errorDeleteItem = "Failed to delete item from database.";
    private const string errorGenerateList = "Error analyzing Meal Plan for ingredients.";

    private readonly IShoppingListService shoppingListService;
    private readonly UserSession userSession;

    [ObservableProperty]
    private ObservableCollection<ShoppingListItem> items = [];

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool isStatusVisible;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private double pendingQuantity = defaultPendingQuantity;

    [ObservableProperty]
    private string pendingIngredientName = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    public ShoppingListViewModel(IShoppingListService shoppingListService, UserSession userSession)
    {
        this.shoppingListService = shoppingListService;
        this.userSession = userSession;
    }

    public async Task LoadItemsAsync()
    {
        int userId = this.userSession.CurrentClientId;
        if (userId <= 0)
        {
            return;
        }

        this.IsLoading = true;
        IReadOnlyList<ShoppingListItem> loadedItems = await this.shoppingListService.GetShoppingItemsAsync(userId);

        this.Items.Clear();
        foreach (ShoppingListItem item in loadedItems)
        {
            this.Items.Add(item);
        }

        this.IsLoading = false;
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        int userId = this.userSession.CurrentClientId;
        if (string.IsNullOrWhiteSpace(this.PendingIngredientName) || userId <= 0)
        {
            return;
        }

        ShoppingListItem? addedItem = await this.shoppingListService.AddItemAsync(
            this.PendingIngredientName.Trim(),
            userId,
            this.PendingQuantity);

        if (addedItem is null)
        {
            this.ShowStatus(errorAddItem, true);
            return;
        }

        await this.LoadItemsAsync().ConfigureAwait(true);
        this.ShowStatus(string.Format(statusAddSuccessFormat, this.PendingIngredientName), false);
        this.PendingIngredientName = string.Empty;
        this.PendingQuantity = defaultPendingQuantity;
    }

    [RelayCommand]
    private async Task MoveToPantryAsync(ShoppingListItem? item)
    {
        if (item is null || !this.Items.Contains(item))
        {
            return;
        }

        bool success = await this.shoppingListService.MoveToPantryAsync(item);
        if (success)
        {
            this.Items.Remove(item);
            this.ShowStatus(string.Format(statusMoveToPantryFormat, item.IngredientName), false);
            return;
        }

        this.ShowStatus(errorMoveToPantry, true);
    }

    [RelayCommand]
    private async Task RemoveItemAsync(ShoppingListItem? item)
    {
        if (item is null || !this.Items.Contains(item))
        {
            return;
        }

        bool success = await this.shoppingListService.RemoveItemAsync(item);
        if (success)
        {
            this.Items.Remove(item);
            this.ShowStatus(statusItemRemoved, false);
            return;
        }

        this.ShowStatus(errorDeleteItem, true);
    }

    [RelayCommand]
    private async Task GenerateListAsync()
    {
        int userId = this.userSession.CurrentClientId;
        if (userId <= 0)
        {
            return;
        }

        int itemsAdded = await this.shoppingListService.GenerateListAsync(userId);
        if (itemsAdded > 0)
        {
            await this.LoadItemsAsync().ConfigureAwait(true);
            this.ShowStatus(string.Format(statusGenerateSuccessFormat, itemsAdded), false);
            return;
        }

        if (itemsAdded == 0)
        {
            this.ShowStatus(statusAlreadyComplete, false);
            return;
        }

        this.ShowStatus(errorGenerateList, true);
    }

    public Task<IReadOnlyList<KeyValuePair<int, string>>> SearchIngredientsAsync(string query)
    {
        return this.shoppingListService.SearchIngredientsAsync(query);
    }

    private void ShowStatus(string message, bool error)
    {
        this.StatusMessage = message;
        this.IsError = error;
        this.IsStatusVisible = true;

        Task.Delay(statusDisplayDurationMs).ContinueWith(_ =>
        {
            this.IsStatusVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
