using ClassLibrary.DTOs; 
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WinUI.IServices; 

namespace WinUI.ViewModels
{
    public partial class MealsViewModel : ObservableObject
    {
        public int UserId { get; set; } = 1; //hardcoded for now, way too much of a headache to get the actual user id in here, and this is just a demo app


        private readonly IMealService _mealService;

        [ObservableProperty]
        private ObservableCollection<FoodItemDto> _meals = new();

        [ObservableProperty]
        private string _searchTerm = string.Empty;

        
        public MealsViewModel(IMealService mealService)
        {
            _mealService = mealService;
            _ = LoadMealsAsync();
        }

        [RelayCommand]
        public async Task LoadMealsAsync()
        {
            var filter = new MealFilterDto { SearchTerm = SearchTerm };

            
            var list = await _mealService.GetFilteredMealsAsync(filter, this.UserId);

            Meals.Clear();
            foreach (var item in list)
            {
                Meals.Add(item);
            }
        }

        [RelayCommand]
        public async Task ToggleFavoriteAsync(FoodItemDto meal)
        {
            if (meal == null) return;

            
            bool newStatus = !meal.IsFavorite;

            
            var request = new FavoriteRequestDto
            {
                UserId = this.UserId, 
                MealId = meal.FoodItemId,
                IsFavorite = newStatus
            };

            
            await _mealService.ToggleFavoriteAsync(request);

            
            meal.IsFavorite = newStatus;
        }

        public async Task<string> GetIngredientsTextAsync(int mealId)
        {
            
            return await _mealService.GetFormattedIngredientsAsync(mealId);
        }
    }
}