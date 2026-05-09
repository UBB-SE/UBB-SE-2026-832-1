using System.Collections.ObjectModel;
using ClassLibrary.Filters;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.IServices;

namespace WinUI.ViewModels;

public partial class MealDetailViewModel : ObservableObject
{
    private const int PAGE_SIZE = 8;
    private readonly IMealService mealService;
    private readonly IUserSession userSession;
    private readonly List<FoodItem> allMeals = [];

    [ObservableProperty]
    private ObservableCollection<FoodItem> meals = [];

    [ObservableProperty]
    private FoodItem? selectedMeal;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isVeganFilterEnabled;

    [ObservableProperty]
    private bool isKetoFilterEnabled;

    [ObservableProperty]
    private bool isGlutenFreeFilterEnabled;

    [ObservableProperty]
    private bool isLactoseFreeFilterEnabled;

    [ObservableProperty]
    private bool isNutFreeFilterEnabled;

    [ObservableProperty]
    private bool isFavoritesOnlyFilterEnabled;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private string pageText = "Page 1";

    [ObservableProperty]
    private bool canGoToPreviousPage;

    [ObservableProperty]
    private bool canGoToNextPage;

    [ObservableProperty]
    private bool isSelectedMealVegan;

    [ObservableProperty]
    private bool isSelectedMealKeto;

    [ObservableProperty]
    private bool isSelectedMealGlutenFree;

    [ObservableProperty]
    private bool isSelectedMealLactoseFree;

    [ObservableProperty]
    private bool isSelectedMealNutFree;

    public MealDetailViewModel(IMealService mealService, UserSession userSession)
    {
        this.mealService = mealService;
        this.userSession = userSession;
    }

    partial void OnSelectedMealChanged(FoodItem? value)
    {
        this.IsSelectedMealVegan = value?.IsVegan == true;
        this.IsSelectedMealKeto = value?.IsKeto == true;
        this.IsSelectedMealGlutenFree = value?.IsGlutenFree == true;
        this.IsSelectedMealLactoseFree = value?.IsLactoseFree == true;
        this.IsSelectedMealNutFree = value?.IsNutFree == true;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            this.IsLoading = true;
            this.ErrorMessage = string.Empty;
            this.CurrentPage = 1;

            FoodItemFilter filter = new()
            {
                SearchTerm = this.SearchText,
                IsVegan = this.IsVeganFilterEnabled,
                IsKeto = this.IsKetoFilterEnabled,
                IsGlutenFree = this.IsGlutenFreeFilterEnabled,
                IsLactoseFree = this.IsLactoseFreeFilterEnabled,
                IsNutFree = this.IsNutFreeFilterEnabled,
                IsFavoriteOnly = this.IsFavoritesOnlyFilterEnabled,
            };

            IReadOnlyList<FoodItem> meals = await this.mealService.SearchMealsAsync(filter);
            this.allMeals.Clear();
            this.allMeals.AddRange(meals);
            this.RefreshVisibleMeals();
        }
        catch (Exception exception)
        {
            this.ErrorMessage = $"Failed to search meals: {exception.Message}";
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (!this.CanGoToPreviousPage)
        {
            return;
        }

        this.CurrentPage--;
        this.RefreshVisibleMeals();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (!this.CanGoToNextPage)
        {
            return;
        }

        this.CurrentPage++;
        this.RefreshVisibleMeals();
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(FoodItem? meal)
    {
        if (meal is null)
        {
            return;
        }

        meal.IsFavorite = !meal.IsFavorite;
        await this.mealService.ToggleFavoriteAsync(this.userSession.CurrentClientId, meal.FoodItemId);

        if (this.IsFavoritesOnlyFilterEnabled && !meal.IsFavorite)
        {
            await this.SearchAsync().ConfigureAwait(true);
        }
    }

    public async Task LoadAsync()
    {
        await this.SearchAsync().ConfigureAwait(true);
    }

    private void RefreshVisibleMeals()
    {
        this.Meals.Clear();

        int skipCount = (this.CurrentPage - 1) * PAGE_SIZE;
        foreach (FoodItem meal in this.allMeals.Skip(skipCount).Take(PAGE_SIZE))
        {
            this.Meals.Add(meal);
        }

        this.PageText = $"Page {this.CurrentPage}";
        this.CanGoToPreviousPage = this.CurrentPage > 1;
        this.CanGoToNextPage = this.CurrentPage * PAGE_SIZE < this.allMeals.Count;
        this.SelectedMeal = this.Meals.FirstOrDefault();
    }
}
