using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.DTOs;        
using ClassLibrary.Models;      
using WinUI.ViewModels;         

namespace WinUI.Views
{
    public sealed partial class MealsPage : Page
    {
        
        public MealsPage ViewModel { get; }

        private int currentPage = 1;
        private int pageSize = 5;
        private List<FoodItemDto> allMeals = new List<FoodItemDto>();

        public MealsPage()
        {
            this.InitializeComponent();


            ///this.ViewModel = App.GetService<MealsViewModel>();
            this.DataContext = ViewModel;

            
           // this.Loaded += (s, e) => btnSearch_Click(this, new RoutedEventArgs());
        }

       /* private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            var filter = new MealFilterDto
            {
                SearchTerm = txtSearch.Text ?? "",
                IsVegan = chkVegan?.IsChecked == true,
                IsKeto = chkKeto?.IsChecked == true,
                IsGlutenFree = chkGlutenFree?.IsChecked == true,
                IsLactoseFree = chkLactoseFree?.IsChecked == true,
                IsNutFree = chkNutFree?.IsChecked == true,
                IsFavoriteOnly = chkFavorites?.IsChecked == true
            };

            // Call the ViewModel - passing the filter and the UserId we added earlier
            var results = await ViewModel.SearchMealsAsync(filter, ViewModel.UserId);

            allMeals = results.ToList();
            currentPage = 1;
            LoadMeals();
        }*/

        private void LoadMeals()
        {
            if (allMeals == null) return;

            var paged = allMeals
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

          //  listMeals.ItemsSource = null;
           // listMeals.ItemsSource = paged;

            int totalPages = (int)Math.Max(1, Math.Ceiling((double)allMeals.Count / pageSize));
           // txtPage.Text = $"{currentPage} / {totalPages}";

           // btnPrev.IsEnabled = currentPage > 1;
           // btnNext.IsEnabled = currentPage < totalPages;
        }

      /*  private async void Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is FoodItemDto meal)
            {
                // Call the RelayCommand we fixed in the ViewModel
                await ViewModel.ToggleFavoriteCommand.ExecuteAsync(meal);

                // Update the button icon visually
                btn.Content = meal.IsFavorite ? "★" : "☆";

                // If we are in "Favorites Only" mode, remove it from the local list immediately
                if (chkFavorites?.IsChecked == true && !meal.IsFavorite)
                {
                    allMeals.Remove(meal);
                    LoadMeals();
                }
            }
        }*/

        private async void listMeals_ItemClick(object sender, ItemClickEventArgs e)
        {
            var meal = e.ClickedItem as FoodItemDto;
            if (meal == null) return;

            // Using the Service method we moved the formatting logic into
           /* var ingredientsText = await ViewModel.GetIngredientsTextAsync(meal.FoodItemId);*/

            var panel = new StackPanel { Spacing = 10 };


            panel.Children.Add(new TextBlock
            {
                Text = $"Calories: {meal.Calories}\nProtein: {meal.Protein}g\nCarbs: {meal.Carbohydrates}g\nFat: {meal.Fat}g\n\nIngredients:\n{string.Join("\n", "placeholder")}"
            });

            ContentDialog dialog = new ContentDialog
            {
                Title = meal.Name,
                Content = panel,
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void Favorite_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is FoodItemDto meal)
            {
                btn.Content = meal.IsFavorite ? "★" : "☆";
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; LoadMeals(); } }
        private void Next_Click(object sender, RoutedEventArgs e) { int totalPages = (int)Math.Ceiling((double)allMeals.Count / pageSize); if (currentPage < totalPages) { currentPage++; LoadMeals(); } }
        ///private void txtSearch_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e) { if (e.Key == Windows.System.VirtualKey.Enter) btnSearch_Click(this, new RoutedEventArgs()); }
    }
}