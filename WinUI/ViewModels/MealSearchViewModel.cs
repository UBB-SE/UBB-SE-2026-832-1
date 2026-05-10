using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClassLibrary.Filters;
using ClassLibrary.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI.Services;

namespace WinUI.ViewModels
{
    public partial class MealSearchViewModel : ObservableObject
    {
        private readonly IMealService mealService;

        public ObservableCollection<FoodItem> Meals { get; private set; } = new ObservableCollection<FoodItem>();

        public string SearchTerm { get; set; } = string.Empty;

        public FoodItem? SelectedMeal { get; set; }

        public MealSearchViewModel(IMealService mealService)
        {
            this.mealService = mealService;
            _ = LoadMealsAsync();
        }

        public async Task LoadMealsAsync(string? filter = null)
        {
            var list = await mealService.SearchMealsAsync(new FoodItemFilter { SearchTerm = filter ?? string.Empty });
            Meals = new ObservableCollection<FoodItem>(list);
            OnPropertyChanged(nameof(Meals));
        }

        public async Task<List<FoodItem>> SearchMealsAsync(FoodItemFilter filter)
        {
            var list = await mealService.SearchMealsAsync(filter);
            Meals = new ObservableCollection<FoodItem>(list);
            OnPropertyChanged(nameof(Meals));
            return new List<FoodItem>(list);
        }

        public async Task<string> GetMealIngredientsTextAsync(int mealId)
        {
            var lines = await mealService.GetMealIngredientsAsync(mealId);
            return lines.Count > 0 ? string.Join("\n", lines) : "No ingredients found.";
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            await LoadMealsAsync(SearchTerm);
        }

        [RelayCommand]
        public async Task ToggleFavoriteAsync(FoodItem meal)
        {
            if (meal == null)
            {
                return;
            }

            int userId = UserSession.UserId ?? 1;
            await mealService.ToggleFavoriteAsync(userId, meal.FoodItemId);
        }
    }
}