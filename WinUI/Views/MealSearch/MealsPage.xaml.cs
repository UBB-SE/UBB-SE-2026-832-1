using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Filters;
using ClassLibrary.Models;
using WinUI.ViewModels;

namespace WinUI.Views
{
    public sealed partial class MealsPage : Page
    {
        private MealSearchViewModel viewModel;
        private int currentPage = 1;
        private int pageSize = 5;
        private List<FoodItem> allMeals = new List<FoodItem>();

        public MealsPage()
        {
            this.InitializeComponent();
            
            // Assuming default HttpClient resolution
            viewModel = new MealSearchViewModel(new WinUI.Services.MealService(new System.Net.Http.HttpClient()));
            
            Loaded += (s, e) => btnSearch_Click(this, new RoutedEventArgs());
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var filter = new FoodItemFilter
            {
                SearchTerm = txtSearch.Text ?? "",
                IsVegan = chkVegan?.IsChecked == true,
                IsKeto = chkKeto?.IsChecked == true,
                IsGlutenFree = chkGlutenFree?.IsChecked == true,
                IsLactoseFree = chkLactoseFree?.IsChecked == true,
                IsNutFree = chkNutFree?.IsChecked == true,
                IsFavoriteOnly = chkFavorites?.IsChecked == true 
            };

            var results = await viewModel.SearchMealsAsync(filter);

            allMeals = results.ToList();
            currentPage = 1;
            LoadMeals();
        }

        private void LoadMeals()
        {
            if (allMeals == null) return;

            var paged = allMeals
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            listMeals.ItemsSource = null;
            listMeals.ItemsSource = paged;

            int totalPages = (int)Math.Max(1, Math.Ceiling((double)allMeals.Count / pageSize));
            txtPage.Text = $"{currentPage} / {totalPages}";

            btnPrev.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = currentPage < totalPages;
        }

        private async void Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is FoodItem meal)
            {
                meal.IsFavorite = !meal.IsFavorite;
                btn.Content = meal.IsFavorite ? "★" : "☆";
                
                await viewModel.ToggleFavoriteAsync(meal);

                if (chkFavorites?.IsChecked == true && !meal.IsFavorite)
                {
                    allMeals.Remove(meal);
                    LoadMeals();
                }
            }
        }

        private async void listMeals_ItemClick(object sender, ItemClickEventArgs e)
        {
            var meal = e.ClickedItem as FoodItem;
            if (meal == null) return;
            var ingredientsText = await viewModel.GetMealIngredientsTextAsync(meal.FoodItemId);
            var panel = new StackPanel { Spacing = 10 };
            if (!string.IsNullOrEmpty(meal.ImageUrl))
            {
                panel.Children.Add(new Image { Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(meal.ImageUrl)), Height = 150 });
            }
            panel.Children.Add(new TextBlock { Text = $"Calories: {meal.Calories}\nProtein: {meal.Protein}g\nCarbohydrates: {meal.Carbohydrates}g\nFat: {meal.Fat}g\n\nIngredients:\n{ingredientsText}" });
            ContentDialog dialog = new ContentDialog { Title = meal.Name, Content = panel, CloseButtonText = "Close", XamlRoot = this.XamlRoot };
            await dialog.ShowAsync();
        }

        private void Favorite_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is FoodItem meal)
            {
                btn.Content = meal.IsFavorite ? "★" : "☆";
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; LoadMeals(); } }
        private void Next_Click(object sender, RoutedEventArgs e) { int totalPages = (int)Math.Max(1, Math.Ceiling((double)allMeals.Count / pageSize)); if (currentPage < totalPages) { currentPage++; LoadMeals(); } }
        private void txtSearch_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e) { if (e.Key == Windows.System.VirtualKey.Enter) btnSearch_Click(this, new RoutedEventArgs()); }
    }
}
