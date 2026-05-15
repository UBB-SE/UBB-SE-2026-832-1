using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Filters;
using ClassLibrary.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using WinUI.ViewModels;

namespace WinUI.Views
{
    public sealed partial class MealsPage : Page
    {
        private readonly MealSearchViewModel mealSearchViewModel;

        private int currentPage = 1;
        private const int ItemsPerPage = 5;

        private List<FoodItem> allMeals = new();

        public MealsPage()
        {
            InitializeComponent();

            mealSearchViewModel = new MealSearchViewModel(
                new ClassLibrary.Proxies.MealProxy(
                    new System.Net.Http.HttpClient()));

            Loaded += MealsPage_Loaded;
        }

        private void MealsPage_Loaded(object sender, RoutedEventArgs e)
        {
            SearchButton_Click(this, new RoutedEventArgs());
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FoodItemFilter filter = new()
            {
                SearchTerm = searchTextBox.Text ?? string.Empty,

                IsVegan = veganCheckBox?.IsChecked == true,
                IsKeto = ketoCheckBox?.IsChecked == true,
                IsGlutenFree = glutenFreeCheckBox?.IsChecked == true,
                IsLactoseFree = lactoseFreeCheckBox?.IsChecked == true,
                IsNutFree = nutFreeCheckBox?.IsChecked == true,
                IsFavoriteOnly = favoritesOnlyCheckBox?.IsChecked == true
            };

            IEnumerable<FoodItem> searchResults =
                await mealSearchViewModel.SearchMealsAsync(filter);

            allMeals = searchResults.ToList();

            currentPage = 1;

            LoadMeals();
        }

        private void LoadMeals()
        {
            if (allMeals == null)
            {
                return;
            }

            List<FoodItem> pagedMeals = allMeals
                .Skip((currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            mealsListView.ItemsSource = null;
            mealsListView.ItemsSource = pagedMeals;

            int totalPages = (int)Math.Max(
                1,
                Math.Ceiling((double)allMeals.Count / ItemsPerPage));

            pageInformationTextBlock.Text =
                $"{currentPage} / {totalPages}";

            previousPageButton.IsEnabled = currentPage > 1;
            nextPageButton.IsEnabled = currentPage < totalPages;
        }

        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button favoriteButton)
            {
                return;
            }

            if (favoriteButton.DataContext is not FoodItem selectedMeal)
            {
                return;
            }

            selectedMeal.IsFavorite = !selectedMeal.IsFavorite;

            favoriteButton.Content =
                selectedMeal.IsFavorite ? "â˜…" : "â˜†";

            await mealSearchViewModel.ToggleFavoriteAsync(selectedMeal);

            if (favoritesOnlyCheckBox?.IsChecked == true &&
                !selectedMeal.IsFavorite)
            {
                allMeals.Remove(selectedMeal);

                LoadMeals();
            }
        }

        private async void MealsListView_ItemClick(
            object sender,
            ItemClickEventArgs e)
        {
            if (e.ClickedItem is not FoodItem selectedMeal)
            {
                return;
            }

            string ingredientsText =
                await mealSearchViewModel
                    .GetMealIngredientsTextAsync(selectedMeal.FoodItemId);

            StackPanel contentPanel = new()
            {
                Spacing = 10
            };

            if (!string.IsNullOrWhiteSpace(selectedMeal.ImageUrl))
            {
                contentPanel.Children.Add(
                    new Image
                    {
                        Source = new BitmapImage(
                            new Uri(selectedMeal.ImageUrl)),

                        Height = 150
                    });
            }

            contentPanel.Children.Add(
                new TextBlock
                {
                    Text =
                        $"Calories: {selectedMeal.Calories}\n" +
                        $"Protein: {selectedMeal.Protein}g\n" +
                        $"Carbohydrates: {selectedMeal.Carbohydrates}g\n" +
                        $"Fat: {selectedMeal.Fat}g\n\n" +
                        $"Ingredients:\n{ingredientsText}"
                });

            ContentDialog mealDialog = new()
            {
                Title = selectedMeal.Name,
                Content = contentPanel,
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };

            await mealDialog.ShowAsync();
        }

        private void FavoriteButton_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button favoriteButton)
            {
                return;
            }

            if (favoriteButton.DataContext is not FoodItem meal)
            {
                return;
            }

            favoriteButton.Content =
                meal.IsFavorite ? "â˜…" : "â˜†";
        }

        private void PreviousPageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentPage <= 1)
            {
                return;
            }

            currentPage--;

            LoadMeals();
        }

        private void NextPageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            int totalPages = (int)Math.Max(
                1,
                Math.Ceiling((double)allMeals.Count / ItemsPerPage));

            if (currentPage >= totalPages)
            {
                return;
            }

            currentPage++;

            LoadMeals();
        }

        private void SearchTextBox_KeyDown(
            object sender,
            KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SearchButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}

