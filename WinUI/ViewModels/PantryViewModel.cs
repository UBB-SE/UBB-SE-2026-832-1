using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class PantryViewModel : ObservableObject
{
    private const double DEFAULT_QUANTITY_TO_ADD = 100;
    private const double MIN_QUANTITY_ALLOWED = 0;
    private const string EMPTY_INVENTORY_MESSAGE = "Your pantry is empty. Start adding items!";
    private const string SELECT_INGREDIENT_MESSAGE = "Please choose an ingredient from suggestions.";
    private const string INVALID_QUANTITY_MESSAGE = "Quantity must be greater than 0.";
    private const string LOAD_INVENTORY_ERROR_FORMAT = "Error loading inventory: {0}";
    private const string LOAD_INGREDIENTS_ERROR_FORMAT = "Error loading ingredients: {0}";
    private const string DELETE_ITEM_ERROR_FORMAT = "Could not delete item: {0}";
    private const string ADD_ITEM_ERROR_FORMAT = "Could not add item: {0}";
    private const string ADD_ITEM_SUCCESS_FORMAT = "Added {0}g of {1}.";

    private readonly IInventoryService inventoryService;
    private readonly int currentUserId;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string emptyListMessage = EMPTY_INVENTORY_MESSAGE;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private string ingredientSearchText = string.Empty;

    [ObservableProperty]
    private IngredientDataTransferObject? selectedIngredient;

    [ObservableProperty]
    private double quantityToAdd = DEFAULT_QUANTITY_TO_ADD;

    public ObservableCollection<InventoryDataTransferObject> Items { get; } = new();

    public ObservableCollection<IngredientDataTransferObject> AvailableIngredients { get; } = new();

    public ObservableCollection<IngredientDataTransferObject> FilteredIngredients { get; } = new();

    public bool IsListEmpty => !this.Items.Any();

    public PantryViewModel(IInventoryService inventoryService, int currentUserId)
    {
        this.inventoryService = inventoryService;
        this.currentUserId = currentUserId;
    }

    partial void OnIngredientSearchTextChanged(string value)
    {
        if (this.SelectedIngredient is not null &&
            !this.SelectedIngredient.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            this.SelectedIngredient = null;
        }

        this.UpdateFilteredIngredients();
    }

    [RelayCommand]
    public async Task LoadInventoryAsync()
    {
        if (this.IsBusy)
        {
            return;
        }

        try
        {
            this.IsBusy = true;
            var inventoryItems = await this.inventoryService.GetUserInventoryAsync(this.currentUserId);

            this.Items.Clear();
            foreach (var item in inventoryItems)
            {
                this.Items.Add(item);
            }

            OnPropertyChanged(nameof(this.IsListEmpty));
        }
        catch (Exception exception)
        {
            this.StatusMessage = string.Format(LOAD_INVENTORY_ERROR_FORMAT, exception.Message);
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RemoveItemAsync(InventoryDataTransferObject? item)
    {
        if (item is null)
        {
            return;
        }

        try
        {
            await this.inventoryService.RemoveItemAsync(item.InventoryId);
            this.Items.Remove(item);
            OnPropertyChanged(nameof(this.IsListEmpty));
        }
        catch (Exception exception)
        {
            this.StatusMessage = string.Format(DELETE_ITEM_ERROR_FORMAT, exception.Message);
        }
    }

    [RelayCommand]
    public async Task AddNewIngredientAsync()
    {
        if (this.SelectedIngredient is null)
        {
            this.StatusMessage = SELECT_INGREDIENT_MESSAGE;
            return;
        }

        if (this.QuantityToAdd <= MIN_QUANTITY_ALLOWED)
        {
            this.StatusMessage = INVALID_QUANTITY_MESSAGE;
            return;
        }

        try
        {
            int quantityGrams = (int)Math.Round(this.QuantityToAdd);
            var request = new AddToPantryRequestDataTransferObject
            {
                UserId = this.currentUserId,
                IngredientId = this.SelectedIngredient.IngredientId,
                QuantityGrams = quantityGrams,
            };

            await this.inventoryService.AddToPantryAsync(request);
            await this.LoadInventoryAsync();

            this.StatusMessage = string.Format(
                ADD_ITEM_SUCCESS_FORMAT,
                quantityGrams,
                this.SelectedIngredient.Name);

            this.IngredientSearchText = string.Empty;
            this.SelectedIngredient = null;
            this.UpdateFilteredIngredients();
        }
        catch (Exception exception)
        {
            this.StatusMessage = string.Format(ADD_ITEM_ERROR_FORMAT, exception.Message);
        }
    }

    [RelayCommand]
    public async Task LoadIngredientsAsync()
    {
        try
        {
            var ingredients = await this.inventoryService.GetAllIngredientsAsync();
            this.AvailableIngredients.Clear();

            foreach (var ingredient in ingredients)
            {
                this.AvailableIngredients.Add(ingredient);
            }

            this.UpdateFilteredIngredients();
        }
        catch (Exception exception)
        {
            this.StatusMessage = string.Format(LOAD_INGREDIENTS_ERROR_FORMAT, exception.Message);
        }
    }

    private void UpdateFilteredIngredients()
    {
        this.FilteredIngredients.Clear();

        string query = this.IngredientSearchText?.Trim() ?? string.Empty;

        var source = string.IsNullOrEmpty(query)
            ? this.AvailableIngredients
            : new ObservableCollection<IngredientDataTransferObject>(
                this.AvailableIngredients.Where(ingredient =>
                    ingredient.Name.Contains(query, StringComparison.OrdinalIgnoreCase)));

        foreach (var ingredient in source)
        {
            this.FilteredIngredients.Add(ingredient);
        }
    }
}
