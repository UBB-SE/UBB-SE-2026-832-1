using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI.Services;

namespace WinUI.ViewModels;

public partial class MealPlanViewModel : ObservableObject
{
    private readonly IMealPlanService mealPlanService;

    [ObservableProperty]
    private ObservableCollection<MealPlanDto> mealPlans = new();

    [ObservableProperty]
    private MealPlanDto? selectedMealPlan;

    [ObservableProperty]
    private ObservableCollection<FoodItemDto> foodItems = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public MealPlanViewModel(IMealPlanService mealPlanService)
    {
        this.mealPlanService = mealPlanService ?? throw new ArgumentNullException(nameof(mealPlanService));
    }

    public async Task LoadMealPlansForUserAsync(int userId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var plans = await this.mealPlanService.GetByUserIdAsync(userId);

            this.MealPlans.Clear();
            foreach (var plan in plans)
            {
                this.MealPlans.Add(plan);
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

    public async Task LoadFoodItemsForSelectedPlanAsync()
    {
        if (this.SelectedMealPlan == null)
        {
            this.FoodItems.Clear();
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var foodItems = await this.mealPlanService.GetFoodItemsForPlanAsync(this.SelectedMealPlan.MealPlanId);

            this.FoodItems.Clear();
            foreach (var foodItem in foodItems)
            {
                this.FoodItems.Add(foodItem);
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

    public async Task AddFoodItemToPlanAsync(int foodItemId)
    {
        if (this.SelectedMealPlan == null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await this.mealPlanService.AddFoodItemToPlanAsync(this.SelectedMealPlan.MealPlanId, foodItemId);
            await LoadFoodItemsForSelectedPlanAsync();
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

    public async Task RemoveFoodItemFromPlanAsync(int foodItemId)
    {
        if (this.SelectedMealPlan == null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            await this.mealPlanService.RemoveFoodItemFromPlanAsync(this.SelectedMealPlan.MealPlanId, foodItemId);
            await LoadFoodItemsForSelectedPlanAsync();
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
