using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class InventoryViewModel : ObservableObject
{
    private readonly IInventoryService inventoryService;

    [ObservableProperty]
    private ObservableCollection<InventoryDataTransferObject> inventoryItems = new();

    [ObservableProperty]
    private ObservableCollection<IngredientDataTransferObject> availableIngredients = new();

    [ObservableProperty]
    private InventoryDataTransferObject? selectedInventoryItem;

    [ObservableProperty]
    private IngredientDataTransferObject? selectedIngredient;

    [ObservableProperty]
    private string ingredientNameInput = string.Empty;

    [ObservableProperty]
    private int quantityGramsInput;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public InventoryViewModel(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
    }

    public async Task LoadUserInventoryAsync(int userId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;

            var items = await this.inventoryService.GetUserInventoryAsync(userId);

            this.InventoryItems.Clear();
            foreach (var item in items)
            {
                this.InventoryItems.Add(item);
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadAllIngredientsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var ingredients = await this.inventoryService.GetAllIngredientsAsync();

            this.AvailableIngredients.Clear();
            foreach (var ingredient in ingredients)
            {
                this.AvailableIngredients.Add(ingredient);
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddToPantryAsync(int userId)
    {
        if (this.SelectedIngredient == null || this.QuantityGramsInput <= 0)
        {
            ErrorMessage = "Please select an ingredient and enter a valid quantity.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var request = new AddToPantryRequestDataTransferObject
            {
                UserId = userId,
                IngredientId = this.SelectedIngredient.IngredientId,
                QuantityGrams = this.QuantityGramsInput,
            };

            await this.inventoryService.AddToPantryAsync(request);
            StatusMessage = "Ingredient added to pantry successfully.";
            await LoadUserInventoryAsync(userId);
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddIngredientByNameToPantryAsync(int userId)
    {
        if (string.IsNullOrWhiteSpace(this.IngredientNameInput))
        {
            ErrorMessage = "Please enter an ingredient name.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var request = new AddIngredientByNameRequestDataTransferObject
            {
                UserId = userId,
                IngredientName = this.IngredientNameInput,
            };

            await this.inventoryService.AddIngredientByNameToPantryAsync(request);
            StatusMessage = "Ingredient added to pantry successfully.";
            IngredientNameInput = string.Empty;
            await LoadUserInventoryAsync(userId);
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ConsumeMealAsync(int userId, int mealPlanId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var request = new ConsumeMealRequestDataTransferObject
            {
                UserId = userId,
                MealPlanId = mealPlanId,
            };

            bool success = await this.inventoryService.ConsumeMealAsync(request);

            if (success)
            {
                StatusMessage = "Meal consumed and inventory updated successfully.";
                await LoadUserInventoryAsync(userId);
            }
            else
            {
                ErrorMessage = "Could not consume meal. Not enough ingredients in pantry.";
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task RemoveSelectedItemAsync(int userId)
    {
        if (this.SelectedInventoryItem == null)
        {
            ErrorMessage = "Please select an inventory item to remove.";
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await this.inventoryService.RemoveItemAsync(this.SelectedInventoryItem.InventoryId);
            StatusMessage = "Item removed from inventory.";
            await LoadUserInventoryAsync(userId);
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
