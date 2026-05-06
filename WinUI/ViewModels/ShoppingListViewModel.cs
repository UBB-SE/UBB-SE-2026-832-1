using System.Collections.ObjectModel;
using ClassLibrary.Models;
using UserSession = WinUI.Services.UserSession;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class ShoppingListViewModel : ObservableObject
{
    private const double DEFAULT_PENDING_QUANTITY = 100;
    private const int STATUS_DISPLAY_DURATION_MS = 3000;
    private const string STATUS_ADD_SUCCESS_FORMAT = "Updated '{0}' successfully!";
    private const string STATUS_MOVE_TO_PANTRY_FORMAT = "Moved '{0}' to Pantry.";
    private const string STATUS_ITEM_REMOVED = "Item removed from list.";
    private const string STATUS_ALREADY_COMPLETE = "You already have everything you need";
    private const string STATUS_GENERATE_SUCCESS_FORMAT = "Successfully generated {0} new items from your Meal Plan!";
    private const string ERROR_ADD_ITEM = "Database error: Could not add item.";
    private const string ERROR_MOVE_TO_PANTRY = "Failed to move item to Pantry.";
    private const string ERROR_DELETE_ITEM = "Failed to delete item from database.";
    private const string ERROR_GENERATE_LIST = "Error analyzing Meal Plan for ingredients.";

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
    private double pendingQuantity = DEFAULT_PENDING_QUANTITY;

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
            this.ShowStatus(ERROR_ADD_ITEM, true);
            return;
        }

        await this.LoadItemsAsync().ConfigureAwait(true);
        this.ShowStatus(string.Format(STATUS_ADD_SUCCESS_FORMAT, this.PendingIngredientName), false);
        this.PendingIngredientName = string.Empty;
        this.PendingQuantity = DEFAULT_PENDING_QUANTITY;
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
            this.ShowStatus(string.Format(STATUS_MOVE_TO_PANTRY_FORMAT, item.IngredientName), false);
            return;
        }

        this.ShowStatus(ERROR_MOVE_TO_PANTRY, true);
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
            this.ShowStatus(STATUS_ITEM_REMOVED, false);
            return;
        }

        this.ShowStatus(ERROR_DELETE_ITEM, true);
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
            this.ShowStatus(string.Format(STATUS_GENERATE_SUCCESS_FORMAT, itemsAdded), false);
            return;
        }

        if (itemsAdded == 0)
        {
            this.ShowStatus(STATUS_ALREADY_COMPLETE, false);
            return;
        }

        this.ShowStatus(ERROR_GENERATE_LIST, true);
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

        Task.Delay(STATUS_DISPLAY_DURATION_MS).ContinueWith(_ =>
        {
            this.IsStatusVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
